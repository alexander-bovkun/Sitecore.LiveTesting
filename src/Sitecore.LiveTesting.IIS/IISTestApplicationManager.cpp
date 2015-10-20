#include <comdef.h>
#include <mscoree.h>

#include "IISTestApplicationManager.h"

Sitecore::LiveTesting::IIS::HostedWebCore^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::HostedWebCore::get()
{
  return m_hostedWebCore;
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

System::Web::Hosting::ApplicationManager^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::ApplicationManagerProvider::GetDefaultApplicationManager()
{
  return System::Web::Hosting::ApplicationManager::GetApplicationManager();
}

Sitecore::LiveTesting::IIS::HostedWebCore^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetNewHostedWebCoreOrExistingIfAlreadyHosted(_In_ Configuration::HostedWebCoreConfigProvider^ hostedWebCoreConfigProvider)
{
  if (hostedWebCoreConfigProvider == nullptr)
  {
    throw gcnew System::ArgumentNullException("hostedWebCoreConfigProvider");
  }

  if (System::String::IsNullOrEmpty(Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostedWebCoreLibraryPath))
  {
    return gcnew Sitecore::LiveTesting::IIS::HostedWebCore(hostedWebCoreConfigProvider->GetProcessedHostConfig(), hostedWebCoreConfigProvider->GetProcessedRootConfig(), DEFAULT_HOSTED_WEB_CORE_INSTANCE_NAME);
  }
  else
  {
    return gcnew Sitecore::LiveTesting::IIS::HostedWebCore(Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostedWebCoreLibraryPath, Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostConfig, Sitecore::LiveTesting::IIS::HostedWebCore::CurrentRootConfig, Sitecore::LiveTesting::IIS::HostedWebCore::CurrentInstanceName);
  }
}

System::Web::Hosting::ApplicationManager^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::GetApplicationManagerFromDefaultAppDomain()
{
  System::AppDomain^ appDomain = GetDefaultAppDomain();

  RegisterExternalAssembly(appDomain, TestApplicationManager::typeid->Assembly->FullName, TestApplicationManager::typeid->Assembly->Location);
  RegisterExternalAssembly(appDomain, IISTestApplicationManager::typeid->Assembly->GetName()->Name, IISTestApplicationManager::typeid->Assembly->Location);

  ApplicationManagerProvider^ applicationManagerProvider = safe_cast<ApplicationManagerProvider^>(appDomain->CreateInstanceAndUnwrap(System::Reflection::Assembly::GetExecutingAssembly()->GetName()->Name, ApplicationManagerProvider::typeid->FullName));

  return applicationManagerProvider->GetDefaultApplicationManager();
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
    System::Collections::Generic::ISet<System::String^>^ runningSites = gcnew System::Collections::Generic::HashSet<System::String^>();

    for each (Sitecore::LiveTesting::Applications::TestApplication^ testApplication in GetRunningApplications())
    {
      runningSites->Add(IISEnvironmentInfo::GetApplicationInfo(testApplication)->SiteName);
    }

    for each (System::Xml::Linq::XElement^ siteElement in System::Linq::Enumerable::ToArray(sites->Elements(SITE_ELEMENT_NAME)))
    {
      if (!runningSites->Contains(siteElement->Attribute(SITE_NAME_ATTRIBUTE)->Value))
      {
        site = siteElement;
        break;
      }
    }

    if (site == nullptr)
    {
      throw gcnew System::InvalidOperationException("Connection pool overflow. There is no free site available.");
    }
    else
    {
      site->Attribute(SITE_NAME_ATTRIBUTE)->Value = applicationHost->ApplicationId;
    }
  }

  for each (System::Xml::Linq::XElement^ application in System::Linq::Enumerable::ToArray(site->Elements(SITE_APPLICATION_XPATH)))
  {
    application->Remove();
  }

  if (applicationHost->VirtualPath != ROOT_VIRTUAL_PATH)
  {
    site->Add(System::Xml::Linq::XElement::Parse(System::String::Format(DEFAULT_SITE_APPLICATION_XML, System::Environment::NewLine, ROOT_VIRTUAL_PATH, System::String::Empty)));
  }

  site->Add(System::Xml::Linq::XElement::Parse(System::String::Format(DEFAULT_SITE_APPLICATION_XML, System::Environment::NewLine, applicationHost->VirtualPath, System::IO::Path::GetFullPath(applicationHost->PhysicalPath))));

  return site;
}

Sitecore::LiveTesting::Applications::TestApplicationHost^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::AdjustApplicationHostToSiteConfiguration(_In_ System::Xml::Linq::XElement^ siteConfiguration)
{
  if (siteConfiguration == nullptr)
  {
    throw gcnew System::ArgumentNullException("siteConfiguration");
  }

  System::String^ virtualPath = System::Linq::Enumerable::Last(siteConfiguration->Elements(SITE_APPLICATION_XPATH))->Attribute(SITE_APPLICATION_VIRTUAL_PATH_ATTRIBUTE_NAME)->Value;

  return gcnew Sitecore::LiveTesting::Applications::TestApplicationHost(System::String::Format(APPLICATION_NAME_TEMPLATE, siteConfiguration->Attribute(SITE_ID_ATTRIBUTE_NAME)->Value, virtualPath == ROOT_VIRTUAL_PATH ? System::String::Empty : virtualPath), virtualPath, siteConfiguration->Element(SITE_APPLICATION_XPATH)->Element(SITE_VIRTUAL_DIRECTORY_ELEMENT_NAME)->Attribute(VIRTUAL_DIRECTORY_PHYSICAL_PATH_ATTRIBUTE_NAME)->Value);
}

Sitecore::LiveTesting::IIS::Applications::IISEnvironmentInfo^ Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::CreateApplicationIISEnvironmentInfo(_In_ Sitecore::LiveTesting::Applications::TestApplication^ application, _In_ System::Xml::Linq::XElement^ siteConfiguration)
{
  if (application == nullptr)
  {
    throw gcnew System::ArgumentNullException("application");
  }

  if (siteConfiguration == nullptr)
  {
    throw gcnew System::ArgumentNullException("siteConfiguration");
  }

  System::Xml::Linq::XAttribute^ bindingInformationAttribute = System::Linq::Enumerable::Single(System::Linq::Enumerable::Cast<System::Xml::Linq::XAttribute^>(safe_cast<System::Collections::IEnumerable^>(System::Xml::XPath::Extensions::XPathEvaluate(siteConfiguration, SITE_BINDING_XPATH))));
  
  return gcnew IISEnvironmentInfo(siteConfiguration->Attribute(SITE_NAME_ATTRIBUTE)->Value, int::Parse(bindingInformationAttribute->Value->Split(':')[1]));
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

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager(_In_ Configuration::HostedWebCoreConfigProvider^ hostedWebCoreConfigProvider, _In_ System::Type^ testApplicationType) : Sitecore::LiveTesting::Applications::TestApplicationManager(GetApplicationManagerFromDefaultAppDomain(), testApplicationType), m_hostedWebCore(GetNewHostedWebCoreOrExistingIfAlreadyHosted(hostedWebCoreConfigProvider))
{
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager::IISTestApplicationManager() : Sitecore::LiveTesting::Applications::TestApplicationManager(GetApplicationManagerFromDefaultAppDomain(), Sitecore::LiveTesting::Applications::TestApplication::typeid), m_hostedWebCore(GetNewHostedWebCoreOrExistingIfAlreadyHosted(gcnew Configuration::HostedWebCoreConfigProvider()))
{
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

  application->ExecuteAction(gcnew System::Action<IISEnvironmentInfo^>(IISEnvironmentInfo::SetApplicationInfo), CreateApplicationIISEnvironmentInfo(application, siteConfiguration));
  SaveHostConfiguration(hostConfiguration);

  return application;
}