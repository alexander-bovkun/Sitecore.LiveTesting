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
            literal System::String^ SITECORE_LIVE_TESTING_TOKEN_KEY = "Sitecore.LiveTesting.Token";
            literal System::String^ UNDEFINED_STRING = "";
            literal int UNDEFINED_INTEGER = int::MinValue;

            static initonly System::Collections::Generic::IDictionary<int, Sitecore::LiveTesting::Initialization::RequestInitializationContext^>^ initializationContexts;
            
            static int tokenCounter;

            initonly Sitecore::LiveTesting::Initialization::InitializationManager^ m_initializationManager;

            static IISRequestManager();

            void OnBeginRequest(_In_ System::Object^ sender, _In_ System::EventArgs^ args);
            void OnEndRequest(_In_ System::Object^ sender, _In_ System::EventArgs^ args);
          protected:
            IISRequestManager(_In_ Sitecore::LiveTesting::Initialization::InitializationManager^ initializationManager);

            static int AddRequestInitializationContext(_In_ Sitecore::LiveTesting::Initialization::RequestInitializationContext^ requestInitializationContext);
            static Sitecore::LiveTesting::Initialization::RequestInitializationContext^ GetRequestInitializationContext(_In_ int token);
            static void RemoveRequestInitializationContext(_In_ int token);

            property Sitecore::LiveTesting::Initialization::InitializationManager^ InitializationManager
            {
              Sitecore::LiveTesting::Initialization::InitializationManager^ get();
            }

            virtual System::Net::HttpWebRequest^ CreateHttpWebRequestFromRequestModel(_In_ Sitecore::LiveTesting::Requests::Request^ request);
            virtual void MapResponseModelFromHttpWebResponse(_In_ Sitecore::LiveTesting::Requests::Response^ response, _In_ System::Net::HttpWebResponse^ httpWebReponse);
            virtual void MapResponseModelFromWebException(_In_ Sitecore::LiveTesting::Requests::Response^ response, _In_ System::Net::WebException^ exception);
          public:
            IISRequestManager();

            virtual Sitecore::LiveTesting::Requests::Response^ ExecuteRequest(_In_ Sitecore::LiveTesting::Requests::Request^ request) override;
        };
      }
    }
  }
}