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
          public:
            virtual void Init(_In_ System::Web::HttpApplication^ application);

            ~InitializationHandlerModule();
        };
      }
    }
  }
}