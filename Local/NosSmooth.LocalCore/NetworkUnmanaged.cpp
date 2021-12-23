#include "NetworkUnmanaged.h"
#include <detours.h>
#include <windows.h>
#include <chrono>
#include <iostream>

using namespace NosSmoothCore;

const BYTE SEND_PATTERN[] = { 0x53, 0x56, 0x8B, 0xF2, 0x8B, 0xD8, 0xEB, 0x04 };
const BYTE RECV_PATTERN[] = { 0x55, 0x8B, 0xEC, 0x83, 0xC4, 0xF4, 0x53, 0x56, 0x57, 0x33, 0xC9, 0x89, 0x4D, 0xF4, 0x89, 0x55, 0xFC, 0x8B, 0xD8, 0x8B, 0x45, 0xFC };
const BYTE PACKET_CALLER_PATTERN[] = { 0xA1, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x00, 0xBA, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x00, 0x00, 0x00, 0x00, 0xA1, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x00, 0x8B, 0x40, 0x40 };

LPCSTR SEND_MASK = "xxxxxxxx";
LPCSTR RECV_MASK = "xxxxxxxxxxxxxxxxxxxxxx";
LPCSTR PACKET_CALLER_MASK = "x????xxx????x????x????x????xxxxx";

NetworkUnmanaged::NetworkUnmanaged()
{
}

void PacketSendDetour()
{
    const char* packet = nullptr;

    __asm
    {
        pushad
        pushfd

        mov packet, edx
    }

    bool isAccepted = NetworkUnmanaged::GetInstance()->ExecuteSendCallback(packet);

    __asm
    {
        popfd
        popad
    }
    
    if (isAccepted) {
        NetworkUnmanaged::GetInstance()->SendPacket(packet);
    }
}

void PacketReceiveDetour()
{
    const char* packet = nullptr;

    __asm
    {
        pushad
        pushfd

        mov packet, edx
    }

    bool isAccepted = NetworkUnmanaged::GetInstance()->ExecuteReceiveCallback(packet);

    __asm
    {
        popfd
        popad
    }

    if (isAccepted) {
        NetworkUnmanaged::GetInstance()->ReceivePacket(packet);
    }
}

void NetworkUnmanaged::Setup(ModuleHook moduleHook)
{
    auto sendFunction = moduleHook.FindPattern(SEND_PATTERN, SEND_MASK);
    auto receiveFunction = moduleHook.FindPattern(RECV_PATTERN, RECV_MASK);
    auto callerObject = *reinterpret_cast<unsigned int*>((DWORD_PTR)moduleHook.FindPattern(PACKET_CALLER_PATTERN, PACKET_CALLER_MASK) + 1);

    if (sendFunction == 0)
    {
        throw "Could not find send packet function.";
    }

    if (receiveFunction == 0)
    {
        throw "Could not find receive packet function.";
    }

    if (callerObject == 0)
    {
        throw "Could not find packet caller object.";
    }

    _sendPacketAddress = sendFunction;
    _receivePacketAddress = receiveFunction;
    _callerObject = callerObject;

    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourAttach(&(PVOID&)_receivePacketAddress, PacketReceiveDetour);
    DetourAttach(&reinterpret_cast<void*&>(_sendPacketAddress), PacketSendDetour);
    DetourTransactionCommit();
}

void NetworkUnmanaged::SendPacket(const char *packet)
{
    __asm
    {
        mov esi, this
        mov eax, dword ptr ds : [esi]._callerObject
        mov eax, dword ptr ds : [eax]
        mov eax, dword ptr ds : [eax]
        mov edx, packet
        call[esi]._sendPacketAddress
    }
}

void NetworkUnmanaged::ReceivePacket(const char* packet)
{
    __asm
    {
        mov esi, this
        mov eax, dword ptr ds : [esi]._callerObject
        mov eax, dword ptr ds : [eax]
        mov eax, dword ptr ds : [eax]
        mov eax, dword ptr ds : [eax + 0x34]
        mov edx, packet
        call[esi]._receivePacketAddress
    }
}

void NetworkUnmanaged::ResetHooks()
{
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    if (_sendCallback != nullptr) {
        DetourDetach(&reinterpret_cast<void*&>(_sendPacketAddress), PacketSendDetour);
    }

    if (_receiveCallback != nullptr) {
    DetourDetach(&(PVOID&)_receivePacketAddress, PacketReceiveDetour);
    }

    DetourTransactionCommit();
}

void NetworkUnmanaged::SetReceiveCallback(PacketCallback callback)
{
    _receiveCallback = callback;
}

void NetworkUnmanaged::SetSendCallback(PacketCallback callback)
{
    _sendCallback = callback;
}

bool NetworkUnmanaged::ExecuteReceiveCallback(const char *packet)
{
    if (_receiveCallback != nullptr) {
        return _receiveCallback(packet);
    }

    return true;
}

bool NetworkUnmanaged::ExecuteSendCallback(const char* packet)
{
    if (_sendCallback != nullptr) {
        return _sendCallback(packet);
    }

    return true;
}