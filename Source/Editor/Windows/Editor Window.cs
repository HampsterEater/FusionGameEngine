/*
 * File: Editor Window.cs
 *
 * Contains all the functional partial code declaration for the EditorWindow form.
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
using System.Diagnostics;
using System.IO;
using System.Threading;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime.Controls;
using BinaryPhoenix.Fusion.Input;
using BinaryPhoenix.Fusion.Runtime.Events;
using BinaryPhoenix.Fusion.Engine;
using BinaryPhoenix.Fusion.Engine.Windows;
using BinaryPhoenix.Fusion.Engine.Entitys;

namespace BinaryPhoenix.Fusion.Editor.Windows
{

	/// <summary>
	///		This class contains the code used to display and operate  
	///		the main editor window.
	/// </summary>
	public partial class EditorWindow : Form
	{
		#region Members
		#region Variables

		private GraphicsCanvas _mapCanvas = null;

		private ConsoleWindow _consoleWindow = null;
		private AssetManagerWindow _assetManagerWindow = null;
		private SceneGraphWindow _sceneGraphWindow = null;
		private EntityPropertiesWindow _entityPropertiesWindow = null;
        private EmitterEditorWindow _effectsDesignerWindow = null;

		private bool _snapToGrid = true;
		private int _gridWidth = 8, _gridHeight = 8;

		private Tools _currentTool = Tools.Camera;

		private object _mapFileUrl = null;
		private bool _mapChangedSinceSave = false;

		private int[] mousePositionBeforeRightClick = new int[2];

		private ToolStripMenuItem[] _recentMenus = new ToolStripMenuItem[5];

		private EventListener _listener = null;

		private bool _toolMoving = false;
		private System.Drawing.Rectangle _toolMovementDimensions = new System.Drawing.Rectangle();
		private bool _toolActive = false;

		private bool _movingObject = false;
		private System.Drawing.Rectangle _moveObjectDimensions = new System.Drawing.Rectangle();

		private bool _sizingObject = false;
		private EntityNode _objectToSize = null;
		private SizingDirection _sizingDirection = 0;
		private Rectangle _objectSizeOriginalBoundingBox;
		private Transformation _objectSizeOriginalTransformation;

		private ArrayList _selectedEntityList = new ArrayList();

		private ArrayList _objectClipboard = new ArrayList();

		private bool _xAxisLocked = false, _yAxisLocked = false;

		private bool _flipTilesX = false, _flipTilesY = false;

		private bool _viewBoundingBoxs = false;
        private bool _viewCollisionBoxs = false;
        private bool _viewEventLines = true;
        private bool _viewOrigin = true;

		private bool _supressSelection = false;
		private HighPreformanceTimer _supressSelectionTimer = new HighPreformanceTimer();

        private Thread _mapThread = null;
        private string _mapUrl = "";
        private string _mapPassword = "";
        private bool _mapLoaded = false;

        private Stack _undoStack = new Stack();
        private Stack _redoStack = new Stack();
        private Hashtable _originalEntityTransformations = new Hashtable();
        private Hashtable _originalEntityBoundingBoxs = new Hashtable();

        private Graphics.Image _pathMarkerImage = null;

        private Graphics.Image _backgroundImage = null;

        private Tileset _selectedTileset = null;

		#endregion
		#region Properties

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class, and sets up the form.
		/// </summary>
		public EditorWindow()
		{
			// Initialize the windows form controls.
			InitializeComponent();

			// Refresh the recent file list.
			RefreshRecentFiles();

			// Create the map canvas to render to.
            _mapCanvas = GraphicsManager.CreateCanvas(mapPanel, 0, new CanvasRenderHandler(Render));

            // Grab the path marker image.
            _pathMarkerImage = new Graphics.Image(ReflectionMethods.GetEmbeddedResourceStream("path_marker.png"), 0);

			// Syncronize the window to the current data.
			SyncronizeWindow();

			// Create a new event listener to listen out for input events.
			_listener = new EventListener(new ProcessorDelegate(EventCaptured));
			EventManager.AttachListener(_listener);
		}

		/// <summary>
		///		Called when a toolbar is resized, its responsible for updating the viewport
		///		size of the camera.
		/// </summary>
		/// <param name="sender">Recent file menu that caused this event.</param>
		/// <param name="e">Arguments explaining why this event was invoked.</param>
		private void fileToolStrip_EndDrag(object sender, EventArgs e)
		{
			Editor.GlobalInstance.CameraNode.Viewport = new Rectangle(0, 0, mapPanel.ClientSize.Width, mapPanel.ClientSize.Height);
		}

		/// <summary>
		///		Called when a toolbar is resized, its responsible for updating the viewport
		///		size of the camera.
		/// </summary>
		/// <param name="sender">Recent file menu that caused this event.</param>
		/// <param name="e">Arguments explaining why this event was invoked.</param>
		private void rotateToolStrip_EndDrag(object sender, EventArgs e)
		{
			Editor.GlobalInstance.CameraNode.Viewport = new Rectangle(0, 0, mapPanel.ClientSize.Width, mapPanel.ClientSize.Height);
		}

		/// <summary>
		///		Called when a toolbar is resized, its responsible for updating the viewport
		///		size of the camera.
		/// </summary>
		/// <param name="sender">Recent file menu that caused this event.</param>
		/// <param name="e">Arguments explaining why this event was invoked.</param>
		private void palleteToolStrip_EndDrag(object sender, EventArgs e)
		{
			Editor.GlobalInstance.CameraNode.Viewport = new Rectangle(0, 0, mapPanel.ClientSize.Width, mapPanel.ClientSize.Height);
		}

		/// <summary>
		///		Called when a toolbar is resized, its responsible for updating the viewport
		///		size of the camera.
		/// </summary>
		/// <param name="sender">Recent file menu that caused this event.</param>
		/// <param name="e">Arguments explaining why this event was invoked.</param>
		private void commandToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			Editor.GlobalInstance.CameraNode.Viewport = new Rectangle(0, 0, mapPanel.ClientSize.Width, mapPanel.ClientSize.Height);
		}

		/// <summary>
		///		Syncronizes the state of all controls on this window relative to the 
		///		current state of the editor.
		/// </summary>
		public void SyncronizeWindow()
		{
			insertContextToolStripButton.Enabled = _assetManagerWindow != null && _assetManagerWindow.SelectedObjectURL != "" ? true : false;
			insertObjectToolStripMenuItem.Enabled = _assetManagerWindow != null && _assetManagerWindow.SelectedObjectURL != "" ? true : false;
			insertContextToolStripButton.Text = "Insert " + (_assetManagerWindow != null && _assetManagerWindow.SelectedObjectURL != "" ? Path.GetFileName(_assetManagerWindow.SelectedObjectURL) : "Object");
			insertObjectToolStripMenuItem.Text = "Insert " + (_assetManagerWindow != null && _assetManagerWindow.SelectedObjectURL != "" ? Path.GetFileName(_assetManagerWindow.SelectedObjectURL) : "Object");

			alignBottomToolStripButton.Enabled = _selectedEntityList.Count > 0;
			alignMiddleToolStripButton.Enabled = _selectedEntityList.Count > 0;
			alignLeftToolStripButton.Enabled = _selectedEntityList.Count > 0;
			alignCenterToolStripButton.Enabled = _selectedEntityList.Count > 0;
			alignRightToolStripButton.Enabled = _selectedEntityList.Count > 0;
			alignTopToolStripButton.Enabled = _selectedEntityList.Count > 0;

			bottomMenuItem.Enabled = _selectedEntityList.Count > 0;
			centerMenuItem.Enabled = _selectedEntityList.Count > 0;
			leftMenuItem.Enabled = _selectedEntityList.Count > 0;
			middleMenuItem.Enabled = _selectedEntityList.Count > 0;
			rightMenuItem.Enabled = _selectedEntityList.Count > 0;
			topMenuItem.Enabled = _selectedEntityList.Count > 0;
            bottomContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;
            centerContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;
            leftContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;
            middleContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;
            rightContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;
            topContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;

			sendToBackMenuItem.Enabled = _selectedEntityList.Count > 0;
			bringToFrontMenuItem.Enabled = _selectedEntityList.Count > 0;
			shiftBackwardsMenuItem.Enabled = _selectedEntityList.Count > 0;
			shiftForewardsMenuItem.Enabled = _selectedEntityList.Count > 0;
            sendToBackContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;
            bringToFrontContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;
            shiftBackwardsContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;
            shiftForewardsContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;

			sendToBackToolStripButton.Enabled = _selectedEntityList.Count > 0;
			bringToFrontToolStripButton.Enabled = _selectedEntityList.Count > 0;
			shiftBackwardsToolStripButton.Enabled = _selectedEntityList.Count > 0;
			shiftForewardsToolStripButton.Enabled = _selectedEntityList.Count > 0;

			groupMenuItem.Enabled = _selectedEntityList.Count > 1;
			groupToolStripButton.Enabled = _selectedEntityList.Count > 1;
			ungroupMenuItem.Enabled = _selectedEntityList.Count > 0;
			ungroupToolStripButton.Enabled = _selectedEntityList.Count > 0;
            groupContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 1;
            unGroupContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;

			flipHorizontalToolStripButton.Enabled = _selectedEntityList.Count > 0;
			flipVerticalToolStripButton.Enabled = _selectedEntityList.Count > 0;
			horizontalMenuItem.Enabled = _selectedEntityList.Count > 0;
			verticalMenuItem.Enabled = _selectedEntityList.Count > 0;
            horizontalContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;
            verticalContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;

			rotate90AntiClockwiseMenuItem.Enabled = _selectedEntityList.Count > 0;
			rotate90ClockwiseMenuItem.Enabled = _selectedEntityList.Count > 0;
			rotateAnticlockwiseToolStripButton.Enabled = _selectedEntityList.Count > 0;
			rotateClockwiseToolStripButton.Enabled = _selectedEntityList.Count > 0;
            rotate90ClockwiseContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;
            rotate90AntiClockwiseContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;

			cutContextToolStripButton.Enabled = _selectedEntityList.Count > 0;
			cutMenuItem.Enabled = _selectedEntityList.Count > 0;
			cutToolStripButton.Enabled = _selectedEntityList.Count > 0;

			copyContextToolStripButton.Enabled = _selectedEntityList.Count > 0;
			copyMenuItem.Enabled = _selectedEntityList.Count > 0;
			copyToolStripButton.Enabled = _selectedEntityList.Count > 0;

			pasteContextToolStripButton.Enabled = _objectClipboard.Count > 0;
			pasteMenuItem.Enabled = _objectClipboard.Count > 0;
			pasteToolStripButton.Enabled = _objectClipboard.Count > 0;

			deleteContextToolStripButton.Enabled = _selectedEntityList.Count > 0;
			deleteMenuItem.Enabled = _selectedEntityList.Count > 0;

			duplicateContextToolStripButton.Enabled = _selectedEntityList.Count > 0;
			duplicateMenuItem.Enabled = _selectedEntityList.Count > 0;

			deselectMenuItem.Enabled = _selectedEntityList.Count > 0;
            deselectContextToolStripMenuItem.Enabled = _selectedEntityList.Count > 0;

			undoMenuItem.Enabled = false;
			undoToolStripButton.Enabled = false;
            undoContextToolStripMenuItem.Enabled = false;

			redoMenuItem.Enabled = false;
			redoToolStripButton.Enabled = false;
            redoContextToolStripMenuItem.Enabled = false;

			propertyContextToolStripButton.Enabled = _selectedEntityList.Count != 0;
			propertyMenuItem.Enabled = _selectedEntityList.Count != 0;

            clearBackgroundToolStripMenuItem.Enabled = (_backgroundImage != null);

			// Check if we have selected a tilemap segment.
			bool tilemapSelected = false;
			foreach (EntityNode entity in _selectedEntityList)
				if (entity is TilemapSegmentNode) 
				{
					tilemapSelected = true;
					break;
				}

			bool tilemapSelectionSelected = tilemapSelected;
			if (_assetManagerWindow == null)
			{
				tilemapSelectionSelected = false;
				//tilemapSelected = false;
			}
			else if (_assetManagerWindow.TilesetSelection.Width == 0 || _assetManagerWindow.TilesetSelection.Height == 0)
				tilemapSelectionSelected = false;

			pencilMenuItem.Enabled = tilemapSelectionSelected;
			pencilToolStripButton.Enabled = tilemapSelectionSelected;
			eraserMenuItem.Enabled = tilemapSelected;
			eraserToolStripButton.Enabled = tilemapSelected;
			lineMenuItem.Enabled = false;// tilemapSelected;
			lineToolStripButton.Enabled = false;//tilemapSelected;
			paintBucketMenuItem.Enabled = tilemapSelectionSelected;
			paintBucketToolStripButton.Enabled = tilemapSelectionSelected;
			tilePickerMenuItem.Enabled = false;//tilemapSelected;
			tilePickerToolStripButton.Enabled = false;//tilemapSelected;
			rectangleMenuItem.Enabled = false;//tilemapSelected;
			rectangleToolStripButton.Enabled = false;//tilemapSelected;
			roundedRectangleMenuItem.Enabled = false;//tilemapSelected;
			roundedRectangleToolStripButton.Enabled = false;//tilemapSelected;
			ellipseMenuItem.Enabled = false;//tilemapSelected;
			ellipseToolStripButton.Enabled = false;//tilemapSelected;
			tileFlipXToolStripButton.Enabled = tilemapSelected;
			flipTileXMenuItem.Enabled = tilemapSelected;
			tileFlipYToolStripButton.Enabled = tilemapSelected;
			flipTileYMenuItem.Enabled = tilemapSelected;
			tileFlippingMenuItem.Enabled = tilemapSelected;

			// If we are not able to edit a tilemap then deselect any tilemap editing tools.
			if (tilemapSelected == false && _currentTool != Tools.Camera && _currentTool != Tools.Selector)
				ChangeTool(Tools.Camera);

            // Undo operations.
            if (_undoStack.Count > 0)
            {
                undoContextToolStripMenuItem.Text = "Undo " + ((UndoOperation)_undoStack.Peek()).Name;
                undoContextToolStripMenuItem.Enabled = true;
                undoMenuItem.Text = "Undo " + ((UndoOperation)_undoStack.Peek()).Name;
                undoMenuItem.Enabled = true;
                undoToolStripButton.Enabled = true;
            }
            else
            {
                undoContextToolStripMenuItem.Text = "Undo";
                undoContextToolStripMenuItem.Enabled = false;
                undoMenuItem.Text = "Undo";
                undoMenuItem.Enabled = false;
                undoToolStripButton.Enabled = false;
            }
            
            // Redo operations.
            if (_redoStack.Count > 0)
            {
                redoContextToolStripMenuItem.Text = "Redo " + ((UndoOperation)_redoStack.Peek()).Name;
                redoContextToolStripMenuItem.Enabled = true;
                redoMenuItem.Text = "Redo " + ((UndoOperation)_redoStack.Peek()).Name;
                redoMenuItem.Enabled = true;
                redoToolStripButton.Enabled = true;
            }
            else
            {
                redoContextToolStripMenuItem.Text = "Redo";
                redoContextToolStripMenuItem.Enabled = false;
                redoMenuItem.Text = "Redo";
                redoMenuItem.Enabled = false;
                redoToolStripButton.Enabled = false;
            }
		}

		/// <summary>
		///		Called when an event is being processed by the EventManager.
		/// </summary>
		/// <param name="firedEvent">Event that needs to be processed.</param>
		public void EventCaptured(Event firedEvent)
		{
			// Switch over to the map canvas so all input is relative to it.
			GraphicsManager.RenderTarget = (IRenderTarget)_mapCanvas;

			// If we are supressing selection see if we can unsupress selection.
			if (_supressSelection == true && _supressSelectionTimer.DurationMillisecond > 500)
				_supressSelection = false;

			// Check what event we have retrieved.
			switch (firedEvent.ID)
			{
				case "key_pressed":
					{
						InputEventData data = (InputEventData)firedEvent.Data;
						int mouseX = InputManager.MouseX;
						int mouseY = InputManager.MouseY;
                        if (ActiveForm != this || mouseX < 0 || mouseY < 0 || mouseX > mapPanel.ClientRectangle.Width || mouseY > mapPanel.ClientRectangle.Height)
							return;
                        if (mapCanvasContextMenuStrip.Visible == true)
                            return;

						if (data.KeyCode == KeyCodes.RightMouse)
						{
							mousePositionBeforeRightClick[0] = mouseX;
							mousePositionBeforeRightClick[1] = mouseY;
							mapCanvasContextMenuStrip.Show(new Point(InputManager.MouseScreenX + 5, InputManager.MouseScreenY - 16));
						}
					}
					break;
			}

			// Update the current tool's logic.
			switch (_currentTool)
			{
				case Tools.Camera: UpdateCamera(firedEvent); break;
				case Tools.Ellipse: break;
				case Tools.Eraser: UpdateEraser(firedEvent); break;
				case Tools.PaintBucket: UpdatePaintBucket(firedEvent); break;
				case Tools.Pencil: UpdatePencil(firedEvent); break;
				case Tools.Rectangle: break;
				case Tools.RoundedRectangle: break;
				case Tools.Line: break;
				case Tools.Selector: UpdateSelector(firedEvent); break;
				case Tools.TilePicker: UpdateTilePicker(firedEvent); break;
			}
		}

		/// <summary>
		///		Called when the camera tool is selected and an event has been recieved.
		/// </summary>
		/// <param name="firedEvent">Event that caused this update to be called.</param>
		public void UpdateCamera(Event firedEvent)
		{
			if (firedEvent.ID == "mouse_move")
			{
				// Zoom in and out.
				InputEventData data = (InputEventData)firedEvent.Data;
				int mouseX = InputManager.MouseX;
				int mouseY = InputManager.MouseY;
				if (data.MouseScroll != 0)
				{
					if (ActiveForm != this || mouseX < 0 || mouseY < 0 || mouseX > ClientRectangle.Width || mouseY > ClientRectangle.Height)
						return;
					Editor.GlobalInstance.CameraNode.Zoom += (data.MouseScroll < 0 ? (Editor.GlobalInstance.CameraNode.Zoom > 0.25 ? -0.25f : 0.0f) : (Editor.GlobalInstance.CameraNode.Zoom < 10.0f ? 0.25f : 0.0f));
                    zoomStatusLabel.Text = "Zoom: " + ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
                    zoomToolStripComboBox.Text = (Editor.GlobalInstance.CameraNode.Zoom * 100.0f) + "%";
                }

				if (_toolMoving == false || InputManager.KeyDown(KeyCodes.LeftMouse) == false) return;
                Editor.GlobalInstance.CameraNode.Move(_xAxisLocked == true ? 0.0f : -((InputManager.MouseX - _toolMovementDimensions.X) / Editor.GlobalInstance.CameraNode.Zoom), _yAxisLocked == true ? 0.0f : -((InputManager.MouseY - _toolMovementDimensions.Y) / Editor.GlobalInstance.CameraNode.Zoom), 0);
				_toolMovementDimensions.X = InputManager.MouseX;
				_toolMovementDimensions.Y = InputManager.MouseY;
			}
			else if (firedEvent.ID == "key_pressed" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
			{
				_toolMoving = true;
				_toolMovementDimensions.X = InputManager.MouseX;
				_toolMovementDimensions.Y = InputManager.MouseY;
			}
			else if (firedEvent.ID == "key_released" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
			{
				_toolMoving = false;
			}
		}

		/// <summary>
		///		Called when the selector tool is selected and an event has been recieved.
		/// </summary>
		/// <param name="firedEvent">Event that caused this update to be called.</param>
		public void UpdateSelector(Event firedEvent)
		{
			if (firedEvent.ID == "mouse_move" && InputManager.KeyDown(KeyCodes.LeftMouse))
			{
				if (_toolMoving == true)
				{
					_toolMovementDimensions.Width = InputManager.MouseX - _toolMovementDimensions.X;
					_toolMovementDimensions.Height = InputManager.MouseY - _toolMovementDimensions.Y;
				}
				else if (_movingObject == true)
				{
					ArrayList groupsMoved = new ArrayList();
					foreach (EntityNode iterationEntity in _selectedEntityList)
					{
						if (iterationEntity.Parent != null && iterationEntity.Parent as GroupNode != null && groupsMoved.Contains(iterationEntity.Parent) == true) continue;
						EntityNode entity = (iterationEntity.Parent != null && iterationEntity.Parent as GroupNode != null) ? (GroupNode)iterationEntity.Parent : iterationEntity;
						
						entity.Move(_xAxisLocked == true ? 0.0f : ((InputManager.MouseX - _moveObjectDimensions.X) / Editor.GlobalInstance.CameraNode.Zoom), _yAxisLocked == true ? 0.0f : ((InputManager.MouseY - _moveObjectDimensions.Y) / Editor.GlobalInstance.CameraNode.Zoom), 0.0f);
						
						if (iterationEntity.Parent != null && iterationEntity.Parent as GroupNode != null)
							groupsMoved.Add(iterationEntity.Parent);
					}
					_moveObjectDimensions.X = InputManager.MouseX;
					_moveObjectDimensions.Y = InputManager.MouseY;
					_mapChangedSinceSave = true;
				}
				else if (_sizingObject == true)
				{
					int movementX = _xAxisLocked == true ? 0 : (int)((InputManager.MouseX - _moveObjectDimensions.X) / Editor.GlobalInstance.CameraNode.Zoom), movementY = _yAxisLocked == true ? 0 : (int)((InputManager.MouseY - _moveObjectDimensions.Y) / Editor.GlobalInstance.CameraNode.Zoom);
					_objectToSize.BoundingRectangle = _objectSizeOriginalBoundingBox;
					_objectToSize.Transformation = _objectSizeOriginalTransformation;

					if ((_sizingDirection & SizingDirection.Top) != 0)
					{
						_objectToSize.Resize(_objectToSize.BoundingRectangle.Width, Math.Max(_objectToSize.BoundingRectangle.Height + -movementY,_gridHeight));
						_objectToSize.Position(_objectToSize.Transformation.X, _objectToSize.Transformation.Y + -(_objectToSize.BoundingRectangle.Height - _objectSizeOriginalBoundingBox.Height), _objectToSize.Transformation.Z);
					}
					if ((_sizingDirection & SizingDirection.Left) != 0)
					{
						_objectToSize.Resize(Math.Max(_objectToSize.BoundingRectangle.Width + -movementX, _gridWidth), _objectToSize.BoundingRectangle.Height);
						_objectToSize.Position(_objectToSize.Transformation.X + -(_objectToSize.BoundingRectangle.Width - _objectSizeOriginalBoundingBox.Width), _objectToSize.Transformation.Y, _objectToSize.Transformation.Z);
					}

					if ((_sizingDirection & SizingDirection.Bottom) != 0)
						_objectToSize.Resize(_objectToSize.BoundingRectangle.Width, Math.Max(_objectToSize.BoundingRectangle.Height + movementY, _gridHeight));
					if ((_sizingDirection & SizingDirection.Right) != 0)
						_objectToSize.Resize(Math.Max(_objectToSize.BoundingRectangle.Width + movementX, _gridWidth), _objectToSize.BoundingRectangle.Height);
					_mapChangedSinceSave = true;
				}
			}
			else if (firedEvent.ID == "key_pressed" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
			{
                bool touchingSizingPoint = false;
                Rectangle mouseRectangle = new Rectangle(InputManager.MouseX, InputManager.MouseY, 1, 1);
                foreach (EntityNode node in _selectedEntityList)
                    if (node.RectangleSizingPointsIntersect(mouseRectangle, Editor.GlobalInstance.CameraNode))
                        touchingSizingPoint = true;

                // Was last click in same spot? If so we want to select the new object.
                if (touchingSizingPoint == false && ((_toolMovementDimensions.X == InputManager.MouseX && _toolMovementDimensions.Y == InputManager.MouseY) || (_toolMovementDimensions.Width == 0 || _toolMovementDimensions.Height == 0)))
                {
                    if (InputManager.KeyDown(KeyCodes.LeftControl))
                    {
                        Rectangle intersectRectangle = new Rectangle(InputManager.MouseX, InputManager.MouseY, 1, 1);
                        foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
                        {
                            EntityNode entity = node as EntityNode;
                            if (entity == null) continue;

                            if (entity.RectangleBoundingBoxIntersect(intersectRectangle, Editor.GlobalInstance.CameraNode) == true)
                            {
                                if (_selectedEntityList.Contains(entity))
                                    RemoveEntityFromSelection(entity);
                                else
                                    AddEntityToSelection(entity);
                                break;
                            }
                        }

                        SyncronizeWindow();
                    }
                    else
                    {
                        Rectangle intersectRectangle = new Rectangle(InputManager.MouseX, InputManager.MouseY, 1, 1);
                        bool canSelect = (_selectedEntityList.Count == 0);
                        EntityNode selectEntity = null, firstEntity = null;

                        foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
                        {
                            EntityNode entity = node as EntityNode;
                            if (entity == null) continue;

                            if (entity.RectangleBoundingBoxIntersect(intersectRectangle, Editor.GlobalInstance.CameraNode) == true)
                            {
                                if (canSelect == true)
                                {
                                    selectEntity = entity;
                                    break;
                                }
                                else if (firstEntity != null)
                                    firstEntity = entity;

                                if (_selectedEntityList.Contains(entity))
                                    canSelect = true;
                            }
                        }
                        if (selectEntity == null)
                            selectEntity = firstEntity;

                        ClearSelection();

                        if (selectEntity != null)
                        {
                            AddEntityToSelection(selectEntity);
                            SyncronizeWindow();
                        }

                        _movingObject = true;
                        _moveObjectDimensions.X = InputManager.MouseX;
                        _moveObjectDimensions.Y = InputManager.MouseY;
                    }
                }
                else
                {
                    ArrayList removeList = new ArrayList();
                    Rectangle intersectRectangle = new Rectangle(InputManager.MouseX, InputManager.MouseY, 1, 1);
                    foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
                    {
                        EntityNode entity = node as EntityNode;
                        if (entity == null) continue;
                        if (entity.RectangleBoundingBoxIntersect(intersectRectangle, Editor.GlobalInstance.CameraNode) == false &&
                            entity.RectangleSizingPointsIntersect(intersectRectangle, Editor.GlobalInstance.CameraNode) == false)
                            removeList.Add(entity);
                    }
                    if (!InputManager.KeyDown(KeyCodes.LeftControl))
                    {
                        foreach (EntityNode entity in removeList)
                            RemoveEntityFromSelection(entity);
                    }

                    _movingObject = true;
                    _moveObjectDimensions.X = InputManager.MouseX;
                    _moveObjectDimensions.Y = InputManager.MouseY;
                }

                // Lets select an object or size one.
                foreach (EntityNode entity in _selectedEntityList)
                {
                    Transformation transformation = entity.CalculateTransformation(Editor.GlobalInstance.CameraNode);
                    int x = (int)(transformation.X - (entity.SizingPointsSize / 2)), y = (int)(transformation.Y - (entity.SizingPointsSize / 2));
                    int w = (int)(entity.BoundingRectangle.Width * Editor.GlobalInstance.CameraNode.Zoom), h = (int)(entity.BoundingRectangle.Height * Editor.GlobalInstance.CameraNode.Zoom);

                    bool previousMoving = _movingObject;
                    _movingObject = false;
                    _sizingObject = true;
                    _objectToSize = entity;
                    _objectSizeOriginalBoundingBox = _objectToSize.BoundingRectangle;
                    _objectSizeOriginalTransformation = _objectToSize.Transformation;
                    if (mouseRectangle.IntersectsWith(new Rectangle(x, y, entity.SizingPointsSize, entity.SizingPointsSize)))
                    {
                        _sizingDirection = SizingDirection.Top | SizingDirection.Left;
                        Cursor = Cursors.SizeNWSE;
                    }
                    else if (mouseRectangle.IntersectsWith(new Rectangle(x + w, y, entity.SizingPointsSize, entity.SizingPointsSize)))
                    {
                        _sizingDirection = SizingDirection.Top | SizingDirection.Right;
                        Cursor = Cursors.SizeNESW;
                    }
                    else if (mouseRectangle.IntersectsWith(new Rectangle(x, y + h, entity.SizingPointsSize, entity.SizingPointsSize)))
                    {
                        _sizingDirection = SizingDirection.Bottom | SizingDirection.Left;
                        Cursor = Cursors.SizeNESW;
                    }
                    else if (mouseRectangle.IntersectsWith(new Rectangle(x + w, y + h, entity.SizingPointsSize, entity.SizingPointsSize)))
                    {
                        _sizingDirection = SizingDirection.Bottom | SizingDirection.Right;
                        Cursor = Cursors.SizeNWSE;
                    }
                    else if (mouseRectangle.IntersectsWith(new Rectangle(x + (w / 2), y, entity.SizingPointsSize, entity.SizingPointsSize)))
                    {
                        _sizingDirection = SizingDirection.Top;
                        Cursor = Cursors.SizeNS;
                    }
                    else if (mouseRectangle.IntersectsWith(new Rectangle(x + (w / 2), y + h, entity.SizingPointsSize, entity.SizingPointsSize)))
                    {
                        _sizingDirection = SizingDirection.Bottom;
                        Cursor = Cursors.SizeNS;
                    }
                    else if (mouseRectangle.IntersectsWith(new Rectangle(x, y + (h / 2), entity.SizingPointsSize, entity.SizingPointsSize)))
                    {
                        _sizingDirection = SizingDirection.Left;
                        Cursor = Cursors.SizeWE;
                    }
                    else if (mouseRectangle.IntersectsWith(new Rectangle(x + w, y + (h / 2), entity.SizingPointsSize, entity.SizingPointsSize)))
                    {
                        _sizingDirection = SizingDirection.Right;
                        Cursor = Cursors.SizeWE;
                    }
                    else
                    {
                        _sizingObject = false;
                        _objectToSize = null;
                        _movingObject = previousMoving;
                    }
                }

                // If we are holding shift then we want to select multiple objects.
                if (InputManager.KeyDown(KeyCodes.LeftShift))
                {
                    _movingObject = false;
                    _sizingObject = false;
                    _toolMoving = true;
                }

                // Log the original transformations of the selected nodes.
                _originalEntityTransformations.Clear();
                _originalEntityBoundingBoxs.Clear();
                foreach (EntityNode entity in _selectedEntityList)
                {
                    _originalEntityTransformations.Add(entity, entity.Transformation);
                    _originalEntityBoundingBoxs.Add(entity, entity.BoundingRectangle);
                }

                _toolMovementDimensions.X = InputManager.MouseX;
                _toolMovementDimensions.Y = InputManager.MouseY;
                _toolMovementDimensions.Width = _selectedEntityList.Count > 0 ? 1 : 0;
                _toolMovementDimensions.Height = _selectedEntityList.Count > 0 ? 1 : 0;
			}
			else if (firedEvent.ID == "key_released" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
			{
				if (_toolMoving == true)
				{
					_toolMoving = false;
                    
					// Create a rectangle from which we can check intersection against.
					Rectangle intersectRectangle = new Rectangle();

					// If we have a negative width then make it positive.
					if (_toolMovementDimensions.Width < 0)
					{
						intersectRectangle.X = _toolMovementDimensions.X + _toolMovementDimensions.Width;
						intersectRectangle.Width = Math.Abs(_toolMovementDimensions.Width);
					}
					else
					{
						intersectRectangle.X = _toolMovementDimensions.X;
						intersectRectangle.Width = _toolMovementDimensions.Width;
					}

					// If we have a negative height then make it positive.		
					if (_toolMovementDimensions.Height < 0)
					{
						intersectRectangle.Y = _toolMovementDimensions.Y + _toolMovementDimensions.Height;
						intersectRectangle.Height = Math.Abs(_toolMovementDimensions.Height);
					}
					else
					{
						intersectRectangle.Y = _toolMovementDimensions.Y;
						intersectRectangle.Height = _toolMovementDimensions.Height;
					}

                    // Unselect all the pervious entitys.
                    ArrayList oldSelectionList = new ArrayList();
                    oldSelectionList.AddRange(_selectedEntityList);

                    // Clear out old selection.
                    if (InputManager.KeyDown(KeyCodes.LeftControl) == false)
                        ClearSelection();

					// Grab all the entitys within our selection rectangle and select them.
                    bool canSelect = (oldSelectionList.Count == 0 || !(intersectRectangle.Width < 2 && intersectRectangle.Height < 2));
                    EntityNode selectEntity = null;
					foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
					{
						EntityNode entity = node as EntityNode;
						if (entity == null) continue;

                        if (entity.RectangleBoundingBoxIntersect(intersectRectangle, Editor.GlobalInstance.CameraNode) == true)
                        {
                            if (canSelect == true)
                            {
                                AddEntityToSelection(entity);
                                if (intersectRectangle.Width < 2 && intersectRectangle.Height < 2)
                                    break;
                            }
                            else if (selectEntity == null) 
                                selectEntity = entity;

                            if (oldSelectionList.Contains(entity))
                                canSelect = true;
                        }
                    }
                    if (canSelect == false && selectEntity != null)
                        AddEntityToSelection(selectEntity);
				}
				else if (_movingObject == true)
				{
					// Snap the objects to the grid.
					if (_snapToGrid == true)
					{
                        foreach (EntityNode entity in _selectedEntityList)
                        {
                            entity.Position(((int)entity.Transformation.X / _gridWidth) * _gridWidth, ((int)entity.Transformation.Y / _gridHeight) * _gridHeight, entity.Transformation.Z);

                            // Create a new transformation undo operation if transformation has changed.
                            Transformation originalTransformation = (Transformation)_originalEntityTransformations[entity];
                            if (originalTransformation != entity.Transformation)
                                PushUndoOperation(new TransformNodeUndoOperation(entity, originalTransformation, entity.Transformation));
                        }
                        _originalEntityTransformations.Clear();
                        if (_entityPropertiesWindow != null && _entityPropertiesWindow.Entity != null)
                            _entityPropertiesWindow.UpdateProperty("location", new Point((int)_entityPropertiesWindow.Entity.Transformation.X, (int)_entityPropertiesWindow.Entity.Transformation.Y));
                    }
					_movingObject = false;
					_mapChangedSinceSave = true;
				}
				else if (_sizingObject == true)
				{
                    if (_snapToGrid == true)
                    {
                        _objectToSize.Position(((int)_objectToSize.Transformation.X / _gridWidth) * _gridWidth, ((int)_objectToSize.Transformation.Y / _gridHeight) * _gridHeight, _objectToSize.Transformation.Z);
                        _objectToSize.BoundingRectangle = new Rectangle(_objectToSize.BoundingRectangle.X, _objectToSize.BoundingRectangle.Y, ((int)_objectToSize.BoundingRectangle.Width / _gridWidth) * _gridWidth, ((int)_objectToSize.BoundingRectangle.Height / _gridHeight) * _gridHeight);
                        
                        Rectangle newBoundingRectangle = _objectToSize.BoundingRectangle;
                        if (newBoundingRectangle.Width < _gridWidth) newBoundingRectangle.Width = _gridWidth;
                        if (newBoundingRectangle.Height < _gridHeight) newBoundingRectangle.Height = _gridHeight;
                        _objectToSize.BoundingRectangle = newBoundingRectangle;

                        // Create a new sizing undo operation if size has changed.
                        Rectangle originalBoundingBox = (Rectangle)_originalEntityBoundingBoxs[_objectToSize];
                        if (!_objectToSize.BoundingRectangle.Equals(originalBoundingBox))
                            PushUndoOperation(new ResizeNodeUndoOperation(_objectToSize, originalBoundingBox, (Transformation)_originalEntityTransformations[_objectToSize], _objectToSize.BoundingRectangle, _objectToSize.Transformation));
                    }
                    if (_entityPropertiesWindow != null && _entityPropertiesWindow.Entity != null)
                    {
                        _entityPropertiesWindow.UpdateProperty("bounding rectangle", new Rectangle(_entityPropertiesWindow.Entity.BoundingRectangle.X, _entityPropertiesWindow.Entity.BoundingRectangle.Y, _entityPropertiesWindow.Entity.BoundingRectangle.Width, _entityPropertiesWindow.Entity.BoundingRectangle.Height));
					    _entityPropertiesWindow.UpdateProperty("collision rectangle", new Rectangle(_entityPropertiesWindow.Entity.CollisionRectangle.X, _entityPropertiesWindow.Entity.CollisionRectangle.Y, _entityPropertiesWindow.Entity.CollisionRectangle.Width, _entityPropertiesWindow.Entity.CollisionRectangle.Height));
				    }
                    Cursor = Cursors.Arrow;
					_sizingObject = false;
					_mapChangedSinceSave = true;
				}

                SyncronizeWindow();
			}
		}

		/// <summary>
		///		Called when the pencil tool is selected and an event has been recieved.
		/// </summary>
		/// <param name="firedEvent">Event that caused this update to be called.</param>
		public void UpdatePencil(Event firedEvent)
		{
			bool placeTiles = false;
			if (firedEvent.ID == "mouse_move" && InputManager.KeyDown(KeyCodes.LeftMouse))
			{
				if (_toolActive == false) return;
				placeTiles = true;
			}
			else if (firedEvent.ID == "key_pressed" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
			{
				_toolActive = true;
				placeTiles = true;
			}
			else if (firedEvent.ID == "key_released" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
				_toolActive = false;

			// Place the tiles onto the map at the cursor position if we have been asked to.
			if (placeTiles == true)
			{
				// Go through entitys and see if we are on a tilemap.
				TilemapSegmentNode tilemapToEdit = null;
				foreach (EntityNode entity in _selectedEntityList)
				{
					if ((entity is TilemapSegmentNode) == false) continue;
					if (entity.RectangleBoundingBoxIntersect(new Rectangle(InputManager.MouseX, InputManager.MouseY, 1, 1), Editor.GlobalInstance.CameraNode) == true)
					{
						tilemapToEdit = entity as TilemapSegmentNode;
						break;
					}
				}
				if (tilemapToEdit == null) return;

				// Work out what tile the mouse if over.
				Transformation tilemapTransformation = tilemapToEdit.CalculateTransformation(Editor.GlobalInstance.CameraNode);
                int tileX = (int)((InputManager.MouseX - tilemapTransformation.X) / (tilemapToEdit.TileWidth * Editor.GlobalInstance.CameraNode.Zoom));
				int tileY = (int)((InputManager.MouseY - tilemapTransformation.Y) / (tilemapToEdit.TileHeight * Editor.GlobalInstance.CameraNode.Zoom));

				// Place the tiles onto the tilemap.
				Rectangle tilesetSelection = _assetManagerWindow.TilesetSelection;
                Tileset selectedTileset = GetSelectedTileset();

				for (int x = tileX; x < tileX + tilesetSelection.Width; x++)
				{
					for (int y = tileY; y < tileY + tilesetSelection.Height; y++)
					{
						if (x < 0 || y < 0 || x >= tilemapToEdit.Width / tilemapToEdit.TileWidth || y >= tilemapToEdit.Height / tilemapToEdit.TileHeight) continue;
						tilemapToEdit[x, y].Tileset = selectedTileset;
						tilemapToEdit[x, y].Frame = selectedTileset.Image.FrameIndexAtPosition((tilesetSelection.X + (x - tileX)) * selectedTileset.Image.Width, (tilesetSelection.Y + (y - tileY)) * selectedTileset.Image.Height);
						tilemapToEdit[x, y].Color = _assetManagerWindow.TilesetColor;
						tilemapToEdit[x, y].Scale(_flipTilesX == true ? -1 : 1, _flipTilesY == true ? -1 : 1, 1);
					}
				}

				_mapChangedSinceSave = true;
			}
		}

        /// <summary>
        ///     Gets the currently selected tileset.
        /// </summary>
        /// <returns>Current selected tileset.</returns>
        public Tileset GetSelectedTileset()
        {
            if (_selectedTileset == null || _assetManagerWindow.SelectedTileset.ToLower() != _selectedTileset.Url.ToString().ToLower())
            {
                _selectedTileset = null;
                for (int i = 0; i < Tileset.TilesetPool.Count; i++)
                {
                    if ((Tileset.TilesetPool[i] as Tileset).Url.ToString().ToLower() == _assetManagerWindow.SelectedTileset.ToLower())
                        _selectedTileset = (Tileset.TilesetPool[i] as Tileset);
                }

                if (_selectedTileset == null)
                {
                    _selectedTileset = new Tileset(_assetManagerWindow.SelectedTileset.ToLower());
                    Tileset.AddToTilesetPool(_selectedTileset);
                }
            }
            return _selectedTileset;
        }

		/// <summary>
		///		Called when the eraser tool is selected and an event has been recieved.
		/// </summary>
		/// <param name="firedEvent">Event that caused this update to be called.</param>
		public void UpdateEraser(Event firedEvent)
		{
			bool placeTiles = false;
			if (firedEvent.ID == "mouse_move" && InputManager.KeyDown(KeyCodes.LeftMouse))
			{
				if (_toolActive == false) return;
				placeTiles = true;
			}
			else if (firedEvent.ID == "key_pressed" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
			{
				_toolActive = true;
				placeTiles = true;
			}
			else if (firedEvent.ID == "key_released" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
				_toolActive = false;

			// Place the tiles onto the map at the cursor position if we have been asked to.
			if (placeTiles == true)
			{
				// Go through entitys and see if we are on a tilemap.
				TilemapSegmentNode tilemapToEdit = null;
				foreach (EntityNode entity in _selectedEntityList)
				{
					if ((entity is TilemapSegmentNode) == false) continue;
                    if (entity.RectangleBoundingBoxIntersect(new Rectangle(InputManager.MouseX, InputManager.MouseY, 1, 1), Editor.GlobalInstance.CameraNode) == true)
						tilemapToEdit = entity as TilemapSegmentNode;
				}
				if (tilemapToEdit == null) return;

				// Work out what tile the mouse if over.
				Transformation tilemapTransformation = tilemapToEdit.CalculateTransformation(Editor.GlobalInstance.CameraNode);
				int tileX = (int)((InputManager.MouseX - tilemapTransformation.X) / (tilemapToEdit.TileWidth * Editor.GlobalInstance.CameraNode.Zoom));
				int tileY = (int)((InputManager.MouseY - tilemapTransformation.Y) / (tilemapToEdit.TileHeight * Editor.GlobalInstance.CameraNode.Zoom));
				if (tileX < 0 || tileY < 0 || tileX >= tilemapToEdit.Width / tilemapToEdit.TileWidth || tileY >= tilemapToEdit.Height / tilemapToEdit.TileHeight) return;
						
				// Erase the given tile.
				tilemapToEdit[tileX, tileY].Tileset = null;
				tilemapToEdit[tileX, tileY].Frame = 0;

				_mapChangedSinceSave = true;
			}
		}

		/// <summary>
		///		Called when the paint bucket tool is selected and an event has been recieved.
		/// </summary>
		/// <param name="firedEvent">Event that caused this update to be called.</param>
		public void UpdateTilePicker(Event firedEvent)
		{
			bool selectTile = false;
			if (firedEvent.ID == "key_pressed" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
			{
				_toolActive = true;
				selectTile = true;
			}
			else if (firedEvent.ID == "key_released" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
			{
				_toolActive = false;
			}
			else if (firedEvent.ID == "mouse_move")
			{
				if (_toolActive == true) selectTile = true;
			}

			if (selectTile == true)
			{
				// Go through entitys and see if we are on a tilemap.
				TilemapSegmentNode tilemapToEdit = null;
				foreach (EntityNode entity in _selectedEntityList)
				{
					if ((entity is TilemapSegmentNode) == false) continue;
                    if (entity.RectangleBoundingBoxIntersect(new Rectangle(InputManager.MouseX, InputManager.MouseY, 1, 1), Editor.GlobalInstance.CameraNode) == true)
						tilemapToEdit = entity as TilemapSegmentNode;
				}
				if (tilemapToEdit == null) return;

				// Work out what tile the mouse if over.
				Transformation tilemapTransformation = tilemapToEdit.CalculateTransformation(Editor.GlobalInstance.CameraNode);
				int tileX = (int)((InputManager.MouseX - tilemapTransformation.X) / (tilemapToEdit.TileWidth * Editor.GlobalInstance.CameraNode.Zoom));
				int tileY = (int)((InputManager.MouseY - tilemapTransformation.Y) / (tilemapToEdit.TileHeight * Editor.GlobalInstance.CameraNode.Zoom));
				if (tileX < 0 || tileY < 0 || tileX >= tilemapToEdit.Width / tilemapToEdit.TileWidth || tileY >= tilemapToEdit.Height / tilemapToEdit.TileHeight) return;
				
				// TO DO Pick the tile.
				//_assetManagerWindow.SelectedTileset = tilemapToEdit[tileX, tileY].Tileset;
				//if (tilemapToEdit[tileX, tileY].Tileset == null || tilemapToEdit[tileX, tileY].Frame == -1) return;

				//int frame = tilemapToEdit[tileX, tileY].Frame;
				//int cellsH = tilemapToEdit[tileX, tileY].Tileset.Image.FullWidth / (tilemapToEdit[tileX, tileY].Tileset.Image.Width + tilemapToEdit[tileX, tileY].Tileset.Image.HorizontalSpacing);
				//int cellsV = tilemapToEdit[tileX, tileY].Tileset.Image.FullHeight / (tilemapToEdit[tileX, tileY].Tileset.Image.Height + tilemapToEdit[tileX, tileY].Tileset.Image.VerticalSpacing);
				//_assetManagerWindow.TilesetSelection = new Rectangle(frame % cellsH, frame / cellsH, 1, 1);
				//_assetManagerWindow.TilesetColor = tilemapToEdit[tileX, tileY].Color;
			}
		}

		/// <summary>
		///		Called when the paint bucket tool is selected and an event has been recieved.
		/// </summary>
		/// <param name="firedEvent">Event that caused this update to be called.</param>
		public void UpdatePaintBucket(Event firedEvent)
		{
			if (firedEvent.ID == "key_pressed" && ((InputEventData)firedEvent.Data).KeyCode == KeyCodes.LeftMouse)
			{
				// Go through entitys and see if we are on a tilemap.
				TilemapSegmentNode tilemapToEdit = null;
				foreach (EntityNode entity in _selectedEntityList)
				{
					if ((entity is TilemapSegmentNode) == false) continue;
                    if (entity.RectangleBoundingBoxIntersect(new Rectangle(InputManager.MouseX, InputManager.MouseY, 1, 1), Editor.GlobalInstance.CameraNode) == true)
						tilemapToEdit = entity as TilemapSegmentNode;
				}
				if (tilemapToEdit == null) return;

				// Work out what tile the mouse if over.
				Transformation tilemapTransformation = tilemapToEdit.CalculateTransformation(Editor.GlobalInstance.CameraNode);
				int tileX = (int)((InputManager.MouseX - tilemapTransformation.X) / (tilemapToEdit.TileWidth * Editor.GlobalInstance.CameraNode.Zoom));
				int tileY = (int)((InputManager.MouseY - tilemapTransformation.Y) / (tilemapToEdit.TileHeight * Editor.GlobalInstance.CameraNode.Zoom));

				// Preform a flood fill at the given position.
				bool[,] tracker = (new bool[tilemapToEdit.Width / tilemapToEdit.TileWidth, tilemapToEdit.Height / tilemapToEdit.TileHeight]);
				FloodFill(tilemapToEdit, tileX, tileY, tilemapToEdit[tileX, tileY].Tileset, tilemapToEdit[tileX, tileY].Frame, ref tracker, _assetManagerWindow.TilesetSelection.X, _assetManagerWindow.TilesetSelection.Y);
				
				_mapChangedSinceSave = true;
			}
		}

		/// <summary>
		///		Preforms a flood fill operation of the given tilemap.
		/// </summary>
		/// <param name="tilemapNode">Tilemap to flood fill.</param>
		/// <param name="x">X position to start flood filling.</param>
		/// <param name="y">Y position to start flood filling.</param>
		/// <param name="fillTileset">Tileset to overwrite when flood filling.</param>
		/// <param name="fillFrame">Frame to overwrite when flood filling.</param>
		/// <param name="fillTracker">Array to keep track of tiles that have been flooded.</param>
		/// <param name="selectionX">X position of the tile being used to fill this flood fill step with.</param>
		/// <param name="selectionY">Y position of the tile being used to fill this flood fill step with.</param>
		private void FloodFill(TilemapSegmentNode tilemapNode, int x, int y, Tileset fillTileset, int fillFrame, ref bool[,] fillTracker, int selectionX, int selectionY)
		{
			// If the selection has gone beyond the width or height of the selection 
			// then wrap it around.
			if (selectionX < _assetManagerWindow.TilesetSelection.X)
				selectionX = _assetManagerWindow.TilesetSelection.X + _assetManagerWindow.TilesetSelection.Width - 1;
			
			if (selectionX > _assetManagerWindow.TilesetSelection.X + _assetManagerWindow.TilesetSelection.Width - 1)
				selectionX = _assetManagerWindow.TilesetSelection.X;			
			
			if (selectionY < _assetManagerWindow.TilesetSelection.Y)
				selectionY = _assetManagerWindow.TilesetSelection.Y + _assetManagerWindow.TilesetSelection.Height - 1;

			if (selectionY > _assetManagerWindow.TilesetSelection.Y + _assetManagerWindow.TilesetSelection.Height - 1)
				selectionY = _assetManagerWindow.TilesetSelection.Y;	

			// Work out the frame of the selection we should be applying to this tile.
            int selectionFrame = GetSelectedTileset().Image.FrameIndexAtPosition(selectionX * GetSelectedTileset().Image.Width, selectionY * GetSelectedTileset().Image.Height);
		
			// If this tile should be set then set it.
			if (tilemapNode[x, y].Tileset == fillTileset && tilemapNode[x, y].Frame == fillFrame)
			{
                tilemapNode[x, y].Tileset = GetSelectedTileset();
				tilemapNode[x, y].Frame = selectionFrame;
				tilemapNode[x, y].Scale(_flipTilesX == true ? -1 : 1, _flipTilesY == true ? -1 : 1, 1);
				tilemapNode[x, y].Color = _assetManagerWindow.TilesetColor;
			}

			// Make this tile as filled.
			fillTracker[x, y] = true;

			int tilemapNodeWidth = tilemapNode.Width / tilemapNode.TileWidth;
			int tilemapNodeHeight = tilemapNode.Height / tilemapNode.TileHeight;

			// Check the tile to the left of this one, and see if it can be filled.
			if (x - 1 >= 0 && x - 1 < tilemapNodeWidth && fillTracker[x - 1, y] == false && tilemapNode[x - 1, y].Tileset == fillTileset && tilemapNode[x - 1, y].Frame == fillFrame)
				FloodFill(tilemapNode, x - 1, y, fillTileset, fillFrame, ref fillTracker, selectionX - 1, selectionY);

			// Check the tile to the right of this one, and see if it can be filled.
			if (x + 1 >= 0 && x + 1 < tilemapNodeWidth && fillTracker[x + 1, y] == false && tilemapNode[x + 1, y].Tileset == fillTileset && tilemapNode[x + 1, y].Frame == fillFrame)
				FloodFill(tilemapNode, x + 1, y, fillTileset, fillFrame, ref fillTracker, selectionX + 1, selectionY);	

			// Check the tile at the top of this one, and see if it can be filled.
			if (y - 1 >= 0 && y - 1 < tilemapNodeHeight && fillTracker[x, y - 1] == false && tilemapNode[x, y - 1].Tileset == fillTileset && tilemapNode[x, y - 1].Frame == fillFrame)
				FloodFill(tilemapNode, x, y - 1, fillTileset, fillFrame, ref fillTracker, selectionX, selectionY - 1);

			// Check the tile at the bottom of this one, and see if it can be filled.
			if (y + 1 >= 0 && y + 1 < tilemapNodeHeight && fillTracker[x, y + 1] == false && tilemapNode[x, y + 1].Tileset == fillTileset && tilemapNode[x, y + 1].Frame == fillFrame)
				FloodFill(tilemapNode, x, y + 1, fillTileset, fillFrame, ref fillTracker, selectionX, selectionY + 1);
		}

		/// <summary>
		///		Called when the selector tool is selected and the scene is being rendered.
		/// </summary>
		public void RenderSelector()
		{
			if (_toolMoving == false) return;
			GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
            string positionText = "(" + _toolMovementDimensions.X + "," + _toolMovementDimensions.Y + ")";
            string sizeText = "(" + _toolMovementDimensions.Width + "," + _toolMovementDimensions.Height + ")";
            GraphicsManager.RenderText(positionText, _toolMovementDimensions.X - GraphicsManager.TextWidth(positionText, false), _toolMovementDimensions.Y - GraphicsManager.TextHeight(positionText, false), 0, false);
            GraphicsManager.RenderText(sizeText, _toolMovementDimensions.X + _toolMovementDimensions.Width, _toolMovementDimensions.Y + _toolMovementDimensions.Height, 0, false);
            GraphicsManager.RenderDashedRectangle(_toolMovementDimensions.X, _toolMovementDimensions.Y, 0, _toolMovementDimensions.Width, _toolMovementDimensions.Height);
		}

		/// <summary>
		///		Populates the recent files list from the engine configuration file.
		/// </summary>
		public void RefreshRecentFiles()
		{
			// Free any old recent files.
			for (int i = 0; i < _recentMenus.Length; i++)
				if (_recentMenus[i] != null) _recentMenus[i].Dispose();

			// Populate the recent files menu from the engine config file.
			int recentFileCount = 0;
			for (int i = 0; i < _recentMenus.Length; i++)
			{
				string file = Editor.GlobalInstance.EngineConfigFile["editor:recentfiles:" + (i + 1), ""];
				if (file == "") break;

				_recentMenus[i] = new ToolStripMenuItem(file);
				recentFilesMenuItem.DropDownItems.Add(_recentMenus[i]);
				_recentMenus[i].Click += new EventHandler(RecentFileMenu_Clicked);
				
				recentFileCount++;
			}
			recentFilesMenuItem.Enabled = recentFileCount > 0 ? true : false;
		}

		/// <summary>
		///		Appends a recent file to the recent files menu.
		/// </summary>
		/// <param name="file">Url of file to append.</param>
		public void AddRecentFile(string file)
		{
			bool foundFile = false;
			for (int i = 0; i < _recentMenus.Length; i++)
			{
				string rf = Editor.GlobalInstance.EngineConfigFile["editor:recentfiles:" + (i + 1), ""];
                if (rf != null && rf.ToLower() == file.ToLower())
                {
                    foundFile = true;
                    break;
                }
			}
			if (foundFile == false)
			{
				for (int i = _recentMenus.Length - 2; i >= 0; i--)
					Editor.GlobalInstance.EngineConfigFile.SetSetting("editor:recentfiles:" + (i + 1), Editor.GlobalInstance.EngineConfigFile["editor:recentfiles:" + ((i + 1) - 1), ""]);
				Editor.GlobalInstance.EngineConfigFile.SetSetting("editor:recentfiles:1", file);
			}
			RefreshRecentFiles();
		}

		/// <summary>
		///		Called when a recent file menu is clicked.
		/// </summary>
		/// <param name="sender">Recent file menu that caused this event.</param>
		/// <param name="e">Arguments explaining why this event was invoked.</param>
		public void RecentFileMenu_Clicked(object sender, EventArgs e)
		{
			// If there are any unsaved changes give the user an option to save. 
			if (_mapChangedSinceSave == true)
			{
				DialogResult result = MessageBox.Show("This map has been modified since it was last saved, would you like to save now?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (result == DialogResult.OK) SaveMap();
				if (result == DialogResult.Cancel) return;
			}

			// Open the recent file based on its text url.
			OpenMap(((ToolStripMenuItem)sender).Text);
		}

		/// <summary>
		///		Renders the current state of the editor to the map canvas. And notifys
		///		any sub-windows to render themselfs.
		/// </summary>  
		public void Render()
		{
			if (Form.ActiveForm == null || Engine.Engine.GlobalInstance.Map.SceneGraph == null)
				return;

			// Render the scene graph to the map canvas.
            HighPreformanceTimer renderTimer = new HighPreformanceTimer();
			GraphicsManager.RenderTarget = (IRenderTarget)_mapCanvas;
			GraphicsManager.ClearColor = unchecked((int)0xFFFFFFFF);
			GraphicsManager.BeginScene();

            // Make sure the viewport is correct.
            GraphicsManager.SetResolution(mapPanel.ClientSize.Width, mapPanel.ClientSize.Height, false);
            Editor.GlobalInstance.CameraNode.Viewport = new Rectangle(0, 0, mapPanel.ClientSize.Width, mapPanel.ClientSize.Height);

			// Render the scene graph.
			Engine.Engine.GlobalInstance.Map.SceneGraph.Render();

			// Render the current tool.
			GraphicsManager.PushRenderState();
			switch (_currentTool)
			{
				case Tools.Camera: break;
				case Tools.Ellipse: break;
				case Tools.Eraser: break;
				case Tools.PaintBucket: break;
				case Tools.Pencil: break;
				case Tools.Rectangle: break;
				case Tools.RoundedRectangle: break;
				case Tools.Line: break;
				case Tools.Selector: RenderSelector(); break;
				case Tools.TilePicker: break;
			}
			GraphicsManager.PopRenderState();

            // Render the origin.
            if (_viewOrigin == true)
            {
                GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF00FF00);
                GraphicsManager.RenderLine(0, -(Editor.GlobalInstance.CameraNode.Transformation.Y * Editor.GlobalInstance.CameraNode.Zoom), 0, _mapCanvas.RenderControl.ClientSize.Width, -(Editor.GlobalInstance.CameraNode.Transformation.Y * Editor.GlobalInstance.CameraNode.Zoom), 0);
                GraphicsManager.RenderLine(-(Editor.GlobalInstance.CameraNode.Transformation.X * Editor.GlobalInstance.CameraNode.Zoom), 0, 0, -(Editor.GlobalInstance.CameraNode.Transformation.X * Editor.GlobalInstance.CameraNode.Zoom), _mapCanvas.RenderControl.ClientSize.Height, 0);
            }

			GraphicsManager.FinishScene();
            GraphicsManager.PresentScene();

			// Update the FPS / rendering time status bars.
			this.fpsStatusLabel.Text = "Frames Per Second: " + Editor.GlobalInstance.CurrentFPS + "/" + Editor.GlobalInstance.FPSLimit;
			
			string renderTime = renderTimer.DurationMillisecond.ToString();
			this.renderTimeStatusLabel.Text = "Rendering time: " + ((renderTime.IndexOf('.') > 0 ? renderTime.Substring(0, renderTime.IndexOf('.') + 2) : renderTime) + "ms").PadRight(5);

            int mapX = (int)Editor.GlobalInstance.CameraNode.Transformation.X + (int)((InputManager.MouseX / Editor.GlobalInstance.CameraNode.Zoom));
            int mapY = (int)Editor.GlobalInstance.CameraNode.Transformation.Y + (int)((InputManager.MouseY / Editor.GlobalInstance.CameraNode.Zoom));
			this.cursorPositionStatusLabel.Text = "X:" + (mapX.ToString().PadRight(4)) + " Y:" + (mapY.ToString().PadRight(4)) + " x:" + (InputManager.MouseX.ToString().PadRight(4)) + " y:" + (InputManager.MouseY.ToString().PadRight(4));
        }

		/// <summary>
		///		Shows the build project window when the user clicks the 
		///		File->Build Project menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void buildProjectMenuItem_Click(object sender, EventArgs e)
		{
			(new ProjectBuildWindow()).ShowDialog();
		}

		/// <summary>
		///		Shows the about window when the Help->About menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void aboutMenuItem_Click(object sender, EventArgs e)
		{
			(new AboutWindow()).ShowDialog();
		}

		/// <summary>
		///		Opens up internet explorer on the binary phoenix page when the 
		///		Help->Visit Website menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void visitWebsiteMenuItem_Click(object sender, EventArgs e)
		{
			VisitWebsite();
		}

		/// <summary>
		///		Opens up the chm help file Help->Contents menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void helpContentsMenuItem_Click(object sender, EventArgs e)
		{
			ShowHelp();
		}

		/// <summary>
		///		Closes the editor if the File->Exit menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void exitMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		///		Shows the preferences window when the File->Preferences menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void preferencesMenuItem_Click(object sender, EventArgs e)
		{
			(new PreferencesWindow()).ShowDialog();
		}

		/// <summary>
		///		Shows the map properties window then the File->Map Properties menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void mapPropertiesMenuItem_Click(object sender, EventArgs e)
		{
			ShowMapProperties();
		}

		/// <summary>
		///		Shows the tip of the day window when the File->Tip Of The Day menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void tipOfTheDayMenuItem_Click(object sender, EventArgs e)
		{
			(new TipOfTheDayWindow()).ShowDialog();
		}

		/// <summary>
		///		Executes the Fusion game engine with the given starting map when the File->Play Map menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void playMapMenuItem_Click(object sender, EventArgs e)
		{
			PlayGame();
		}

		/// <summary>
		///		Executes the Fusion game engine with the given starting map when the Play Maap toolbar item is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void playMapToolStripButton_Click(object sender, EventArgs e)
		{
			PlayGame();
		}

		/// <summary>
		///		Executes the Fusion software updater.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void checkForUpdatesMenuItem_Click(object sender, EventArgs e)
		{
			CheckForUpdates();
		}

		/// <summary>
		///		Shows the debugging console if the console tool strip button is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void viewConsoleToolStripButton_Click(object sender, EventArgs e)
		{
			ShowConsole();
		}

		/// <summary>
		///		Shows the debugging console if the View->Console menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void consoleMenuItem_Click(object sender, EventArgs e)
		{
			ShowConsole();
		}

		/// <summary>
		///		Processes a console command when the user enters a new line character in the 
		///		console command text box.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void commandToolStripTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				Runtime.Console.Console.ProcessCommand(commandToolStripTextBox.Text.Replace("\n", "").Replace("\r", ""));
				commandToolStripTextBox.Text = "";
			}
		}

		/// <summary>
		///		Updates the snap-to-grid option when the user clicks the snap-to-grid tool strip button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void snapToGridToolStripButton_Click(object sender, EventArgs e)
		{
			_snapToGrid = !_snapToGrid;
			snapToGridToolStripButton.Checked = _snapToGrid;
		}

		/// <summary>
		///		Updates the width of the grid when the user changes the grid width tool strip button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void gridWidthToolStripTextBox_TextChanged(object sender, EventArgs e)
		{
            _gridWidth = gridWidthToolStripTextBox.Text == "" || gridWidthToolStripTextBox.Text == null ? 16 : int.Parse(gridWidthToolStripTextBox.Text);
		}

		/// <summary>
		///		Updates the height of the grid when the user changes the grid height tool strip button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void gridHeightToolStripTextBox_TextChanged(object sender, EventArgs e)
		{
            _gridHeight = gridHeightToolStripTextBox.Text == "" || gridHeightToolStripTextBox.Text == null ? 16 : int.Parse(gridHeightToolStripTextBox.Text);
		}

		/// <summary>
		///		Changes the tool to the one that corresponds to the tool pallete button 
		///		that has been clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void cameraToolStripButton_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Camera);
		}
		private void selectorToolStripButton_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Selector);
		}
		private void pencilToolStripButton_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Pencil);
		}
		private void eraserToolStripButton_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Eraser);
		}
		private void paintBucketToolStripButton_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.PaintBucket);
		}
		private void tilePickerToolStripButton_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.TilePicker);
		}
		private void rectangleToolStripButton_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Rectangle);
		}
		private void roundedRectangleToolStripButton_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.RoundedRectangle);
		}
		private void ellipseToolStripButton_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Ellipse);
		}
		private void lineToolStripButton_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Line);
		}

		/// <summary>
		///		Changes the tool to the one that corresponds to the menu button 
		///		that has been clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void cameraMenuItem_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Camera);
		}
		private void selectorMenuItem_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Selector);
		}
		private void pencilMenuItem_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Pencil);
		}
		private void eraserMenuItem_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Eraser);
		}
		private void paintBucketMenuItem_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.PaintBucket);
		}
		private void tilePickerMenuItem_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.TilePicker);
		}
		private void rectangleMenuItem_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Rectangle);
		}
		private void roundedRectangleMenuItem_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.RoundedRectangle);
		}
		private void ellipseMenuItem_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Ellipse);
		}
		private void lineMenuItem_Click(object sender, EventArgs e)
		{
			ChangeTool(Tools.Line);
		}

		/// <summary>
		///		Creates a new map when the user clicks the File->New Map menu.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void newMenuItem_Click(object sender, EventArgs e)
		{
			NewMap();
		}

		/// <summary>
		///		Opens a map from a file requested by he user when the user clicks the File->Open Map menu.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void openMenuItem_Click(object sender, EventArgs e)
		{
			OpenMap();
		}

		/// <summary>
		///		Saves a map to a file requested by he user when the user clicks the File->Save Map menu.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void saveMenuItem_Click(object sender, EventArgs e)
		{
			SaveMap();
		}

		/// <summary>
		///		Saves a map to a file requested by he user when the user clicks the File->Save As Map menu.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void saveAsMenuItem_Click(object sender, EventArgs e)
		{
			SaveMapAs();
		}

		/// <summary>
		///		Creates a new map when the user clicks the New Map toolbarbutton.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			NewMap();
		}

		/// <summary>
		///		Opens a map from a file requested by he user when the user clicks the Open Map toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			OpenMap();
		}

		/// <summary>
		///		Saves a map to a file requested by he user when the user clicks the Save Map toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			SaveMap();
		}

		/// <summary>
		///		Preforms a cut operation when the user clicks the Cut toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void cutToolStripButton_Click(object sender, EventArgs e)
		{
			Cut();
		}

		/// <summary>
		///		Preforms a copy operation when the user clicks the Copy toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void copyToolStripButton_Click(object sender, EventArgs e)
		{
			Copy();
		}

		/// <summary>
		///		Preforms a paste operation when the user clicks the Paste toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void pasteToolStripButton_Click(object sender, EventArgs e)
		{
			Paste(true);
		}

		/// <summary>
		///		Preforms an undo operation when the user clicks the Undo toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void undoToolStripButton_Click(object sender, EventArgs e)
		{
			Undo();
		}

		/// <summary>
		///		Preforms a redo operation when the user clicks the Redo toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void redoToolStripButton_Click(object sender, EventArgs e)
		{
			Redo();
		}

		/// <summary>
		///		Shows the project build window when the user clicks the Build Project toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void buildProjectToolStripButton_Click(object sender, EventArgs e)
		{
			(new ProjectBuildWindow()).ShowDialog();
		}

		/// <summary>
		///		Shows the map properties window when the user clicks the Map Properties toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void mapPropertiesToolStripButton_Click(object sender, EventArgs e)
		{
			ShowMapProperties();
		}

		/// <summary>
		///		Shows the asset manager window when the user clicks the Asset Manager toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void assetManagerToolStripButton_Click(object sender, EventArgs e)
		{
			ShowAssetManager();
		}

		/// <summary>
		///		Shows the scene graph window when the user clicks the Scene Graph toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void sceneGraphToolStripButton_Click(object sender, EventArgs e)
		{
			ShowSceneGraph();
		}

        /// <summary>
        ///		Shows the effects designer window when the user clicks the Effects Designer toolbar button.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void effectsDesignerMenuItem_Click(object sender, EventArgs e)
        {
            ShowEffectsDesigner();
        }

		/// <summary>
		///		Runs the help CHM file when the user clicks the Help toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void helpToolStripButton_Click(object sender, EventArgs e)
		{
			ShowHelp();
		}

		/// <summary>
		///		Shows the about window when the user clicks the About toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void aboutToolStripButton_Click(object sender, EventArgs e)
		{
			(new AboutWindow()).ShowDialog();
		}

		/// <summary>
		///		Preforms a horizontal selection flip when the user clicks the Horizontal Flip toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void flipHorizontalToolStripButton_Click(object sender, EventArgs e)
		{
			FlipSelection(false);
		}

		/// <summary>
		///		Preforms a vertical selection flip when the user clicks the Vertical Flip toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void flipVerticalToolStripButton_Click(object sender, EventArgs e)
		{
			FlipSelection(true);
		}

		/// <summary>
		///		Preforms a clockwise selection rotation when the user clicks the Clockwise Rotate toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void rotateClockwiseToolStripButton_Click(object sender, EventArgs e)
		{
			RotateSelection(90);
		}

		/// <summary>
		///		Preforms an Anticlockwise selection rotation when the user clicks the Anticlockwise Rotate toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void rotateAnticlockwiseToolStripButton_Click(object sender, EventArgs e)
		{
			RotateSelection(-90);
		}

		/// <summary>
		///		Aligns the current selection to the bottom when the user clicks the Align Bottom toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void alignBottomToolStripButton_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Bottom);
		}

		/// <summary>
		///		Aligns the current selection to the top when the user clicks the Align Top toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void alignTopToolStripButton_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Top);
		}

		/// <summary>
		///		Aligns the current selection to the left when the user clicks the Align Left toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void alignLeftToolStripButton_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Left);
		}

		/// <summary>
		///		Aligns the current selection to the right when the user clicks the Align Right toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void alignRightToolStripButton_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Right);
		}

		/// <summary>
		///		Aligns the current selection to the center when the user clicks the Align Center toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void alignCenterToolStripButton_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Center);
		}

		/// <summary>
		///		Aligns the current selection to the middle when the user clicks the Align Middle toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void alignMiddleToolStripButton_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Middle);
		}

		/// <summary>
		///		Groups the current selection together when the user clicks the Group toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void groupToolStripButton_Click(object sender, EventArgs e)
		{
			Group();
		}

		/// <summary>
		///		Ungroups the current selection together when the user clicks the Ungroup toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void ungroupToolStripButton_Click(object sender, EventArgs e)
		{
			Ungroup();
		}

		/// <summary>
		///		Sends the current selection to the back of the map when the user clicks the Send To Back toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void sendToBackToolStripButton_Click(object sender, EventArgs e)
		{
			SendSelectionToBack();
		}

		/// <summary>
		///		Brings the current selection to the back of the map when the user clicks the Bring To Front toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void bringToFrontToolStripButton_Click(object sender, EventArgs e)
		{
			BringSelectionToFront();
		}

		/// <summary>
		///		Shifts the current selection backwards when the user clicks the Shift Backwards toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void shiftBackwardsToolStripButton_Click(object sender, EventArgs e)
		{
			ShiftSelectionBackwards();
		}

		/// <summary>
		///		Shifts the current selection forewards when the user clicks the Shift Forewards toolbar button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void shiftForewardsToolStripButton_Click(object sender, EventArgs e)
		{
			ShiftSelectionForewards();
		}

		/// <summary>
		///		Preforms an undo operations when the user clicks the Edit->Undo menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void undoMenuItem_Click(object sender, EventArgs e)
		{
			Undo();
		}

		/// <summary>
		///		Preforms an redo operations when the user clicks the Edit->Redo menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void redoMenuItem_Click(object sender, EventArgs e)
		{
			Redo();
		}

		/// <summary>
		///		Preforms a cut operations when the user clicks the Edit->Cut menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void cutMenuItem_Click(object sender, EventArgs e)
		{
			Cut();
		}

		/// <summary>
		///		Preforms a copy operations when the user clicks the Edit->Copy menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void copyMenuItem_Click(object sender, EventArgs e)
		{
			Copy();
		}

		/// <summary>
		///		Preforms a paste operations when the user clicks the Edit->Paste menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void pasteMenuItem_Click(object sender, EventArgs e)
		{
			Paste(true);
		}

		/// <summary>
		///		Deletes the current selection when the user clicks the Edit->Delete menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void deleteMenuItem_Click(object sender, EventArgs e)
		{
			Delete();
		}

		/// <summary>
		///		Duplicates the current selection when the user clicks the Edit->Duplicate menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void duplicateMenuItem_Click(object sender, EventArgs e)
		{
			Duplicate();
		}

		/// <summary>
		///		Selects all entitys on the map when the user clicks the Edit->Select All menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void selectAllMenuItem_Click(object sender, EventArgs e)
		{
			SelectAll();
		}

		/// <summary>
		///		Deselects the current selection when the user clicks the Edit->Deselect menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void deselectMenuItem_Click(object sender, EventArgs e)
		{
			Deselect();
		}

		/// <summary>
		///		Shows the asset manager window when the user clicks the View->Asset Manager menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void assetManagerMenuItem_Click(object sender, EventArgs e)
		{
			ShowAssetManager();
		}

		/// <summary>
		///		Shows the scene graph window when the user clicks the View->Scene Graph menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void sceneGraphViewerMenuItem_Click(object sender, EventArgs e)
		{
			ShowSceneGraph();
		}

		/// <summary>
		///		Rotates the current selection 90 degress clockwise when the user clicks the Format->Rotate->90 Clockwise menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void rotate90ClockwiseMenuItem_Click(object sender, EventArgs e)
		{
			RotateSelection(90);
		}

		/// <summary>
		///		Rotates the current selection 90 degress anticlockwise when the user clicks the Format->Rotate->90 Anitclockwise menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void rotate90AntiClockwiseMenuItem_Click(object sender, EventArgs e)
		{
			RotateSelection(-90);
		}

		/// <summary>
		///		Aligns the current selection to the bottom when the user clicks the Format->Align->Bottom menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void bottomMenuItem_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Bottom);
		}

		/// <summary>
		///		Aligns the current selection to the top when the user clicks the Format->Align->Top menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void topMenuItem_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Top);
		}

		/// <summary>
		///		Aligns the current selection to the left when the user clicks the Format->Align->Left menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void leftMenuItem_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Left);
		}

		/// <summary>
		///		Aligns the current selection to the right when the user clicks the Format->Align->Right menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void rightMenuItem_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Right);
		}

		/// <summary>
		///		Aligns the current selection to the center when the user clicks the Format->Align->Center menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void centerMenuItem_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Center);
		}

		/// <summary>
		///		Aligns the current selection to the middle when the user clicks the Format->Align->Middle menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void middleMenuItem_Click(object sender, EventArgs e)
		{
			AlignSelection(Alignment.Middle);
		}

		/// <summary>
		///		Preforms a horizontal selection flip when the user clicks the Format->Mirror->Horizontal menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void horizontalMenuItem_Click(object sender, EventArgs e)
		{
			FlipSelection(false);
		}

		/// <summary>
		///		Preforms a vertical selection flip when the user clicks the Format->Mirror->Vertical menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void verticalMenuItem_Click(object sender, EventArgs e)
		{
			FlipSelection(true);
		}

		/// <summary>
		///		Sends the current selection to the back of the map when the user clicks the Format->Order->Send To Back menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void sendToBackMenuItem_Click(object sender, EventArgs e)
		{
			SendSelectionToBack();
		}

		/// <summary>
		///		Sends the current selection to the front of the map when the user clicks the Format->Order->Send To Front menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void bringToFrontMenuItem_Click(object sender, EventArgs e)
		{
			BringSelectionToFront();
		}

		/// <summary>
		///		Shifts the current selection backwards when the user clicks the Format->Order->Shift Backwards menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void shiftBackwardsMenuItem_Click(object sender, EventArgs e)
		{
			ShiftSelectionBackwards();
		}

		/// <summary>
		///		Shifts the current selection forewards when the user clicks the Format->Order->Shift Forewards menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void shiftForewardsMenuItem_Click(object sender, EventArgs e)
		{
			ShiftSelectionForewards();
		}

		/// <summary>
		///		Groups the current selection together when the user clicks the Format->Group menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void groupMenuItem_Click(object sender, EventArgs e)
		{
			Group();
		}

		/// <summary>
		///		Ungroups the current selection together when the user clicks the Format->Ungroup menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void ungroupMenuItem_Click(object sender, EventArgs e)
		{
			Ungroup();
		}

		/// <summary>
		///		Closes all the sub windows when the user requests this window to be closed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void EditorWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			// If there are any unsaved changes give the user an option to save. 
			if (_mapChangedSinceSave == true)
			{
				DialogResult result = MessageBox.Show("This map has been modified since it was last saved, would you like to save now?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (result == DialogResult.Yes) SaveMap();
				if (result == DialogResult.Cancel)
				{
					e.Cancel = true;
					return;
				}
			}

			// Free all the child windows.
			if (_assetManagerWindow != null) _assetManagerWindow.Dispose();
            if (_consoleWindow != null) _consoleWindow.Dispose();
            if (_sceneGraphWindow != null) _sceneGraphWindow.Dispose();
            if (_entityPropertiesWindow != null) _entityPropertiesWindow.Dispose();
			//this.Dispose();
		}

		/// <summary>
		///		Preforms a cut operation when the user clicks the Context Menu->Cut menu button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void cutContextToolStripButton_Click(object sender, EventArgs e)
		{
			Cut();
		}

		/// <summary>
		///		Preforms a copy operation when the user clicks the Context Menu->Copy menu button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void copyContextToolStripButton_Click(object sender, EventArgs e)
		{
			Copy();
		}

		/// <summary>
		///		Preforms a paste operation when the user clicks the Context Menu->Paste menu button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void pasteContextToolStripButton_Click(object sender, EventArgs e)
		{
			Paste(false);
		}

		/// <summary>
		///		Deletes the currently selected object when the user clicks the Context Menu->Delete button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void deleteContextToolStripButton_Click(object sender, EventArgs e)
		{
			Delete();
		}

		/// <summary>
		///		Duplicates the currently selected object when the user clicks the Context Menu->Duplicate button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void duplicateContextToolStripButton_Click(object sender, EventArgs e)
		{
			Duplicate();
		}

		/// <summary>
		///		Inserts the current object into the map when the user clicks the Context Menu->Insert Object button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void insertContextToolStripButton_Click(object sender, EventArgs e)
		{
			InsertCurrentObject(false);
		}

        /// <summary>
        ///		Inserts the a new objectt into the map when the user clicks the Context Menu->Insert Entity button.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void insertEntityContextToolStripButton_Click(object sender, EventArgs e)
        {
            InsertCurrentObject(!(sender == insertEntityContextToolStripButton), "entity");
        }

		/// <summary>
		///		Inserts the current object into the map when the user clicks the Edit->insert object button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void insertObjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			InsertCurrentObject(!(sender == insertContextToolStripButton));
		}

		/// <summary>
		///		Locks and unlocks movement / sizing on the X-axis when the user checks the 
		///		X-axis tool strip button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void toolStripButtonXAxis_Click(object sender, EventArgs e)
		{
			_xAxisLocked = !_xAxisLocked;
			toolStripButtonXAxis.Checked = !_xAxisLocked;
		}

		/// <summary>
		///		Locks and unlocks movement / sizing on the Y-axis when the user checks the 
		///		Y-axis tool strip button.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void toolStripButtonYAxis_Click(object sender, EventArgs e)
		{
			_yAxisLocked = !_yAxisLocked;
			toolStripButtonYAxis.Checked = !_yAxisLocked;
		}

		/// <summary>
		///		Shows the entity properties window when the user clicks the Edit->Properties menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void propertyMenuItem_Click(object sender, EventArgs e)
		{
			ShowEntityProperties((EntityNode)_selectedEntityList[0]);
		}

		/// <summary>
		///		Shows the entity properties window when the user clicks the Properties context menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void propertyContextToolStripButton_Click(object sender, EventArgs e)
		{
			ShowEntityProperties((EntityNode)_selectedEntityList[0]);
		}

		/// <summary>
		///		Shows the the bounding boxs for all entitys when the user clicks the View Bounding Boxs menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void viewBoundingboxsMenuItem_Click(object sender, EventArgs e)
		{
			_viewBoundingBoxs = !_viewBoundingBoxs;
			viewBoundingboxsMenuItem.Checked = _viewBoundingBoxs;
			foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
				if (node as EntityNode != null && _selectedEntityList.Contains(node) == false) ((EntityNode)node).IsBoundingBoxVisible = _viewBoundingBoxs;
		}

        /// <summary>
        ///		Shows the the collision boxs for all entitys when the user clicks the View Collision Boxs menu item.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void viewCollisionboxsMenuItem_Click(object sender, EventArgs e)
        {
            _viewCollisionBoxs = !_viewCollisionBoxs;
            viewCollisionboxsMenuItem.Checked = _viewCollisionBoxs;
            foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
                if (node as EntityNode != null) ((EntityNode)node).IsCollisionBoxVisible = _viewCollisionBoxs;
        }

        /// <summary>
        ///		Shows the the event lines for all entitys when the user clicks the View Event Lines menu item.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void viewEventLinesMenuItem_Click(object sender, EventArgs e)
        {
            _viewEventLines = !_viewEventLines;
            viewEventLinesMenuItem.Checked = _viewEventLines;
            foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
                if (node as EntityNode != null) ((EntityNode)node).IsEventLinesVisible = _viewEventLines;
        }

        /// <summary>
        ///		Shows the the origin when the user clicks the View Origin menu item.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void viewOriginMenuItem_Click(object sender, EventArgs e)
        {
            _viewOrigin = !_viewOrigin;
            viewOriginMenuItem.Checked = _viewOrigin;
        }

		/// <summary>
		///		Resets the position and zoom of the camera when the user clicks the View->Reset Camera menu item.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void resetCameraMenuItem_Click(object sender, EventArgs e)
		{
			Editor.GlobalInstance.CameraNode.Position(0, 0, Editor.GlobalInstance.CameraNode.Transformation.Z);
			Editor.GlobalInstance.CameraNode.Zoom = 1.0f;
            zoomStatusLabel.Text = "Zoom: " + ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
            zoomToolStripComboBox.Text = (Editor.GlobalInstance.CameraNode.Zoom * 100.0f) + "%";
		}

		/// <summary>
		///		Toggles if the next tiles placed should be flipped horizontaly 
		///		or not, when the tile flip x button is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void tileFlipXToolStripButton_Click(object sender, EventArgs e)
		{
			_flipTilesX = !_flipTilesX;
			tileFlipXToolStripButton.Checked = _flipTilesX;
		}

		/// <summary>
		/// 	Toggles if the next tiles placed should be flipped verticaly 
		///		or not, when the tile flip y button is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void tileFlipYToolStripButton_Click(object sender, EventArgs e)
		{
			_flipTilesY = !_flipTilesY;
			tileFlipYToolStripButton.Checked = _flipTilesY;
		}

		/// <summary>
		///		Toggles if the next tiles placed should be flipped horizontaly 
		///		or not, when the tile flip x menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void flipTileXMenuItem_Click(object sender, EventArgs e)
		{
			_flipTilesX = !_flipTilesX;
			flipTileXMenuItem.Checked = _flipTilesX;
		}

		/// <summary>
		/// 	Toggles if the next tiles placed should be flipped verticaly 
		///		or not, when the tile flip y menu is clicked.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void flipTileYMenuItem_Click(object sender, EventArgs e)
		{
			_flipTilesY = !_flipTilesY;
			flipTileYMenuItem.Checked = _flipTilesY;
		}

        /// <summary>
        ///     Called when the file toolbar menut item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void fileToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileToolStrip.Visible = !fileToolStrip.Visible;
            fileToolbarToolStripMenuItem.Checked = fileToolStrip.Visible;
        }

        /// <summary>
        ///     Called when the format toolbar menut item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void formatToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatToolStrip.Visible = !formatToolStrip.Visible;
            formatToolbarToolStripMenuItem.Checked = formatToolStrip.Visible;
        }

        /// <summary>
        ///     Called when the tools toolbar menut item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void toolsToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolsToolStrip.Visible = !toolsToolStrip.Visible;
            toolsToolbarToolStripMenuItem.Checked = toolsToolStrip.Visible;
        }

        /// <summary>
        ///     Called when the command toolbar menut item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void commandToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            commandToolStrip.Visible = !commandToolStrip.Visible;
            commandToolbarToolStripMenuItem.Checked = commandToolStrip.Visible;
        }

        /// <summary>
        ///     Inserts an entity when the insert entity menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void insertEntityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCurrentObject(false, "entity");
        }

        /// <summary>
        ///     Inserts a tilemap segment when the insert tilemap menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void insertTilemapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InsertCurrentObject(!(sender == insertTilemapContextToolStripMenuItem), "tilemap segment");
        }

        /// <summary>
        ///     Inserts a path marker when the insert path marker menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void insertPathMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCurrentObject(!(sender == insertPathMarkerContextToolStripMenuItem), "path marker");
        }

        /// <summary>
        ///     Inserts an emitter when the insert emitter menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void insertEmitterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InsertCurrentObject(!(sender == insertEmitterContextToolStripMenuItem), "emitter");
        }

        /// <summary>
        ///     Opens the effects designer when the user clicks the toolbar icon.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void emitterDesignerToolStripButton_Click(object sender, EventArgs e)
        {
            ShowEffectsDesigner();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renderMapMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This function has not been implemented yet!", "Sorry");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomToolStripComboBox_TextChanged(object sender, EventArgs e)
        {
            float result = 0;
            float originalValue = Editor.GlobalInstance.CameraNode.Zoom;
            string newValue = !zoomToolStripComboBox.Text.EndsWith("%") ? zoomToolStripComboBox.Text : zoomToolStripComboBox.Text.Substring(0, zoomToolStripComboBox.Text.Length - 1);

            Editor.GlobalInstance.CameraNode.Zoom = Math.Max(0.25f, Math.Min(10.0f, float.TryParse(newValue, out result) ? (float.Parse(newValue) / 100.0f) : 1.0f));
            if (Editor.GlobalInstance.CameraNode.Zoom == originalValue)
                return;
            
            zoomStatusLabel.Text = "Zoom: " + ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
            zoomToolStripComboBox.Text = (Editor.GlobalInstance.CameraNode.Zoom * 100.0f) + "%";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            Editor.GlobalInstance.CameraNode.Zoom -= Editor.GlobalInstance.CameraNode.Zoom > 0.25f ? 0.25f : 0;
            zoomStatusLabel.Text = "Zoom: " + ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
            zoomToolStripComboBox.Text = ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomInToolStripButton_Click(object sender, EventArgs e)
        {

            float zoom = Editor.GlobalInstance.CameraNode.Zoom;
            Editor.GlobalInstance.CameraNode.Zoom += Editor.GlobalInstance.CameraNode.Zoom < 10.0f ? 0.25f : 0;
            zoomStatusLabel.Text = "Zoom: " + ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
            zoomToolStripComboBox.Text = ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetCameraToolStripButton_Click(object sender, EventArgs e)
        {
            Editor.GlobalInstance.CameraNode.Position(0, 0, Editor.GlobalInstance.CameraNode.Transformation.Z);
            Editor.GlobalInstance.CameraNode.Zoom = 1.0f;
            zoomStatusLabel.Text = "Zoom: " + ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
            zoomToolStripComboBox.Text = (Editor.GlobalInstance.CameraNode.Zoom * 100.0f) + "%";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor.GlobalInstance.CameraNode.Zoom -= Editor.GlobalInstance.CameraNode.Zoom > 0.25f ? 0.25f : 0;
            zoomStatusLabel.Text = "Zoom: " + ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
            zoomToolStripComboBox.Text = ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor.GlobalInstance.CameraNode.Zoom += Editor.GlobalInstance.CameraNode.Zoom < 10.0f ? 0.25f : 0;
            zoomStatusLabel.Text = "Zoom: " + ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
            zoomToolStripComboBox.Text = ((int)(Editor.GlobalInstance.CameraNode.Zoom * 100.0f)) + "%";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cameraToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cameraToolStrip.Visible = !cameraToolStrip.Visible;
            cameraToolbarToolStripMenuItem.Checked = cameraToolStrip.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.png;*.tga;*.bmp";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _backgroundImage = GraphicsManager.LoadImage(dialog.FileName, 0);
                Editor.GlobalInstance.CameraNode.BackgroundImage = _backgroundImage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _backgroundImage = null;
            Editor.GlobalInstance.CameraNode.BackgroundImage = new Graphics.Image(ReflectionMethods.GetEmbeddedResourceStream("grid.png"), 0);
        }

		/// <summary>
		///		Undo's the last operation preformed by the user.
		/// </summary>
		private void Undo()
		{
			_mapChangedSinceSave = true;
            UndoOperation operation = _undoStack.Pop() as UndoOperation;
            operation.Undo();
            PushRedoOperation(operation);

            Fusion.Engine.Engine.GlobalInstance.Map.SceneGraph.SyncronizeNodes();
			_mapChangedSinceSave = true;
            if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();

            // Update undo / redo menus.
            SyncronizeWindow();
        }

		/// <summary>
		///		Redo's the last undone operation preformed by the user.
		/// </summary>
		private void Redo()
		{
            _mapChangedSinceSave = true;
            UndoOperation operation = _redoStack.Pop() as UndoOperation;
            operation.Redo();
            PushUndoOperation(operation);

            Fusion.Engine.Engine.GlobalInstance.Map.SceneGraph.SyncronizeNodes();
			_mapChangedSinceSave = true;
            if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();

            // Update undo / redo menus.
            SyncronizeWindow();
		}

        /// <summary>
        ///     Pushs a new undo operation onto the undo stack.
        /// </summary>
        /// <param name="operation">Operation to push onto stack.</param>
        private void PushUndoOperation(UndoOperation operation)
        {
            _undoStack.Push(operation);
        }

        /// <summary>
        ///     Pushs a new reddo operation onto the reddo stack.
        /// </summary>
        /// <param name="operation">Operation to push onto stack.</param>
        private void PushRedoOperation(UndoOperation operation)
        {
            _redoStack.Push(operation);
        }

		/// <summary>
		///		Cuts the current selection out and places it on the clipboard for later use.
		/// </summary>
		private void Cut()
		{
			_objectClipboard.Clear();
			foreach (EntityNode entity in _selectedEntityList)
			{
				if (entity.Parent != null) entity.Parent.RemoveChild(entity);
				_objectClipboard.Add(entity);
			}
			_objectClipboard.Reverse();
			ClearSelection();
			SyncronizeWindow();
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Copys the current selection onto the clipboard for later use.
		/// </summary>
		private void Copy()
		{
			_objectClipboard.Clear();
			foreach (EntityNode entity in _selectedEntityList)
				_objectClipboard.Add(entity);
			_objectClipboard.Reverse();
			SyncronizeWindow();
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Pastes the current contents of the clipboard onto the map.
		/// </summary>
		/// <param name="inCenter">If set to true the objects will be pasted into the center of the map</param>
		private void Paste(bool inCenter)
		{
			int mapX, mapY;
			if (inCenter == false)
			{
				mapX = (int)((mousePositionBeforeRightClick[0] / Editor.GlobalInstance.CameraNode.Zoom) - (int)Editor.GlobalInstance.CameraNode.Transformation.X);
				mapY = (int)((mousePositionBeforeRightClick[1] / Editor.GlobalInstance.CameraNode.Zoom) - (int)Editor.GlobalInstance.CameraNode.Transformation.Y);
			}
			else
			{
				mapX = (int)((mapPanel.ClientSize.Width / 2) / Editor.GlobalInstance.CameraNode.Zoom) - (int)Editor.GlobalInstance.CameraNode.Transformation.X;
				mapY = (int)((mapPanel.ClientSize.Height / 2) / Editor.GlobalInstance.CameraNode.Zoom) - (int)Editor.GlobalInstance.CameraNode.Transformation.Y;
			}

			if (_snapToGrid == true)
			{
				mapX = (mapX / _gridWidth) * _gridWidth;
				mapY = (mapY / _gridHeight) * _gridHeight;
			}

			// Find the lowest X / Y values which we will use to place the 
			// objects relative to the pasting position.
			int minX = -999, minY = -999;
			foreach (EntityNode entity in _objectClipboard)
			{
				if (entity.Transformation.X < minX || minX == -999) minX = (int)entity.Transformation.X;
				if (entity.Transformation.X < minY || minY == -999) minY = (int)entity.Transformation.Y;
			}

			ClearSelection();
			foreach (EntityNode entity in _objectClipboard)
			{
				EntityNode clone = entity.Clone() as EntityNode;
				if (clone.Parent != null) clone.Parent.RemoveChild(clone);

				foreach (SceneNode subNode in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
				{
					EntityNode subEntityNode = subNode as EntityNode;
					if (subEntityNode == null) continue;
					if (subEntityNode.Name.ToLower() == clone.Event.ToLower()) clone.EventNodes.Add(subEntityNode);
					if (clone.Name.ToLower() == subEntityNode.Event.ToLower()) subEntityNode.EventNodes.Add(clone);				
				}

				Engine.Engine.GlobalInstance.Map.SceneGraph.RootNode.AddChild(clone);
				clone.Position(mapX - (clone.Transformation.X - minX), mapY - (clone.Transformation.Y - minY), clone.Transformation.Z);
				AddEntityToSelection(clone, true);
			}
			if (_entityPropertiesWindow != null) _entityPropertiesWindow.Entity = (EntityNode)_selectedEntityList[0];
			//_objectClipboard.Clear();
			_mapChangedSinceSave = true;
            if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();
		}

		/// <summary>
		///		Deletes the current selection.
		/// </summary>
		private void Delete()
		{
            SceneNode[] nodes = (SceneNode[])_selectedEntityList.ToArray(typeof(SceneNode));
            PushUndoOperation(new DeleteNodesUndoOperation(nodes));
            
            foreach (EntityNode entity in _selectedEntityList)
				entity.Parent.RemoveChild(entity);
			ClearSelection();
			SyncronizeWindow();
            Fusion.Engine.Engine.GlobalInstance.Map.SceneGraph.SyncronizeNodes();
			_mapChangedSinceSave = true;
            if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();
		}

		/// <summary>
		///		Duplicates the current selection.
		/// </summary>
		private void Duplicate()
		{
			ArrayList newList = new ArrayList();
			foreach (EntityNode entity in _selectedEntityList)
			{
				EntityNode newEntity = entity.Clone() as EntityNode;
				newEntity.Move(16, 16, 0);
				newList.Add(newEntity);

				entity.IsSizingPointsVisible = entity.IsBoundingBoxVisible = false;
				newEntity.IsSizingPointsVisible = newEntity.IsBoundingBoxVisible = true;

				foreach (SceneNode subNode in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
				{
					EntityNode subEntityNode = subNode as EntityNode;
					if (subEntityNode == null) continue;
					if (subEntityNode.Name.ToLower() == newEntity.Event.ToLower()) newEntity.EventNodes.Add(subEntityNode);
					if (newEntity.Name.ToLower() == subEntityNode.Event.ToLower()) subEntityNode.EventNodes.Add(newEntity);
				}
			}
			ClearSelection();
			_selectedEntityList = newList;
			_mapChangedSinceSave = true;
            if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();
            PushUndoOperation(new DuplicateNodesUndoOperation((SceneNode[])newList.ToArray(typeof(SceneNode))));
            SyncronizeWindow();
		}

		/// <summary>
		///		Selects all the entitys on the current map.
		/// </summary>
		private void SelectAll()
		{
			ClearSelection();
			foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
			{
				EntityNode entity = node as EntityNode;
				if (entity == null) continue;

				AddEntityToSelection(entity);
			}
			SyncronizeWindow();
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Deselects the current selection.
		/// </summary>
		private void Deselect()
		{
			ClearSelection();
			SyncronizeWindow();
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Called by the asset maneger window when the selected object is changed.
		/// </summary>
		/// <param name="sender">Object that caused this event to be triggered.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void ObjectChanged(object sender, EventArgs e)
		{
			SyncronizeWindow();
		}

		/// <summary>
		///		Shows the asset manager window.
		/// </summary>
		private void ShowAssetManager()
		{
			if (_assetManagerWindow == null || _assetManagerWindow.IsDisposed == true)
			{
				_assetManagerWindow = new AssetManagerWindow();
				_assetManagerWindow.Show(this);
				_assetManagerWindow.ObjectChanged += new EventHandler(ObjectChanged);
				_assetManagerWindow.ScriptModified += new EventHandler(ScriptModified);
				_assetManagerWindow.TilesetSelectionChanged += new EventHandler(TilesetSelectionChanged);
				SyncronizeWindow();
			}
			else
			{
				if (_assetManagerWindow.Visible == false)
					_assetManagerWindow.Show(this);
				_assetManagerWindow.Focus();
			}
		}

        /// <summary>
        ///		Shows the effects manager.
        /// </summary>
        private void ShowEffectsDesigner()
        {
            if (_effectsDesignerWindow == null || _effectsDesignerWindow.IsDisposed == true)
            {
                _effectsDesignerWindow = new EmitterEditorWindow();
                _effectsDesignerWindow.Show(this);
            }
            else
            {
                if (_effectsDesignerWindow.Visible == false)
                    _effectsDesignerWindow.Show(this);
                _effectsDesignerWindow.Focus();
            }
        }

		/// <summary>
		///		Called by the asset manager whenever the tileset selection is modified.
		/// </summary>
		/// <param name="sender">The script window that invoked this event.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void TilesetSelectionChanged(object sender, EventArgs e)
		{
			SyncronizeWindow();
		}

		/// <summary>
		///		Called by the asset manager whenever a script is modified.
		/// </summary>
		/// <param name="sender">The script window that invoked this event.</param>
		/// <param name="e">Arguments explaining why this event occured.</param>
		private void ScriptModified(object sender, EventArgs e)
		{
			ScriptEditorWindow editorWindow = sender as ScriptEditorWindow;

			// Compile the script.
			ScriptCompiler compiler = new ScriptCompiler();
			bool errorOccured = false;
			string errorDescription = "";
			if (compiler.Compile(editorWindow.Url) > 0)
				foreach (CompileError error in compiler.ErrorList)
					if (error.AlertLevel == ErrorAlertLevel.Error || error.AlertLevel == ErrorAlertLevel.FatalError)
					{
						errorDescription += (errorDescription == "" ? "" : "\n") + error.ToString();
						errorOccured = true;
					}

			// If an error occured notify the user if not then 
			// insert a new scripted entity into the map.
			if (errorOccured == true)
			{
				MessageBox.Show("Unable to recompile object's script, the following error(s) occured while attempt to compile object's script.\n\n" + errorDescription, "Compile Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				DebugLogger.WriteLog("Object's script recompile aborted. The following error(s) occured while attempting to compile object's script.\n\n" + errorDescription, LogAlertLevel.Warning);
				return;
			}

			foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
			{
				if (node is ScriptedEntityNode == false) continue;
				ScriptedEntityNode scriptEntityNode = node as ScriptedEntityNode;
				if (scriptEntityNode.ScriptProcess.Url.ToLower() != ((string)editorWindow.Url).ToLower()) continue;

				// Dump the compiled code into a memory stream.
				MemoryStream memStream = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(memStream);
				BinaryReader reader = new BinaryReader(memStream);
				compiler.DumpExecutableFile(writer);
				memStream.Position = 0;

				// Store current values of properties so we can set them in the 
				// new script later.
				Hashtable propertyValueTable = new Hashtable();
				foreach (Symbol symbol in scriptEntityNode.ScriptProcess.GlobalScope.Symbols)
				{
					if (symbol is VariableSymbol == false || ((VariableSymbol)symbol).IsProperty == false) continue;
					VariableSymbol propertySymbol = symbol as VariableSymbol;
					propertyValueTable.Add(propertySymbol.Identifier, propertySymbol);
				}

				// Create a new scripted entity with the given script attached to it.
				ScriptProcess previousScript = scriptEntityNode.ScriptProcess;
				scriptEntityNode.ScriptProcess = new ScriptProcess(null, reader);
				scriptEntityNode.ScriptProcess.Url = previousScript.Url;

				// Free up the streams.
				memStream.Close();
				reader.Close();
				writer.Close();

				// Go through each property in this process and set its value to
				// the old one (if it exists).
				foreach (Symbol symbol in scriptEntityNode.ScriptProcess.GlobalScope.Symbols)
				{
					if (symbol is VariableSymbol == false || ((VariableSymbol)symbol).IsProperty == false) continue;
					VariableSymbol propertySymbol = symbol as VariableSymbol;
					if (propertyValueTable.Contains(propertySymbol.Identifier) == true)
					{
						VariableSymbol previousPropertySymbol = (VariableSymbol)propertyValueTable[propertySymbol.Identifier];
						if (previousPropertySymbol.DataType != propertySymbol.DataType) continue;
						switch (propertySymbol.DataType.DataType)
						{
							case DataType.Bool:
								scriptEntityNode.ScriptProcess[0].SetGlobalVariable(propertySymbol.Identifier, previousScript[0].GetBooleanGlobal(propertySymbol.Identifier));
								break;
							case DataType.Byte:
								scriptEntityNode.ScriptProcess[0].SetGlobalVariable(propertySymbol.Identifier, previousScript[0].GetByteGlobal(propertySymbol.Identifier));
								break;
							case DataType.Double:
								scriptEntityNode.ScriptProcess[0].SetGlobalVariable(propertySymbol.Identifier, previousScript[0].GetDoubleGlobal(propertySymbol.Identifier));
								break;
							case DataType.Float:
								scriptEntityNode.ScriptProcess[0].SetGlobalVariable(propertySymbol.Identifier, previousScript[0].GetFloatGlobal(propertySymbol.Identifier));
								break;
							case DataType.Int:
								scriptEntityNode.ScriptProcess[0].SetGlobalVariable(propertySymbol.Identifier, previousScript[0].GetIntegerGlobal(propertySymbol.Identifier));
								break;
							case DataType.Long:
								scriptEntityNode.ScriptProcess[0].SetGlobalVariable(propertySymbol.Identifier, previousScript[0].GetLongGlobal(propertySymbol.Identifier));
								break;
							case DataType.Short:
								scriptEntityNode.ScriptProcess[0].SetGlobalVariable(propertySymbol.Identifier, previousScript[0].GetShortGlobal(propertySymbol.Identifier));
								break;
							case DataType.String:
								scriptEntityNode.ScriptProcess[0].SetGlobalVariable(propertySymbol.Identifier, previousScript[0].GetStringGlobal(propertySymbol.Identifier));
								break;
						}
					}
				}

				if (_entityPropertiesWindow != null && _entityPropertiesWindow.Entity == node)
					_entityPropertiesWindow.SyncronizeData();
			}
		}

		/// <summary>
		///		Shows the scene graph viewer window.
		/// </summary>
		private void ShowSceneGraph()
		{
			if (_sceneGraphWindow == null || _sceneGraphWindow.IsDisposed == true)
			{
				_sceneGraphWindow = new SceneGraphWindow(Engine.Engine.GlobalInstance.Map.SceneGraph);
				_sceneGraphWindow.Show(this);
			}
			else
			{
				if (_sceneGraphWindow.Visible == false)
					_sceneGraphWindow.Show(this);
				_sceneGraphWindow.Focus();
			}
		}

		/// <summary>
		///		Shows the entity properties window.
		/// </summary>
		private void ShowEntityProperties(EntityNode entity)
		{
			if (_entityPropertiesWindow == null || _entityPropertiesWindow.IsDisposed == true)
			{
				_entityPropertiesWindow = new EntityPropertiesWindow(entity);
				_entityPropertiesWindow.Show(this);
				_entityPropertiesWindow.PropertyChangedDelegate += new PropertyChangedEventHandler(EntityPropertiesWindow_PropertyChanged);
				_supressSelection = true;
				_supressSelectionTimer.Restart();
			}
			else
			{
				if (_entityPropertiesWindow.Visible == false)
				{
					_entityPropertiesWindow.Show(this);
					_supressSelection = true;
					_supressSelectionTimer.Restart();
				}
				_entityPropertiesWindow.Focus();
				_entityPropertiesWindow.Entity = entity;
			}
		}

		/// <summary>
		///		Called when the user modifys a property shown in the entity properties window.
		/// </summary>
    /// <param name="source">Instance of EntityPropertyWindow that event originated from.</param>
    /// <param name="propertyName">Name of property that was changed.</param>
    /// <param name="value">Value that property was changed to.</param>
		private void EntityPropertiesWindow_PropertyChanged(object source, string name, object value)
		{
			EntityNode entityNode = (source as EntityPropertiesWindow).Entity;
			
			// If the name of an entity has been changed then update the scene graph window.
			if (name.ToLower() == "name")
			{
				if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();
				foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
				{
					EntityNode subNode = node as EntityNode;
					if (subNode == null) continue;
					if (subNode.EventNodes.Contains(entityNode) == true) subNode.EventNodes.Remove(entityNode);
					if (subNode.Event.ToLower() == value.ToString().ToLower()) 
						subNode.EventNodes.Add(entityNode);
				}
			}

			// If the event has been modified then gather all the nodes relating to this event
			// and add them to this modified entitys event node list.
			else if (name.ToLower() == "event")
			{
				entityNode.EventNodes.Clear();
				foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
				{
					if (node is EntityNode == false) continue;
					if (((EntityNode)node).Name.ToLower() == value.ToString().ToLower())
						entityNode.EventNodes.Add(node);
				}
			}
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Rotates the current selection clockwise by angle.
		/// </summary>
		/// <param name="angle">Angle of rotation.</param>
		private void RotateSelection(int angle)
		{
			foreach (EntityNode entity in _selectedEntityList)
				entity.Turn(0,0,angle);
            if (_selectedEntityList.Count == 1 && _entityPropertiesWindow != null && _entityPropertiesWindow.Visible == true && _entityPropertiesWindow.Entity != null)
                _entityPropertiesWindow.UpdateProperty("angle", new Vector(_entityPropertiesWindow.Entity.Transformation.AngleX, _entityPropertiesWindow.Entity.Transformation.AngleY, _entityPropertiesWindow.Entity.Transformation.AngleZ));
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Aligns the selection based on the alignment passed.
		/// </summary>
		/// <param name="alignment">Position to align selectiot to.</param>
		private void AlignSelection(Alignment alignment)
		{
			float minX = -999, minY = -999, maxX = -999, maxY = -999;
			if (_selectedEntityList.Count > 1)
			{
				foreach (EntityNode entity in _selectedEntityList)
				{
					if (entity.Transformation.X < minX || minX == -999) minX = entity.Transformation.X;
					if (entity.Transformation.X > maxX || maxX == -999) maxX = entity.Transformation.X;
					if (entity.Transformation.Y < minY || minY == -999) minY = entity.Transformation.Y;
					if (entity.Transformation.Y > maxY || maxY == -999) maxY = entity.Transformation.Y;
				}
			}
			else
			{
				minX = -Editor.GlobalInstance.CameraNode.Transformation.X;
				minY = -Editor.GlobalInstance.CameraNode.Transformation.Y;
				maxX = minX + GraphicsManager.RenderTarget.Width;
				maxY = minY + GraphicsManager.RenderTarget.Height;
			}
			foreach (EntityNode entity in _selectedEntityList)
			{
				switch (alignment)
				{
					case Alignment.Bottom:
						entity.Position(entity.Transformation.X, maxY - entity.BoundingRectangle.Height, entity.Transformation.Z);
						break;
					case Alignment.Center:
						entity.Position((minX + ((maxX - minX) / 2)) - (entity.BoundingRectangle.Width / 2), entity.Transformation.Y, entity.Transformation.Z);
						break;
					case Alignment.Left:
						entity.Position(minX, entity.Transformation.Y, entity.Transformation.Z);
						break;
					case Alignment.Middle:
						entity.Position(entity.Transformation.X, (minY + ((maxY - minY) / 2)) - (entity.BoundingRectangle.Height / 2), entity.Transformation.Z);
						break;
					case Alignment.Right:
						entity.Position(maxX - entity.BoundingRectangle.Width, entity.Transformation.Y, entity.Transformation.Z);
						break;
					case Alignment.Top:
						entity.Position(entity.Transformation.X, minY, entity.Transformation.Z);
						break;
				}
				if (_snapToGrid == true)
					entity.Position(((int)entity.Transformation.X / _gridWidth) * _gridWidth, ((int)entity.Transformation.Y / _gridHeight) * _gridHeight, entity.Transformation.Z);
			}
            if (_selectedEntityList.Count == 1 && _entityPropertiesWindow != null && _entityPropertiesWindow.Visible == true && _entityPropertiesWindow.Entity != null)
                _entityPropertiesWindow.UpdateProperty("location", new Point((int)_entityPropertiesWindow.Entity.Transformation.X, (int)_entityPropertiesWindow.Entity.Transformation.Y));
                   
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Flips the current selection horizontaly or verticaly if vertical is true.
		/// </summary>
		/// <param name="vertical">If set to true the flip will be vertical not horizontal</param>
		private void FlipSelection(bool vertical)
		{
			foreach (EntityNode entity in _selectedEntityList)
				if (vertical == true)
					entity.Scale(entity.Transformation.ScaleX, -entity.Transformation.ScaleY, 1);
				else
					entity.Scale(-entity.Transformation.ScaleX, entity.Transformation.ScaleY, 1);
            if (_selectedEntityList.Count == 1 && _entityPropertiesWindow != null && _entityPropertiesWindow.Visible == true && _entityPropertiesWindow.Entity != null)
                _entityPropertiesWindow.UpdateProperty("scale", new Vector(_entityPropertiesWindow.Entity.Transformation.ScaleX, _entityPropertiesWindow.Entity.Transformation.ScaleY, _entityPropertiesWindow.Entity.Transformation.ScaleZ));
                   
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Sends the selection to the back of the map.
		/// </summary>
		private void SendSelectionToBack()
		{
			foreach (EntityNode entity in _selectedEntityList)
				entity.Parent.SendChildToBack(entity);
			if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Brings the selection to the front of the map.
		/// </summary>
		private void BringSelectionToFront()
		{
			foreach (EntityNode entity in _selectedEntityList)
				entity.Parent.BringChildToFront(entity);
			if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Shifts the current selection backwards.
		/// </summary>
		private void ShiftSelectionBackwards()
		{
			foreach (EntityNode entity in _selectedEntityList)
				entity.Parent.ShiftChildBackwards(entity);
			if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Shifts the current selection forewards.
		/// </summary>
		private void ShiftSelectionForewards()
		{
			foreach (EntityNode entity in _selectedEntityList)
				entity.Parent.ShiftChildForewards(entity);
			if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Adds the given entity to the selection.
		/// </summary>
		/// <param name="node">Entity node to add.</param>
		/// <param name="doNotShowProperties">IF set to true the entitsy properties will not be shown in the entity properties window.</param>
		private void AddEntityToSelection(EntityNode node, bool doNotShowProperties)
		{
			if (_supressSelection == true) return;
			_selectedEntityList.Add(node);
			node.IsBoundingBoxVisible = true;
			node.IsSizingPointsVisible = true;
			if (doNotShowProperties == false && _selectedEntityList.Count == 1 && _entityPropertiesWindow != null && _entityPropertiesWindow.Visible == true) ShowEntityProperties((EntityNode)_selectedEntityList[0]);
			SyncronizeWindow();
		}
		private void AddEntityToSelection(EntityNode node)
		{
			AddEntityToSelection(node, false);
		}

		/// <summary>
		///		Removes the given entity from the selection.
		/// </summary>
		/// <param name="node">Entity node to remove.</param>
		private void RemoveEntityFromSelection(EntityNode node)
		{
			if (_supressSelection == true) return;
			_selectedEntityList.Remove(node);
			node.IsBoundingBoxVisible = _viewBoundingBoxs;
            node.IsCollisionBoxVisible = _viewCollisionBoxs;
            node.IsEventLinesVisible = _viewEventLines;
			node.IsSizingPointsVisible = false;
			if (_entityPropertiesWindow != null && node == _entityPropertiesWindow.Entity)
			{
				_entityPropertiesWindow.Entity = null;
				if (_selectedEntityList.Count == 1 && _entityPropertiesWindow != null && _entityPropertiesWindow.Visible == true) ShowEntityProperties((EntityNode)_selectedEntityList[0]);
			}
			SyncronizeWindow();
		}

		/// <summary>
		///		Clears all entitys from the current selection.
		/// </summary>
		private void ClearSelection()
		{
			if (_supressSelection == true) return;
			foreach (EntityNode entity in _selectedEntityList)
			{
				if (_entityPropertiesWindow != null && entity == _entityPropertiesWindow.Entity && _entityPropertiesWindow.Visible == true) ShowEntityProperties(null);
				entity.IsBoundingBoxVisible = _viewBoundingBoxs;
                entity.IsCollisionBoxVisible = _viewCollisionBoxs;
                entity.IsEventLinesVisible = _viewEventLines;
				entity.IsSizingPointsVisible = false;
			}
			_selectedEntityList.Clear();
			SyncronizeWindow();
		}

		/// <summary>
		///		Groups all the selected entitys together.
		/// </summary>
		private void Group()
		{
			// Create a new group node to place these nodes into.
			GroupNode groupSceneNode = new GroupNode("Group");
			Engine.Engine.GlobalInstance.Map.SceneGraph.RootNode.AddChild(groupSceneNode);
		
			// Add all scene nodes to this group node.
			foreach (EntityNode entity in _selectedEntityList)
			{
				if (entity.Parent != null) entity.Parent.RemoveChild(entity);
				groupSceneNode.AddChild(entity);
			}

			if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Ungroups the selected entitys.
		/// </summary>
		private void Ungroup()
		{
			ArrayList destroyedGroups = new ArrayList();
			foreach (EntityNode entity in _selectedEntityList)
			{
				if (entity.Parent == null || entity.Parent as GroupNode == null || destroyedGroups.Contains(entity.Parent) == true) 
					continue;
				
				if (entity.Parent.Children.Count - 1 <= 1 && entity.Parent.Parent != null)
				{
					entity.Parent.Parent.RemoveChild(entity.Parent);
					destroyedGroups.Add(entity.Parent);
					foreach(SceneNode node in entity.Parent.Children)
					{
						entity.Parent.RemoveChild(node);
						Engine.Engine.GlobalInstance.Map.SceneGraph.RootNode.AddChild(node);
					}
					continue;
				}
				
				entity.Parent.RemoveChild(entity);
				Engine.Engine.GlobalInstance.Map.SceneGraph.RootNode.AddChild(entity);
			}
			if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();
			_mapChangedSinceSave = true;
		}

		/// <summary>
		///		Changes the current tool used to edit the map to the given one and updates
		///		the tool pallete and menu.
		/// </summary>
		private void ChangeTool(Tools tool)
		{
			_currentTool = tool;
			ToolStripButton[] toolButtons = new ToolStripButton[]
			{
				cameraToolStripButton,
				selectorToolStripButton,
				pencilToolStripButton,
				eraserToolStripButton,
				paintBucketToolStripButton,
				tilePickerToolStripButton,
				rectangleToolStripButton,
				roundedRectangleToolStripButton,
				ellipseToolStripButton,
				lineToolStripButton
			};
			ToolStripMenuItem[] toolMenus = new ToolStripMenuItem[]
			{
				cameraMenuItem,
				selectorMenuItem,
				pencilMenuItem,
				eraserMenuItem,
				paintBucketMenuItem,
				tilePickerMenuItem,
				rectangleMenuItem,
				roundedRectangleMenuItem,
				ellipseMenuItem,
				lineMenuItem,
			};
			for (int i = 0; i < toolButtons.Length; i++)
			{
				toolButtons[i].Checked = ((int)tool) == i ? true : false;
				toolMenus[i].Checked = ((int)tool) == i ? true : false;	
			}
			SyncronizeWindow();
		}

		/// <summary>
		///		Shows the map properties window and sets the map properties to thoses
		///		selected by the user if the dialog result is Ok.
		/// </summary>
		private void ShowMapProperties()
		{
			MapPropertiesWindow window = new MapPropertiesWindow(Engine.Engine.GlobalInstance.Map.MapProperties);
			if (window.ShowDialog() == DialogResult.OK)
			{
				Engine.Engine.GlobalInstance.Map.MapProperties = window.MapProperties;
				_mapChangedSinceSave = true;
			}
		}

		/// <summary>
		///		Shows the debugging console or sets focus to it if its already been created.
		/// </summary>
		private void ShowConsole()
		{
			if (_consoleWindow == null || _consoleWindow.IsDisposed == true)
			{
				_consoleWindow = new ConsoleWindow();
				_consoleWindow.Show((Form)this);
			}
			else
			{
				if (_consoleWindow.Visible == false)
					_consoleWindow.Show((Form)this);	
				_consoleWindow.Focus();
			}
		}

		/// <summary>
		///		Opens up internet explorer and directs it to the binary phoenix website.
		/// </summary>
		private void VisitWebsite()
		{
			// Boot up internet explorer to the binary phoenix page.
			Process process = new Process();
			process.StartInfo.FileName = @"iexplore";
			process.StartInfo.Arguments = "www.binaryphoenix.com";
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			process.Start();
		}

		/// <summary>
		///		Executes the help chm file.
		/// </summary>
		private void ShowHelp()
		{
			if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\help.chm") == false)
			{
				MessageBox.Show("Unable to locate help file, please make sure that the file \"Help.chm\" is inside the same directory as this executable.", "Unable to locate file");
				return;
			}

			// Boot up the help page.
			Process process = new Process();
			process.StartInfo.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
			process.StartInfo.FileName = "help.chm"; 
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			process.Start();
		}

		/// <summary>
		///		Executes the fusion updater executable to updater and requests it to update Fusion.
		/// </summary>
		private void CheckForUpdates()
		{
			if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\updater.exe") == false)
			{
				MessageBox.Show("Unable to locate updater executable, please make sure that the file \"Updater.exe\" is inside the same directory as this executable.", "Unable to locate file");
				return;
			}

			// Check that the user dosen't mind closing this program down while 
			// the update is run.
			if (MessageBox.Show("The editor needs to close before updating, do you wish to continue?", "Close Required", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
				return;

			// Close the editor.
			Close();

			// Boot up the updater executable with the correct 
			// command line arguments.
			Process process = new Process();
			process.StartInfo.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
			process.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + @"\updater.exe";
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			process.Start();
		}

		/// <summary>
		///		Initializes the Fusion game engine and forces it to open and run the current map.
		/// </summary>
		private void PlayGame()
		{
			if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\fusion.exe") == false)
			{
				MessageBox.Show("Unable to locate engine executable, please make sure that the file \"Fusion.exe\" is inside the same directory as this executable.", "Unable to locate file");
				return;
			}

			// Save the map to a temporary file.
            object fileURL = _mapFileUrl;
            string mapURL = _mapFileUrl == null || _mapFileUrl.ToString() == "" ? Path.GetTempPath() + "play_map.fmp" : _mapFileUrl.ToString();
            if (mapURL.ToLower().StartsWith(Editor.GlobalInstance.GamePath.ToLower())) mapURL = mapURL.Substring(Editor.GlobalInstance.GamePath.Length + 1);
            SaveMap(mapURL);
            _mapFileUrl = fileURL;
            Text = "Fusion Editor - " + Path.GetFileName((string)_mapFileUrl);

			// Boot up the game engine executable with the correct 
			// command line arguments.
			Process process = new Process();
			process.StartInfo.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
			process.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + @"\fusion.exe";
            process.StartInfo.Arguments = "-game:\"" + Editor.GlobalInstance.GameName + "\" -map:\"" + mapURL + "\"";
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			process.Start();
		}

		/// <summary>
		///		Fress resource used by the old map and creates a new one.
		/// </summary>
		private void NewMap()
		{
			// If there are any unsaved changes give the user an option to save. 
			if (_mapChangedSinceSave == true)
			{
				DialogResult result = MessageBox.Show("This map has been modified since it was last saved, would you like to save now?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (result == DialogResult.OK) SaveMap();
				if (result == DialogResult.Cancel) return;
			}

            // Close down tool windows.
            if (_assetManagerWindow != null) _assetManagerWindow.Hide();
            if (_sceneGraphWindow != null) _sceneGraphWindow.Hide();
            if (_effectsDesignerWindow != null) _effectsDesignerWindow.Hide();
            if (_consoleWindow != null) _consoleWindow.Hide();
            if (_entityPropertiesWindow != null) _entityPropertiesWindow.Hide();

			// Create a new map.
			Engine.Engine.GlobalInstance.Map.SceneGraph.RootNode.ClearChildren();
			_mapChangedSinceSave = false;
			_mapFileUrl = null;
            Text = "Fusion Editor";
            Runtime.Resources.ResourceManager.EmptyCache();

			Editor.GlobalInstance.CameraNode.Position(0, 0, Editor.GlobalInstance.CameraNode.Transformation.Z);
			Editor.GlobalInstance.CameraNode.Zoom = 1.0f;

			_selectedEntityList.Clear();
            _undoStack.Clear();
            _redoStack.Clear();
			if (_entityPropertiesWindow != null) _entityPropertiesWindow.Entity = null;
		}

        /// <summary>
        ///     Entry point of the map saving thread.
        /// </summary>
        private void SaveMapThread()
        {
            Engine.Engine.GlobalInstance.Map.Save(_mapUrl);
        }

		/// <summary>
		///		Saves the current map to a given url.
		/// </summary>
		/// <param name="url">Url of file to save map to.</param>
        public void SaveMap(object url)
		{
            if (File.Exists(url.ToString()))
                File.Copy(url.ToString(), url.ToString() + ".bk", true);

            _mapUrl = url as string;
            _mapThread = new Thread(new ThreadStart(SaveMapThread));
            _mapThread.IsBackground = true;
            _mapThread.Start();

            ProgressWindow window = new ProgressWindow("Saving map", "Saving map");
            window.Marquee = true;
            window.Show(this);

            while (_mapThread != null && _mapThread.IsAlive == true)
                Application.DoEvents();

            window.Close();
            _mapThread = null;

			_mapFileUrl = url;
            Text = "Fusion Editor - " + Path.GetFileName((string)_mapFileUrl);
			_mapChangedSinceSave = false;
		}
        public void SaveMap()
		{
			if (_mapFileUrl == null)
			{
				// Request a file to save to from the user.
				SaveFileDialog dialog = new SaveFileDialog();
				dialog.Filter = "Fusion Map Files|*.fmp";
				dialog.InitialDirectory = Editor.GlobalInstance.MapPath;
				dialog.RestoreDirectory = true;
				if (dialog.ShowDialog() == DialogResult.Cancel) return;

				// Save the file to the request file.
				SaveMap(dialog.FileName);
			}
			else
			{
				SaveMap(_mapFileUrl);
			}
		}

		/// <summary>
		///		Saves the current map to a url of the users choice.
		/// </summary>
        public void SaveMapAs()
		{
			// Request a file to save to from the user.
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "Fusion Map Files|*.fmp";
			dialog.InitialDirectory = Editor.GlobalInstance.MapPath;
			dialog.RestoreDirectory = true;
			if (dialog.ShowDialog() == DialogResult.Cancel) return;

			// Save the file to the request file.
			SaveMap(dialog.FileName);
		}

        /// <summary>
        ///     Entry point of the map loading thread.
        /// </summary>
        private void LoadMapThread()
        {
            ProgressWindow window = new ProgressWindow("Loading map", "Loading map");
            window.Show();

            while (_mapLoaded == false)
            {
                window.Progress = Engine.Engine.GlobalInstance.Map.LoadingProgress;
                Application.DoEvents();
                Thread.Sleep(0);
            }

            window.Close();
        }

		/// <summary>
		///		Opens the current map to a given url.
		/// </summary>
		/// <param name="url">Url of map file to open.</param>
		public void OpenMap(object url)
		{
            string password = "";
			if (Map.IsMapPasswordProtected(url) == true)
			{
				// The map is password protected so pop up a window asking for the password.
				InputWindow inputWindow = new InputWindow("Password Required", "A password is required to access this map file, please enter it here if you know it. If you do not loading will be aborted.", "", true);
				if (inputWindow.ShowDialog() == DialogResult.Cancel) return;
                password = inputWindow.Input;
			}

            // Load in the map but check for any invalid password errors.
            try
            {
                _mapUrl = url as string;
                _mapPassword = password;
                _mapLoaded = false;
                _mapThread = new Thread(new ThreadStart(LoadMapThread));
                _mapThread.IsBackground = true;
                _mapThread.Start();

                Runtime.Resources.ResourceManager.EmptyCache();
                Engine.Engine.GlobalInstance.Map.Load(_mapUrl, _mapPassword, null, false, true);

                // Close down tool windows.
                if (_assetManagerWindow != null) _assetManagerWindow.Hide();
                if (_sceneGraphWindow != null) _sceneGraphWindow.Hide();
                if (_effectsDesignerWindow != null) _effectsDesignerWindow.Hide();
                if (_consoleWindow != null) _consoleWindow.Hide();
                if (_entityPropertiesWindow != null) _entityPropertiesWindow.Hide();

                _mapLoaded = true;
                _mapThread = null;
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured while opening the map, either the password you provided was invalid or the map file is corrupt. The map file could not be opened.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

			foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
			{
                // Path marker nodes need an image.
                if (node is PathMarkerNode && ((PathMarkerNode)node).Image == null)
                    ((PathMarkerNode)node).Image = _pathMarkerImage;

				// Find this nodes event nodes.
				EntityNode entityNode = node as EntityNode;
                if (entityNode == null) continue;

                entityNode.IsEventLinesVisible = true;
                entityNode.ForceVisibility = true;
                entityNode.IsBoundingBoxVisible = _viewBoundingBoxs;
                entityNode.IsCollisionBoxVisible = _viewCollisionBoxs;
                entityNode.IsEventLinesVisible = _viewEventLines;
			}

			AddRecentFile(url.ToString());
			_mapFileUrl = url;
            Text = "Fusion Editor - " + Path.GetFileName((string)_mapFileUrl);
			_mapChangedSinceSave = false;

			Editor.GlobalInstance.CameraNode.Position(0, 0, Editor.GlobalInstance.CameraNode.Transformation.Z);
			Editor.GlobalInstance.CameraNode.Zoom = 1.0f;

			_selectedEntityList.Clear();
            _undoStack.Clear();
            _redoStack.Clear();
			if (_entityPropertiesWindow != null) _entityPropertiesWindow.Entity = null;
		}
        public void OpenMap()
		{
			// If there are any unsaved changes give the user an option to save. 
			if (_mapChangedSinceSave == true)
			{
				DialogResult result = MessageBox.Show("This map has been modified since it was last saved, would you like to save now?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (result == DialogResult.OK) SaveMap();
				if (result == DialogResult.Cancel) return;
			}

			// Request a file to save to from the user.
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Fusion Map Files|*.fmp";
			dialog.InitialDirectory = Editor.GlobalInstance.MapPath;
			dialog.RestoreDirectory = true;
			if (dialog.ShowDialog() == DialogResult.Cancel) return;

			// Save the file to the request file.
			OpenMap(dialog.FileName);
		}

		/// <summary>
		///		Inserts the current object into the map at the cursors position (or center-screen - 
		///     - if this is not the active window) at the highest level on the scene graph
		/// </summary>
		private void InsertCurrentObject(bool inCenter, string overrideObjectURL)
		{
            string objectURL = overrideObjectURL != "" ? overrideObjectURL : _assetManagerWindow.SelectedObjectURL;
			EntityNode entity = null;

			// Keep a log of this insertion.
			DebugLogger.WriteLog("Inserting object of type '" + objectURL + "' into map");

			// Check the extension to see if this is a built-in object or not.
			if (objectURL.IndexOf(".fso") >= 0)
			{
				// Compile the script.
				ScriptCompiler compiler = new ScriptCompiler();
				bool errorOccured = false;
				string errorDescription = "\t";
				if (compiler.Compile(objectURL) > 0)
					foreach (CompileError error in compiler.ErrorList)
						if (error.AlertLevel == ErrorAlertLevel.Error || error.AlertLevel == ErrorAlertLevel.FatalError)
						{
							errorDescription += (errorDescription == "" ? "" : "\n\t") + error.ToString();
							errorOccured = true;
						}

				// If an error occured notify the user if not then 
				// insert a new scripted entity into the map.
				if (errorOccured == true)
				{
					MessageBox.Show("Unable to insert object into map, the following error(s) occured while attempt to compile this objects script.\n\n"+errorDescription,"Compile Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					DebugLogger.WriteLog("Object insertion aborted. The following error(s) occured while attempting to compile this objects script.\n\n" + errorDescription, LogAlertLevel.Warning); 
				}
				else
				{
					// Dump the compiled code into a memory stream.
					MemoryStream memStream = new MemoryStream();
					BinaryWriter writer = new BinaryWriter(memStream);
					BinaryReader reader = new BinaryReader(memStream);
					compiler.DumpExecutableFile(writer);
					memStream.Position = 0;

					// Create a new scripted entity with the given script attached to it.
					ScriptedEntityNode scriptedEntity = new ScriptedEntityNode();
					entity = scriptedEntity;
					entity.RenderMode = EntityRenderMode.Rectangle;
					entity.BoundingRectangle = new Rectangle(0, 0, 16, 16);
					entity.Width = 16;
					entity.Height = 16;
                    entity.Name = Path.GetFileNameWithoutExtension(objectURL);

                    ScriptProcess process = new ScriptProcess(VirtualMachine.GlobalInstance, reader);
					process.Url = objectURL;
                    if (process.DefaultEditorState != null)
                        process.ChangeState(process.DefaultEditorState.Identifier);
                    else
                        process.State = null;
					scriptedEntity.ScriptProcess = process;

                    // Are we allowed to place the entity?
                    bool canPlace = true;
                    string placeError = "This entity has been flagged as unplaceable. Please choose another or check this objects script.";
                    foreach (Define define in process.Defines)
                    {
                        switch (define.Ident.ToUpper())
                        {
                            case "UNPLACEABLE":
                                canPlace = false;
                                break;
                            case "UNPLACEABLE_ERROR":
                                placeError = define.Value;
                                break;
                        }
                    }
                    if (canPlace == false)
                    {
                        MessageBox.Show(placeError, "Unplaceable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

					Engine.Engine.GlobalInstance.Map.SceneGraph.RootNode.AddChild(entity);

                    // Invoke OnCreate event.
                    scriptedEntity.ScriptProcess[0].InvokeFunction("OnCreate", true, false);

					// Free up the streams.
					memStream.Close();
					reader.Close();
					writer.Close();
				}
			}

			// Its built in so create the object requested.
			else
			{
				switch (objectURL.ToLower())
				{
					case "tilemap segment":
						TilemapSegmentNode segment = new TilemapSegmentNode(8, 8, 16, 16);
						Engine.Engine.GlobalInstance.Map.SceneGraph.RootNode.AddChild(segment);
						segment.IsGridVisible = true;
						entity = segment;
						break;

					case "emitter":
						entity = new EmitterNode();
						Engine.Engine.GlobalInstance.Map.SceneGraph.RootNode.AddChild(entity);
						entity.BoundingRectangle = new Rectangle(0, 0, 16, 16);
						entity.Width = 16;
						entity.Height = 16;
						break;

					case "entity":
						entity = new EntityNode();
						Engine.Engine.GlobalInstance.Map.SceneGraph.RootNode.AddChild(entity);
						entity.BoundingRectangle = new Rectangle(0, 0, 16, 16);
						entity.Width = 16;
						entity.Height = 16;
						break;

                    case "path marker":
                        entity = new PathMarkerNode();
                        Engine.Engine.GlobalInstance.Map.SceneGraph.RootNode.AddChild(entity);
                        entity.BoundingRectangle = new Rectangle(0, 0, 16, 16);
                        entity.Width = 16;
                        entity.Height = 16;
                        entity.Image = _pathMarkerImage;
                        entity.RenderMode = EntityRenderMode.Image;
                        entity.Name = "Path Marker " + MathMethods.Random(MathMethods.Random(0, 10000), MathMethods.Random(10000, 10000000));
                        
                        foreach (EntityNode selEntity in _selectedEntityList)
                            if (selEntity is PathMarkerNode && ((PathMarkerNode)selEntity).NextNodeName == "")
                            {
                                ((PathMarkerNode)selEntity).NextNode = (PathMarkerNode)entity;
                                ((PathMarkerNode)selEntity).NextNodeName = entity.Name;
                            }

                        break;
				}
			}

			if (entity == null) return;

			// Find the correct position for our new entity.
			int mapX, mapY;
			if (inCenter == false)
			{
				mapX = (int)Editor.GlobalInstance.CameraNode.Transformation.X + (int)((mousePositionBeforeRightClick[0] / Editor.GlobalInstance.CameraNode.Zoom));
				mapY = (int)Editor.GlobalInstance.CameraNode.Transformation.Y + (int)((mousePositionBeforeRightClick[1] / Editor.GlobalInstance.CameraNode.Zoom));
			}
			else
			{
				mapX = (int)Editor.GlobalInstance.CameraNode.Transformation.X + (int)(((mapPanel.ClientSize.Width - entity.BoundingRectangle.Width) / 2) / Editor.GlobalInstance.CameraNode.Zoom);
				mapY = (int)Editor.GlobalInstance.CameraNode.Transformation.Y + (int)(((mapPanel.ClientSize.Height - entity.BoundingRectangle.Height) / 2) / Editor.GlobalInstance.CameraNode.Zoom);
			}

			if (_snapToGrid == true)
			{
				mapX = (mapX / _gridWidth) * _gridWidth;
				mapY = (mapY / _gridHeight) * _gridHeight;
			}

			entity.Position(mapX, mapY, 0.0f);
            entity.IsBoundingBoxVisible = _viewBoundingBoxs;
			entity.IsEventLinesVisible = _viewEventLines;
            entity.IsCollisionBoxVisible = _viewCollisionBoxs;
            entity.ForceVisibility = true;

            ClearSelection();
            AddEntityToSelection(entity);

			// Update the scene graph window if it exists.
			if (_sceneGraphWindow != null) _sceneGraphWindow.SyncronizeData();

			// Update this entitys event nodes and the event nodes of others.
			foreach (SceneNode node in Engine.Engine.GlobalInstance.Map.SceneGraph.EnumerateNodes())
			{
				EntityNode subNode = node as EntityNode;
				if (subNode == null) continue;
				if (subNode.Event.ToLower() == entity.Name.ToString().ToLower()) subNode.EventNodes.Add(entity);
			}

            PushUndoOperation(new InsertNodesUndoOperation(new SceneNode[] { entity }));
            SyncronizeWindow();

			_mapChangedSinceSave = true;
		}
        private void InsertCurrentObject(bool inCenter)
        {
            InsertCurrentObject(inCenter, "");
        }

		#endregion
	}

    /// <summary>
    ///     Used as a base for all undo operations.
    /// </summary>
    public abstract class UndoOperation
    {
        #region Members
        #region Variables

        #endregion
        #region Properties

        /// <summary>
        ///     Gets the name of this operation.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Undos the operation.
        /// </summary>
        public virtual void Undo()
        {
            throw new Exception("Unable to undo operation, undo method not implemented.");
        }

        /// <summary>
        ///     Redos the operation.
        /// </summary>
        public virtual void Redo()
        {
            throw new Exception("Unable to redo operation, redo method not implemented.");
        }

        #endregion
    }

    /// <summary>
    ///     Deals with undoing and redo node insertion operations.
    /// </summary>
    public class InsertNodesUndoOperation : UndoOperation
    {
        #region Members
        #region Variables

        private SceneNode[] _nodes = null;
        private Hashtable _nodeParents = new Hashtable();

        #endregion
        #region Properties

        /// <summary>
        ///     Gets the name of this operation.
        /// </summary>
        public override string Name
        {
            get { return "Insert Nodes"; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Undos the operation.
        /// </summary>
        public override void Undo()
        {
            foreach (SceneNode node in _nodes)
                ((SceneNode)_nodeParents[node]).RemoveChild(node);
        }

        /// <summary>
        ///     Redos the operation.
        /// </summary>
        public override void Redo()
        {
            foreach (SceneNode node in _nodes)
                ((SceneNode)_nodeParents[node]).AddChild(node);
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="nodes">Nodes that were inserted.</param>
        public InsertNodesUndoOperation(SceneNode[] nodes)
        {
            _nodes = nodes;
            foreach (SceneNode node in nodes)
                _nodeParents.Add(node, node.Parent);
        }

        #endregion
    }

    /// <summary>
    ///     Deals with undoing and redo node deletion operations.
    /// </summary>
    public class DeleteNodesUndoOperation : UndoOperation
    {
        #region Members
        #region Variables

        private SceneNode[] _nodes = null;
        private Hashtable _nodeParents = new Hashtable();

        #endregion
        #region Properties

        /// <summary>
        ///     Gets the name of this operation.
        /// </summary>
        public override string Name
        {
            get { return "Delete Nodes"; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Undos the operation.
        /// </summary>
        public override void Undo()
        {
            foreach (SceneNode node in _nodes)
                ((SceneNode)_nodeParents[node]).AddChild(node);
        }

        /// <summary>
        ///     Redos the operation.
        /// </summary>
        public override void Redo()
        {
            foreach (SceneNode node in _nodes)
                ((SceneNode)_nodeParents[node]).RemoveChild(node);
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="nodes">Nodes that were deleted.</param>
        public DeleteNodesUndoOperation(SceneNode[] nodes)
        {
            _nodes = nodes;
            foreach (SceneNode node in nodes)
                _nodeParents.Add(node, node.Parent);
        }

        #endregion
    }

    /// <summary>
    ///     Deals with undoing and redo node duplication operations.
    /// </summary>
    public class DuplicateNodesUndoOperation : UndoOperation
    {
        #region Members
        #region Variables

        private SceneNode[] _nodes = null;
        private Hashtable _nodeParents = new Hashtable();

        #endregion
        #region Properties

        /// <summary>
        ///     Gets the name of this operation.
        /// </summary>
        public override string Name
        {
            get { return "Duplicate Nodes"; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Undos the operation.
        /// </summary>
        public override void Undo()
        {
            foreach (SceneNode node in _nodes)
                ((SceneNode)_nodeParents[node]).RemoveChild(node);
        }

        /// <summary>
        ///     Redos the operation.
        /// </summary>
        public override void Redo()
        {
            foreach (SceneNode node in _nodes)
                ((SceneNode)_nodeParents[node]).AddChild(node);
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="nodes">Nodes that were duplicated.</param>
        public DuplicateNodesUndoOperation(SceneNode[] nodes)
        {
            _nodes = nodes;
            foreach (SceneNode node in nodes)
                _nodeParents.Add(node, node.Parent);
        }

        #endregion
    }

    /// <summary>
    ///     Stores details on a node move operation.
    /// </summary>
    public class TransformNodeUndoOperation : UndoOperation
    {
        #region Members
        #region Variables

        private SceneNode _node = null;
        private Transformation _originalTransformation;
        private Transformation _newTransformation;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets the name of this operation.
        /// </summary>
        public override string Name
        {
            get { return "Transform Node"; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Undos the operation.
        /// </summary>
        public override void Undo()
        {
            _node.Transformation = _originalTransformation;
        }

        /// <summary>
        ///     Redos the operation.
        /// </summary>
        public override void Redo()
        {
            _node.Transformation = _newTransformation;
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="node">Node that was transformed.</param>
        /// <param name="originalTransformation">Nodes original transformation.</param>
        /// <param name="newTransformation">Nodes new transformation.</param>
        public TransformNodeUndoOperation(SceneNode node, Transformation originalTransformation, Transformation newTransformation)
        {
            _node = node;
            _newTransformation = newTransformation;
            _originalTransformation = originalTransformation;
        }

        #endregion
    }

    /// <summary>
    ///     Stores details on a node size operation.
    /// </summary>
    public class ResizeNodeUndoOperation : UndoOperation
    {
        #region Members
        #region Variables

        private EntityNode _node = null;
        private Transformation _originalTransformation;
        private Transformation _newTransformation;
        private Rectangle _originalBoundingBox;
        private Rectangle _newBoundingBox;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets the name of this operation.
        /// </summary>
        public override string Name
        {
            get { return "Resize Node"; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Undos the operation.
        /// </summary>
        public override void Undo()
        {
            _node.Transformation = _originalTransformation;
            _node.BoundingRectangle = _originalBoundingBox;
        }

        /// <summary>
        ///     Redos the operation.
        /// </summary>
        public override void Redo()
        {
            _node.Transformation = _newTransformation;
            _node.BoundingRectangle = _newBoundingBox;
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="node">Node that was transformed.</param>
        /// <param name="originalBoundingBox">Nodes original bounding box.</param>
        /// <param name="originalTransformation">Nodes original transformation.</param>
        /// <param name="newBoundingBox">Nodes new bounding box.</param>
        /// <param name="newTransformation">Nodes new transformation.</param>
        public ResizeNodeUndoOperation(EntityNode node, Rectangle originalBoundingBox, Transformation originalTransformation, Rectangle newBoundingBox, Transformation newTransformation)
        {
            _node = node;
            _newBoundingBox = newBoundingBox;
            _newTransformation = newTransformation;
            _originalBoundingBox = originalBoundingBox;
            _originalTransformation = originalTransformation;
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    //public class ModifyPropertiesUndoOperation : UndoOperation
    //{

    //}

    /// <summary>
    /// 
    /// </summary>
    //public class ShiftNodesUndoOperation : UndoOperation
    //{

    //}

    /// <summary>
    /// 
    /// </summary>
    //public class ModifyTilesUndoOperation : UndoOperation
    //{

    //}

    /// <summary>
    /// 
    /// </summary>
    //public class GroupNodesUndoOperation : UndoOperation
    //{

    //}

    /// <summary>
    /// 
    /// </summary>
    //public class UngroupNodesUndoOperation : UndoOperation
    //{

    //}

    /// <summary>
    /// 
    /// </summary>
    //public class CutUndoOperation : UndoOperation
    //{

    //}

    /// <summary>
    /// 
    /// </summary>
    //public class CopyUndoOperation : UndoOperation
    //{

    //}

    /// <summary>
    /// 
    /// </summary>
    //public class PasteUndoOperation : UndoOperation
    //{

    //}

	/// <summary>
	///		Used to identify the current tool being used to edit the map.
	/// </summary>
	public enum Tools
	{
		Camera,
		Selector,
		Pencil,
		Eraser,
		PaintBucket,
		TilePicker,
		Rectangle,
		RoundedRectangle,
		Ellipse,
		Line
	}

	/// <summary>
	///		Used by the EditorWindow class to describe the alignment of an entity.
	/// </summary>
	public enum Alignment
	{
		Top,
		Bottom,
		Left,
		Right,
		Center,
		Middle
	}

	/// <summary>
	///		Used by the EditorWindow class to describe the direction to size an entity in.
	/// </summary>
	public enum SizingDirection
	{
		Top = 0x00000001,
		Bottom = 0x00000002,
		Left = 0x00000004,
		Right = 0x00000008,
	}

}