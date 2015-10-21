#pragma once

#include <sal.h>

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      [System::Serializable]
      public ref class HostedWebCoreSetup sealed : public System::Runtime::Serialization::ISerializable
      {
        private:
          literal System::String^ HOSTED_WEB_CORE_LIBRARY_PATH_SERIALIZATION_KEY = "hostedWebCoreLibraryPath";
          literal System::String^ HOST_CONFIG_SERIALIZATION_KEY = "hostConfig";
          literal System::String^ ROOT_CONFIG_SERIALIZATION_KEY = "rootConfig";
          literal System::String^ INSTANCE_NAME_SERIALIZATION_KEY = "instanceName";

          initonly System::String^ m_hostedWebCoreLibraryPath;
          initonly System::String^ m_hostConfig;
          initonly System::String^ m_rootConfig;
          initonly System::String^ m_instanceName;
        
          HostedWebCoreSetup(_In_ System::Runtime::Serialization::SerializationInfo^ info, _In_ System::Runtime::Serialization::StreamingContext context);
          virtual void GetObjectData(_In_ System::Runtime::Serialization::SerializationInfo^ info, _In_ System::Runtime::Serialization::StreamingContext context) sealed = System::Runtime::Serialization::ISerializable::GetObjectData;
        public:
          HostedWebCoreSetup(_In_ System::String^ hostedWebCoreLibraryPath, _In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName);

          property System::String^ HostedWebCoreLibraryPath
          {
            System::String^ get();
          }

          property System::String^ HostConfig
          {
            System::String^ get();
          }

          property System::String^ RootConfig
          {
            System::String^ get();
          }

          property System::String^ InstanceName
          {
            System::String^ get();
          }
      };
    }
  }
}