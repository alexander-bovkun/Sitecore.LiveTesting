#pragma once

#include "HostedWebCore.h"

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      namespace Applications
      {
        public ref class IISTestApplicationManager : public Sitecore::LiveTesting::Applications::TestApplicationManager
        {
          private:
            literal System::String^ APPLICATION_NAME_TEMPLATE = "/LM/W3SVC/{0}/ROOT";
            literal System::String^ SITE_ROOT_XPATH = "/configuration/system.applicationHost/sites";
            literal System::String^ SITE_SEARCH_TEMPLATE = "site[@name='{0}']";
            literal System::String^ NEW_SITE_TEMPLATE = "<site name='{1}' id='{2}'>{0}<bindings>{0}<binding protocol='http' bindingInformation='*:{3}:localhost' />{0}</bindings>{0}<application applicationPool='{4}' path='{5}'>{0}<virtualDirectory path='{5}' physicalPath='{6}' />{0}</application>{0}</site>";
            literal System::String^ SINGLE_APP_POOL_XPATH = "/configuration/system.applicationHost/applicationPools/add[last()]/@name";

            initonly IIS::HostedWebCore^ m_hostedWebCore;

            static int GetFreePort();
          protected:
            property IIS::HostedWebCore^ HostedWebCore
            {
              IIS::HostedWebCore^ get();
            }

            virtual Sitecore::LiveTesting::Applications::TestApplicationHost^ GetAdjustedApplicationHost(Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost);

            virtual void CreateSiteForApplicationHost(Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost);
            virtual void RemoveSiteForApplication(Sitecore::LiveTesting::Applications::TestApplication^ application);
          public:
            IISTestApplicationManager(_In_ IIS::HostedWebCore^ hostedWebCore, _In_ System::Web::Hosting::ApplicationManager^ applicationManager, _In_ System::Type^ testApplicationType);

            Sitecore::LiveTesting::Applications::TestApplication^ StartApplication(_In_ Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost) override;
            void StopApplication(_In_ Sitecore::LiveTesting::Applications::TestApplication^ application) override;
        };
      }
    }
  }
}