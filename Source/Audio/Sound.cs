/* 
 * File: Sound.cs
 *
 * This source file contains the declaration of the Sound class which is
 * used as an intermediary between the driver and audio sample.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Audio
{

    /// <summary>
    ///     Descripes how a sound should be loaded and played.
    /// </summary>
    public enum SoundFlags : int
    {
        Dynamic = 1,
        Streamed = 2,
        Positional = 4,
    }

	/// <summary>
	///		The Sound class is
	///		used as an intermediary between the driver and audio sample.
	/// </summary>
	public sealed class Sound
	{
		#region Members
		#region Variables

        private string _url;

        private SoundFlags _flags = SoundFlags.Dynamic;
        private ISampleBuffer _globalBuffer = null;

		private Sample _sample;
        private int _sampleLength = 0;
        private ArrayList _buffers = new ArrayList();

        private float _pan = 0.0f; 
        private float _volume = 100.0f;
        private float _innerRadius = 0.0f;
        private float _outerRadius = 0.0f;
        private bool _looping = false;
        private int _frequency = -1;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the global buffer that all sub-buffers are created from.
		/// </summary>
		public ISampleBuffer GlobalBuffer
		{
			get { return _globalBuffer;  }
			set { _globalBuffer = value; } 
		}

        /// <summary>
        ///		Gets or Sets the audio sample used by this sound.
        /// </summary>
        public Sample Sample
        {
            get 
            {
                if ((_flags & SoundFlags.Dynamic) == 0)
                    throw new Exception("Sample data can't be modified in none dynamic sound.");
                return _sample; 
            }
            set 
            {
                if ((_flags & SoundFlags.Dynamic) == 0)
                    throw new Exception("Sample data can't be modified in none dynamic sound.");
                _sample = value; 
            }
        }

		/// <summary>
		///		Gets or sets the list of buffers currently being played..
		/// </summary>
		public ArrayList SampleBuffers
		{
			get { return _buffers; }
			set {_buffers = value; } 
		}

        /// <summary>
        ///		Sets the pan of all buffers.
        /// </summary>
        public float Pan
        {
            set { _pan = value; }
            get { return _pan; }
        }

        /// <summary>
        ///		Sets the volume of all buffers.
        /// </summary>
        public float Volume
        {
            set { _volume = value; }
            get { return _volume; }
        }

        /// <summary>
        ///     Gets or sets the current inner radius of this sound.
        /// </summary>
        public float InnerRadius
        {
            set { _innerRadius = value; }
            get { return _innerRadius; }
        }

        /// <summary>
        ///     Gets or sets the current outer radius of this sound.
        /// </summary>
        public float OuterRadius
        {
            set { _outerRadius = value; }
            get { return _outerRadius; }
        }

        /// <summary>
        ///     Gets if this sound is begining streamed.
        /// </summary>
        public bool Streaming
        {
            get { return (_flags & SoundFlags.Streamed) != 0; }
        }

        /// <summary>
        ///     Gets if this sound is positional.
        /// </summary>
        public bool Positional
        {
            get { return (_flags & SoundFlags.Positional) != 0; }
        }

		/// <summary>
		///		Gets the length (in bytes) of this sound sample.
		/// </summary>
		public int Length
		{
			get 
            { 
                return ((_flags & SoundFlags.Dynamic) != 0) ? _sample.Data.Length : _sampleLength;
            }
		}

        /// <summary>
        ///		Sets the looping state of all buffers.
        /// </summary>
        public bool Looping
        {
            set { _looping = value; }
            get { return _looping; }
        }

        /// <summary>
        ///		Sets the frequency of all buffers.
        /// </summary>
        public int Frequency
        {
            set { _frequency = value; }
            get { return _frequency; }
        }

        /// <summary>
        ///     Gets the URL this sound was loaded from.
        /// </summary>
        public string URL
        {
            get { return _url; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Starts playing this sound.
		/// </summary>
		public ISampleBuffer Play()
		{
            ISampleBuffer playbackBuffer = null;

            // Global buffer not playing?
            if (_globalBuffer != null && _globalBuffer.Finished == true)
            {
                playbackBuffer = _globalBuffer;
            }

            // See if we can just replay one that has finished.
            if (playbackBuffer == null)
            {
                foreach (ISampleBuffer buffer in _buffers)
                    if (buffer.Finished == true)
                    {
                        playbackBuffer = buffer;
                    }
            }

            // No available buffers? Create a new one.
            if (playbackBuffer == null)
            {
                // Refill the samples data from the global buffer if we are not dynamic.
                if ((_flags & SoundFlags.Dynamic) == 0 && _globalBuffer != null)
                    _sample.Data = _globalBuffer.GetCurrentData();

                playbackBuffer = _globalBuffer.Clone(); 

                if ((_flags & SoundFlags.Dynamic) == 0 && _globalBuffer != null)
                    _sample.Data = new byte[0];

                _buffers.Add(playbackBuffer);
            }

            // Play buffer
            playbackBuffer.Play();

            // Set values.
            if (_pan != 0.0f) playbackBuffer.Pan = _pan;
            if (_volume != 0.0f) playbackBuffer.Volume = _volume;
            if (_innerRadius != 0.0f) playbackBuffer.InnerRadius = _innerRadius;
            if (_outerRadius != 0.0f) playbackBuffer.OuterRadius = _outerRadius;
            playbackBuffer.Looping = _looping;
            if (_frequency != 0.0f) playbackBuffer.Frequency = _frequency;

            // To many buffers? Delete some.
            while (_buffers.Count > 4)
            {
                ((ISampleBuffer)_buffers[0]).Stop();
                _buffers.RemoveAt(0);
            }
            
            // Return it.
            return playbackBuffer;
        }

		/// <summary>
		///		Stops all buffers containing this sound.
		/// </summary>
		public void Stop()
		{
            foreach (ISampleBuffer buffer in _buffers)
                buffer.Stop();
		}

		/// <summary>
		///		Initializes a new sound instance, and loads its sample from the given
		///		file path (and or memory url).
		/// </summary>
		/// <param name="path">File path or memory url to sound to load.</param>
        /// <param name="flags">Describes how this sound should be loaded and played.</param>
		public Sound(object path, SoundFlags flags)
		{
            // Save details.
            _flags = flags;
            _url = path as string;

            // Load the audio sample then.
			_sample = SampleFactory.LoadSample(path, (flags & SoundFlags.Streamed) != 0);
            
            // Load one buffer for quick response.
            ISampleBuffer buffer = AudioManager.Driver.CreateSampleBuffer(_sample, flags);
            _frequency = buffer.Frequency;
            _buffers.Add(buffer);

            // If its not dynamic then destroy the samples data as its just cluttering up space.
            if ((_flags & SoundFlags.Dynamic) == 0)
            {
                if (_sample != null)
                {
                    _sampleLength = _sample.Data.Length;
                    _sample.Data = null;
                }
                //GC.Collect();
                _globalBuffer = buffer; // Save the global buffer as its what we will get our data from.
            }
		}

        ~Sound()
        {
            //foreach (ISampleBuffer buffer in _buffers)
            //    buffer.Dispose();
            _buffers.Clear();
        }

		#endregion
	}

}