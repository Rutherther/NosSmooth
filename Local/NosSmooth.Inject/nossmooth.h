#pragma once
#include <Windows.h>

#pragma pack(push, 1)
struct LoadParams
{
    wchar_t *libraryPath;
    wchar_t *runtimeConfigPath;
    wchar_t *typePath;
    wchar_t *methodName;
};
#pragma pack(pop)
#define DllExport extern "C" __declspec( dllexport )

DllExport bool LoadAndCallMethod(LoadParams* params);