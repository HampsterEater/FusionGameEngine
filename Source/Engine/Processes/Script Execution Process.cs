/* 
 * File: Script Execution Process.cs
 *
 * This source file contains the declaration of the Script Execiton Process class which 
 * can be attached to game entitys to allow their logic to be run from a FusionScript script.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Engine.ScriptingFunctions;
using BinaryPhoenix.Fusion.Runtime.Events;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Engine.Processes
{

	/// <summary>
	///		Can be attached to game entitys to allow their 
	///		logic to be run from a FusionScript script.
	/// </summary>
	public class ScriptExecutionProcess : Process
	{
		#region Members
		#region Variables

        private static bool _useEditorState = false;

		private ScriptProcess _process = null;
		
		private EntityNode _entity = null;

		private EventListener _eventListener = null;

        private FunctionSymbol _tickFunction = null;

		#endregion
		#region Properties

        /// <summary>
        ///     If sets then all script execution processes will default to the editor state.
        /// </summary>
        public static bool DefaultToEditorState
        {
            get { return _useEditorState; }
            set { _useEditorState = value; }
        }

		/// <summary>
		///		Gets or sets the entity that this process is attached to.
		/// </summary>
		public EntityNode Entity
		{
			get { return _entity; }
			set { _entity = value; }
		}

		/// <summary>
		///		Gets or sets the script process that this entity's logic is running off.
		/// </summary>
		public ScriptProcess Process
		{
			get { return _process; }
			set 
			{
				if (_process != null) VirtualMachine.GlobalInstance.DetachProcess(_process);
				_process = value;
				if (_process != null)
				{
                    if (_entity != null)
                    {
                        SceneNodeScriptObject obj = new SceneNodeScriptObject(_entity);
                        obj.Collectable = false;
                        obj.ReferenceCount = 999999;
                        _process[0].SetGlobalVariable("this", obj);
                    }
                    if (_process.State == null)
                    {
                        if (_useEditorState == true)
                            _process.State = _process.DefaultEditorState;
                        else
                            _process.State = _process.DefaultEngineState;                      
                    }
                    if (_process != null)
                    {
                        _process.OnStateChange += this.OnStateChange;
                        OnStateChange(_process, _process.State);
                    }
                    VirtualMachine.GlobalInstance.AttachProcess(_process);
				}
			}
		}

        /// <summary>
        ///     Gets if this process should be disposed of when the map changes.
        /// </summary>
        protected override bool DisposeOnMapChange
        {
            get { return (_entity == null || _entity.IsPersistent == false); }
        }

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Called when the state of this entities script is changed.
        /// </summary>
        /// <param name="process">Process that had its state changed.</param>
        /// <param name="sate">New state.</param>
        public void OnStateChange(ScriptProcess process, StateSymbol state)
        {
            _tickFunction = (state == null ? null : (state.FindSymbol("OnTick", SymbolType.Function) as FunctionSymbol));
        }

		/// <summary>
		///		Updates the logic of the entity associated with this process.
		/// </summary>
		/// <param name="deltaTime">Elapsed time since last frame.</param>
		public override void Run(float deltaTime)
		{
            if (_isFinished == true || (_entity != null && _entity.IsEnabled == false)) return;
            if (_tickFunction != null && _process != null) _process[0].InvokeFunction(_tickFunction, true, true, true);
            if (_isFinished == true || (_entity != null && _entity.IsEnabled == false)) return; // It may have been destroyed / disabled in the tick function, so check again. 
			_process[0].Run(-1); // Run until the script finishs what it is doing.
        }

		/// <summary>
		///		Called by the event manager when an event is fired that this class should process.
		/// </summary>
		/// <param name="firedEvent">Event to process.</param>
		public void ProcessEvent(Event firedEvent)
		{
            if (_process == null) return;
			switch (firedEvent.ID)
			{
				case "game_begin":
                    if (_process != null) _process[0].InvokeFunction("OnGameBegin", true, false);
					break;
				case "game_finish":
                    if (_process.SymbolExists("OnGameFinish", SymbolType.Function) == false) 
                        _process.Stop();
					else
                        _process[0].InvokeFunction("OnGameFinish", true, false);
					break;
				case "map_begin":
                    if (_process != null) _process[0].InvokeFunction("OnMapBegin", true, false);
					break;
				case "map_finish":
                    if (_process != null) _process[0].InvokeFunction("OnMapFinish", true, false);
					break;
				case "key_pressed":
				case "key_released":
				case "mouse_move":
				//	if (_process != null) _process[0].InvokeFunction("OnInputRecieved");
					break;
			}
		}

		/// <summary>
		///		Initializes a new instance of this class with the given data.
		/// </summary>
		/// <param name="entity">Entity to attach this process to.</param>
		/// <param name="process">Script process that this entitys logic script is contained in.</param>
		public ScriptExecutionProcess(EntityNode entity, ScriptProcess process)
		{

			_entity = entity;
			Process = process;

			_eventListener = new EventListener(ProcessEvent);
			EventManager.AttachListener(_eventListener);

            _priority = 1; // We always want to be executed before other processes.
		}
		public ScriptExecutionProcess(EntityNode entity)
		{

			_entity = entity;

			_eventListener = new EventListener(ProcessEvent);
			EventManager.AttachListener(_eventListener);

            _priority = 1; // We always want to be executed before other processes.
		}
		public ScriptExecutionProcess()
		{

			_eventListener = new EventListener(ProcessEvent);
			EventManager.AttachListener(_eventListener);

            _priority = 1; // We always want to be executed before other processes.
		}

		/// <summary>
		///		Initializes a new instance of this class with the given data.
		/// </summary>
		/// <param name="url">Url of script this process should run.</param>
		public ScriptExecutionProcess(object url)
		{
            Process = VirtualMachine.GlobalInstance.LoadScript(url);

			_eventListener = new EventListener(ProcessEvent);
			EventManager.AttachListener(_eventListener);

            _priority = 1; // We always want to be executed before other processes.
		}

        /// <summary>
        ///     Called when this process needs to be destroyed.
        /// </summary>
        public override void Dispose()
        {
            if (_eventListener != null) EventManager.DetachListener(_eventListener);
            if (_process != null)
                _process.Dispose();
            _eventListener = null;
            _process = null;
            _entity = null;
        }

        /// <summary>
        ///     Converts this instance to a textural form.
        /// </summary>
        /// <returns>Textural form of this instance.</returns>
        public override string ToString()
        {
            return _process != null && _process.Url != null ? "ScriptExecutionProcess["+_process.Url.ToString()+"]" : base.ToString();
        }

		#endregion
	}

}
