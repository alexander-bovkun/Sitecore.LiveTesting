#pragma once

#include "NativeHostedWebCore.h"

#include <msclr\marshal_cppstd.h>

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      public ref class HostedWebCore sealed : System::IDisposable
      {
        private:
          msclr::interop::marshal_context^ m_marshalContext;
          NativeHostedWebCore* m_pHostedWebCore;
        protected:
          !HostedWebCore();
        public:
          HostedWebCore(System::String^ hostedWebCoreLibraryPath, System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName);
          HostedWebCore(System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName);

          void Stop(System::Boolean immediate);

          ~HostedWebCore();
      };
    }
  }
}