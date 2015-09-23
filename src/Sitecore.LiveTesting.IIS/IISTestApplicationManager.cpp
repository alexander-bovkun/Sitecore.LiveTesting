#include "IISTestApplicationManager.h"

Sitecore::LiveTesting::IIS::HostedWebCore^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::HostedWebCore::get()
{
  return m_hostedWebCore;
}

Sitecore::LiveTesting::Applications::TestApplicationHost^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetAdjustedApplicationHost(Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost)
{
  if (applicationHost == nullptr)
  {
    throw gcnew System::ArgumentNullException("applicationHost");
  }

  return gcnew Sitecore::LiveTesting::Applications::TestApplicationHost(applicationHost->ApplicationId, applicationHost->VirtualPath, applicationHost->PhysicalPath);
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

  applicationHost = GetAdjustedApplicationHost(applicationHost);

  Sitecore::LiveTesting::Applications::TestApplication^ application = Sitecore::LiveTesting::Applications::TestApplicationManager::StartApplication(applicationHost);

  CreateSiteForApplication(application);

  return application;
}

void Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::StopApplication(_In_ Sitecore::LiveTesting::Applications::TestApplication^ application)
{
  if (application == nullptr)
  {
    throw gcnew System::ArgumentNullException("application");
  }

  RemoveSiteForApplication(application);
  Sitecore::LiveTesting::Applications::TestApplicationManager::StopApplication(application);
}