#pragma once

#include <sal.h>

#include "IISTestApplicationManager.h"

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      [System::Runtime::InteropServices::ComVisible(false)]
      public ref class IISLiveTest : public Sitecore::LiveTesting::LiveTest
      {
        private:
          static Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager^ DefaultIISTestApplicationManager;
        public:
          static Sitecore::LiveTesting::IIS::Applications::IISTestApplicationManager^ GetDefaultTestApplicationManager(_In_ System::Type^ testType, ... _In_ array<System::Object^>^ arguments);
      };
    }
  }
}