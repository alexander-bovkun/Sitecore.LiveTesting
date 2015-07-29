#pragma unmanaged
#include "NativeHostedWebCore.h"

NativeHostedWebCore::NativeHostedWebCore() : m_hostedWebCoreLibrary(GetExpandedPath(L"%windir%\\system32\\inetsrv\\hwebcore.dll").data())
{
  m_activationFunction = m_hostedWebCoreLibrary.GetFunction<PFN_WEB_CORE_ACTIVATE>("WebCoreActivate");
  m_shutdownFunction = m_hostedWebCoreLibrary.GetFunction<PFN_WEB_CORE_SHUTDOWN>("WebCoreShutdown");
}

void NativeHostedWebCore::Start(PCWSTR hostConfig, PCWSTR rootConfig, PCWSTR instanceName)
{
  HRESULT result = m_activationFunction(hostConfig, rootConfig, instanceName);

  if (result != S_OK)
  {
    throw std::runtime_error("Could not activate IIS server core.");
  }
}

void NativeHostedWebCore::Stop(DWORD immediate)
{
  HRESULT result = m_shutdownFunction(immediate);

  if (result != S_OK)
  {
    throw std::runtime_error("Could not shut down IIS server core.");
  }
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

std::wstring GetExpandedPath(const std::wstring& path)
{
  DWORD length = ExpandEnvironmentStringsW(path.data(), NULL, 0);
  std::wstring result(length, 0);

  ExpandEnvironmentStringsW(path.data(), const_cast<LPWSTR>(result.data()), length);

  return result;
}