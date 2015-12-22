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
        public ref class InitializationHandlerModule sealed : public System::Web::IHttpModule
        {
          private:
            System::Web::HttpApplication^ m_application;

            void OnBeginRequest(_In_ System::Object^ sender, _In_ System::EventArgs^ args);
            void OnEndRequest(_In_ System::Object^ sender, _In_ System::EventArgs^ args);
          public:
            static event System::EventHandler^ BeginRequest;
            static event System::EventHandler^ EndRequest;

            virtual void Init(_In_ System::Web::HttpApplication^ application);

            ~InitializationHandlerModule();
        };
      }
    }
  }
}