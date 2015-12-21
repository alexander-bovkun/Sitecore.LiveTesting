#include "IISEnvironmentInfo.h"
#include "IISRequestManager.h"

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

  System::String^ url = System::String::Format(BASE_URL_TEMPLATE, environmentInfo->Port);

  if (!System::String::IsNullOrEmpty(request->Path))
  {
    System::String^ path = request->Path->Replace('\\', '/');

    if (!path->StartsWith("/"))
    {
      path = '/' + path;
    }

    url += path;
  }

  System::Net::HttpWebRequest^ result = safe_cast<System::Net::HttpWebRequest^>(System::Net::WebRequest::Create(url));
  
  result->AllowAutoRedirect = false;
  result->KeepAlive = false;

  result->Host = HOST_NAME;
  result->Method = request->Verb;
  result->ProtocolVersion = System::Version::Parse(request->HttpVersion->Replace(HTTP_VERSION_PREFIX, System::String::Empty));
  
  for each (System::Collections::Generic::KeyValuePair<System::String^, System::String^>^ header in request->Headers)
  {
    result->Headers->Add(header->Key, header->Value);
  }

  return result;
}

Sitecore::LiveTesting::Requests::Response^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::CreateResponseModelFromHttpWebResponse(_In_ System::Net::HttpWebResponse^ httpWebReponse)
{
  if (httpWebReponse == nullptr)
  {
    throw gcnew System::ArgumentNullException("httpWebReponse");
  }

  Sitecore::LiveTesting::Requests::Response^ result = gcnew Sitecore::LiveTesting::Requests::Response();
  System::IO::StreamReader^ responseStreamReader = gcnew System::IO::StreamReader(httpWebReponse->GetResponseStream());

  try
  {
    result->Content = responseStreamReader->ReadToEnd();
  }
  finally
  {
    responseStreamReader->~StreamReader();
  }

  for each (System::String^ headerKey in httpWebReponse->Headers->AllKeys)
  {
    result->Headers->Add(headerKey, System::String::Join(HEADER_VALUE_SEPARATOR, httpWebReponse->Headers->GetValues(headerKey)));
  }

  result->StatusCode = System::Convert::ToInt32(httpWebReponse->StatusCode);
  result->StatusDescription = httpWebReponse->StatusDescription;

  return result;
}

Sitecore::LiveTesting::Requests::Response^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::CreateResponseModelFromWebException(_In_ System::Net::WebException^ exception)
{
  if (exception == nullptr)
  {
    throw gcnew System::ArgumentNullException("exception");
  }

  System::Net::HttpWebResponse^ httpWebReponse = safe_cast<System::Net::HttpWebResponse^>(exception->Response);

  return CreateResponseModelFromHttpWebResponse(httpWebReponse);
}

Sitecore::LiveTesting::Requests::Response^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::ExecuteRequest(_In_ Sitecore::LiveTesting::Requests::Request^ request)
{
  System::Net::HttpWebRequest^ httpWebRequest = CreateHttpWebRequestFromRequestModel(request);
  try
  {
    System::Net::HttpWebResponse^ httpWebResponse = safe_cast<System::Net::HttpWebResponse^>(httpWebRequest->GetResponse());

    try
    {
      return CreateResponseModelFromHttpWebResponse(httpWebResponse);
    }
    finally
    {
      httpWebResponse->~HttpWebResponse();
    }
  }
  catch (System::Net::WebException^ exception)
  {
    return CreateResponseModelFromWebException(exception);
  }
}