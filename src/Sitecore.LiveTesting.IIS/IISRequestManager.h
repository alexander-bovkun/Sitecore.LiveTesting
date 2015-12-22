#pragma once

#include <sal.h>

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      namespace Requests
      {
        [System::Runtime::InteropServices::ComVisible(false)]
        public ref class IISRequestManager : public Sitecore::LiveTesting::Requests::RequestManager
        {
          private:
            literal System::String^ DEFAULT_SCHEME = "http";
            literal System::String^ DEFAULT_HOST_NAME = "localhost";
            literal System::String^ GET_VERB = "GET";
            literal System::String^ HEAD_VERB = "HEAD";
            literal System::String^ HTTP_VERSION_PREFIX = "HTTP/";
            literal System::String^ HEADER_VALUE_SEPARATOR = ",";
          protected:
            virtual System::Net::HttpWebRequest^ CreateHttpWebRequestFromRequestModel(_In_ Sitecore::LiveTesting::Requests::Request^ request);
            virtual Sitecore::LiveTesting::Requests::Response^ CreateResponseModelFromHttpWebResponse(_In_ System::Net::HttpWebResponse^ httpWebReponse);
            virtual Sitecore::LiveTesting::Requests::Response^ CreateResponseModelFromWebException(_In_ System::Net::WebException^ exception);
          public:
            virtual Sitecore::LiveTesting::Requests::Response^ ExecuteRequest(_In_ Sitecore::LiveTesting::Requests::Request^ request) override;
        };
      }
    }
  }
}