/* 
 * File: Statistics.cs
 *
 * This source file contains the declaration of the Statics class which
 * includes several basic functions for tracking statistics.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime
{

    /// <summary>
    ///     This class contains several methods from tracking statistics.
    /// </summary>
	public static class Statistics
    {
        #region Members
        #region Variables

        private static Hashtable _statisticTable = new Hashtable();

        #endregion
        #region Properties

        /// <summary>
        ///     Gets the statistics table.
        /// </summary>
        public static Hashtable StatisticsTable
        {
            get { return _statisticTable; }
        }

        #endregion
        #endregion
        #region Methods

        public static int ReadInt(string name)
        {
            if (_statisticTable.Contains(name.ToLower()) == false) return 0;
            return int.Parse((string)_statisticTable[name.ToLower()]);
        }

        public static void StoreInt(string name, int value)
        {
            _statisticTable[name.ToLower()] = value.ToString();
        }

        public static float ReadFloat(string name)
        {
            if (_statisticTable.Contains(name.ToLower()) == false) return 0.0f;
            return float.Parse((string)_statisticTable[name.ToLower()]);
        }

        public static void StoreFloat(string name, float value)
        {
            _statisticTable[name.ToLower()] = value.ToString();
        }

        public static string ReadString(string name)
        {
            if (_statisticTable.Contains(name.ToLower()) == false) return "";
            return (string)_statisticTable[name.ToLower()];
        }

        public static void StoreString(string name, string value)
        {
            _statisticTable[name.ToLower()] = value;
        }

        #endregion
    }

}