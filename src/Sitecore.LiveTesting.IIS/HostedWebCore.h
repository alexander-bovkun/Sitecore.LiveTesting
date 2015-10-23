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

          System::AppDomain^ GetHostAppDomain();

          [System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::ControlAppDomain)]
          void RegisterExternalAssembly(_In_ System::AppDomain^ appDomain, _In_ System::String^ assemblyName, _In_ System::String^ assemblyPath);

          !HostedWebCore();

          ref class HostAppDomainUtility : public System::MarshalByRefObject
          {
            private:
              static initonly System::Collections::Generic::IDictionary<System::String^, System::String^>^ externalAssemblies;

              static HostAppDomainUtility();

              System::Reflection::Assembly^ AssemblyResolve(_In_ System::Object^ sender, _In_ System::ResolveEventArgs^ args);
            internal:
              [System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::ControlAppDomain)]
              void RegisterExternalAssembly(_In_ System::String^ assemblyName, _In_ System::String^ assemblyPath);
              
              System::Web::Hosting::ApplicationManager^ GetApplicationManager();
          };
        public:
          HostedWebCore(_In_ HostedWebCoreSetup^ hostedWebCoreSetup);
          HostedWebCore(_In_ System::String^ hostedWebCoreLibraryPath, _In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName);
          HostedWebCore(_In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName);

          static property HostedWebCoreSetup^ CurrentHostedWebCoreSetup
          {
            HostedWebCoreSetup^ get();
          }

          [System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::ControlAppDomain)]
          System::Web::Hosting::ApplicationManager^ GetHostApplicationManager();

          ~HostedWebCore();
      };
    }
  }
}