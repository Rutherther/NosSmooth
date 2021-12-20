#pragma once
#include "ModuleHook.h"

namespace NosSmoothCore
{
	public delegate bool NetworkCallback(System::String^ packet);
	typedef bool(__stdcall* PacketCallback)(const char* packet);

	class NetworkUnmanaged
	{
	public:
		void Setup(NosSmoothCore::ModuleHook moduleHook);

		/// <summary>
		/// Send the given packet to the server.
		/// </summary>
		/// <param name="packet">The packed to send.</param>
		void SendPacket(const char * packet);

		/// <summary>
		/// Receive the given packet on the client.
		/// </summary>
		/// <param name="packet">The packet to receive.</param>
		void ReceivePacket(const char * packet);

		/// <summary>
		/// Sets the receive callback delegate to be called when packet is received.
		/// </summary>
		/// <param name="callback"></param>
		void SetReceiveCallback(PacketCallback callback);

		/// <summary>
		/// Sets the send callback delegate to be called when the packet is sent.
		/// </summary>
		/// <param name="callback"></param>
		void SetSendCallback(PacketCallback callback);

		/// <summary>
		/// Resets all the function hooks.
		/// </summary>
		void ResetHooks();

		bool ExecuteSendCallback(const char *packet);

		bool ExecuteReceiveCallback(const char *packet);

		static NetworkUnmanaged* GetInstance()
		{
			static NetworkUnmanaged instance;
			return reinterpret_cast<NetworkUnmanaged*>(&instance);
		}
	private:
		NetworkUnmanaged();
		unsigned int _callerObject;
		unsigned int _receivePacketAddress;
		unsigned int _sendPacketAddress;

		PacketCallback _sendCallback;
		PacketCallback _receiveCallback;
	};
}

