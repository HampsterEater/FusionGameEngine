/*
 * File: Project Build Window.cs
 *
 * Contains all the functional partial code declaration for the ProjectBuildWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.IO;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Editor;
using BinaryPhoenix.Fusion.Engine.Windows;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Editor.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the project build window.
	/// </summary>
	public partial class ProjectBuildWindow : Form
	{
		#region Members
		#region Variables

		private string _buildDirectory = Editor.GlobalInstance.BuildPath;
		private bool _copySaves = true;
        private bool _buildStandAlone = false;
        private bool _buildFGE = false;
        private bool _compressPakFiles = false;

		private bool _compilePakFiles = Editor.GlobalInstance.GameConfigFile["resources:usepakfiles", "1"] == "1" ? true : false;
		private int _pakFileMaximumSize = int.Parse(Editor.GlobalInstance.GameConfigFile["resources:maximumpakfilesize", "250"]);
		private string _pakFilePrefix = Editor.GlobalInstance.GameConfigFile["resources:pakfileprefix", ".pk"];

		private bool _compileScripts = Editor.GlobalInstance.GameConfigFile["resources:scripts:compilescripts", "1"] == "1" ? true : false;
#if DEBUG
		private string _scriptDefines = Editor.GlobalInstance.GameConfigFile["resources:scripts:scriptdefines", ""];
		private bool _compileInDebugMode = Editor.GlobalInstance.GameConfigFile["resources:scripts:compileindebugmode", "1"] == "1" ? true : false;
#else
		private string _scriptDefines = Editor.GlobalInstance.GameConfigFile["resources:scripts:scriptdefines", ""] != "" ? "RELEASE," + Editor.GlobalInstance.GameConfigFile["resources:scripts:scriptdefines", ""] : "RELEASE";
		private bool _compileInDebugMode = Editor.GlobalInstance.GameConfigFile["resources:scripts:compileindebugmode", "1"] == "1" ? true : false;
#endif
		private bool _treatWarningsAsErrors = Editor.GlobalInstance.GameConfigFile["resources:scripts:treatwarningsaserrors", "1"] == "1" ? true : false;
		private bool _treatMessagesAsErrors = Editor.GlobalInstance.GameConfigFile["resources:scripts:treatmessagesaserrors", "1"] == "1" ? true : false;

		private bool _keepScriptSource = Editor.GlobalInstance.GameConfigFile["resources:scripts:keepscriptsource", "1"] == "1" ? true : false;

		private BuildProjectLogWindow _progressWindow = null;

		private PakFile _pakFile = null;
		private int _pakFileIndex = 0, _pakFileSize = 0;
		private string _buildBasePath;

		private CompileFlags _compileFlags = 0;
		private Define[] _scriptDefineList = null;
		private string[] _scriptIncludePathList = null;

		private TreeNode[] _nodes = new TreeNode[3];

		#endregion
		#region Properties

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class, and sets up the form.
		/// </summary>
		public ProjectBuildWindow()
		{
			// Initialize the windows form controls.
			InitializeComponent();
			SyncProperties();

			// Create the navigation nodes and add them to the navigation tree.
			_nodes[0] = new System.Windows.Forms.TreeNode("General");
			_nodes[1] = new System.Windows.Forms.TreeNode("Pak Files");
			_nodes[2] = new System.Windows.Forms.TreeNode("Scripts");
			optionTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { _nodes[0], _nodes[1], _nodes[2] });
		}

		/// <summary>
		///		Syncronizes the properties displayed on this window to those stored in this instance.
		/// </summary>
		public void SyncProperties()
		{
			buildDirTextBox.Text = _buildDirectory;
			copySaveDirCheckBox.Checked = _copySaves;
			compilePakFilesCheckBox.Checked = _compilePakFiles;
			pakFilePanel.Enabled = _compilePakFiles;
			maxFileSizeNumericUpDown.Value = (decimal)_pakFileMaximumSize;
			prefixTextBox.Text = _pakFilePrefix;
			compileScriptsCheckBox.Checked = _compileScripts;
			compileScriptsPanel.Enabled = _compileScripts;
			defineTextBox.Text = _scriptDefines;
			keepSourceCheckBox.Checked = _keepScriptSource;
			compileInDebugCheckBox.Checked = _compileInDebugMode;
			treatWarningsAsErrorsCheckBox.Checked = _treatWarningsAsErrors;
			treatMessagesAsErrorsCheckBox.Checked = _treatMessagesAsErrors;
            compressPakFileCheckBox.Checked = _compressPakFiles;
            standAloneCheckBox.Checked = _buildStandAlone;
            fgeDistributableCheckBox.Checked = _buildFGE;
		}

		/// <summary>
		///		Called when the users clicks a navigation node. Simply switchs to
		///		the correct option page.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void optionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			for (int i = 0; i < _nodes.Length; i++)
				if (optionTreeView.SelectedNode == _nodes[i])
					stepTabControl.SelectTab(i);
		}

		/// <summary>
		///		Closes the window if the cancel button is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		///		Starts to build the given project if the build button is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void okButton_Click(object sender, EventArgs e)
		{
			// Build the current project.
			BuildProject();
		}

		/// <summary>
		///		Prompts the user to select a directory when they click the browse
		///		button next to the build directory text box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void buildDirButton_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.SelectedPath = Editor.GlobalInstance.GamePath;
			dialog.Description = "Select directory to place finished builds in.";
			if (dialog.ShowDialog() == DialogResult.Cancel) return;
			if (dialog.SelectedPath.ToLower().StartsWith(Editor.GlobalInstance.GamePath.ToLower()) == false)
			{
				MessageBox.Show("Build directory must be a sub directory of the game directory.", "Relative Path Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			buildDirTextBox.Text = dialog.SelectedPath.Substring(Editor.GlobalInstance.GamePath.Length + 1);
		}

		/// <summary>
		///		Gathers infomation from the build directory text box when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void buildDirTextBox_TextChanged(object sender, EventArgs e)
		{
			_buildDirectory = buildDirTextBox.Text;
		}

		/// <summary>
		///		Gathers infomation from the copy saves check box when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void copySaveDirCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_copySaves = copySaveDirCheckBox.Checked;
		}

		/// <summary>
		///		Gathers infomation from the compile pak files check box when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void compilePakFilesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_compilePakFiles = compilePakFilesCheckBox.Checked;
			pakFilePanel.Enabled = compilePakFilesCheckBox.Checked;
            standAloneCheckBox.Checked = !_compilePakFiles;
		}

		/// <summary>
		///		Gathers infomation from the max pak file size numeric up/down when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void maxFileSizeNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			_pakFileMaximumSize = (int)maxFileSizeNumericUpDown.Value;
		}

		/// <summary>
		///		Gathers infomation from the prefix text box when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void prefixTextBox_TextChanged(object sender, EventArgs e)
		{
			_pakFilePrefix = prefixTextBox.Text;
		}

		/// <summary>
		///		Gathers infomation from the compile scripts check box when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void compileScriptsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_compileScripts = compileScriptsCheckBox.Checked;
			compileScriptsPanel.Enabled = compileScriptsCheckBox.Checked;
		}

		/// <summary>
		///		Gathers infomation from the script defines text box when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void defineTextBox_TextChanged(object sender, EventArgs e)
		{
			_scriptDefines = defineTextBox.Text;
		}

		/// <summary>
		///		Gathers infomation from the keep source check box when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void keepSourceCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_keepScriptSource = keepSourceCheckBox.Checked;
		}

		/// <summary>
		///		Gathers infomation from the compile in debug check box when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void compileInDebugCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_compileInDebugMode = compileInDebugCheckBox.Checked;
		}

		/// <summary>
		///		Gathers infomation from the treat warnings as errors check box when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void treatWarningsAsErrorsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_treatWarningsAsErrors = treatWarningsAsErrorsCheckBox.Checked;
		}

		/// <summary>
		///		Gathers infomation from the treat messages as errors check box when its value is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void treatMessagesAsErrorsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_treatMessagesAsErrors = treatMessagesAsErrorsCheckBox.Checked;
		}

        /// <summary>
        ///		Gathers infomation from the stand alone check box when its value is changed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void standAloneCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _buildStandAlone = standAloneCheckBox.Checked;
            compilePakFilesCheckBox.Checked = !_buildStandAlone;
            fgeDistributableCheckBox.Checked = !_buildStandAlone;
        }

        /// <summary>
        ///		Gathers infomation from the FGE distrabutable check box when its value is changed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void fgeDistributableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _buildFGE = fgeDistributableCheckBox.Checked;
            standAloneCheckBox.Checked = !_buildFGE;
        }

        /// <summary>
        ///		Gathers infomation from the compress pak files check box when its value is changed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void compressPakFileCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _compressPakFiles = compressPakFileCheckBox.Checked;
        }

        /// <summary>
		///		Builds the current project to the build directory specified 
		///		in the game configuration file.
		/// </summary>
        string _subTask = "", _task = "";
        int _subTaskProgress, _taskProgress;
        Stack _logStack = new Stack();
        private void BuildProject()
        {
            // Create a progress window and disable the current window.
            this.Enabled = false;
            _progressWindow = new BuildProjectLogWindow("Building project", "Copying system files");
            _progressWindow.Show(this);

            Thread thread = new Thread(new ThreadStart(BuildProjectThread));
            thread.IsBackground = true;
            thread.Start();

            while (thread != null && thread.IsAlive == true)
            {
                Application.DoEvents();

                if (_subTask != "")
                {
                    _progressWindow.UpdateSubTaskProgress(_subTaskProgress, _subTask);
                    _subTask = "";
                }

                if (_task != "")
                {
                    _progressWindow.UpdateTaskProgress(_taskProgress, _task);
                    _task = "";
                }

                if (_logStack.Count > 0)
                    _progressWindow.AddLog((string)_logStack.Pop());
            }

            // Enable current window and destroy the progress window.
            _progressWindow.Close();
            this.Enabled = true;
        }

		/// <summary>
		///		Used as an entry point for the project building thread.
		/// </summary>
		private void BuildProjectThread()
		{
			// Work out script compiling options
			_compileFlags = _compileInDebugMode == true ? CompileFlags.Debug : 0;
			if (_treatMessagesAsErrors) _compileFlags |= CompileFlags.TreatMessagesAsErrors;
			if (_treatWarningsAsErrors) _compileFlags |= CompileFlags.TreatWarningsAsErrors;

			// Split up the script defines string.
			string[] defines = _scriptDefines.Split(new char[1] { ',' });
			_scriptDefineList = new Define[defines.Length + 1];
			for (int i = 0; i < defines.Length; i++)
			{
				if (defines[i].IndexOf('=') >= 0)
				{
					string[] splitList = defines[i].Split(new char[1] { '=' });
					_scriptDefineList[i] = new Define(splitList[0], splitList[1], TokenID.TypeIdentifier);
				}
				else
					_scriptDefineList[i] = new Define(defines[i], "", TokenID.TypeIdentifier);
			}
			_scriptDefineList[_scriptDefineList.Length - 1] = new Define(_compileInDebugMode ? "__DEBUG__" : "__RELEASE__", "", TokenID.TypeBoolean);

            // Set the include path list to include the script library path.
            _scriptIncludePathList = new string[] { Environment.CurrentDirectory + "\\" + Editor.GlobalInstance.ScriptLibraryPath, Editor.GlobalInstance.GlobalScriptLibraryPath };

			// Work out a directory that we can build to.
			_buildBasePath = _buildDirectory;
            _buildDirectory = _buildBasePath + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute+"-"+DateTime.Now.Second;
			int buildIndex = 2;

			if (Directory.Exists(_buildBasePath) == false) 
				Directory.CreateDirectory(_buildBasePath);

			while (Directory.Exists(_buildDirectory) == true)
                _buildDirectory = _buildBasePath + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute +"-"+ DateTime.Now.Second + (buildIndex++).ToString().PadLeft(4, '0');

			// Create the build directory.
			Directory.CreateDirectory(_buildDirectory);

			// Open the game configuration file, change the pak file settings and save it into
			// the new build folder.
			XmlConfigFile configFile = new XmlConfigFile(Editor.GlobalInstance.GameName + ".xml");
			configFile.SetSetting("path:resources:usepakfiles", _compilePakFiles == true || _buildStandAlone == true ? "1" : "0");
            if (_buildStandAlone == false)
			    configFile.Save(_buildDirectory + "\\" + Editor.GlobalInstance.GameName + ".xml");

			// Copy the configuration directory into the build directory.
			//IOMethods.CopyDirectory(Editor.GlobalInstance.ConfigPath, _buildDirectory + "\\" + Editor.GlobalInstance.ConfigPath);

			// Copy the language directory into the build directory.
			//IOMethods.CopyDirectory(Editor.GlobalInstance.LanguagePath, _buildDirectory + "\\" + Editor.GlobalInstance.LanguagePath);

			// Copy the saves file if we have been told to.
            if (_copySaves == true && Directory.GetFiles(Editor.GlobalInstance.SavePath).Length != 0)
				IOMethods.CopyDirectory(Editor.GlobalInstance.SavePath, _buildDirectory + "\\" + Editor.GlobalInstance.SavePath);

            // Create a plugins folder.
            //IOMethods.CopyDirectory(Editor.GlobalInstance.GamePluginPath, _buildDirectory + "\\" + Editor.GlobalInstance.GamePluginPath);

            // Copy the icon.
            if (_buildStandAlone == false && File.Exists(Editor.GlobalInstance.GamePath + "\\icon.ico"))
                File.Copy(Editor.GlobalInstance.GamePath + "\\icon.ico", _buildDirectory + "\\icon.ico");

			// Compile the pak files or copy the media directory if we are not using pak files.
            if (_buildStandAlone == true)
            {
                _taskProgress = 0;
                _task = "Packing files";
                _logStack.Push(_task);

                // Work out the game ID code.
                long gameIDCode = DateTime.Now.Ticks ^ (long)configFile["title", "engine"].GetHashCode();

                // Create the pak file to save resources to.
                _pakFile = new PakFile();
                _pakFileIndex = 0;
                _pakFileSize = 0;
                _pakFileMaximumSize = 0;

                // Compile the media directory to the pak file.
                CompileDirectoryToPak(Editor.GlobalInstance.MediaPath);
                CompileDirectoryToPak(Editor.GlobalInstance.ConfigPath);
                CompileDirectoryToPak(Editor.GlobalInstance.LanguagePath);

                // Work out game files that we need.
                ArrayList gameFiles = new ArrayList();
                gameFiles.Add(AppDomain.CurrentDomain.BaseDirectory + "fusion.exe");
                gameFiles.Add(AppDomain.CurrentDomain.BaseDirectory + "graphics.dll");
                gameFiles.Add(AppDomain.CurrentDomain.BaseDirectory + "runtime.dll");
                gameFiles.Add(AppDomain.CurrentDomain.BaseDirectory + "input.dll");
                gameFiles.Add(AppDomain.CurrentDomain.BaseDirectory + "audio.dll");
                gameFiles.Add(AppDomain.CurrentDomain.BaseDirectory + "engine.dll");

                // Copy plugins.
                string[] requiredPlugins = Editor.GlobalInstance.GameConfigFile.GetSettings("requirements");
                if (requiredPlugins != null)
                {
                    foreach (string requirement in requiredPlugins)
                    {
                        string[] pathSplit = requirement.Split(new char[] { ':' });
                        string name = pathSplit[pathSplit.Length - 1];
                        string value = Editor.GlobalInstance.GameConfigFile[requirement, ""];
                        switch (name.ToLower())
                        {
                            case "plugin":
                                if (File.Exists(Editor.GlobalInstance.PluginPath + "\\" + value))
                                {
                                    //if (Directory.Exists(_buildDirectory + "\\" + Editor.GlobalInstance.PluginPath) == false)
                                    //    Directory.CreateDirectory(_buildDirectory + "\\" + Editor.GlobalInstance.PluginPath);
                                    //File.Copy(Editor.GlobalInstance.PluginPath + "\\" + value, _buildDirectory + "\\" + Editor.GlobalInstance.PluginPath + value);
                                    gameFiles.Add(AppDomain.CurrentDomain.BaseDirectory + Editor.GlobalInstance.PluginPath + "\\" + value);
                                }
                                break;
                        }
                    }
                }

                // Copy all!
                else
                {
                    if (Directory.Exists(Editor.GlobalInstance.PluginPath))
                    {
                        foreach (string file in Directory.GetFiles(Editor.GlobalInstance.PluginPath))
                        {
                            if (Path.GetExtension(file).ToLower() != ".dll") continue;
                            gameFiles.Add(file);
                        }
                    }

                    // Game plugins as well.
                    // TODO
                }

                _taskProgress = 50;
                _task = "Building executable";

                // Create the sub that we are going to add the files into.
                string stubFile = _buildDirectory + "\\" + string.Join("", configFile["title", "engine"].Split(new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' }, StringSplitOptions.None)) + ".exe";
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Stand Alone Stub.exe", stubFile);

                // Open up the stub.
                Stream stubStream = StreamFactory.RequestStream(stubFile, StreamMode.Append);
                BinaryWriter stubWriter = new BinaryWriter(stubStream);

                // Grab the offset.
                long offset = stubStream.Position;
                stubWriter.Write(gameFiles.Count + 3);
                foreach (string gameFilePath in gameFiles)
                {
                    Stream fileStream = null;
                    fileStream = StreamFactory.RequestStream(gameFilePath, StreamMode.Open);
                    if (fileStream == null)
                    {
                        string tmpFile = Path.GetTempFileName();
                        File.Delete(tmpFile);
                        File.Copy(gameFilePath, tmpFile);
                        fileStream = StreamFactory.RequestStream(tmpFile, StreamMode.Open);
                    }
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, (int)fileStream.Length);

                    string url = gameFilePath;
                    if (url.ToLower().StartsWith(AppDomain.CurrentDomain.BaseDirectory.ToLower()))
                        url = url.Substring(AppDomain.CurrentDomain.BaseDirectory.Length);

                    stubWriter.Write(url);
                    stubWriter.Write(buffer.Length);
                    stubWriter.Write(buffer, 0, buffer.Length);
                }

                stubStream.Flush();
                GC.Collect();

                // Write the pak file into a memory stream.
                MemoryStream engineMemStream = new MemoryStream();
                BinaryWriter engineMemWriter = new BinaryWriter(engineMemStream);

                XmlConfigFile engineConfigFile = new XmlConfigFile(AppDomain.CurrentDomain.BaseDirectory + "fusion.xml");
                engineConfigFile.SetSetting("standalone", _buildStandAlone == true ? "1" : "0");
                engineConfigFile.Save(engineMemWriter);

                GC.Collect();

                // Write the game config file.
                stubWriter.Write("fusion.xml");
                stubWriter.Write((int)engineMemStream.Length);
                engineMemStream.WriteTo(stubStream);



                // Write the pak file into a memory stream.
                MemoryStream gameMemStream = new MemoryStream();
                BinaryWriter gameMemWriter = new BinaryWriter(gameMemStream);
                configFile.Save(gameMemWriter);
                GC.Collect();

                // Write the game config file.
                stubWriter.Write("standalone.xml");
                stubWriter.Write((int)gameMemStream.Length);
                gameMemStream.WriteTo(stubStream);



                // Write the pak file into a memory stream.
                MemoryStream memStream = new MemoryStream();
                BinaryWriter memWriter = new BinaryWriter(memStream);
                _pakFile.Save(memWriter);
                _pakFile.Resources.Clear();
                GC.Collect();

                // Write in the pak file.
                stubWriter.Write("data.pk");
                stubWriter.Write((int)memStream.Length);
                memStream.WriteTo(stubStream);



                // Write in the offset footer
                stubWriter.Write(gameIDCode);
                stubWriter.Write(offset);

                // Write the data into the stub.
                stubWriter.Close();
                stubStream.Close();
            }
            else
            {
                if (_compilePakFiles == true)
                {
                    _taskProgress = 0;
                    _task = "Compiling pak files";
                    _logStack.Push(_task);

                    // Create the pak file to save resources to.
                    _pakFile = new PakFile();
                    _pakFileIndex = 0;
                    _pakFileSize = 0;

                    // Compile the media directory to the pak file.
                    CompileDirectoryToPak(Editor.GlobalInstance.MediaPath);
                    CompileDirectoryToPak(Editor.GlobalInstance.ConfigPath);
                    CompileDirectoryToPak(Editor.GlobalInstance.LanguagePath);

                    // Save the pak file to the hard drive.
                    if (_pakFile.Resources.Count != 0)
                    {
                        string filePath = _buildDirectory + "\\" + _pakFilePrefix.Replace("#", _pakFileIndex.ToString()) + ".pk";
                        int index = 0;
                        while (File.Exists(filePath) == true)
                            filePath = _buildDirectory + "\\" + _pakFilePrefix.Replace("#", _pakFileIndex.ToString()) + (index++) + ".pk";
                        _pakFile.Save(filePath);
                        _pakFile.Dispose();
                    }
                }
                else
                {
                    _taskProgress = 0;
                    _task = "Copying media";
                    _logStack.Push(_task);
                    CopyDirectoryWithProgress(Editor.GlobalInstance.MediaPath, _buildDirectory + "\\" + Editor.GlobalInstance.MediaPath);
                    CopyDirectoryWithProgress(Editor.GlobalInstance.ConfigPath, _buildDirectory + "\\" + Editor.GlobalInstance.ConfigPath);
                    CopyDirectoryWithProgress(Editor.GlobalInstance.LanguagePath, _buildDirectory + "\\" + Editor.GlobalInstance.LanguagePath);
                }

                // If we are building an FGE distrubutable file then copy the icon file and copile all to pak file.
                if (_buildFGE)
                {
                    _taskProgress = 0;
                    _task = "Compiling FGE files";
                    _logStack.Push(_task);

                    // Create the pak file to save resources to.
                    _pakFile = new PakFile();
                    _pakFileIndex = 0;
                    _pakFileSize = 0;
                    _pakFileMaximumSize = 0;

                    // Pak the build directory.
                    string oldDirectory = Directory.GetCurrentDirectory();

                    Directory.SetCurrentDirectory(_buildDirectory);
                    CompileDirectoryToPak(Directory.GetCurrentDirectory());

                    // Save the pak file.
                    _pakFile.Save(Editor.GlobalInstance.GameName + ".pk");
                    _pakFile.Dispose();

                    Directory.SetCurrentDirectory(oldDirectory);

                    // Delete folder.
                    if (_copySaves == true && Directory.GetFiles(Editor.GlobalInstance.SavePath).Length != 0)
                        Directory.Delete(_buildDirectory + "\\" + Editor.GlobalInstance.SavePath);

                    // Delete all files and folders.
                    File.Delete(_buildDirectory + "\\" + Editor.GlobalInstance.GameName + ".xml");
                    if (Directory.Exists(_buildDirectory + "\\" + Editor.GlobalInstance.MediaPath))
                        Directory.Delete(_buildDirectory + "\\" + Editor.GlobalInstance.MediaPath, true);
                    if (Directory.Exists(_buildDirectory + "\\" + Editor.GlobalInstance.ConfigPath))
                        Directory.Delete(_buildDirectory + "\\" + Editor.GlobalInstance.ConfigPath, true);
                    if (Directory.Exists(_buildDirectory + "\\" + Editor.GlobalInstance.LanguagePath))
                        Directory.Delete(_buildDirectory + "\\" + Editor.GlobalInstance.LanguagePath, true);
                }
            }
		}

		/// <summary>
		///		Compiles the given directory to the current pak file.
		/// </summary>
		/// <param name="from">Directory to compile.</param>
		private void CompileDirectoryToPak(string from)
		{
			// Retrieve all the files in the directory to be copied.
			string[] files = Directory.GetFileSystemEntries(from);

            if (from != Directory.GetCurrentDirectory() && from.ToLower().StartsWith(Directory.GetCurrentDirectory().ToLower()))
                from = from.Substring(Directory.GetCurrentDirectory().Length + 1);

			// Update the progress bar.
            _subTaskProgress = 0;
            _subTask = "Paking directory: " + from;
            _logStack.Push(_subTask);
			DebugLogger.WriteLog("Paking directory \"" + from + "\"...");

			// Go through each sub directory and file and copy it.
			int index = 1;
			foreach (string subFile in files)
			{
                string file = subFile;

                if (file.ToLower().StartsWith(Directory.GetCurrentDirectory().ToLower()))
                    file = file.Substring(Directory.GetCurrentDirectory().Length + 1);

				if (Directory.Exists(file) == true)
					CompileDirectoryToPak(file);
				else
				{
					DebugLogger.WriteLog("Paking file \"" + file + "\"...");
                    _logStack.Push("Paking file \"" + file + "\"...");

					// Check if its a script file which would mean we have to compile it.
                    if (file.ToLower().EndsWith(".fso") == true || file.ToLower().EndsWith(".fs") == true)
					{
						// See if we should compile it or not.
                        if (_compileScripts == true)
                        {
                            // Create a compile and compile the script.
                            ScriptCompiler compiler = new ScriptCompiler();
                            DebugLogger.WriteLog("Compiling script \"" + file + "\"...");
                            _logStack.Push("Compiling script \"" + file + "\"...");

                            // If there are any errors when compiling the script yell
                            // at the user.
                            string errorMessage = "";
                            bool showErrorMessage = false;
                            if (compiler.Compile(file, _compileFlags, _scriptDefineList, _scriptIncludePathList) > 0)
                            {
                                errorMessage = compiler.ErrorList.Count + " errors occured while compiling the script \"" + file + "\" \n\n";
                                foreach (CompileError error in compiler.ErrorList)
                                {
                                    if (error.AlertLevel == ErrorAlertLevel.Error ||
                                        error.AlertLevel == ErrorAlertLevel.FatalError ||
                                        (error.AlertLevel == ErrorAlertLevel.Warning && _treatWarningsAsErrors == true) ||
                                        (error.AlertLevel == ErrorAlertLevel.Message && _treatMessagesAsErrors == true)
                                        ) showErrorMessage = true;
                                    errorMessage += error.ToString() + "\n";
                                }
                            }
                            if (showErrorMessage == true)
                            {
                                DebugLogger.WriteLog(errorMessage, LogAlertLevel.Error);
                                _logStack.Push(errorMessage);
                            }

                            // Dump the compiled byte code to a temporary folder and set its path
                            // as this files path.
                            if (showErrorMessage == false)
                            {
                                string tempFile = Path.GetTempFileName();
                                compiler.DumpExecutableFile(tempFile);
                                PakFile(tempFile, file);
                            }

                            // Copy the source code if we have been told to keep it.
                            if (_keepScriptSource == true)
                                PakFile(file, file + ".source");
                        }
                        else
                        {
                            PakFile(file, file);
                        }
					}

					// Check if its a library script file. 
                    else if (file.ToLower().EndsWith(".fsl") == true)
					{
						if (_keepScriptSource == true)
							PakFile(file, file);
					}

					// Its a normal file so pak it normally.
					else
						PakFile(file, file);
				}

                _subTaskProgress = (int)((100.0f / (float)files.Length) * index);
                _subTask = "Compiling directory: " + from;
			}
		}

		/// <summary>
		///		Adds the file at a given url to the currently compiling pak file.
		/// </summary>
		/// <param name="path">Url of file to add to pak file.</param>
		/// <param name="locator">Path that should be used by the pak file to locate the paked file in future.</param>
		private void PakFile(string path, string locator)
		{
			// Create a pak resource to represent this file and add it to the pak file.
			PakResource resource = new PakResource(_pakFile, locator, 0, 0);
			_pakFile.AddResource(resource);

			// Load the files data into a memory stream and attach it to the resource.
			Stream stream = StreamFactory.RequestStream(path, StreamMode.Open);

			// If the size of the data is over the maximum pak size then 
			// create a new one.
			if (_pakFileMaximumSize != 0 && _pakFileSize + stream.Length > ((_pakFileMaximumSize * 1024) * 1024) && _pakFile.Resources.Count > 1)
			{
				_pakFile.RemoveResource(resource);

				// Find a new pak file we can create without overwriting anything, and save to it.
				string filePath = _buildDirectory + "\\" + _pakFilePrefix.Replace("#", _pakFileIndex.ToString()) + ".pk";
				int fileIndex = 0;
				while (File.Exists(filePath) == true)
					filePath = _buildDirectory + "\\" + _pakFilePrefix.Replace("#", _pakFileIndex.ToString()) + (fileIndex++) + ".pk";
				_pakFile.Save(filePath);
				_pakFileIndex++;
				_pakFileSize = 0;

				// Create a new pak file to put the rest of the resources into.
				_pakFile = new PakFile();
				_pakFile.AddResource(resource);
			}

            resource.DataStream = stream;
            _pakFileSize += (int)stream.Length;
		}

		/// <summary>
		///		Copys a directory tree from one location to another, unlike the one in IOMethods this
		///		one will also increment a progress bar as it copys the directory tree.
		/// </summary>
		/// <param name="from">The source directory tree.</param>
		/// <param name="to">The destination path of the directory tree.</param>
		private void CopyDirectoryWithProgress(string from, string to)
		{
			// Append a directory seperator to the end of the path.
			if (to[to.Length - 1] != Path.DirectorySeparatorChar)
				to += Path.DirectorySeparatorChar;

			// If the destination directory does not exist then create it.
			if (Directory.Exists(to) == false) Directory.CreateDirectory(to);

			// Retrieve all the files in the directory to be copied.
			string[] files = Directory.GetFileSystemEntries(from);

			// Update the progress bar.
			DebugLogger.WriteLog("Copying directory \"" + from + "\"...");
            _subTaskProgress = 0;
            _subTask = "Copying directory: " + from;
            _logStack.Push(_subTask);

			// Go through each sub directory and file and copy it.
			int index = 1;
			foreach (string file in files)
			{
				if (Directory.Exists(file) == true)
					CopyDirectoryWithProgress(file, to + Path.GetFileName(file));
				else
				{
                    _logStack.Push("Copying file \"" + file + "\"...");

					// Check if its a script file which would mean we have to compile it.
                    if (file.ToLower().EndsWith(".fs") == true || file.ToLower().EndsWith(".fso") == true)
					{
						// See if we should compile it or not.
						if (_compileScripts == true)
						{
							// Create a compile and compile the script.
							ScriptCompiler compiler = new ScriptCompiler();
                            _logStack.Push("Compiling script \"" + file + "\"...");

							// If there are any errors when compiling the script yell
							// at the user.
							string errorMessage = "";
							bool showErrorMessage = false;
							if (compiler.Compile(file, _compileFlags, _scriptDefineList, _scriptIncludePathList) > 0)
							{
								errorMessage = compiler.ErrorList.Count + " errors occured while compiling the script \""+file+"\" \n\n";
								foreach (CompileError error in compiler.ErrorList)
								{
									if (error.AlertLevel == ErrorAlertLevel.Error ||
										error.AlertLevel == ErrorAlertLevel.FatalError ||
										(error.AlertLevel == ErrorAlertLevel.Warning && _treatWarningsAsErrors == true) ||
										(error.AlertLevel == ErrorAlertLevel.Message && _treatMessagesAsErrors == true)
										) showErrorMessage = true;
									errorMessage += error.ToString() + "\n";
								}
							}
							if (showErrorMessage == true)
							{
                                DebugLogger.WriteLog(errorMessage, LogAlertLevel.Error);
                                _logStack.Push(errorMessage);
							}
	
							// Dump the compiled byte code to the build directory.
							if (showErrorMessage == false)
								compiler.DumpExecutableFile(to + Path.GetFileName(file));
						}

						// Copy the source code if we have been told to keep it.
						if (_keepScriptSource == true)
							File.Copy(file, to + Path.GetFileName(file), true);
					}

					// Check if its a script script file. 
					else if (file.ToLower().EndsWith(".fsl") == true)
					{
						if (_keepScriptSource == true)
							File.Copy(file, to + Path.GetFileName(file), true);
					}

					// Its a normal file so we can just ignore it.
					else
						File.Copy(file, to + Path.GetFileName(file), true);
				}

                _subTaskProgress = (int)(((float)index / (float)files.Length) * 100.0f);
                _subTask = "Copying directory: " + from;
			}
		}

		#endregion
    }

}