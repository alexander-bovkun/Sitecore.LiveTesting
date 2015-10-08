#include <comdef.h>
#include <mscoree.h>

#include "IISTestApplicationManager.h"

Sitecore::LiveTesting::IIS::HostedWebCore^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::HostedWebCore::get()
{
  return m_hostedWebCore;
}

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

System::AppDomain^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetDefaultAppDomain()
{
  ICorRuntimeHost* pRuntimeHost;
  HRESULT result = CoCreateInstance(CLSID_CorRuntimeHost, NULL, CLSCTX_INPROC_SERVER, IID_ICorRuntimeHost, reinterpret_cast<LPVOID*>(&pRuntimeHost));

  if (result != S_OK)
  {
    throw gcnew System::InvalidOperationException(System::String::Format(gcnew System::String("Could not create an instance of ICorRuntimeHost interface implementation. {0}"), gcnew System::String(_com_error(result).ErrorMessage())));
  }

  IUnknown* pAppDomain;

  result = pRuntimeHost->GetDefaultDomain(&pAppDomain);
  pRuntimeHost->Release();

  if (result != S_OK)
  {
    throw gcnew System::InvalidOperationException(System::String::Format(gcnew System::String("Could not create an instance of ICorRuntimeHost interface implementation. {0}"), gcnew System::String(_com_error(result).ErrorMessage())));
  }

  return safe_cast<System::AppDomain^>(System::Runtime::InteropServices::Marshal::GetObjectForIUnknown(System::IntPtr(pAppDomain)));
}

void Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::SetApplicationSiteName(System::String^ siteName)
{
  ApplicationSiteName = siteName;
}

System::Web::Hosting::ApplicationManager^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::ApplicationManagerProvider::GetDefaultApplicationManager()
{
  return System::Web::Hosting::ApplicationManager::GetApplicationManager();
}

System::String^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetDefaultRootConfigFileName()
{
  return System::IO::Path::Combine(System::IO::Path::GetDirectoryName(System::Configuration::ConfigurationManager::OpenMachineConfiguration()->FilePath), "web.config");
}

Sitecore::LiveTesting::IIS::HostedWebCore^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetDefaultHostedWebCore()
{
  if (System::String::IsNullOrEmpty(Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostedWebCoreLibraryPath))
  {
    System::String^ rootConfigFileName = GetDefaultRootConfigFileName();
    System::String^ iisBinPath = System::IO::Path::Combine(System::Environment::SystemDirectory, "inetsrv");
    System::String^ iisConfigPath = System::IO::Path::Combine(iisBinPath, "config\\applicationHost.config");
    System::String^ rawConfiguration = System::IO::File::ReadAllText(iisConfigPath)->Replace("%IIS_BIN%", iisBinPath)->Replace("%windir%", System::Environment::GetFolderPath(System::Environment::SpecialFolder::Windows))->Replace("%SystemDrive%", System::IO::Path::GetPathRoot(System::Environment::SystemDirectory));
    System::Xml::Linq::XDocument^ configuration = System::Xml::Linq::XDocument::Parse(rawConfiguration);

    System::Xml::XPath::Extensions::XPathSelectElement(configuration, SITE_ROOT_XPATH)->RemoveNodes();

    System::String^ hostConfigFileName = System::IO::Path::GetTempFileName();

    configuration->Save(hostConfigFileName);
    
    return gcnew Sitecore::LiveTesting::IIS::HostedWebCore(hostConfigFileName, rootConfigFileName, DEFAULT_HOSTED_WEB_CORE_INSTANCE_NAME);
  }
  else
  {
    return gcnew Sitecore::LiveTesting::IIS::HostedWebCore(Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostedWebCoreLibraryPath, Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostConfig, Sitecore::LiveTesting::IIS::HostedWebCore::CurrentRootConfig, Sitecore::LiveTesting::IIS::HostedWebCore::CurrentInstanceName);
  }
}

System::Web::Hosting::ApplicationManager^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetApplicationManagerFromDefaultAppDomain()
{
  System::AppDomain^ appDomain = GetDefaultAppDomain();
  ApplicationManagerProvider^ applicationManagerProvider = safe_cast<ApplicationManagerProvider^>(appDomain->CreateInstanceAndUnwrap(System::Reflection::Assembly::GetExecutingAssembly()->GetName()->Name, ApplicationManagerProvider::typeid->FullName));

  return applicationManagerProvider->GetDefaultApplicationManager();
}

int Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetIncrementedGlobalSiteCounter()
{
  static int globalSiteCounter = 0;
  
  return ++globalSiteCounter;
}

System::Xml::Linq::XDocument^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::LoadHostConfiguration()
{
  return System::Xml::Linq::XDocument::Load(IIS::HostedWebCore::CurrentHostConfig);
}

System::Xml::Linq::XElement^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetSiteConfigurationForApplication(_In_ System::Xml::Linq::XDocument^ hostConfiguration, _In_ Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost)
{
  if (hostConfiguration == nullptr)
  {
    throw gcnew System::ArgumentNullException("hostConfiguration");
  }

  if (applicationHost == nullptr)
  {
    throw gcnew System::ArgumentNullException("applicationHost");
  }

  System::Xml::Linq::XElement^ sites = System::Xml::XPath::Extensions::XPathSelectElement(hostConfiguration, SITE_ROOT_XPATH);
  System::Xml::Linq::XElement^ site = System::Xml::XPath::Extensions::XPathSelectElement(sites, System::String::Format(SITE_SEARCH_TEMPLATE, applicationHost->ApplicationId));

  if (site == nullptr)
  {
    System::String^ applicationPoolName = System::Linq::Enumerable::Single(System::Linq::Enumerable::Cast<System::Xml::Linq::XAttribute^>((System::Collections::IEnumerable^)System::Xml::XPath::Extensions::XPathEvaluate(hostConfiguration, SINGLE_APP_POOL_XPATH)))->Value;

    site = System::Xml::Linq::XElement::Parse(System::String::Format(NEW_SITE_TEMPLATE, gcnew array<System::String^> { System::Environment::NewLine, applicationHost->ApplicationId, GetIncrementedGlobalSiteCounter().ToString(), GetFreePort().ToString(), applicationPoolName, applicationHost->VirtualPath, System::IO::Path::GetFullPath(applicationHost->PhysicalPath) }));
    sites->Add(site);
  }

  return site;
}

Sitecore::LiveTesting::Applications::TestApplicationHost^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::AdjustApplicationHostToSiteConfiguration(_In_ System::Xml::Linq::XElement^ siteConfiguration)
{
  if (siteConfiguration == nullptr)
  {
    throw gcnew System::ArgumentNullException("siteConfiguration");
  }

  return gcnew Sitecore::LiveTesting::Applications::TestApplicationHost(System::String::Format(APPLICATION_NAME_TEMPLATE, siteConfiguration->Attribute(System::Xml::Linq::XName::Get("id"))->Value), siteConfiguration->Element("application")->Element("virtualDirectory")->Attribute("path")->Value, siteConfiguration->Element("application")->Element("virtualDirectory")->Attribute("physicalPath")->Value);
}

void Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::SaveHostConfiguration(_In_ System::Xml::Linq::XDocument^ hostConfiguration)
{
  if (hostConfiguration == nullptr)
  {
    throw gcnew System::ArgumentNullException("hostConfiguration");
  }

  hostConfiguration->Save(IIS::HostedWebCore::CurrentHostConfig);
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(_In_ IIS::HostedWebCore^ hostedWebCore, _In_ System::Web::Hosting::ApplicationManager^ applicationManager, _In_ System::Type^ testApplicationType) : Sitecore::LiveTesting::Applications::TestApplicationManager(applicationManager, testApplicationType), m_hostedWebCore(hostedWebCore)
{
  if (m_hostedWebCore == nullptr)
  {
    throw gcnew System::ArgumentNullException("hostedWebCore");
  }
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(_In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::Type^ testApplicationType) : Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager(gcnew IIS::HostedWebCore(hostConfig, rootConfig, DEFAULT_HOSTED_WEB_CORE_INSTANCE_NAME), GetApplicationManagerFromDefaultAppDomain(), testApplicationType)
{
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(_In_ System::String^ hostConfig, _In_ System::String^ rootConfig) : Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager(hostConfig, rootConfig, Sitecore::LiveTesting::Applications::TestApplication::typeid)
{
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(_In_ System::String^ hostConfig, _In_ System::Type^ testApplicationType) : Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager(hostConfig, GetDefaultRootConfigFileName(), testApplicationType)
{
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(_In_ System::String^ hostConfig) : Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager(hostConfig, GetDefaultRootConfigFileName())
{
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(_In_ System::Type^ testApplicationType) : Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager(GetDefaultHostedWebCore(), GetApplicationManagerFromDefaultAppDomain(), testApplicationType)
{
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager() : Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager(Sitecore::LiveTesting::Applications::TestApplication::typeid)
{
}

System::String^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetApplicationSiteName(_In_ Sitecore::LiveTesting::Applications::TestApplication^ application)
{
  if (application == nullptr)
  {
    return ApplicationSiteName;
  }

  return safe_cast<System::String^>(application->ExecuteAction(gcnew System::Func<Sitecore::LiveTesting::Applications::TestApplication^, System::String^>(GetApplicationSiteName), gcnew array<System::Object^> { nullptr }));
}

System::String^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetApplicationVirtualPath(_In_ Sitecore::LiveTesting::Applications::TestApplication^ application)
{
  if (application == nullptr)
  {
    throw gcnew System::ArgumentNullException("application");
  }

  return safe_cast<System::String^>(application->ExecuteAction(System::Delegate::CreateDelegate(System::Func<System::String^>::typeid, System::Web::Hosting::HostingEnvironment::typeid->GetProperty("ApplicationVirtualPath")->GetGetMethod())));
}

System::String^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetApplicationPhysicalPath(_In_ Sitecore::LiveTesting::Applications::TestApplication^ application)
{
  if (application == nullptr)
  {
    throw gcnew System::ArgumentNullException("application");
  }

  return safe_cast<System::String^>(application->ExecuteAction(System::Delegate::CreateDelegate(System::Func<System::String^>::typeid, System::Web::Hosting::HostingEnvironment::typeid->GetProperty("ApplicationPhysicalPath")->GetGetMethod())));
}

Sitecore::LiveTesting::Applications::TestApplication^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::StartApplication(_In_ Sitecore::LiveTesting::Applications::TestApplicationHost^ applicationHost)
{
  if (applicationHost == nullptr)
  {
    throw gcnew System::ArgumentNullException("applicationHost");
  }

  System::Xml::Linq::XDocument^ hostConfiguration = LoadHostConfiguration();
  System::Xml::Linq::XElement^ siteConfiguration = GetSiteConfigurationForApplication(hostConfiguration, applicationHost);

  Sitecore::LiveTesting::Applications::TestApplication^ application = Sitecore::LiveTesting::Applications::TestApplicationManager::StartApplication(AdjustApplicationHostToSiteConfiguration(siteConfiguration));
  application->ExecuteAction(gcnew System::Action<System::String^>(SetApplicationSiteName), applicationHost->ApplicationId);

  SaveHostConfiguration(hostConfiguration);

  return application;
}

void Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::StopApplication(_In_ Sitecore::LiveTesting::Applications::TestApplication^ application)
{
  if (application == nullptr)
  {
    throw gcnew System::ArgumentNullException("application");
  }

  System::Xml::Linq::XDocument^ hostConfiguration = LoadHostConfiguration();
  System::Xml::Linq::XElement^ siteConfiguration = GetSiteConfigurationForApplication(hostConfiguration, gcnew Sitecore::LiveTesting::Applications::TestApplicationHost(GetApplicationSiteName(application), GetApplicationVirtualPath(application), GetApplicationPhysicalPath(application)));

  siteConfiguration->Remove();

  SaveHostConfiguration(hostConfiguration);

  Sitecore::LiveTesting::Applications::TestApplicationManager::StopApplication(application);
}