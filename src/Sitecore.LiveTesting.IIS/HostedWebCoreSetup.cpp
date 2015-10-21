#include "HostedWebCoreSetup.h"

Sitecore::LiveTesting::IIS::HostedWebCoreSetup::HostedWebCoreSetup(_In_ System::Runtime::Serialization::SerializationInfo^ info, _In_ System::Runtime::Serialization::StreamingContext)
{
  if (info == nullptr)
  {
    throw gcnew System::ArgumentNullException("info");
  }

  m_hostedWebCoreLibraryPath = info->GetString(HOSTED_WEB_CORE_LIBRARY_PATH_SERIALIZATION_KEY);

  if (m_hostedWebCoreLibraryPath == nullptr)
  {
    throw gcnew System::ArgumentException(System::String::Format(System::Globalization::CultureInfo::InvariantCulture, "Serialization info should not contain null value for '{0}' key.", HOSTED_WEB_CORE_LIBRARY_PATH_SERIALIZATION_KEY));
  }

  m_hostConfig = info->GetString(HOST_CONFIG_SERIALIZATION_KEY);

  if (m_hostConfig == nullptr)
  {
    throw gcnew System::ArgumentException(System::String::Format(System::Globalization::CultureInfo::InvariantCulture, "Serialization info should not contain null value for '{0}' key.", HOST_CONFIG_SERIALIZATION_KEY));
  }

  m_rootConfig = info->GetString(ROOT_CONFIG_SERIALIZATION_KEY);

  if (m_rootConfig == nullptr)
  {
    throw gcnew System::ArgumentException(System::String::Format(System::Globalization::CultureInfo::InvariantCulture, "Serialization info should not contain null value for '{0}' key.", ROOT_CONFIG_SERIALIZATION_KEY));
  }

  m_instanceName = info->GetString(INSTANCE_NAME_SERIALIZATION_KEY);

  if (m_instanceName == nullptr)
  {
    throw gcnew System::ArgumentException(System::String::Format(System::Globalization::CultureInfo::InvariantCulture, "Serialization info should not contain null value for '{0}' key.", INSTANCE_NAME_SERIALIZATION_KEY));
  }
}

void Sitecore::LiveTesting::IIS::HostedWebCoreSetup::GetObjectData(_In_ System::Runtime::Serialization::SerializationInfo^ info, _In_ System::Runtime::Serialization::StreamingContext)
{
  if (info == nullptr)
  {
    throw gcnew System::ArgumentNullException("info");
  }

  info->AddValue(HOSTED_WEB_CORE_LIBRARY_PATH_SERIALIZATION_KEY, HostedWebCoreLibraryPath);
  info->AddValue(HOST_CONFIG_SERIALIZATION_KEY, HostConfig);
  info->AddValue(ROOT_CONFIG_SERIALIZATION_KEY, RootConfig);
  info->AddValue(INSTANCE_NAME_SERIALIZATION_KEY, InstanceName);
}

Sitecore::LiveTesting::IIS::HostedWebCoreSetup::HostedWebCoreSetup(_In_ System::String^ hostedWebCoreLibraryPath, _In_ System::String^ hostConfig, _In_ System::String^ rootConfig, _In_ System::String^ instanceName) : m_hostedWebCoreLibraryPath(hostedWebCoreLibraryPath), m_hostConfig(hostConfig), m_rootConfig(rootConfig), m_instanceName(instanceName)
{
  if (hostedWebCoreLibraryPath == nullptr)
  {
    throw gcnew System::ArgumentNullException("hostedWebCoreLibraryPath");
  }

  if (hostConfig == nullptr)
  {
    throw gcnew System::ArgumentNullException("hostConfig");
  }

  if (rootConfig == nullptr)
  {
    throw gcnew System::ArgumentNullException("rootConfig");
  }

  if (instanceName == nullptr)
  {
    throw gcnew System::ArgumentNullException("instanceName");
  }
}

System::String^ Sitecore::LiveTesting::IIS::HostedWebCoreSetup::HostedWebCoreLibraryPath::get()
{
  return m_hostedWebCoreLibraryPath;
}

System::String^ Sitecore::LiveTesting::IIS::HostedWebCoreSetup::HostConfig::get()
{
  return m_hostConfig;
}

System::String^ Sitecore::LiveTesting::IIS::HostedWebCoreSetup::RootConfig::get()
{
  return m_rootConfig;
}

System::String^ Sitecore::LiveTesting::IIS::HostedWebCoreSetup::InstanceName::get()
{
  return m_instanceName;
}