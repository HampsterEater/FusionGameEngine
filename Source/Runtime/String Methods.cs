/* 
 * File: String.cs
 *
 * This source file contains the declaration of the StringMethods class which
 * includes several generic functions used to manipulate strings.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Runtime
{

	public static class StringMethods
	{
		#region Methods

		/// <summary>
		///		Word-wraps a string to the maximum line length specified.
		/// </summary>
		/// <param name="textToWrap">String to preform word wraping on.</param>
		/// <param name="lengthOfLine">Maximum line length.</param>
		/// <returns>Word wrapped version of textToWrap.</returns>
		public static string WordWrap(string textToWrap, int lengthOfLine)
		{
			string wrappedText = "";

			while (true)
			{
				int index = lengthOfLine;

				if (textToWrap[index] == ' ')
				{
					wrappedText += textToWrap.Substring(0, index) + "\n";
					textToWrap = textToWrap.Substring(index);
				}
				else
				{
					while (textToWrap[index] != ' ')
					{
						index--;
						if (index == 0)
						{
							index = lengthOfLine;
							break;
						}
					}
					wrappedText += textToWrap.Substring(0, index) + "\n";
					textToWrap = textToWrap.Substring(index);
				}
			}
		}

		/// <summary>
		///		Returns true if the given string is a valid identifier.
		/// </summary>
		/// <param name="input">String to check valididity of.</param>
		/// <returns>True if the given string is a valid identifier.</returns>
		public static bool IsStringIdentifier(string input)
		{
			for (int i = 0; i < input.Length; i++)
				if (!((input[i] >= 'a' && input[i] <= 'z') || (input[i] >= 'A' && input[i] <= 'Z') || (input[i] >= '0' && input[i] <= '9') || input[i] == '_'))
					return false;
			return true;
		}

		/// <summary>
		///		Returns true if the given string is numeric.
		/// </summary>
		/// <param name="input">String to check valididity of.</param>
		/// <returns>True if the given string is numeric.</returns>
		public static bool IsStringNumeric(string input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (!((input[i] >= '0' && input[i] <= '9') || 
					  (input[i] >= 'A' && input[i] <= 'F') || 
					  (input[i] >= 'a' && input[i] <= 'f') ||
					   input[i] == '-' || input[i] == '.' || input[i] == 'x' || input[i] == 'f' || input[i] == 'd' || input[i] == 'l' || input[i] == 's' || input[i] == 'b'))
					return false;
			}
			return true;
		}

        /// <summary>
        ///     Gets the count of a specific character in a given string.
        /// </summary>
        /// <param name="haystack">String to search.</param>
        /// <param name="needle">Character to search for.</param>
        /// <returns>Count of the specific character.</returns>
        public static int CharacterCount(string haystack, char needle)
        {
            int count = 0;
            for (int i = 0; i < haystack.Length; i++)
                if (haystack[i] == needle)
                    count++;
            return count;
        }


        /// <summary>
        ///		Converts a time in seconds into a mnemonic form and changes the postfix to
        ///		minutes if neccessary.
        /// </summary>
        /// <param name="time">Time in seconds to convert.</param>
        /// <returns>String containing the mneomnic of the given time.</returns>
        public static string TimeFromSeconds(int time)
        {
            if (time > (60 * 60))
                return ((time / 60) / 60) + " hours";
            else if (time > 60)
                return (time / 60) + " minutes";
            else
                return time + " seconds";
        }

        /// <summary>
        ///		Converts a size in bytes into a mnemonic form and changes the postfix to
        ///		megabytes / kilobytes if neccessary.
        /// </summary>
        /// <param name="size">Size in bytes to convert.</param>
        /// <returns>String containing the mneomnic of the given size.</returns>
        public static string SizeFromBytes(int size)
        {
            float value = size;
            string postFix = "bytes";
            if ((size / 1024.0f) / 1024.0f > 1)
            {
                value = (size / 1024.0f) / 1024.0f;
                postFix = "mb";
            }
            else if ((size / 1024.0f) > 1)
            {
                value = size / 1024.0f;
                postFix = "kb";
            }

            int radixIndex = value.ToString().IndexOf('.');
            if (radixIndex != -1 && value.ToString().Length >= radixIndex + 3)
                return value.ToString().Substring(0, radixIndex + 3) + postFix;
            else
                return value.ToString() + postFix;
        }

		#endregion
	}

}