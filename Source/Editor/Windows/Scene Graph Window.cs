/*
 * File: Scene Graph Window.cs
 *
 * Contains all the functional partial code declaration for the SceneGraphWindow form.
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
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine;

namespace BinaryPhoenix.Fusion.Editor.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the scene graph window.
	/// </summary>
	public partial class SceneGraphWindow : Form
	{
		#region Members
		#region Variables

		private SceneGraph _sceneGraph = null;
		private Hashtable _nodeHashTable = new Hashtable();
		
		#endregion
		#region Properties

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this window and fills it with the given scene graph.
		/// </summary>
		/// <param name="graph">Scene graph to fill window with.</param>
		public SceneGraphWindow(SceneGraph graph)
		{
			InitializeComponent();
			_sceneGraph = graph;
			SyncronizeData();
		}

		/// <summary>
		///		Syncronizes the data shown on this window.
		///		This is made public so that the Editor Window that owns this window can 
		///		update this window whenever the scene graph is altered dramatically.
		/// </summary>
		public void SyncronizeData()
		{
			// Clear out the old nodes.
			sceneGraphTreeView.Nodes.Clear();

			// Fill the scene graph tree view with the given
			// scene graphs nodes.
			TreeNode treeNode = new TreeNode("Root Camera");
			sceneGraphTreeView.Nodes.Add(treeNode);
			_nodeHashTable.Clear();
			FillSceneGraph(Editor.GlobalInstance.Map.SceneGraph.RootNode, treeNode.Nodes);
			sceneGraphTreeView.ExpandAll();
			
			SyncronizeButtonState();
		}

		/// <summary>
		///		Syncronizes the enabled state of the buttons on this window depending
		///		on what node is selected.
		/// </summary>
		private void SyncronizeButtonState()
		{
			// Enable the buttons if they can be used.
			if (sceneGraphTreeView.SelectedNode != null && _nodeHashTable.Contains(sceneGraphTreeView.SelectedNode) == true && (SceneNode)_nodeHashTable[sceneGraphTreeView.SelectedNode] != Editor.GlobalInstance.CameraNode)
			{
				SceneNode node = (SceneNode)_nodeHashTable[sceneGraphTreeView.SelectedNode];
				deleteButton.Enabled = duplicateButton.Enabled = true;
				upButton.Enabled = node.Parent.Children.Count > 0 && node.Parent.Children[0] != node;
                downButton.Enabled = node.Parent.Children.Count > 0 && node.Parent.Children[node.Parent.Children.Count - 1] != node;
				leftButton.Enabled = node.Parent != Editor.GlobalInstance.CameraNode;
				rightButton.Enabled = upButton.Enabled == true;
			}
			else
			{
				deleteButton.Enabled = duplicateButton.Enabled = upButton.Enabled = downButton.Enabled = leftButton.Enabled = rightButton.Enabled = false;
			}
		}

		/// <summary>
		///		Stops the window from closing, but instead make it hide itself.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void SceneGraphWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

		/// <summary>
		///		Removes the selected node from the scene graph and deletes it.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void deleteButton_Click(object sender, EventArgs e)
		{
			SceneNode node = ((SceneNode)_nodeHashTable[sceneGraphTreeView.SelectedNode]);
			node.Parent.RemoveChild(node);
			SyncronizeData();
		}

		/// <summary>
		///		Clones the selected node, and adds it to the scene graph.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void duplicateButton_Click(object sender, EventArgs e)
		{
			SceneNode node = ((SceneNode)_nodeHashTable[sceneGraphTreeView.SelectedNode]);
			node.Clone();
			SyncronizeData();
		}

		/// <summary>
		///		Moves the node up in the rendering list of its parent.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void upButton_Click(object sender, EventArgs e)
		{
			SceneNode node = ((SceneNode)_nodeHashTable[sceneGraphTreeView.SelectedNode]);
			node.Parent.ShiftChildBackwards(node);
			SyncronizeData();
		}

		/// <summary>	
		///		Moves the node down in the rendering list of its parent.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void downButton_Click(object sender, EventArgs e)
		{
			SceneNode node = ((SceneNode)_nodeHashTable[sceneGraphTreeView.SelectedNode]);
			node.Parent.ShiftChildForewards(node);
			SyncronizeData();
		}

		/// <summary>
		///		Moves the node to the left (ie. parent = nodeAboveThis) on the scene graph.
		/// </summary>	
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void leftButton_Click(object sender, EventArgs e)
		{
			SceneNode node = ((SceneNode)_nodeHashTable[sceneGraphTreeView.SelectedNode]);
			SceneNode parent = node.Parent;
			parent.RemoveChild(node);
			parent.Parent.AddChild(node);
			SyncronizeData();
		}

		/// <summary>
		///		Moves the node to the right (ie. parent = parent.parent) on the 
		///		scene graph.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void rightButton_Click(object sender, EventArgs e)
		{
			SceneNode node = ((SceneNode)_nodeHashTable[sceneGraphTreeView.SelectedNode]);
			SceneNode parent = node.Parent;
			int index = parent.Children.IndexOf(node);
			SceneNode newParent = (SceneNode)parent.Children[index - 1];
			parent.RemoveChild(node);
			newParent.AddChild(node);
			SyncronizeData();
		}

		/// <summary>
		///		Refreshs the enabled state of the buttons on the window when a 
		///		new node is selected.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void sceneGraphTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			SyncronizeButtonState();
		}

		/// <summary>
		///		Fills the scene graph tree view with the given scene node and its child nodes.
		/// </summary>
		/// <param name="node">Node to add to scene graph.</param>
		/// <param name="collection">Collection to add node to.</param>
		private void FillSceneGraph(SceneNode node, TreeNodeCollection collection)
		{
			foreach (SceneNode subNode in node.Children)
			{
				TreeNode treeNode = new TreeNode((subNode.Name == null ? "Untitled" : subNode.Name) + "("+subNode.Transformation.X+","+subNode.Transformation.Y+","+subNode.Transformation.Z+")");
				collection.Add(treeNode);
				_nodeHashTable.Add(treeNode, subNode);
				FillSceneGraph(subNode, treeNode.Nodes);
			}
		}

		#endregion
	}
}