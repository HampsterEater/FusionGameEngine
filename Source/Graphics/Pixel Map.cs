/* 
 * File: PixelMap.cs
 *
 * This source file contains the declaration of several PixelMap classes which are
 * used as an intermediate stage between loading an image from a file and 
 * creating a texture from it in one of the graphics APIs.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 * 
 * 10 July 2006:
 *		- To bring loading times down to a minimum all pixelmaps now have a static
 *		  format (A8R8G8B8), this shaves about 30ms loading time off as format-2-format 
 *		  conversions don't have to take place.
 */

using System.Collections;

namespace BinaryPhoenix.Fusion.Graphics
{
	#region Enumerations

	/// <summary>
	///		Desciribes how a pixelmap should be saved, can be combined together
	///		into a bitmask.
	/// </summary>
	public enum PixelMapSaveFlags : int
	{
		BITDEPTH1		= 0x000001,
		BITDEPTH4		= 0x000002,
		BITDEPTH8		= 0x000004,
		BITDEPTH16		= 0x000008,
		BITDEPTH24		= 0x000010,
		BITDEPTH32		= 0x000020,
		RLECOMPRESS		= 0x000040
	}

	#endregion
	#region PixelMap Classes
	/// <summary>
    ///     The PixelMap class is used to store image data that has been loaded 
    ///     from an image file (png, bmp..etc) in an intermediate format (A8R8G8B8 color format) that
    ///     can be easily loaded into various graphics API's
    /// </summary>
    public sealed class PixelMap
	{
		#region Members
		#region Variables
		
		private int[] _pixels;      // Stores details on every pixel in the PixelMap
        private int _width,  _height;
		private int _maskColor;

        private bool _ignoreAlphaChannel;
		
		#endregion
		#region Properties
		/// <summary>
        ///     Permits array like access to the PixelMaps pixel data.
        /// </summary>
        /// <param name="x">Position of the pixel on the x-axis.</param>
        /// <param name="y">Position of the pixel on the y-axis.</param>
        /// <returns>The pixel at x/y.</returns>
        public int this[int x, int y]
        {
            get { return _pixels[x + (y * _width)];   }
			set { _pixels[x + (y * _width)] = value; }
        }

		/// <summary>
		///     Permits array like access to the PixelMaps pixel data.
		/// </summary>
		/// <param name="index">Position of the pixel in data array.</param>
		/// <returns>The pixel at index.</returns>
		public int this[int index]
		{
			get { return _pixels[index]; }
			set { _pixels[index] = value; }
		}

        /// <summary>
        ///     If set to true the alpha channel of this pixelmap will be ignored when it is used as an image.
        /// </summary>
        public bool IgnoreAlphaChannel
        {
            get { return _ignoreAlphaChannel; }
            set { _ignoreAlphaChannel = value; }
        }

		/// <summary>
		///		Allows direct access to the PixelMaps data array.
		/// </summary>
		public int[] Data
		{
			get { return _pixels;   }
			set { _pixels = value;  }
		}

		/// <summary>
		///		Sets or gets the current width of this PixelMap.
		/// </summary>
        public int Width
        {
            get { return _width; }
            set 
            {
                _width = value;
                _pixels = new int[_width * _height];
            }
        }

		/// <summary>
		///		Sets or gets the current height of this PixelMap.
		/// </summary>
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                _pixels = new int[_width * _height];
            }
		}

		/// <summary>
		///		Sets or gets the mask color of this PixelMap.
		/// </summary>
		public int MaskColor
		{
			get { return _maskColor; }
			set { Mask(value);  }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
        ///     Constructs a PixelMap of width / height.
        /// </summary>
        /// <param name="width">Width of the PixelMap to create.</param>
        public PixelMap(int width, int height)
        {
            _width  = width;
            _height = height;
            _pixels = new int[width * height];
        }

		/// <summary>
		///		Creates an exact copy of this PixelMap and returns it.
		/// </summary>
		/// <returns>The newly copied PixelMap.</returns>
		public PixelMap Copy()
		{
			PixelMap pixelMap = new PixelMap(_width,_height);
			pixelMap.Data = (int[])_pixels.Clone();
            pixelMap.MaskColor = _maskColor;
            pixelMap.IgnoreAlphaChannel = _ignoreAlphaChannel;
			return pixelMap;
		}

		/// <summary>
		///		Creates an copy of a specific area of this PixelMap and returns it.
		/// </summary>
		/// <param name="sectionX">Position on the x-axis to start copying from.</param>
		/// <param name="sectionY">Position on the y-axis to start copying from.</param>
		/// <param name="sectionWidth">Width of section to copy.</param>
		/// <param name="sectionHeight">Height of section to copy.</param>
		/// <returns>The newly copied PixelMap.</returns>
		public PixelMap Copy(int sectionX,int sectionY,int sectionWidth,int sectionHeight)
		{
			PixelMap pixelMap = new PixelMap(sectionWidth, sectionHeight);

			for (int x = 0; x < sectionWidth; x++)
				for (int y = 0; y < sectionHeight; y++)
					pixelMap[x,y] = this[sectionX + x, sectionY + y];
                    //pixelMap.Data[x + (y * sectionWidth)] = _pixels[(x + sectionX) + ((y + sectionY) * _width)];

			return pixelMap;
		}

		/// <summary>
		///		Resizes the PixelMap, any extra space will be padded with invisible
		///		pixels.
		/// </summary>
		/// <param name="width">New width to resize to.</param>
		/// <param name="height">New height to resize to.</param>
		public void Resize(int width, int height)
		{
			int[] newPixels = new int[width * height];
			for (int x = 0; x < _width; x++)
				for (int y = 0; y < _height; y++)
					newPixels[x + (y * width)] = _pixels[x + (y * _width)];

			_width = width;
			_height = height;
			_pixels = newPixels;
		}

		/// <summary>
		///		Fills the PixelMap with a specific ARGB color.
		/// </summary>
		/// <param name="color">Color to fill PixelMap with.</param>
		public void Fill(int color)
		{
			for (int i = 0; i < _pixels.Length; i++)
				_pixels[i] = color;
		}

        /// <summary>
        ///     Flips this pixelmap horizontally or vertically.
        /// </summary>
        /// <param name="h">If set to true this pixelmap will be flipped horizontally.</param>
        /// <param name="v">If set to true this pixelmap will be flipped vertically.</param>
        public void Flip(bool h, bool v)
        {
            int[] newPixels = new int[_pixels.Length];
            if (h == true)
            {
                for (int x = 0; x < _width; x++)
                    for (int y = 0; y < _height; y++)
                        newPixels[x + (y * _width)] = this[(_width - 1) - x, y];
            }
            if (v == true)
            {
                for (int x = 0; x < _width; x++)
                    for (int y = 0; y < _height; y++)
                        newPixels[x + (y * _width)] = this[x, (_height - 1) - y];
            }
            _pixels = newPixels;
        }

		/// <summary>
		///		Masks the PixelMap with a specific ARGB color key.
		/// </summary>
		/// <param name="color">ARGB color key to mask PixelMap with.</param>
		public void Mask(int color)
		{
			for (int i = 0; i < _pixels.Length; i++)
				if (_pixels[i] == color) _pixels[i] = 0x00000000;

			_maskColor = color;
		}

		/// <summary>
		///		Pastes the contents of a PixelMap onto this PixelMap.
		/// </summary>
		/// <param name="pixelMap">pixelMap to paste.</param>
		/// <param name="x">Position on the x-axis to start pasting from.</param>
		/// <param name="y">Position on the y-axis to start pasting from.</param>
		public void Paste(PixelMap pixelMap, int x, int y)
		{
			for (int _x = 0; _x < pixelMap.Width; _x++)
				for (int _y = 0; _y < pixelMap.Height; _y++)
				{
					int _xPos = x + _x;
					int _yPos = y + _y;
					if (_xPos >= 0 && _yPos >= 0 && _xPos < _width && _yPos < _height)
					{
						_pixels[_xPos + (_yPos * _width)] = pixelMap.Data[_x + (_y * pixelMap.Width)];
					}
				}
		}

		#endregion
	}
	#endregion
	#region PixelMap Factory Classes
	/// <summary>
    ///     The PixelMapFactory class is used as a way to easily add new image 
    ///     formats, all you need to do is inheret a class of this and call the
    ///     register method.
    /// 
    ///     When an pixelmap load is requested the LoadPixelMap method will be called for
    ///     all registered PixelMapFactorys, if it returns a pixmap, then the pixmap will 
    ///     be returned to the user, else it will continue through 
    ///     the list of PixelMapFactorys until and instance does return one.
    /// </summary>
    public abstract class PixelMapFactory
	{
		#region Members
		#region Variables

		private static ArrayList _loaderList = new ArrayList();

		#endregion
        #endregion
        #region Methods
        /// <summary>
        ///     This method is called when PixelMap load is requested, if you return a PixelMap 
        ///     it will return it from the user else it will keep trying all the other PixelMapLoaders
        ///     until it does get one
        /// </summary>
        /// <param name="path">File path or object of the image to load.</param>
        /// <returns>A PixelMap or NULL if this factory can't load the given image file.</returns>
        protected abstract PixelMap RequestLoad(object path);

		/// <summary>
		///     This method is called when PixelMap save is requested, if it returns true
		///		the calling method will stop illiterating through the PixelMapFactorys and 
		///		return success to the user.
		/// </summary>
		/// <param name="path">File path or object of the image to load.</param>
		/// <param name="pixelMap">PixelMap to save.</param>
		/// <param name="flags">Bitmask of flags defining how the pixel map should be saved.</param>
		/// <returns>True if the save was successfull else false.</returns>
		protected abstract bool RequestSave(object path, PixelMap pixelMap, PixelMapSaveFlags flags);

        /// <summary>
        ///     This method is called when a PixelMap load is requested, it illiterates through all
        ///     the registered PixelMapFactory instances to see if there is one capable to loading the
        ///     given format.
        /// </summary>
        /// <param name="path">File path or object of the image to load.</param>
        /// <returns>A PixelMap or NULL if it can't find a factory able to load the given image format.</returns>
        public static PixelMap LoadPixelMap(object path) 
        {
            foreach(PixelMapFactory factory in _loaderList) 
            {
				PixelMap pixelMap = factory.RequestLoad(path);
                if (pixelMap != null) return pixelMap;
            }
            return null;
        }

		/// <summary>
		///     This method is called when a PixelMap save is requested, it illiterates through all
		///     the registered PixelMapFactory instances to see if there is one capable to saving the
		///     given format.
		/// </summary>
		/// <param name="path">File path or save pixel map to.</param>
		/// <param name="flags">Bitmask of flags to define how the pixel map should be saved.</param>
		/// <returns>True if save was successfull else false.</returns>
		public static bool SavePixelMap(object path, PixelMap pixelMap, PixelMapSaveFlags flags)
		{
			foreach (PixelMapFactory factory in _loaderList)
			{
				if (factory.RequestSave(path, pixelMap, flags) == true) return true;
			}
			return false;
		}
		public static void SavePixelMap(object path, PixelMap pixelMap)
		{
			SavePixelMap(path, pixelMap, 0);
		}

        /// <summary>
        ///     Adds (registers) an instance to the LoaderList, so it will be used when a PixelMap load 
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