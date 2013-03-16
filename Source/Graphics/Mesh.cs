/* 
 * File: Mesh.cs
 *
 * This source file contains the declaration of the Mesh class which encapsulates
 * all the details needed to render mesh data to the screen. It also contains the 
 * MeshFrame class which is used to isolate the graphics API rendering from the game.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Drawing;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Graphics
{
    #region Interfaces

    /// <summary>
    ///     Used to store details on a single mesh, and
    ///     to isolate graphics API rendering of mesh from the game.
    /// </summary>
    public interface IDriverMesh 
    {
        #region Methods

        IGraphicsDriver Driver { get; }
        VertexMap VertexMap { get; set; }
        Point Origin { get; set; }
        void Render(float x, float y, float z);

        #endregion
    }

    #endregion
    #region Enumeration

    /// <summary>
    ///     Describes how and mesh should be loaded, rendered and stored.
    /// </summary>
    public enum MeshFlags : int
    {
        Dynamic = 1,
    }

    #endregion
    #region Mesh Classes

    /// <summary>
    ///     Used to store details on a single piece of renderable mesh data.
    /// </summary>
    public class Mesh
    {
        #region Members
        #region Variables

        private VertexMap _vertexMap = null;
        private IDriverMesh _driverMesh = null;
        private MeshFlags _flags = 0;

        private string _url = "";

        #endregion
        #region Properties

        /// <summary>
        ///		Gets the full vertex map as it was in the mesh file.
        /// </summary>
        public VertexMap VertexMap
        {
            get
            {
                if ((_flags & MeshFlags.Dynamic) == 0)
                    throw new Exception("VertexMap data can't be modified in none dynamic image.");
                return _vertexMap;
            }
            set
            {
                if ((_flags & MeshFlags.Dynamic) == 0)
                    throw new Exception("VertexMap data can't be modified in none dynamic image.");
                _vertexMap = value;
                CreateMesh();
            }
        }

        /// <summary>
        ///     Sets the origin of all this mesh. The origin is 
        ///     the offset from the top-left corner used when rendering this mesh.
        /// </summary>
        public Point Origin
        {
            set { _driverMesh.Origin = value; }
            get { return _driverMesh.Origin; }
        }

        /// <summary>
        ///     Gets or sets the mesh class the driver is using to render.
        /// </summary>
        public IDriverMesh DriverMesh
        {
            get { return _driverMesh; }
            set { _driverMesh = value; }
        }

        /// <summary>
        ///		Gets or sets the url this mesh was loaded from.
        /// </summary>
        public string URL
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        ///		Gets the width of this mesh.
        /// </summary>
        public float Width
        {
            get { return _vertexMap.Width; }
        }

        /// <summary>
        ///		Gets the height of this mesh.
        /// </summary>
        public float Height
        {
            get { return _vertexMap.Height; }
        }

        /// <summary>
        ///		Gets the depth of this mesh.
        /// </summary>
        public float Depth
        {
            get { return _vertexMap.Depth; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///		Deconstructs this mesh.
        /// </summary>
        ~Mesh()
        {
            Dispose();
        }

        /// <summary>
        ///     Returns a textural form of this class.
        /// </summary>
        /// <returns>Textural form of this class.</returns>
        public override string ToString()
        {
            return _url != null ? _url : "Unknown URL";
        }

        /// <summary>
        ///		Disposes of all resources used by this image.
        /// </summary>
        public void Dispose()
        {
            _vertexMap = null;
            _driverMesh = null;
        }

        /// <summary>
        ///		Recreates the mesh  and resources required to render this mesh.
        /// </summary>
        public void CreateMesh()
        {
            _driverMesh = GraphicsManager.CreateDriverMesh(_vertexMap);
        }

        /// <summary>
        ///		Renders this mesh to the screen.
        /// </summary>
        /// <param name="x">Position on the x-axis to render mesh.</param>
        /// <param name="y">Position on the y-axis to render mesh.</param>
        /// <param name="z">Position on the z-axis to render mesh.</param>
        /// <param name="frame">Frame of this mesh to render.</param>
        public void Render(float x, float y, float z)
        {
            _driverMesh.Render(x, y, z);
        }

        // Disable constructors.
        private Mesh() { }

        /// <summary>
        ///		Initilizes a new mesh from a file.
        /// </summary>
        /// <param name="path">Memory location (or url) of mesh file to load.</param>
        public Mesh(object path, MeshFlags flags)
        {
            if ((path is VertexMap) == false)
            {
                if (path is string) _url = (string)path;
                _vertexMap = VertexMapFactory.LoadVertexMap(path);
            }
            else
                _vertexMap = (path as VertexMap).Copy();

            if (_vertexMap == null)
                return;

            _flags = flags;
            _driverMesh = GraphicsManager.CreateDriverMesh(_vertexMap);

            // Are we dynamic? If not we don't need the full vertexmaps data so lets destroy it and
            // free up a bit of memory.
            //if ((_flags & MeshFlags.Dynamic) == 0 && _vertexMap != null)
            //{
            //    _vertexMap.Data = null;
            //}
        }

        /// <summary>
        ///		Initializes a blank mesh.
        /// </summary>
        public Mesh(MeshFlags flags)
        {
            _flags = flags;
        }

        #endregion
    }

    #endregion
}
