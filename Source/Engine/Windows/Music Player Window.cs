/*
 * File: Music Player Window.cs
 *
 * Contains all the functional partial code declaration for the MusicPlayerWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */ 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Audio;

namespace BinaryPhoenix.Fusion.Engine.Windows
{

    /// <summary>
    ///     Contains the partial code to show a music player window.
    /// </summary>
    public partial class MusicPlayerWindow : Form
    {
        #region Members
        #region Variables

        private Sound _sound = null;
        private ISampleBuffer _soundChannel = null;

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Invoked when the looping button is check or unchecked.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void loopingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_soundChannel.Finished == true && _soundChannel.Paused == false)
            {
                pauseButton.ImageIndex = 0;
                pauseButton.Text = "Pause";
            }
            _soundChannel.Looping = !_soundChannel.Looping;
        }

        /// <summary>
        ///     Invoked when the pause button is clicked.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (_soundChannel.Finished == true && _soundChannel.Paused == false)
            {
                _soundChannel = null;
                _soundChannel = _sound.Play();
                pauseButton.ImageIndex = 0;
                pauseButton.Text = "Pause";
                return;
            }

            if (_soundChannel.Paused == false)
            {
                _soundChannel.Paused = true;
                pauseButton.ImageIndex = 1;
                pauseButton.Text = "Resume";
            }
            else
            {
                _soundChannel.Paused = false;
                pauseButton.ImageIndex = 0;
                pauseButton.Text = "Pause";
            }
        }

        /// <summary>
        ///     Invoked when the volume is changed.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void volumeTrackBar_Scroll(object sender, EventArgs e)
        {
            _soundChannel.Volume = volumeTrackBar.Value;
        }

        /// <summary>
        ///     Invoked when the pan is changed.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void panTrackBar_Scroll(object sender, EventArgs e)
        {
            _soundChannel.Pan = panTrackBar.Value / 50.0f;
        }

        /// <summary>
        ///     Invoked when the frequency is changed.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void frequencyTrackBar_Scroll(object sender, EventArgs e)
        {
            _soundChannel.Frequency = frequencyTrackBar.Value;
        }

        /// <summary>
        ///     Invoked when the update timer ticks.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (_soundChannel.Length != 0 && _soundChannel.Position != 0) progressBar.Value = (int)((100.0f / (float)_soundChannel.Length) * (float)_soundChannel.Position);
            if (_soundChannel.Finished == true && _soundChannel.Paused == false)
            {
                pauseButton.ImageIndex = 1;
                pauseButton.Text = "Restart";
            }
        }

        /// <summary>
        ///     Disposes of this class when it is closed.
        /// </summary>
        /// <param name="sender">Object that invoked this event.</param>
        /// <param name="e">Reason why this event was invoked.</param>
        private void MusicPlayerWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _soundChannel.Stop();
            _soundChannel = null;
            _sound.Stop();
            _sound = null;
            GC.Collect();
        }

        /// <summary>
        ///     Initializes a new instance of this class. 
        /// </summary>
        /// <param name="file">Music file to play.</param>
        public MusicPlayerWindow(string file)
        {
            InitializeComponent();

            _sound = AudioManager.LoadSound(file, 0);
            _soundChannel = _sound.Play();
            frequencyTrackBar.Value = _soundChannel.Frequency;
            panTrackBar.Value = (int)(_soundChannel.Pan * 50);
            volumeTrackBar.Value = (int)(_soundChannel.Volume);
            updateTimer.Start();
            fileLabel.Text = Path.GetFileName(file);
            Text = "Music Player - " + Path.GetFileName(file);
        }

        #endregion
    }

}
