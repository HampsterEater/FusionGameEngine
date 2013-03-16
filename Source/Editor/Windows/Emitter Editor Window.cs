/*
 * File: Emitter Selector Window.cs
 *
 * Contains all the functional partial code declaration for the EmitterEditorWindow form.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */ 

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Input;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Editor.Windows
{
    /// <summary>
    ///     Main emitter editor classs, contains the code required to construct and run the emitter editor window.
    /// </summary>
    public partial class EmitterEditorWindow : Form
    {
        #region Members
        #region Variables

        private EmitterNode _emitterNode = new EmitterNode();
        private EmitterProperties _emitterProperties = new EmitterProperties();

        private GraphicsCanvas _canvas = null;

        private Transformation _cameraTransformation = new Transformation(0, 0, 0, 0, 0, 0, 1, 1, 1);
        private float _cameraMoveX, _cameraMoveY;
        private bool _cameraMoving = false;

        private bool _showBoundingBox = true;
        private bool _showOrigin = true;

        private PointF _originalEmitterPosition = new PointF(0, 0);
        private bool _originalEmitterEnabled = true;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the emitter this editor is editing.
        /// </summary>
        public EmitterNode Emitter
        {
            get { return _emitterNode; }
            set 
            { 
                _emitterNode = value; 
                _emitterProperties.Emitter = value; 
            }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Invoked when a new instance of this class is created.
        /// </summary>
        public EmitterEditorWindow()
        {
            InitializeComponent();

            _emitterProperties.Emitter = _emitterNode;
            _canvas = new GraphicsCanvas(previewPanel, 0, new CanvasRenderHandler(Render));

            // Create a callback so we can render the canvas (it won't be done my the main loop due to 
            // this window being show in model).
            Application.Idle += new EventHandler(Application_Idle);

            SyncronizeTypes();
        }

        /// <summary>
        ///     Called when the application is idle. This function is used to render the canvas.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        void Application_Idle(object sender, EventArgs e)
        {
            if (Visible == false)
                return;

            while (NativeMethods.IsApplicationIdle == true)
            {
                GraphicsCanvas.RenderAll();
                InputManager.Poll();
            }
        }

        /// <summary>
        ///     Syncronizes the type list.
        /// </summary>
        private void SyncronizeTypes()
        {
            // Syncronizes the type list box.
            int selectedIndex = typeListBox.SelectedIndex;
            typeListBox.Items.Clear();

            int index = 0;
            typeListBox.Items.Add("Emitter");
            foreach (ParticleType type in _emitterNode.ParticleTypes)
            {
                typeListBox.Items.Add(new ParticleTypeItem(type, ++index));
                int modIndex = 0;
                foreach (ParticleModifier modifier in type.Modifiers)
                    typeListBox.Items.Add(new ParticleModifierItem(type, modifier, ++modIndex));
            }

            if (selectedIndex >= 0 && selectedIndex < typeListBox.Items.Count)
                typeListBox.SelectedIndex = selectedIndex;
            else
                typeListBox.SelectedIndex = typeListBox.Items.Count - 1;
        }

        /// <summary>
        ///     Called when the preview panel needs rendering.
        /// </summary>
        private void Render() 
        {
            if (Visible == false)
                return;

            IRenderTarget previousRenderTarget = GraphicsManager.RenderTarget;

            // Render the preview.
            GraphicsManager.RenderTarget = _canvas;
            GraphicsManager.BeginScene();
            GraphicsManager.ClearColor = unchecked((int)0xFFACACAC);
            GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
            GraphicsManager.SetResolution(previewPanel.ClientSize.Width, previewPanel.ClientSize.Height,false);
            GraphicsManager.Viewport = new Rectangle(0, 0, previewPanel.ClientSize.Width, previewPanel.ClientSize.Height);
            GraphicsManager.ClearScene();

            // Render the origin.
            if (_showOrigin == true)
            {
                GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
                GraphicsManager.RenderLine(0, _cameraTransformation.Y + (_canvas.RenderControl.ClientSize.Height / 2), 0, _canvas.RenderControl.ClientSize.Width, _cameraTransformation.Y + (_canvas.RenderControl.ClientSize.Height / 2), 0);
                GraphicsManager.RenderLine(_cameraTransformation.X + (_canvas.RenderControl.ClientSize.Width / 2), 0, 0, _cameraTransformation.X + (_canvas.RenderControl.ClientSize.Width / 2), _canvas.RenderControl.ClientSize.Height, 0);
            }

            // Render the emitter node in the middle.
            if (_emitterNode != null)
            {
                _emitterNode.Think(1.0f); // Allow it some thinking time as well.

                _emitterNode.IsBoundingBoxVisible = _showBoundingBox;
                _emitterNode.IsCollisionBoxVisible = false;
                _emitterNode.IsEventLinesVisible = false;
                _emitterNode.IsSizingPointsVisible = false;
                _emitterNode.Transformation = _cameraTransformation;
                _emitterNode.Render(new Transformation((previewPanel.ClientSize.Width - _emitterNode.BoundingRectangle.Width) / 2.0f, (previewPanel.ClientSize.Height - _emitterNode.BoundingRectangle.Height) / 2.0f, 0, 0, 0, 0, 1, 1, 1), null, 0);
            }

            GraphicsManager.FinishScene();
            GraphicsManager.PresentScene();

            // Set the status strips.
            fpsToolStripStatusLabel.Text = "FPS: " + Editor.GlobalInstance.CurrentFPS;
            particleCountToolStripStatusLabel.Text = "Particles: " + _emitterNode.Particles.Count + "/" + _emitterNode.MaximumParticles;

            // Move the camera around if we have been asked to.
            if (InputManager.KeyDown(KeyCodes.LeftMouse) || InputManager.KeyDown(KeyCodes.RightMouse))
            {
                if (_cameraMoving == true)
                {
                    _cameraTransformation.X += (InputManager.MouseX - _cameraMoveX);
                    _cameraTransformation.Y += (InputManager.MouseY - _cameraMoveY);
                    _cameraMoveX = InputManager.MouseX;
                    _cameraMoveY = InputManager.MouseY;
                }
                else if (InputManager.MouseX >= 0 && InputManager.MouseY >= 0 && InputManager.MouseX < previewPanel.ClientSize.Width && InputManager.MouseY < previewPanel.ClientSize.Height)
                {
                    if (InputManager.KeyPressed(KeyCodes.LeftMouse) || InputManager.KeyPressed(KeyCodes.RightMouse))
                    {
                        _cameraMoving = true;
                        _cameraMoveX = InputManager.MouseX;
                        _cameraMoveY = InputManager.MouseY;
                    }
                }
            }
            else
            {
                _cameraMoving = false;
            }

            GraphicsManager.RenderTarget = previousRenderTarget;
       }

        /// <summary>
        ///     Called when the new type button is clicked.
        /// </summary>
       /// <param name="sender">Object that caused this event to be triggered.</param>
       /// <param name="e">Arguments explaining why this event occured.</param>
        private void newTypeButton_Click(object sender, EventArgs e)
        {
            ParticleType type = new ParticleType(new MinMax(10, 300), new MinMax(1, 3), EmitShape.Rectangle, 0, new MinMax(500, 2000));
            _emitterNode.RegisterParticleType(type);
            SyncronizeTypes();
        }

        /// <summary>
        ///     Called when the type list box's selected item is changed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void typeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeListBox.SelectedIndex < 0)
            {
                deleteButton.Enabled = false;
                addModButton.Enabled = false;
                modifierTypeComboBox.Enabled = false;
                return;
            }

            bool isType = typeListBox.SelectedItem is ParticleTypeItem;
            bool isMod = typeListBox.SelectedItem is ParticleModifierItem;
            bool isEmitter = typeListBox.SelectedItem is string;

            addModButton.Enabled = isType || isMod;
            modifierTypeComboBox.Enabled = isType || isMod;
            deleteButton.Enabled = isType || isMod;
            propertyGrid.SelectedObject = isEmitter == true ? (object)_emitterProperties : (isMod == true ? (object)((ParticleModifierItem)typeListBox.SelectedItem).Modifier : (object)((ParticleTypeItem)typeListBox.SelectedItem).Type);
        }

        /// <summary>
        ///     Called when the add modifier button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void addModButton_Click(object sender, EventArgs e)
        {
            if (modifierTypeComboBox.SelectedIndex < 0 || (!(typeListBox.SelectedItem is ParticleTypeItem) && !(typeListBox.SelectedItem is ParticleModifierItem)))
            {
                MessageBox.Show("You must select a modifier type and a particle type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ParticleModifier modifier = null;
            switch (modifierTypeComboBox.SelectedItem.ToString().ToLower())
            {
                case "acceleration": modifier = new ParticleAccelerationModifier(); break;
                case "scale": modifier = new ParticleScaleModifier(); break;
                case "rotation": modifier = new ParticleRotationModifier(); break;
                case "color": modifier = new ParticleColorModifier(); break;
                case "render state": modifier = new ParticleRenderStateModifier(); break;
                case "animation": modifier = new ParticleAnimationModifier(); break;
            }
            if (typeListBox.SelectedItem is ParticleTypeItem)
                ((ParticleTypeItem)typeListBox.SelectedItem).Type.RegisterParticleModifier(modifier);
            else
                ((ParticleModifierItem)typeListBox.SelectedItem).Type.RegisterParticleModifier(modifier);
                SyncronizeTypes();
        }

        /// <summary>
        ///     Called when the new effect button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void newEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _emitterNode = new EmitterNode();
            SyncronizeTypes();
        }

        /// <summary>
        ///     Called when the delete button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (!(typeListBox.SelectedItem is ParticleModifierItem) && !(typeListBox.SelectedItem is ParticleTypeItem))
            {
                MessageBox.Show("You must select a particle modifier or particle type to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (typeListBox.SelectedItem is ParticleModifierItem)
            {
                ParticleModifierItem modifier = ((ParticleModifierItem)typeListBox.SelectedItem);
                modifier.Type.UnregisterParticleModifier(modifier.Modifier);
            }
            else
            {
                ((ParticleTypeItem)typeListBox.SelectedItem).Type.Dispose();
                _emitterNode.UnregisterParticleType(((ParticleTypeItem)typeListBox.SelectedItem).Type);
            }
            SyncronizeTypes();
        }

        /// <summary>   
        ///     Called when the reset emitter menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void resetEmitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _emitterNode.Restart();
        }

        /// <summary>
        ///     Called when the reset camera menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void resetCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _cameraTransformation = new Transformation(0, 0, 0, 0, 0, 0, 1, 1, 1);
        }

        /// <summary>
        ///     Called when the view bounding box menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void viewBoundingBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _showBoundingBox = !_showBoundingBox;
            viewBoundingBoxToolStripMenuItem.Checked = _showBoundingBox;
            viewBoundingBoxToolStripContextMenuItem.Checked = _showBoundingBox;
        }

        /// <summary>
        ///     Called when the view origin menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void viewOriginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _showOrigin = !_showOrigin;
            viewOriginToolStripContextMenuItem.Checked = _showOrigin;
            viewOriginToolStripMenuItem.Checked = _showOrigin;
        }

        /// <summary>
        ///     Called when the import effect menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void importEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Editor.GlobalInstance.EffectPath;
            dialog.Filter = "Effects files|*.fef";
            dialog.Title = "Import effect";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string file = dialog.FileName;

                if (file.ToLower().StartsWith(Editor.GlobalInstance.GamePath.ToLower() + "\\") == true)
                    file = file.Substring(Editor.GlobalInstance.GamePath.Length + 1);

                if (file.ToLower().StartsWith(Editor.GlobalInstance.EffectPath.ToLower()) == false)
                {
                    MessageBox.Show("Effect file must be within the effect directory or a sub directory of it.", "Relative Path Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Stream stream = StreamFactory.RequestStream(file, StreamMode.Open);
                BinaryReader reader = new BinaryReader(stream);

                if (reader.ReadByte() != 'F' || reader.ReadByte() != 'E' || reader.ReadByte() != 'F')
                {
                    MessageBox.Show("Effect file has been corrupted, it could not be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                _emitterNode = new EmitterNode();
                _emitterProperties.Emitter = _emitterNode;
                _emitterNode.Load(reader);

                reader.Close();
                stream.Close();

                SyncronizeTypes();
            }
        }

        /// <summary>
        ///     Called when the export effect menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void exportEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = Editor.GlobalInstance.EffectPath;
            dialog.Filter = "Effects files|*.fef";
            dialog.Title = "Export effect";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string file = dialog.FileName;

                if (file.ToLower().StartsWith(Editor.GlobalInstance.GamePath.ToLower() + "\\") == true)
                    file = file.Substring(Editor.GlobalInstance.GamePath.Length + 1);

                if (file.ToLower().StartsWith(Editor.GlobalInstance.EffectPath.ToLower()) == false)
                {
                    MessageBox.Show("Effect file must be within the effect directory or a sub directory of it.", "Relative Path Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Stream stream = StreamFactory.RequestStream(file, StreamMode.Truncate);
                BinaryWriter writer = new BinaryWriter(stream);

                writer.Write(new byte[] { (byte)'F', (byte)'E', (byte)'F' });
                _emitterNode.Save(writer);

                writer.Close();
                stream.Close();
            }
        }

        /// <summary>
        ///     Called when the exit menu item is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Called when the visibility of this window is changed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void EmitterEditorWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                _originalEmitterPosition.X = _emitterNode.Transformation.X;
                _originalEmitterPosition.Y = _emitterNode.Transformation.Y;
                _originalEmitterEnabled = _emitterNode.IsEnabled;
                _emitterNode.IsEnabled = true;
                SyncronizeTypes();
            }
        }

        /// <summary>
        ///     Invoked when the form is closed.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void EmitterEditorWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _emitterNode.Position(_originalEmitterPosition.X, _originalEmitterPosition.Y, _emitterNode.Transformation.Z);
            _emitterNode.IsEnabled = _originalEmitterEnabled;
        }

        #endregion
    }

    /// <summary>
    ///     Holds several properties about and emiiter than can be edited by the property grid.
    /// </summary>
    internal class EmitterProperties
    {
        #region Members
        #region Variables

        private EmitterNode _emitterNode = null;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the emitter the properties in this class relate to.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public EmitterNode Emitter
        {
            get { return _emitterNode; }
            set { _emitterNode = value; }
        }

        /// <summary>
        ///     Gets or sets the bounding box of the associated emitter.
        /// </summary>
        [Category("Emitter"), DisplayName("Bounding Rectangle"), Description("Indicates the bounding rectangle in which particles are spawned in.")]
        public Rectangle BoundingRectangle
        {
            get { return _emitterNode.BoundingRectangle; }
            set { _emitterNode.BoundingRectangle = value; }
        }

        /// <summary>
        ///     Gets or sets the maximum particles the associated emiter can display.
        /// </summary>  
        [Category("Emitter"), DisplayName("Maximum Particles"), Description("Indicates the maximum amount of particles this emitter can process.")]
        public int MaximumParticles
        {
            get { return _emitterNode.MaximumParticles; }
            set { _emitterNode.MaximumParticles = value; }
        }

        #endregion
        #endregion
        #region Methods

        #endregion
    }

    /// <summary>
    ///     Holds details about a specific particle type inserted into the emitter editor
    ///     windows type list.
    /// </summary>
    internal class ParticleTypeItem
    {
        #region Members
        #region Variables

        private ParticleType _type = null;
        private int _index = 0;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the particle type this class is associated with.
        /// </summary>
        public ParticleType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        ///     Gets or sets the index of the item.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this class's data into a textural form.
        /// </summary>
        /// <returns>Textural form of this class's data.</returns>
        public override string ToString()
        {
            return "  Particle Type " + _index;
        }

        /// <summary>
        ///     Invoked when a new instance of this class is created.
        /// </summary>
        /// <param name="type">Particle type this class relates to.</param>
        /// <param name="index">Index of this item.</param>
        public ParticleTypeItem(ParticleType type, int index)
        {
            _index = index;
            _type = type;
        }

        #endregion
    }

    /// <summary>
    ///     Holds details about a specific particle modifier inserted into the emiiter editor
    ///     windows type list.
    /// </summary>
    internal class ParticleModifierItem
    {
        #region Members
        #region Variables

        private ParticleType _type = null;
        private ParticleModifier _modifier = null;
        private int _index = 0;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the particle type associated with this class's particle modifier.
        /// </summary>
        public ParticleType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        ///     Gets or sets the particle modifier associated with this class.
        /// </summary>
        public ParticleModifier Modifier
        {
            get { return _modifier; }
            set { _modifier = value; }
        }

        /// <summary>
        ///     Gets or sets the index of this item.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this class's data into a textural form.
        /// </summary>
        /// <returns>Textural form of this class's data.</returns>
        public override string ToString()
        {
            return "    " + _modifier.ToString();
        }

        /// <summary>
        ///     Invoked when a new instance of this class is created.
        /// </summary>
        /// <param name="type">Particle type this class is associated with.</param>
        /// <param name="index">Index of this item.</param>
        /// <param name="modifier">Particle modifier this class is associated with.</param>
        public ParticleModifierItem(ParticleType type, ParticleModifier modifier, int index)
        {
            _index = index;
            _type = type;
            _modifier = modifier;
        }

        #endregion
    }

}