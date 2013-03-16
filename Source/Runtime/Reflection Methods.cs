/* 
 * File: Reflection Methods.cs
 *
 * This source file contains the declaration of the ReflectionMethods class which contains
 * several commonly used function to do with reflection.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Runtime
{

	public static class ReflectionMethods
	{
		#region Methods

		/// <summary>
		///		Creates an object given the objects name. This method is exceptionally slow (1/2 sec or so)
		///		and is not advisable for use in speed critical code.
		/// </summary>
		/// <returns>An instance of the object requested.</returns>
		public static object CreateObject(string objectName)
		{
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type objectType = null;

                    try
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            //if (type.FullName.ToLower().StartsWith("binaryphoenix.") == false) break;
                            if (type.FullName.ToLower() == objectName.ToLower()) objectType = type;
                        }
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        Debug.DebugLogger.WriteLog("Exception caught while attempting to gather reflection types from assembly '" + assembly.FullName + "'");
                        continue;
                    }

                if (objectType != null)
                    return Activator.CreateInstance(objectType);
            }

			return null;
		}

		/// <summary>
		///		Retrieves a stream to a resource embedded in the editor assembly.
		/// </summary>
		/// <param name="name">Name of resource to retrieve</param>
		/// <returns>A stream to the embedded resource or null if it dosen't exist.</returns>
		public static Stream GetEmbeddedResourceStream(string name)
		{
			DebugLogger.WriteLog("Looking for embedded resource with name '" + name + "'.");

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				string[] resources = assembly.GetManifestResourceNames();
				foreach (string resourceName in resources)
				{
					if (resourceName.ToLower().EndsWith(name.ToLower()) == true)
					{
						return assembly.GetManifestResourceStream(resourceName);
					}
				}
			}

			DebugLogger.WriteLog("Unable to find embedded resource.");
			return null;
		}

		#endregion
	}

}