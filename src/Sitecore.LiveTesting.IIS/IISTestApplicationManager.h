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
          public:
            IISTestApplicationManager(IIS::HostedWebCore^ hostedWebCore, System::Web::Hosting::ApplicationManager^ applicationManager, System::Type^ testApplicationType);

            Sitecore::LiveTesting::Applications::TestApplication^ StartApplication(Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost) override;
            void StopApplication(Sitecore::LiveTesting::Applications::TestApplication^ application) override;
        };
      }
    }
  }
}