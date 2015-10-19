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

            System::String^ m_siteName;
            int m_port;
          protected:
            IISEnvironmentInfo(_In_ System::Runtime::Serialization::SerializationInfo^ info, _In_ System::Runtime::Serialization::StreamingContext context);
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

            virtual void GetObjectData(_In_ System::Runtime::Serialization::SerializationInfo^ info, _In_ System::Runtime::Serialization::StreamingContext context);
        };
      }
    }
  }
}