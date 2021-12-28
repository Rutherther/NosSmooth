#pragma once
#include "ModuleHook.h"

namespace NosSmoothCore
{
	public delegate bool WalkCallback(int position);
	typedef bool(__stdcall* NativeWalkCallback)(int position);

	class CharacterUnmanaged
	{
	public:
		/// <summary>
		/// Set ups the addresses of objects.
		/// </summary>
		/// <param name="moduleHook">The hooking module holding the information about NostaleX.dat</param>
		void Setup(NosSmoothCore::ModuleHook moduleHook);

		/// <summary>
		/// Starts walking to the specified x, y position
		/// </summary>
		/// <param name="x">The coordinate to walk to.</param>
		void Walk(DWORD position);

		/// <summary>
		/// Registers the callback for walk function.
		/// </summary>
		/// <param name="walkCallback">The callback to call.</param>
		void SetWalkCallback(NativeWalkCallback walkCallback);

		/// <summary>
		/// Reset the registered hooks.
		/// </summary>
		void ResetHooks();

		/// <summary>
		/// Executes the walk callback.
		/// </summary>
		/// <param name="position">The coordinate the user wants to walk to.</param>
		/// <returns>Whether to accept the walk.</returns>
		bool ExecuteWalkCallback(const DWORD position);

		static CharacterUnmanaged* GetInstance()
		{
			static CharacterUnmanaged instance;
			return reinterpret_cast<CharacterUnmanaged*>(&instance);
		}
		unsigned int _walkFunctionAddress;
		unsigned int _characterObjectAddress;
	private:
		CharacterUnmanaged();


		NativeWalkCallback _walkCallback;
	};
}

