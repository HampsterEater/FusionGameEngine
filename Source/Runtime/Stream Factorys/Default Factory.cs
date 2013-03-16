/* 
 * File: Default Factory.cs
 *
 * This source file contains the declaration of the DefaultFactory class, which is
 * used to spawn a stream to a file if other factorys are not capable of doing so.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.IO;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime.StreamFactorys
{

    /// <summary>
	///     The DefaultStreamFactory class is used to spawn a stream associated
	///		with a file if none of the other factorys can create one.
    /// </summary>
    public sealed class DefaultStreamFactory : StreamFactory
	{
		#region Methods

		public DefaultStreamFactory()
        {
            Register();
        }

        /// <summary>
        ///     This method is called when stream is requested, if you return a stream
        ///     it will return it to the user else it will keep trying all the other StreamFactorys
        ///     until one does return a stream.
        /// </summary>
        /// <param name="path">Path of file or object to create stream to.</param>
        /// <param name="mode">Determines how to open or create stream.</param>
        /// <returns>A Stream instance or NULL if this factory can't open the given stream.</returns>
        protected override Stream Request(object path, StreamMode mode)
        {
           // if (File.Exists(path.ToString()) == false && moede != StreamMode.Open) return null;
            try
            {
                switch (mode)
                {
                    case StreamMode.Append:
                        if (Directory.Exists(Path.GetDirectoryName(path.ToString()))) Directory.CreateDirectory(Path.GetDirectoryName(path.ToString()));
                        if (File.Exists(path.ToString()) == false) File.Create(path.ToString()).Close();
                        return new FileStream(path.ToString(), FileMode.Append);

                    case StreamMode.Open:
                        return new FileStream(path.ToString(), FileMode.Open);

                    case StreamMode.Truncate:
                        if (Directory.Exists(Path.GetDirectoryName(path.ToString()))) Directory.CreateDirectory(Path.GetDirectoryName(path.ToString()));
                        if (File.Exists(path.ToString()) == false) File.Create(path.ToString()).Close();
                        return new FileStream(path.ToString(), FileMode.Truncate);

                }
            }
            catch (Exception) {}
            return null;
        }
		#endregion
    }

}