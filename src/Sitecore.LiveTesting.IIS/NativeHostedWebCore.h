#pragma once

#define WIN32_LEAN_AND_MEAN

#include <memory>
#include <stdexcept>
#include <string>

#include <windows.h>
#include <hwebcore.h>

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

std::wstring GetExpandedPath(const std::wstring& path);