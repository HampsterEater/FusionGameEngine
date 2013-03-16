/* 
 * File: Manager.cs
 *
 * This source file contains the declaration of the EventManager class which 
 * is used to minipulate and fire events in a correct sequence.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Runtime.Events
{

	/// <summary>
	///		Used as a base for any fireable events in the game engine.
	/// </summary>
	public sealed class Event
	{
		#region Members
		#region Variables

		private string _id;
		private object _source;
		private object _data;
		
		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the unique ID representing this event.
		/// </summary>
		public string ID
		{
			get { return _id;  }
			set { _id = value; }
		}

		/// <summary>
		///		Get or sets the object that this event originated from.
		/// </summary>
		public object Source
		{
			get { return _source;  }
			set { _source = value; }
		}

		/// <summary>
		///		Gets or sets the data associated with this event.
		/// </summary>
		public object Data
		{
			get { return _data;  }
			set { _data = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Returns a textural version of this events data.
		/// </summary>
		/// <returns>A textural version of this events data.</returns>
		public override string ToString()
		{
			return _id;
		}

		/// <summary>
		///		Initializes a new instance of this class with the given id, source and data.
		/// </summary>
		/// <param name="id">ID to identify this event with.</param>
		/// <param name="source">Event that fired this event.</param>
		/// <param name="data">Any data contained in this event.</param>
		public Event(string id, object source, object data)
		{
			_id = id;
			_source = source;
			_data = data;
		}

		#endregion
	}

	/// <summary>
	///		Used to describe a callable native function that is used to process events.
	/// </summary>
	/// <param name="event">Event to process.</param>
	public delegate void ProcessorDelegate(Event fireEvent);

	/// <summary>
	///		This class is used by the event manager to give objects 
	///		notification when events are fired.
	/// </summary>
	public class EventListener
	{
		#region Members
		#region Variables

		private ProcessorDelegate _delegate;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the delegate of the function used to process events.
		/// </summary>
		public ProcessorDelegate Delegate
		{
			get { return _delegate;  }
			set { _delegate = value; }
		}

		#endregion
		#endregion
		#region Methods


        /// <summary>
        ///     Converts this class to a textural form.
        /// </summary>
        /// <returns>Textural form of this class.</returns>
        public override string ToString()
        {
            return "Listener [" + (_delegate.Target != null ? _delegate.Target.ToString() : "") + "]";
        }

		/// <summary>
		///		Notifys this listeners delegate about the given event.
		/// </summary>
		/// <param name="processEvent">Event to process.</param>
		public void ProcessEvent(Event processEvent)
		{
			_delegate(processEvent);
		}

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		/// <param name="processorDelegate">Delegate of method to notify of processable events.</param>
		public EventListener(ProcessorDelegate processorDelegate)
		{
			_delegate = processorDelegate;
		}

		#endregion
	}

	/// <summary>
	///		The event manager is used to keep track of pending and currently
	///		fired events and routing them to their correct listeners.
	/// </summary>
	public static class EventManager
	{
		#region Members
		#region Variables

		private static ArrayList _listenerList = new ArrayList();
		private static Stack _eventStack = new Stack();

		#endregion
		#region Properties

        /// <summary>
        ///		Gets a list of all listeners attached to this manager.
        /// </summary>
        public static ArrayList Listeners
        {
            get { return _listenerList; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Attachs a given event listener to the listener list so that it
		///		recieves notification on all fired events.
		/// </summary>
		/// <param name="listener">Listener to attach.</param>
		public static void AttachListener(EventListener listener)
		{
			_listenerList.Add(listener);
		}

		/// <summary>
		///		Removes the given event listener from the listener list so it no 
		///		longer recieves event notifications.
		/// </summary>
		/// <param name="listener">Listener to dettach.</param>
		public static void DetachListener(EventListener listener)
		{
			_listenerList.Remove(listener);
		}

		/// <summary>
		///		Add the given event to the event stack so it can be processed later.
		/// </summary>
		/// <param name="fireEvent">Event to fire.</param>
		public static void FireEvent(Event fireEvent)
		{
			//DebugLogger.WriteLog("Event fired: " + fireEvent.ToString());
			_eventStack.Push(fireEvent);
		}

		/// <summary>
		///		Processes all events that are currently waiting to be processed.
		/// </summary>
		public static void ProcessEvents()
		{
			while (_eventStack.Count > 0)
			{
				Event fireEvent = (Event)_eventStack.Pop();
				if (fireEvent == null) continue;
                try
                {
                    foreach (EventListener listener in _listenerList)
                        listener.ProcessEvent(fireEvent);
                   
                }
                catch (InvalidOperationException) { } // This is just here to catch enumeration-changed exceptions.
            }
		}

		#endregion
	}

}