/* 
 * File: Bitmap Font.cs
 *
 * This source file contains the declaration of the BitmapFont class which has
 * the functionality to render text from a bitmap font file.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Xml;
using System.Collections;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Graphics
{

	/// <summary>
	///		This class is used to load a render bitmap font. Bitmap fonts can be
	///		loaded from xml bitmap font declaration files.
	/// </summary>
	public sealed class BitmapFont
	{
		#region Members
		#region Variables

		// This is used for the [rainbow] tag, its a list of predeterminded colors
		// that should be looped through for each rainbow tag.
		private int[] _rainbowColorScale = new int[]
			{
				unchecked((int)0xFFFF0000), unchecked((int)0xFFFF6400), unchecked((int)0xFFFFC800),
				unchecked((int)0xFFFFFF00), unchecked((int)0xFFC8FF00), unchecked((int)0xFF64FF00),
				unchecked((int)0xFF00FF00), unchecked((int)0xFF00FF64), unchecked((int)0xFF00C8C8),
				unchecked((int)0xFF0064FF), unchecked((int)0xFF0000FF), unchecked((int)0xFF6400FF),
				unchecked((int)0xFFC800C8), unchecked((int)0xFFFF0064),
			};

		private string _url = "";

		private Image _normalImage, _boldImage, _italicImage;
		private BitmapFontGlyph[] _glyphList = new BitmapFontGlyph[256];

		private HighPreformanceTimer _shakeTimer = new HighPreformanceTimer();
		private int[] _shakeGrid = new int[1];
		private int _shakeInterval = 50;
		private bool _shakeEnabled = true;
		private bool _colorEnabled = true;
		private bool _imageEnabled = true;
		private bool _boldEnabled = true;
		private bool _underlineEnabled = true;
		private bool _italicEnabled = true;
		private bool _rainbowEnabled = true;
		private bool _shadowEnabled = true;
		private int _shadowHOffset = -1, _shadowVOffset = -1, _shadowColor = unchecked((int)0xFF000000);
		private bool _strikeThroughEnabled = true;	 

		private Image _imageTagImages = null;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the url this font was loaded from.
		/// </summary>
		public string URL
		{
			get { return _url; }
			set { _url = value; }
		}

        /// <summary>
        ///		Gets or sets the normal image used by this font.
        /// </summary>
        public Image NormalImage
        {
            get { return _normalImage; }
            set { _normalImage = value; }
        }

        /// <summary>
        ///		Gets or sets the bold image used by this font.
        /// </summary>
        public Image BoldImage
        {
            get { return _boldImage; }
            set { _boldImage = value; }
        }

        /// <summary>
        ///		Gets or sets the normal image used by this font.
        /// </summary>
        public Image ItalicImage
        {
            get { return _italicImage; }
            set { _italicImage = value; }
        }

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Returns a textural form of this class.
        /// </summary>
        /// <returns>Textural form of this class.</returns>
        public override string ToString()
        {
            return _url != null ? _url : "Unknown URL";
        }

		/// <summary>
		///		Returns the width in pixels of the given piece of text, if it was rendered
		///		by this font.
		/// </summary>
		/// <param name="text">Text to measure width of.</param>
		/// <param name="bfCodeEnabled">If set to true BFCode will be taken into account.</param>
		/// <returns>Width in pixels of the given text.</returns>
		public int TextWidth(string text, bool bfCodeEnabled)
		{
            if (text == null || text == "")
                return 0;

			int width = 0, widthCounter = 0;

			for (int i = 0; i < text.Length; i++)
			{
				char character = text[i];
                BitmapFontGlyph glyph = text[i] >= 32 ? _glyphList[text[i] - 32] : null;
				if (character == '[' && bfCodeEnabled == true)
				{
					int endIndex = text.IndexOf("]", i);
					if (endIndex <= 0) continue;

					string bfCode = text.Substring(i + 1, endIndex - i - 1);
					bool argumentsExist = bfCode.IndexOf("=") != -1;
					string command = argumentsExist == true ? bfCode.Split(new char[1] { '=' })[0] : bfCode;

					// Check what command it is and check if the size of the text will be increased by it.
					switch (command)
					{
						case "image":
							if (_imageEnabled == false || _imageTagImages == null) break;
							widthCounter += _imageTagImages.Width;
							break;
					}

					i += bfCode.Length + 1;
				}
				else if (glyph != null)
					widthCounter += glyph.Width;

				if (character == '\n')
				{
					if (widthCounter > width) width = widthCounter;
					widthCounter = 0;
				}
			}

			if (widthCounter > width) width = widthCounter;

			return width;
		}

		/// <summary>
		///		Returns the height in pixels of the given piece of text, if it was rendered
		///		by this font.
		/// </summary>
		/// <param name="text">Text to measure height of.</param>
		/// <param name="bfCodeEnabled">If set to true BFCode will be taken into account.</param>
		/// <returns>Height in pixels of the given text.</returns>
		public int TextHeight(string text, bool bfCodeEnabled)
		{
            if (text == null || text == "") 
                return 0;

			int height = _normalImage.Height;

			for (int i = 0; i < text.Length; i++)
			{
				char character = text[i];
				if (character == '[' && bfCodeEnabled == true)
				{
					int endIndex = text.IndexOf("]", i);
					if (endIndex <= 0) continue;
					i = endIndex;
					continue;
				}
                else if (character == '\n' && i < text.Length - 1)
					height += _normalImage.Height;
			}

			return height;
		}

        /// <summary>
        ///		Returns the position of a character within text.
        /// </summary>
        /// <param name="text">Text that character is in.</param>
        /// <param name="characterIndex">Index of character to check position of</param>
        /// <param name="bfCodeEnabled">If set to true BFCode will be taken into account.</param>
        /// <returns>Position of the given character.</returns>
        public System.Drawing.PointF TextCharacterPosition(string text, int characterIndex, bool bfCodeEnabled)
        {
            if (text == null || text == "")
                return new System.Drawing.PointF(0,0);

            float renderX = 0, renderY = 0;
            int characterTracker = 0;

            // If there are no tags then don't use bfcode.
            if (bfCodeEnabled == true && text.IndexOf('[') == -1) bfCodeEnabled = false;

            if (bfCodeEnabled == true)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    char character = text[i];
                    BitmapFontGlyph glyph = text[i] >= 32 ? _glyphList[text[i] - 32] : null;
                    bool parsed = false;

                    if (character == '[' && glyph != null)
                    {
                        int endIndex = text.IndexOf("]", i);
                        if (endIndex <= 0) continue;

                        string bfCode = text.Substring(i + 1, endIndex - i - 1);
                        bool argumentsExist = bfCode.IndexOf("=") != -1;
                        string command = bfCode;

                        parsed = true;
                        i += bfCode.Length + 1;
                    }
                    else if (character == '\n')
                    {
                        parsed = true;
                        renderX = 0;
                        renderY += _normalImage.Height * GraphicsManager.ScaleFactor[1];
                        characterTracker++;
                    }
                    else if (character == '\t')
                    {
                        parsed = true;
                        renderX += _glyphList[32].Width * 4;
                        characterTracker++;
                    }

                    if (glyph != null && parsed == false)
                    {
                        renderX += glyph.Width * GraphicsManager.ScaleFactor[0];
                        characterTracker++;
                    }

                    if (characterIndex == characterTracker) break;
                }
            }
            else
                for (int i = 0; i < text.Length; i++)
                {
                    char character = text[i];

                    if (text[i] >= 32 && _glyphList[text[i] - 32] != null)
                        renderX += _glyphList[text[i] - 32].Width * GraphicsManager.ScaleFactor[0];
                    else if (character == '\n')
                    {
                        renderX = 0;
                        renderY += _normalImage.Height * GraphicsManager.ScaleFactor[1];
                    }
                    else if (character == '\t')
                        renderX += _glyphList[32].Width * 4;

                    if (i == characterIndex) break;
                }

            return new System.Drawing.PointF(renderX, renderY);
        }

		/// <summary>
		///		Renders a string of text using this bitmap font at the specific position
		///		of the screen.
		///		If bfCode is set to true BFCode is enabled which allows you to write
		///		inline effect code into your text, the following code tags are valid.
		/// 
		///		[/?] = Closes the current BFCode tag. 
		///		[b] = Sets the following text as bold.
		///		[i] = Sets the following text as italic.
		///		[u] = Sets the following text as underlined.
		///		[s] = Strikes through the following text.
		///		[color=r,g,b] / [color=0x00000000] / [color=name] = Sets the following text's color as the given color.
		///		[image=id] = Renders an bitmap font image.
		///		[shake=intensity] = Makes the following text shake.
		///		[rainbow] = Creates a rainbow effect on the texts color.
		///		[shadow] = Draws a shadow behind the text.
		/// 
		/// </summary>
		/// <param name="text">String of text to render.</param>
		/// <param name="x">Position on the x-axis to render text.</param>
		/// <param name="y">Position on the y-axis to render text.</param>
		/// <param name="z">Position on the z-axis to render text.</param>
		/// <param name="bfCodeEnabled">If set to true BFCode will be enabled.</param>
		public void Render(string text, float x, float y, float z, bool bfCodeEnabled)
		{
			Image glyphImage = _normalImage;
			float renderX = x;
			float renderY = y;
			bool shaking = false;
			int shakeIntensity = 0;
			int rainbowColor = 0;
			bool inRainbow = false, inShadow = false, inUnderline = false, inItalic = false;
			bool inBold = false, inStrikeThrough = false;
			Image previousBoldImage = null, previousItalicImage = null;
            if (text == null || text == "")
                return;

            // Grab the current color.
            int originalRed, originalGreen, originalBlue, originalAlpha;
            ColorMethods.SplitColor(ColorFormats.A8R8G8B8, GraphicsManager.VertexColors[0], out originalRed, out originalGreen, out originalBlue, out originalAlpha);

			// If there are no tags then don't use bfcode.
			if (bfCodeEnabled == true && text.IndexOf('[') == -1) bfCodeEnabled = false;
			bool inStatePush = false;

			if (bfCodeEnabled == true)
			{
				// Update the shaking bbcode offset grid.
				if (_shakeTimer.DurationMillisecond > _shakeInterval)
				{
					Random random = new Random(Environment.TickCount);
					_shakeTimer.Restart();
					_shakeGrid = new int[128];
					for (int i = 0; i < 128; i++)
						_shakeGrid[i] = random.Next(100) - random.Next(100);
				}

				for (int i = 0; i < text.Length; i++)
				{
					char character = text[i];
                    BitmapFontGlyph glyph = text[i] >= 32 ? _glyphList[text[i] - 32] : null;
                    bool parsed = false;

                    if (character == '[' && glyph != null)
					{
						int endIndex = text.IndexOf("]", i);
						if (endIndex <= 0) continue;

						string bfCode = text.Substring(i + 1, endIndex - i - 1);
						bool argumentsExist = bfCode.IndexOf("=") != -1;
						string command = bfCode;
						string[] arguments = null;

						if (argumentsExist == true)
						{
							string[] equalSplit = bfCode.Split(new char[1] { '=' });
							if (equalSplit.Length == 2)
							{
								command = equalSplit[0];
								arguments = equalSplit[1].Split(new char[1] { ',' });
							}
							else
								argumentsExist = false;
                        }

                        #region BBCode Action
                        parsed = true;
                        switch (command)
						{
							case "/color":
								if (_colorEnabled == false) break;
								if (inStatePush == true)
								{
									GraphicsManager.PopRenderState();
									inStatePush = false;
								}
								break;

							case "color":
								if (_colorEnabled == false) break;
								if (argumentsExist == true && arguments.Length == 1)
								{
                                    // Work out the new color.
                                    GraphicsManager.PushRenderState();
									inStatePush = true;
									if (arguments[0].StartsWith("0x") == true)
										GraphicsManager.VertexColors.AllVertexs = Convert.ToInt32(arguments[0], 16);
									else
										switch (arguments[0].ToLower())
										{
											#region Built In Colors
											case "red":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFF0000);
												break;
											case "blue":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF0000FF);
												break;
											case "green":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF00FF00);
												break;
											case "aqua":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF00FFFF);
												break;
											case "orange":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFA500);
												break;
											case "grey":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF808080);
												break;
											case "black":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF000000);
												break;
											case "white":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFFFF);
												break;
											case "brown":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFA52A2A);
												break;
											case "cyan":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF00FFFF);
												break;
											case "fuchsia":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFF00FF);
												break;
											case "gold":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFD700);
												break;
											case "goldenrod":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFDAA520);
												break;
											case "indigo":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF4B0082);
												break;
											case "lavender":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFE6E6FA);
												break;
											case "lime":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF00FF00);
												break;
											case "maroon":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF800000);
												break;
											case "navy":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF000080);
												break;
											case "olive":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF808000);
												break;
											case "orchid":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFDA70D6);
												break;
											case "pink":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFC0CB);
												break;
											case "purple":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF800080);
												break;
											case "royalblue":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF4169E1);
												break;
											case "steelblue":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF4682B4);
												break;
											case "yellow":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFFFFF00);
												break;
											case "yellowgreen":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF9ACD32);
												break;
											case "darkred":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF8B0000);
												break;
											case "darkgreen":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF008B00);
												break;
											case "darkblue":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFF00008B);
												break;
											case "lightgrey":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFD3D3D3);
												break;
											case "darkgrey":
												GraphicsManager.VertexColors.AllVertexs = unchecked((int)0xFFA9A9A9);
												break;
											#endregion
										}

                                    // Associate the alpha value as well.
                                    int rn, gn, bn, an;
                                    ColorMethods.SplitColor(ColorFormats.A8R8G8B8, GraphicsManager.VertexColors[0], out rn, out gn, out bn, out an);
                                    GraphicsManager.VertexColors.AllVertexs = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, rn, gn, bn, originalAlpha);  
								}
								else if (argumentsExist == true && arguments.Length == 3)
								{
									GraphicsManager.PushRenderState();
									inStatePush = true;
									GraphicsManager.VertexColors.AllVertexs = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, int.Parse(arguments[0]), int.Parse(arguments[1]), int.Parse(arguments[2]), originalAlpha);
								}
								break;

							case "/shake":
								if (_shakeEnabled == false) break;
								shaking = false;
								break;

							case "shake":
								if (_shakeEnabled == false) break;
								shaking = true;
								if (argumentsExist == false)
								{
									shakeIntensity = 1;
									break;
								}
                                else
								    shakeIntensity = int.Parse(arguments[0]);
								break;

							case "image":
								int frame = 0;
								if (_imageEnabled == false || _imageTagImages == null || argumentsExist == false) break;
								if (int.TryParse(arguments[0], out frame) == false) break;
								if (frame >= _imageTagImages.FrameCount || frame < 0) break;
								GraphicsManager.RenderImage(_imageTagImages, renderX + (shaking == true ? _shakeGrid[i % _shakeGrid.Length] / (100.0f / shakeIntensity) : 0), renderY + (shaking == true ? _shakeGrid[(i * 2) % _shakeGrid.Length] / (100.0f / shakeIntensity) : 0), z, frame);
								renderX += _imageTagImages.Width * GraphicsManager.ScaleFactor[0];
								break;

							case "/b":
								if (_boldEnabled == false || inBold == false || _boldImage == null) break;
								inBold = false;
								glyphImage = previousBoldImage;
								break;

							case "b":
								if (_boldEnabled == false || inBold == true || _boldImage == null) break;
								inBold = true;
								previousBoldImage = glyphImage;
								glyphImage = _boldImage;
								break;

							case "/i":
								if (_italicEnabled == false || inItalic == false || _italicImage == null) break;
								inItalic = false;
								glyphImage = previousItalicImage;
								break;

							case "i":
								if (_italicEnabled == false || inItalic == true || _italicImage == null) break;
								inItalic = true;
								previousItalicImage = glyphImage;
								glyphImage = _italicImage;
								break;

							case "/u":
								if (_underlineEnabled == false) break;
								inUnderline = false;
								break;

							case "u":
								if (_underlineEnabled == false) break;
								inUnderline = true;
								break;

							case "/s":
								if (_strikeThroughEnabled == false) break;
								inStrikeThrough = false;
								break;

							case "s":
								if (_strikeThroughEnabled == false) break;
								inStrikeThrough = true;
								break;

							case "/rainbow":
								if (inRainbow == true && _rainbowEnabled == true)
								{
									if (inStatePush == true)
									{
										GraphicsManager.PopRenderState();
										inStatePush = false;
									}
									inRainbow = false;
								}
								break;

							case "rainbow":
								if (_rainbowEnabled == false) break;
								GraphicsManager.PushRenderState();
								inStatePush = true;
								inRainbow = true;
								rainbowColor = 0;
								break;

							case "shadow":
								if (_shadowEnabled == false) break;
								inShadow = true;
								break;

							case "/shadow":
								if (_shadowEnabled == false) break;
								inShadow = false;
								break;

                            default: // Keep it, its not bbcode.
                                i -= (bfCode.Length + 1);
                                parsed = false;
                                break;
                        }
                        #endregion
                        i += bfCode.Length + 1;
					}
                    else if (character == '\n')
                    {
                        parsed = true;
                        renderX = x;
                        renderY += glyphImage.Height * GraphicsManager.ScaleFactor[1];
                    }
                    else if (character == '\t')
                    {
                        parsed = true;
                        renderX += _glyphList[32].Width * 4;
                    }

                    if (glyph != null && parsed == false)
					{
						// If we are in a rainbow tag then change the color.
                        if (inRainbow == true)
                        {
                            int rc, gc, bc, ac;
                            int rn, gn, bn, an;
                            ColorMethods.SplitColor(ColorFormats.A8R8G8B8, GraphicsManager.VertexColors[0], out rc, out gc, out bc, out ac);
                            ColorMethods.SplitColor(ColorFormats.A8R8G8B8, _rainbowColorScale[rainbowColor++ % _rainbowColorScale.Length], out rn, out gn, out bn, out an);
                            GraphicsManager.VertexColors.AllVertexs = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, rn, gn, bn, ac);  
                        }

						// Render the correct glyph, but ignore the space characters. 
						if (character != ' ')
						{
							float glyphX = renderX + glyph.HorizontalOffset + (shaking == true ? _shakeGrid[i % _shakeGrid.Length] / (100.0f / shakeIntensity) : 0);
							float glyphY = renderY + glyph.VerticalOffset + (shaking == true ? _shakeGrid[(i * 2) % _shakeGrid.Length] / (100.0f / shakeIntensity) : 0);

							// If we are in a shadow tag then render the shadow first.
							if (inShadow == true)
							{
								int originalColor = GraphicsManager.VertexColors.AllVertexs;
								GraphicsManager.VertexColors.AllVertexs = _shadowColor;

								// Render the glyphs shadow.
								GraphicsManager.RenderImage(glyphImage, glyphX + _shadowHOffset, glyphY + _shadowVOffset, z, text[i] - 32 - 1);

								// Render the underline and-or strikethrough shadow.
								if (inUnderline == true) GraphicsManager.RenderLine(glyphX - 1 + _shadowHOffset, glyphY + glyphImage.Height + _shadowVOffset, z, glyphX + glyph.Width - 1 + _shadowHOffset, glyphY + glyphImage.Height + _shadowVOffset, z);
								if (inStrikeThrough == true) GraphicsManager.RenderLine(glyphX - 1 + _shadowHOffset, glyphY + (glyphImage.Height / 2) + _shadowVOffset, z, glyphX + glyph.Width - 1 + _shadowHOffset, glyphY + (glyphImage.Height / 2) + _shadowVOffset, z);

								GraphicsManager.VertexColors.AllVertexs = originalColor;
							}

							// Check if we need to underline and-or strikethrough this glyph
							if (inUnderline == true) GraphicsManager.RenderLine(glyphX - 1, glyphY + glyphImage.Height, z, glyphX + glyph.Width - 1, glyphY + glyphImage.Height, z);
							if (inStrikeThrough == true) GraphicsManager.RenderLine(glyphX - 1, glyphY + (glyphImage.Height / 2), z, glyphX + glyph.Width - 1, glyphY + (glyphImage.Height / 2), z);

							GraphicsManager.RenderImage(glyphImage, glyphX, glyphY, z, text[i] - 32 - 1);
						}

						// Increase the spacing correctly.
						renderX += glyph.Width * GraphicsManager.ScaleFactor[0];
					}
				}
			}
			else
				for (int i = 0; i < text.Length; i++)
				{
					char character = text[i];

                    if (text[i] >= 32 && _glyphList[text[i] - 32] != null)
                    {
						if (character != ' ')
                            GraphicsManager.RenderImage(glyphImage, renderX + _glyphList[text[i] - 32].HorizontalOffset, renderY + _glyphList[text[i] - 32].VerticalOffset, z, text[i] - 32 - 1);

                        renderX += _glyphList[text[i] - 32].Width * GraphicsManager.ScaleFactor[0];
                    }
                    else if (character == '\n')
                    {
                        renderX = x;
                        renderY += glyphImage.Height * GraphicsManager.ScaleFactor[1];
                    }
                    else if (character == '\t')
                        renderX += _glyphList[32].Width * 4;
				}

			if (inStatePush == true) GraphicsManager.PopRenderState();
		}
		public void Render(string text, float x, float y, float z)
		{
			Render(text, x, y, z, false);
		}

		/// <summary>
		///		Loads this bitmap fonts data from the given xml font 
		///		declaration file.
		/// </summary>
		/// <param name="url">Url of xml font declaration file to load from.</param>
		public void Load(object url)
		{
			if (url is string) _url = (string)url;

			Stream stream = StreamFactory.RequestStream(url, StreamMode.Open);
			if (stream == null) return;
			XmlDocument document = new XmlDocument();
			document.Load(stream);
			stream.Close();

			string imagePath = "";
			int cellWidth = 16, cellHeight = 16;
			int hSpacing = 0, vSpacing = 0;
			int maskColor = unchecked((int)0xFFFF00FF);

			// Check there is actually a font declaration in this file.
			if (document["font"] == null) throw new Exception("Font declaration file missing font element.");

			#region Glyph image parsing

			// Load in the normal image if it exists.
			XmlNode imageNode = document["font"]["images"]["normal"];
			if (imageNode != null)
			{
				foreach (XmlAttribute attribute in imageNode.Attributes)
				{
					if (attribute.Name.ToLower() == "path")
						imagePath = attribute.Value;
					else if (attribute.Name.ToLower() == "cellwidth")
						cellWidth = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "cellheight")
						cellHeight = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "hspacing")
						hSpacing = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "vspacing")
						vSpacing = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "maskcolor")
						maskColor = Convert.ToInt32(attribute.Value, 16);	
				}

				// Load the fonts image's.
				GraphicsManager.ColorKey = maskColor;
				_normalImage = GraphicsManager.LoadImage(imagePath, cellWidth, cellHeight, hSpacing, vSpacing, 0);
			}
			else
				throw new Exception("Font declaration file is missing a normal image element.");

			// Load in the bold image if it exists.
			imageNode = document["font"]["images"]["bold"];
			if (imageNode != null)
			{
				foreach (XmlAttribute attribute in imageNode.Attributes)
				{
					if (attribute.Name.ToLower() == "path")
						imagePath = attribute.Value;
					else if (attribute.Name.ToLower() == "cellwidth")
						cellWidth = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "cellheight")
						cellHeight = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "hspacing")
						hSpacing = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "vspacing")
						vSpacing = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "maskcolor")
						maskColor = Convert.ToInt32(attribute.Value, 16);	
				}

				// Load the fonts image's.
				GraphicsManager.ColorKey = maskColor;
				_boldImage = GraphicsManager.LoadImage(imagePath, cellWidth, cellHeight, hSpacing, vSpacing, 0);
			}

			// Load in the italic image if it exists.
			imageNode = document["font"]["images"]["italic"];
			if (imageNode != null)
			{
				foreach (XmlAttribute attribute in imageNode.Attributes)
				{
					if (attribute.Name.ToLower() == "path")
						imagePath = attribute.Value;
					else if (attribute.Name.ToLower() == "cellwidth")
						cellWidth = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "cellheight")
						cellHeight = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "hspacing")
						hSpacing = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "vspacing")
						vSpacing = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "maskcolor")
						maskColor = Convert.ToInt32(attribute.Value, 16);	
				}

				// Load the fonts image's.
				GraphicsManager.ColorKey = maskColor;
				_italicImage = GraphicsManager.LoadImage(imagePath, cellWidth, cellHeight, hSpacing, vSpacing, 0);
			}

			#endregion

			// See if there are any bfcode settings in this font.
			if (document["font"]["bfcode"] != null)
			{
				#region BFCode setting parsing

				// See if it gives any settings for the shake tag.
				XmlNode tagNode = document["font"]["bfcode"]["shake"];
				if (tagNode != null)
					foreach (XmlAttribute attribute in tagNode.Attributes)
						if (attribute.Name.ToLower() == "interval")
							_shakeInterval = int.Parse(attribute.Value);
						else if (attribute.Name.ToLower() == "enabled")
							_shakeEnabled = attribute.Value == "0" ? false : true;

				// See if it gives any settings for the color tag.
				tagNode = document["font"]["bfcode"]["color"];
				if (tagNode != null)
					foreach (XmlAttribute attribute in tagNode.Attributes)
						if (attribute.Name.ToLower() == "enabled")
							_colorEnabled = attribute.Value == "0" ? false : true;

				// See if it gives any settings for the color tag.
				tagNode = document["font"]["bfcode"]["bold"];
				if (tagNode != null)
					foreach (XmlAttribute attribute in tagNode.Attributes)
						if (attribute.Name.ToLower() == "enabled")
							_boldEnabled = attribute.Value == "0" ? false : true;

				// See if it gives any settings for the underline tag.
				tagNode = document["font"]["bfcode"]["underline"];
				if (tagNode != null)
					foreach (XmlAttribute attribute in tagNode.Attributes)
						if (attribute.Name.ToLower() == "enabled")
							_underlineEnabled = attribute.Value == "0" ? false : true;

				// See if it gives any settings for the italic tag.
				tagNode = document["font"]["bfcode"]["italic"];
				if (tagNode != null)
					foreach (XmlAttribute attribute in tagNode.Attributes)
						if (attribute.Name.ToLower() == "enabled")
							_italicEnabled = attribute.Value == "0" ? false : true;

				// See if it gives any settings for the strike through tag.
				tagNode = document["font"]["bfcode"]["strikethrough"];
				if (tagNode != null)
					foreach (XmlAttribute attribute in tagNode.Attributes)
						if (attribute.Name.ToLower() == "enabled")
							_strikeThroughEnabled = attribute.Value == "0" ? false : true;

				// See if it gives any settings for the rainbow tag.
				tagNode = document["font"]["bfcode"]["rainbow"];
				if (tagNode != null)
					foreach (XmlAttribute attribute in tagNode.Attributes)
						if (attribute.Name.ToLower() == "enabled")
							_rainbowEnabled = attribute.Value == "0" ? false : true;
						else if (attribute.Name.ToLower() == "colorscale")
						{
							string[] colors = attribute.Value.Split(new char[] { ',' });
							_rainbowColorScale = new int[colors.Length];
							for (int i = 0; i < colors.Length; i++)
								_rainbowColorScale[i] = Convert.ToInt32(colors[i], 16);
						}

				// See if it gives any settings for the shadow tag.
				tagNode = document["font"]["bfcode"]["shadow"];
				if (tagNode != null)
					foreach (XmlAttribute attribute in tagNode.Attributes)
						if (attribute.Name.ToLower() == "enabled")
							_shadowEnabled = attribute.Value == "0" ? false : true;
						else if (attribute.Name.ToLower() == "hoffset")
							_shadowHOffset = int.Parse(attribute.Value);
						else if (attribute.Name.ToLower() == "voffset")
							_shadowVOffset = int.Parse(attribute.Value);
						else if (attribute.Name.ToLower() == "color")
							_shadowColor = Convert.ToInt32(attribute.Value,16);

				// See if it gives any settings for the image tag.
				tagNode = document["font"]["bfcode"]["image"];
				if (tagNode != null)
				{
					foreach (XmlAttribute attribute in tagNode.Attributes)
						if (attribute.Name.ToLower() == "enabled")
							_imageEnabled = attribute.Value == "0" ? false : true;
						else if (attribute.Name.ToLower() == "path")
							imagePath = attribute.Value;
						else if (attribute.Name.ToLower() == "cellwidth")
							cellWidth = int.Parse(attribute.Value);
						else if (attribute.Name.ToLower() == "cellheight")
							cellHeight = int.Parse(attribute.Value);
						else if (attribute.Name.ToLower() == "hspacing")
							hSpacing = int.Parse(attribute.Value);
						else if (attribute.Name.ToLower() == "vspacing")
							vSpacing = int.Parse(attribute.Value);
						else if (attribute.Name.ToLower() == "maskcolor")
							maskColor = Convert.ToInt32(attribute.Value, 16);

					GraphicsManager.ColorKey = maskColor;
					_imageTagImages = GraphicsManager.LoadImage(imagePath, cellWidth, cellHeight, hSpacing, vSpacing, 0);
				}

				#endregion
			}

			#region Glyph parsing

			// If everything went successfully then load the font's image.
			XmlNode glyphsNode = document["font"]["glyphs"];
			if (glyphsNode == null) throw new Exception("Font declaration file is missing glyph declaration.");

			foreach(XmlNode glyphNode in glyphsNode.ChildNodes)
			{
				if (glyphNode.Name.ToLower() != "glyph") continue;
				
				// Find the settings of this glyph.
				char character = '!';
				int voffset = 0, hoffset = 0, width = 0;
				foreach(XmlNode attribute in glyphNode.Attributes)
				{
					if (attribute.Name.ToLower() == "character")
						character = attribute.Value[0];
					else if (attribute.Name.ToLower() == "hoffset")
						hoffset = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "voffset")
						voffset = int.Parse(attribute.Value);
					else if (attribute.Name.ToLower() == "width")
						width = int.Parse(attribute.Value);
				}

				// Create the glyph and add it to the list.
				_glyphList[character - 32] = new BitmapFontGlyph(character, width, hoffset, voffset);
			}

			#endregion
		}

		/// <summary>
		///		Initializes this class and loads the font from a given xml 
		///		bitmap font declaration file.
		/// </summary>
		/// <param name="url">Url of xml font declaration file to load from.</param>
		public BitmapFont(object url)
		{
			Load(url);
		}

		#endregion
	}

	/// <summary>
	///		This class is used to store details on a single bitmap font glyph. 
	///		A glyph is the name given to a single character in a font file with 
	///		properties such as offset, width and height.
	/// </summary>
	public sealed class BitmapFontGlyph
	{
		#region Members
		#region Variables

		private char _character;
		private int _width, _vOffset, _hOffset;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the character this glyph represents.
		/// </summary>
		public char Character
		{
			get { return _character; }
			set { _character = value; }
		}
		
		/// <summary>
		///		Gets or sets the width of this glyph.
		/// </summary>
		public int Width
		{
			get { return _width; }
			set { _width = value; }
		}

		/// <summary>
		///		Gets or sets the horizontal offset that should be applied to this 
		///		glpyh while rendering it.
		/// </summary>
		public int HorizontalOffset
		{
			get { return _hOffset; }
			set { _hOffset = value; }
		}

		/// <summary>
		///		Gets or sets the vertical offset that should be applied to this 
		///		glpyh while rendering it.
		/// </summary>
		public int VerticalOffset
		{
			get { return _vOffset; }
			set { _vOffset = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes this class with the given chracter, size and offset.
		/// </summary>
		/// <param name="character">Character this glyph should represent.</param>
		/// <param name="width">Width in pixels of this glyph.</param>
		/// <param name="hOffset">Horizontal offset in pixels this glyph should be rendered at.</param>
		/// <param name="vOffset">Vertical offset in pixels this glyph should be rendered at.</param>
		public BitmapFontGlyph(char character, int width, int hOffset, int vOffset)
		{
			_character = character;
			_width = width;
			_hOffset = hOffset;
			_vOffset = vOffset;
		}

		#endregion
	}

}