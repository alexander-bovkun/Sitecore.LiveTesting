#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <httpserv.h>

HRESULT RegisterModule(DWORD dwServerVersion, IHttpModuleRegistrationInfo* pModuleInfo, IHttpServer* pHttpServer)
{
  dwServerVersion; pModuleInfo; pHttpServer;
  return S_OK;
}