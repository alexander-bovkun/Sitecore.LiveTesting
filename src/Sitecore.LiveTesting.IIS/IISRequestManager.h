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
        public ref class IISRequestManager : public Sitecore::LiveTesting::Requests::RequestManager
        {
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