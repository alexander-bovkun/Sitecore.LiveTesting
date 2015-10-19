#pragma once

#define WIN32_LEAN_AND_MEAN

#include <memory>
#include <string>

#include <windows.h>
#include <hwebcore.h>

namespace
{
  class CriticalSection
  {
    private:
      CRITICAL_SECTION m_criticalSection;

      CriticalSection(CriticalSection const&);
      void operator=(CriticalSection const&);

      friend class CriticalSectionGuard;
    public:
      CriticalSection();
      ~CriticalSection();
  };

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

class NativeHostedWebCore
{
  private:
    static std::weak_ptr<NativeHostedWebCore> instance;
    static std::wstring currentHostedWebCoreLibraryPath;
    static std::wstring currentHostConfig;
    static std::wstring currentRootConfig;
    static std::wstring currentInstanceName;

    Library m_hostedWebCoreLibrary;
    PFN_WEB_CORE_SHUTDOWN m_pfnShutdown;

    NativeHostedWebCore(const std::wstring& hostedWebCoreLibraryPath, const std::wstring& hostConfig, const std::wstring& rootConfig, const std::wstring& instanceName);
    NativeHostedWebCore(NativeHostedWebCore const&);
    void operator=(NativeHostedWebCore const&);
  public:
    static std::shared_ptr<NativeHostedWebCore> GetInstance(const std::wstring& hostedWebCoreLibraryPath, const std::wstring& hostConfig, const std::wstring& rootConfig, const std::wstring& instanceName);
    static const std::wstring& GetCurrentHostedWebCoreLibraryPath();
    static const std::wstring& GetCurrentHostConfig();
    static const std::wstring& GetCurrentRootConfig();
    static const std::wstring& GetCurrentInstanceName();

    ~NativeHostedWebCore();
};