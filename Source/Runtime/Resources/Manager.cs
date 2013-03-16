/* 
 * File: Manager.cs
 *
 * This source file contains the declaration of the resource manager class
 * which is responsible for retrieving requested resources from pak files, HD files
 * optical files, and anywhere else they are requested from.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Xml;
using System.Collections;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Runtime.Resources
{

	/// <summary>
	///		This class is responsible for retrieving requested resources from pak files, HD files
	///		optical files, and anywhere else they are requested from.
	/// </summary>
	public static class ResourceManager
	{
		#region Members
		#region Variables

		private static bool _usePakFiles = false;
		private static ArrayList _registedPakFiles = new ArrayList();

        private static bool _resourceCacheEnabled = true;
		private static Hashtable _resourceCache = new Hashtable();
        private static Hashtable _resourceCacheAges = new Hashtable();

		#endregion
		#region Properties

        /// <summary>
        ///     Gets or sets if the resource cache should be used or not.
        /// </summary>
        public static bool ResourceCacheEnabled
        {
            get { return _resourceCacheEnabled; }
            set { _resourceCacheEnabled = value; }
        }

		/// <summary>
		///		Gets or sets if this resource manager should try to extract resources from
		///		registered pak files rather than HD files.
		/// </summary>
		public static bool UsePakFiles
		{
			get { return _usePakFiles;  }
			set { _usePakFiles = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Registers a pak file to this resource manager so resource can 
		///		be extracted from it. 
		/// </summary>
		/// <param name="file">Pak file to register.</param>
		public static void RegisterPakFile(PakFile file)
		{
			_registedPakFiles.Add(file);
		}

		/// <summary>
		///		Unregisters a pak file from this resource manager.
		/// </summary>
		/// <param name="file">Pak file to unregister.</param>
		public static void UnregisterPakFile(PakFile file)
		{
			_registedPakFiles.Remove(file);
		}

		/// <summary>
		///		Checks if a resource exists.
		/// </summary>
		/// <param name="url">Url of resource to check.</param>
        /// <param name="onlyCheckInPakFiles">Only checks in pak files if set.</param>
		/// <returns>True if resource exists, else false.</returns>
		public static bool ResourceExists(object url, bool onlyCheckInPakFiles)
		{
            if (url.ToString().ToLower().StartsWith("pak@"))
                url = url.ToString().Substring(4); // Legacy support.

			if (_usePakFiles == true)
			{
				// If there is a local unpacked copy present load that instead as it 
				// is probably a mod of some sort.
                if (onlyCheckInPakFiles == false && File.Exists(url.ToString().ToLower()) == true)
					return true;

				// Look for the resource in all the pak files.
				foreach (PakFile file in _registedPakFiles)
					foreach (PakResource resource in file.Resources)
						if (resource.URL.ToString().ToLower() == url.ToString().ToLower()) return true;

				return false;
			}
			else
				return  onlyCheckInPakFiles ? false : File.Exists(url.ToString());
		}
        public static bool ResourceExists(object url)
        {
            return ResourceExists(url, false);
        }

		/// <summary>
		///		Attempts to retrieve a stream to a specified resource based on a Url.
		/// </summary>
		/// <param name="url">Url of resource to retrieve stream for.</param>
		/// <returns>Stream to resource, or null if resource could not be retrieved.</returns>
		public static Stream RequestResourceStream(object url)
		{
            if (url.ToString().ToLower().StartsWith("pak@"))
                url = url.ToString().Substring(4); // Legacy support.

			if (_usePakFiles == true)
			{
				DebugLogger.WriteLog("Resource stream requested for \""+url.ToString()+"\".");

				// If there is a local unpacked copy present load that instead as it 
				// is probably a mod of some sort.
				if (File.Exists(url.ToString().ToLower()) == true)
				{
					DebugLogger.WriteLog("Resource \""+url.ToString()+"\" found unpacked on local hard drive.");
					return StreamFactory.RequestStream(url, StreamMode.Open);
				}

				// Look for the resource in all the pak files.
				foreach (PakFile file in _registedPakFiles)
				{
					Stream stream = file.RequestResourceStream(url);
					if (stream != null)
					{
						DebugLogger.WriteLog("Resource \"" + url.ToString() + "\" found in pak file.");
						return stream;
					}
				}

				// Oh dear, we can't find this resource. Alert the user.
				DebugLogger.WriteLog("Unable to find resource \"" + url.ToString() + "\".", LogAlertLevel.Warning);
			}
			else
				return StreamFactory.RequestStream(url, StreamMode.Open);
			return null;
		}

		/// <summary>
		///		Adds a given resource into the resource cache and associates it with the given name.
		/// </summary>
		/// <param name="name">Name to associate resource width.</param>
		/// <param name="resource">Resource to add to cache</param>
		public static void CacheResource(string name, object resource)
		{
            if (_resourceCacheEnabled == false) return;

			// See if its already in the resource cache, if it is remove it so it can
			// be moved to the front.
			if (_resourceCache.ContainsKey(name.ToLower()) == true)
				_resourceCache.Remove(name.ToLower());

			// Add the given resource to the resource cache.
			_resourceCache.Add(name.ToLower(), resource);
		}

		/// <summary>
		///		Retrieves a previous added resource from the resource cache.
		/// </summary>
		/// <param name="name">Name of resource to retrieve.</param>
		/// <returns>Resource if it is found, null otherwise.</returns>
		public static object RetrieveResource(string name)
		{
            if (_resourceCacheEnabled == false) return null;

            if (_resourceCache.ContainsKey(name.ToLower()))
            {
                _resourceCacheAges[name.ToLower()] = 0;
                return _resourceCache[name.ToLower()];
            }
            else
                return null;
		}

		/// <summary>
		///		Retturns true if the given resource exists in the resource cache.
		/// </summary>
		/// <param name="name">Name of resource to check.</param>
		/// <returns>True if the resource is in the resource cache.</returns>
		public static bool ResourceIsCached(string name)
		{
            if (_resourceCacheEnabled == false) return false;
			return _resourceCache.ContainsKey(name.ToLower());
		}


		/// <summary>
		///		Removes a given resource from the resource cache.
		/// </summary>
		/// <param name="name">Name of resource to remove.</param>
		public static void UncacheResource(string name)
		{
            if (_resourceCacheEnabled == false) return;
			_resourceCache.Remove(name.ToLower());
		}


        /// <summary>
        ///     Removes any resources that have not been used for n calls to this function.
        /// </summary>
        /// <param name="n">Age limit used to determine what is garbage.</param>
        public static void CollectGarbage(int n)
        {
            if (_resourceCacheEnabled == false) return;           

            // Remove any resources older than n.
            ArrayList garbage = new ArrayList();
            foreach (string key in _resourceCache.Keys)
            {
                if (_resourceCacheAges.Contains(key) && (int)_resourceCacheAges[key] > n)
                    garbage.Add(key);
            }
            foreach (string key in garbage)
            {
                _resourceCache.Remove(key);
                _resourceCacheAges.Remove(key);
            }

            // Notify of any garbage collected.
            if (garbage.Count > 0)
                DebugLogger.WriteLog("Collect "+garbage.Count+" pieces of garbage from the resource cache.");

            // Increase the age or resources.
            foreach (string key in _resourceCache.Keys)
                if (_resourceCacheAges.Contains(key))
                    _resourceCacheAges[key] = ((int)_resourceCacheAges[key] + 1);
                else    
                    _resourceCacheAges[key] = 1;
        }


		/// <summary>
		///		Emptys the resource cache.
		/// </summary>
		public static void EmptyCache()
		{
            if (_resourceCacheEnabled == false) return;
			_resourceCache.Clear();
		}

		#endregion
	}

}