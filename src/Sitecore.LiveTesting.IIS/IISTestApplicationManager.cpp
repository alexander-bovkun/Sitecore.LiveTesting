#include "IISTestApplicationManager.h"

Sitecore::LiveTesting::IIS::HostedWebCore^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::HostedWebCore::get()
{
  return m_hostedWebCore;
}

void Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::CreateSiteForApplication(Sitecore::LiveTesting::Applications::TestApplication^ application)
{
  application;
}

void Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::RemoveSiteForApplication(Sitecore::LiveTesting::Applications::TestApplication^ application)
{
  application;
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(_In_ IIS::HostedWebCore^ hostedWebCore, _In_ System::Web::Hosting::ApplicationManager^ applicationManager, _In_ System::Type^ testApplicationType) : Sitecore::LiveTesting::Applications::TestApplicationManager(applicationManager, testApplicationType), m_hostedWebCore(hostedWebCore)
{
  if (m_hostedWebCore == nullptr)
  {
    throw gcnew System::ArgumentNullException("hostedWebCore");
  }
}

Sitecore::LiveTesting::Applications::TestApplication^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::StartApplication(_In_ Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost)
{
  if (applicationHost == nullptr)
  {
    throw gcnew System::ArgumentNullException("applicationHost");
  }

  Sitecore::LiveTesting::Applications::TestApplication^ result = Sitecore::LiveTesting::Applications::TestApplicationManager::StartApplication(applicationHost);

  CreateSiteForApplication(result);

  return result;
}

void Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::StopApplication(_In_ Sitecore::LiveTesting::Applications::TestApplication^ application)
{
  RemoveSiteForApplication(application);
  Sitecore::LiveTesting::Applications::TestApplicationManager::StopApplication(application);
}