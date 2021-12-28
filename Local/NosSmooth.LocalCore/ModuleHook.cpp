#include "ModuleHook.h"
#include <psapi.h>
using namespace NosSmoothCore;

ModuleHook ModuleHook::CreateNostaleXDatModule()
{
	auto moduleHandle = GetModuleHandleA(nullptr);
	if (moduleHandle == nullptr)
	{
		throw "Could not obtain NostaleX.dat module handle";
	}

	MODULEINFO moduleInfo = {};
	if (!GetModuleInformation(GetCurrentProcess(), moduleHandle, &moduleInfo, sizeof(moduleInfo)))
	{
		throw "Could not get module handle information";
	}
	unsigned int moduleBase = reinterpret_cast<unsigned int>(moduleInfo.lpBaseOfDll);
	unsigned int moduleSize = moduleInfo.SizeOfImage;

	return ModuleHook(moduleBase, moduleSize);
}

ModuleHook::ModuleHook(unsigned int baseAddress, unsigned int moduleSize)
	: _baseAddress(baseAddress), _moduleSize(moduleSize)
{
}

unsigned int ModuleHook::FindPattern(const BYTE* lpPattern, LPCSTR szMask)
{
	DWORD dwLength = strlen(szMask);
	DWORD dwImageEnd = _baseAddress + _moduleSize - dwLength;
	DWORD_PTR i, j;

	// Scan the whole image for the pattern
	for (j = _baseAddress; j < dwImageEnd; ++j)
	{
		// If the pattern is found, return the address at which it begins
		for (i = 0; i < dwLength && (szMask[i] == '?' || *(BYTE*)(j + i) == lpPattern[i]); ++i);
		if (i == dwLength) return j;
	}

	return NULL;
}

unsigned int ModuleHook::GetModuleBaseAddress()
{
	return _baseAddress;
}