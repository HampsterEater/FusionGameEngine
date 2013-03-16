/* 
 * File: Image.cs
 *
 * This source file contains the declaration of the Image class which encapsulates
 * all the details needed to render image data to the screen. It also contains the 
 * ImageFrame class which is used to isolate the graphics API rendering from the game.
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
    ///     Used to store details on a single frame of animation (sort of a sub-image), and
    ///     to isolate graphics API rendering of images from the game.
    /// </summary>
	public interface IImageFrame //: IRenderTarget
	{
		#region Methods

		IGraphicsDriver Driver { get; }
		PixelMap PixelMap { get; set; }
        Point Origin { get; set; }
		void Render(float x, float y, float z);
        void Tile(float x, float y, float z);
		
		#endregion
	}

	#endregion
    #region Enumeration

    /// <summary>
    ///     Describes how and image should be loaded, rendered and stored.
    /// </summary>
    public enum ImageFlags : int
    {
        Dynamic = 1,
    }

    #endregion
    #region Image Classes

    /// <summary>
    ///     Used to store details on a single piece of renderable image data.
    /// </summary>
    public class Image 
	{
		#region Members
		#region Variables

		private PixelMap _fullPixelMap = null;
		
		private IImageFrame[] _frames = new IImageFrame[1];
		private int _width, _height;
		private int _hSpacing = 0, _vSpacing = 0;
        private ImageFlags _flags = 0;

		private string _url = "";

        private int _mask = unchecked((int)0xFFFF00FF);

		#endregion
		#region Properties

		/// <summary>
		///		Gets the full pixel map as it was in the image file.
		/// </summary>
		public PixelMap FullPixelMap
		{
			get 
            {
                if ((_flags & ImageFlags.Dynamic) == 0)
                    throw new Exception("Pixelmap data can't be modified in none dynamic image.");
                return _fullPixelMap; 
            }
			set 
            {
                if ((_flags & ImageFlags.Dynamic) == 0)
                    throw new Exception("Pixelmap data can't be modified in none dynamic image.");
                _fullPixelMap = value; 
                CreateFrames(); 
            }
		}

		/// <summary>
		///		Gets the horizontal spacing of each frames cell.
		/// </summary>
		public int HorizontalSpacing
		{
			get { return _hSpacing; }
			set 
            {
                if ((_flags & ImageFlags.Dynamic) == 0)
                    throw new Exception("Pixelmap data can't be modified in none dynamic image.");
                _hSpacing = value; 
                CreateFrames(); 
            }
		}

        /// <summary>
        ///     Sets the origin of all this images frames. The origin is 
        ///     the offset from the top-left corner used when rendering this image.
        /// </summary>
        public Point Origin
        {
            set 
            { 
                for (int i = 0; i < _frames.Length; i++)
                    if (_frames[i] != null) _frames[i].Origin = value;
            }
            get { return _frames[0] != null ? _frames[0].Origin : new Point(0,0); }
        }

        /// <summary>
        ///		Gets the vertical spacing of each frames cell.
        /// </summary>
        public int VerticalSpacing
		{
			get { return _vSpacing; }
			set 
            {
                if ((_flags & ImageFlags.Dynamic) == 0)
                    throw new Exception("Pixelmap data can't be modified in none dynamic image.");
                _vSpacing = value; 
                CreateFrames(); 
            }
		}

		/// <summary>
		///		Gets the width of this image.
		/// </summary>
        public int Width
        {
            get { return _width; }
			set 
            {
                if ((_flags & ImageFlags.Dynamic) == 0)
                    throw new Exception("Pixelmap data can't be modified in none dynamic image.");
                _width = value; 
                CreateFrames(); 
            } 
        }

		/// <summary>
		///		Gets the height of this image.
		/// </summary>
        public int Height
        {
            get { return _height; }
			set
            {
                if ((_flags & ImageFlags.Dynamic) == 0)
                    throw new Exception("Pixelmap data can't be modified in none dynamic image.");
                _height = value; 
                CreateFrames(); 
            } 
        }

		/// <summary>
		///		Gets the full (before its its cut up into individual animation frames) width of this image.
		/// </summary>
		public int FullWidth
		{
			get { return _fullPixelMap.Width; }
			set 
            {
                if ((_flags & ImageFlags.Dynamic) == 0)
                    throw new Exception("Pixelmap data can't be modified in none dynamic image.");
                _fullPixelMap.Resize(value, _fullPixelMap.Height); 
                CreateFrames(); 
            }
		}

		/// <summary>
		///		Gets the full (before its its cut up into individual animation frames) height of this image.
		/// </summary>
		public int FullHeight
		{
			get { return _fullPixelMap.Height; }
			set 
            {
                if ((_flags & ImageFlags.Dynamic) == 0)
                    throw new Exception("Pixelmap data can't be modified in none dynamic image.");
                _fullPixelMap.Resize(_fullPixelMap.Width, value); 
                CreateFrames(); 
            }
		}

		/// <summary>
		///		Gets the amount of frames in this image.
		/// </summary>
		public int FrameCount
		{
			get { return _frames.Length; }
			set 
            {
                if ((_flags & ImageFlags.Dynamic) == 0)
                    throw new Exception("Pixelmap data can't be modified in none dynamic image.");
                Array.Resize<IImageFrame>(ref _frames, value); 
            }
		}

        /// <summary>
        ///		Gets the amount of color key of this image.
        /// </summary>
        public int ColorKey
        {
            get { return _mask; }
            set 
            {
                if ((_flags & ImageFlags.Dynamic) == 0)
                    throw new Exception("Pixelmap data can't be modified in none dynamic image.");
                _mask = value;
                CreateFrames();
            }
        }

		/// <summary>
		///     Sets or gets the IImageFrame at the given index.
		/// </summary>
		/// <param name="index">Index of IImageFrame to get or set.</param>
		/// <returns>IImageFrame atn given index.</returns>
		public IImageFrame this[int index]
        {
            get 
			{
				RecreateFrame(index);
				return _frames[index]; 
			}
            set { _frames[index] = value; }
		}

		/// <summary>
		///		Gets or sets the url this image was loaded from.
		/// </summary>
		public string URL
		{
			get { return _url; }
			set { _url = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Deconstructs this image and all its frames.
		/// </summary>
		~Image()
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
			_fullPixelMap = null;
			if (_frames == null) return;
			for (int i = 0; i < _frames.Length; i++)
            {
				_frames[i] = null;
			}
            _frames = null;
		}

		/// <summary>
		///		Recreates the pixel maps and resources required to render this image.
		/// </summary>
		public void CreateFrames()
		{
			if (_frames == null) return;
            if (_fullPixelMap != null)
            {
                if (_width > _fullPixelMap.Width) _width = _fullPixelMap.Width;
                if (_height > _fullPixelMap.Height) _height = _fullPixelMap.Height;

                int cellsH = (_fullPixelMap.Width / (_width + _hSpacing));
                int cellsV = (_fullPixelMap.Height / (_height + _vSpacing));
                _frames = new IImageFrame[cellsH * cellsV];
                for (int i = 0; i < _frames.Length; i++)
                {
                    int px = (i % cellsH) * _width;
                    int py = (i / cellsH) * _height;
                    PixelMap map = _fullPixelMap.Copy(px + ((i % cellsH) * _hSpacing), py + ((i / cellsH) * _vSpacing), _width, _height);
                    map.Mask(GraphicsManager.ColorKey);

                    _frames[i] = GraphicsManager.CreateImageFrame(map);
                }
            }
            else
            {
                    for (int i = 0; i < _frames.Length; i++)
                        _frames[i] = GraphicsManager.CreateImageFrame(new PixelMap(_width, _height));
            }
            _mask = GraphicsManager.ColorKey;
        }

		/// <summary>
		///		Calculates the index of the frame at the given position.
		/// </summary>
		/// <param name="x">Position on the x-axis of the frame.</param>
		/// <param name="y">Position on the y-axis of the frame.</param>
		public int FrameIndexAtPosition(int x, int y)
		{
			// This needs speeding up, I know there are better ways of doing it ^^.
			int cellsH = (_fullPixelMap.Width / (_width + _hSpacing));
			int cellsV = (_fullPixelMap.Height / (_height + _vSpacing));
			for (int i = 0; i < _frames.Length; i++)
			{
				int px = (i % cellsH) * _width;
				int py = (i / cellsH) * _height;
				if (px >= x && py >= y && px < x + _width && py < y + _height) return i;
			}
			return -1;
		}

		/// <summary>
		///		This function will recreate an image frame if the graphics driver has been changed.
		/// </summary>
		private void RecreateFrame(int index)
		{

			// Recreate the frame if the current driver has been changed
			if (index <= _frames.Length - 1 && (_frames[index] != null && _frames[index].Driver != GraphicsManager.Driver))
			{
				_frames[index] = null;
                _frames[index] = GraphicsManager.CreateImageFrame(_frames[index].PixelMap);
			}
		}

		/// <summary>
		///		Renders a frame of this image to the screen.
		/// </summary>
		/// <param name="x">Position on the x-axis to render image.</param>
		/// <param name="y">Position on the y-axis to render image.</param>
		/// <param name="z">Position on the z-axis to render image.</param>
		/// <param name="frame">Frame of this image to render.</param>
 		public void Render(float x, float y, float z, int frame)
		{
            if (frame >= _frames.Length || _frames[frame] == null) return;
			RecreateFrame(frame);
			_frames[frame].Render(x, y, z);
        }
		public void Render(float x, float y, float z) 
		{ 
			Render(x,y,z,0); 
		}

        /// <summary>
        ///		Tiles a frame of this image to the screen.
        /// </summary>
        /// <param name="x">Position on the x-axis to render image.</param>
        /// <param name="y">Position on the y-axis to render image.</param>
        /// <param name="z">Position on the z-axis to render image.</param>
        /// <param name="frame">Frame of this image to render.</param>
        public void Tile(float x, float y, float z, int frame)
        {
            if (frame >= _frames.Length || _frames[frame] == null) return;
            RecreateFrame(frame);
            _frames[frame].Tile(x, y, z);
        }
        public void Tile(float x, float y, float z)
        {
            Tile(x, y, z, 0);
        }

		// Disable constructors.
		private Image() { }

		/// <summary>
		///		Initilizes a new image from a file.
		/// </summary>
		/// <param name="path">Memory location (or url) of image file to load.</param>
		public Image(object path, ImageFlags flags)
		{
			if ((path is PixelMap) == false)
			{
				if (path is string) _url = (string)path;
				_fullPixelMap = PixelMapFactory.LoadPixelMap(path);
			}
			else
                _fullPixelMap = (path as PixelMap).Copy();

            if (_fullPixelMap == null)
                return;

            PixelMap map = _fullPixelMap.Copy(); 
            map.Mask(GraphicsManager.ColorKey);
            _mask = GraphicsManager.ColorKey;

			_width = _fullPixelMap.Width;
			_height = _fullPixelMap.Height;
            _flags = flags;

			_frames[0] = GraphicsManager.CreateImageFrame(map);

            // Are we dynamic? If not we don't need the full pixelmaps data so lets destroy it and
            // free up a bit of memory.
            if ((_flags & ImageFlags.Dynamic) == 0 && _fullPixelMap != null)
            {
               _fullPixelMap.Data = null;

                 _frames[0].PixelMap.Data = null;

               //GC.Collect();
            }
		}

		/// <summary>
		///		Initilizes a new image with mutliple frame cells from a file.
		/// </summary>
		/// <param name="path">Memory location (or url) of image file to load.</param>
		/// <param name="cellW">Width of each cell in image.</param>
		/// <param name="cellH">Height of each cell in image.</param>
        public Image(object path, int cellW, int cellH, ImageFlags flags)
		{
			if ((path is PixelMap) == false)
			{
				if (path is string) _url = (string)path;
				_fullPixelMap = PixelMapFactory.LoadPixelMap(path);
			}
			else
				_fullPixelMap = (path as PixelMap).Copy();

            if (_fullPixelMap == null)
                return;

			_width = cellW;
			_height = cellH;
            _flags = flags;

			CreateFrames();

            // Are we dynamic? If not we don't need the full pixelmaps data so lets destroy it and
            // free up a bit of memory.
            if ((_flags & ImageFlags.Dynamic) == 0 && _fullPixelMap != null)
            {
                _fullPixelMap.Data = null;
                    for (int i = 0; i < _frames.Length; i++)
                        _frames[i].PixelMap.Data = null;
               //GC.Collect();
            }
		}

		/// <summary>
		///		Initilizes a new image with mutliple frame cells spaced evenly out from a file.
		/// </summary>
		/// <param name="path">Memory location (or url) of image file to load.</param>
		/// <param name="cellW">Width of each cell in image.</param>
		/// <param name="cellH">Height of each cell in image.</param>
		/// <param name="cellHSeperation">Vertical seperation of each cell in image.</param>
		/// <param name="cellVSeperation">Horizontal seperation of each cell in image.</param>
        public Image(object path, int cellW, int cellH, int cellHSeperation, int cellVSeperation, ImageFlags flags)
		{
			if ((path is PixelMap) == false)
			{
				if (path is string) _url = (string)path;
				_fullPixelMap = PixelMapFactory.LoadPixelMap(path);
			}
			else
                _fullPixelMap = (path as PixelMap).Copy();

            if (_fullPixelMap == null)
                return;

			_width = cellW;
			_height = cellH;
			_hSpacing = cellHSeperation;
			_vSpacing = cellVSeperation;
            _flags = flags;

			CreateFrames();

            // Are we dynamic? If not we don't need the full pixelmaps data so lets destroy it and
            // free up a bit of memory.
            if ((_flags & ImageFlags.Dynamic) == 0 && _fullPixelMap != null)
            {
               _fullPixelMap.Data = null;
                   for (int i = 0; i < _frames.Length; i++)
                       _frames[i].PixelMap.Data = null;
               //GC.Collect();
            }
		}

		/// <summary>
		///		Initializes a blank image of size Width x Height with Frames amount of frames.
		/// </summary>
		/// <param name="width">Width of image to create.</param>
		/// <param name="height">Height of image to create.</param>
		/// <param name="frames">Amount of frames the image should contain.</param>
		public Image(int width, int height, int frames, ImageFlags flags)
		{
			_width  = width;
			_height = height;
            _flags  = flags;

			_frames = new IImageFrame[frames];

                for (int i = 0; i < frames; i++)
                    _frames[i] = GraphicsManager.CreateImageFrame(new PixelMap(_width, _height));

            // Are we dynamic? If not we don't need the full pixelmaps data so lets destroy it and
            // free up a bit of memory.
            if ((_flags & ImageFlags.Dynamic) == 0 && _fullPixelMap != null)
            {
                    for (int i = 0; i < _frames.Length; i++)
                        _frames[i].PixelMap.Data = null;
                    //GC.Collect();
            }
		}

		/// <summary>
		///		Initializes a blank image of size Width x Height with 1 frame.
		/// </summary>
		/// <param name="width">Width of image to create.</param>
		/// <param name="height">Height of image to create.</param>
        public Image(int width, int height, ImageFlags flags)
		{
			_width  = width;
			_height = height;
            _flags = flags;

			_frames = new IImageFrame[1];

    			_frames[0] = GraphicsManager.CreateImageFrame(new PixelMap(_width, _height));

            // Are we dynamic? If not we don't need the full pixelmaps data so lets destroy it and
            // free up a bit of memory.
            if ((_flags & ImageFlags.Dynamic) == 0 && _fullPixelMap != null)
            {
                  _frames[0].PixelMap.Data = null;
                //GC.Collect();
            }
		}

		#endregion
	}
	
	#endregion
}
