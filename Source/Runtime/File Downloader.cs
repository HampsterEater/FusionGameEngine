/* 
 * File: File Downloader.cs
 *
 * This source file contains the declaration of the File Downloader class
 * which is used to download file from a network to the local host.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Web;
using System.Diagnostics;

using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime
{

	/// <summary>
	///		Used by the file downloader to notify the user when the 
	///		download progress is updated.
	/// </summary>
	/// <param name="downloader">FileDownloader instance that invoked this event.</param>
	/// <param name="progress">Current progress of download.</param>
	public delegate void ProgressUpdateEventHandler(FileDownloader downloader, int progress);

    /// <summary>
    ///		Used by the file downloader to notify the user when an error occurs.
    /// </summary>
    /// <param name="downloader">FileDownloader instance that invoked this event.</param>
    /// <param name="error">Description of error.</param>
    public delegate void ErrorOccuredEventHandler(FileDownloader downloader, string error);

    /// <summary>
    ///     Used to specify different types of download method.
    /// </summary>
    public enum DownloadMethod
    {
        GET,
        POST
    }

	/// <summary>
	///		The file downloader class which is used to 
	///		download file from a network to the local host.
	/// </summary>
	public sealed class FileDownloader
	{
		#region Members
		#region Variables

		private Thread _downloadThread = null;

		private Socket _socket = null;
		private NetworkStream _stream = null;

		private string _host, _file, _url;
		private int _speed, _speedTracker, _downloadedBytes, _totalSize;
		private bool _complete = false;
		private int _progress;

		private byte[] _fileData;

		private bool _paused = false;

        private DownloadMethod _method = DownloadMethod.GET;
        private Hashtable _postVariables = new Hashtable();

        private string _error = "";

        private ArrayList _averageCounter = new ArrayList();
        private int _averageSpeed = 0;

		#endregion
        #region Events

        public event ProgressUpdateEventHandler ProgressUpdate;
        public event ErrorOccuredEventHandler ErrorOccured;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets if this file download is paused.
        /// </summary>
        public bool Paused
        {
            get { return _paused; }
            set { _paused = value; }
        }

        /// <summary>
        ///     Gets or sets the post variables that should be sent to the server.
        /// </summary>
        public Hashtable PostVariables
        {
            get { return _postVariables; }
            set { _postVariables = value; }
        }

        /// <summary>
        ///     Gets or sets the method used to download the remote files data.
        /// </summary>
        public DownloadMethod Method
        {
            get { return _method; }
            set { _method = value; }
        }

		/// <summary>
		///		Gets the internal stream being used to download the remote files data.
		/// </summary>
		public NetworkStream Stream
		{
			get { return _stream; }
		}

		/// <summary>
		///		Gets the internal socket being used to download the remote files data.
		/// </summary>
		public Socket Socket
		{
			get { return _socket; }
		}

        /// <summary>
        ///		Gets the last error that occured while downloading.
        /// </summary>
        public string Error
        {
            get { return _error; }
        }

		/// <summary>
		///		Gets the url of the remote file currently being downloaded.
		/// </summary>
		public string URL
		{
			get { return _url;  }
		}

		/// <summary>
		///		Gets the speed (in bytes per second) of the current download.
		/// </summary>
		public int Speed
		{
			get { return _speed;  }
		}

        /// <summary>
        ///     Gets the average speed that this download is downloading at.
        /// </summary>
        public int AverageSpeed
        {
            get { return _averageSpeed; }
        }

		/// <summary>
		///		Gets the progress (between 0 and 100) of the current download.
		/// </summary>
		public int Progress
		{
			get { return _progress; }
		}

		/// <summary>
		///		Gets the amount of bytes currently downloaded.
		/// </summary>
		public int Size
		{
			get { return _downloadedBytes; }
		}

		/// <summary>
		///		Gets the total size of the remote file being downloaded.
		/// </summary>
		public int TotalSize
		{
			get { return _totalSize; }
		}

		/// <summary>
		///		Gets a byte array containing the data downloaded from the remote file.
		/// </summary>
		public byte[] FileData
		{
			get { return _fileData; }
		}

		/// <summary>
		///		Gets the completion status of this download.
		/// </summary>
		public bool Complete
		{
			get { return _complete; }
		}

		/// <summary>
		///		Gets the approximate time inseconds before this download completes.
		/// </summary>
		public int TimeRemaining
		{
			get 
            { 
                int kbremaining = (_totalSize - _downloadedBytes) / 1024;
                return (int)((kbremaining == 0 ? 1 : kbremaining) / (_averageSpeed == 0 ? 1 : _averageSpeed)); 
            }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Entry point of the data download thread.
		/// </summary>
		private void DownloadThread()
		{
            try
            {
                // Open up a connection to the binary phoenix software updater.
                if (_socket != null) _socket.Close();
                try
                {
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _socket.Connect(new IPAddress(Dns.GetHostEntry(_host).AddressList[0].GetAddressBytes()), 80);
                    _socket.ReceiveTimeout = 5000;
                }
                catch (SocketException)
                {
                    _error = "Unable to connect to host \"" + _host + "\".";
                    Debug.DebugLogger.WriteLog("An error occured while downloading '" + _url + "', \"" + _error + "\" ");
                    if (ErrorOccured != null) ErrorOccured(this, _error);
                    return;
                }
                _stream = new NetworkStream(_socket);

                // Build the post variables.
                int var = 0;
                string postRequest = "";
                foreach (string key in _postVariables.Keys)
                {
                    postRequest += (var != 0 ? "&" : "") + key + "=" + System.Web.HttpUtility.UrlEncode(_postVariables[key] as string);
                    var++;
                }

                // Try and access the php updater file with the given account.
                string getRequest = (_method == DownloadMethod.POST ? "POST " : "GET ") + (_file + (postRequest != "" ? "&" + postRequest : "")).Replace(" ", "%20") + " HTTP/1.0\n" +
                                    "host: " + _host + "\n" +
                                    "user-agent: FusionDownloader\n" +
                                    "accept: * /*\n" +
                                    "connection: keep-alive\n" +
                                    "Keep-Alive: 300\n" +
                                    "cache-control: no-cache\n" +
                                    "content-type: application/x-www-form-urlencoded\n" +
                                    (_method == DownloadMethod.POST ? "content-length: " + postRequest.Length : "") +
                                    "\n";
                _socket.Send(Encoding.ASCII.GetBytes(getRequest));

                // Check this request went through OK.
                string result = ReadLineFromSocket(_socket);
                if (result.IndexOf("200 OK") < 0)
                {
                    _error = "File \"" + _file + "\" does not exist on host \"" + _host + "\".";
                    Debug.DebugLogger.WriteLog("An error occured while downloading '" + _url + "', \"" + _error + "\" ");
                    if (ErrorOccured != null) ErrorOccured(this, _error);
                    return;
                }

                // Skip the header as we don't require it.
                string header = "#";
                while (header != "")
                {
                    header = ReadLineFromSocket(_socket);
                    if (header.ToLower().StartsWith("content-length:") == true)
                        _totalSize = int.Parse(header.Substring(15));
                }

                // Create an array to hold the data downloaded.
                _fileData = new byte[_totalSize];

                // Create a timer to use when tracking the speed of the download.
                HighPreformanceTimer timer = new HighPreformanceTimer();

                if (_totalSize != 0)
                {
                    while (_downloadedBytes < _totalSize)
                    {
                        // If this download has been paused then go to sleep for a little bit.
                        while (_paused == true)
                            Thread.Sleep(1);

                        // Read in the next lot of data and quit if we don't recieve any.
                        int bytesRead = 0;
                        try
                        {
                            bytesRead = _socket.Receive(_fileData, _downloadedBytes, _totalSize - _downloadedBytes < 1024 ? _totalSize - _downloadedBytes : 1024, 0);
                            if (bytesRead == 0) break;
                        }
                        catch (SocketException)
                        {
                            _error = "Download of \"" + _url + "\" was forceable stopped by the remote host.";
                            Debug.DebugLogger.WriteLog("Download of \"" + _url + "\" was forceable stopped by the remote host.");
                            if (ErrorOccured != null) ErrorOccured(this, _error);
                            return;
                        }

                        // Update the amount of bytes downloaded and break out 
                        // of this loop if we haven't read in a full buffer.
                        _downloadedBytes += bytesRead;

                        // Keep track of the current download speed.
                        if (timer.DurationMillisecond >= 1000)
                        {
                            timer.Restart();
                            _speed = (int)(((float)_downloadedBytes - (float)_speedTracker) / 1024.0f);
                            _speedTracker = _downloadedBytes;

                            // Push this speed onto the average stack.
                            _averageCounter.Add(_speed);
                            if (_averageCounter.Count > 20)
                                _averageCounter.RemoveAt(0);

                            // Work out the average speed.
                            int total = 0;
                            foreach (int avgSpeed in _averageCounter)
                                total += avgSpeed;
                            _averageSpeed = total / _averageCounter.Count;
                        }

                        // Invoke the update progress notification event.
                        _progress = (int)((100.0f / (float)_totalSize) * (float)_downloadedBytes);
                        if (ProgressUpdate != null)
                            ProgressUpdate.Invoke(this, _progress);
                    }

                    // Check we managed to download the whole file.
                    if (_fileData.Length != _totalSize)
                    {
                        _error = "Unable to download all data for \"" + _file + "\" expecting " + _totalSize + " bytes but got " + _fileData.Length + " bytes.";
                        Debug.DebugLogger.WriteLog("An error occured while downloading '" + _url + "', \"" + _error + "\" ");
                        if (ErrorOccured != null) ErrorOccured(this, _error);
                        return;
                    }
                }
                else
                {
                    while (true)
                    {
                        // If this download has been paused then go to sleep for a little bit.
                        while (_paused == true)
                            Thread.Sleep(1);

                        // Read in the next lot of data and quit if we don't recieve any.
                        byte[] dataBuffer = new byte[1024];
                        int bytesRead = 0;
                        try
                        {
                            bytesRead = _socket.Receive(dataBuffer, 1024, 0);
                        }
                        catch
                        {
                            _error = "Unable to download all data for \"" + _file + "\", require timed out.";
                            Debug.DebugLogger.WriteLog("An error occured while downloading '" + _url + "', \"" + _error + "\" ");
                            if (ErrorOccured != null) ErrorOccured(this, _error);
                            return;
                        }
                        if (bytesRead == 0) break;

                        // Convert the bytes into characters and append it to the data.
                        if (_downloadedBytes + bytesRead - 1 > _fileData.Length)
                            Array.Resize<byte>(ref _fileData, _downloadedBytes + bytesRead);

                        for (int i = 0; i < bytesRead; i++)
                            _fileData[_downloadedBytes + i] = dataBuffer[i];
                        _downloadedBytes += bytesRead;
                    }
                }

                // Close the stream.
                _stream.Close();
                _socket.Close();
                _complete = true;

                Debug.DebugLogger.WriteLog("Downloaded file of " + _fileData.Length + " bytes from '" + _url + "'...");
            }
            catch (Exception)
            {
                _error = "The server forceably closed the connection.";
            }
		}

		/// <summary>
		///		Reads a line of text from a given socket.
		/// </summary>
		/// <param name="socket">Socket to read from.</param>
		/// <returns>Line of text read from socket.</returns>
		private string ReadLineFromSocket(Socket socket)
		{
			string data = "";
			while (true)
			{
				byte[] buffer = new byte[1];
				if (socket.Receive(buffer) == 0) break;
				if (buffer[0] == '\n')
					break;
				else
					data += (char)buffer[0];
			}
			return data.Replace("\r", "");
		}

		/// <summary>
		///		Aborts the download.
		/// </summary>
		public void Abort()
		{
			_complete = false;
			_downloadThread.Abort();
			if (_socket != null) _socket.Close();
		}

		/// <summary>
		///		Pauses the downloaded until Resume is called.
		/// </summary>
		public void Pause()
		{
			_paused = true;
		}

		/// <summary>
		///		Resumes the download if it has been paused with the Pause method.
		/// </summary>
		public void Resume()
		{
			_paused = false;
		}

		/// <summary>
		///		Starts to download the remote files data.
		/// </summary>
		public void Start()
		{
			if (_downloadThread != null) _downloadThread.Abort();
			if (_socket != null) _socket.Close();

			_paused = false;
			_complete = false;
			_downloadThread = new Thread(DownloadThread);
			_downloadThread.IsBackground = true;
			_downloadThread.Start();

            Debug.DebugLogger.WriteLog("Downloading file from '"+_url+"'...");
		}

		/// <summary>
		///		Initializes a new instance of this class and points it to the file
		///		you specify.
		/// </summary>
		/// <param name="url">Url of remote file to download.</param>
		public FileDownloader(string url)
		{
			_url = url;
			url = url.Replace('\\', '/'); // Replace windows slashs with unix slashs.
			if (url.ToLower().StartsWith("http://") == true) url = url.Substring(7); // Remove the unneeded http:// prefix.
			_file = url.Substring(url.IndexOf('/'));
			_host = url.Substring(0, url.IndexOf('/'));
		}

		#endregion
	}

}