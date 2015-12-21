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
          literal System::String^ PROCESS_HOST_FIELD_NAME = "_theProcessHost";
          literal System::String^ CLONE_APP_DOMAINS_COLLECTION_METHOD_NAME = "CloneAppDomainsCollection";
          literal System::String^ LOCKABLE_APP_DOMAIN_CONTEXT_TYPE_NAME = "System.Web.Hosting.LockableAppDomainContext";
          literal System::String^ HOST_ENV_PROPERTY_NAME = "HostEnv";
          literal System::String^ MANAGED_V2_NATIVE_MODULE_NAME = "webengine.dll";
          literal System::String^ MANAGED_V4_NATIVE_MODULE_NAME = "webengine4.dll";

          std::shared_ptr<NativeHostedWebCore>* m_pHostedWebCore;

          [System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::ControlAppDomain)]
          static void ReloadModule(_In_ System::Diagnostics::ProcessModule^ module);

          [System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::ControlAppDomain)]
          void CreateHostedWebCore(_In_ HostedWebCoreSetup^ hostedWebCoreSetup);

          System::AppDomain^ GetHostAppDomain();

          [System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::ControlAppDomain)]
          void RegisterExternalAssembly(_In_ System::AppDomain^ appDomain, _In_ System::String^ assemblyName, _In_ System::String^ assemblyPath);

          void ResetManagedEnvironment(_In_ System::AppDomain^ appDomain);

          !HostedWebCore();

          ref class HostAppDomainUtility sealed : public System::MarshalByRefObject
          {
            private:
              static initonly System::Collections::Generic::IDictionary<System::String^, System::String^>^ externalAssemblies;

              static HostAppDomainUtility();

              System::Reflection::Assembly^ AssemblyResolve(_In_ System::Object^ sender, _In_ System::ResolveEventArgs^ args);
            internal:
              [System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::ControlAppDomain)]
              void RegisterExternalAssembly(_In_ System::String^ assemblyName, _In_ System::String^ assemblyPath);

              void ResetManagedEnvironment();
              
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