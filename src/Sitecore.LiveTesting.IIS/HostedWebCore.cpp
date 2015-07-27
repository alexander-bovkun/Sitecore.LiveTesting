#define WIN32_LEAN_AND_MEAN

#include <memory>
#include <stdexcept>
#include <string>

#include <windows.h>
#include <hwebcore.h>

#include <msclr\marshal_cppstd.h>

#using <mscorlib.dll>

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

class NativeHostedWebCore {
  private:
    Library m_hostedWebCoreLibrary;
    PFN_WEB_CORE_ACTIVATE m_activationFunction;
    PFN_WEB_CORE_SHUTDOWN m_shutdownFunction;
  public:
    NativeHostedWebCore();

    void Start(PCWSTR hostConfig, PCWSTR rootConfig, PCWSTR instanceName);
    void Stop(DWORD immediate);
};

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      public ref class HostedWebCore sealed : System::IDisposable
      {
        private:
          NativeHostedWebCore* m_pHostedWebCore;
          msclr::interop::marshal_context^ m_marshalContext;

          bool m_disposedUnmanagedResources;
          bool m_disposedManagedResources;
        protected:
          !HostedWebCore();
        public:
          HostedWebCore();

          void Start(System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName);
          void Stop(System::Boolean immediate);

          ~HostedWebCore();
      };
    }
  }
}

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore() : m_pHostedWebCore(new NativeHostedWebCore()), m_marshalContext(gcnew msclr::interop::marshal_context())
{
}

void Sitecore::LiveTesting::IIS::HostedWebCore::Start(System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName)
{
  m_pHostedWebCore->Start(m_marshalContext->marshal_as<PCWSTR>(hostConfig), m_marshalContext->marshal_as<PCWSTR>(rootConfig), m_marshalContext->marshal_as<PCWSTR>(instanceName));
}

void Sitecore::LiveTesting::IIS::HostedWebCore::Stop(System::Boolean immediate)
{
  m_pHostedWebCore->Stop(immediate);
}

Sitecore::LiveTesting::IIS::HostedWebCore::~HostedWebCore()
{
  if (!m_disposedManagedResources) {
    m_marshalContext->~marshal_context();
    m_disposedManagedResources = true;
  }

  this->!HostedWebCore();
}

Sitecore::LiveTesting::IIS::HostedWebCore::!HostedWebCore()
{
  if (!m_disposedUnmanagedResources)
  {
    delete m_pHostedWebCore;
    m_disposedUnmanagedResources = true;
  }
}

std::wstring GetExpandedPath(const std::wstring& path);

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