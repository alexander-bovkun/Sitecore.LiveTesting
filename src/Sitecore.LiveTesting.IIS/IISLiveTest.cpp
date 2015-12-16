#include "IISLiveTest.h"

static Sitecore::LiveTesting::IIS::IISLiveTest::IISLiveTest()
{
  DefaultIISTestApplicationManager = gcnew Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager();
}

Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager^ Sitecore::LiveTesting::IIS::IISLiveTest::GetDefaultTestApplicationManager(_In_ System::Type^ testType, _In_ ... array<System::Object^>^ arguments)
{
  testType; arguments;

  return DefaultIISTestApplicationManager;
}