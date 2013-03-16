/*
 * File: Tileset Window.cs
 *
 * Contains all the functional partial code declaration for the TilesetWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */ 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Events;

namespace BinaryPhoenix.Fusion.Editor.Windows
{

    /// <summary>
    ///     Contains the partial code to show a tileset viewer window.
    /// </summary>
    public partial class TilesetWindow : Form
    {
        #region Members
        #region Variables

        private GraphicsCanvas _canvas;
        private Tileset _tileset;

        private bool _selectingTiles = false;
        private bool _selectedTiles = false;

        private Rectangle _selection;
        private int _color = unchecked((int)0xFFFFFFFF);

        private EventListener _listener;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the current selection.
        /// </summary>
        public Rectangle Selection
        {
            get { return _selection; }
            set { _selection = value; }
        }

        /// <summary>
        ///     Gets or sets the current color.
        /// </summary>
        public int Color
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        ///     Gets or sets the tileset this window shows.
        /// </summary>
        public Tileset Tileset
        {
            get { return _tileset; }
            set { _tileset = value; }
        }

        #endregion
        #region Events

        public event EventHandler SelectionChanged;
        public event EventHandler ColorChanged;

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///		Called when an event is being processed by the EventManager.
        /// </summary>
        /// <param name="firedEvent">Event that needs to be processed.</param>
        public void EventCaptured(Event firedEvent)
        {
            GraphicsManager.RenderTarget = (IRenderTarget)_canvas;
            int mouseX = Input.InputManager.MouseX, mouseY = Input.InputManager.MouseY;
            bool mouseInPanel = !(mouseX < 0 || mouseY < 0 || mouseX > canvasPanel.ClientSize.Width || mouseY > canvasPanel.ClientSize.Height);
            int hOffset = -(tilesetHScrollBar.Value * (_tileset.Image.Width + _tileset.Image.HorizontalSpacing));
            int vOffset = -(tilesetVScrollBar.Value * (_tileset.Image.Height + _tileset.Image.VerticalSpacing));
            int tileX = (mouseX / (_tileset.Image.Width + _tileset.Image.HorizontalSpacing)) + tilesetHScrollBar.Value;
            int tileY = (mouseY / (_tileset.Image.Height + _tileset.Image.VerticalSpacing)) + tilesetVScrollBar.Value;
            int hTileCount = _tileset.Image.FullWidth / (_tileset.Image.Width + _tileset.Image.HorizontalSpacing);
            int vTileCount = _tileset.Image.FullHeight / (_tileset.Image.Height + _tileset.Image.VerticalSpacing);

            // Check what event we have retrieved.
            switch (firedEvent.ID)
            {
                case "key_pressed":
                    if (((Input.InputEventData)firedEvent.Data).KeyCode != BinaryPhoenix.Fusion.Input.KeyCodes.LeftMouse) break;
                    if (mouseInPanel == false) break;
                    if (tileX < 0 || tileY < 0 || tileX >= hTileCount || tileY >= vTileCount) break;
                    _selectingTiles = true;
                    _selectedTiles = false;
                    _selection.X = tileX;
                    _selection.Y = tileY;
                    _selection.Width = _selection.Height = 1;
                    break;

                case "key_released":
                    if (_selectingTiles == false || ((Input.InputEventData)firedEvent.Data).KeyCode != BinaryPhoenix.Fusion.Input.KeyCodes.LeftMouse) break;
                    _selectedTiles = true;
                    _selectingTiles = false;
                    _selection.Width = tileX - _selection.X + (_selection.Width >= 0 ? 1 : 0);
                    _selection.Height = tileY - _selection.Y + (_selection.Height >= 0 ? 1 : 0);
                    if (_selection.Width == 0) _selection.Width = 1;
                    if (_selection.Height == 0) _selection.Height = 1;
                    if (_selection.X + _selection.Width < 0 || _selection.X + _selection.Width > hTileCount) _selection.Width = hTileCount - _selection.X;
                    if (_selection.Y + _selection.Height < 0 || _selection.Y + _selection.Height > vTileCount) _selection.Height = vTileCount - _selection.Y;

                    // If the selection is negative then correct it.
                    if (_selection.Width < 0)
                    {
                        _selection.X += _selection.Width;
                        _selection.Width = -_selection.Width;
                    }
                    if (_selection.Height < 0)
                    {
                        _selection.Y += _selection.Height;
                        _selection.Height = -_selection.Height;
                    }

                    if (SelectionChanged != null) SelectionChanged(this, new EventArgs());
                    break;

                case "mouse_move":
                    if (mouseInPanel == false || _selectingTiles == false) break;
                    _selection.Width = tileX - _selection.X + (_selection.Width >= 0 ? 1 : 0);
                    _selection.Height = tileY - _selection.Y + (_selection.Height >= 0 ? 1 : 0);
                    if (_selection.Width == 0) _selection.Width = 1;
                    if (_selection.Height == 0) _selection.Height = 1;
                    if (_selection.X + _selection.Width < 0 || _selection.X + _selection.Width > hTileCount) _selection.Width = hTileCount - _selection.X;
                    if (_selection.Y + _selection.Height < 0 || _selection.Y + _selection.Height > vTileCount) _selection.Height = vTileCount - _selection.Y;
                    break;
            }

            GraphicsManager.RenderTarget = null;
        }

        /// <summary>
        ///     Invoked when the canvas needs to be rendered.
        /// </summary>
        public void Render()
        {
            if (Visible == false) return;
            GraphicsManager.RenderTarget = (IRenderTarget)_canvas;
            GraphicsManager.BeginScene();
            GraphicsManager.ClearColor = unchecked((int)0xFFACACAC);
            GraphicsManager.ClearScene();
            GraphicsManager.VertexColors.AllVertexs = _color;

            if (_tileset != null)
            {
                // Work out the offset based on the slider values.
                int hOffset = -(tilesetHScrollBar.Value * (_tileset.Image.Width + _tileset.Image.HorizontalSpacing));
                int vOffset = -(tilesetVScrollBar.Value * (_tileset.Image.Height + _tileset.Image.VerticalSpacing));

                // Render the current tileset.
                GraphicsManager.VertexColors.AllVertexs = _color;
                GraphicsManager.RenderImage(_tileset.FullImage, hOffset, vOffset, 0, 0);
                GraphicsManager.DepthBufferEnabled = false;

                // Render the selection cursor to if something has been selected.
                if (_selectedTiles == true || _selectingTiles == true)
                {
                    int x = ((_selection.X) * (_tileset.Image.Width + _tileset.Image.HorizontalSpacing)) + hOffset;
                    int y = ((_selection.Y) * (_tileset.Image.Height + _tileset.Image.VerticalSpacing)) + vOffset;
                    int w = (_selection.Width * (_tileset.Image.Width + _tileset.Image.HorizontalSpacing));
                    int h = (_selection.Height * (_tileset.Image.Height + _tileset.Image.VerticalSpacing));

                    GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
                    GraphicsManager.RenderRectangle(x, y, 0, w, h, false);
                    GraphicsManager.RenderRectangle(x + 2, y + 2, 0, w - 4, h - 4, false);
                    GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFF0000);
                    GraphicsManager.RenderRectangle(x + 1, y + 1, 0, w - 2, h - 2, false);
                    GraphicsManager.VertexColors.AllVertexs = unchecked((int)0x6600AAFF);
                    GraphicsManager.RenderRectangle(x + 3, y + 3, 0, w - 6, h - 6, true);
                }

                // Render the tile cursor.
                int mouseX = Input.InputManager.MouseX, mouseY = Input.InputManager.MouseY;
                bool mouseInPanel = !(mouseX < 0 || mouseY < 0 || mouseX > canvasPanel.ClientSize.Width || mouseY > canvasPanel.ClientSize.Height);
                int tileX = (mouseX / (_tileset.Image.Width + _tileset.Image.HorizontalSpacing)) + tilesetHScrollBar.Value;
                int tileY = (mouseY / (_tileset.Image.Height + _tileset.Image.VerticalSpacing)) + tilesetVScrollBar.Value;
                int rx = (tileX * _tileset.Image.Width) + hOffset + (_tileset.Image.HorizontalSpacing * tileX);
                int ry = (tileY * _tileset.Image.Height) + vOffset + (_tileset.Image.VerticalSpacing * tileY);
                int rw = _tileset.Image.Width;
                int rh = _tileset.Image.Height;

                GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
                GraphicsManager.RenderRectangle(rx, ry, 0, rw, rh, false);
                GraphicsManager.RenderRectangle(rx + 2, ry + 2, 0, rw - 4, rh - 4, false);
                GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFF0000);
                GraphicsManager.RenderRectangle(rx + 1, ry + 1, 0, rw - 2, rh - 2, false);
            }

            GraphicsManager.FinishScene();
            GraphicsManager.PresentScene();
            GraphicsManager.RenderTarget = null;
        }

        /// <summary>
        ///     Invoked when the color panel is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void colorPanel_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = System.Drawing.Color.FromArgb(_color);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                int r, g, b, a;
                ColorMethods.SplitColor(ColorFormats.A8R8G8B8, _color, out r, out g, out b, out a);
                _color = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B, a);
                colorPanel.BackColor = System.Drawing.Color.FromArgb(_color);
                if (SelectionChanged != null) SelectionChanged(this, new EventArgs());
            }
        }

        /// <summary>
        ///     Invoked when the alpha track bar's value is changed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void alphaTrackBar_Scroll(object sender, EventArgs e)
        {
            int r, g, b, a;
            ColorMethods.SplitColor(ColorFormats.A8R8G8B8, _color, out r, out g, out b, out a);
            _color = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, r, g, b, alphaTrackBar.Value);
            if (SelectionChanged != null) SelectionChanged(this, new EventArgs());	
        }

        /// <summary>
        ///     Disposes of this tileset window.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void TilesetWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            EventManager.DetachListener(_listener);
            _listener = null;
            _tileset = null;
            GC.Collect();
        }

        /// <summary>
        ///     Invoked when this window is resized.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void TilesetWindow_ResizeEnd(object sender, EventArgs e)
        {
            tilesetHScrollBar.Maximum = Math.Max((_tileset.Image.FullWidth / _tileset.Image.Width) - (canvasPanel.ClientSize.Width / _tileset.Image.Width) + tilesetHScrollBar.LargeChange - 1, tilesetHScrollBar.LargeChange - 1);
            tilesetVScrollBar.Maximum = Math.Max((_tileset.Image.FullHeight / _tileset.Image.Height) - (canvasPanel.ClientSize.Height / _tileset.Image.Height) + tilesetVScrollBar.LargeChange - 1, tilesetVScrollBar.LargeChange - 1);
            tilesetHScrollBar.Enabled = tilesetHScrollBar.Maximum == tilesetHScrollBar.LargeChange - 1 ? false : true;
            tilesetVScrollBar.Enabled = tilesetVScrollBar.Maximum == tilesetVScrollBar.LargeChange - 1 ? false : true;
            tilesetHScrollBar.Value = tilesetHScrollBar.Maximum < tilesetHScrollBar.LargeChange ? 0 : tilesetHScrollBar.Value;
            tilesetVScrollBar.Value = tilesetVScrollBar.Maximum < tilesetHScrollBar.LargeChange ? 0 : tilesetVScrollBar.Value;
        }

        /// <summary>
        ///     Initializes a new instance of this control.
        /// </summary>
        /// <param name="file">Tileset file to display.</param>
        public TilesetWindow(string file)
        {
            InitializeComponent();

            _canvas = new GraphicsCanvas(canvasPanel, 0, Render);
            _tileset = new Tileset(file);

            TilesetWindow_ResizeEnd(null, new EventArgs());

            EventManager.AttachListener(_listener = new EventListener(EventCaptured));
        }

        #endregion
    }

}
