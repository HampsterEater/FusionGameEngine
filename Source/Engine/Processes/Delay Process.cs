/* 
 * File: Delay Process.cs
 *
 * This source file contains the declaration of the DelayProcess class which 
 * can be used to delay the running of another process.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Engine.Processes
{

	/// <summary>
	///		Used to delay the running of another process.
	/// </summary>
	public sealed class DelayProcess : Process
	{
		#region Members
		#region Variables

        private HighPreformanceTimer _timer = new HighPreformanceTimer();
        private int _delay = 0;
        
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
            if (_timer.DurationMillisecond > _delay)
                Finish(ProcessResult.Success);
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
        /// <param name="delay">Time this process should delay for.</param>
        public DelayProcess(int delay)
        {
            _delay = delay;
        }

		#endregion
	}

}
