//
//  TcpConnectionHelper.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace NosSmooth.Pcap;

/// <summary>
/// A class for obtaining process tcp connections.
/// </summary>
/// <remarks>
/// Works on Windows only so far.
/// </remarks>
public static class TcpConnectionHelper
{
    private const int AF_INET = 2; // IP_v4 = System.Net.Sockets.AddressFamily.InterNetwork
    private const int AF_INET6 = 23; // IP_v6 = System.Net.Sockets.AddressFamily.InterNetworkV6

    /// <summary>
    /// Get TCP IPv4 connections of the specified processes.
    /// </summary>
    /// <param name="processIds">The process ids to look for.</param>
    /// <returns>Map from process ids to connecitons.</returns>
    /// <exception cref="NotImplementedException">Thrown if not windows.</exception>
    public static IReadOnlyDictionary<int, List<TcpConnection>> GetConnections(IReadOnlyList<int> processIds)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new NotImplementedException();
        }

        var result = new Dictionary<int, List<TcpConnection>>();
        var tcpv4Connections = GetAllTCPv4Connections();

        foreach (var connection in tcpv4Connections)
        {
            var process = processIds.FirstOrDefault(x => x == connection.OwningPid, -1);
            if (process != -1)
            {
                if (!result.ContainsKey(process))
                {
                    result.Add(process, new List<TcpConnection>());
                }

                result[process].Add
                (
                    new TcpConnection
                    (
                        connection.LocalAddr,
                        (ushort)(connection.LocalPort[1] | (connection.LocalPort[0] << 8)),
                        connection.RemoteAddr,
                        (ushort)(connection.RemotePort[1] | (connection.RemotePort[0] << 8))
                    )
                );
            }
        }

        return result;
    }

    private static List<MIB_TCPROW_OWNER_PID> GetAllTCPv4Connections()
    {
        return GetTCPConnections<MIB_TCPROW_OWNER_PID, MIB_TCPTABLE_OWNER_PID>(AF_INET);
    }

    private static List<MIB_TCP6ROW_OWNER_PID> GetAllTCPv6Connections()
    {
        return GetTCPConnections<MIB_TCP6ROW_OWNER_PID, MIB_TCP6TABLE_OWNER_PID>(AF_INET6);
    }

    private static List<TIPR> GetTCPConnections<TIPR, TIPT>(int ipVersion)
    {
        // IPR = Row Type, IPT = Table Type

        TIPR[] tableRows;
        int buffSize = 0;
        var dwNumEntriesField = typeof(TIPT).GetField("DwNumEntries");

        // how much memory do we need?
        uint ret = GetExtendedTcpTable
        (
            IntPtr.Zero,
            ref buffSize,
            true,
            ipVersion,
            TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL
        );
        IntPtr tcpTablePtr = Marshal.AllocHGlobal(buffSize);

        try
        {
            ret = GetExtendedTcpTable
            (
                tcpTablePtr,
                ref buffSize,
                true,
                ipVersion,
                TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL
            );
            if (ret != 0)
            {
                return new List<TIPR>();
            }

            // get the number of entries in the table
            TIPT table = (TIPT)Marshal.PtrToStructure(tcpTablePtr, typeof(TIPT))!;
            int rowStructSize = Marshal.SizeOf(typeof(TIPR));
            uint numEntries = (uint)dwNumEntriesField!.GetValue(table)!;

            // buffer we will be returning
            tableRows = new TIPR[numEntries];

            IntPtr rowPtr = (IntPtr)((long)tcpTablePtr + 4);
            for (int i = 0; i < numEntries; i++)
            {
                TIPR tcpRow = (TIPR)Marshal.PtrToStructure(rowPtr, typeof(TIPR))!;
                tableRows[i] = tcpRow;
                rowPtr = (IntPtr)((long)rowPtr + rowStructSize); // next entry
            }
        }
        finally
        {
            // Free the Memory
            Marshal.FreeHGlobal(tcpTablePtr);
        }
        return tableRows != null ? tableRows.ToList() : new List<TIPR>();
    }

    // https://msdn2.microsoft.com/en-us/library/aa366913.aspx
    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCPROW_OWNER_PID
    {
        public uint State;
        public uint LocalAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] LocalPort;
        public uint RemoteAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] RemotePort;
        public uint OwningPid;
    }

    // https://msdn2.microsoft.com/en-us/library/aa366921.aspx
    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCPTABLE_OWNER_PID
    {
        public uint DwNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public MIB_TCPROW_OWNER_PID[] Table;
    }

    // https://msdn.microsoft.com/en-us/library/aa366896
    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCP6ROW_OWNER_PID
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] LocalAddr;
        public uint LocalScopeId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] LocalPort;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] RemoteAddr;
        public uint RemoteScopeId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] RemotePort;
        public uint State;
        public uint OwningPid;
    }

    // https://msdn.microsoft.com/en-us/library/windows/desktop/aa366905
    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCP6TABLE_OWNER_PID
    {
        public uint DwNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public MIB_TCP6ROW_OWNER_PID[] Table;
    }

    [DllImport("iphlpapi.dll", SetLastError = true)]
    private static extern uint GetExtendedTcpTable
    (
        IntPtr pTcpTable,
        ref int dwOutBufLen,
        bool sort,
        int ipVersion,
        TCP_TABLE_CLASS tblClass,
        uint reserved = 0
    );

    private enum TCP_TABLE_CLASS
    {
        TCP_TABLE_BASIC_LISTENER,
        TCP_TABLE_BASIC_CONNECTIONS,
        TCP_TABLE_BASIC_ALL,
        TCP_TABLE_OWNER_PID_LISTENER,
        TCP_TABLE_OWNER_PID_CONNECTIONS,
        TCP_TABLE_OWNER_PID_ALL,
        TCP_TABLE_OWNER_MODULE_LISTENER,
        TCP_TABLE_OWNER_MODULE_CONNECTIONS,
        TCP_TABLE_OWNER_MODULE_ALL
    }
}