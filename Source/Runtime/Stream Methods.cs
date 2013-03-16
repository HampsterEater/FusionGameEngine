/* 
 * File: Stream.cs
 *
 * This source file contains the declarations of several stream 
 * releated classes used to allow the opening of streams in differnt ways,
 * and from different places, for example from a pak file or server.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.IO;
using BinaryPhoenix.Fusion.Runtime.StreamFactorys;

namespace BinaryPhoenix.Fusion.Runtime
{
	#region Endian Swapping Classes

	public static class EndianSwapMethods
	{

		/// <summary>
		///		Converts the endian of the value from big to little or vis-versa.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <returns>Value with endian swapped.</returns>
		public static byte[] SwapEndian(byte[] value)
		{
			Array.Reverse(value);
			return value;
		}
		public static bool SwapEndian(bool value)
		{
			return BitConverter.ToBoolean(SwapEndian(BitConverter.GetBytes(value)),0);
		}
		public static char SwapEndian(char value)
		{
			return BitConverter.ToChar(SwapEndian(BitConverter.GetBytes(value)), 0);
		}
		public static double SwapEndian(double value)
		{
			return BitConverter.ToDouble(SwapEndian(BitConverter.GetBytes(value)), 0);
		}
		public static Int16 SwapEndian(Int16 value)
		{
			return BitConverter.ToInt16(SwapEndian(BitConverter.GetBytes(value)), 0);
		}
		public static Int32 SwapEndian(Int32 value)
		{
			return BitConverter.ToInt32(SwapEndian(BitConverter.GetBytes(value)), 0);
		}
		public static Int64 SwapEndian(Int64 value)
		{
			return BitConverter.ToInt64(SwapEndian(BitConverter.GetBytes(value)), 0);
		}
		public static Single SwapEndian(Single value)
		{
			return BitConverter.ToSingle(SwapEndian(BitConverter.GetBytes(value)), 0);
		}
		public static UInt16 SwapEndian(UInt16 value)
		{
			return BitConverter.ToUInt16(SwapEndian(BitConverter.GetBytes(value)), 0);
		}
		public static UInt32 SwapEndian(UInt32 value)
		{
			return BitConverter.ToUInt32(SwapEndian(BitConverter.GetBytes(value)), 0);
		}
		public static UInt64 SwapEndian(UInt64 value)
		{
			return BitConverter.ToUInt64(SwapEndian(BitConverter.GetBytes(value)), 0);
		}

	}

	#endregion
	#region Enumerations

	/// <summary>
    ///     Used to determine how to open or create a stream.
    /// </summary>
    public enum StreamMode : int
    {
        Append   = 0x000001,
        Open     = 0x000002,
		Truncate = 0x000004
	}

	#endregion
	#region Stream Factory Classes
	/// <summary>
    ///     The StreamFactory class is used as a way to easily add stream systems,
    ///     all you need to do is inheret a class of this and call the
    ///     register method.
    /// 
    ///     When an stream is requested the RequestStream method will be called for
    ///     all registered StreamFactorys, if it returns a stream, then the stream will 
    ///     be returned to the user, else it will continue through 
    ///     the list of StreamFactorys until and instance does return one.
    /// </summary>
    public abstract class StreamFactory
    {
		#region Members
		#region Variables

		private static ArrayList _loaderList = new ArrayList();
		
		#endregion
		#endregion
		#region Methods
		/// <summary>
        ///     This method is called when stream is requested, if you return a stream
        ///     it will return it to the user else it will keep trying all the other StreamFactorys
        ///     until one does return a stream.
        /// </summary>
        /// <param name="path">Path of file or object to create stream to.</param>
        /// <param name="mode">Determines how to open or create stream.</param>
        /// <returns>A Stream instance or NULL if this factory can't open the given stream.</returns>
        protected abstract Stream Request(object path, StreamMode mode);

        /// <summary>
        ///     This method is called when a stream is requested, it illiterates through all
        ///     the registered StreamFactory instances to see if there is one capable of opening the
        ///     given stream.
        /// </summary>
        /// <param name="path">Path of file or object to create stream to.</param>
        /// <param name="mode">Determines how to open or create stream.</param>
        /// <returns>A Stream instance or NULL if unable to find a factory to open this stream.</returns>
        public static Stream RequestStream(object path, StreamMode mode)
        {
            if (path.ToString().ToLower().StartsWith("pak@")) 
                path = path.ToString().Substring(4); // Legacy support.

			// If this is already a stream then copy its data to a new stream 
			// that we can read from.
			if (path as Stream != null)
			{
				Stream stream = (path as Stream);
				MemoryStream memStream = new MemoryStream();

				byte[] data = new byte[stream.Length];
				stream.Read(data, 0, (int)stream.Length);
				stream.Position = 0;

				memStream.Write(data, 0, data.Length);
				memStream.Position = 0;

				return memStream;
			}

			foreach (StreamFactory factory in _loaderList)
            {
#if RELEASE || PROFILE
                try
                {
#endif
					Stream stream = factory.Request(path, mode);
                    if (stream != null) return stream;
#if RELEASE || PROFILE
				}
                catch (System.IO.IOException)
                {
                    // Don't do anything here, just here to catch any errors that may occur.
                }
#endif
			}
            return null;
        }

        /// <summary>
        ///     Adds (registers) an instance to the LoaderList, so it will be used when a stream
        ///     is requested.
        /// </summary>
        protected void Register()
        {
            _loaderList.Add(this);
        }

		#endregion
	}
	#endregion
}