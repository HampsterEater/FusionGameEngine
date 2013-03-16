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
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Engine.Processes
{

	/// <summary>
	///		Used to delay the running of another process.
	/// </summary>
	public sealed class StartScriptProcess : Process
	{
		#region Members
		#region Variables

        private ScriptThread _thread = null;

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
            _thread.IsWaiting = false;
            _thread.Start();
            Finish(ProcessResult.Success);
		}

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="thread">Thread to start.</param>
        public StartScriptProcess(ScriptThread thread)
        {
            _thread = thread;
        }

		#endregion
	}

}
