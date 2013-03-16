/*
 * File: Sound Selector Window.cs
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
using BinaryPhoenix.Fusion.Audio;

namespace BinaryPhoenix.Fusion.Engine.Windows
{
    /// <summary>
    ///     Contains all the code to show a dialog window where the user can browse through
    ///     and select different sounds.
    /// </summary>
    public partial class SoundSelectorWindow : Form
    {
        #region Members
        #region Variables

        private ISampleBuffer _soundChannel = null;
        private Sound _sound = null;
        private bool _ignoreSelections = false;

        private bool _streamed = false;
        private bool _positional = false;

        #endregion
        #region Properties

        /// <summary>
        ///     Get or set sound currently being viewed.
        /// </summary>
        public Sound Sound
        {
            get { return _sound;}
            set { _sound = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Called when a new instance of this class is created.
        /// </summary>
        public SoundSelectorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SyncronizeWindow()
        {
            loopingCheckBox.Enabled = (_sound != null);
            streamedCheckBox.Enabled = (_sound != null);
            positionalCheckBox.Enabled = (_sound != null);
            panTrackBar.Enabled = (_sound != null);
            volumeTrackBar.Enabled = (_sound != null);
            frequencyTrackBar.Enabled = (_sound != null);
            fileLabel.Enabled = (_sound != null);
            pauseButton.Enabled = (_sound != null);
            progressBar.Enabled = (_sound != null);
            updateTimer.Enabled = (_sound != null);

            if (_sound != null)
            {
                fileLabel.Text = Path.GetFileNameWithoutExtension(_sound.URL);
                frequencyTrackBar.Value = _soundChannel.Frequency;
                panTrackBar.Value = (int)(_soundChannel.Pan * 50);
                volumeTrackBar.Value = (int)(_soundChannel.Volume);  
            }
            else
            {
                fileLabel.Text = "";
            }
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
                if (File.Exists(file) && (extension == ".wav" || extension == ".ogg" || extension == ".mp3" || extension == ".mid" || extension == ".midi" || extension == ".wma"))
                {
                    TreeNode newNode = new TreeNode(Path.GetFileName(file));
                    newNode.ImageIndex = 1;
                    newNode.SelectedImageIndex = 1;
                    collection.Add(newNode);

                    string imageUrl = _sound != null && _sound.URL != null ? _sound.URL.ToString().Replace('\\', '/').ToLower() : "";
                    if (imageUrl.IndexOf('@') >= 0)
                    {
                        imageUrl = imageUrl.Substring(imageUrl.IndexOf('@') + 1);
                    }
                    string nodeUrl = (Engine.GlobalInstance.AudioPath + "/" + newNode.FullPath).Replace('\\', '/').ToLower();
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
            ReloadSound();
        }

        /// <summary>
        ///     Reloads the currently selected sound.
        /// </summary>
        private void ReloadSound()
        {
            if (_soundChannel != null)
            {
                _soundChannel.Stop();
                _soundChannel = null;
            }
            if (_sound != null)
            {
                _sound.Stop();
                _sound = null;
            }
            GC.Collect();

            SoundFlags flags = 0;
            if (_streamed == true) flags |= SoundFlags.Streamed;
            if (_positional == true) flags |= SoundFlags.Positional;
            if (fileTreeView.SelectedNode != null && File.Exists(Engine.GlobalInstance.AudioPath + "/" + fileTreeView.SelectedNode.FullPath))
            {
                _sound = AudioManager.LoadSound(Engine.GlobalInstance.AudioPath + "/" + fileTreeView.SelectedNode.FullPath, flags);
                _soundChannel = _sound.Play();
            }
            SyncronizeWindow();
        }

        /// <summary>
        ///     Loads the current sound and closes down the window when the user
        ///     clicks the load key.
        /// </summary>
        /// <param name="sender">Recent file menu that caused this event.</param>
        /// <param name="e">Arguments explaining why this event was invoked.</param>
        private void loadButton_Click(object sender, EventArgs e)
        {
            // Check we have selected a node.
            if (fileTreeView.SelectedNode == null || !File.Exists(Engine.GlobalInstance.AudioPath + "/" + fileTreeView.SelectedNode.FullPath))
            {
                MessageBox.Show("Please select an sound to load.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ReloadSound();

            updateTimer.Enabled = false;
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void volumeTrackBar_Scroll(object sender, EventArgs e)
        {
            _soundChannel.Volume = volumeTrackBar.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panTrackBar_Scroll(object sender, EventArgs e)
        {
            _soundChannel.Pan = panTrackBar.Value / 50.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frequencyTrackBar_Scroll(object sender, EventArgs e)
        {
            _soundChannel.Frequency = frequencyTrackBar.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void streamedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _streamed = streamedCheckBox.Checked; ;
            ReloadSound();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void positionalCheckBox_Click(object sender, EventArgs e)
        {
            _positional = positionalCheckBox.Checked;
            ReloadSound();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SoundSelectorWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_soundChannel != null)
            {
                _soundChannel.Stop();
                _soundChannel = null;
            }
            GC.Collect();
        }

        /// <summary>
        ///     Called when the visibility of this window is changed.
        /// </summary>
        /// <param name="sender">Recent file menu that caused this event.</param>
        /// <param name="e">Arguments explaining why this event was invoked.</param>
        private void SoundSelectorWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                fileTreeView.Nodes.Clear();
                _ignoreSelections = true;
                PopulateFileTree(Engine.GlobalInstance.AudioPath, fileTreeView.Nodes);
                _ignoreSelections = false;
                ReloadSound();
            }
        }

        #endregion
    }

    /// <summary>
    ///     Used to select and edit sounds through the propertygrid control.
    /// </summary>
    public class SoundEditor : UITypeEditor
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
            SoundSelectorWindow soundSelectorWindow = new SoundSelectorWindow();
            soundSelectorWindow.Sound = value as Audio.Sound;
            if (soundSelectorWindow.ShowDialog() == DialogResult.OK)
                return soundSelectorWindow.Sound;

            return base.EditValue(context, provider, value);
        }
        #endregion
    }

}