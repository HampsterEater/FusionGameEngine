/* 
 * File: Channel Fade Process.cs
 *
 * This source file contains the declaration of the ChannelFadeProcess class which 
 * can be attached to audio channels to fade them in or out.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Audio;

namespace BinaryPhoenix.Fusion.Engine.Processes
{

	/// <summary>
	///		Used to force an sound channel to fade in or out.
	/// </summary>
	public sealed class ChannelFadeProcess : Process
	{
		#region Members
		#region Variables

        private ISampleBuffer _channel;
        private HighPreformanceTimer _timer = new HighPreformanceTimer();
        private int _duration;
        private float _startVolume, _endVolume;
        
		#endregion
		#region Properties

        /// <summary>
        ///     Gets if this process should be disposed of when the map changes.
        /// </summary>
        protected override bool DisposeOnMapChange
        {
            get { return false; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Updates the animation of the entity associated with this process.
		/// </summary>
		/// <param name="deltaTime">Elapsed time since last frame.</param>
		public override void Run(float deltaTime)
		{
            // Couple of useful variables.
            float expired = (int)_timer.DurationMillisecond;
            float duration = (float)_duration;
            if (expired > _duration)
            {
                _channel.Volume = _endVolume;
                Finish(ProcessResult.Success);
                return;
            }

            // Set the volume.
            _channel.Volume = (_startVolume + (((_endVolume - _startVolume) / duration) * expired));
		}

        /// <summary>
        ///		Called when the process this process is waiting for finishes.
        /// </summary>
        public override void WaitFinished()
        {
            _timer.Restart();
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public ChannelFadeProcess(ISampleBuffer channel, float start, float end, int duration)
        {
            _channel = channel;
            _startVolume = start;
            _endVolume = end;
            _duration = duration;
            channel.Volume = start;
        }

		#endregion
	}

}
