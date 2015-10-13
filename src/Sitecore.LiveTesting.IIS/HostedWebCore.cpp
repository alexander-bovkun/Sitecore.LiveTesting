#include "HostedWebCore.h"

System::String^ Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostedWebCoreLibraryPath::get()
{
  return gcnew System::String(NativeHostedWebCore::GetCurrentHostedWebCoreLibraryPath().data());
}

System::String^ Sitecore::LiveTesting::IIS::HostedWebCore::CurrentHostConfig::get()
{
  return gcnew System::String(NativeHostedWebCore::GetCurrentHostConfig().data());
}

System::String^ Sitecore::LiveTesting::IIS::HostedWebCore::CurrentRootConfig::get()
{
  return gcnew System::String(NativeHostedWebCore::GetCurrentRootConfig().data());
}

System::String^ Sitecore::LiveTesting::IIS::HostedWebCore::CurrentInstanceName::get()
{
  return gcnew System::String(NativeHostedWebCore::GetCurrentInstanceName().data());
}

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(_In_ System::String^ hostedWebCoreLibraryPath, _In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName) try : m_marshalContext(gcnew msclr::interop::marshal_context()), m_pHostedWebCore(new std::shared_ptr<NativeHostedWebCore>(NativeHostedWebCore::GetInstance(m_marshalContext->marshal_as<PCWSTR>(hostedWebCoreLibraryPath), m_marshalContext->marshal_as<PCWSTR>(hostConfig), m_marshalContext->marshal_as<PCWSTR>(rootConfig), m_marshalContext->marshal_as<PCWSTR>(instanceName))))
{
  m_marshalContext->~marshal_context();
}
catch (const std::runtime_error& e)
{
  m_marshalContext->~marshal_context();
  throw gcnew System::InvalidOperationException(gcnew System::String(e.what()));
}
catch (const std::invalid_argument& e)
{
  m_marshalContext->~marshal_context();
  throw gcnew System::ArgumentException(gcnew System::String(e.what()));
}

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(_In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName) try : m_marshalContext(gcnew msclr::interop::marshal_context()), m_pHostedWebCore(new std::shared_ptr<NativeHostedWebCore>(NativeHostedWebCore::GetInstance(m_marshalContext->marshal_as<PCWSTR>(System::IO::Path::Combine(System::Environment::GetFolderPath(System::Environment::SpecialFolder::ProgramFilesX86), "IIS Express\\hwebcore.dll")), m_marshalContext->marshal_as<PCWSTR>(hostConfig), m_marshalContext->marshal_as<PCWSTR>(rootConfig), m_marshalContext->marshal_as<PCWSTR>(instanceName))))
{
  m_marshalContext->~marshal_context();
}
catch (const std::runtime_error& e)
{
  m_marshalContext->~marshal_context();
  throw gcnew System::InvalidOperationException(gcnew System::String(e.what()));
}
catch (const std::invalid_argument& e)
{
  m_marshalContext->~marshal_context();
  throw gcnew System::ArgumentException(gcnew System::String(e.what()));
}

Sitecore::LiveTesting::IIS::HostedWebCore::!HostedWebCore()
{
  this->~HostedWebCore();
}

Sitecore::LiveTesting::IIS::HostedWebCore::~HostedWebCore()
{
  if (m_pHostedWebCore)
  {
    delete m_pHostedWebCore;
    m_pHostedWebCore = NULL;
  }
}