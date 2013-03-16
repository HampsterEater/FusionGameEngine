/*
 * File: Plugin.cs
 *
 * Contains several classes used to bind plugins and update them.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Engine
{

    /// <summary>
    ///     This class is used to bind and update plugins.
    /// </summary>
    public class Plugin
    {
        #region Members
        #region Variables

        private static ArrayList _boundPluginPool = new ArrayList();

        private string _file;
        private Assembly _assembly;
        private object _instance;
        private Hashtable _methodTable = new Hashtable();

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the array containing the whole plugin pool.
        /// </summary>
        public ArrayList BoundPluginPool
        {
            get { return _boundPluginPool; }
            set { _boundPluginPool = value; }
        }

        /// <summary>
        ///     Gets or sets the library file this plugin refers to.
        /// </summary>
        public string File
        {
            get { return _file; }
            set 
            {
                if (_assembly == null)
                    _file = value;
                else
                {
                    UnBind();
                    _file = value;
                    Bind();
                }
            }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Bind()
        {
			// Notify the user that we are loading this plugin.
			DebugLogger.WriteLog("Binding plugin '" + _file + "'.");

            // Load a new instance of the assembly.
#if !DEBUG
            try
            {
#endif
                _assembly = Assembly.LoadFile(Path.GetFullPath(_file));
#if !DEBUG
            }
            catch (Exception) { }
#endif
                
            // Throw up an error if the plugin failed to bind.
            if (_assembly == null)
            {
                DebugLogger.WriteLog("Failed to bind plugin, unable to create assembly for file '" + _file + "'.", LogAlertLevel.Error);
                return false;
            }

            // Create a new instance of the plugins main class and call its Bind method.
            string className = "BinaryPhoenix.Fusion." + Path.GetFileNameWithoutExtension(_file).Replace(" ", "") + "." + Path.GetFileNameWithoutExtension(_file).Replace(" ", "");
 #if !DEBUG
            try
            {
#endif
                _instance = _assembly.CreateInstance(className);
#if !DEBUG
            }
            catch (Exception) { }
#endif

            // Throw up an error if the plugin failed to bind.
            if (_instance == null)
            {
                DebugLogger.WriteLog("Failed to bind plugin, unable to create plugin class '" + className + "' instance.", LogAlertLevel.Error);
                return false;
            }

            // Call the bind method.
            if (CallMethod("Bind") == false)
            {
                DebugLogger.WriteLog("Failed to bind plugin, unable to call Bind method.", LogAlertLevel.Error);
                return false;
            }

            // Add it to the bound plugin pool.
            _boundPluginPool.Add(this);

            return true;
        }

        /// <summary>
        ///     Calls the given method within the plugin.
        /// </summary>
        /// <param name="method">Name of method to call.</param>
        /// <returns>True if successfull.</returns>
        public bool CallMethod(string method)
        {
            if (_assembly == null || _instance == null) return false;

            // Do we have it stored in the method table?
            if (_methodTable.Contains(method))
            {
                // Call it!
                ((MethodInfo)_methodTable[method]).Invoke(_instance, null);
            }
            else
            {
                // Find it!
                MethodInfo info = _instance.GetType().GetMethod(method);
                if (info == null) return false;

                // Call it!
                info.Invoke(_instance, null);

                // Store it for later!
                _methodTable.Add(method, info);
            }

            return true;
        }

        /// <summary>
        ///     Unbinds this plugin.
        /// </summary>
        public bool UnBind()
        {
            if (_assembly == null || _instance == null) return false;

            // Call the unbind method.
            if (CallMethod("UnBind") == false)
            {
                DebugLogger.WriteLog("Failed to unbind plugin, unable to call UnBind method.", LogAlertLevel.Error);
                return false;
            }

            // Kill it off!
            _assembly = null;
            _instance = null;
            _methodTable.Clear();
            _boundPluginPool.Remove(this);

            return true;
        }

        /// <summary>
        ///     Creates a new instance of this class.
        /// </summary>
        /// <param name="file">File this plugin refers to.</param>
        public Plugin(string file)
        {
            _file = file;
        }

        #endregion
        #region Functions

        /// <summary>
        ///     Calls the given method on all plugins in the plugin pool.
        /// </summary>
        /// <param name="name">Name of method to call.</param>
        public static void CallPluginMethod(string name)
        {
            foreach (Plugin plugin in _boundPluginPool)
                plugin.CallMethod(name);
        }

        /// <summary>
        ///     This method will unbind all currently bound plugins.
        /// </summary>
        public static void UnBindPlugins()
        {
            for (int i = 0; i < _boundPluginPool.Count; i++)
                ((Plugin)_boundPluginPool[i]).UnBind();
        }

        #endregion
    }
}
