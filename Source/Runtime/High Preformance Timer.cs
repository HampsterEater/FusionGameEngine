/* 
 * File: Timing.cs
 *
 * This source file contains the declaration of the Timing class, which contains
 * a number of usefull functions used for high-preformance timing.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

//#define DISABLE_HIGH_RESOLUTION_TIMER

using System;

namespace BinaryPhoenix.Fusion.Runtime
{

	/// <summary>
	///		The HighPreformanceTimer class is used to time operations
	///		that require exceptionally high accuracy.
	/// </summary>
	public sealed class HighPreformanceTimer
	{
		#region Members
		#region Variables

		private long _start, _frequency;
		private bool _supported = true;

		#endregion
		#region Properties

		/// <summary>
		///		Gets the duration in seconds between the last Start() / Stop() calls.
		/// </summary>
		public double DurationMillisecond
		{
			get
			{
                if (_supported == false)
                {
                    return (double)(System.Environment.TickCount - _start);
                }
                else
                {
                    long _stop;
                    NativeMethods.QueryPerformanceCounter(out _stop);
                    return ((double)(_stop - _start) / (double)_frequency) * 1000.0;
                }
			}
		}

		#endregion
		#endregion
		#region Methods

		public HighPreformanceTimer() 
        {
#if DISABLE_HIGH_RESOLUTION_TIMER 
            _supported = false;
            _start = System.Environment.TickCount;
            return;
#else
			try
			{
				if (NativeMethods.QueryPerformanceFrequency(out _frequency) == false) _supported = false;
				NativeMethods.QueryPerformanceCounter(out _start);
			}
			catch
			{
				_supported = false;
			}
#endif
		}

		public void Restart()
		{
            if (_supported == false)
            {
                _start = System.Environment.TickCount;
                return;
            }
			NativeMethods.QueryPerformanceCounter(out _start);
		}

		#endregion
	}

}
