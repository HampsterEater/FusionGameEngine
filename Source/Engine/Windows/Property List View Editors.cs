/*
 * File: Property List View Editors.cs
 *
 * Contains several editors used by the property list view to modify values.
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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Engine.Windows;
using BinaryPhoenix.Fusion.Runtime.Controls;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Engine.Windows
{
    /// <summary>
    ///     Used to select and edit file through the propertygrid control.
    /// </summary>
    public class FileEditorValue
    {
        #region Members
        #region Variables

        private string _fileUrl, _filter, _baseDir;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the url of the file this class is refering to.
        /// </summary>
        public string FileUrl
        {
            get { return _fileUrl; }
            set { _fileUrl = value; }
        }

        /// <summary>
        ///     Gets or sets the filter used when selecting a file.
        /// </summary>
        public string Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        /// <summary>   
        ///     Gets or sets the root directory that all files must be selected out of.
        /// </summary>  
        public string BaseDir
        {
            get { return _baseDir; }
            set { _baseDir = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this class to a textural format.
        /// </summary>
        /// <returns>Textural form of this class.</returns>
        public override string ToString()
        {
            return _fileUrl;
        }

        /// <summary>
        ///     Invoked when a new instance of this class is created.
        /// </summary>
        /// <param name="file">Url of initial file this value should hold.</param>
        /// <param name="filter">Filter to use when selecting files.</param>
        /// <param name="baseDir">Root directory that file must be selected out of.</param>
        public FileEditorValue(string file, string filter, string baseDir)
        {
            _fileUrl = file;
            _filter = filter;
            _baseDir = baseDir;
        }

        #endregion
    }

    /// <summary>
    ///     Used to select and edit file through the propertygrid control.
    /// </summary>
    public class FileEditor : UITypeEditor
    {
        #region Members
        #region Variables

        private FileEditorValue _editorValue;

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
            _editorValue = value as FileEditorValue;
            if (_editorValue == null) return base.EditValue(context, provider, value);

            OpenFileDialog fileWindow = new OpenFileDialog();
            if (_editorValue.Filter != "") fileWindow.Filter = _editorValue.Filter;
            if (_editorValue.FileUrl != "" && File.Exists(_editorValue.FileUrl)) fileWindow.FileName = _editorValue.FileUrl;
            if (_editorValue.BaseDir != "") fileWindow.InitialDirectory = _editorValue.BaseDir;
            fileWindow.FileOk += new CancelEventHandler(fileWindow_FileOk);
            fileWindow.RestoreDirectory = true;
            if (fileWindow.ShowDialog() == DialogResult.OK)
            {
                _editorValue.FileUrl = fileWindow.FileName;
                string baseDir = _editorValue.BaseDir;
                if (baseDir == "") baseDir = Engine.GlobalInstance.GamePath;

                if (baseDir != "" && _editorValue.FileUrl.ToLower().StartsWith(baseDir.ToLower()) == true)
                    _editorValue.FileUrl = _editorValue.FileUrl.Substring(baseDir.Length + 1);

                // We've got to create a new file editor value or the 
                // property grid will play up.
                FileEditorValue newValue = new FileEditorValue(_editorValue.FileUrl, _editorValue.Filter, _editorValue.BaseDir);
                _editorValue = newValue;
                return newValue;
            }

            return base.EditValue(context, provider, value);
        }

        /// <summary>
        ///     Invoked when the user has selected a file in the file window and clicks ok.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        void fileWindow_FileOk(object sender, CancelEventArgs e)
        {
            FileDialog fileWindow = sender as FileDialog;
            string baseDir = _editorValue.BaseDir;
            if (baseDir == "") baseDir = Engine.GlobalInstance.GamePath;

            if (fileWindow.FileName.ToLower().StartsWith(baseDir.ToLower()) == false)
            {
                MessageBox.Show("File must be within the root directory or a sub directory of it.", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fileWindow.InitialDirectory = baseDir;
                e.Cancel = true;
            }
        }

        #endregion
    }
}