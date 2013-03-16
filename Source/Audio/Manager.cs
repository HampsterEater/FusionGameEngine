/* 
 * File:  Manager.cs
 *
 * This source file contains the declaration of the AudioManager class, which 
 * keeps track of the current audio driver and contains static functions for easier
 * access to the drivers methods.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Resources;

namespace BinaryPhoenix.Fusion.Audio
{

	/// <summary>
	///		The AudioManager class keeps track of the current audio driver and contains static
	///		 functions for easier access to the drivers methods.
	/// </summary>
	public static class AudioManager
	{
		#region Members
		#region Variables

		private static IAudioDriver _driver;

		#endregion
		#region Properties

		/// <summary>
		///		Returns the current audio driver used to play back audio samples.
		/// </summary>
		public static IAudioDriver Driver
		{
			get { return _driver;  }
			set { _driver = value; }
		}

        /// <summary>
        ///		Gets or sets the current 3D sound listeners position.
        /// </summary>
        public static Vector ListenerPosition
        {
            get { return _driver.ListenerPosition; }
            set { _driver.ListenerPosition = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Loads an sound from a file or memory location.
		/// </summary>
		/// <param name="path">Memory location (or url) of sound file to load.</param>
        /// <param name="cache">If set to true this sound is added to the resource cache to save loading
        ///                     time when this sound is requested again.</param>
		/// <returns>The newly loaded sound.</returns>
		public static Sound LoadSound(object path, SoundFlags flags, bool cache)
		{
            if (ResourceManager.ResourceIsCached(path.ToString()) == true)
            {
                DebugLogger.WriteLog("Loading sound " + path.ToString() + " from cache.");
                return (Sound)ResourceManager.RetrieveResource(path.ToString());
            }

            DebugLogger.WriteLog("Loading sound from " + path.ToString() + ".");
			Sound sound = new Sound(path, flags);
            if (cache == true)
            {
                if ((flags & SoundFlags.Dynamic) != 0)
                    throw new Exception("Dynamic sounds cannot be cached.");
                ResourceManager.CacheResource(path.ToString(), sound);
            }
            return sound;
		}
        public static Sound LoadSound(object path, SoundFlags flags)
        {
            return LoadSound(path, flags, true);
        }
        public static Sound LoadSound(object path)
        {
            return LoadSound(path, 0, true);
        }

		/// <summary>
		///		Creates and returns a new sample buffer compatible with the current
		///		audio driver filled with the audio contents of the given sample.
		/// </summary>
		/// <param name="sample">The sample that should be used to create the sound buffer from.</param>
        /// <param name="flags">Describes how this sample buffer should act.</param>
		/// <returns>A newly created ISampleBuffer instance.</returns>
		public static ISampleBuffer CreateSampleBuffer(Sample sample, SoundFlags flags)
		{
			return _driver.CreateSampleBuffer(sample, flags);
		}

		#endregion
	}

}