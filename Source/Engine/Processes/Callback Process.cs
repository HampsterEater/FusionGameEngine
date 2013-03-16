/* 
 * File: Call back Process.cs
 *
 * This source file contains the declaration of the CallbackProcess class which 
 * can be attached to multiple methods so that they are invoked when this process
 * is run.
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
	///		Used by the CallbackProcess class to notify a method of a callback.
	/// </summary>
	/// <param name="deltaTime">Time elapsed since the last frame.</param>
	public delegate void CallbackProcessEventHandler(float deltaTime);

	/// <summary>
	///		Used to automate the animation of an game entitys image frames.
	/// </summary>
	public sealed class CallbackProcess : Process
	{
		#region Members
		#region Variables

		#endregion
		#region Properties

		#endregion
		#region Events

		public event CallbackProcessEventHandler CallbackMethod;

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Updates the animation of the entity associated with this process.
		/// </summary>
		/// <param name="deltaTime">Elapsed time since last frame.</param>
		public override void Run(float deltaTime)
		{
			if (CallbackMethod != null) CallbackMethod(deltaTime);
		}

		/// <summary>
		///		Initializes a new instance of this class with the data given.
		/// </summary>
		/// <param name="handler">Handle of method to callback.</param>
		public CallbackProcess(CallbackProcessEventHandler handler)
		{
			CallbackMethod += handler;
		}

		#endregion
	}

}
