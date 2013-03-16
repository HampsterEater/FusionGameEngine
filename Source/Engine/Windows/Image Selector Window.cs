/*
 * File: Image Selector Window.cs
 *
 * Contains all the functional partial code declaration for the ImageSelectorWindow form.
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
    ///     and select different images.
    /// </summary>
    public partial class ImageSelectorWindow : Form
    {
        #region Members
        #region Variables

        private Graphics.Image _image = null;
        private object _imageLock = new Object();

        private Thread _loadImageThread = null;
        private string _loadImageUrl = "";

        private GraphicsCanvas _canvas = null;

        private bool _ignoreSelections = false;

        private int _finalCellWidth, _finalCellHeight, _finalCellXSpace, _finalCellYSpace, _finalColorKey;

        private bool _reloadImage = false;

        #endregion
        #region Properties

        /// <summary>
        ///     Get or set image currently being viewed.
        /// </summary>
        public Graphics.Image Image
        {
            get 
            {
                lock (_imageLock)
                {
                    // Reload image.
                    Graphics.Image newImage = null;
                    if (_image != null)
                    {
                        GraphicsManager.ColorKey = _finalColorKey;
                        if (_finalCellWidth == 0 || _finalCellHeight == 0)
                            newImage = GraphicsManager.LoadImage(_image.URL, 0, false);
                        else
                            newImage = GraphicsManager.LoadImage(_image.URL, _finalCellWidth, _finalCellHeight, _finalCellXSpace, _finalCellYSpace, 0, false);
                    }
                    return newImage;
                }
            }
            set 
            {
               lock (_imageLock)
               {
                    _image = value;
                    if (_image != null)
                    {
                        _finalCellWidth = _image.Width;
                        _finalCellHeight = _image.Height;
                        _finalCellXSpace = _image.HorizontalSpacing;
                        _finalCellYSpace = _image.VerticalSpacing;
                        _finalColorKey = _image.ColorKey;

                        GraphicsManager.ColorKey = _finalColorKey;
                        if (_finalCellWidth == 0 || _finalCellHeight == 0)
                            _image = GraphicsManager.LoadImage(_image.URL, 0, false);
                        else
                            _image = GraphicsManager.LoadImage(_image.URL, _finalCellWidth, _finalCellHeight, _finalCellXSpace, _finalCellYSpace, 0, false);
                    }
               }
            }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Called when a new instance of this class is created.
        /// </summary>
        public ImageSelectorWindow()
        {
            InitializeComponent();

            // Create the canvas we will be rendering to.
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
                string extension = Path.GetExtension(file).ToLower();
                if (File.Exists(file) && (extension == ".png" || extension == ".bmp" || extension == ".tga"))
                {
                    TreeNode newNode = new TreeNode(Path.GetFileName(file));
                    newNode.ImageIndex = 1;
                    newNode.SelectedImageIndex = 1;
                    collection.Add(newNode);

                    string imageUrl = _image != null && _image.URL != null ? _image.URL.ToString().Replace('\\', '/').ToLower() : "";
                    if (imageUrl.IndexOf('@') >= 0)
                    {
                        imageUrl = imageUrl.Substring(imageUrl.IndexOf('@') + 1);
                    }
                    string nodeUrl = (Engine.GlobalInstance.GraphicPath + "/" + newNode.FullPath).Replace('\\', '/').ToLower();
                    if (nodeUrl == imageUrl)
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
            _reloadImage = true;
        }

        /// <summary>
        ///     Reloads the currently selected image.
        /// </summary>
        private void ReloadImage()
        {
            // Check we have selected a node.
            if (fileTreeView.SelectedNode == null || _loadImageThread != null)
                return;

            // Work out the path to the node.
            _loadImageUrl = Engine.GlobalInstance.GraphicPath + "\\" + fileTreeView.SelectedNode.FullPath;

            // Check its not a dictionary.
            if (ResourceManager.ResourceExists(_loadImageUrl) == false)
                return;

            // Start up the loading thread.
            int originalColorKey = GraphicsManager.ColorKey;
            GraphicsManager.ColorKey = maskColorPanel.BackColor.ToArgb();
            _image = GraphicsManager.LoadImage(_loadImageUrl, 0, false);
            GraphicsManager.ColorKey = originalColorKey;
        }

        /// <summary>
        ///     Renders a preview of the image to the preview panel.
        /// </summary>
        private void RenderPreview()
        {
            if (Visible == false) return;
            lock (_imageLock)
            {
                IRenderTarget previousTarget = GraphicsManager.RenderTarget;
                GraphicsManager.RenderTarget = _canvas;
                GraphicsManager.Viewport = new Rectangle(0, 0, previewPanel.Width, previewPanel.Height);
                GraphicsManager.SetResolution(previewPanel.Width, previewPanel.Height, false);
                GraphicsManager.BeginScene();
                GraphicsManager.ClearColor = unchecked((int)0xFFD3D3D3);
                GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
                GraphicsManager.ClearScene();

                if (_reloadImage == true)
                {
                    ReloadImage();
                    _reloadImage = false;
                }

                if (_image != null)
                {
                    int imageX = (int)((GraphicsManager.Viewport.Width / 2.0f) - (_image.Width / 2.0f));
                    int imageY = (int)((GraphicsManager.Viewport.Height / 2.0f) - (_image.Height / 2.0f));
                    int cellWidth = (int)cellWidthBox.Value;
                    int cellHeight = (int)cellHeightBox.Value;
                    int cellSpacingX = (int)cellSpaceXBox.Value;
                    int cellSpacingY = (int)cellSpaceYBox.Value;

                    GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
                    GraphicsManager.RenderImage(_image, imageX, imageY, 0);
                    if (cellWidth != 0 && cellHeight != 0)
                    {
                        GraphicsManager.BlendMode = BlendMode.Invert;
                        GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF666666);
                        int cellsH = (_image.Width / (cellWidth + cellSpacingX));
                        int cellsV = (_image.Height / (cellHeight + cellSpacingY));
                            
                        for (int i = 0; i < (cellsH * cellsV); i++)
                        {
                            int px = (i % cellsH) * cellWidth;
                            int py = (i / cellsH) * cellHeight;
                            GraphicsManager.RenderDashedRectangle(imageX + px + ((i % cellsH) * cellSpacingX), imageY + py + ((i / cellsH) * cellSpacingY), 0, cellWidth, cellHeight, 1);
                        }
                        GraphicsManager.BlendMode = BlendMode.Alpha;
                        GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
                    }
                }
                else if (_loadImageThread != null)
                    GraphicsManager.RenderText("Loading...", 5, 5, 0);
                GraphicsManager.FinishScene();
                GraphicsManager.PresentScene();
                GraphicsManager.RenderTarget = previousTarget;
            }
        }

        /// <summary>
        ///     Shows a color select dialog when the user clicks the mask color panel.
        /// </summary>
        /// <param name="sender">Recent file menu that caused this event.</param>
        /// <param name="e">Arguments explaining why this event was invoked.</param>
        private void maskColorPanel_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = maskColorPanel.BackColor;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                maskColorPanel.BackColor = dialog.Color;
                ReloadImage();
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
                MessageBox.Show("Please select an image to load.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Work out the path to the node.
            _loadImageUrl = Engine.GlobalInstance.GraphicPath + "\\" + fileTreeView.SelectedNode.FullPath;

            // Check its not a dictionary.
            if (ResourceManager.ResourceExists(_loadImageUrl) == false)
            {
                MessageBox.Show("File could not be accessed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _finalCellWidth = (int)cellWidthBox.Value;
            _finalCellHeight = (int)cellHeightBox.Value;
            _finalCellXSpace = (int)cellSpaceXBox.Value;
            _finalCellYSpace = (int)cellSpaceYBox.Value;
            _finalColorKey = maskColorPanel.BackColor.ToArgb();

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
                PopulateFileTree(Engine.GlobalInstance.GraphicPath, fileTreeView.Nodes);
                _ignoreSelections = false;

                if (_image != null)
                {
                    maskColorPanel.BackColor = Color.FromArgb(_image.ColorKey);
                    if (!(_image.Width == _image.FullWidth && _image.Height == _image.FullHeight))
                    {
                        cellWidthBox.Value = _image.Width;
                        cellHeightBox.Value = _image.Height;
                    }
                    cellSpaceXBox.Value = _image.HorizontalSpacing;
                    cellSpaceYBox.Value = _image.VerticalSpacing;
                    ReloadImage();
                }
            }
            else if (_canvas != null)
            {
                fileTreeView.Nodes.Clear();
            }
        }

        #endregion
    }

    /// <summary>
    ///     Used to select and edit images through the propertygrid control.
    /// </summary>
    public class ImageEditor : UITypeEditor
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
            ImageSelectorWindow imageSelectorWindow = new ImageSelectorWindow();
            imageSelectorWindow.Image = value as Graphics.Image;
            if (imageSelectorWindow.ShowDialog() == DialogResult.OK)
                return imageSelectorWindow.Image;

            return base.EditValue(context, provider, value);
        }
        #endregion
    }

}