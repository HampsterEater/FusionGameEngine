/* 
 * File: Color methods.cs
 *
 * This source file contains the declaration of the ColorMethods class which
 * includes several basic color functions used by the rest of the engine.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime
{

	/// <summary>
	///     The ColorsFormats enumerations are used to describe different formats
	///		that can be converted between with the split / combine functions in the ColorMethods class.
	/// </summary>
	public enum ColorFormats : int
	{
		A8R8G8B8,
		R8G8B8,
		B8G8R8A8,
		B8G8R8
	}

	public static class ColorMethods
	{
		#region Methods


		/// <summary>
		///     Converts a red, green, blue and alpha value into an ARGB int value. 
		/// </summary>
		/// <param name="format">Format to convert color values into.</param>
		/// <param name="red">Red color value.</param>
		/// <param name="green">Green color value.</param>
		/// <param name="blue">Blue color value.</param>
		/// <param name="alpha">Alpha color value.</param>
		public static int CombineColor(ColorFormats format, int red, int green, int blue, int alpha)
		{
			switch (format)
			{
				case ColorFormats.A8R8G8B8: return (alpha << 24) | (red << 16) | (green << 8) | blue;
				case ColorFormats.R8G8B8: return (red << 16) | (green << 8) | blue;
				case ColorFormats.B8G8R8A8: return (blue << 24) | (green << 16) | (red << 8) | alpha;
				case ColorFormats.B8G8R8: return (blue << 16) | (green << 8) | red;
			}
			return 0;
		}

		/// <summary>
		///     Converts a color value into another format 
		/// </summary>
		/// <param name="fromformat">Format of color value to convert.</param>
		/// <param name="fromformat">Format to convert color value into.</param>
		/// <param name="color">Color value to convert.</param>
		public static int CombineColor(ColorFormats fromFormat, ColorFormats toFormat, int color)
		{
			int a, r, g, b;
			SplitColor(fromFormat, color, out r, out g, out b, out a);
			return CombineColor(toFormat, r, g, b, a);
		}

		/// <summary>
		///     Converts an ARGB int color value into its seperate red, green and blue values.
		/// </summary>
		/// <param name="color">Color value to convert from.</param>
		/// <param name="format">Format of color.</param>
		/// <param name="red">Red color value.</param>
		/// <param name="green">Green color value.</param>
		/// <param name="blue">Blue color value.</param>
		/// <param name="alpha">Alpha color value.</param>
		public static void SplitColor(ColorFormats format, int color, out int red, out int green, out int blue, out int alpha)
		{
			switch (format)
			{
				case ColorFormats.A8R8G8B8:
					alpha = (color >> 24) & 0xFF;
					red = (color >> 16) & 0xFF;
					green = (color >> 8) & 0xFF;
					blue = color & 0xFF;
					break;
				case ColorFormats.R8G8B8:
					red = (color >> 16 & 0xFF);
					green = (color >> 8) & 0xFF;
					blue = color & 0xFF;
					alpha = 255;
					break;
				case ColorFormats.B8G8R8A8:
					blue = (color >> 24) & 0xFF;
					green = (color >> 16) & 0xFF;
					red = (color >> 8) & 0xFF;
					alpha = color & 0xFF;
					break;
				case ColorFormats.B8G8R8:
					blue = (color >> 16) & 0xFF;
					green = (color >> 8) & 0xFF;
					red = color & 0xFF;
					alpha = 255;
					break;
				default:
					red = 255;
					green = 255;
					blue = 255;
					alpha = 255;
					break;
			}
		}

		#endregion
	}

}