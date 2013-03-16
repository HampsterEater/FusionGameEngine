/*
 * File: Asset Manager Window.cs
 *
 * Contains all the functional partial code declaration for the AssetManagerWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */ 

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime.Events;
using BinaryPhoenix.Fusion.Audio;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Engine.Windows;

namespace BinaryPhoenix.Fusion.Editor.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the asset manager window.
	/// </summary>
	public partial class AssetManagerWindow : Form
	{
		#region Members
		#region Variables

        private string _selectedObjectURL = "";

		private EventListener _listener = null;

        private string _selectedTileset = "";
        private Rectangle _tilesetSelection;
        private int _tilesetColor = 0;

		#endregion
		#region Events

		public event EventHandler ObjectChanged;
		public event EventHandler TilesetSelectionChanged;
        public event EventHandler ScriptModified;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the tileset that has been selected.
		/// </summary>
		public string SelectedTileset
		{
			get { return _selectedTileset; }
            set { _selectedTileset = value; }
		}

		/// <summary>
		///		Gets or sets the tiles that have been selected in the rectangle.
		/// </summary>
		public Rectangle TilesetSelection
		{
            get { return _tilesetSelection; }
            set { _tilesetSelection = value; }
		}

		/// <summary>
		///		Gets or sets the color the tileset should be colored.
		/// </summary>
		public int TilesetColor
		{
			get { return _tilesetColor; }
            set { _tilesetColor = value; }
		}

		/// <summary>
		///		Gets the url of the currently selected object.
		/// </summary>
		public string SelectedObjectURL
		{
            get { return _selectedObjectURL; }
            set { _selectedObjectURL = value; }
		}


		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class and sets up all the 
		///		controls on this form.
		/// </summary>
		public AssetManagerWindow()
		{
			// Initialize the controls on this window.
			InitializeComponent();

			// Create a new event listener to listen out for input events.
			_listener = new EventListener(new ProcessorDelegate(EventCaptured));
			EventManager.AttachListener(_listener);

			// Fill the audio and object tree views.
			FillTreeViews();

			// Sync all the page's data up. ;).
			SyncronizeData();
		}

		/// <summary>
		///		Fills the audio and entity tree view will all files appropriate.
		/// </summary>
		public void FillTreeViews()
		{
			// Fill the audio and object tree views with the files in the audio and object directorys.
            projectTreeView.Nodes.Clear();
            FillProjectTreeView(Environment.CurrentDirectory, projectTreeView.Nodes);
		}

        /// <summary>
        ///		Fills the project tree view with all the files in the project folder.
        /// </summary>
        /// <param name="folder">Folder to fill tree view from.</param>
        /// <param name="nodeCollection">Collection to add nodes to.</param>
        public void FillProjectTreeView(string folder, TreeNodeCollection nodeCollection)
        {
            foreach (string file in Directory.GetFileSystemEntries(folder))
            {
                string extension = Path.GetExtension(file).ToLower();
                if (File.Exists(file))
                {
                    TreeNode newNode = new TreeNode(Path.GetFileName(file));
                    newNode.ImageIndex = ExtensionToImageIndex(Path.GetExtension(file).ToLower());
                    newNode.SelectedImageIndex = newNode.ImageIndex;
                    nodeCollection.Add(newNode);
                }
                else if (Directory.Exists(file))
                {
                    TreeNode newNode = new TreeNode(Path.GetFileName(file));
                    nodeCollection.Add(newNode);
                    FillProjectTreeView(file, newNode.Nodes);
                }
            }
        }

        /// <summary>
        ///     Works out the correct image index to use on the project tree based on an extension.
        /// </summary>
        /// <param name="extension">Extension to convert.</param>
        /// <returns>Iamge index representing extension.</returns>
        public int ExtensionToImageIndex(string extension)
        {
            switch (extension.ToLower())
            {
                case ".wav":
                case ".ogg":
                    return 1;

                case ".fso":
                case ".fs":
                case ".fsl":
                    return 7;

                case ".pk":
                case ".dll":
                    return 5;

                case ".fef":
                    return 2;

                case ".fmp":
                    return 4;

                case ".xml":
                    return 8;

                case ".png":
                case ".bmp":
                case ".tga":
                    return 9;

                default:
                    return 6;
            }
        }

		/// <summary>
		///		Called when an event is being processed by the EventManager.
		/// </summary>
		/// <param name="firedEvent">Event that needs to be processed.</param>
		public void EventCaptured(Event firedEvent)
		{
			
		}

		/// <summary>
		///		Syncronizes all the data shown in the asset manager to the current data.
		/// </summary>
		public void SyncronizeData()
		{
            bool isFile = (projectTreeView.SelectedNode != null && File.Exists(projectTreeView.SelectedNode.FullPath)) ? true : false;
            bool isFolder = (projectTreeView.SelectedNode != null && Directory.Exists(projectTreeView.SelectedNode.FullPath)) ? true : false;

            // Context menu / toolbar.
            newFileToolStripButton.Enabled = newFileToolStripMenuItem.Enabled = isFolder || isFile;
            newFolderToolStripButton.Enabled = newFolderToolStripMenuItem.Enabled = isFolder || isFile;
            openToolStripButton.Enabled = openToolStripMenuItem.Enabled = isFile;
            exportToolStripButton.Enabled = exportToolStripMenuItem.Enabled = isFile || isFolder;
            importToolStripButton.Enabled = importToolStripMenuItem.Enabled = isFile || isFolder;
            deleteToolStripButton.Enabled = deleteToolStripMenuItem.Enabled = isFile || isFolder;
            renameToolStripButton.Enabled = renameToolStripMenuItem.Enabled = isFile || isFolder;
            duplicateToolStripButton.Enabled = duplicateToolStripMenuItem.Enabled = isFile || isFolder;
		}

		/// <summary>
		///		Stops the window from closing, but instead make it hide itself.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void AssetManagerWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

        /// <summary>
        ///     Called when the visibility of this form is changed, mainly used to refresh 
        ///     data displayed on the form.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void AssetManagerWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                // Fill the audio and object tree views.
                FillTreeViews();

                // Sync all the page's data up. ;).
                SyncronizeData();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string extension = Path.GetExtension(e.Node.FullPath).ToLower();
            if (extension == ".fso")
            {
                _selectedObjectURL = e.Node.FullPath;
                if (ObjectChanged != null) ObjectChanged(this, new EventArgs());
            }
            SyncronizeData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null)
                return;

            string oldFile = Environment.CurrentDirectory + "\\" + e.Node.FullPath;
            string newPath = e.Node.FullPath;
            if (newPath.LastIndexOf('\\') >= 0)
                newPath = newPath.Substring(0, newPath.LastIndexOf('\\') + 1) + e.Label;
            else
                newPath = e.Label;
            newPath = Environment.CurrentDirectory + "\\" + newPath;

            if (File.Exists(newPath) || Directory.Exists(newPath))
            {
                MessageBox.Show("The file name given is already in use. Please use another.", "Name Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.CancelEdit = true;
                return;
            }

            if (File.Exists(oldFile) == true)
            {
                File.Move(oldFile, newPath);
                e.Node.ImageIndex = ExtensionToImageIndex(Path.GetExtension(newPath).ToLower());
                e.Node.SelectedImageIndex = e.Node.ImageIndex;
            }
            else if (Directory.Exists(oldFile) == true)
                Directory.Move(oldFile, newPath);
            SyncronizeData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectTreeView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && projectTreeView.SelectedNode != null)
                fileContextMenuStrip.Show(projectTreeView, e.X, e.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectTreeView.SelectedNode == null) return;
            switch (Path.GetExtension(projectTreeView.SelectedNode.FullPath.ToLower()))
            {
                case ".wav":
                case ".ogg":
                    {
                        MusicPlayerWindow window = new MusicPlayerWindow(projectTreeView.SelectedNode.FullPath);
                        window.Show(this.Parent);
                        break;
                    }

                case ".fso":
                case ".fs":
                case ".fsl":
                    {
                        ScriptEditorWindow window = new ScriptEditorWindow(projectTreeView.SelectedNode.FullPath);
                        window.FormClosing += new FormClosingEventHandler(ScriptEditorClosed);
                        window.Show(this);
                    }
                    break;

                //case ".pk":
                //case ".dll":
                //     break;

                case ".fef":
                     {
                         EmitterEditorWindow window = new EmitterEditorWindow();
                         if (IOMethods.FileSize(projectTreeView.SelectedNode.FullPath.ToLower()) > 3)
                         {
                             Stream stream = StreamFactory.RequestStream(projectTreeView.SelectedNode.FullPath, StreamMode.Open);
                             BinaryReader reader = new BinaryReader(stream);
                             reader.ReadBytes(3);

                             window.Emitter = new EmitterNode();
                             window.Emitter.Load(reader);

                             reader.Close();
                             stream.Close();
                         }
                         //if (window.ShowDialog() == DialogResult.OK)
                         //{
                         //   Stream stream = StreamFactory.RequestStream(projectTreeView.SelectedNode.FullPath, StreamMode.Truncate);
                         //    BinaryWriter writer = new BinaryWriter(stream);

                         //    writer.Write(new byte[] { (byte)'F', (byte)'E', (byte)'F' });
                         //    window.Emitter.Save(writer);

                         //    writer.Close();
                         //    stream.Close();
                         //}
                         break;
                     }

                case ".fmp":
                     Editor.GlobalInstance.Window.OpenMap(projectTreeView.SelectedNode.FullPath);
                     break;

                case ".xml":
                     {
                         if (projectTreeView.SelectedNode.FullPath.ToLower().StartsWith(Fusion.Editor.Editor.GlobalInstance.TilesetPath.ToLower()))
                         {
                             if (IOMethods.FileSize(projectTreeView.SelectedNode.FullPath.ToLower()) == 0)
                             {
                                 MessageBox.Show("This tileset configuration file is empty, unable to open tileset viewer.", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                 return;
                             }
                             TilesetWindow window = new TilesetWindow(projectTreeView.SelectedNode.FullPath);
                             window.SelectionChanged += new EventHandler(tilesetWindow_TilesetSelectionChanged);
                             window.ColorChanged += new EventHandler(tilesetWindow_TilesetColorChanged);
                             window.Show(this.Parent);
                         }
                         else
                         {
                             ScriptEditorWindow window = new ScriptEditorWindow(projectTreeView.SelectedNode.FullPath);
                             window.IsPlainFile = true;
                             window.Show(this);
                         }
                         break;
                     }

                case ".png":
                case ".bmp":
                case ".tga":
                     {
                         ImageWindow window = new ImageWindow(projectTreeView.SelectedNode.FullPath);
                         window.Show(this.Parent);
                         break;
                     }

                default:
                     Process process = new Process();
                     process.StartInfo.FileName = @"explorer";
                     process.StartInfo.Arguments = projectTreeView.SelectedNode.FullPath;
                     process.StartInfo.Verb = "Open";
                     process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                     process.Start();
                     break;
            }           
        }

        /// <summary>
        ///		Invoked when a script editor is closed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void ScriptEditorClosed(object sender, FormClosingEventArgs e)
        {
            if (ScriptModified != null) ScriptModified(sender, new EventArgs());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tilesetWindow_TilesetSelectionChanged(object sender, EventArgs e)
        {
            _selectedTileset = ((TilesetWindow)sender).Tileset.Url.ToString();
            _tilesetSelection = ((TilesetWindow)sender).Selection;
            _tilesetColor = ((TilesetWindow)sender).Color;
            if (TilesetSelectionChanged != null) TilesetSelectionChanged(this, new EventArgs());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tilesetWindow_TilesetColorChanged(object sender, EventArgs e)
        {
            _tilesetColor = ((TilesetWindow)sender).Color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = "Untitled";
            int index = 0;
            while (File.Exists(projectTreeView.SelectedNode.FullPath + "\\" + fileName))
            {
                fileName = "Untitled " + index;
                index++;
            }

            TreeNode folderNode = projectTreeView.SelectedNode;
            if (Directory.Exists(projectTreeView.SelectedNode.FullPath) == false) folderNode = projectTreeView.SelectedNode.Parent;

            File.Create(((folderNode != null) ? folderNode.FullPath + "\\" : "") + fileName);

            TreeNode node = new TreeNode("Untitled");
            node.ImageIndex = 6;
            node.SelectedImageIndex = 6;
            if (folderNode == null)
                projectTreeView.Nodes.Add(node);
            else
            {
                folderNode.Expand();
                folderNode.Nodes.Add(node);
            }
            node.BeginEdit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = "Untitled";
            int index = 0;
            while (File.Exists(projectTreeView.SelectedNode.FullPath + "\\" + fileName))
            {
                fileName = "Untitled " + index;
                index++;
            }

            TreeNode folderNode = projectTreeView.SelectedNode;
            if (Directory.Exists(projectTreeView.SelectedNode.FullPath) == false) folderNode = projectTreeView.SelectedNode.Parent;

            Directory.CreateDirectory(((folderNode != null) ? folderNode.FullPath + "\\" : "") + fileName);

            TreeNode node = new TreeNode("Untitled");
            if (folderNode == null)
                projectTreeView.Nodes.Add(node);
            else
            {
                folderNode.Expand();
                folderNode.Nodes.Add(node);
            }
            node.BeginEdit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(projectTreeView.SelectedNode.FullPath))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "All Files|*.*";
                dialog.Title = "Export File ...";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                    File.Copy(projectTreeView.SelectedNode.FullPath, dialog.FileName);
            }
            else if (Directory.Exists(projectTreeView.SelectedNode.FullPath))
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.ShowNewFolderButton = true;
                dialog.Description = "Select folder to export to.";
                if (dialog.ShowDialog() == DialogResult.OK)
                    IOMethods.CopyDirectory(projectTreeView.SelectedNode.FullPath, dialog.SelectedPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode folderNode = projectTreeView.SelectedNode;
            if (Directory.Exists(projectTreeView.SelectedNode.FullPath) == false) folderNode = projectTreeView.SelectedNode.Parent;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Files|*.*";
            dialog.Title = "Import File ...";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(dialog.FileName, ((folderNode != null) ? folderNode.FullPath + "\\" : "") + Path.GetFileName(dialog.FileName));

                TreeNode node = new TreeNode(Path.GetFileName(dialog.FileName));
                node.ImageIndex = ExtensionToImageIndex(Path.GetExtension(dialog.FileName).ToLower());
                node.SelectedImageIndex = node.ImageIndex;
                if (folderNode == null)
                    projectTreeView.Nodes.Add(node);
                else
                {
                    folderNode.Expand();
                    folderNode.Nodes.Add(node);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(projectTreeView.SelectedNode.FullPath))
                File.Delete(projectTreeView.SelectedNode.FullPath);
            else if (Directory.Exists(projectTreeView.SelectedNode.FullPath))
                Directory.Delete(projectTreeView.SelectedNode.FullPath, true);
            projectTreeView.SelectedNode.Remove();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            projectTreeView.SelectedNode.BeginEdit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = "Copy Of "+projectTreeView.SelectedNode.FullPath;
            int index = 0;
            while (File.Exists(projectTreeView.SelectedNode.FullPath + "\\" + fileName))
            {
                fileName = "Copy Of " + projectTreeView.SelectedNode.FullPath+" "+index;
                index++;
            }
            File.Copy(projectTreeView.SelectedNode.FullPath, fileName);

            TreeNode folderNode = projectTreeView.SelectedNode;
            if (Directory.Exists(projectTreeView.SelectedNode.FullPath) == false)
                folderNode = projectTreeView.SelectedNode.Parent;

            TreeNode node = new TreeNode(Path.GetFileName(fileName));
            node.SelectedImageIndex = projectTreeView.SelectedNode.SelectedImageIndex;
            node.ImageIndex = projectTreeView.SelectedNode.ImageIndex;
            if (folderNode == null)
                projectTreeView.Nodes.Add(node);
            else
            {
                folderNode.Expand();
                folderNode.Nodes.Add(node);
            }
        }

		#endregion
	}
}