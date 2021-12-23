#include "CharacterUnmanaged.h"
#include <detours.h>

using namespace NosSmoothCore;

const BYTE WALK_OBJECT_PATTERN[] = { 0x33, 0xC9, 0x8B, 0x55, 0xFC, 0xA1, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00 };
const BYTE WALK_FUNCTION_PATTERN[] = { 0x55, 0x8B, 0xEC, 0x83, 0xC4, 0xEC, 0x53, 0x56, 0x57, 0x66, 0x89, 0x4D, 0xFA };

LPCSTR WALK_OBJECT_MASK = "xxxxxx????x????";
LPCSTR WALK_FUNCTION_MASK = "xxxxxxxxxxxxx";

void CharacterWalkDetourIn()
{
    DWORD position = 0;

    __asm
    {
        pushad
        pushfd

        mov position, edx
    }

    bool isAccepted = CharacterUnmanaged::GetInstance()->ExecuteWalkCallback(position);

    __asm
    {
        popfd
        popad
    }

    if (isAccepted) {
        CharacterUnmanaged::GetInstance()->Walk(position);
    }
}

// Detour entrypoint
// declspec naked to not mess up the stack
void __declspec(naked) CharacterWalkDetour()
{
    unsigned int returnPush;
    __asm {
        pop eax
        pop ebx
        mov returnPush, eax // we have to push this value on the stack before returning
    }

    CharacterWalkDetourIn();

    __asm {
        push returnPush
        ret
    }
}

CharacterUnmanaged::CharacterUnmanaged()
{
}

void CharacterUnmanaged::Setup(ModuleHook moduleHook)
{
    auto walkFunction = moduleHook.FindPattern(WALK_FUNCTION_PATTERN, WALK_FUNCTION_MASK);
    auto walkObject = *(unsigned int*)(moduleHook.FindPattern(WALK_OBJECT_PATTERN, WALK_OBJECT_MASK) + 0x6);

    if (walkFunction == 0)
    {
        throw "Could not find walk function.";
    }

    if (walkObject == 0)
    {
        throw "Could not find player object.";
    }

    _walkFunctionAddress = walkFunction;
    _characterObjectAddress = walkObject;
}

void CharacterUnmanaged::SetWalkCallback(NativeWalkCallback walkCallback)
{
    _walkCallback = walkCallback;
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourAttach(&(PVOID&)_walkFunctionAddress, CharacterWalkDetour);
    DetourTransactionCommit();
}

void CharacterUnmanaged::Walk(DWORD position)
{
    unsigned int walkFunction = _walkFunctionAddress;
    unsigned int characterObject = _characterObjectAddress;

    __asm
    {
        push 1
        xor ecx, ecx
        mov edx, position
        mov eax, dword ptr ds : [characterObject]
        mov eax, dword ptr ds : [eax]
        call walkFunction
    }
}

void CharacterUnmanaged::ResetHooks()
{
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourDetach(&(PVOID&)_walkFunctionAddress, CharacterWalkDetour);
    DetourTransactionCommit();
}

bool CharacterUnmanaged::ExecuteWalkCallback(const DWORD position)
{
    if (_walkCallback != nullptr) {
        return _walkCallback(position);
    }

    return true;
}