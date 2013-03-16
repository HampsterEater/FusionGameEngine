/* 
 * File: Manager.cs
 *
 * This source file contains the declaration of the NetworkManager class which 
 * contains several things used to do with networking multiplayer games.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using System.Collections;
using System.Security.Cryptography;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Events;
//using BinaryPhoenix.Fusion.Graphics.Direct3D9Driver;

namespace BinaryPhoenix.Fusion.Engine.Networking
{

    /// <summary>
    ///		Used by some of the network classes to notify of packet reception.
    /// </summary>
    /// <param name="packet">Packet that has been recieved.</param>
    /// <param name="connection">Connection that this event refers to.</param>
    public delegate void PacketRecievedEventHandler(object sender, NetworkPacket packet);

    /// <summary>
    ///		Used by some of the network classes to notify of client connections.
    /// </summary>
    /// <param name="client">Client that has connected.</param>
    /// <param name="connection">Connection that this event refers to.</param>
    public delegate void ClientConnectedEventHandler(object sender, NetworkClient client);

    /// <summary>
    ///		Used by some of the network classes to notify of client disconnections. 
    /// </summary>
    /// <param name="client">Client that has disconnected.</param>
    /// <param name="connection">Connection that this event refers to.</param>
    public delegate void ClientDisconnectedEventHandler(object sender, NetworkClient client);

    /// <summary>
    ///		Used by some of the network classes to notify of a sever disconnection.
    /// </summary>
    /// <param name="connection">Connection that this event refers to.</param>
    public delegate void DisconnectedEventHandler(object sender);

    /// <summary>
    ///     Determines what level of access a network account has.
    /// </summary>
    public enum NetworkAccountPosition
    {
        Administrator = 0,
        Moderator = 1,
        Member = 2
    }

    /// <summary>
    ///     This is used to store details on network accounts. 
    /// </summary>
    public class NetworkAccount
    {
        #region Members
        #region Variables

        private string _lastKnownIP = "";
        private DateTime _dateRegistered = DateTime.Now;
        private DateTime _lastActive = DateTime.Now;
        private string _username = ""; 
        private string _passwordHash = "", _passwordSalt = "";
        private NetworkAccountPosition _position = NetworkAccountPosition.Member;

        private Hashtable _customSettings = new Hashtable();

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the hash table storing this accounts custom settings.
        /// </summary>
        public Hashtable CustomSettings
        {
            get { return _customSettings; }
            set { _customSettings = value; }
        }

        /// <summary>
        ///     Gets or sets the last known IP used to log into this account.
        /// </summary>
        public string LastKnownIP
        {
            get { return _lastKnownIP; }
            set { _lastKnownIP = value; }
        }

        /// <summary>
        ///     Gets or sets the date this account was registered.
        /// </summary>
        public DateTime DateRegistered
        {
            get { return _dateRegistered; }
            set { _dateRegistered = value; }
        }

        /// <summary>
        ///     Gets or sets the date this account was last active.
        /// </summary>
        public DateTime LastActive
        {
            get { return _lastActive; }
            set { _lastActive = value; }
        }

        /// <summary>
        ///     Gets or sets the username of this account.
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        /// <summary>
        ///     Gets or sets the password hash of this account. 
        /// </summary>
        public string PasswordHash
        {
            get { return _passwordHash; }
            set { _passwordHash = value; }
        }

        /// <summary>
        ///     Gets or sets the password salt of this account.
        /// </summary>
        public string PasswordSalt
        {
            get { return _passwordSalt; }
            set { _passwordSalt = value; }
        }

        /// <summary>
        ///     Gets or sets the access position this account has.
        /// </summary>
        public NetworkAccountPosition Position
        {
            get { return _position; }
            set { _position = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Takes a password and generates a hash for it. Allows password
        ///     to be stored more securely.
        /// </summary>
        /// <param name="password">Password to generate hash for.</param>
        private void GeneratePasswordHash(string password)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            byte[] saltBytes = new byte[MathMethods.Random(8, 16)];
            rng.GetNonZeroBytes(saltBytes);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordAndSaltBytes = new byte[saltBytes.Length + passwordBytes.Length];
            Array.Copy(passwordBytes, 0, passwordAndSaltBytes, 0, passwordBytes.Length);
            Array.Copy(saltBytes, 0, passwordAndSaltBytes, passwordBytes.Length, saltBytes.Length);

            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
            byte[] hashBytes = hashProvider.ComputeHash(passwordAndSaltBytes);
            string hashValue = Convert.ToBase64String(hashBytes);

            _passwordSalt = Convert.ToBase64String(saltBytes);
            _passwordHash = hashValue;
        }

        /// <summary>
        ///     Compares a password against the one for this account and returns true if they match.
        /// </summary>
        /// <param name="password">Password to compare.</param>
        /// <returns>True if passwords match.</returns>
        private bool ValidatePassword(string password)
        {
            byte[] saltBytes = Convert.FromBase64String(_passwordSalt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordAndSaltBytes = new byte[saltBytes.Length + passwordBytes.Length];
            Array.Copy(passwordBytes, 0, passwordAndSaltBytes, 0, passwordBytes.Length);
            Array.Copy(saltBytes, 0, passwordAndSaltBytes, passwordBytes.Length, saltBytes.Length);

            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
            byte[] hashBytes = hashProvider.ComputeHash(passwordAndSaltBytes);
            string hashValue = Convert.ToBase64String(hashBytes);

            return (_passwordHash == hashValue);
        }

        #endregion
    }

    /// <summary>
    ///     This is used to store details on a packet that can be sent or recieved from a client / server.
    /// </summary>
    public class NetworkPacket
    {
        #region Members
        #region Variables

        public const int HEADER_SIZE = 8;

        private int _id = 0;

        private object[] _dataBlocks = new object[255];
        private bool[] _dataBlocksModified = new bool[255];

        private int _dataLength = 0;

        private int _clientID = 0;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the ID of the client this packet was recieved from.
        /// </summary>
        public int ClientID
        {
            get { return _clientID; }
            set { _clientID = value;}
        }

        /// <summary>
        ///     Gets or sets the ID of this message.
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        ///     Returns true if this message has been modified and needs to be resent.
        /// </summary>
        public bool Modified
        {
            get 
            {
                for (int i = 0; i < _dataBlocksModified.Length; i++)
                    if (_dataBlocksModified[i] == true) return true;
                return false;
            }
        }

        /// <summary>
        ///     Returns the data length of the message currently being decoded.
        /// </summary>
        public int DataLength
        {
            get
            {
                return _dataLength;
            }
        }

        /// <summary>
        ///     Gets or sets the data blocks stored in this message.
        /// </summary>
        public object this[int index] 
        {
            get { return _dataBlocks[index]; }
            set 
            {
                _dataBlocksModified[index] = (_dataBlocks[index] != value);
                _dataBlocks[index] = value;
            }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Encodes this packet into a buffer of bytes.
        /// </summary>
        /// <returns>Encoded version of packet.</returns>
        public byte[] Encode()
        {
            // Encode seperate data blocks into byte array.
            MemoryStream dataStream = new MemoryStream();
            BinaryWriter dataWriter = new BinaryWriter(dataStream);

            int dataBlockCount = 0;
            for (int i = 0; i < _dataBlocks.Length; i++)
            {
                if (_dataBlocks[i] != null && _dataBlocksModified[i] == true)
                {
                    #region Data Encoding
                    dataWriter.Write((byte)i);
                    if (_dataBlocks[i] is byte)
                    {
                        dataWriter.Write((byte)1);
                        dataWriter.Write((byte)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is sbyte)
                    {
                        dataWriter.Write((byte)2);
                        dataWriter.Write((sbyte)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is short)
                    {
                        dataWriter.Write((byte)3);
                        dataWriter.Write((short)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is ushort)
                    {
                        dataWriter.Write((byte)4);
                        dataWriter.Write((ushort)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is int)
                    {
                        dataWriter.Write((byte)5);
                        dataWriter.Write((int)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is uint)
                    {
                        dataWriter.Write((byte)6);
                        dataWriter.Write((uint)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is long)
                    {
                        dataWriter.Write((byte)7);
                        dataWriter.Write((long)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is ulong)
                    {
                        dataWriter.Write((byte)8);
                        dataWriter.Write((ulong)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is float)
                    {
                        dataWriter.Write((byte)9);
                        dataWriter.Write((float)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is double)
                    {
                        dataWriter.Write((byte)10);
                        dataWriter.Write((double)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is string)
                    {
                        dataWriter.Write((byte)11);
                        dataWriter.Write((string)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is bool)
                    {
                        dataWriter.Write((byte)12);
                        dataWriter.Write((bool)_dataBlocks[i]);
                    }
                    else if (_dataBlocks[i] is byte[])
                    {
                        dataWriter.Write((byte)13);
                        dataWriter.Write(((byte[])_dataBlocks[i]).Length);
                        dataWriter.Write((byte[])_dataBlocks[i]);
                    }
                    #endregion
                    dataBlockCount++;
                }
            }
            
            dataWriter.Close();
            dataStream.Close();

            byte[] dataBlock = dataStream.ToArray();

            // Encode data and header into a single array.
            byte[] encodedPacket = new byte[HEADER_SIZE + dataBlock.Length];
            MemoryStream stream = new MemoryStream(encodedPacket);
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(_id);
            writer.Write(dataBlock.Length);
            if (dataBlock.Length != 0)
                writer.Write(dataBlock);

            writer.Close();
            stream.Close();

            return encodedPacket;
        }

        /// <summary>
        ///     Decodes this packet from a buffer of bytes.
        ///     If data is only the length of the header then only header information
        ///     will be decoded.
        /// </summary>
        /// <param name="data">Buffer to decode packet from.</param>
        /// <param name="hasData">If set then the buffer has a header.</param>
        /// <param name="hasHeader">If set then the buffer has a data block.</param>
        public void Decode(byte[] data, bool hasHeader, bool hasData)
        {
            if (hasHeader == true)
            {
                // Decode ID (32 bit)
                _id = BitConverter.ToInt32(data, 0);

                // Decode Data Length (32 bit)
                _dataLength = BitConverter.ToInt32(data, 4);
            }

            // Decode the data.
            if (hasData == true)
            {
                for (int i = 0; i < _dataBlocksModified.Length; i++)
                    _dataBlocksModified[i] = false;

                MemoryStream stream = new MemoryStream(data, (hasHeader == true ? HEADER_SIZE : 0), data.Length - (hasHeader == true ? HEADER_SIZE : 0));
                BinaryReader reader = new BinaryReader(stream);

                // Read each block until end of stream.
                while (stream.Position < stream.Length)
                {
                    int blockIndex = stream.ReadByte();
                    int dataType = stream.ReadByte();
                    switch (dataType)
                    {
                        case 1: this[blockIndex] = reader.ReadByte(); break; // byte
                        case 2: this[blockIndex] = reader.ReadSByte(); break; // sbyte
                        case 3: this[blockIndex] = reader.ReadInt16(); break; // short
                        case 4: this[blockIndex] = reader.ReadUInt16(); break; // ushort
                        case 5: this[blockIndex] = reader.ReadInt32(); break; // int
                        case 6: this[blockIndex] = reader.ReadUInt32(); break; // uint
                        case 7: this[blockIndex] = reader.ReadInt64(); break; // long
                        case 8: this[blockIndex] = reader.ReadUInt64(); break; // ulong
                        case 9: this[blockIndex] = reader.ReadSingle(); break; // float
                        case 10: this[blockIndex] = reader.ReadDouble(); break; // double
                        case 11: this[blockIndex] = reader.ReadString(); break; // string
                        case 12: this[blockIndex] = reader.ReadBoolean(); break; // bool
                        case 13: this[blockIndex] = reader.ReadBytes(reader.ReadInt32()); break; // byte[]
                    }
                }

                reader.Close();
                stream.Close();
            }
        }

        #endregion
    }

    /// <summary>
    ///     Wraps some functionality of a socket connection. 
    /// </summary>
    public class NetworkConnection
    {
        #region Members
        #region Events

        public event PacketRecievedEventHandler PacketRecieved;
        public event ClientConnectedEventHandler ClientConnected;
        public event ClientDisconnectedEventHandler ClientDisconnected;
        public event DisconnectedEventHandler Disconnected;

        #endregion
        #region Variables

        private static int _globalBytesSent = 0, _globalBytesRecieved;
        private Socket _socket = null;
        private int _bytesSent = 0, _bytesRecieved = 0;

        private ArrayList _clientList = new ArrayList();
        private ArrayList _newClientList = new ArrayList();

        private NetworkPacket _recievedPacket = null;
        private NetworkPacket _asyncRecievePacket = null;
        private int _asyncRecievePacketID = 0;
        private byte[] _recieveBuffer = new byte[NetworkPacket.HEADER_SIZE];

        private bool _isConnected = false;
        private bool _isListening = false;

        private HighPreformanceTimer _pingDelayTimer = new HighPreformanceTimer();
        private HighPreformanceTimer _pingTimer = new HighPreformanceTimer();

        private bool _waitingForPong = false;
        private float _ping = 0;
        private int _pingAverageCount = 0;
        private float _pingAverageCounter = 0;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the global amount of bytes sent through any connection.
        /// </summary>
        public static int GlobalBytesSent
        {
            get { return _globalBytesSent; }
            set { _globalBytesSent = value; }
        }

        /// <summary>
        ///     Gets or sets the global amount of bytes recieved by any connection.
        /// </summary>
        public static int GlobalBytesRecieved
        {
            get { return _globalBytesRecieved; }
            set { _globalBytesRecieved = value; }
        }

        /// <summary>
        ///     Gets or sets the socket through which data is being sent down this connection.
        /// </summary>
        public Socket Socket
        {
            get { return _socket; }
            set { _socket = value; }
        }

        /// <summary>
        ///     Gets or sets the amount of bytes sent through this connection.
        /// </summary>
        public int BytesSent
        {
            get { return _bytesSent; }
            set { _bytesSent = value; }
        }

        /// <summary>
        ///     Gets or sets the amount of bytes recived through this connection.
        /// </summary>
        public int BytesRecieved
        {
            get { return _bytesRecieved; }
            set { _bytesRecieved = value; }
        }

        /// <summary>
        ///     Gets or sets the client list of this network connection.
        /// </summary>
        public ArrayList ClientList
        {
            get 
            {
                return _clientList;
            }
        }

        /// <summary>
        ///     Gets or sets if this connection is connected.
        /// </summary>
        public bool Connected
        {
            get { return _isConnected; }
        }

        /// <summary>
        ///     Gets the current ping rate (in milliseconds) of this connection.
        /// </summary>
        public float Ping
        {
            get { return _ping; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Connects this connection.
        /// </summary>
        /// <param name="listen">If set to true, this connection will work as a server and listen for new connections, else it connect to a server.</param>
        /// <param name="port">Port to connect to.</param>
        /// <param name="ip">Server IP to connect to.</param>
        /// <returns>True if connection was successfull.</returns>
        public bool Connect(string ip, int port, bool listen)
        {
            _isListening = listen;
            try
            {
                if (listen == true)
                {
                    // Set this connection up as a server.
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    _socket.Bind(new IPEndPoint(IPAddress.Any, port));
                    _socket.Listen(64);
                    _socket.BeginAccept(new AsyncCallback(OnClientConnect), null);
                    _isConnected = true;
                }
                else
                {
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    _socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                    BeginRecieving(NetworkPacket.HEADER_SIZE);
                    _isConnected = true;
                }
            }
            catch (Exception)
            {
                _isConnected = false;
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Closes the current connection.
        /// </summary>
        public void Close()
        {
            if (_socket.Connected == true) 
            {
                _socket.Close();
                _isConnected = false;
                if (Disconnected != null) Disconnected(this);
            }
            lock (_clientList)
            {
                _clientList.Clear();
            }
            _socket.Close();
            _socket = null;
        }

        /// <summary>
        ///     Updates this connection.
        /// </summary>
        public void Poll()
        {
            // Any clients disconnected?
            lock (_clientList)
            {
                ArrayList disconnectedList = new ArrayList();
                foreach (NetworkClient client in _clientList)
                    if (client.Connection.Connected == false)
                        disconnectedList.Add(client);
                foreach (NetworkClient client in disconnectedList)
                {
                    _clientList.Remove(client); // Well kiss my arse to.
                    if (ClientDisconnected != null) ClientDisconnected(this, client);
                    DebugLogger.WriteLog("Client, id " + client.ID + ", was disconnected.");
                }
            }

            // Have we disconnected?
            if (_socket.Connected == false && _isListening == false && _isConnected == true)
            {
                if (Disconnected != null) 
                    Disconnected(this);
                _isConnected = false;
            }

            if (_isConnected == true)
            {
                // Only bother with pings if we are not listening.
                if (_isListening == false)
                {
                    // Not recieved a pong in the last (PingDelay * 2)? Something must have gone wrong, lets resend the packet. 
                    if (_pingTimer.DurationMillisecond > 60000 && _waitingForPong == true)
                        _waitingForPong = false;

                    // Time to sent a ping packet to this client?
                    if (_pingDelayTimer.DurationMillisecond > (NetworkManager.PingDelay * 2) && _waitingForPong == false)
                    {
                        _pingTimer.Restart();
                        _pingDelayTimer.Restart();
                        _waitingForPong = true;

                        // Lets send a pong message to the server.
                        NetworkPacket pingPacket = new NetworkPacket();
                        pingPacket.ID = "PING".GetHashCode();
                        SendPacket(pingPacket);
                    }
                }
            }
        }

        /// <summary>
        ///     Invoked when a client attempts to connect to this server.
        /// </summary>
        /// <param name="asyn">Asyncronous state.</param>
        private void OnClientConnect(IAsyncResult asyn)
        {
            if (_socket == null || (_socket.Connected == false && _isListening == false && _isConnected == true))
                return;

            lock (_clientList)
            {
                try
                {
                    Socket clientSocket = _socket.EndAccept(asyn);
                    NetworkConnection clientConnection = new NetworkConnection(clientSocket); 

                    // To many clients?
                    if (_clientList.Count >= NetworkManager.MaximumClients)
                    {
                        DebugLogger.WriteLog("Denied connection from " + clientSocket.RemoteEndPoint.ToString() + ", maximum clients already connected.");

                        NetworkPacket banPacket = new NetworkPacket();
                        banPacket.ID = "CONNECTION_RESULT".GetHashCode();
                        banPacket[0] = (byte)2;
                        banPacket[1] = (int)0;
                        clientConnection.SendPacket(banPacket);

                        clientSocket.Close();
                        return;
                    }

                    // Is this IP banned?
                    bool banned = false;
                    string currentIP = clientSocket.RemoteEndPoint.ToString();
                    if (currentIP.IndexOf(":") >= 0) currentIP = currentIP.Substring(0, currentIP.IndexOf(":"));
                    foreach (NetworkBan ban in NetworkManager.BansList)
                    {
                        if (ban.IsIPBanned(currentIP.ToLower()))
                        {
                            banned = true;
                            break;
                        }
                    }
                    if (banned == true)
                    {
                        DebugLogger.WriteLog("Denied connection from " + clientSocket.RemoteEndPoint.ToString() + ", IP address is banned.");

                        NetworkPacket banPacket = new NetworkPacket();
                        banPacket.ID = "CONNECTION_RESULT".GetHashCode();
                        banPacket[0] = (byte)1;
                        banPacket[1] = (int)0;
                        clientConnection.SendPacket(banPacket);

                        clientSocket.Close();
                        return;
                    }

                    // Work out a new unique ID for this client.
                    int id = 0;
                    while (true)
                    {
                        bool found = false;
                        foreach (NetworkClient subclient in _clientList)
                            if (subclient.ID == id) found = true;
                        if (found == false) break;
                        id++;
                    }

                    // Create a new client object to store details on this connection.
                    NetworkClient client = new NetworkClient();
                    client.Connection = clientConnection;
                    client.ID = id;
                    _clientList.Add(client);

                    // Ackowledge its existance.
                    NetworkPacket helloPacket = new NetworkPacket();
                    helloPacket.ID = "CONNECTION_RESULT".GetHashCode();
                    helloPacket[0] = (byte)0;
                    helloPacket[1] = id;
                    client.Connection.SendPacket(helloPacket);

                    // Log this connection.
                    DebugLogger.WriteLog("Accepted connection from " + clientSocket.RemoteEndPoint.ToString() + ", connection assigned id " + id);

                    // Throw an event.
                    if (ClientConnected != null) ClientConnected(this, client);

                    // Go back to waiting for client connections.
                    _socket.BeginAccept(new AsyncCallback(OnClientConnect), null);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        ///     Sends a packet down this connection.
        /// </summary>
        /// <param name="packet">Packet to send down this connection.</param>
        public void SendPacket(NetworkPacket packet)
        {
            if (_socket == null || (_socket.Connected == false && _isListening == false && _isConnected == true))
                return;

            try
            {
                byte[] data = packet.Encode();
                _socket.Send(data);
                _bytesSent += data.Length;
                _globalBytesSent += data.Length;
            }
            catch (Exception) {}
        }

        /// <summary>
        ///     Broadcasts a packet to all clients.
        /// </summary>
        /// <param name="packet">Packet to broadcast.</param>
        public void BroadcastPacket(NetworkPacket packet)
        {
            lock (_clientList)
            {
                foreach (NetworkClient client in _clientList)
                    client.Connection.SendPacket(packet);
            }
        }

        /// <summary>
        ///     Waits until a given packet is recieved and returns it.
        /// </summary>
        /// <param name="id">ID of packet to recieve. -1 for first available packet.</param>
        /// <param name="timeout">Maximum time to wait for packet.</param>
        /// <returns>Recieved packet.</returns>
        public NetworkPacket RecievePacket(int id, int timeout)
        {
            _asyncRecievePacketID = id;
            _asyncRecievePacket = null;

            HighPreformanceTimer timer = new HighPreformanceTimer();
            while (true)
            {
                if (_asyncRecievePacket != null)
                    return _asyncRecievePacket;

                Thread.Sleep(1);
                Application.DoEvents();

                if (timer.DurationMillisecond > timeout)
                    return null;
            }
        }

        /// <summary>
        ///     Begins recieving packets from this client.
        /// </summary>
        private void BeginRecieving(int size)
        {
            if (_socket == null || (_socket.Connected == false && _isListening == false && _isConnected == true))
                return;
            try
            {
                _recieveBuffer = new byte[size];
                _socket.BeginReceive(_recieveBuffer, 0, size, SocketFlags.None, new AsyncCallback(OnRecievePacket), null);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        ///     Invoked when this server recieves a packet from this client.
        /// </summary>
        /// <param name="asyn">Asyncronous state.</param>
        private void OnRecievePacket(IAsyncResult asyn)
        {
            if (_socket == null || (_socket.Connected == false && _isListening == false && _isConnected == true))
                return;
            try
            {
                // Reading the header.
                if (_recieveBuffer.Length == NetworkPacket.HEADER_SIZE)
                {
                    // Finish recieving.
                    _socket.EndReceive(asyn);

                    // Decode the header.
                    _recievedPacket = new NetworkPacket();
                    _recievedPacket.Decode(_recieveBuffer, true, false);

                    _globalBytesRecieved += _recieveBuffer.Length;
                    _bytesRecieved += _recieveBuffer.Length;

                    // No extra data?
                    if (_recievedPacket.DataLength <= 0)
                    {
                        // Evaluate the packet.
                        if (EvaluatePacket(_recievedPacket) == false)
                        {
                            if (PacketRecieved != null) PacketRecieved(this, _recievedPacket);
                            if (_asyncRecievePacketID == -1 || _recievedPacket.ID == _asyncRecievePacketID)
                                _asyncRecievePacket = _recievedPacket;
                            _recievedPacket = null;
                        }

                        // Go back to recieving general data.
                        BeginRecieving(NetworkPacket.HEADER_SIZE);
                    }
                    else
                    {
                        // Read in the rest of the packet.
                        BeginRecieving(_recievedPacket.DataLength);
                    }
                }

                // Reading the data.
                else
                {
                    // Finish recieving.
                    _socket.EndReceive(asyn);

                    // Decode rest of data.
                    _recievedPacket.Decode(_recieveBuffer, false, true);

                    _globalBytesRecieved += _recieveBuffer.Length;
                    _bytesRecieved += _recieveBuffer.Length;

                    // Evaluate the packet.
                    if (EvaluatePacket(_recievedPacket) == false)
                    {
                        if (PacketRecieved != null) PacketRecieved(this, _recievedPacket);
                        if (_asyncRecievePacketID == -1 || _recievedPacket.ID == _asyncRecievePacketID)
                            _asyncRecievePacket = _recievedPacket;
                        _recievedPacket = null;
                    }

                    // Go back to recieving general data.
                    BeginRecieving(NetworkPacket.HEADER_SIZE);
                }
            }
            catch (Exception)
            { }
        }

        /// <summary>
        ///     Evaluates a recieved packet to see if its a internal message.
        /// </summary>
        /// <param name="packet">Packet that has been recieved.</param>
        /// <returns>True if packet is an internal message.</returns>
        private bool EvaluatePacket(NetworkPacket packet)
        {
            if (packet.ID == "PONG".GetHashCode())
            {
                _pingAverageCount++;
                _pingAverageCounter += (float)_pingTimer.DurationMillisecond;
                _ping = _pingAverageCounter / _pingAverageCount;
                _waitingForPong = false;

                return true;
            }
            else if (packet.ID == "PING".GetHashCode())
            {
                NetworkPacket pongPacket = new NetworkPacket();
                pongPacket.ID = "PONG".GetHashCode();
                SendPacket(pongPacket);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="connection">Socket through which to send and recieve data.</param>
        public NetworkConnection(Socket connection)
        {
            _isConnected = true;
            _socket = connection;
            BeginRecieving(NetworkPacket.HEADER_SIZE);
        }
        public NetworkConnection()
        {
        }

        #endregion
    }

    /// <summary>
    ///     This is used by the server to store details on a network client.
    /// </summary>
    public class NetworkClient
    {
        #region Members
        #region Events

        public event PacketRecievedEventHandler PacketRecieved;
        public event DisconnectedEventHandler Disconnected;

        #endregion
        #region Variables

        private NetworkConnection _connection = null;
        private int _id = 0;

        private DateTime _connectTime = DateTime.Now;
        private bool _loggedIn = false;
        private int _accountID = 0;
        private NetworkAccount _account = null;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the time when this user connected.
        /// </summary>
        public DateTime ConnectTime
        {
            get { return _connectTime; }
            set { _connectTime = value; }
        }

        /// <summary>
        ///     Gets or sets if this user is logged in.
        /// </summary>
        public bool LoggedIn
        {
            get { return _loggedIn; }
            set { _loggedIn = value; }
        }

        /// <summary>
        ///     Gets or sets the account this user is logged in to.
        /// </summary>
        public NetworkAccount Account
        {
            get { return _account; }
            set { _account = value; }
        }

        /// <summary>
        ///     Gets or sets the account this client is logged into.
        /// </summary>
        public int AccountID
        {
            get { return _accountID; }
            set { _accountID = value; }
        }

        /// <summary>
        ///     Gets the current download rate in bytes per second.
        /// </summary>
        public float DownRate
        {
            get { return (float)_connection.BytesRecieved / ((float)_connectTime.Ticks / 1000.0f); }
        }

        /// <summary>
        ///     Gets the current upload rate in bytes per second.
        /// </summary>
        public float UpRate
        {
            get { return (float)_connection.BytesSent / ((float)_connectTime.Ticks / 1000.0f); }
        }

        /// <summary>
        ///     Gets or sets the connection used to send and recieve data.
        /// </summary>
        public NetworkConnection Connection
        {
            get { return _connection; }
            set 
            {
                if (_connection != null)
                {
                    _connection.PacketRecieved -= OnPacketRecieved;
                    _connection.Disconnected -= OnDisconnected;
                }
                _connection = value;
                if (_connection != null)
                {
                    _connection.PacketRecieved += new PacketRecievedEventHandler(OnPacketRecieved);
                    _connection.Disconnected += new DisconnectedEventHandler(OnDisconnected);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the ID of this client.
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Invoked when this client is disconnected.
        /// </summary>
        /// <param name="connection">Connection that was disconnected.</param>
        void OnDisconnected(object connection)
        {
            if (Disconnected != null) Disconnected(this);
        }

        /// <summary>
        ///     Invoked when a packet is recieved.
        /// </summary>
        /// <param name="connection">Connection that was disconnected.</param>
        /// <param name="packet">Packet that was recieved.</param>
        void OnPacketRecieved(object connection, NetworkPacket packet)
        {
            EvaluatePacket(packet);
            if (PacketRecieved != null) PacketRecieved(this, packet);
        }

        /// <summary>
        ///     Evaluates the given packet and executes the correct code based on what it contains.
        /// </summary>
        /// <param name="packet">Packet to evaluate.</param>
        public void EvaluatePacket(NetworkPacket packet)
        {

        }

        /// <summary>
        ///     Updates this client.
        /// </summary>
        public void Poll()
        {
            // Poll the connection.
            _connection.Poll();
        }

        #endregion
    }

    /// <summary>
    ///     Stores details on a ban.
    /// </summary>
    public class NetworkBan
    {
        #region Members
        #region Variables

        private string _ip;
        private int _accountID = 0;
        private DateTime _expiration;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the IP banned by this ban.
        /// </summary>
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }

        /// <summary>
        ///     Gets or sets the ID of the account banned by this ban.
        /// </summary>
        public int AccountID
        {
            get { return _accountID; }
            set { _accountID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Expiration
        {
            get { return _expiration; }
            set { _expiration = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Checks to see if a given IP is banned by this ban.
        /// </summary>
        /// <param name="ip">IP to check.</param>
        /// <returns>True if IP is covered by this ban.</returns>
        public bool IsIPBanned(string ip)
        {
            if (DateTime.Now > _expiration) return false;
            return _ip.ToLower() == ip.ToLower();
        }

        /// <summary>
        ///     Checks to see if a given account ID is banned by this ban.
        /// </summary>
        /// <param name="accountID">Account ID to check.</param>
        /// <returns>True if accont id is covered by this ban.</returns>
        public bool IsAccountBanned(int accountID)
        {
            if (DateTime.Now > _expiration) return false;
            return _accountID == accountID;
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="ip">IP to ban.</param>
        /// <param name="accountID">Account ID to ban.</param>
        /// <param name="expiration">Expiration of this account.</param>
        public NetworkBan(string ip, int accountID, DateTime expiration)
        {
            _ip = ip;
            _accountID = accountID;
            _expiration = expiration;
        }

        #endregion
    }

    /// <summary>
    ///		The network manager class deals with networked multiplayer games.
    /// </summary>
    public static class NetworkManager
    {
        #region Members
        #region Events

        public static event PacketRecievedEventHandler PacketRecieved;
        public static event ClientConnectedEventHandler ClientConnected;
        public static event ClientDisconnectedEventHandler ClientDisconnected;
        public static event DisconnectedEventHandler Disconnected;

        #endregion
        #region Variables

        private static NetworkConnection _connection = null;

        private static int _port = 12345;
        private static string _serverIP = "127.0.0.1";
        private static bool _isServer = false;

        private static ArrayList _bans = new ArrayList();
        private static ArrayList _accountList = new ArrayList();

        private static string _networkConfigFile = "network.xml";
        private static int _maximumClients = 255;
        private static int _pingDelay = 5000;

        private static int _id = 0;
        private static int _connectionResult = 0;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the current client list.
        /// </summary>
        public static ArrayList ClientList
        {
            get 
            {
                return _connection.ClientList;
            }
        }

        /// <summary>
        ///     Gets or sets the list of currently banned IP addresses.
        /// </summary>
        public static ArrayList BansList
        {
            get { return _bans; }
            set { _bans = value; }
        }

        /// <summary>
        ///     Gets or sets the list of accounts.
        /// </summary>
        public static ArrayList AccountList
        {
            get { return _accountList; }
            set { _accountList = value; }
        }

        /// <summary>
        ///     Gets or sets the current connection used by the network.
        /// </summary>
        public static NetworkConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        /// <summary>
        ///     Gets or sets port number of network.
        /// </summary>
        public static int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        ///     Gets or sets the server IP to connect to.
        /// </summary>
        public static string ServerIP
        {
            get { return _serverIP; }
            set { _serverIP = value; }
        }

        /// <summary>
        ///     Gets or sets if we are running a server.
        /// </summary>
        public static bool IsServer
        {
            get { return _isServer; }
            set { _isServer = value; }
        }

        /// <summary>
        ///     Gets or sets the configuration file that stores network details.
        /// </summary>
        public static string NetworkConfigFile
        {
            get { return _networkConfigFile; }
            set { _networkConfigFile = value; }
        }

        /// <summary>
        ///     Gets or sets the maximum amount of clients that can be processed on this server.
        /// </summary>
        public static int MaximumClients
        {
            get { return _maximumClients; }
            set { _maximumClients = value; }
        }


        /// <summary>
        ///     Gets or sets the delay between ping requests to and from the server.
        /// </summary>
        public static int PingDelay
        {
            get { return _pingDelay; }
            set { _pingDelay = value; }
        }

        /// <summary>
        ///     Gets or sets the ID of this client.
        /// </summary>
        public static int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        ///     Gets or sets the result of the connection to the server.
        /// </summary>
        public static int ConnectionResult
        {
            get { return _connectionResult; }
            set { _connectionResult = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Starts the network connection with the currnet settings.
        /// </summary>
        /// <returns>True if successfull.</returns>
        public static bool Start()
        {
#if !DEBUG
            try
            {
#endif
                if (_isServer == true)
                {
                    DebugLogger.WriteLog("Setting up server...");

                    // Load in network configuration file.
                    DebugLogger.WriteLog("Loading network configuration file...");
                    DebugLogger.IncreaseIndent();

                    Stream stream = StreamFactory.RequestStream(_networkConfigFile, StreamMode.Open);
                    if (stream == null) return false;
                    XmlDocument document = new XmlDocument();
                    document.Load(stream);
                    stream.Close();

                    // Get statistics.
                    _port = Convert.ToInt32(document["network"]["settings"]["port"].InnerText);
                    _maximumClients = Convert.ToInt32(document["network"]["settings"]["maximumclients"].InnerText);
                    _pingDelay = Convert.ToInt32(document["network"]["settings"]["pingdelay"].InnerText);

                    // Load in banned IP's
                    XmlNode bansNode = document["network"]["bans"];
                    if (bansNode == null) throw new Exception("Network configuration declaration file is missing bans declaration.");
                    foreach (XmlNode subNode in bansNode.ChildNodes)
                    {
                        if (subNode.Name.ToLower() != "ban") continue;

                        NetworkBan ban = new NetworkBan("", 0, DateTime.Now);
                        _bans.Add(ban);

                        foreach (XmlNode subSubNode in subNode.ChildNodes)
                        {
                            switch (subSubNode.Name.ToLower())
                            {
                                case "ip": ban.IP = subSubNode.InnerText; break;
                                case "accountid": ban.AccountID = Convert.ToInt32(subSubNode.InnerText); break;
                                case "expiration": ban.Expiration = new DateTime(Convert.ToInt64(subSubNode.InnerText)); break;
                            }
                        }

                        DebugLogger.WriteLog("Loaded ban, ip:"+ban.IP+" accountID:" + ban.AccountID);
                    }

                    // Load account details.
                    XmlNode accountsNode = document["network"]["accounts"];
                    if (accountsNode == null) throw new Exception("Network configuration declaration file is missing accounts declaration.");
                    foreach (XmlNode subNode in accountsNode.ChildNodes)
                    {
                        if (subNode.Name.ToLower() != "account") continue;

                        NetworkAccount account = new NetworkAccount();
                        _accountList.Add(account);

                        foreach (XmlNode subSubNode in subNode.ChildNodes)
                        {
                            switch (subSubNode.Name.ToLower())
                            {
                                case "lastknownip": account.LastKnownIP = subSubNode.InnerText; break;
                                case "passwordhash": account.PasswordHash = subSubNode.InnerText; break;
                                case "passwordsalt": account.PasswordSalt = subSubNode.InnerText; break;
                                case "dateregistered": account.DateRegistered = new DateTime(long.Parse(subSubNode.InnerText)); break;
                                case "lastactive": account.LastActive = new DateTime(long.Parse(subSubNode.InnerText)); break;
                                case "username": account.Username = subSubNode.InnerText; break;
                                case "position": account.Position = (NetworkAccountPosition)Enum.Parse(typeof(NetworkAccountPosition), subSubNode.InnerText); break;           
                                case "custom":
                                    {
                                        foreach (XmlNode customNode in subSubNode.ChildNodes)
                                            account.CustomSettings[customNode.Name] = customNode.InnerText;
                                    break;
                                    }
                            }
                        }

                        DebugLogger.WriteLog("Loaded account: " + account.Username);
                    }

                    // Connect to server.
                    DebugLogger.DecreaseIndent();
                    DebugLogger.WriteLog("Setting up connection...");
                    _connection = new NetworkConnection();
                    _connection.Connect("", _port, true);
                    _connection.PacketRecieved += new PacketRecievedEventHandler(OnPacketRecieved); 
                    _connection.ClientConnected += new ClientConnectedEventHandler(OnClientConnected);
                    _connection.ClientDisconnected += new ClientDisconnectedEventHandler(OnClientDisconnected);
                    _connection.Disconnected += new DisconnectedEventHandler(OnDisconnected);
                    DebugLogger.WriteLog("Server setup.");
                    return true;
                }
                else
                {
                    DebugLogger.WriteLog("Connecting to server...");
                    _connection = new NetworkConnection();
                    _connection.Connect(_serverIP, _port, false);
                    _connection.PacketRecieved += new PacketRecievedEventHandler(OnPacketRecieved);
                    _connection.Disconnected += new DisconnectedEventHandler(OnDisconnected);
                    DebugLogger.WriteLog("Connected to server, waiting for connection result packet.");

                    NetworkPacket packet = _connection.RecievePacket("CONNECTION_RESULT".GetHashCode(), 5000);
                    if (packet != null)
                    {
                        EvaluatePacket(packet);
                        DebugLogger.WriteLog("Connection result packet recieved.");
                    }
                    else
                    {
                        _connection.Close();
                        _connectionResult = -1;
                        DebugLogger.WriteLog("Connection to server closed, server failed to respond with connection result packet.", BinaryPhoenix.Fusion.Runtime.Debug.LogAlertLevel.Error);
                        return false;
                    }
                }

                return true;
#if !DEBUG
            }
            catch (Exception)
            {
                //_isConnected = false;
               return false;
            }
#endif
        }

        /// <summary>
        ///     Closes the network connection and saves all settings.
        /// </summary>
        public static void Close()
        {
            Stream stream = StreamFactory.RequestStream(_networkConfigFile, StreamMode.Open);
            if (stream == null) return;
            XmlDocument document = new XmlDocument();
            document.Load(stream);
            stream.Close();

            // Save in the banned IP addresses.
            XmlNode bansNode = document["network"]["bans"];
            if (bansNode == null) throw new Exception("Network configuration declaration file is missing bans declaration.");
            bansNode.RemoveAll();
            foreach (NetworkBan ban in _bans)
            {
                XmlNode banNode = document.CreateDocumentFragment();
                banNode.InnerXml = "<ban><ip>"+ban.IP.ToString()+"</ip><accountid>"+ban.AccountID.ToString()+"</accountid><expiration>"+ban.Expiration.Ticks+"</expiration></ban>";
                bansNode.AppendChild(banNode);
            }

            // Save in the accounts.
            XmlNode accountsNode = document["network"]["accounts"];
            if (accountsNode == null) throw new Exception("Network configuration declaration file is missing accounts declaration.");
            accountsNode.RemoveAll();
            foreach (NetworkAccount account in _accountList)
            {
                string customData = "";
                foreach (string key in account.CustomSettings.Keys)
                    customData += "<"+key+">"+account.CustomSettings[key]+"</"+key+">";

                XmlNode accountNode = document.CreateDocumentFragment();
                accountNode.InnerXml = "<account><lastknownip>"+account.LastKnownIP+"</lastknownip><passwordhash>"+account.PasswordHash+"</passwordhash><passwordsalt>"+account.PasswordSalt+"</passwordsalt><dateregistered>"+account.DateRegistered.Ticks+"</dateregistered><lastactive>"+account.LastActive.Ticks+"</lastactive><username>"+account.Username+"</username><position>"+(int)account.Position+"</position><custom>"+customData+"</custom></account>";
                accountsNode.AppendChild(accountNode);
            }

            document.Save(_networkConfigFile);
        }

        /// <summary>
        ///     Invoked when this connection is closed.
        /// </summary>
        /// <param name="sender">Connection that was closed.</param>
        private static void OnDisconnected(object sender)
        {
            DebugLogger.WriteLog("Connection was closed.");
            if (Disconnected != null) Disconnected(_connection);
        }

        /// <summary>
        ///     Invoked when a client disconnected.
        /// </summary>
        /// <param name="sender">Connection that client disconnected from.</param>
        /// <param name="client">Client that disconnected.</param>
        private static void OnClientDisconnected(object sender, NetworkClient client)
        {
            if (ClientDisconnected != null) ClientDisconnected(sender, client);
        }

        /// <summary>
        ///     Invoked when a client is connected.
        /// </summary>
        /// <param name="sender">Connection that client connected to.</param>
        /// <param name="client">Client that connected.</param>
        private static void OnClientConnected(object sender, NetworkClient client)
        {
            if (ClientConnected != null) ClientConnected(sender, client);
        }

        /// <summary>
        ///     Invoked when a packet is recieved.
        /// </summary>
        /// <param name="sender">Connection hat packet was recieved by.</param>
        /// <param name="packet">Packet that was recieved.</param>
        private static void OnPacketRecieved(object sender, NetworkPacket packet)
        {
            EvaluatePacket(packet);
            if (PacketRecieved != null) PacketRecieved(sender, packet);
        }

        /// <summary>
        ///     Closes the network connections.
        /// </summary>
        public static void Finish()
        {
            // Close main connection.
            if (_connection.Connected == true)
                _connection.Close();

            if (_isServer == true)
            {
                // Save configuration file.
            }
            else
            {

            }
        }

        /// <summary>
        ///     Evaluates the given packet and executes the correct code based on what it contains.
        /// </summary>
        /// <param name="packet">Packet to evaluate.</param>
        public static void EvaluatePacket(NetworkPacket packet)
        {
            if (packet.ID == "CONNECTION_RESULT".GetHashCode() && _isServer == false)
            {
                _connectionResult = (byte)packet[0];
                switch ((byte)packet[0])
                {
                    case 0: // Success.
                        _id = (int)packet[1];
                        break;
                    case 1: // Banned.
                        if (_connection.Connected == true) _connection.Close();
                        DebugLogger.WriteLog("Unable to connect to server, IP is banned.", BinaryPhoenix.Fusion.Runtime.Debug.LogAlertLevel.Warning);
                        break;
                    case 2: // To many clients.
                        if (_connection.Connected == true) _connection.Close();
                        DebugLogger.WriteLog("Unable to connect to server, maximum amount of clients are already connected.", BinaryPhoenix.Fusion.Runtime.Debug.LogAlertLevel.Warning);
                        break;
                }
            }
            else if (packet.ID == "CONSOLE_COMMAND".GetHashCode() && _isServer == false)
            {
                DebugLogger.WriteLog("Executing console command sent by server.");
                Runtime.Console.Console.ProcessCommand(packet[0].ToString());
            }
        }

        /// <summary>
        ///     Updates the network.
        /// </summary>
        public static void Poll()
        {
            if (_connection == null) return;

            // Poll the connection.
            _connection.Poll();
            if (_connection.Connected == false)
                return;

            if (_isServer == true)
            {   
                // Syncronize connected clients?
                foreach (NetworkClient client in _connection.ClientList)
                    client.Poll();
            }
            else
            {

            }
        }

        #endregion
    }

}