/* 
 * File: Manager.cs
 *
 * This source file contains the declaration of the ProcessManager class which 
 * is used to run game processes in the correct order.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Runtime.Processes
{

	/// <summary>
	///		Used to define the success result of a process.
	/// </summary>
	public enum ProcessResult
	{
		Success = 0,
		Failed = 1
	}

	/// <summary>
	///		Stores flags on a single game process.
	/// </summary>
	public abstract class Process : IComparable
	{
		#region Members
		#region Variables

        protected bool _isFinished = false;
        protected ProcessResult _finishResult = ProcessResult.Success;

        protected bool _waitingForProcess = false;
        protected Process _waitForProcess = null;
        protected ProcessResult _waitForResult = ProcessResult.Success;

        protected HighPreformanceTimer _pauseTimer = new HighPreformanceTimer();
        protected bool _isPaused = false;
        protected int _pauseDelay = 0;

        protected bool _isStopped = false;

        protected int _priority = 0;

        protected bool _persistent = false;

        protected bool _active = false;

		#endregion
		#region Properties

        /// <summary>
        ///     Gets or sets this processes priority of execution.
        /// </summary>
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        /// <summary>
        ///     Sets or gets if this process is still active.
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

		/// <summary>
		///		Gets a boolean describing if this process has finished executing or not.
		/// </summary>
		public bool IsFinished
		{
			get { return _isFinished; }
		}

		/// <summary>
		///		Gets a boolean describing if this processes execution has been stopped.
		/// </summary>
		public bool IsStopped
		{
			get { return _isStopped; }
		}

		/// <summary>
		///		Gets a boolean describing if this process is currently paused or not.
		/// </summary>
		public bool IsPaused
		{
			get
			{
				if (_pauseTimer.DurationMillisecond > _pauseDelay)
					Resume();
				return _isPaused;
			}
		}

		/// <summary>
		///		Gets a boolean describing if this process is currently waiting for a process to complete.
		/// </summary>
		public bool IsWaitingForProcess
		{
			get { return _waitingForProcess; }
		}

		/// <summary>
		///		Gets the process this process is currently waiting to complete.
		/// </summary>
		public Process WaitingForProcess
		{
			get { return _waitForProcess; }
		}

		/// <summary>
		///		Gets the result this process expects from the process it is waiting for.
		/// </summary>
		public ProcessResult WaitingForResult
		{
			get { return _waitForResult; }
		}

		/// <summary>
		///		Gets the result this process finished with.
		/// </summary>
		public ProcessResult FinishResult
		{
			get { return _finishResult; }
		}

        /// <summary>
        ///     Gets if this process should be disposed of when the map changes.
        /// </summary>
        protected virtual bool DisposeOnMapChange
        {
            get { return true; }
        }

        /// <summary>
        ///     Gets or sets if this process is persistent.
        /// </summary>
        public bool IsPersistent
        {
            get { return _persistent == true || DisposeOnMapChange == false; }
            set { _persistent = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Runs this process's logic with the given elapsed delta time.
		/// </summary>
		/// <param name="deltaTime">Elapsed time used to update logic.</param>
		public abstract void Run(float deltaTime);

        /// <summary>
        ///     Called when this process needs to be destroyed.
        /// </summary>
        public virtual void Dispose()
        {

        }

        /// <summary>
        ///     This method allows this process to be sorted based on its priority.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        int IComparable.CompareTo(object obj)
        {
            Process process = obj as Process;
            if (process == null) return 0;
            if (process._priority > _priority)
                return 1;
            else if (process._priority < _priority)
                return -1;
            else
                return 0;
        }

        /// <summary>
        ///		Called when the process this process is waiting for finishes.
        /// </summary>
        public virtual void WaitFinished()
        {

        }

		/// <summary>
		///		Stops this process executing until the given process completed with the correct result.
		/// </summary>
		/// <param name="process">Process to wait for.</param>
		/// <param name="result">Result to wait for.</param>
        public virtual void WaitForProcess(Process process, ProcessResult result)
		{
			_waitingForProcess = true;
			_waitForProcess = process;
			_waitForResult = result;
		}

		/// <summary>
		///		Cancels a process wait caused by called WaitForProcess.
		/// </summary>
        public virtual void CancelProcessWait()
		{
			_waitingForProcess = false;
			_waitForResult = 0;
			_waitForProcess = null;
		}

		/// <summary>
		///		Pauses the execution of the current process for the given amount of time.
		/// </summary>
		/// <param name="time">Amount of time to pause for.</param>
        public virtual void Pause(int time)
		{
			_pauseTimer.Restart();
			_isPaused = true;
			_pauseDelay = time;
		}

		/// <summary>
		///		Stops the execution of the current process.
		/// </summary>
        public virtual void Stop()
		{
			_isStopped = true;
		}

		/// <summary>
		///		Resumes this process if it has been paused by calling the Pause or Stop method.
		/// </summary>
        public virtual void Resume()
		{
			_isStopped = false;
			_isPaused = false;
			_pauseDelay = 0;
		}

		/// <summary>
		///		Finishes execution of this process permenently, and returns the given result
		///		to the process manager.
		/// </summary>
		/// <param name="result">Result of process execution.</param>
		public virtual void Finish(ProcessResult result)
		{
			_isFinished = true;
			_finishResult = result;
		}

        /// <summary>
        ///     Gives the process notification that it has been attached to the manager.
        /// </summary>
        public virtual void Attached()
        {

        }

        /// <summary>
        ///     Gives the process notification that it has been dettached from the manager.
        /// </summary>
        public virtual void Dettached()
        {

        }

		#endregion
	}

	/// <summary>
	///		Used to run game processes in the correct order. A game process
	///		is simply a callback to an update method that can have certain flags
	///		attached to it to produce effects like waiting for another process
	///		to finish.
	/// </summary>
	public static class ProcessManager
	{
		#region Members
		#region Variables

		private static ArrayList _processList = new ArrayList();

		#endregion
		#region Properties

		/// <summary>
		///		Gets the process list containing all the processes attached to this manager.
		/// </summary>
		public static ArrayList Processes
		{
			get { return _processList; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Attachs a given game process to the process list so it will be
		///		called when RunProcesses is called.
		/// </summary>
		/// <param name="process">Process to attach.</param>
		public static void AttachProcess(Process process)
		{
            if (_processList.Contains(process)) return;
			_processList.Add(process);
            process.Attached();
            process.Active = true;

            // Sort list based on priority. (Ech, slow).
            _processList.Sort();
		}

		/// <summary>
		///		Removes the given event game process from the process list so it no 
		///		longer gets called when RunProcesses is called.
		/// </summary>
		/// <param name="process">Process to dettach.</param>
		public static void DettachProcess(Process process)
		{
			_processList.Remove(process);
            process.Dettached();
            process.Active = false;

            // Sort list based on priority. (Ech, slow).
            _processList.Sort();
        }

		/// <summary>
		///		Clears the process list of all currently running processes.
		/// </summary>
		public static void Clear()
		{
			_processList.Clear();
		}

		/// <summary>
		///		Processes all events that are currently waiting to be processed.
		/// </summary>
		/// <param name="deltaTime">Elapsed time since the last frame to use 
		///							when updating game objects.</param>
		public static void RunProcesses(float deltaTime)
		{
            ArrayList removeList = new ArrayList();
            ArrayList runProcesses = new ArrayList();
            ArrayList newList = new ArrayList(_processList);
			for (int i = 0; i < newList.Count; i++)
			{
				Process process = (Process)newList[i];//(Process)newProcessList[i];
                if (runProcesses.Contains(process) == true || process.Active == false) continue;
                runProcesses.Add(process);

				// Check this process is allowed to run or not.
				if (process.IsFinished == true)
				{
                    removeList.Add(process);
					continue;
				}
				if (process.IsPaused == true || process.IsStopped == true) continue;

				// Check the status of the process we are waiting for (if any).
				if (process.IsWaitingForProcess == true)
				{
					if (process.WaitingForProcess.IsFinished == false) continue;
                    if (process.WaitingForProcess.FinishResult == process.WaitingForResult)
                    {
                        process.CancelProcessWait();
                        process.WaitFinished();
                    }
                    else
                    {
                        process.Finish(ProcessResult.Failed);
                        continue;
                    }
				}

				// w00t everything is in order so lets run!
                process.Run(deltaTime);
            }

            foreach (Process process in removeList)
                _processList.Remove(process);
		}

		#endregion
	}

}