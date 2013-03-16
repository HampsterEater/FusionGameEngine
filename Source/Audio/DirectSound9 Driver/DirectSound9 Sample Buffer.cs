/* 
 * File: DirectSound9 Sound.cs
 *
 * This source file contains the declaration of the DirectSound9Sound class
 * which is used by the directsound driver to store details on a playable 
 * audio buffer.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Audio.DirectSound9Driver
{

	/// <summary>
	///		Used by the directsound driver to store details on a playable 
	///		audio buffer.
	/// </summary>
	class DirectSound9SampleBuffer : ISampleBuffer
	{
		#region Members
		#region Variables

		private DirectSound9Driver _dx9Driver;

		private Sample _sample;
		private SecondaryBuffer _buffer;
        private Buffer3D _3dBuffer = null;

        private int _bufferLength = 0;

        private SoundFlags _flags = SoundFlags.Dynamic;

        private bool _started = false;
		private bool _isPaused  = false;
		private bool _isPlaying = false;
		private bool _isLooping = false;
		private float _volume   = 100;
		private float _pan	    = 0;
		private int _frequency  = 44600;

        private Thread _streamingThread = null;
        private int _streamedBufferSize = 256;
        private int _streamThreshold = 256;
        private int _streamingPosition = 0;
        private int _streamingDataPosition = 0;
        private bool _streamFinishing = false;
        private bool _firstStreamingChunkLoaded = false;

		#endregion
		#region Properties

		/// <summary>
		///		Returns the audio driver associated with this sample buffer.
		/// </summary>
		IAudioDriver ISampleBuffer.Driver
		{
			get { return _dx9Driver; }
		}

		/// <summary>
		///		Gets or sets the sample used by this sample buffer.
		/// </summary>
		Sample ISampleBuffer.Sample
		{
			get { return _sample; }
			set
			{
                if ((_flags & SoundFlags.Dynamic) == 0)
                    throw new Exception("Sample data can't be modified in none dynamic sound buffer.");

				_sample = value;
				RefreshSampleBuffer();
			}
		}
		
		/// <summary>
		///		Gets or sets if this sample buffer is paused or not.
		/// </summary>
		bool ISampleBuffer.Paused
		{
			get { return _isPaused; }
			set 
			{ 
				_isPaused = value;
				if (_isPlaying == true)
				{
					if (_isPaused == true)
						_buffer.Stop();
					else
                        _buffer.Play(0, (_flags & SoundFlags.Streamed) != 0 ? (_streamFinishing == true ? 0 : BufferPlayFlags.Looping)  : (_isLooping ? BufferPlayFlags.Looping : 0));
				}
			}
		}

		/// <summary>
		///		Gets or sets the pan of this sample buffer.
		/// </summary>
		float ISampleBuffer.Pan
		{
			get { return _pan; }
			set 
			{ 
				_pan = value;
				_buffer.Pan = -(int)(_pan * 10000.0f);
			}
		}

		/// <summary>
		///		Gets or sets the volume of this sample buffer.
		/// </summary>
		float ISampleBuffer.Volume
		{
			get { return _volume; }
			set 
			{ 
				_volume = value;
				_buffer.Volume = (int)(-((101.0f - _volume) * ( 5000.0f / 100.0f )));
			}
		}

		/// <summary>
		///		Gets or sets the position (in bytes) where this sample buffer is playing from.
		/// </summary>
		int ISampleBuffer.Position
		{
			get { return _buffer.PlayPosition; }
			set { _buffer.SetCurrentPosition(value); }
		}

		/// <summary>
		///		Gets the length (in bytes) of this sample buffer.
		/// </summary>
		int ISampleBuffer.Length
		{
			get 
            { 
                return ((_flags & SoundFlags.Dynamic) != 0) ? _sample.Data.Length : _bufferLength; 
            }
		}

		/// <summary>
		///		Retrieves a boolean describing if this buffer has finished playing.
		/// </summary>
		bool ISampleBuffer.Finished
		{
			get 
            { 
                return !_started ? true : (_buffer.Status.Playing == false && _isPlaying == true && _isLooping == false); 
            }
		}

		/// <summary>
		///		Gets or sets if this sample buffer should loop or not.
		/// </summary>
		bool ISampleBuffer.Looping
		{
			get { return _isLooping; }
			set 
			{ 
				_isLooping = value;
				if (_isPlaying == true)
				{
					if (_isPaused == false) _buffer.Stop();
                    _buffer.Play(0, (_flags & SoundFlags.Streamed) != 0 ? (_streamFinishing == true ? 0 : BufferPlayFlags.Looping) : (_isLooping ? BufferPlayFlags.Looping : 0));
				}
			}
		}

		/// <summary>
		///		Gets or sets the frequency this sample buffer should play at.
		/// </summary>
		int ISampleBuffer.Frequency
		{
			get { return _frequency; }
			set
			{
				_frequency = value;
				_buffer.Frequency = value;
			}
		}

        /// <summary>
        ///     Gets or sets the current 3D position of this sound.
        /// </summary>
        Vector ISampleBuffer.Position3D
        {
            get 
            {
                if ((_flags & SoundFlags.Positional) == 0)
                    return new Vector(0, 0, 0);
                return new Vector(_3dBuffer.Position.X, _3dBuffer.Position.Y, _3dBuffer.Position.Z); 
            }
            set 
            {
                if ((_flags & SoundFlags.Positional) == 0)
                    return;
                _3dBuffer.Position = new Vector3(value.X, value.Y, value.Z); 
            }
        }

        /// <summary>
        ///     Gets or sets the inner radius of this sound.
        /// </summary>
        float ISampleBuffer.InnerRadius
        {
            get
            {
                if ((_flags & SoundFlags.Positional) == 0)
                    return 0;
                return _3dBuffer.MinDistance;
            }
            set
            {
                if ((_flags & SoundFlags.Positional) == 0)
                    return;
                _3dBuffer.MinDistance = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Outer radius of this sound.
        /// </summary>
        float ISampleBuffer.OuterRadius
        {
            get
            {
                if ((_flags & SoundFlags.Positional) == 0)
                    return 0;
                return _3dBuffer.MaxDistance;
            }
            set
            {
                if ((_flags & SoundFlags.Positional) == 0)
                    return;
                _3dBuffer.MaxDistance = value;
            }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Starts playing this sample buffer.
		/// </summary>
		void ISampleBuffer.Play()
		{
			_buffer.Play(0, (_flags & SoundFlags.Streamed) != 0 ? (_streamFinishing == true ? 0 : BufferPlayFlags.Looping) : (_isLooping ? BufferPlayFlags.Looping : 0));
			_isPlaying = true;
            _started = true;
            if (_streamingThread != null && _streamingThread.IsAlive == false)
                _streamingThread.Start();
		}

		/// <summary>
		///		Stops playing this sample buffer.
		/// </summary>
		void ISampleBuffer.Stop()
		{
            if (_buffer != null)
			    _buffer.Stop();

            if (_streamingThread != null)
            {
                _streamingPosition = 0;
                _streamingThread.Abort();
            }
            _isPlaying = false;
            _started = false;
		}

        /// <summary>
        ///     Clones this sample buffer.
        /// </summary>
        ISampleBuffer ISampleBuffer.Clone()
        {
            return new DirectSound9SampleBuffer(_dx9Driver, _sample, _flags);
        }

        /// <summary>
        ///		Rewrites the audio data into the buffer and resets the format.
        /// </summary>
        public void RefreshSampleBuffer()
        {
            if (_sample == null)
                return;

            // Create the format description used by the buffer.
            WaveFormat format = new WaveFormat();
            format.BitsPerSample = (short)_sample.BitsPerSample;
            format.Channels = (short)_sample.ChannelCount;
            format.FormatTag = WaveFormatTag.Pcm;
            format.SamplesPerSecond = _sample.SampleRate;
            format.BlockAlign = (short)(format.Channels * (format.BitsPerSample / 8));
            format.AverageBytesPerSecond = format.SamplesPerSecond * format.BlockAlign;

            // Only mono sounds can be used for 3D.
            if ((_flags & SoundFlags.Positional) != 0 && format.Channels != 1)
                _flags &= ~SoundFlags.Positional;

            // Store some sample details.
            _frequency = _sample.SampleRate;
            _streamedBufferSize = MSToBytes(1000, format); // 1 seconds of audio.
            _streamThreshold = MSToBytes(400, format); // 400 millisecond threshold

            // Create the buffer description.
            BufferDescription desc = new BufferDescription(format);
            desc.BufferBytes = ((_flags & SoundFlags.Streamed) != 0) ? _streamedBufferSize : _sample.Data.Length;
            if ((_flags & SoundFlags.Positional) != 0)
            {
                desc.StickyFocus = true;
                desc.Control3D = true;
                desc.Mute3DAtMaximumDistance = true;
                desc.Guid3DAlgorithm = DSoundHelper.Guid3DAlgorithmHrtfFull;
            }
            else
            {
                desc.ControlVolume = true;
                desc.ControlPan = true;
                desc.ControlFrequency = true;
            }

            // Create DirectSound buffer.
            _buffer = new SecondaryBuffer(desc, _dx9Driver.Device);

            // Create the 3D buffer.
            if ((_flags & SoundFlags.Positional) != 0)
            {
                _3dBuffer = new Buffer3D(_buffer);
                _3dBuffer.Mode = Mode3D.Normal;
                _3dBuffer.MaxDistance = 300;
                _3dBuffer.MinDistance = 30;
            }

            // Write in the samples data.
            if (_sample.Data.Length > 0)
            {
                _buffer.Write(0, _sample.Data, LockFlag.EntireBuffer);
                _bufferLength = _sample.Data.Length;
            }
        }

        /// <summary>
        ///     Retrieves the current data this buffer holds.
        /// </summary>
        byte[] ISampleBuffer.GetCurrentData()
        {
            if (_bufferLength == 0)
                return new byte[0];

            Stream stream = new MemoryStream(_bufferLength);
            _buffer.Read(0, stream, _bufferLength, LockFlag.EntireBuffer);
            byte[] data = new byte[_bufferLength];
            stream.Read(data, 0, _bufferLength);
            stream.Close();
            return data;
        }

		/// <summary>
		///		Initializes a new sample buffer and writes all the audio data in the given
		///		sample into it.
		/// </summary>
		/// <param name="driver">Audio driver to associate this sound buffer with.</param>
		/// <param name="sample">Sample to load audio data from.</param>
		public DirectSound9SampleBuffer(DirectSound9Driver driver, Sample sample, SoundFlags flags)
		{
			// Store the sample and driver for later use.
			_sample    = sample;
			_dx9Driver = driver;
            _flags     = flags;

            // Naughty people :S.
           // if (sample == null)
            //    return;

            // Create a streaming thread.
            if ((flags & SoundFlags.Streamed) != 0)
            {
                _streamingThread = new Thread(StreamingThread);
                _streamingThread.IsBackground = true;
                _streamingThread.Start();
            }
            else if (_sample != null)
                GC.AddMemoryPressure(_sample.Data.Length);

			// Write the data into the sample buffer.
			RefreshSampleBuffer();
		}

        /// <summary>
        ///     Looks after refilling the audio buffer.
        /// </summary>  
        public void StreamingThread()
        {
            // Set stream position to the start :).
            _sample.DataStream.Position = _sample.DataChunkPosition;

            while (true)
            {
                if (_buffer != null && _sample != null && _isPaused == false && (_isPlaying == true || _firstStreamingChunkLoaded == false))
                {
                    // Mark that we have loaded the first chunk.
                    _firstStreamingChunkLoaded = true;

                    // Work out how much to read.
                    int bytes = _buffer.PlayPosition <= _streamingPosition ?
                                _buffer.PlayPosition + _streamedBufferSize - _streamingPosition :
                                _buffer.PlayPosition - _streamingPosition;

                    bytes = Math.Min(bytes, _streamThreshold);

                    if (_streamFinishing == true && _buffer.Status.Playing == false)
                    {
                        ((ISampleBuffer)this).Stop();
                        break;
                    }

                    if (bytes > 0 && _streamFinishing == false)
                    {
                        // Ask for the data.                             
                        byte[] data = _sample.StreamedDataRequired(_sample, bytes, ref _streamingDataPosition, _isLooping);
                        if (data.Length < bytes && _isLooping == false)
                        {
                            _streamFinishing = true;
                            _buffer.Play(0, 0);
                        }

                        // Copy data to the buffer
                        Stream dataStream = new MemoryStream(data);
                        dataStream.Position = 0;
                        _buffer.Write(_streamingPosition, dataStream, data.Length, LockFlag.None);
                        dataStream.Close();

                        _streamingPosition += data.Length;
                        if (_streamingPosition >= _streamedBufferSize)
                            _streamingPosition -= _streamedBufferSize;
                    }
                }

                // Sleep until we are needed again :P.
                Thread.Sleep(_buffer == null ? 50 : BytesToMS(_streamedBufferSize) / 6);
            }
        }

        /// <summary>
        ///     Converts the given number of bytes to the appropriate time in milliseconds.
        /// </summary>
        /// <param name="bytes">Amount of bytes.</param>
        /// <param name="format">If set this will override the default wave format that is used to calculate the result.</param>
        /// <returns>Equivilent millisecionds of the given number of bytes.</returns>
        private int BytesToMS(int bytes, WaveFormat format)
        {
            return bytes * 1000 / format.AverageBytesPerSecond;
        }
        private int BytesToMS(int bytes)
        {
            return BytesToMS(bytes, _buffer.Format);
        }

        /// <summary>
        ///     Converts the given millisecond count into bytes.
        /// </summary>
        /// <param name="ms">Milliseconds to convert.</param>
        /// <param name="format">If set this will override the default wave format that is used to calculate the result.</param>
        /// <returns>Equivilent bytes of the given number of milliseconds.</returns>
        private int MSToBytes(int ms, WaveFormat format)
        {
            int bytes = ms * format.AverageBytesPerSecond / 1000;
            bytes -= bytes % format.BlockAlign;
            return bytes;
        }
        private int MSToBytes(int ms)
        {
            return MSToBytes(ms, _buffer.Format);
        }

        /// <summary>
        ///     Called when this buffer is disposed of.
        /// </summary>
        ~DirectSound9SampleBuffer()
        {
            int bytes = ((_flags & SoundFlags.Dynamic) != 0) ? _sample.Data.Length : _bufferLength;
            if (bytes > 0) GC.RemoveMemoryPressure(bytes);
        }

		#endregion
	}

}