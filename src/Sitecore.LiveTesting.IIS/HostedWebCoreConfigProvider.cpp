#include "HostedWebCoreConfigProvider.h"

System::String^ Sitecore::LiveTesting::IIS::Configuration::HostedWebCoreConfigProvider::GetDefaultHostConfigFileName()
{
  return System::IO::Path::Combine(System::Environment::GetFolderPath(System::Environment::SpecialFolder::ProgramFilesX86), "IIS Express\\AppServer\\applicationHost.config");
}

System::String^ Sitecore::LiveTesting::IIS::Configuration::HostedWebCoreConfigProvider::GetDefaultRootConfigFileName()
{
  return System::IO::Path::Combine(System::Configuration::ConfigurationManager::OpenMachineConfiguration()->FilePath, "..\\web.config");
}

int Sitecore::LiveTesting::IIS::Configuration::HostedWebCoreConfigProvider::GetFreePort()
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

System::String^ Sitecore::LiveTesting::IIS::Configuration::HostedWebCoreConfigProvider::OriginalHostConfig::get()
{
  return m_originalHostConfig;
}

System::String^ Sitecore::LiveTesting::IIS::Configuration::HostedWebCoreConfigProvider::OriginalRootConfig::get()
{
  return m_originalRootConfig;
}

Sitecore::LiveTesting::IIS::Configuration::HostedWebCoreConfigProvider::HostedWebCoreConfigProvider()
{
  m_originalHostConfig = GetDefaultHostConfigFileName();
  m_originalRootConfig = GetDefaultRootConfigFileName();
}

Sitecore::LiveTesting::IIS::Configuration::HostedWebCoreConfigProvider::HostedWebCoreConfigProvider(_In_ System::String^ originalHostConfig, _In_ System::String^ originalRootConfig)
{
  if (originalHostConfig == nullptr)
  {
    throw gcnew System::ArgumentNullException(originalHostConfig);
  }

  if (originalRootConfig == nullptr)
  {
    throw gcnew System::ArgumentNullException(originalRootConfig);
  }

  m_originalHostConfig = originalHostConfig;
  m_originalRootConfig = originalRootConfig;
}

System::String^ Sitecore::LiveTesting::IIS::Configuration::HostedWebCoreConfigProvider::GetProcessedHostConfig()
{
  System::String^ rawConfiguration = System::IO::File::ReadAllText(OriginalHostConfig)->Replace(IIS_BIN_ENVIRONMENT_VARIABLE_TOKEN, System::IO::Path::Combine(GetDefaultHostConfigFileName(), "..\\.."));
  System::Xml::Linq::XDocument^ configuration = System::Xml::Linq::XDocument::Parse(rawConfiguration);
  System::Xml::Linq::XElement^ sites = System::Xml::XPath::Extensions::XPathSelectElement(configuration, SITE_ROOT_XPATH);
  System::Xml::Linq::XElement^ appPools = System::Xml::XPath::Extensions::XPathSelectElement(configuration, APP_POOL_ROOT_XPATH);

  for each (System::Xml::Linq::XElement^ site in System::Linq::Enumerable::ToArray(sites->Elements(SITE_ELEMENT_NAME)))
  {
    site->Remove();
  }

  for each (System::Xml::Linq::XElement^ appPool in System::Linq::Enumerable::ToArray(System::Linq::Enumerable::Concat(System::Linq::Enumerable::Concat(appPools->Elements(COLLECTION_ADD), appPools->Elements(COLLECTION_REMOVE)), appPools->Elements(COLLECTION_CLEAR))))
  {
    appPool->Remove();
  }

  appPools->Add(System::Xml::Linq::XElement::Parse(DEFAULT_APP_POOL_XML));

  for (int index = 1; index <= 5; ++index)
  {
    sites->Add(System::Xml::Linq::XElement::Parse(System::String::Format(DEFAULT_SITE_XML, System::Environment::NewLine, index, GetFreePort())));
  }

  System::String^ hostConfigFileName = System::IO::Path::GetFullPath(DEFAULT_HOST_CONFIG_FILE_NAME);

  configuration->Save(hostConfigFileName);

  return hostConfigFileName;
}

System::String^ Sitecore::LiveTesting::IIS::Configuration::HostedWebCoreConfigProvider::GetProcessedRootConfig()
{
  return OriginalRootConfig;
}