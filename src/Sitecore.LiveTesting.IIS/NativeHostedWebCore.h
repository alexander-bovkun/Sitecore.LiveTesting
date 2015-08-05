#pragma once

#define WIN32_LEAN_AND_MEAN

#include <memory>

#include <windows.h>
#include <hwebcore.h>

namespace
{
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
}

class NativeHostedWebCore {
  private:
    Library m_hostedWebCoreLibrary;
    PFN_WEB_CORE_SHUTDOWN m_shutdownFunction;
  public:
    NativeHostedWebCore(PCWSTR iisBinFolder, PCWSTR hostConfig, PCWSTR rootConfig, PCWSTR instanceName);
    
    void Stop(DWORD immediate);

    ~NativeHostedWebCore();
};