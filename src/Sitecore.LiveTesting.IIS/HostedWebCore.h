#pragma once

#include "HostedWebCoreSetup.h"
#include "NativeHostedWebCore.h"

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      public ref class HostedWebCore sealed : public System::IDisposable
      {
        private:
          std::shared_ptr<NativeHostedWebCore>* m_pHostedWebCore;

          void CreateHostedWebCore(_In_ HostedWebCoreSetup^ hostedWebCoreSetup);

          !HostedWebCore();
        public:
          HostedWebCore(_In_ HostedWebCoreSetup^ hostedWebCoreSetup);
          HostedWebCore(_In_ System::String^ hostedWebCoreLibraryPath, _In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName);
          HostedWebCore(_In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName);

          static property HostedWebCoreSetup^ CurrentHostedWebCoreSetup
          {
            HostedWebCoreSetup^ get();
          }
          
          System::AppDomain^ GetHostAppDomain();

          ~HostedWebCore();
      };
    }
  }
}