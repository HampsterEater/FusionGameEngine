/* 
 * File: Memory Factory.cs
 *
 * This source file contains the declaration of the MemoryFactory class, which will
 * spawn a stream associated with a block of memory if the given url starts with mem@.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System.Collections;
using System.IO;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime.StreamFactorys
{

    /// <summary>
    ///     The MemoryStreamFactory class is used to spawn a stream associated with
	///		a block of memory if the given url starts with mem@
    /// </summary>
    public sealed class MemoryStreamFactory : StreamFactory
	{
		#region Methods

		public MemoryStreamFactory()
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
        protected override Stream Request(object path,StreamMode mode)
        {
			if (path.ToString().ToLower().Substring(0, 4) != "mem@") return null;

			// Check if we have been handed a capacity as well.
			if (path.ToString().Length > 4)
			{
				string capacityStr = path.ToString().Substring(4, path.ToString().Length - 4);
				int capacity = 0;
				if (int.TryParse(capacityStr, out capacity) == true)
					return new MemoryStream(capacity);
				else
					return new MemoryStream();
			}
			else
			{
				return new MemoryStream();
			}

		}
		#endregion
    }

}