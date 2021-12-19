#include "Character.h"
using namespace NosSmoothCore;

const BYTE WALK_OBJECT_PATTERN[] = { 0x33, 0xC9, 0x8B, 0x55, 0xFC, 0xA1, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00 };
const BYTE WALK_FUNCTION_PATTERN[] = { 0x55, 0x8B, 0xEC, 0x83, 0xC4, 0xEC, 0x53, 0x56, 0x57, 0x66, 0x89, 0x4D, 0xFA };

LPCSTR WALK_OBJECT_MASK = "xxxxxx????x????";
LPCSTR WALK_FUNCTION_MASK = "xxxxxxxxxxxxx";

Character::Character(ModuleHook moduleHook)
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

    _walkFunction = walkFunction;
    _playerObject = walkObject;
}

void CallWalk(int x, int y, unsigned int playerObject, unsigned int walkFunction)
{
    DWORD position = (y << 16) | x;

    __asm
    {
        push 1
        xor ecx, ecx
        mov edx, position
        mov eax, dword ptr ds : [playerObject]
        mov eax, dword ptr ds : [eax]
        call walkFunction
    }
}

void Character::Walk(int x, int y)
{
    CallWalk(x, y, _playerObject, _walkFunction);
}