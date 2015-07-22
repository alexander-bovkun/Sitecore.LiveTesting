#define WIN32_LEAN_AND_MEAN

#include "windows.h"
#include "hwebcore.h"

__declspec(dllexport) int StartHostedWebCore();
__declspec(dllexport) int StopHostedWebCore();