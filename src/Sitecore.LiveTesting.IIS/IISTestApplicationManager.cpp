#include "IISTestApplicationManager.h"

int Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetFreePort()
{
  System::Net::Sockets::TcpListener^ listener = gcnew System::Net::Sockets::TcpListener(System::Net::IPAddress::Loopback, 0);
  listener->Start();
  try
  {
    return ((System::Net::IPEndPoint^)listener->LocalEndpoint)->Port;
  }
  finally
  {
    listener->Stop();
  }
}

Sitecore::LiveTesting::IIS::HostedWebCore^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::HostedWebCore::get()
{
  return m_hostedWebCore;
}

Sitecore::LiveTesting::IIS::HostedWebCore^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetDefaultHostedWebCore()
{
  if (System::String::IsNullOrEmpty(Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostedWebCoreLibraryPath))
  {
    System::String^ rootConfig = System::IO::Path::Combine(System::IO::Path::GetDirectoryName(System::Configuration::ConfigurationManager::OpenMachineConfiguration()->FilePath), "web.config");
    
    return gcnew Sitecore::LiveTesting::IIS::HostedWebCore(System::IO::Path::GetFullPath("applicationHostWithExpandedVariables.config"), rootConfig, "Sitecore.LiveTesting");
  }
  else
  {
    return gcnew Sitecore::LiveTesting::IIS::HostedWebCore(Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostedWebCoreLibraryPath, Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostConfig, Sitecore::LiveTesting::IIS::HostedWebCore::CurrentRootConfig, Sitecore::LiveTesting::IIS::HostedWebCore::CurrentInstanceName);
  }
}

System::Web::Hosting::ApplicationManager^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetApplicationManagerFromDefaultAppDomain()
{
  return System::Web::Hosting::ApplicationManager::GetApplicationManager();
}

Sitecore::LiveTesting::Applications::TestApplicationHost^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetAdjustedApplicationHost(Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost)
{
  if (applicationHost == nullptr)
  {
    throw gcnew System::ArgumentNullException("applicationHost");
  }

  return gcnew Sitecore::LiveTesting::Applications::TestApplicationHost(System::String::Format(APPLICATION_NAME_TEMPLATE, applicationHost->ApplicationId), applicationHost->VirtualPath, applicationHost->PhysicalPath);
}

void Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::CreateSiteForApplicationHost(Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost)
{
  static int applicationCounter;

  if (applicationHost == nullptr)
  {
    throw gcnew System::ArgumentNullException("applicationHost");
  }

  System::Xml::Linq::XDocument^ configuration = System::Xml::Linq::XDocument::Load(IIS::HostedWebCore::CurrentHostConfig);
  System::Xml::Linq::XElement^ sites = System::Xml::XPath::Extensions::XPathSelectElement(configuration, SITE_ROOT_XPATH);
  System::Xml::Linq::XElement^ site = System::Xml::XPath::Extensions::XPathSelectElement(sites, System::String::Format(SITE_SEARCH_TEMPLATE, applicationHost->ApplicationId));

  if (site == nullptr)
  {
    System::String^ applicationPoolName = System::Linq::Enumerable::Single(System::Linq::Enumerable::Cast<System::Xml::Linq::XAttribute^>((System::Collections::IEnumerable^)System::Xml::XPath::Extensions::XPathEvaluate(configuration, SINGLE_APP_POOL_XPATH)))->Value;

    site = System::Xml::Linq::XElement::Parse(System::String::Format(NEW_SITE_TEMPLATE, gcnew array<System::String^> { System::Environment::NewLine, applicationHost->ApplicationId, (++applicationCounter).ToString(), GetFreePort().ToString(), applicationPoolName, applicationHost->VirtualPath, applicationHost->PhysicalPath }));
    sites->Add(site);
  }

  configuration->Save(IIS::HostedWebCore::CurrentHostConfig);
}

void Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::RemoveSiteForApplication(Sitecore::LiveTesting::Applications::TestApplication^ application)
{
  if (application == nullptr)
  {
    throw gcnew System::ArgumentNullException("application");
  }

  System::Xml::Linq::XDocument^ configuration = System::Xml::Linq::XDocument::Load(IIS::HostedWebCore::CurrentHostConfig);
  System::Xml::Linq::XElement^ site = System::Xml::XPath::Extensions::XPathSelectElement(configuration, System::String::Format("{0}/{1}", SITE_ROOT_XPATH, System::String::Format(SITE_SEARCH_TEMPLATE, application->Id)));

  if (site == nullptr)
  {
    throw gcnew System::InvalidOperationException(System::String::Format("Site registration for the '{0}' application has been previously removed. Probably this was due to concurrency issues.", application->Id));
  }

  site->Remove();

  configuration->Save(IIS::HostedWebCore::CurrentHostConfig);
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(_In_ IIS::HostedWebCore^ hostedWebCore, _In_ System::Web::Hosting::ApplicationManager^ applicationManager, _In_ System::Type^ testApplicationType) : Sitecore::LiveTesting::Applications::TestApplicationManager(applicationManager, testApplicationType), m_hostedWebCore(hostedWebCore)
{
  if (m_hostedWebCore == nullptr)
  {
    throw gcnew System::ArgumentNullException("hostedWebCore");
  }
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(_In_ System::Type^ testApplicationType) : Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager(GetDefaultHostedWebCore(), GetApplicationManagerFromDefaultAppDomain(), testApplicationType)
{
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager() : Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager(Sitecore::LiveTesting::Applications::TestApplication::typeid)
{
}

Sitecore::LiveTesting::Applications::TestApplication^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::StartApplication(_In_ Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost)
{
  if (applicationHost == nullptr)
  {
    throw gcnew System::ArgumentNullException("applicationHost");
  }

  Sitecore::LiveTesting::Applications::TestApplication^ application = Sitecore::LiveTesting::Applications::TestApplicationManager::StartApplication(GetAdjustedApplicationHost(applicationHost));

  CreateSiteForApplicationHost(applicationHost);

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