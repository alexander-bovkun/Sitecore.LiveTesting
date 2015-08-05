#include "IISTestApplicationManager.h"

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(IIS::HostedWebCore^ hostedWebCore, System::Web::Hosting::ApplicationManager^ applicationManager, System::Type^ testApplicationType) : Sitecore::LiveTesting::Applications::TestApplicationManager(applicationManager, testApplicationType), m_hostedWebCore(hostedWebCore)
{
  if (m_hostedWebCore == nullptr)
  {
    throw gcnew System::ArgumentNullException("hostedWebCore");
  }
}

Sitecore::LiveTesting::Applications::TestApplication^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::StartApplication(Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost)
{
  if (applicationHost == nullptr)
  {
    throw gcnew System::ArgumentNullException("applicationHost");
  }

  return Sitecore::LiveTesting::Applications::TestApplicationManager::StartApplication(applicationHost);
}

void Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::StopApplication(Sitecore::LiveTesting::Applications::TestApplication^ application)
{
  Sitecore::LiveTesting::Applications::TestApplicationManager::StopApplication(application);
}