#include "InitializationHandlerModule.h"

void Sitecore::LiveTesting::IIS::Requests::InitializationHandlerModule::OnBeginRequest(_In_ System::Object^ sender, _In_ System::EventArgs^ args)
{
  BeginRequest(sender, args);
}

void Sitecore::LiveTesting::IIS::Requests::InitializationHandlerModule::OnEndRequest(_In_ System::Object^ sender, _In_ System::EventArgs^ args)
{
  EndRequest(sender, args);
}

void Sitecore::LiveTesting::IIS::Requests::InitializationHandlerModule::Init(_In_ System::Web::HttpApplication^ application)
{
  m_application = application;
  
  m_application->BeginRequest += gcnew System::EventHandler(this, &InitializationHandlerModule::OnBeginRequest);
  m_application->EndRequest += gcnew System::EventHandler(this, &InitializationHandlerModule::OnEndRequest);
}

Sitecore::LiveTesting::IIS::Requests::InitializationHandlerModule::~InitializationHandlerModule()
{
  m_application->EndRequest -= gcnew System::EventHandler(this, &InitializationHandlerModule::OnEndRequest);
  m_application->BeginRequest -= gcnew System::EventHandler(this, &InitializationHandlerModule::OnBeginRequest);
}