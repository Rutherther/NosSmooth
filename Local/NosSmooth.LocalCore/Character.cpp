#include "Character.h"
#include <windows.h>
using namespace NosSmoothCore;
using namespace System;
using namespace System::Runtime::InteropServices;

Character::Character(ModuleHook moduleHook)
{
    CharacterUnmanaged::GetInstance()->Setup(moduleHook);
}

void Character::Walk(int x, int y)
{
    DWORD position = (y << 16) | x;
    CharacterUnmanaged::GetInstance()->Walk(position);
}

void Character::SetWalkCallback(WalkCallback^ walkCallback)
{
    _walkCallback = walkCallback;
    IntPtr functionPointer = Marshal::GetFunctionPointerForDelegate(walkCallback);
    CharacterUnmanaged::GetInstance()->SetWalkCallback(static_cast<NativeWalkCallback>(functionPointer.ToPointer()));
}

void NosSmoothCore::Character::ResetHooks()
{
    CharacterUnmanaged::GetInstance()->ResetHooks();
}
