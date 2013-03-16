/* 
 * File: DirectSound9 Driver.cs
 *
 * This source file contains the declaration of the DirectSound9Driver class, which adds
 * the capability of using DirectSound to playback audio samples to the audio system.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Audio.DirectSound9Driver
{

	/// <summary>
	///     The DirectSound9Driver wraps all the calls to the DirectSound API
	///		up in the IAudioDriver interface so that DirectSound can be used 
	///		to playback audio samples.
	/// </summary>
	public sealed class DirectSound9Driver : IAudioDriver
	{
		#region Members
		#region Variables

		private Device _dx9Device;
        private Listener3D _listener3D = null;

		#endregion
		#region Properties

		/// <summary>
		///		Retrieves the DirectSound device used by this driver to 
		///		playback audio samples.
		/// </summary>
		public Device Device
		{
			get { return _dx9Device; }
		}

        /// <summary>
        ///     Gets or sets the current 3D position of this sound.
        /// </summary>
        Vector IAudioDriver.ListenerPosition
        {
            get { return new Vector(_listener3D.Position.X, _listener3D.Position.Y, _listener3D.Position.Z); }
            set 
            { 
                _listener3D.Position = new Vector3(value.X, value.Y, value.Z);
                _listener3D.CommitDeferredSettings();
            }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Calls the Initialize method to setup this audio driver and its
		///		playback API.
		/// </summary>
		public DirectSound9Driver()
		{
			InitializeDevice();
		}

		/// <summary>
		///		Sets up an instance of DirectSound9Driver and sets ups 
		///		DirectSound so its render for audio playback.
		/// </summary>
		private void InitializeDevice()
		{
			// Setup the DirectSound device
            _dx9Device = new Device();
			if (GraphicsManager.RenderTarget as GraphicsCanvas != null)
				_dx9Device.SetCooperativeLevel(((GraphicsCanvas)GraphicsManager.RenderTarget).RenderControl, CooperativeLevel.Priority);

            // Create our 3D sound listener.
            BufferDescription bufferDescription = new BufferDescription();
            bufferDescription.PrimaryBuffer = true;
            bufferDescription.Control3D = true;

            // Get the primary buffer
            Microsoft.DirectX.DirectSound.Buffer buffer = new Microsoft.DirectX.DirectSound.Buffer(bufferDescription, _dx9Device);
           
            // Attach the listener to the primary buffer
            _listener3D = new Listener3D(buffer);
            _listener3D.DistanceFactor = 0.03f;
            //_listener3D.DopplerFactor = 0.1f;
            //_listener3D.RolloffFactor = 0.1f;
        }

		/// <summary>
		///		Attachs this drivers input / output control to the given one.
		/// </summary>
		/// <param name="control">Control to attach this driver to.</param>
		void IAudioDriver.AttachToControl(Control control)
		{
			_dx9Device.SetCooperativeLevel(control, CooperativeLevel.Priority);
		}

		/// <summary>
		///		Creates a new sample buffer capable of being played back by this driver.
		/// </summary>
		/// <param name="sample">Sample to create buffer from.</param>
        /// <param name="flags">Flags describing how to create buffer.</param>
		ISampleBuffer IAudioDriver.CreateSampleBuffer(Sample sample, SoundFlags flags)
		{
			return new DirectSound9SampleBuffer(this,sample,flags);
		}

		#endregion
	}

}