#include "IISRequestManager.h"

System::Net::HttpWebRequest^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::CreateHttpWebRequestFromRequestModel(_In_ Sitecore::LiveTesting::Requests::Request^ request)
{
  if (request == nullptr)
  {
    throw gcnew System::ArgumentNullException("request");
  }

  System::Net::HttpWebRequest^ result = safe_cast<System::Net::HttpWebRequest^>(System::Net::WebRequest::Create(request->Path));

  return result;
}

Sitecore::LiveTesting::Requests::Response^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::CreateResponseModelFromHttpWebResponse(_In_ System::Net::HttpWebResponse^ httpWebReponse)
{
  if (httpWebReponse == nullptr)
  {
    throw gcnew System::ArgumentNullException("httpWebReponse");
  }

  Sitecore::LiveTesting::Requests::Response^ result = gcnew Sitecore::LiveTesting::Requests::Response();

  result->StatusCode = System::Convert::ToInt32(httpWebReponse->StatusCode);

  return result;
}

Sitecore::LiveTesting::Requests::Response^ Sitecore::LiveTesting::IIS::Requests::IISRequestManager::CreateResponseModelFromWebException(_In_ System::Net::WebException^ exception)
{
  if (exception == nullptr)
  {
    throw gcnew System::ArgumentNullException("exception");
  }

  return nullptr;
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