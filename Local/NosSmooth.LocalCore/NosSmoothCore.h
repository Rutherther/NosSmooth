#pragma once
#using <mscorlib.dll> // to use Console::WriteLine

#include "Character.h"
#include "ModuleHook.h"
#include "Network.h"
#include "NostaleString.h"
#include <stdio.h> // to printf()

namespace NosSmoothCore
{
	public ref class NosClient
	{
	public:
		NosClient();
		~NosClient();
		Character^ GetCharacter();
		Network^ GetNetwork();
		void ResetHooks();
	private:
		Network^ _network;
		Character^ _character;
	};
}