/* 
 * File: networking.cs
 *
 * This source file contains the declaration of the NetworkingFunctionSet class which 
 * contains the few networking functions.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Engine.Networking;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Engine.ScriptingFunctions
{
    /// <summary>
    ///     Wraps a network connection object used in a script.
    /// </summary>
    public class NetworkConnectionScriptObject : NativeObject
    {
        public NetworkConnectionScriptObject(NetworkConnection connection)
        {
            _nativeObject = connection;
        }

        public NetworkConnectionScriptObject() { }
    }

    /// <summary>
    ///     Wraps a network client object used in a script.
    /// </summary>
    public class NetworkClientScriptObject : NativeObject
    {
        public NetworkClientScriptObject(NetworkClient client)
        {
            _nativeObject = client;
        }

        public NetworkClientScriptObject() { }
    }

    /// <summary>
    ///     Wraps a network packet object used in a script.
    /// </summary>
    public class NetworkPacketScriptObject : NativeObject
    {
        public NetworkPacketScriptObject(NetworkPacket packet)
        {
            _nativeObject = packet;
        }

        public NetworkPacketScriptObject() { }
    }

    /// <summary>
    ///		Contains numerous native functions that are commonly used in scripts.
    ///		None are commented as they are all fairly self-explanatory.
    /// </summary>
    public class NetworkFunctionSet : NativeFunctionSet
    {
        [NativeFunctionInfo("ConnectToServer", "bool", "string,int")]
        public void ConnectToServer(ScriptThread thread)
        {
            Networking.NetworkManager.ServerIP = thread.GetStringParameter(0);
            if (thread.GetIntegerParameter(1) != -1) Networking.NetworkManager.Port = thread.GetIntegerParameter(1);
            thread.SetReturnValue(Networking.NetworkManager.Start());
        }

        [NativeFunctionInfo("IsServer", "bool", "")]
        public void IsServer(ScriptThread thread)
        {
            thread.SetReturnValue(Networking.NetworkManager.IsServer);
        }

        [NativeFunctionInfo("ConnectionResult", "int", "")]
        public void ConnectionResult(ScriptThread thread)
        {
            thread.SetReturnValue((int)Networking.NetworkManager.ConnectionResult);
        }

        [NativeFunctionInfo("CloseConnection", "void", "")]
        public void CloseConnection(ScriptThread thread)
        {
            Networking.NetworkManager.Finish();
        }

        [NativeFunctionInfo("ClientID", "int", "")]
        public void ClientID(ScriptThread thread)
        {
            thread.SetReturnValue((int)Networking.NetworkManager.ID);
        }

        [NativeFunctionInfo("SetPacketID", "void", "object,int")]
        public void SetPacketID(ScriptThread thread)
        {
            NetworkPacket packet = ((NativeObject)thread.GetObjectParameter(0)).Object as NetworkPacket;
            if (packet == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetPacketID with an invalid object.", LogAlertLevel.Error);
                return;
            }
            packet.ID = thread.GetIntegerParameter(0);
        }

        [NativeFunctionInfo("PacketID", "int", "object")]
        public void PacketID(ScriptThread thread)
        {
            NetworkPacket packet = ((NativeObject)thread.GetObjectParameter(0)).Object as NetworkPacket;
            if (packet == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called PacketID with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(packet.ID);
        }


    }
}