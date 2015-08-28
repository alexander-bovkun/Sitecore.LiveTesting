#pragma once

#define WIN32_LEAN_AND_MEAN

#include <memory>
#include <string>

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
    static std::unique_ptr<NativeHostedWebCore> instance;
    static std::wstring currentIISBinFolder;
    static std::wstring currentHostConfig;
    static std::wstring currentRootConfig;
    static std::wstring currentInstanceName;

    Library m_hostedWebCoreLibrary;
    PFN_WEB_CORE_SHUTDOWN m_shutdownFunction;

    NativeHostedWebCore(PCWSTR iisBinFolder, PCWSTR hostConfig, PCWSTR rootConfig, PCWSTR instanceName);
    NativeHostedWebCore(NativeHostedWebCore const&);
    void operator=(NativeHostedWebCore const&);
  public:
    static NativeHostedWebCore& GetInstance(PCWSTR iisBinFolder, PCWSTR hostConfig, PCWSTR rootConfig, PCWSTR instanceName);
    static const std::wstring& GetCurrentIISBinFolder();
    static const std::wstring& GetCurrentHostConfig();
    static const std::wstring& GetCurrentRootConfig();
    static const std::wstring& GetCurrentInstanceName();
    
    void Stop(DWORD immediate);

    ~NativeHostedWebCore();
};