/* 
 * File: Bmp Factory.cs
 *
 * This source file contains the declaration of the BmpFactory class, which is
 * used to load pixelmaps from a windows bitmap file (.bmp).
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System.Collections;
using System.IO;
using System;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Graphics.PixelMapFactorys
{

    /// <summary>
    ///     The BmpFactory class is used to load pixelmaps from a windows
    ///     bitmap file (.bmp).
    /// </summary>
    public sealed class BMPFactory : PixelMapFactory
	{
		#region Methods

		public BMPFactory()
        {
            Register();
        }

		/// <summary>
		///     This method is called when PixelMap save is requested, if it returns true
		///		the calling method will stop illiterating through the PixelMapFactorys and 
		///		return success to the user.
		/// </summary>
		/// <param name="path">File path or object of the image to load.</param>
		/// <param name="pixelMap">PixelMap to save.</param>
		/// <param name="flags">Bitmask of flags defining how the pixel map should be saved.</param>
		/// <returns>True if the save was successfull else false.</returns>
		protected override bool RequestSave(object path, PixelMap pixelMap, PixelMapSaveFlags flags)
		{
			if (path.ToString().ToLower().EndsWith(".bmp") == false &&
				path.ToString().ToLower().EndsWith(".dib") == false) return false;

			Stream stream = StreamFactory.RequestStream(path, StreamMode.Truncate);
			if (stream == null) return false;

			BinaryWriter writer = new BinaryWriter(stream);

			// Work out how to save.
			int bitCount = 24;
			if ((flags & PixelMapSaveFlags.BITDEPTH1) != 0)
				bitCount = 1;
			if ((flags & PixelMapSaveFlags.BITDEPTH4) != 0)
				bitCount = 4;
			if ((flags & PixelMapSaveFlags.BITDEPTH8) != 0)
				bitCount = 8;
			if ((flags & PixelMapSaveFlags.BITDEPTH24) != 0)
				bitCount = 24;
			if ((flags & PixelMapSaveFlags.BITDEPTH32) != 0)
				bitCount = 32;

			// Write in header.
			writer.Write(new char[2] { 'B', 'M' });

			// Write in file header.
			writer.Write((int)0);
			writer.Write((short)0);
			writer.Write((short)0);
			writer.Write((int)54);

			// Write in image header.
			writer.Write((int)40);
			writer.Write((int)pixelMap.Width);
			writer.Write((int)pixelMap.Height);
			writer.Write((short)1);
			writer.Write((short)bitCount);
			writer.Write((int)0);
			writer.Write((int)0);
			writer.Write((int)3780);
			writer.Write((int)3780);
			writer.Write((int)0);
			writer.Write((int)0);

			// Work out the horizontal padding size.
			int paddingWidth = (int)((float)pixelMap.Width * (float)bitCount / 8.0f);
			while (paddingWidth % 4 != 0) paddingWidth++;

			// Grab a few usefull details from the pixel map.
			int maskColor = pixelMap.MaskColor, maskRed, maskGreen, maskBlue, maskAlpha;
			ColorMethods.SplitColor(ColorFormats.A8R8G8B8, maskColor, out maskRed, out maskGreen, out maskBlue, out maskAlpha);

			// Write in based of bit count
			switch (bitCount)
			{
				case 1:

					// Write out our lovely monocromatic colors, black and white obviously :).
					writer.Write((int)ColorMethods.CombineColor(ColorFormats.A8R8G8B8, ColorFormats.B8G8R8, unchecked((int)0xFF000000)));
					writer.Write((int)ColorMethods.CombineColor(ColorFormats.A8R8G8B8, ColorFormats.B8G8R8, unchecked((int)0xFFFFFFFF)));

					// Write byte here!
					for (int y = pixelMap.Height - 1; y >= 0; y--)
					{
						int bit = 128;
						int bitMask = 0;

						for (int x = 0; x < pixelMap.Width; x++)
						{
							// Mask with bit if pixel should be black.
							int red, green, blue, alpha;
							ColorMethods.SplitColor(ColorFormats.A8R8G8B8, pixelMap[x, y], out red, out green, out blue, out alpha);
							if (red > 128 || green > 128 || blue > 128) 
								bitMask |= bit;

							// Flips horizontal direction at every 8th horizontal pixel.
							// Update bit mask.
							bit >>= 1;
							if (bit < 1)
							{
								writer.Write((byte)bitMask);
								bit     = 128;
								bitMask = 0;
							}
						}

						// Write in last bit if neccessary.
						if (bitMask > 0) 
							writer.Write((byte)bitMask);

						// Pad out scan line.
						for (int i = (pixelMap.Width / 8); i < paddingWidth - 1; i++) 
							writer.Write((byte)0);
					}

					break;

				//case 4: // - Can't think when this would be needed? Can you?

				//	break;

				//case 8: // Same as 4bit.

				//	break;

				case 24:

					for (int y = pixelMap.Height - 1; y >= 0; y--)
					{
						for (int x = 0; x < paddingWidth / (bitCount / 8); x++)
						{
							int red = 0, green = 0, blue = 0, alpha = 0;
							if (x < pixelMap.Width) 
							{
								// If the pixel map is masked we need to substitute the pixel color
								// for the correct mask color or we will just end up with an invisible pixel (>_>).
								if (pixelMap[x, y] != pixelMap.MaskColor) 
								{
									ColorMethods.SplitColor(ColorFormats.A8R8G8B8, pixelMap[x, y], out red, out green, out blue, out alpha);
								}
								else
								{
									blue   = maskBlue;
									green  = maskGreen;
									red	   = maskRed;
								}
								writer.Write((byte)blue);
								writer.Write((byte)green);
								writer.Write((byte)red);
							}
							else
							{
								writer.Write((byte)0);
							}
						}
					}

					break;

				case 32:

					for (int y = pixelMap.Height - 1; y >= 0; y--)
					{
						for (int x = 0; x < paddingWidth / (bitCount / 8); x++)
						{
							int red = 0, green = 0, blue = 0, alpha = 0;
							if (x < pixelMap.Width) 
							{
								// If the pixel map is masked we need to substitute the pixel color
								// for the correct mask color or we will just end up with an invisible pixel (>_>).
								if (pixelMap[x, y] != pixelMap.MaskColor) 
								{
									ColorMethods.SplitColor(ColorFormats.A8R8G8B8, pixelMap[x, y], out red, out green, out blue, out alpha);
								}
								else
								{
									blue   = maskBlue;
									green  = maskGreen;
									red	   = maskRed;
									alpha  = maskAlpha;
								}
								writer.Write((byte)blue);
								writer.Write((byte)green);
								writer.Write((byte)red);
								writer.Write((byte)alpha);
							}
							else
							{
								writer.Write((byte)0);
							}
						}
					}

					break;

				default:

					stream.Close();
					throw new Exception(bitCount+"bit bmp saving is not supported");

			}

			// Seek back to start and write in file size
			stream.Flush();
			int fileSize = (int)stream.Length;
			stream.Seek(2, SeekOrigin.Begin);
			writer.Write(fileSize);

			// Seek back to start and write in image size.
			stream.Seek(34, SeekOrigin.Begin);
			writer.Write(fileSize - 54);

			// Cleanup stream and return success.
			stream.Close();
			return true;
		}

        /// <summary>
        ///     This method is called when PixelMap load is requested, if you return a PixelMap 
        ///     it will return it from the user else it will keep trying all the other PixelMapLoaders
        ///     until it does get one
        /// </summary>
        /// <param name="file">File path of the image to load.</param>
        /// <returns>A PixelMap or NULL if this factory can't load the given image file.</returns>
		protected override PixelMap RequestLoad(object path)
        {
            Stream stream = StreamFactory.RequestStream(path, StreamMode.Open);
			if (stream == null) return null;
			BinaryReader reader = new BinaryReader(stream);

			if (reader.ReadByte() == 'B' && reader.ReadByte() == 'M')
            {

                // Read in file header.
                int fileSize        = reader.ReadInt32();
                short reserved1     = reader.ReadInt16();
                short reserved2     = reader.ReadInt16();
                int pixelDataOffset = reader.ReadInt32();

                // Read in image header
                int imageHeaderSize         = reader.ReadInt32();
                int imageWidth              = reader.ReadInt32();
                int imageHeight             = reader.ReadInt32();
                short imagePlanes           = reader.ReadInt16();
                short imageBitCount         = reader.ReadInt16();
                int imageCompression        = reader.ReadInt32();
                int imageSize               = reader.ReadInt32();
                int imageXPixPerMeter       = reader.ReadInt32();
                int imageYPixPerMeter       = reader.ReadInt32();
                int imageColorMapUsed       = reader.ReadInt32();
                int imageColorMapImportant  = reader.ReadInt32();
				if (imageCompression > 0)
				{
					stream.Close();
					return null;
				}

				byte[] pixels;
				int[]  pallete;
				byte[] tempPallete;
				PixelMap pixelMap= new PixelMap(imageWidth, imageHeight);

				// Work out the horizontal padding size.
				int paddingWidth = (int)((float)imageWidth * (float)imageBitCount / 8.0);
				while (paddingWidth % 4 != 0) paddingWidth++;

                switch (imageBitCount)
                {
                    case 1: // Monochrome

						int color1 = ColorMethods.CombineColor(ColorFormats.B8G8R8, ColorFormats.A8R8G8B8, reader.ReadInt32());
						int color2 = ColorMethods.CombineColor(ColorFormats.B8G8R8, ColorFormats.A8R8G8B8, reader.ReadInt32());

						for (int y = imageHeight - 1; y >= 0; y--)
						{
							pixels = reader.ReadBytes(paddingWidth);
							for (int x = 0; x < imageWidth; x++)  
								pixelMap[x, y] = ((pixels[x >> 3] & (128 >> (x & 7))) != 0) ? color2 : color1;
                        }

						break;

                    case 4: // 16 colors.

						pallete = new int[16];
						tempPallete = new byte[16 * 4];
						reader.Read(tempPallete, 0, 16 * 4);
						for (int i = 0; i < 16; i++)
						{
							pallete[i] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, tempPallete[(i * 4) + 2], tempPallete[(i * 4) + 1], tempPallete[(i * 4)], 255);
						}

						for (int y = imageHeight - 1; y >= 0; y--)
						{
							pixels = reader.ReadBytes(paddingWidth);
							for (int x = 0; x < imageWidth; x++)
								pixelMap[x, y] = pallete[(x % 2 == 0) ? (pixels[x / 2] >> 4) : (pixels[x / 2] & 0x0F)];
						}

						break;

                    case 8: // 256 colors.

                        pallete = new int[256];
						tempPallete = new byte[256 * 4];
						reader.Read(tempPallete, 0, 256 * 4);
						for (int i = 0; i < 256; i++)
						{
							pallete[i] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, tempPallete[(i * 4) + 2], tempPallete[(i * 4) + 1], tempPallete[(i * 4)], 255);
						}

						for (int y = imageHeight - 1; y >= 0; y--)
						{
							pixels = reader.ReadBytes(paddingWidth);
							for (int x = 0; x < imageWidth; x++)  
								pixelMap[x, y] = pallete[pixels[x]];
                        }

						break;

                    case 24: // 16,777,216 colors

						for (int y = imageHeight - 1; y >= 0; y--)
						{
							pixels = reader.ReadBytes(paddingWidth);
							for (int x = 0; x < imageWidth; x++)
								pixelMap[x, y] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, pixels[(x * 3) + 2], pixels[(x * 3) + 1], pixels[(x * 3)], 255);
						}

						break;

                    case 32: // 16,777,216 colors with alpha

						for (int y = imageHeight - 1; y >= 0; y--)
						{
							pixels = reader.ReadBytes(paddingWidth);
							for (int x = 0; x < imageWidth; x++)
								pixelMap[x, y] = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, pixels[(x * 4) + 2], pixels[(x * 4) + 1], pixels[(x * 4)], pixels[(x * 4) + 3]);
						}

						break;

					default:

						// The user has either tried to load an corrupt file, or
						// a file with an unsupported bit count.
						throw new Exception("Attempt to load BMP file with an bit format storing the image data.");

                }

				stream.Close();
                return pixelMap;
            
			}
            else // Header check
            {
                reader.Close();
				return null;
			}

		}
		#endregion
	}

}