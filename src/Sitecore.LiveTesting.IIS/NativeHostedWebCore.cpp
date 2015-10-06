#pragma unmanaged

#include <stdexcept>
#include <string>

#include <comdef.h>

#include "NativeHostedWebCore.h"

std::unique_ptr<NativeHostedWebCore> NativeHostedWebCore::instance;
std::wstring NativeHostedWebCore::currentHostedWebCoreLibraryPath;
std::wstring NativeHostedWebCore::currentHostConfig;
std::wstring NativeHostedWebCore::currentRootConfig;
std::wstring NativeHostedWebCore::currentInstanceName;

NativeHostedWebCore::NativeHostedWebCore(PCWSTR hostedWebCoreLibraryPath, PCWSTR hostConfig, PCWSTR rootConfig, PCWSTR instanceName) : m_hostedWebCoreLibrary(hostedWebCoreLibraryPath)
{
  PFN_WEB_CORE_ACTIVATE pfnActivation = m_hostedWebCoreLibrary.GetFunction<PFN_WEB_CORE_ACTIVATE>("WebCoreActivate");
  m_pfnShutdown = m_hostedWebCoreLibrary.GetFunction<PFN_WEB_CORE_SHUTDOWN>("WebCoreShutdown");

  HRESULT result = pfnActivation(hostConfig, rootConfig, instanceName);

  if (result != S_OK)
  {
    std::string errorMessage(_com_error(result).ErrorMessage());
    errorMessage.insert(0, "Could not activate IIS server core. ");

    m_pfnShutdown = NULL;

    throw std::runtime_error(errorMessage);
  }
}

NativeHostedWebCore& NativeHostedWebCore::GetInstance(PCWSTR hostedWebCoreLibraryPath, PCWSTR hostConfig, PCWSTR rootConfig, PCWSTR instanceName)
{
  if (instance)
  {
    if (!((currentHostedWebCoreLibraryPath == hostedWebCoreLibraryPath) && (currentHostConfig == hostConfig) && (currentRootConfig == rootConfig) && (currentInstanceName == instanceName)))
    {
      throw std::invalid_argument("Cannot create hosted web core with parameters other than ones specified during instantiation of the very first instance. Use GetCurrentIISBinFolder, GetCurrentHostConfig, GetCurrentRootConfig, GetCurrentInstanceName methods to get corresponding parameter values for the first instantiation.");
    }
  }
  else
  {
    instance = std::unique_ptr<NativeHostedWebCore>(new NativeHostedWebCore(hostedWebCoreLibraryPath, hostConfig, rootConfig, instanceName));
    currentHostedWebCoreLibraryPath = hostedWebCoreLibraryPath;
    currentHostConfig = hostConfig;
    currentRootConfig = rootConfig;
    currentInstanceName = instanceName;
  }

  return *instance;
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

void NativeHostedWebCore::Stop(DWORD immediate)
{
  if (m_pfnShutdown)
  {
    HRESULT result = m_pfnShutdown(immediate);

    if (result == S_OK)
    {
      m_pfnShutdown = NULL;

      instance.release();

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

NativeHostedWebCore::~NativeHostedWebCore()
{
  Stop(true);
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