#include <comdef.h>
#include <mscoree.h>

#include <msclr\marshal_cppstd.h>

#include "HostedWebCore.h"

#pragma managed(push, off)

class CriticalSection
{
  private:
    CRITICAL_SECTION m_criticalSection;
    volatile bool m_disposed;

    CriticalSection(CriticalSection const&);
    void operator=(CriticalSection const&);
  public:
    CriticalSection();

    void Enter();
    void Leave();

    ~CriticalSection();
} instanceCriticalSection;

class CriticalSectionGuard
{
  private:
    CriticalSection& m_criticalSection;

    CriticalSectionGuard(CriticalSectionGuard const&);
    void operator=(CriticalSectionGuard const&);
  public:
    CriticalSectionGuard(CriticalSection& primitive);
    ~CriticalSectionGuard();
};

CriticalSection::CriticalSection()
{
  InitializeCriticalSection(&m_criticalSection);
  m_disposed = false;
}

void CriticalSection::Enter()
{
  if (!m_disposed)
  {
    EnterCriticalSection(&m_criticalSection);
  }
}

void CriticalSection::Leave()
{
  if (!m_disposed)
  {
    LeaveCriticalSection(&m_criticalSection);
  }
}

CriticalSection::~CriticalSection()
{
  m_disposed = true;
  DeleteCriticalSection(&m_criticalSection);
}

CriticalSectionGuard::CriticalSectionGuard(CriticalSection& primitive) : m_criticalSection(primitive)
{
  m_criticalSection.Enter();
}

CriticalSectionGuard::~CriticalSectionGuard()
{
  m_criticalSection.Leave();
}

#pragma managed(pop)

void Sitecore::LiveTesting::IIS::HostedWebCore::CreateHostedWebCore(_In_ HostedWebCoreSetup^ hostedWebCoreSetup)
{
  msclr::interop::marshal_context^ marshalContext = gcnew msclr::interop::marshal_context();
  
  try
  {
    CriticalSectionGuard instanceGuard(instanceCriticalSection);
    
    m_pHostedWebCore = new std::shared_ptr<NativeHostedWebCore>(NativeHostedWebCore::GetInstance(marshalContext->marshal_as<PCWSTR>(hostedWebCoreSetup->HostedWebCoreLibraryPath), marshalContext->marshal_as<PCWSTR>(hostedWebCoreSetup->HostConfig), marshalContext->marshal_as<PCWSTR>(hostedWebCoreSetup->RootConfig), marshalContext->marshal_as<PCWSTR>(hostedWebCoreSetup->InstanceName)));
  }
  catch (const std::runtime_error& e)
  {
    throw gcnew System::InvalidOperationException(gcnew System::String(e.what()));
  }
  catch (const std::invalid_argument& e)
  {
    throw gcnew System::ArgumentException(gcnew System::String(e.what()));
  }
}

System::AppDomain^ Sitecore::LiveTesting::IIS::HostedWebCore::GetHostAppDomain()
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

void Sitecore::LiveTesting::IIS::HostedWebCore::RegisterExternalAssembly(_In_ System::AppDomain^ appDomain, _In_ System::String^ assemblyName, _In_ System::String^ assemblyPath)
{
  if (appDomain == nullptr)
  {
    throw gcnew System::ArgumentNullException("appDomain");
  }

  if (assemblyName == nullptr)
  {
    throw gcnew System::ArgumentNullException("assemblyName");
  }

  if (assemblyPath == nullptr)
  {
    throw gcnew System::ArgumentNullException("assemblyPath");
  }

  HostAppDomainUtility^ hostAppDomainUtility = safe_cast<HostAppDomainUtility^>(appDomain->CreateInstanceFromAndUnwrap(System::Reflection::Assembly::GetExecutingAssembly()->Location, HostAppDomainUtility::typeid->FullName));
  hostAppDomainUtility->RegisterExternalAssembly(assemblyName, assemblyPath);
}

void Sitecore::LiveTesting::IIS::HostedWebCore::ResetProcessHost(_In_ System::AppDomain^ appDomain)
{
  if (appDomain == nullptr)
  {
    throw gcnew System::ArgumentNullException("appDomain");
  }

  HostAppDomainUtility^ hostAppDomainUtility = safe_cast<HostAppDomainUtility^>(appDomain->CreateInstanceFromAndUnwrap(System::Reflection::Assembly::GetExecutingAssembly()->Location, HostAppDomainUtility::typeid->FullName));
  hostAppDomainUtility->ResetProcessHost();
}

Sitecore::LiveTesting::IIS::HostedWebCore::!HostedWebCore()
{
  this->~HostedWebCore();
}

static Sitecore::LiveTesting::IIS::HostedWebCore::HostAppDomainUtility::HostAppDomainUtility()
{
  externalAssemblies = gcnew System::Collections::Generic::Dictionary<System::String^, System::String^>();
}

System::Reflection::Assembly^ Sitecore::LiveTesting::IIS::HostedWebCore::HostAppDomainUtility::AssemblyResolve(_In_ System::Object^, _In_ System::ResolveEventArgs^ args)
{
  System::String^ assemblyPath = nullptr;

  if (args == nullptr)
  {
    throw gcnew System::ArgumentNullException("args");
  }

  System::Threading::Monitor::Enter(externalAssemblies);
  try
  {
    if (!externalAssemblies->TryGetValue(args->Name, assemblyPath))
    {
      return nullptr;
    }
  }
  finally
  {
    System::Threading::Monitor::Exit(externalAssemblies);
  }

  return System::Reflection::Assembly::LoadFrom(assemblyPath);
}

void Sitecore::LiveTesting::IIS::HostedWebCore::HostAppDomainUtility::RegisterExternalAssembly(_In_ System::String^ assemblyName, _In_ System::String^ assemblyPath)
{
  if (assemblyName == nullptr)
  {
    throw gcnew System::ArgumentNullException("assemblyName");
  }

  if (assemblyPath == nullptr)
  {
    throw gcnew System::ArgumentNullException("assemblyPath");
  }

  System::Threading::Monitor::Enter(externalAssemblies);
  try
  {
    if (externalAssemblies->Count == 0)
    {
      System::AppDomain::CurrentDomain->AssemblyResolve += gcnew System::ResolveEventHandler(this, &HostAppDomainUtility::AssemblyResolve);
    }

    if (externalAssemblies->ContainsKey(assemblyName))
    {
      externalAssemblies[assemblyName] = assemblyPath;
    }
    else
    {
      externalAssemblies->Add(assemblyName, assemblyPath);
    }
  }
  finally
  {
    System::Threading::Monitor::Exit(externalAssemblies);
  }
}

void Sitecore::LiveTesting::IIS::HostedWebCore::HostAppDomainUtility::ResetProcessHost()
{
  System::Reflection::FieldInfo^ processHostFieldInfo = System::Web::Hosting::ProcessHost::typeid->GetField(PROCESS_HOST_FIELD_NAME, System::Reflection::BindingFlags::NonPublic | System::Reflection::BindingFlags::Static);

  if (processHostFieldInfo != nullptr)
  {
    processHostFieldInfo->SetValue(nullptr, nullptr);
  }
}

System::Web::Hosting::ApplicationManager^ Sitecore::LiveTesting::IIS::HostedWebCore::HostAppDomainUtility::GetApplicationManager()
{
  return System::Web::Hosting::ApplicationManager::GetApplicationManager();
}

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(_In_ HostedWebCoreSetup^ hostedWebCoreSetup)
{
  if (hostedWebCoreSetup == nullptr)
  {
    throw gcnew System::ArgumentNullException("hostedWebCoreSetup");
  }

  CreateHostedWebCore(hostedWebCoreSetup);
}

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(_In_ System::String^ hostedWebCoreLibraryPath, _In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName)
{
  CreateHostedWebCore(gcnew HostedWebCoreSetup(hostedWebCoreLibraryPath, hostConfig, rootConfig, instanceName));
}

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(_In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName)
{
  CreateHostedWebCore(gcnew HostedWebCoreSetup(System::IO::Path::Combine(System::Environment::GetFolderPath(System::Environment::SpecialFolder::ProgramFilesX86), "IIS Express\\hwebcore.dll"), hostConfig, rootConfig, instanceName));
}

Sitecore::LiveTesting::IIS::HostedWebCoreSetup^ Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostedWebCoreSetup::get()
{
  CriticalSectionGuard instanceGuard(instanceCriticalSection);

  return gcnew HostedWebCoreSetup(gcnew System::String(NativeHostedWebCore::GetCurrentHostedWebCoreLibraryPath().data()), gcnew System::String(NativeHostedWebCore::GetCurrentHostConfig().data()), gcnew System::String(NativeHostedWebCore::GetCurrentRootConfig().data()), gcnew System::String(NativeHostedWebCore::GetCurrentInstanceName().data()));
}

System::Web::Hosting::ApplicationManager^ Sitecore::LiveTesting::IIS::HostedWebCore::GetHostApplicationManager()
{
  System::AppDomain^ hostAppDomain = GetHostAppDomain();

  RegisterExternalAssembly(hostAppDomain, Sitecore::LiveTesting::Applications::TestApplicationManager::typeid->Assembly->FullName, Sitecore::LiveTesting::Applications::TestApplicationManager::typeid->Assembly->Location);
  RegisterExternalAssembly(hostAppDomain, HostedWebCore::typeid->Assembly->GetName()->Name, HostedWebCore::typeid->Assembly->Location);

  HostAppDomainUtility^ hostAppDomainUtility = safe_cast<HostAppDomainUtility^>(hostAppDomain->CreateInstanceAndUnwrap(System::Reflection::Assembly::GetExecutingAssembly()->GetName()->Name, HostAppDomainUtility::typeid->FullName));

  return hostAppDomainUtility->GetApplicationManager();

}

Sitecore::LiveTesting::IIS::HostedWebCore::~HostedWebCore()
{
  CriticalSectionGuard instanceGuard(instanceCriticalSection);

  if (m_pHostedWebCore)
  {
    delete m_pHostedWebCore;
    m_pHostedWebCore = NULL;

    ResetProcessHost(GetHostAppDomain());
  }
}