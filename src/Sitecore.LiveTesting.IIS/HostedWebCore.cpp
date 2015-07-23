#define WIN32_LEAN_AND_MEAN

#include <memory>
#include <stdexcept>
#include <string>

#include "windows.h"
#include "hwebcore.h"

std::wstring GetExpandedPath(const std::wstring& path)
{
  DWORD length = ExpandEnvironmentStringsW(path.data(), NULL, 0);
  std::wstring result(length, 0);

  ExpandEnvironmentStringsW(path.data(), const_cast<LPWSTR>(result.data()), length);

  return result;
}

class Library
{
  private:
    struct Deleter
    {
      typedef HMODULE pointer;

      void operator()(HMODULE module)
      {
        FreeLibrary(module);
      }
    };

    std::unique_ptr<HMODULE, Deleter> m_module;
  public:
    Library(LPCWSTR fileName);

    template<typename TFunctionPointer> TFunctionPointer GetFunction(LPCSTR name) const;
};

class IISServer {
  private:
    Library m_hostedWebCoreLibrary;
    PFN_WEB_CORE_ACTIVATE m_activationFunction;
    PFN_WEB_CORE_SHUTDOWN m_shutdownFunction;
  public:
    IISServer();

    void Start();
    void Stop();

    ~IISServer();
};

IISServer::IISServer() : m_hostedWebCoreLibrary(GetExpandedPath(L"%windir%\\system32\\inetsrv\\hwebcore.dll").data())
{
  m_activationFunction = m_hostedWebCoreLibrary.GetFunction<PFN_WEB_CORE_ACTIVATE>("WebCoreActivate");
  m_shutdownFunction = m_hostedWebCoreLibrary.GetFunction<PFN_WEB_CORE_SHUTDOWN>("WebCoreShutdown");
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

__declspec(dllexport) int StartHostedWebCore();
__declspec(dllexport) int StopHostedWebCore();