#pragma once

#include <sal.h>

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      namespace Configuration
      {
        public ref class HostedWebCoreConfigProvider
        {
          private:
            literal System::String^ IIS_BIN_ENVIRONMENT_VARIABLE_TOKEN = "%IIS_BIN%";
            literal System::String^ SITE_ROOT_XPATH = "/configuration/system.applicationHost/sites";
            literal System::String^ APP_POOL_ROOT_XPATH = "/configuration/system.applicationHost/applicationPools";
            literal System::String^ SITE_ELEMENT_NAME = "site";
            literal System::String^ COLLECTION_ADD = "add";
            literal System::String^ COLLECTION_REMOVE = "remove";
            literal System::String^ COLLECTION_CLEAR = "clear";
            literal System::String^ DEFAULT_APP_POOL_XML = "<add name='Sitecore.LiveTesting' managedRuntimeVersion='v4.0' managedPipelineMode='Integrated' />";
            literal System::String^ DEFAULT_SITE_XML = "<site name='{1}' id='{1}' serverAutoStart='true'>{0}<bindings>{0}<binding protocol='http' bindingInformation='*:{2}:localhost' />{0}</bindings>{0}<application applicationPool='Sitecore.LiveTesting' path='/'>{0}<virtualDirectory path='/' physicalPath='' />{0}</application>{0}</site>";
            literal System::String^ DEFAULT_HOST_CONFIG_FILE_NAME = "Sitecore.LiveTesting.IIS.ApplicationHost.config";

            static System::String^ GetDefaultHostConfigFileName();
            static System::String^ GetDefaultRootConfigFileName();
            static int GetFreePort();

            initonly System::String^ m_originalHostConfig;
            initonly System::String^ m_originalRootConfig;
          protected:
            property System::String^ OriginalHostConfig
            {
              System::String^ get();
            }

            property System::String^ OriginalRootConfig
            {
              System::String^ get();
            }
          public:
            HostedWebCoreConfigProvider();
            HostedWebCoreConfigProvider(_In_ System::String^ originalHostConfig, _In_ System::String^ originalRootConfig);
            
            virtual System::String^ GetProcessedHostConfig();
            virtual System::String^ GetProcessedRootConfig();
        };
      }
    }
  }
}