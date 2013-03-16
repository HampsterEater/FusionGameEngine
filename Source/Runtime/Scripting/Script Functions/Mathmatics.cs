/* 
 * File: Mathmatics.cs
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
	public class MathmaticsFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("Random", "float", "float,float")]
		public void RandomA(ScriptThread thread)
		{
            thread.SetReturnValue(MathMethods.Random(thread.GetFloatParameter(0), thread.GetFloatParameter(1)));
		}

		[NativeFunctionInfo("Random", "int", "int,int")]
		public void RandomB(ScriptThread thread)
		{
			thread.SetReturnValue(MathMethods.Random(thread.GetIntegerParameter(0), thread.GetIntegerParameter(1)));
		}

		[NativeFunctionInfo("SeedRandom", "void", "int")]
		public void SeedRandom(ScriptThread thread)
		{
			MathMethods.SeedRandom(thread.GetIntegerParameter(0));
		}

		[NativeFunctionInfo("Abs","double","double")]
		public void Abs(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Abs(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Acos","double","double")]
		public void Acos(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Acos(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Asin", "double", "double")]
		public void Asin(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Asin(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Atan", "double", "double")]
		public void Atan(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Atan(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Atan2", "double", "double,double")]
		public void Atan2(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Atan2(thread.GetDoubleParameter(0), thread.GetDoubleParameter(1)));
		}

		[NativeFunctionInfo("Ceiling", "double", "double")]
		public void Ceiling(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Ceiling(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Cos", "double", "double")]
		public void Cos(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Cos(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Cosh", "double", "double")]
		public void Cosh(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Cosh(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Exp", "double", "double")]
		public void Exp(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Exp(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Floor", "double", "double")]
		public void Floor(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Floor(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Log", "double", "double")]
		public void LogA(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Log(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Log", "double", "double,double")]
		public void LogB(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Log(thread.GetDoubleParameter(0), thread.GetDoubleParameter(1)));
		}

		[NativeFunctionInfo("Log10", "double", "double")]
		public void Log10(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Log10(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Max", "double", "double,double")]
		public void Max(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Max(thread.GetDoubleParameter(0), thread.GetDoubleParameter(1)));
		}

		[NativeFunctionInfo("Min", "double", "double,double")]
		public void Min(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Min(thread.GetDoubleParameter(0), thread.GetDoubleParameter(1)));
		}

		[NativeFunctionInfo("Pow", "double", "double,double")]
		public void Pow(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Pow(thread.GetDoubleParameter(0), thread.GetDoubleParameter(1)));
		}

		// TODO: More overloads to add.
		[NativeFunctionInfo("Round", "double", "double")]
		public void Round(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Round(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Sign", "int", "double")]
		public void Sign(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Sign(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Sin", "double", "double")]
		public void Sin(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Sin(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Sinh", "double", "double")]
		public void Sinh(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Sinh(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Sqrt", "double", "double")]
		public void Sqrt(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Sqrt(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Tan", "double", "double")]
		public void Tan(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Tan(thread.GetDoubleParameter(0)));
		}

		[NativeFunctionInfo("Tanh", "double", "double")]
		public void Tanh(ScriptThread thread)
		{
			thread.SetReturnValue(Math.Tanh(thread.GetDoubleParameter(0)));
		}

        [NativeFunctionInfo("DistanceToPoint", "double", "double,double,double,double")]
        public void DistanceToPoint(ScriptThread thread)
        {
            double vectorX = thread.GetDoubleParameter(0) - thread.GetDoubleParameter(2);
            double vectorY = thread.GetDoubleParameter(1) - thread.GetDoubleParameter(3);
            double distance = Math.Sqrt(vectorX * vectorX + vectorY * vectorY);
            thread.SetReturnValue(distance);
        }
	}

}
