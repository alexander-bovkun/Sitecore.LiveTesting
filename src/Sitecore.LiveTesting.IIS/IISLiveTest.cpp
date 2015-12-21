#include "IISLiveTest.h"

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager^ Sitecore::LiveTesting::IIS::IISLiveTest::GetDefaultTestApplicationManager(_In_ System::Type^ testType, ... _In_ array<System::Object^>^ arguments)
{
  testType; arguments;

  if ((!System::Web::Hosting::HostingEnvironment::IsHosted) && (DefaultIISTestApplicationManager == nullptr))
  {
    DefaultIISTestApplicationManager = gcnew Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager();
  }

  return DefaultIISTestApplicationManager;
}