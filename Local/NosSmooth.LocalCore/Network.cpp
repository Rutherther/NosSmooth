#include "Network.h"
#include "NostaleString.h"

using namespace NosSmoothCore;
using namespace System::Runtime::InteropServices;
using namespace System;

Network::Network(ModuleHook moduleHook)
{
    _networkUnmanaged = NetworkUnmanaged::GetInstance();
    _networkUnmanaged->Setup(moduleHook);
}

void Network::ResetHooks()
{
    NetworkUnmanaged::GetInstance()->ResetHooks();
}

void Network::SendPacket(System::String^ packet)
{
    char* str = (char*)(System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(packet)).ToPointer();
	NetworkUnmanaged::GetInstance()->SendPacket(NostaleStringA(str).get());
}

void Network::ReceivePacket(System::String^ packet)
{
    char* str = (char*)(System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(packet)).ToPointer();
    NetworkUnmanaged::GetInstance()->ReceivePacket(NostaleStringA(str).get());
}

void Network::SetReceiveCallback(NetworkCallback^ callback)
{
    IntPtr functionPointer = Marshal::GetFunctionPointerForDelegate(callback);
    _receiveCallback = callback;
    NetworkUnmanaged::GetInstance()->SetReceiveCallback(static_cast<PacketCallback>(functionPointer.ToPointer()));
}

void Network::SetSendCallback(NetworkCallback^ callback)
{
    IntPtr functionPointer = Marshal::GetFunctionPointerForDelegate(callback);
    _sendCallback = callback;
    NetworkUnmanaged::GetInstance()->SetSendCallback(static_cast<PacketCallback>(functionPointer.ToPointer()));
}