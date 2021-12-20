#pragma once
#include "ModuleHook.h"
#include "NetworkUnmanaged.h"

namespace NosSmoothCore
{
	public ref class Network
	{
	public:
		Network(NosSmoothCore::ModuleHook moduleHook);

		/// <summary>
		/// Send the given packet to the server.
		/// </summary>
		/// <param name="packet">The packed to send.</param>
		void SendPacket(System::String^ packet);
		
		/// <summary>
		/// Receive the given packet on the client.
		/// </summary>
		/// <param name="packet">The packet to receive.</param>
		void ReceivePacket(System::String^ packet);

		/// <summary>
		/// Sets the receive callback delegate to be called when packet is received.
		/// </summary>
		/// <param name="callback"></param>
		void SetReceiveCallback(NetworkCallback^ callback);

		/// <summary>
		/// Sets the send callback delegate to be called when the packet is sent.
		/// </summary>
		/// <param name="callback"></param>
		void SetSendCallback(NetworkCallback^ callback);

		/// <summary>
		/// Resets all the function hooks.
		/// </summary>
		void ResetHooks();
	private:
		NetworkUnmanaged* _networkUnmanaged;
	};
}

