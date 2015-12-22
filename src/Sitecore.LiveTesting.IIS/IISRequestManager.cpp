#include "IISEnvironmentInfo.h"
#include "InitializationHandlerModule.h"

#include "IISRequestManager.h"

void Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnBeginRequest(_In_ System::Object^ sender, _In_ System::EventArgs^ args)
{
  args;

  System::Web::HttpApplication^ application = safe_cast<System::Web::HttpApplication^>(sender);

  if (application->Request->Headers[SITECORE_LIVE_TESTING_TOKEN_KEY] == m_currentToken.ToString(System::Globalization::CultureInfo::InvariantCulture))
  {
    InitializationManager->Initialize(m_currentToken, m_initializationContext);
  }
}

void Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnEndRequest(_In_ System::Object^ sender, _In_ System::EventArgs^ args)
{
  args;

  System::Web::HttpApplication^ application = safe_cast<System::Web::HttpApplication^>(sender);

  if (application->Request->Headers[SITECORE_LIVE_TESTING_TOKEN_KEY] == m_currentToken.ToString(System::Globalization::CultureInfo::InvariantCulture))
  {
    InitializationManager->Cleanup(m_currentToken, m_initializationContext);
  }
}

Sitecore::LiveTesting::IIS::Requests::IISRequestManager::IISRequestManager(_In_ Sitecore::LiveTesting::Initialization::InitializationManager^ initializationManager) : Sitecore::LiveTesting::Requests::RequestManager(initializationManager), m_requestLock(gcnew System::Object())
{
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

  System::IO::StreamReader^ responseStreamReader = gcnew System::IO::StreamReader(httpWebReponse->GetResponseStream());

  try
  {
    response->Content = responseStreamReader->ReadToEnd();
  }
  finally
  {
    responseStreamReader->~StreamReader();
  }

  for each (System::String^ headerKey in httpWebReponse->Headers->AllKeys)
  {
    response->Headers->Add(headerKey, System::String::Join(HEADER_VALUE_SEPARATOR, httpWebReponse->Headers->GetValues(headerKey)));
  }

  response->StatusCode = System::Convert::ToInt32(httpWebReponse->StatusCode);
  response->StatusDescription = httpWebReponse->StatusDescription;
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

Sitecore::LiveTesting::IIS::Requests::IISRequestManager::IISRequestManager() : m_requestLock(gcnew System::Object())
{
}

Sitecore::LiveTesting::Requests::Response^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::ExecuteRequest(_In_ Sitecore::LiveTesting::Requests::Request^ request)
{
  System::Net::HttpWebRequest^ httpWebRequest = CreateHttpWebRequestFromRequestModel(request);
  Sitecore::LiveTesting::Requests::Response^ response = gcnew Sitecore::LiveTesting::Requests::Response();

  System::Threading::Monitor::Enter(m_requestLock);

  try
  {
    m_currentToken = tokenCounter++;
    m_initializationContext = gcnew Sitecore::LiveTesting::Initialization::RequestInitializationContext(request, response);

    InitializationHandlerModule::BeginRequest += gcnew System::EventHandler(this, &Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnBeginRequest);
    InitializationHandlerModule::EndRequest += gcnew System::EventHandler(this, &Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnEndRequest);

    httpWebRequest->Headers->Add(SITECORE_LIVE_TESTING_TOKEN_KEY, m_currentToken.ToString(System::Globalization::CultureInfo::InvariantCulture));

    try
    {
      System::Net::HttpWebResponse^ httpWebResponse = safe_cast<System::Net::HttpWebResponse^>(httpWebRequest->GetResponse());

      try
      {
        MapResponseModelFromHttpWebResponse(response, httpWebResponse);
      }
      finally
      {
        httpWebResponse->~HttpWebResponse();
      }
    }
    catch (System::Net::WebException^ exception)
    {
      MapResponseModelFromWebException(response, exception);
    }
    finally
    {
      InitializationHandlerModule::EndRequest -= gcnew System::EventHandler(this, &Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnEndRequest);
      InitializationHandlerModule::BeginRequest -= gcnew System::EventHandler(this, &Sitecore::LiveTesting::IIS::Requests::IISRequestManager::OnBeginRequest);
    }

    return response;
  }
  finally
  {
    System::Threading::Monitor::Exit(m_requestLock);
  }
}