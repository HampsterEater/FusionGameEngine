/*
 * File: Splash Screen Window.cs
 *
 * Contains all the functional partial code declaration for the SplashScreenWindow form.
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

namespace BinaryPhoenix.Fusion.Engine.Windows
{

    /// <summary>
    ///     Contains all the code required to create a splash screen form.
    /// </summary>
    public partial class SplashScreenWindow : Form
    {

        /// <summary>
        ///     Gets or sets the messages shown in the loading bar.
        /// </summary>
        public string LoadingMessage
        {
            get { return loadingLabel.Text; }
            set { loadingLabel.Text = value; }
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public SplashScreenWindow()
        {
            InitializeComponent();
        }

    }
}