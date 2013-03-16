/* 
 * File: Image.cs
 *
 * This source file contains the declaration of the Image class which encapsulates
 * all the details needed to render image data to the screen. It also contains the 
 * ImageFrame class which is used to isolate the graphics API rendering from the game.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Collections;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.StreamFactorys;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Graphics
{
    #region Shader Interfaces

    /// <summary>
    ///     Used as an abstract layer between API's.
    /// </summary>
    public interface IShader
    {
        int Begin();
        void Finish();
        void BeginPass(int pass);
        void FinishPass();
        object this[string key] { get; set; }
    }

    #endregion
    #region Shader Classes

    /// <summary>
    ///     Used to store details on a single renderable shader.
    /// </summary>
    public class Shader
	{
		#region Members
		#region Variables

        private IGraphicsDriver _driver = null;

        private string _url = "";
        private Hashtable _shaderCode = new Hashtable();
		private IShader _shader = null;

		#endregion
		#region Properties

        /// <summary>
        ///     Gets or sets the URL this shader was loaded from.
        /// </summary>
        public string URL
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        ///     Gets or sets the local variables of this shader.
        /// </summary>
        public object this[string key]
        {
            get { return _shader[key]; }
            set { _shader[key] = value; }
        }

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Begins applying the shader.
        /// </summary>
        /// <returns>Amount of passes required.</returns>
        public int Begin()
        {
            if (_driver != GraphicsManager.Driver)
                CreateShader();
            if (_shader == null)
                return 1;

            return _shader.Begin();
        }

        /// <summary>
        ///     Begins a application pass.
        /// </summary>
        public void BeginPass(int index)
        {
            if (_driver != GraphicsManager.Driver)
                CreateShader();
            if (_shader == null)
                return;

            _shader.BeginPass(index);
        }

        /// <summary>
        ///     Finishs applying the shader.
        /// </summary>
        public void Finish()
        {
            if (_driver != GraphicsManager.Driver)
                CreateShader();
            if (_shader == null)
                return;

            _shader.Finish();
        }

        /// <summary>
        ///     Finishs an application pass.
        /// </summary>
        public void FinishPass()
        {
            if (_driver != GraphicsManager.Driver)
                CreateShader();
            if (_shader == null)
                return;

            _shader.FinishPass();
        }

        /// <summary>
        ///     Recreates the driver dependent shader.
        /// </summary>
        private void CreateShader()
        {
            if (_shader != null)
            {
                _shader = null;
                //GC.Collect();
            }

            if (!_shaderCode.ContainsKey(GraphicsManager.Driver.ToString()))
                return;

            _shader = GraphicsManager.Driver.CreateShader(_shaderCode[GraphicsManager.Driver.ToString()] as string);
            _driver = GraphicsManager.Driver;
        }

        /// <summary>
        ///     Loads this shaders code from the given url.
        /// </summary>
        /// <param name="url">Url of shader to load.</param>
        public void Load(object url)
        {
            if (url is string) _url = (string)url;

            Stream stream = StreamFactory.RequestStream(url, StreamMode.Open);
            if (stream == null) return;
            XmlDocument document = new XmlDocument();
            document.Load(stream);
            stream.Close();

            // Check there is actually a shader declaration in this file.
            if (document["shader"] == null) throw new Exception("Shader declaration file missing shader element.");

            foreach (XmlNode driverNode in document["shader"].ChildNodes)
            {
                if (driverNode.Name.ToLower() != "driver") continue;

                // Find the settings of this glyph.
                string name = "";
                foreach (XmlNode attribute in driverNode.Attributes)
                {
                    if (attribute.Name.ToLower() == "name")
                        name = attribute.Value;
                }

                // Create the glyph and add it to the list.
                _shaderCode.Add(name, driverNode.InnerText);
            }
            
            // Create the actual shader.
            CreateShader();
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="url">Url of shader to load.</param>
        public Shader(object url)
		{
            Load(url);
		}

		#endregion
	}
	
	#endregion
}
