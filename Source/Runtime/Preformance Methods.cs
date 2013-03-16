/* 
 * File: Preformance.cs
 *
 * This source file contains the declaration of the PreformanceMethods class which
 * includes several basic preformance counter functions used by the rest of the engine.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Diagnostics;
using System.IO;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime
{

	/// <summary>
	///		Contains several basic preformance counter functions using by the rest of the engine.
	/// </summary>
	public static class PreformanceMethods
    {
        #region Members
        #region Variables

        private static PerformanceCounter _cpuCounter;
        private static PerformanceCounter _ramCounter; 

        #endregion
        #region Properties

        /// <summary>
        ///     Gets the current CPU usage.
        /// </summary>
        public static float CPUUsage
        {
            get 
            {
                if (_cpuCounter == null)
                {
                    _cpuCounter = new PerformanceCounter();
                    _cpuCounter.CategoryName = "Processor";
                    _cpuCounter.CounterName = "% Processor Time";
                    _cpuCounter.InstanceName = "_Total"; 
                }
                return _cpuCounter.NextValue();
            }
        }

        /// <summary>
        ///     Gets the current ram usage.
        /// </summary>
        public static int AvailableRAM
        {
            get 
            {
                if (_ramCounter == null)
                    _ramCounter = new PerformanceCounter("Memory", "Available MBytes"); 
                return (int)(_ramCounter.NextValue() * 1024) * 1024;           
            }
        }

        #endregion
        #endregion
    }

}