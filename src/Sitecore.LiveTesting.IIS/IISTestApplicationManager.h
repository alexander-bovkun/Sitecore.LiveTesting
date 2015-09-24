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