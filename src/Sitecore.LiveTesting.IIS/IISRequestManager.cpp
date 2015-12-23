#include "IISEnvironmentInfo.h"
#include "InitializationHandlerModule.h"

#include "IISRequestManager.h"

static Sitecore::LiveTesting::IIS::Requests::IISRequestManager::IISRequestManager()
{
  initializationContexts = gcnew System::Collections::Concurrent::ConcurrentDictionary<int, Sitecore::LiveTesting::Initialization::RequestInitializationContext^>();
  tokenCounter = 0;
}

void Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnBeginRequest(_In_ System::Object^ sender, _In_ System::EventArgs^)
{
  System::Web::HttpApplication^ application = safe_cast<System::Web::HttpApplication^>(sender);
  int token;

  if (int::TryParse(application->Request->Headers[SITECORE_LIVE_TESTING_TOKEN_KEY], token))
  {
    Sitecore::LiveTesting::Initialization::RequestInitializationContext^ requestInitializationContext = GetRequestInitializationContext(token);

    requestInitializationContext->HttpContext = application->Context;

    InitializationManager->Initialize(token, requestInitializationContext);
  }
}

void Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnEndRequest(_In_ System::Object^ sender, _In_ System::EventArgs^)
{
  System::Web::HttpApplication^ application = safe_cast<System::Web::HttpApplication^>(sender);
  int token;

  if (int::TryParse(application->Request->Headers[SITECORE_LIVE_TESTING_TOKEN_KEY], token))
  {
    Sitecore::LiveTesting::Initialization::RequestInitializationContext^ requestInitializationContext = GetRequestInitializationContext(token);

    InitializationManager->Cleanup(token, requestInitializationContext);
  }
}

Sitecore::LiveTesting::IIS::Requests::IISRequestManager::IISRequestManager(_In_ Sitecore::LiveTesting::Initialization::InitializationManager^ initializationManager)
{
  if (initializationManager == nullptr)
  {
    throw gcnew System::ArgumentNullException("initializationManager");
  }

  m_initializationManager = initializationManager;
}

int Sitecore::LiveTesting::IIS::Requests::IISRequestManager::AddRequestInitializationContext(_In_ Sitecore::LiveTesting::Initialization::RequestInitializationContext^ requestInitializationContext)
{
  if (requestInitializationContext == nullptr)
  {
    throw gcnew System::ArgumentNullException("requestInitializationContext");
  }

  int currentToken = System::Threading::Interlocked::Increment(tokenCounter);
  initializationContexts->Add(currentToken, requestInitializationContext);

  return currentToken;
}

Sitecore::LiveTesting::Initialization::RequestInitializationContext^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::GetRequestInitializationContext(_In_ int token)
{
  return initializationContexts[token];
}

void Sitecore::LiveTesting::IIS::Requests::IISRequestManager::RemoveRequestInitializationContext(_In_ int token)
{
  initializationContexts->Remove(token);
}

Sitecore::LiveTesting::Initialization::InitializationManager^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::InitializationManager::get()
{
  return m_initializationManager;
}

System::Net::HttpWebRequest^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::CreateHttpWebRequestFromRequestModel(_In_ Sitecore::LiveTesting::Requests::Request^ request)
{
  if (request == nullptr)
  {
    throw gcnew System::ArgumentNullException("request");
  }

  Sitecore::LiveTesting::IIS::Applications::IISEnvironmentInfo^ environmentInfo = Sitecore::LiveTesting::IIS::Applications::IISEnvironmentInfo::GetApplicationInfo(nullptr);

  if (environmentInfo == nullptr)
  {
    throw gcnew System::InvalidOperationException("Cannot execute request in environment which is not hosted. Consider using Sitecore.LiveTesting.IIS.Applications.IISTestApplicationManager.StartApplication to create corresponding application and execute the action on it's behalf.");
  }

  System::String^ normalizedQueryString = request->QueryString->StartsWith("?") ? request->QueryString : "?" + request->QueryString;
  System::String^ normalizedPath = request->Path->Replace('\\', '/');
  System::String^ url = (gcnew System::UriBuilder(DEFAULT_SCHEME, DEFAULT_HOST_NAME, environmentInfo->Port, normalizedPath, normalizedQueryString))->ToString();

  System::Net::HttpWebRequest^ result = safe_cast<System::Net::HttpWebRequest^>(System::Net::WebRequest::Create(url));

  result->AllowAutoRedirect = false;
  result->KeepAlive = false;

  result->Host = DEFAULT_HOST_NAME;
  result->Method = request->Verb;
  result->ProtocolVersion = System::Version::Parse(request->HttpVersion->Replace(HTTP_VERSION_PREFIX, System::String::Empty));

  if ((request->Verb != GET_VERB) && (request->Verb != HEAD_VERB))
  {
    System::IO::StreamWriter^ streamWriter = gcnew System::IO::StreamWriter(result->GetRequestStream());

    try
    {
      streamWriter->Write(request->Data);
    }
    finally
    {
      streamWriter->~StreamWriter();
    }
  }

  for each (System::Collections::Generic::KeyValuePair<System::String^, System::String^>^ header in request->Headers)
  {
    result->Headers->Add(header->Key, header->Value);
  }

  return result;
}

void Sitecore::LiveTesting::IIS::Requests::IISRequestManager::MapResponseModelFromHttpWebResponse(_In_ Sitecore::LiveTesting::Requests::Response^ response, _In_ System::Net::HttpWebResponse^ httpWebReponse)
{
  if (response == nullptr)
  {
    throw gcnew System::ArgumentNullException("response");
  }

  if (httpWebReponse == nullptr)
  {
    throw gcnew System::ArgumentNullException("httpWebReponse");
  }

  if (System::Object::ReferenceEquals(response->Content, UNDEFINED_STRING))
  {
    System::IO::StreamReader^ responseStreamReader = gcnew System::IO::StreamReader(httpWebReponse->GetResponseStream());

    try
    {
      response->Content = responseStreamReader->ReadToEnd();
    }
    finally
    {
      responseStreamReader->~StreamReader();
    }
  }

  for each (System::String^ headerKey in httpWebReponse->Headers->AllKeys)
  {
    if (!response->Headers->ContainsKey(headerKey))
    {
      response->Headers->Add(headerKey, System::String::Join(HEADER_VALUE_SEPARATOR, httpWebReponse->Headers->GetValues(headerKey)));
    }
  }

  if (response->StatusCode == UNDEFINED_INTEGER)
  {
    response->StatusCode = System::Convert::ToInt32(httpWebReponse->StatusCode);
  }

  if (System::Object::ReferenceEquals(response->StatusDescription, UNDEFINED_STRING))
  {
    response->StatusDescription = httpWebReponse->StatusDescription;
  }
}

void Sitecore::LiveTesting::IIS::Requests::IISRequestManager::MapResponseModelFromWebException(_In_ Sitecore::LiveTesting::Requests::Response^ response, _In_ System::Net::WebException^ exception)
{
  if (response == nullptr)
  {
    throw gcnew System::ArgumentNullException("response");
  }

  if (exception == nullptr)
  {
    throw gcnew System::ArgumentNullException("exception");
  }

  System::Net::HttpWebResponse^ httpWebReponse = safe_cast<System::Net::HttpWebResponse^>(exception->Response);

  MapResponseModelFromHttpWebResponse(response, httpWebReponse);
}

Sitecore::LiveTesting::IIS::Requests::IISRequestManager::IISRequestManager()
{
  m_initializationManager = gcnew Sitecore::LiveTesting::Initialization::InitializationManager(gcnew Sitecore::LiveTesting::Initialization::RequestInitializationActionDiscoverer(), gcnew Sitecore::LiveTesting::Initialization::InitializationActionExecutor());
}

Sitecore::LiveTesting::Requests::Response^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::ExecuteRequest(_In_ Sitecore::LiveTesting::Requests::Request^ request)
{
  System::Net::HttpWebRequest^ httpWebRequest = CreateHttpWebRequestFromRequestModel(request);
  Sitecore::LiveTesting::Requests::Response^ result = gcnew Sitecore::LiveTesting::Requests::Response();

  result->Content = UNDEFINED_STRING;
  result->StatusCode = UNDEFINED_INTEGER;
  result->StatusDescription = UNDEFINED_STRING;

  int token = AddRequestInitializationContext(gcnew Sitecore::LiveTesting::Initialization::RequestInitializationContext(request, result));

  try
  {
    httpWebRequest->Headers->Add(SITECORE_LIVE_TESTING_TOKEN_KEY, token.ToString(System::Globalization::CultureInfo::InvariantCulture));

    InitializationHandlerModule::BeginRequest += gcnew System::EventHandler(this, &Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnBeginRequest);
    InitializationHandlerModule::EndRequest += gcnew System::EventHandler(this, &Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnEndRequest);

    try
    {
      System::Net::HttpWebResponse^ httpWebResponse = safe_cast<System::Net::HttpWebResponse^>(httpWebRequest->GetResponse());

      try
      {
        MapResponseModelFromHttpWebResponse(result, httpWebResponse);
      }
      finally
      {
        httpWebResponse->~HttpWebResponse();
      }
    }
    catch (System::Net::WebException^ exception)
    {
      MapResponseModelFromWebException(result, exception);
    }
    finally
    {
      InitializationHandlerModule::EndRequest -= gcnew System::EventHandler(this, &Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnEndRequest);
      InitializationHandlerModule::BeginRequest -= gcnew System::EventHandler(this, &Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnBeginRequest);
    }
  }
  finally
  {
    RemoveRequestInitializationContext(token);
  }

  return result;
}