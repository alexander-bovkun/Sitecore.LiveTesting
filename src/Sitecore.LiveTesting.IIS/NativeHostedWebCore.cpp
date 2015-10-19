#include <stdexcept>
#include <string>

#include <comdef.h>

#include "NativeHostedWebCore.h"

#pragma unmanaged

CriticalSection instanceCriticalSection;

std::weak_ptr<NativeHostedWebCore> NativeHostedWebCore::instance;
std::wstring NativeHostedWebCore::currentHostedWebCoreLibraryPath;
std::wstring NativeHostedWebCore::currentHostConfig;
std::wstring NativeHostedWebCore::currentRootConfig;
std::wstring NativeHostedWebCore::currentInstanceName;

NativeHostedWebCore::NativeHostedWebCore(const std::wstring& hostedWebCoreLibraryPath, const std::wstring& hostConfig, const std::wstring& rootConfig, const std::wstring& instanceName) : m_hostedWebCoreLibrary(hostedWebCoreLibraryPath.data())
{
  PFN_WEB_CORE_ACTIVATE pfnActivation = m_hostedWebCoreLibrary.GetFunction<PFN_WEB_CORE_ACTIVATE>("WebCoreActivate");
  m_pfnShutdown = m_hostedWebCoreLibrary.GetFunction<PFN_WEB_CORE_SHUTDOWN>("WebCoreShutdown");

  HRESULT result = pfnActivation(hostConfig.data(), rootConfig.data(), instanceName.data());

  if (result != S_OK)
  {
    std::string errorMessage(_com_error(result).ErrorMessage());
    errorMessage.insert(0, "Could not activate IIS server core. ");

    m_pfnShutdown = NULL;

    throw std::runtime_error(errorMessage);
  }
}

std::shared_ptr<NativeHostedWebCore> NativeHostedWebCore::GetInstance(const std::wstring& hostedWebCoreLibraryPath, const std::wstring& hostConfig, const std::wstring& rootConfig, const std::wstring& instanceName)
{
  CriticalSectionGuard instanceGuard(instanceCriticalSection);
  std::shared_ptr<NativeHostedWebCore> result(instance.lock());

  if (result)
  {
    if (!((currentHostedWebCoreLibraryPath == hostedWebCoreLibraryPath) && (currentHostConfig == hostConfig) && (currentRootConfig == rootConfig) && (currentInstanceName == instanceName)))
    {
      throw std::invalid_argument("Cannot create hosted web core with parameters other than ones specified during instantiation of the very first instance. Use GetCurrentHostedWebCoreLibraryPath, GetCurrentHostConfig, GetCurrentRootConfig, GetCurrentInstanceName methods to get corresponding parameter values for the first instantiation.");
    }
  }
  else
  {
    result = std::shared_ptr<NativeHostedWebCore>(new NativeHostedWebCore(hostedWebCoreLibraryPath, hostConfig, rootConfig, instanceName));
    instance = std::weak_ptr<NativeHostedWebCore>(result);

    currentHostedWebCoreLibraryPath = hostedWebCoreLibraryPath;
    currentHostConfig = hostConfig;
    currentRootConfig = rootConfig;
    currentInstanceName = instanceName;
  }

  return result;
}

const std::wstring& NativeHostedWebCore::GetCurrentHostedWebCoreLibraryPath()
{
  return currentHostedWebCoreLibraryPath;
}

const std::wstring& NativeHostedWebCore::GetCurrentHostConfig()
{
  return currentHostConfig;
}

const std::wstring& NativeHostedWebCore::GetCurrentRootConfig()
{
  return currentRootConfig;
}

const std::wstring& NativeHostedWebCore::GetCurrentInstanceName()
{
  return currentInstanceName;
}

NativeHostedWebCore::~NativeHostedWebCore()
{
  CriticalSectionGuard instanceGuard(instanceCriticalSection);

  if (m_pfnShutdown)
  {
    HRESULT result = m_pfnShutdown(TRUE);

    if (result == S_OK)
    {
      m_pfnShutdown = NULL;

      currentHostedWebCoreLibraryPath.clear();
      currentHostConfig.clear();
      currentRootConfig.clear();
      currentInstanceName.clear();
    }
    else
    {
      std::string errorMessage(_com_error(result).ErrorMessage());
      errorMessage.insert(0, "Could not shut down IIS server core. ");

      throw std::runtime_error(errorMessage);
    }
  }
}

Library::Library(LPCWSTR fileName) {
  HMODULE module = LoadLibraryW(fileName);

  if (module)
  {
    m_module = std::unique_ptr<HMODULE, Deleter>(module, Deleter());
  }
  else
  {
    throw std::runtime_error("Could not load the requested library.");
  }
}

template<typename TFunctionPointer> TFunctionPointer Library::GetFunction(LPCSTR name) const
{
  FARPROC result = GetProcAddress(m_module.get(), name);

  if (result)
  {
    return reinterpret_cast<TFunctionPointer>(result);
  }
  else
  {
    throw std::runtime_error("Could not find the requested function.");
  }
}

CriticalSectionGuard::CriticalSectionGuard(CriticalSection& primitive) : m_criticalSection(primitive)
{
  EnterCriticalSection(&m_criticalSection.m_criticalSection);
}

CriticalSectionGuard::~CriticalSectionGuard()
{
  LeaveCriticalSection(&m_criticalSection.m_criticalSection);
}

CriticalSection::CriticalSection()
{
  InitializeCriticalSection(&m_criticalSection);
}

CriticalSection::~CriticalSection()
{
  DeleteCriticalSection(&m_criticalSection);
}