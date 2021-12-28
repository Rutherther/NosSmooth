#include "NosSmoothCore.h"
using namespace NosSmoothCore;

NosClient::NosClient()
{
	ModuleHook _moduleHook = ModuleHook::CreateNostaleXDatModule();
	_character = gcnew Character(_moduleHook);
	_network = gcnew Network(_moduleHook);
}

NosClient::~NosClient()
{
	delete _network;
	delete _character;

	_network = nullptr;
	_character = nullptr;
}

Character^ NosClient::GetCharacter()
{
	return _character;
}

Network^ NosClient::GetNetwork()
{
	return _network;
}

void NosClient::ResetHooks() 
{
	_network->ResetHooks();
	_character->ResetHooks();
}