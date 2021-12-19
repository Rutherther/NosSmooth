#pragma once
#include "ModuleHook.h"

namespace NosSmoothCore
{
	public ref class Character
	{
	public:
		/// <summary>
		/// Creates new instance of Character.
		/// </summary>
		/// <param name="moduleHook">The hooking module holding the information about NostaleX.dat</param>
		Character(NosSmoothCore::ModuleHook moduleHook);

		/// <summary>
		/// Starts walking to the specified x, y position
		/// </summary>
		/// <param name="x">The x coordinate to walk to.</param>
		/// <param name="y">The y coordinate to walk to.</param>
		void Walk(int x, int y);
	private:
		unsigned int _playerObject;
		unsigned int _walkFunction;
	};
}

