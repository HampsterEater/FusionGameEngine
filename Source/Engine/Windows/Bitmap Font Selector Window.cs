/*
 * File: Bitmap Font Selector Window.cs
 *
 * Contains all the functional partial code declaration for the BitmapFontSelectorWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */ 

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Engine;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Engine.Windows
{
    /// <summary>
    ///     Contains all the code to show a dialog window where the user can browse through
    ///     and select different bitmap fonts.
    /// </summary>
    public partial class BitmapFontSelectorWindow : Form
    {
        #region Members
        #region Variables

        private Graphics.BitmapFont _font = null;
        private Graphics.Image _fontImage = null;
        private object _fontLock = new Object();

        private Thread _loadFontThread = null;
        private string _loadFontUrl = "";

        private GraphicsCanvas _canvas = null;

        private bool _ignoreSelections = false;

        #endregion
        #region Properties

        /// <summary>
        ///     Get or set image currently being viewed.
        /// </summary>
        public Graphics.BitmapFont BitmapFont
        {
            get 
            {
                lock (_fontLock)
                {
                    return _font;
                }
            }
            set 
            {
               lock (_fontLock)
               {
                    _font = value;
               }
            }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Called when a new instance of this class is created.
        /// </summary>
        public BitmapFontSelectorWindow()
        {
            InitializeComponent();

            _canvas = new GraphicsCanvas(previewPanel, 0, new CanvasRenderHandler(RenderPreview));

            // Create a callback so we can render the canvas (it won't be done my the main loop due to 
            // this window being show in model).
            Application.Idle += new EventHandler(Application_Idle);
        }

        /// <summary>
        ///     Called when the application is idle. This function is used to render the canvas.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        void Application_Idle(object sender, EventArgs e)
        {
            if (Visible == false)
                return;

            while (NativeMethods.IsApplicationIdle == true)
                GraphicsCanvas.RenderAll();
        }

        /// <summary>
        ///     Populates the list view tree with files that the user can select.
        /// </summary>
        /// <param name="path">Path to populate from.</param>
        /// <param name="collection">Node collection to add new tree nodes to.</param>
        private void PopulateFileTree(string path, TreeNodeCollection collection)
        {
            foreach (string file in Directory.GetFileSystemEntries(path))
            {
                if (File.Exists(file) && Path.GetExtension(file).ToLower() == ".xml")
                {
                    TreeNode newNode = new TreeNode(Path.GetFileName(file));
                    newNode.ImageIndex = 1;
                    newNode.SelectedImageIndex = 1;
                    collection.Add(newNode);

                    string fontUrl = _font != null && _font.URL != null ? _font.URL.ToString().Replace('\\', '/').ToLower() : "";
                    if (fontUrl.IndexOf('@') >= 0)
                    {
                        fontUrl = fontUrl.Substring(fontUrl.IndexOf('@') + 1);
                    }
                    string nodeUrl = (Engine.GlobalInstance.FontPath + "/" + newNode.FullPath).Replace('\\', '/').ToLower();
                    if (nodeUrl == fontUrl)
                        fileTreeView.SelectedNode = newNode;
                }
                else if (Directory.Exists(file))
                {
                    TreeNode newNode = new TreeNode(Path.GetFileName(file));
                    collection.Add(newNode);
                    PopulateFileTree(file, newNode.Nodes);
                }
            }    
        }

        /// <summary>
        ///     Called after a node is selected in the file tree.
        /// </summary>
        /// <param name="sender">Recent file menu that caused this event.</param>
        /// <param name="e">Arguments explaining why this event was invoked.</param>
        private void fileTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_ignoreSelections == true) return;
            ReloadFont();
        }

        /// <summary>
        ///     Reloads the currently selected font.
        /// </summary>
        private void ReloadFont()
        {
            // Check we have selected a node.
            if (fileTreeView.SelectedNode == null || _loadFontThread != null)
                return;

            // Work out the path to the node.
            _loadFontUrl = Engine.GlobalInstance.FontPath + "\\" + fileTreeView.SelectedNode.FullPath;

            // Check its not a dictionary.
            if (ResourceManager.ResourceExists(_loadFontUrl) == false)
                return;

            // Start up the loading thread.
            _font = null;
            _fontImage = null;
            _loadFontThread = new Thread(new ThreadStart(LoadFont));
            _loadFontThread.IsBackground = true;
            _loadFontThread.Start();
        }

        /// <summary>
        ///     Entry point for the font loading thread.
        /// </summary>
        private void LoadFont()
        {
            lock (_fontLock)
            {
                _font = GraphicsManager.LoadFont(_loadFontUrl);
                _fontImage = GraphicsManager.LoadImage(_font.NormalImage.URL, 0);
            }
            _loadFontThread = null;
        }

        /// <summary>
        ///     Renders a preview of the image to the preview panel.
        /// </summary>
        private void RenderPreview()
        {
            if (Visible == false) return;
            lock (_fontLock)
            {
                IRenderTarget previousTarget = GraphicsManager.RenderTarget;
                GraphicsManager.RenderTarget = _canvas;
                GraphicsManager.BeginScene();
                GraphicsManager.ClearColor = unchecked((int)0xFFD3D3D3);
                GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
                GraphicsManager.ClearScene();
                if (_fontImage != null)
                {
                    int imageX = (int)((GraphicsManager.Viewport.Width / 2.0f) - (_fontImage.Width / 2.0f));
                    int imageY = (int)((GraphicsManager.Viewport.Height / 2.0f) - (_fontImage.Height / 2.0f));

                    GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
                    GraphicsManager.RenderImage(_fontImage, imageX, imageY, 0);
                }
                else if (_loadFontThread != null)
                    GraphicsManager.RenderText("Loading...", 5, 5, 0);
                GraphicsManager.FinishScene();
                GraphicsManager.PresentScene();
                GraphicsManager.RenderTarget = previousTarget;
            }
        }

        /// <summary>
        ///     Loads the current image and closes down the window when the user
        ///     clicks the load key.
        /// </summary>
        /// <param name="sender">Recent file menu that caused this event.</param>
        /// <param name="e">Arguments explaining why this event was invoked.</param>
        private void loadButton_Click(object sender, EventArgs e)
        {
            // Check we have selected a node.
            if (fileTreeView.SelectedNode == null)
            {
                MessageBox.Show("Please select a font to load.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Work out the path to the node.
            _loadFontUrl = Engine.GlobalInstance.FontPath + "\\" + fileTreeView.SelectedNode.FullPath;

            // Check its not a dictionary.
            if (ResourceManager.ResourceExists(_loadFontUrl) == false)
            {
                MessageBox.Show("File could not be accessed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _font = GraphicsManager.LoadFont(_loadFontUrl);

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        ///     Closes the window when the user clicks the cancel button.
        /// </summary>
        /// <param name="sender">Recent file menu that caused this event.</param>
        /// <param name="e">Arguments explaining why this event was invoked.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        ///     Called when the visibility of this window is changed.
        /// </summary>
        /// <param name="sender">Recent file menu that caused this event.</param>
        /// <param name="e">Arguments explaining why this event was invoked.</param>
        private void ImageSelectorWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                fileTreeView.Nodes.Clear();
                _ignoreSelections = true;
                PopulateFileTree(Engine.GlobalInstance.FontPath, fileTreeView.Nodes);
                _ignoreSelections = false;
                if (_font != null && _fontImage == null)
                    _fontImage = GraphicsManager.LoadImage(_font.NormalImage.URL, 0);
            }
            else if (_canvas != null)
            {
                fileTreeView.Nodes.Clear();
            }
        }

        #endregion
    }

    /// <summary>
    ///     Used to select and edit bitmap fonts through the propertygrid control.
    /// </summary>
    public class BitmapFontEditor : UITypeEditor
    {
        #region Members
        #region Variables


        #endregion
        #region Properties


        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Gets the style in which this editor edits the value.
        /// </summary>
        /// <param name="context">Context of editing.</param>
        /// <returns>Editor editing mode.</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///     Called when the value needs to be edited.
        /// </summary>
        /// <param name="context">Context of editing.</param>
        /// <param name="provider">Provider of editing.</param>
        /// <param name="value">Original value to edit.</param>
        /// <returns>Edited version of original value.</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            BitmapFontSelectorWindow fontSelectorWindow = new BitmapFontSelectorWindow();
            fontSelectorWindow.BitmapFont = value as Graphics.BitmapFont;
            if (fontSelectorWindow.ShowDialog() == DialogResult.OK)
                return fontSelectorWindow.BitmapFont;

            return base.EditValue(context, provider, value);
        }
        #endregion
    }
}