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
          NativeHostedWebCore* m_pHostedWebCore;
          msclr::interop::marshal_context^ m_marshalContext;

          bool m_disposedUnmanagedResources;
          bool m_disposedManagedResources;
        protected:
          !HostedWebCore();
        public:
          HostedWebCore();

          void Start(System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName);
          void Stop(System::Boolean immediate);

          ~HostedWebCore();
      };
    }
  }
}

Sitecore::LiveTesting::IIS::HostedWebCore::HostedWebCore() : m_pHostedWebCore(new NativeHostedWebCore()), m_marshalContext(gcnew msclr::interop::marshal_context())
{
}

void Sitecore::LiveTesting::IIS::HostedWebCore::Start(System::String^ hostConfig, System::String^ rootConfig, System::String^ instanceName)
{
  m_pHostedWebCore->Start(m_marshalContext->marshal_as<PCWSTR>(hostConfig), m_marshalContext->marshal_as<PCWSTR>(rootConfig), m_marshalContext->marshal_as<PCWSTR>(instanceName));
}

void Sitecore::LiveTesting::IIS::HostedWebCore::Stop(System::Boolean immediate)
{
  m_pHostedWebCore->Stop(immediate);
}

Sitecore::LiveTesting::IIS::HostedWebCore::~HostedWebCore()
{
  if (!m_disposedManagedResources) {
    m_marshalContext->~marshal_context();
    m_disposedManagedResources = true;
  }

  this->!HostedWebCore();
}

Sitecore::LiveTesting::IIS::HostedWebCore::!HostedWebCore()
{
  if (!m_disposedUnmanagedResources)
  {
    delete m_pHostedWebCore;
    m_disposedUnmanagedResources = true;
  }
}