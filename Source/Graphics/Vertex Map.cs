/* 
 * File: VertexMap.cs
 *
 * This source file contains the declaration of several VertexMap classes which are
 * used as an intermediate stage between loading an mesh from a file and 
 * creating a driver mesh from it in one of the graphics APIs.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 *
 */

using System.Collections;

namespace BinaryPhoenix.Fusion.Graphics
{
	#region Enumerations

	/// <summary>
	///		Desciribes how a mesh should be saved, can be combined together
	///		into a bitmask.
	/// </summary>
    public enum VertexMapSaveFlags : int
	{
		UNKNOWN
	}

	#endregion
	#region VertexMap Classes
	/// <summary>
    ///     The VertexMap class is used to store mesh data that has been loaded 
    ///     from an mesh file (raw, b3d..etc) in an intermediate format that
    ///     can be easily loaded into various graphics API's
    /// </summary>
    public sealed class VertexMap
	{
		#region Members
		#region Variables
		
		private Vertex[] _vertexs = null;
        private float _width, _height, _depth;

		#endregion
		#region Properties

		/// <summary>
		///     Permits array like access to the VertexMaps pixel data.
		/// </summary>
		/// <param name="index">Position of the vertex in data array.</param>
		/// <returns>The vertex at index.</returns>
		public Vertex this[int index]
		{
			get { return _vertexs[index]; }
            set { _vertexs[index] = value; CalculateDimensions(); }
		}

		/// <summary>
		///		Allows direct access to the VertexMaps data array.
		/// </summary>
		public Vertex[] Data
		{
			get { return _vertexs;   }
            set { _vertexs = value; CalculateDimensions(); }
		}

        /// <summary>
        ///		Gets the width of this vertex map.
        /// </summary>
        public float Width
        {
            get { return _width; }
        }

        /// <summary>
        ///		Gets the height of this vertex map.
        /// </summary>
        public float Height
        {
            get { return _height; }
        }

        /// <summary>
        ///		Gets the depth of this vertex map.
        /// </summary>
        public float Depth
        {
            get { return _depth; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
        ///     Constructs an empty vertex map.
        /// </summary>
        /// <param name="count">Number of vertexs in this map.</param>
        public VertexMap(int count)
        {
            _vertexs = new Vertex[count];
        }

        /// <summary>
        ///     Calculates the width, height and depth of this vertex map.
        /// </summary>
        public void CalculateDimensions()
        {
            float minX = 0, maxX = 0;
            float minY = 0, maxY = 0;
            float minZ = 0, maxZ = 0;
            foreach (Vertex vertex in _vertexs)
            {
                if (vertex.X < minX) minX = vertex.X;
                if (vertex.X > maxX) maxX = vertex.X;
                if (vertex.Y < minY) minY = vertex.Y;
                if (vertex.Y > maxY) maxY = vertex.Y;
                if (vertex.Z < minZ) minZ = vertex.Z;
                if (vertex.Z > maxZ) maxZ = vertex.Z;
            }
            _width = (maxX - minX);
            _height = (maxY - minY);
            _depth = (maxZ - minZ);
        }

        /// <summary>
        ///		Creates an exact copy of this VertexMap and returns it.
        /// </summary>
        /// <returns>The newly copied VertexMap.</returns>
        public VertexMap Copy()
		{
			VertexMap vertexMap = new VertexMap(_vertexs.Length);
			vertexMap.Data = (Vertex[])_vertexs.Clone();
            vertexMap._width = _width;
            vertexMap._height = _height;
            vertexMap._depth = _depth;
			return vertexMap;
		}

		#endregion
	}
	#endregion
	#region VertexMap Factory Classes
	/// <summary>
    ///     The VertexMapFactory class is used as a way to easily add new mesh
    ///     formats, all you need to do is inheret a class of this and call the
    ///     register method.
    /// 
    ///     When an vertexmap load is requested the LoadVertexMap method will be called for
    ///     all registered VertexMapFactorys, if it returns a vertexMap, then the vertexMap will 
    ///     be returned to the user, else it will continue through 
    ///     the list of VertexMapFactorys until and instance does return one.
    /// </summary>
    public abstract class VertexMapFactory
	{
		#region Members
		#region Variables

		private static ArrayList _loaderList = new ArrayList();

		#endregion
		#endregion
		#region Methods
		/// <summary>
        ///     This method is called when VertexMap load is requested, if you return a VertexMap 
        ///     it will return it from the user else it will keep trying all the other VertexMapLoaders
        ///     until it does get one
        /// </summary>
        /// <param name="path">File path or object of the image to load.</param>
        /// <returns>A VertexMap or NULL if this factory can't load the given mesh file.</returns>
        protected abstract VertexMap RequestLoad(object path);

		/// <summary>
		///     This method is called when VertexMap save is requested, if it returns true
		///		the calling method will stop illiterating through the VertexMapFactorys and 
		///		return success to the user.
		/// </summary>
		/// <param name="path">File path or object of the image to load.</param>
		/// <param name="pixelMap">VertexMap to save.</param>
		/// <param name="flags">Bitmask of flags defining how the vertex map should be saved.</param>
		/// <returns>True if the save was successfull else false.</returns>
		protected abstract bool RequestSave(object path, VertexMap vertexMap, VertexMapSaveFlags flags);

        /// <summary>
        ///     This method is called when a VertexMap load is requested, it illiterates through all
        ///     the registered VertexMapFactory instances to see if there is one capable to loading the
        ///     given format.
        /// </summary>
        /// <param name="path">File path or object of the mesh to load.</param>
        /// <returns>A VertexMap or NULL if it can't find a factory able to load the given mesh format.</returns>
        public static VertexMap LoadVertexMap(object path) 
        {
            foreach(VertexMapFactory factory in _loaderList) 
            {
				VertexMap vertexMap = factory.RequestLoad(path);
                if (vertexMap != null) return vertexMap;
            }
            return null;
        }

		/// <summary>
		///     This method is called when a VertexMap save is requested, it illiterates through all
		///     the registered VertexMapFactory instances to see if there is one capable to saving the
		///     given format.
		/// </summary>
		/// <param name="path">File path to save vertex map to.</param>
		/// <param name="flags">Bitmask of flags to define how the vertex map should be saved.</param>
		/// <returns>True if save was successfull else false.</returns>
		public static bool SaveVertexMap(object path, VertexMap vertexMap, VertexMapSaveFlags flags)
		{
			foreach (VertexMapFactory factory in _loaderList)
			{
				if (factory.RequestSave(path, vertexMap, flags) == true) return true;
			}
			return false;
		}
		public static void SaveVertexMap(object path, VertexMap vertexMap)
		{
			SaveVertexMap(path, vertexMap, 0);
		}

        /// <summary>
        ///     Adds (registers) an instance to the LoaderList, so it will be used when a VertexMap load 
        ///     is requested.
        /// </summary>
        protected void Register() 
        {
            _loaderList.Add(this);
        }

		#endregion
	}
	#endregion
}