/* 
 * File: Pak Factory.cs
 *
 * This source file contains the declaration of the PakStreamFactory class, which is
 * used to spawn a stream to a file contained within a pak file.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System.Collections;
using System.IO;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime.StreamFactorys
{

    /// <summary>
	///     The PakStreanFactory class is  used to spawn a stream to 
	///		a file contained within a pak file.
    /// </summary>
    public sealed class PakStreamFactory : StreamFactory
	{
		#region Methods

		public PakStreamFactory()
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
            if (Resources.ResourceManager.ResourceExists(path, true) == false) 
                return null;
			return Resources.ResourceManager.RequestResourceStream(path.ToString());
        }

		#endregion
    }

}