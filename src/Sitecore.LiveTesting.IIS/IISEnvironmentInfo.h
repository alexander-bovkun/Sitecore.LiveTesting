#pragma once

#include <sal.h>

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      namespace Applications
      {
        [System::Serializable]
        public ref class IISEnvironmentInfo : public System::Runtime::Serialization::ISerializable
        {
          private:
            literal System::String^ SITE_NAME_SERIALIZATION_KEY = "siteName";
            literal System::String^ PORT_SERIALIZATION_KEY = "port";

            static IISEnvironmentInfo^ EnvironmentInfo;

            System::String^ m_siteName;
            int m_port;
          protected:
            IISEnvironmentInfo(_In_ System::Runtime::Serialization::SerializationInfo^ info, _In_ System::Runtime::Serialization::StreamingContext context);
          internal:
            static void SetApplicationInfo(_In_ Applications::IISEnvironmentInfo^ iisEnvironmentInfo);
          public:
            IISEnvironmentInfo(_In_ System::String^ siteName, _In_ int port);

            property System::String^ SiteName
            {
              System::String^ get();
            }

            property int Port
            {
              int get();
            }

            static Applications::IISEnvironmentInfo^ GetApplicationInfo(_In_ Sitecore::LiveTesting::Applications::TestApplication^ application);

            [System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::SerializationFormatter)]
            virtual void GetObjectData(_In_ System::Runtime::Serialization::SerializationInfo^ info, _In_ System::Runtime::Serialization::StreamingContext context);
        };
      }
    }
  }
}