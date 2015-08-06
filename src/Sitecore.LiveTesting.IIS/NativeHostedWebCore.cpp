#pragma unmanaged

#include <stdexcept>
#include <string>

#include <comdef.h>

#include "NativeHostedWebCore.h"

std::unique_ptr<NativeHostedWebCore> NativeHostedWebCore::instance;

NativeHostedWebCore::NativeHostedWebCore(PCWSTR hostedWebCoreLibraryPath, PCWSTR hostConfig, PCWSTR rootConfig, PCWSTR instanceName) : m_hostedWebCoreLibrary(hostedWebCoreLibraryPath)
{
  PFN_WEB_CORE_ACTIVATE activationFunction = m_hostedWebCoreLibrary.GetFunction<PFN_WEB_CORE_ACTIVATE>("WebCoreActivate");
  m_shutdownFunction = m_hostedWebCoreLibrary.GetFunction<PFN_WEB_CORE_SHUTDOWN>("WebCoreShutdown");

  HRESULT result = activationFunction(hostConfig, rootConfig, instanceName);

  if (result != S_OK)
  {
    std::string errorMessage(_com_error(result).ErrorMessage());
    errorMessage.insert(0, "Could not activate IIS server core. ");

    m_shutdownFunction = NULL;

    throw std::runtime_error(errorMessage);
  }
}

NativeHostedWebCore& NativeHostedWebCore::GetInstance(PCWSTR iisBinFolder, PCWSTR hostConfig, PCWSTR rootConfig, PCWSTR instanceName)
{
  if (instance == NULL)
  {
    instance = std::unique_ptr<NativeHostedWebCore>(new NativeHostedWebCore(iisBinFolder, hostConfig, rootConfig, instanceName));
  }

  return *instance;
}

void NativeHostedWebCore::Stop(DWORD immediate)
{
  if (m_shutdownFunction != NULL)
  {
    HRESULT result = m_shutdownFunction(immediate);

    if (result == S_OK)
    {
      m_shutdownFunction = NULL;

      instance.release();
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

  if (module != NULL)
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

  if (result != NULL)
  {
    return reinterpret_cast<TFunctionPointer>(result);
  }
  else
  {
    throw std::runtime_error("Could not find the requested function.");
  }
}