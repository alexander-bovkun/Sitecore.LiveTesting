#pragma once

#include "HostedWebCore.h"
#include "HostedWebCoreConfigProvider.h"
#include "IISEnvironmentInfo.h"

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      namespace Applications
      {
        [System::Runtime::InteropServices::ComVisible(false)]
        public ref class IISTestApplicationManager : public Sitecore::LiveTesting::Applications::TestApplicationManager, public System::IDisposable
        {
          private:
            literal System::String^ DEFAULT_HOSTED_WEB_CORE_INSTANCE_NAME = "Sitecore.LiveTesting";
            literal System::String^ APPLICATION_NAME_TEMPLATE = "/LM/W3SVC/{0}/ROOT{1}";
            literal System::String^ SITE_ROOT_XPATH = "/configuration/system.applicationHost/sites";
            literal System::String^ SITE_ELEMENT_NAME = "site";
            literal System::String^ SITE_SEARCH_TEMPLATE = "site[@name='{0}']";
            literal System::String^ SITE_NAME_ATTRIBUTE = "name";
            literal System::String^ SITE_APPLICATION_XPATH = "application";
            literal System::String^ SITE_APPLICATION_VIRTUAL_PATH_ATTRIBUTE_NAME = "path";
            literal System::String^ SITE_ID_ATTRIBUTE_NAME = "id";
            literal System::String^ SITE_VIRTUAL_DIRECTORY_ELEMENT_NAME = "virtualDirectory";
            literal System::String^ VIRTUAL_DIRECTORY_PHYSICAL_PATH_ATTRIBUTE_NAME = "physicalPath";
            literal System::String^ DEFAULT_SITE_APPLICATION_XML = "<application applicationPool='Sitecore.LiveTesting' path='{1}'>{0}<virtualDirectory path='/' physicalPath='{2}' />{0}</application>";
            literal System::String^ ROOT_VIRTUAL_PATH = "/";
            literal System::String^ SITE_BINDING_XPATH = "bindings/binding[@protocol='http']/@bindingInformation";

            initonly IIS::HostedWebCore^ m_hostedWebCore;

            bool m_disposed;
          protected:
            property IIS::HostedWebCore^ HostedWebCore
            {
              IIS::HostedWebCore^ get();
            }

            static IIS::HostedWebCore^ GetNewHostedWebCoreOrExistingIfAlreadyHosted(_In_ Configuration::HostedWebCoreConfigProvider^ hostedWebCoreConfigProvider);

            virtual System::Xml::Linq::XDocument^ LoadHostConfiguration();
            virtual System::Xml::Linq::XElement^ GetSiteConfigurationForApplication(_In_ System::Xml::Linq::XDocument^ hostConfiguration, _In_ Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost);
            virtual Sitecore::LiveTesting::Applications::TestApplicationHost^ AdjustApplicationHostToSiteConfiguration(_In_ System::Xml::Linq::XElement^ siteConfiguration);
            virtual IISEnvironmentInfo^ CreateApplicationIISEnvironmentInfo(_In_ Sitecore::LiveTesting::Applications::TestApplication^ application, _In_ System::Xml::Linq::XElement^ siteConfiguration);
            virtual void SaveHostConfiguration(_In_ System::Xml::Linq::XDocument^ hostConfiguration);
          
            IISTestApplicationManager(_In_ IIS::HostedWebCore^ hostedWebCore, _In_ System::Web::Hosting::ApplicationManager^ applicationManager, _In_ System::Type^ testApplicationType);
          public:
            [System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::ControlAppDomain)]
            IISTestApplicationManager(_In_ Configuration::HostedWebCoreConfigProvider^ hostedWebCoreConfigProvider, _In_ System::Type^ testApplicationType);
            
            [System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::ControlAppDomain)]
            IISTestApplicationManager();

            virtual Sitecore::LiveTesting::Applications::TestApplication^ StartApplication(_In_ Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost) override;
        
            virtual ~IISTestApplicationManager();
        };
      }
    }
  }
}