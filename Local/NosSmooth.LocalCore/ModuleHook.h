#pragma once
#include <windows.h>

namespace NosSmoothCore
{
	class ModuleHook
	{
	public:
		static ModuleHook CreateNostaleXDatModule();
		ModuleHook(unsigned int baseAddress, unsigned int moduleSize);

		unsigned int GetModuleBaseAddress();
		unsigned int FindPattern(const BYTE* lpPattern, LPCSTR szMask);
	private:
		unsigned int _baseAddress;
		unsigned int _moduleSize;
	};
}

