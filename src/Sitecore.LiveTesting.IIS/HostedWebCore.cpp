#include "NativeHostedWebCore.h"

#include <msclr\marshal_cppstd.h>

#using <mscorlib.dll>

namespace Sitecore
{
  namespace LiveTesting
  {
    namespace IIS
    {
      public ref class HostedWebCore sealed : System::IDisposable
      {
        private:
          msclr::interop::marshal_context^ m_marshalContext;
          NativeHostedWebCore* m_pHostedWebCore;
        protected:
          !HostedWebCore();
        public:
          HostedWebCore(System::String^ hostedWebCoreLibraryPath, System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName);
          HostedWebCore(System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName);

          void Stop(System::Boolean immediate);

          ~HostedWebCore();
      };
    }
  }
}

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(System::String^ hostedWebCoreLibraryPath, System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName) : m_marshalContext(gcnew msclr::interop::marshal_context()), m_pHostedWebCore(new NativeHostedWebCore(m_marshalContext->marshal_as<PCWSTR>(hostedWebCoreLibraryPath), m_marshalContext->marshal_as<PCWSTR>(hostConfig), m_marshalContext->marshal_as<PCWSTR>(rootConfig), m_marshalContext->marshal_as<PCWSTR>(instanceName)))
{
  m_marshalContext->~marshal_context();
}

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName) : Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore(System::Environment::ExpandEnvironmentVariables("%windir%\\system32\\inetsrv\\hwebcore.dll"), hostConfig, rootConfig, instanceName)
{
}

void Sitecore::LiveTesting::IIS::HostedWebCore::Stop(System::Boolean immediate)
{
  if (m_pHostedWebCore != NULL)
  {
    m_pHostedWebCore->Stop(immediate);
    delete m_pHostedWebCore;

    m_pHostedWebCore = NULL;
  }
}

Sitecore::LiveTesting::IIS::HostedWebCore::~HostedWebCore()
{
  this->!HostedWebCore();
}

Sitecore::LiveTesting::IIS::HostedWebCore::!HostedWebCore()
{
  Stop(true);
}