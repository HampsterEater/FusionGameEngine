/* 
 * File: Strings.cs
 *
 * This source file contains the declaration of the MathmaticsFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Runtime.Scripting.ScriptingFunctions
{

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class StringFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("Contains", "bool", "string,string")]
		public void Contains(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).Contains(thread.GetStringParameter(1)));
		}

		[NativeFunctionInfo("EndsWith", "bool", "string,string")]
		public void EndsWith(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).EndsWith(thread.GetStringParameter(1)));
		}

		[NativeFunctionInfo("StartsWith", "bool", "string,string")]
		public void StartsWith(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).StartsWith(thread.GetStringParameter(1)));
		}

        [NativeFunctionInfo("CharacterCount", "int", "string,string")]
        public void CharacterCount(ScriptThread thread)
        {
            string haystack = thread.GetStringParameter(0);
            string needle = thread.GetStringParameter(1);
            int count = 0;
            for (int i = 0; i < haystack.Length; i++)
                if (haystack[i] == needle[0]) count++;
            thread.SetReturnValue(count);
        }

		[NativeFunctionInfo("IndexOf", "int", "string,string")]
		public void IndexOfA(ScriptThread thread)
		{			
			thread.SetReturnValue(thread.GetStringParameter(0).IndexOf(thread.GetStringParameter(1)));
		}

		[NativeFunctionInfo("IndexOf", "int", "string,string,int")]
		public void IndexOfB(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).IndexOf(thread.GetStringParameter(1), thread.GetIntegerParameter(2)));
		}

		[NativeFunctionInfo("LastIndexOf", "int", "string,string")]
		public void LastIndexOfA(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).LastIndexOf(thread.GetStringParameter(1)));
		}

		[NativeFunctionInfo("LastIndexOf", "int", "string,string,int")]
		public void LastIndexOfB(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).LastIndexOf(thread.GetStringParameter(1), thread.GetIntegerParameter(2)));
		}

		[NativeFunctionInfo("Insert", "string", "string,string,int")]
		public void Insert(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).Insert(thread.GetIntegerParameter(2), thread.GetStringParameter(1)));
		}

		[NativeFunctionInfo("PadLeft", "string", "string,int")]
		public void PadLeftA(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).PadLeft(thread.GetIntegerParameter(1)));
		}

		[NativeFunctionInfo("PadLeft", "string", "string,int,string")]
		public void PadLeftB(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).PadLeft(thread.GetIntegerParameter(1), thread.GetStringParameter(2)[0]));
		}

		[NativeFunctionInfo("PadRight", "string", "string,int")]
		public void PadRightA(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).PadRight(thread.GetIntegerParameter(1)));
		}

		[NativeFunctionInfo("PadRight", "string", "string,int,string")]
		public void PadRightB(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).PadRight(thread.GetIntegerParameter(1), thread.GetStringParameter(2)[0]));
		}

		[NativeFunctionInfo("Replace", "string", "string,string,string")]
		public void Replace(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).Replace(thread.GetStringParameter(1), thread.GetStringParameter(2)));
		}

		[NativeFunctionInfo("Explode", "string[]", "string,string")]
		public void Explode(ScriptThread thread)
		{
			string explodee = thread.GetStringParameter(0);
			char seperator = thread.GetStringParameter(1)[0];
			string[] exploded = explodee.Split(new char[] { seperator });
			
			int arrayMemoryIndex = thread.AllocateArray(DataType.String, exploded.Length);
			for (int i = 0; i < exploded.Length; i++)
				thread.SetArrayElement(arrayMemoryIndex, i, exploded[i]);

			thread.SetReturnValueArray(arrayMemoryIndex);
		}

		[NativeFunctionInfo("Implode", "string", "string[]")]
		public void Implode(ScriptThread thread)
		{
			int arrayMemoryIndex = thread.GetArrayParameter(0);
			int arrayLength = thread.GetArrayLength(arrayMemoryIndex);
			string implodedString = "";

			for (int i = 0; i < arrayLength; i++)
				implodedString += thread.GetStringArrayElement(arrayMemoryIndex, i);

			thread.SetReturnValue(implodedString);
		}

		[NativeFunctionInfo("SubString", "string", "string,int")]
		public void SubstringA(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).Substring(thread.GetIntegerParameter(1)));
		}

		[NativeFunctionInfo("SubString", "string", "string,int,int")]
		public void SubstringB(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).Substring(thread.GetIntegerParameter(1), thread.GetIntegerParameter(2)));
		}

		[NativeFunctionInfo("ToLowerCase", "string", "string")]
		public void ToLowerCase(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).ToLower());
		}

		[NativeFunctionInfo("ToUpperCase", "string", "string")]
		public void ToUpperCase(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).ToUpper());
		}

		// TODO: Possible overloads?
		[NativeFunctionInfo("Trim", "string", "string")]
		public void Trim(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).Trim());
		}

		[NativeFunctionInfo("TrimEnd", "string", "string")]
		public void TrimEnd(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).TrimEnd());
		}

		[NativeFunctionInfo("TrimStart", "string", "string")]
		public void TrimStart(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).TrimStart());
		}

		[NativeFunctionInfo("Length", "int", "string")]
		public void Length(ScriptThread thread)
		{
			thread.SetReturnValue(thread.GetStringParameter(0).Length);
		}

		[NativeFunctionInfo("Chr", "string", "int")]
		public void Chr(ScriptThread thread)
		{	
			thread.SetReturnValue(Convert.ToString((char)thread.GetIntegerParameter(0)));
		}

		[NativeFunctionInfo("Asc", "int", "string")]
		public void Asc(ScriptThread thread)
		{
			thread.SetReturnValue((int)thread.GetStringParameter(0)[0]);
		}

        [NativeFunctionInfo("HashCode", "int", "string")]
        public void HashCode(ScriptThread thread)
        {
            thread.SetReturnValue(thread.GetStringParameter(0).GetHashCode());
        }

		[NativeFunctionInfo("WordWrap", "string", "string,int")]
		public void WordWrap(ScriptThread thread)
		{
			thread.SetReturnValue(StringMethods.WordWrap(thread.GetStringParameter(0), thread.GetIntegerParameter(1)));
		}
	}

}