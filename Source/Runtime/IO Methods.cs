/* 
 * File: IO.cs
 *
 * This source file contains the declaration of the IOMethods class which
 * includes several basic in/out functions used by the rest of the engine.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime
{

	/// <summary>
	///		Contains several basic IO functions using by the rest of the engine.
	/// </summary>
	public static class IOMethods
	{
		#region Methods

		/// <summary>
		///		Copys a directory tree from one location to another.
		/// </summary>
		/// <param name="from">The source directory tree.</param>
		/// <param name="to">The destination path of the directory tree.</param>
		public static void CopyDirectory(string from, string to)
		{
			// Append a directory seperator to the end of the path.
			if (to[to.Length - 1] != Path.DirectorySeparatorChar)
				to += Path.DirectorySeparatorChar;

			// If the destination directory does not exist then create it.
			if (Directory.Exists(to) == false) Directory.CreateDirectory(to);

			// Go through each sub directory and file and copy it.
			foreach(string file in Directory.GetFileSystemEntries(from))
				if (Directory.Exists(file) == true)
					CopyDirectory(file, to + Path.GetFileName(file));
				else
					File.Copy(file, to + Path.GetFileName(file), true);
		}

        /// <summary>
        ///     Works out the size of a given file.
        /// </summary>
        /// <param name="file">File to work out size of.</param>
        /// <returns>Size of the given file.</returns>
        public static int FileSize(string file)
        {
            Stream stream = StreamFactory.RequestStream(file, StreamMode.Open);
            int length = (int)stream.Length;
            stream.Close();
            return length;
        }

		#endregion
	}

}