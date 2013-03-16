/* 
 * File: Math.cs
 *
 * This source file contains the declaration of the MathMethods class which
 * includes several basic mathmatical functions used by the rest of the engine.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime
{

	public static class MathMethods
	{
		#region Members
		#region Variables

		private static Random _randomCounter = new Random(Environment.TickCount);
        //private static HighPreformanceTimer _reseedTimer = new HighPreformanceTimer();

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Converts an angle in degrees to an angle in radians.
		/// </summary>
		/// <param name="degrees">Angle to convert</param>
		/// <returns></returns>
		public static double DegreesToRadians(double degrees)
		{
			return degrees * (Math.PI / 180.0f);
		}

		/// <summary>
		///		Converts an angle in radians to an angle in degrees.
		/// </summary>
		/// <param name="radians">Angle to convert</param>
		/// <returns></returns>
		public static double RadiansToDegree(double radians)
		{
			return radians * (180.0f / Math.PI);
		}

        /// <summary>
        ///     Simple helper function, takes a integer and finds the closest power-of-2 integer
        ///     from it.
        /// </summary>
        /// <param name="n">Value to find closest power-of-2 from.</param>
        /// <returns>Closest power-of-2.</returns>
        public static int Pow(int n, int p)
        {
            int i = 1;
            while (i < n)
                i *= p;
            return i;
        }

		/// <summary>
		///		Generates a random value between start and end.
		/// </summary>
		/// <param name="start">Lowest value of random number.</param>
		/// <param name="end">Highest value of random number.</param>
		/// <returns>A random number between start and end</returns>
		public static float Random(float start, float end)
		{
            if (start > end)
            {
                float temp = start;
                start = end;
                end = temp;
            }
            //if (_randomCounter == null || _reseedTimer.DurationMillisecond > 200)
            //{
            //    _reseedTimer.Restart();
            //    _randomCounter = new Random(Environment.TickCount);
            //}
			return (((float)_randomCounter.NextDouble()) * (end - start)) + start;
		}
		public static int Random(int start, int end)
		{
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }
            //if (_randomCounter == null || _reseedTimer.DurationMillisecond > 200)
            //{
            //    _reseedTimer.Restart();
            //    _randomCounter = new Random(Environment.TickCount);
            //}
            return _randomCounter.Next(start, end);
		}
        public static float Random()
        {
            //if (_randomCounter == null || _reseedTimer.DurationMillisecond > 200)
            //{
            //    _reseedTimer.Restart();
            //    _randomCounter = new Random(Environment.TickCount);
            //}
            return (float)_randomCounter.NextDouble();
        }

		/// <summary>
		///		Seeds the random number generator with a given value.
		/// </summary>
		/// <param name="seed">Number to seed random number generator with</param>
		public static void SeedRandom(int seed)
		{
			_randomCounter = new Random(seed);
		}

        /// <summary>
        ///     Restricts a integer value to minimum and maximum bounds.
        /// </summary>
        /// <param name="value">Value to restrict.</param>
        /// <param name="min">Minimum boundry.</param>
        /// <param name="max">Maximum boundry.</param>
        /// <returns>Restricted version of value.</returns>
        public static int Restrict(int value, int min, int max)
        {
            return (int)Math.Max(Math.Min(value, max), min);
        }

        /// <summary>
        ///     Checks if two given rectangles overlap.
        /// </summary>
        /// <param name="x0">X coordinate of first rectangle.</param>
        /// <param name="y0">Y coordinate of first rectangle.</param>
        /// <param name="w0">Width of first rectangle.</param>
        /// <param name="h0">Height of first rectangle.</param>
        /// <param name="x2">X coordinate of second rectangle.</param>
        /// <param name="y2">Y coordinate of second rectangle.</param>
        /// <param name="w2">Width of secnd rectangle.</param>
        /// <param name="h2">Height of second rectangle.</param>
        /// <returns>True if both rectangles overlap.</returns>
        public static bool RectanglesOverlap(int x0, int y0, int w0, int h0, int x2, int y2, int w2, int h2)
        {
            if (x0 > (x2 + w2) || (x0 + w0) < x2) return false;
            if (y0 > (y2 + h2) || (y0 + h0) < y2) return false;
	        return true;
        }

		#endregion
	}

}