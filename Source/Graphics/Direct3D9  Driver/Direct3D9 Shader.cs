/* 
 * File: DirectX9 Shader.cs
 *
 * This source file contains the declaration of the DirectX9Shader class, which is
 * used to store details on a single shader loaded by the DX9 driver.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Graphics.Direct3D9Driver
{

    /// <summary>
    ///     The DirectX9 Shader is used to store details on a single shader loaded
    ///     by the DirectX9 driver.
    /// </summary>
    public sealed class Direct3D9Shader : IShader
	{
		#region Members
		#region Variables

		private Effect _shader = null;
        private Direct3D9Driver _dx9Driver;

        private Hashtable _localVariables = new Hashtable();
        private Hashtable _valueHandles = new Hashtable();

		#endregion
		#region Properties

        /// <summary>
        ///     Gets or sets the driver dependent shader.
        /// </summary>
        public Effect Shader
        {
            get { return _shader; }
            set { _shader = value; }
        }

        /// <summary>
        ///     Gets or sets the local variables of this shader.
        /// </summary>
        object IShader.this[string key]
        {
            get { return _valueHandles[key]; }
            set 
            { 
                bool hadValue = _localVariables.ContainsKey(key);
                _localVariables[key] = value;
                if (hadValue == false)
                    SyncVariables();
            }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Creates a new image frame out of a PixelMap and associates it with a specific image.
		/// </summary>
		/// <param name="driver">Direct3D9Driver that this image frame should be associated with.</param>
		/// <param name="pixelMap">PixelMap to create frame from.</param>
		public Direct3D9Shader(Direct3D9Driver driver, string code)
		{
			_dx9Driver = driver;

            // Create the shader.
            try
            {
                _shader = Effect.FromString(driver.DX9Device, code, null, null, ShaderFlags.None, null);
                _shader.Technique = "Main";

                // Grab the variable handles.
                SyncVariables();
            }
            catch (Exception) 
            {
                Runtime.Debug.DebugLogger.WriteLog("An error occured while trying to load shader.", BinaryPhoenix.Fusion.Runtime.Debug.LogAlertLevel.Warning);
            }

            // Hook into the dx9 disposal event so we can clean up.
            _dx9Driver.DX9Device.Disposing += new EventHandler(DX9Device_Disposing);
		}

        /// <summary>
        ///     Gets the correct handles of each variable within the shader.
        /// </summary>
        public void SyncVariables()
        {
            foreach (string key in _dx9Driver.ShaderValues.Keys)
                _valueHandles[key] = _shader.GetParameter(null, key);

            foreach (string key in _localVariables.Keys)
                _valueHandles[key] = _shader.GetParameter(null, key);
        }

        /// <summary>
        ///     Begins applying the shader.
        /// </summary>
        /// <returns>Amount of passes required.</returns>
        int IShader.Begin()
        {
            if (_shader == null)
                return 1;

            // Syncs up the variable handles if neccessary.
            if (_dx9Driver.ShaderValuesChanged == true || _dx9Driver.ShaderValues.Count + _localVariables.Count != _valueHandles.Count)
                SyncVariables();

            // Apply global values.
            foreach (string key in _dx9Driver.ShaderValues.Keys)
            {
                if (_valueHandles[key] != null)
                {
                    if (_dx9Driver.ShaderValues[key] is float)
                        _shader.SetValue(_valueHandles[key] as EffectHandle, (float)_dx9Driver.ShaderValues[key]);
                    else if (_dx9Driver.ShaderValues[key] is float[])
                        _shader.SetValue(_valueHandles[key] as EffectHandle, _dx9Driver.ShaderValues[key] as float[]);
                    else if (_dx9Driver.ShaderValues[key] is int)
                        _shader.SetValue(_valueHandles[key] as EffectHandle, (int)_dx9Driver.ShaderValues[key]);
                    else if (_dx9Driver.ShaderValues[key] is int[])
                        _shader.SetValue(_valueHandles[key] as EffectHandle, _dx9Driver.ShaderValues[key] as int[]);
                }
            }

            // Apply local values.
            foreach (string key in _localVariables.Keys)
            {
                if (_valueHandles[key] != null)
                {
                    if (_localVariables[key] is float)
                        _shader.SetValue(_valueHandles[key] as EffectHandle, (float)_localVariables[key]);
                    else if (_localVariables[key] is float[])
                        _shader.SetValue(_valueHandles[key] as EffectHandle, _localVariables[key] as float[]);
                    else if (_localVariables[key] is int)
                        _shader.SetValue(_valueHandles[key] as EffectHandle, (int)_localVariables[key]);
                    else if (_localVariables[key] is int[])
                        _shader.SetValue(_valueHandles[key] as EffectHandle, _localVariables[key] as int[]);
                }
            }

            return _shader.Begin(FX.None);
        }

        /// <summary>
        ///     Finishs applying the shader.
        /// </summary>
        void IShader.Finish()
        {
            if (_shader == null)
                return;

            _shader.End();
        }

        /// <summary>
        ///     Begins a application pass.
        /// </summary>
        void IShader.BeginPass(int i)
        {
            if (_shader == null)
                return;

            _shader.BeginPass(i);
        }

        /// <summary>
        ///     Finishs an application pass.
        /// </summary>
        void IShader.FinishPass()
        {
            if (_shader == null)
                return;

            _shader.EndPass();
        }

		/// <summary>
		///		Disposes of this image frame, and frees all the resources it has 
		///		allocated.
		/// </summary>
		~Direct3D9Shader()
		{
			if (_shader != null)      
                _shader.Dispose();
		}

        /// <summary>
        ///     Invoked when the DX9 device that created this shader is destroyed.
        /// </summary>
        /// <param name="sender">Object that invoked this function.</param>
        /// <param name="e">Arguments explaining why this was invoked.</param>
        private void DX9Device_Disposing(object sender, EventArgs e)
        {
            if (_shader != null && _shader.Disposed == false)
                _shader.Dispose();
        }
		#endregion
    }

}