#include "HostedWebCore.h"

System::String^ Sitecore::LiveTesting::IIS::HostedWebCore::CurrentIISBinFolder::get()
{
  return gcnew System::String(NativeHostedWebCore::GetCurrentIISBinFolder().data());
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

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(System::String^ hostedWebCoreLibraryPath, System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName) try : m_marshalContext(gcnew msclr::interop::marshal_context()), m_pHostedWebCore(&NativeHostedWebCore::GetInstance(m_marshalContext->marshal_as<PCWSTR>(hostedWebCoreLibraryPath), m_marshalContext->marshal_as<PCWSTR>(hostConfig), m_marshalContext->marshal_as<PCWSTR>(rootConfig), m_marshalContext->marshal_as<PCWSTR>(instanceName)))
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

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName) : Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(System::Environment::ExpandEnvironmentVariables("%windir%\\system32\\inetsrv\\hwebcore.dll"), hostConfig, rootConfig, instanceName)
{
}

void Sitecore::LiveTesting::IIS::HostedWebCore::Stop(System::Boolean immediate)
{
  if (m_pHostedWebCore)
  {
    m_pHostedWebCore->Stop(immediate);
    delete m_pHostedWebCore;

    m_pHostedWebCore = NULL;
  }
}

Sitecore::LiveTesting::IIS::HostedWebCore::~HostedWebCore()
{
  Stop(true);
}

Sitecore::LiveTesting::IIS::HostedWebCore::!HostedWebCore()
{
  this->~HostedWebCore();
}