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
          std::shared_ptr<NativeHostedWebCore>* m_pHostedWebCore;
        protected:
          !HostedWebCore();
        public:
          static property System::String^ CurrentHostedWebCoreLibraryPath
          {
            System::String^ get();
          }

          static property System::String^ CurrentHostConfig
          {
            System::String^ get();
          }

          static property System::String^ CurrentRootConfig
          {
            System::String^ get();
          }

          static property System::String^ CurrentInstanceName
          {
            System::String^ get();
          }

          HostedWebCore(_In_ System::String^ hostedWebCoreLibraryPath, _In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName);
          HostedWebCore(_In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName);

          ~HostedWebCore();
      };
    }
  }
}