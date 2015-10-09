#define WIN32_LEAN_AND_MEAN

#include <windows.h>

#pragma warning(push)
#pragma warning(disable: 6101)
#include <httpserv.h>
#pragma warning(pop)

#include "HttpServer.h"

IHttpServer* pGlobalHttpServer = NULL;

HRESULT RegisterModule(DWORD dwServerVersion, IHttpModuleRegistrationInfo* pModuleInfo, IHttpServer* pHttpServer)
{
  dwServerVersion; pModuleInfo;
  pGlobalHttpServer = pHttpServer;
  return S_OK;
}