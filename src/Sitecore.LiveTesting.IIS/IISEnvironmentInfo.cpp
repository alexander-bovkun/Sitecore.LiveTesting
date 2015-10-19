#include "IISEnvironmentInfo.h"

Sitecore::LiveTesting::IIS::Applications::IISEnvironmentInfo::IISEnvironmentInfo(_In_ System::Runtime::Serialization::SerializationInfo^ info, _In_ System::Runtime::Serialization::StreamingContext)
{
  if (info == nullptr)
  {
    throw gcnew System::ArgumentNullException("info");
  }

  m_siteName = info->GetString(SITE_NAME_SERIALIZATION_KEY);

  if (m_siteName == nullptr)
  {
    throw gcnew System::ArgumentException(System::String::Format(System::Globalization::CultureInfo::InvariantCulture, "Serialization info should not contain null value for '{0}' key.", SITE_NAME_SERIALIZATION_KEY));
  }

  m_port = info->GetInt32(PORT_SERIALIZATION_KEY);

  if (m_port == 0)
  {
    throw gcnew System::ArgumentException(System::String::Format(System::Globalization::CultureInfo::InvariantCulture, "Serialization info should not contain value of '0' for '{0}' key.", PORT_SERIALIZATION_KEY));
  }
}

Sitecore::LiveTesting::IIS::Applications::IISEnvironmentInfo::IISEnvironmentInfo(_In_ System::String^ siteName, _In_ int port) : m_siteName(siteName), m_port(port)
{
  if (siteName == nullptr)
  {
    throw gcnew System::ArgumentNullException("siteName");
  }

  if (port == 0)
  {
    throw gcnew System::ArgumentOutOfRangeException("port");
  }
}

System::String^ Sitecore::LiveTesting::IIS::Applications::IISEnvironmentInfo::SiteName::get()
{
  return m_siteName;
}

int Sitecore::LiveTesting::IIS::Applications::IISEnvironmentInfo::Port::get()
{
  return m_port;
}

[System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::SerializationFormatter)]
void Sitecore::LiveTesting::IIS::Applications::IISEnvironmentInfo::GetObjectData(_In_ System::Runtime::Serialization::SerializationInfo^ info, _In_ System::Runtime::Serialization::StreamingContext)
{
  if (info == nullptr)
  {
    throw gcnew System::ArgumentNullException("info");
  }

  info->AddValue(SITE_NAME_SERIALIZATION_KEY, m_siteName);
  info->AddValue(PORT_SERIALIZATION_KEY, m_port);
}