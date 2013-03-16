/*
 * File: Image Window.cs
 *
 * Contains all the functional partial code declaration for the ImageWindow form.
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
using BinaryPhoenix.Fusion.Runtime;
using System.IO;

namespace BinaryPhoenix.Fusion.Engine.Windows
{

    /// <summary>
    ///     Contains the partial code required to render and update an image viewer window.
    /// </summary>
    public partial class ImageWindow : Form
    {
        #region Members
        #region Variables

        private string _file;
        private Image _image;
        private float _zoom = 1.0f;
        private float _zoomIncrement = 0.25f;

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Initializes a new instance of this control.
        /// </summary>
        /// <param name="file">File containing image to view.</param>
        public ImageWindow(string file)
        {
            InitializeComponent();

            _file = file;
            Stream stream = StreamFactory.RequestStream(file, StreamMode.Open);
            _image = Image.FromStream(stream);
            stream.Close();
            
            pictureBox.BackgroundImage = _image;
            pictureBox.Size = new Size((int)(_image.Width * _zoom), (int)(_image.Height * _zoom));
            pictureBox.Location = new Point((picturePanel.ClientSize.Width / 2) - (pictureBox.Size.Width / 2), (picturePanel.ClientSize.Height / 2) - (pictureBox.Size.Height / 2));
            Text = "Image Viewer (" + ((int)(_zoom * 100)) + "%) - " + file;
        }

        /// <summary>
        ///     Invoked when the zoom out button is clicked.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void zoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            _zoom -= _zoomIncrement;
            zoomOutToolStripButton.Enabled = !(_zoom - _zoomIncrement < _zoomIncrement);

            pictureBox.Size = new Size((int)(_image.Width * _zoom), (int)(_image.Height * _zoom));
            pictureBox.Location = new Point((picturePanel.ClientSize.Width / 2) - (pictureBox.Size.Width / 2), (picturePanel.ClientSize.Height / 2) - (pictureBox.Size.Height / 2));
            Text = "Image Viewer (" + ((int)(_zoom * 100)) + "%) - " + _file;
        }

        /// <summary>
        ///     Invoked when the zoom in button is clicked.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void zoomInToolStripButton_Click(object sender, EventArgs e)
        {
            _zoom += _zoomIncrement;
            zoomOutToolStripButton.Enabled = !(_zoom - _zoomIncrement < _zoomIncrement);

            pictureBox.Size = new Size((int)(_image.Width * _zoom), (int)(_image.Height * _zoom));
            pictureBox.Location = new Point((picturePanel.ClientSize.Width / 2) - (pictureBox.Size.Width / 2), (picturePanel.ClientSize.Height / 2) - (pictureBox.Size.Height / 2));
            Text = "Image Viewer (" + ((int)(_zoom * 100)) + "%) - " + _file;
        }

        #endregion
    }
}
