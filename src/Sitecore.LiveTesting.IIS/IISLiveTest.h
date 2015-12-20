#pragma once

#include <sal.h>

#include "IISTestApplicationManager.h"

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      public ref class IISLiveTest : public Sitecore::LiveTesting::LiveTest
      {
        private:
          static initonly Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager^ DefaultIISTestApplicationManager;

          static IISLiveTest();
        public:
          static Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager^ GetDefaultTestApplicationManager(_In_ System::Type^ testType, ... _In_ array<System::Object^>^ arguments);
      };
    }
  }
}