/* 
 * File: Graphics.cs
 *
 * This source file contains the declaration of the GraphicsFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Engine.ScriptingFunctions
{

    /// <summary>
    ///     Wraps an image object used in a script.
    /// </summary>
    public class ImageScriptObject : NativeObject
    {
        public ImageScriptObject(Image image)
        {
            _nativeObject = image;
        }

        public override string ToString()
        {
            return _nativeObject != null ? ((Image)_nativeObject).URL.ToString() : base.ToString();
        }

        public override void Deallocate()
        {
            base.Deallocate();
        }

        public ImageScriptObject() { }
    }

    /// <summary>
    ///     Wraps a font object used in a script.
    /// </summary>
    public class FontScriptObject : NativeObject
    {
        public FontScriptObject(BitmapFont font)
        {
            _nativeObject = font;
        }

        public FontScriptObject() { }
    }

    /// <summary>
    ///     Wraps a shader object used in a script.
    /// </summary>
    public class ShaderScriptObject : NativeObject
    {
        public ShaderScriptObject(Shader shader)
        {
            _nativeObject = shader;
        }

        public ShaderScriptObject() { }
    }

    /// <summary>
    ///     Wraps a render target object used in a script.
    /// </summary>
    public class RenderTargetScriptObject : NativeObject
    {
        public RenderTargetScriptObject(IRenderTarget target)
        {
            _nativeObject = target;
        }

        public RenderTargetScriptObject() { }
    }

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class GraphicsFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("SetBitmapFont", "void", "object")]
		public void SetBitmapFont(ScriptThread thread)
		{
            if (thread.GetObjectParameter(0) == null)
            {
                GraphicsManager.BitmapFont = null;
                return;
            }

			BitmapFont font = ((NativeObject)thread.GetObjectParameter(0)).Object as BitmapFont;
			if (font == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetBitmapFont with an invalid object.", LogAlertLevel.Error);
				return;
			}
			GraphicsManager.BitmapFont = font;
		}

		[NativeFunctionInfo("BitmapFont", "object", "")]
		public void BitmapFont(ScriptThread thread)
		{
            thread.SetReturnValue(new FontScriptObject(GraphicsManager.BitmapFont));
		}

		[NativeFunctionInfo("LoadBitmapFont", "object", "string")]
		public void LoadBitmapFont(ScriptThread thread)
		{
			thread.SetReturnValue(new FontScriptObject(new BitmapFont(thread.GetStringParameter(0))));
		}

        [NativeFunctionInfo("LoadShader", "object", "string")]
        public void LoadShader(ScriptThread thread)
        {
            thread.SetReturnValue(new ShaderScriptObject(new Shader(thread.GetStringParameter(0))));
        }

        [NativeFunctionInfo("SetShader", "void", "object")]
        public void SetShader(ScriptThread thread)
        {
            Shader shader = ((NativeObject)thread.GetObjectParameter(0)).Object as Shader;
            if (shader == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetShader with an invalid object.", LogAlertLevel.Error);
                return;
            }
            GraphicsManager.Shader = shader;
        }

        [NativeFunctionInfo("Shader", "object", "")]
        public void Shader(ScriptThread thread)
        {
            thread.SetReturnValue(new ShaderScriptObject(GraphicsManager.Shader));
        }

		[NativeFunctionInfo("SetBlendMode", "void", "int")]
		public void SetBlendMode(ScriptThread thread)
		{
			GraphicsManager.BlendMode = (BlendMode)thread.GetIntegerParameter(0);
		}

		[NativeFunctionInfo("BlendMode", "int", "")]
		public void BlendMode(ScriptThread thread)
		{
			thread.SetReturnValue((int)GraphicsManager.BlendMode);
		}

		[NativeFunctionInfo("SetClearColor", "void", "int")]
		public void SetClearColor(ScriptThread thread)
		{
			GraphicsManager.ClearColor = thread.GetIntegerParameter(0);		
		}

		[NativeFunctionInfo("ClearColor", "int", "")]
		public void ClearColor(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.ClearColor);
		}

        [NativeFunctionInfo("SetDepthBufferEnabled", "void", "bool")]
        public void SetDepthBufferEnabled(ScriptThread thread)
        {
            GraphicsManager.DepthBufferEnabled = thread.GetBooleanParameter(0);
        }

        [NativeFunctionInfo("DepthBufferEnabled", "int", "")]
        public void DepthBufferEnabled(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.DepthBufferEnabled);
        }

        [NativeFunctionInfo("SetVertexColor", "void", "int,int")]
        public void SetVertexColor(ScriptThread thread)
        {
            GraphicsManager.VertexColors[thread.GetIntegerParameter(0)] = thread.GetIntegerParameter(1);
        }

        [NativeFunctionInfo("VertexColor", "int", "int")]
        public void VertexColor(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.VertexColors[thread.GetIntegerParameter(0)]);
        }

		[NativeFunctionInfo("SetColorKey", "void", "int")]
		public void SetColorKey(ScriptThread thread)
		{
			GraphicsManager.ColorKey = thread.GetIntegerParameter(0);
		}

		[NativeFunctionInfo("ColorKey", "int", "")]
		public void ColorKey(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.ColorKey);
		}

		[NativeFunctionInfo("SetRenderTarget", "void", "object")]
		public void SetRenderTarget(ScriptThread thread)
		{
			IRenderTarget renderTarget = ((NativeObject)thread.GetObjectParameter(0)).Object as IRenderTarget;
			if (renderTarget == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetRenderTarget with an invalid object.", LogAlertLevel.Error);
				return;
			}
			GraphicsManager.RenderTarget = renderTarget;
		}

		[NativeFunctionInfo("RenderTarget", "object", "")]
		public void RenderTarget(ScriptThread thread)
		{
			thread.SetReturnValue(new RenderTargetScriptObject(GraphicsManager.RenderTarget));
		}

		[NativeFunctionInfo("CreateImage", "void", "int,int,int")]
		public void CreateImageA(ScriptThread thread)
		{
            thread.SetReturnValue(new ImageScriptObject(GraphicsManager.CreateImage(thread.GetIntegerParameter(0), thread.GetIntegerParameter(1), (ImageFlags)thread.GetIntegerParameter(2))));
		}

		[NativeFunctionInfo("CreateImage", "void", "int,int,int,int")]
		public void CreateImageB(ScriptThread thread)
		{
            thread.SetReturnValue(new ImageScriptObject(GraphicsManager.CreateImage(thread.GetIntegerParameter(0), thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), (ImageFlags)thread.GetIntegerParameter(3))));
		}

		[NativeFunctionInfo("FramesPerSecond", "int", "")]
		public void FramesPerSecond(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.FramesPerSecond);
		}

		[NativeFunctionInfo("FramesPerSecondLimit", "int", "")]
		public void FramesPerSecondLimit(ScriptThread thread)
		{
			thread.SetReturnValue(Engine.GlobalInstance.FPSLimit);
		}

		[NativeFunctionInfo("FramesPerSecondTarget", "int", "")]
		public void FramesPerSecondTarget(ScriptThread thread)
		{
			thread.SetReturnValue(Engine.GlobalInstance.FPSTarget);
		}

		[NativeFunctionInfo("LoadImage", "object", "string,int")]
		public void LoadImageA(ScriptThread thread)
		{
            thread.SetReturnValue(new ImageScriptObject(GraphicsManager.LoadImage(thread.GetStringParameter(0), (ImageFlags)thread.GetIntegerParameter(1)))); 
		}

		[NativeFunctionInfo("LoadImage", "object", "string,int,int,int")]
		public void LoadImageB(ScriptThread thread)
		{
            thread.SetReturnValue(new ImageScriptObject(GraphicsManager.LoadImage(thread.GetStringParameter(0), thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), (ImageFlags)thread.GetIntegerParameter(3)))); 
		}

		[NativeFunctionInfo("LoadImage", "object", "string,int,int,int,int,int")]
		public void LoadImageC(ScriptThread thread)
		{
            thread.SetReturnValue(new ImageScriptObject(GraphicsManager.LoadImage(thread.GetStringParameter(0), thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), thread.GetIntegerParameter(3), thread.GetIntegerParameter(4), (ImageFlags)thread.GetIntegerParameter(5)))); 		
		}

		[NativeFunctionInfo("ImageWidth", "int", "object")]
		public void ImageWidth(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ImageWidth with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(image.FullWidth);
		}

		[NativeFunctionInfo("ImageHeight", "int", "object")]
		public void ImageHeight(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ImageHeight with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(image.FullHeight);
		}

		[NativeFunctionInfo("ImageFrameWidth", "int", "object")]
		public void ImageFrameWidth(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ImageFrameWidth with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(image.Width);
		}

		[NativeFunctionInfo("ImageFrameHeight", "int", "object")]
		public void ImageFrameHeight(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ImageFrameHeight with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(image.Height);
		}

		[NativeFunctionInfo("ImageHorizontalSeperation", "int", "object")]
		public void ImageHorizontalSeperation(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ImageHorizontalSeperation with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(image.HorizontalSpacing);
		}

		[NativeFunctionInfo("ImageVerticalSeperation", "int", "object")]
		public void ImageVerticalSeperation(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ImageVerticalSeperation with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(image.VerticalSpacing);
		}

		[NativeFunctionInfo("ImageFrameCount", "int", "object")]
		public void ImageFrameCount(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ImageFrameCount with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(image.FrameCount);
		}

        [NativeFunctionInfo("SetImageOrigin", "void", "object,int,int")]
        public void SetImageOrigin(ScriptThread thread)
        {
            Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
            if (image == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetImageOrigin with an invalid object.", LogAlertLevel.Error);
                return;
            }
            image.Origin = new System.Drawing.Point(thread.GetIntegerParameter(1), thread.GetIntegerParameter(2));
        }

        [NativeFunctionInfo("ImageOriginX", "int", "object")]
        public void ImageOriginX(ScriptThread thread)
        {
            Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
            if (image == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ImageOriginX with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(image.Origin.X);
        }

        [NativeFunctionInfo("ImageOriginY", "int", "object")]
        public void ImageOriginY(ScriptThread thread)
        {
            Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
            if (image == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ImageOriginY with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(image.Origin.Y);
        }

		[NativeFunctionInfo("SetOffset", "void", "float,float,float")]
		public void SetOffset(ScriptThread thread)
		{
			GraphicsManager.Offset = new float[3] { thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2) };
		}

		[NativeFunctionInfo("OffsetX", "float", "")]
		public void OffsetX(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.Offset[0]);
		}

		[NativeFunctionInfo("OffsetY", "float", "")]
		public void OffsetY(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.Offset[1]);
		}

		[NativeFunctionInfo("OffsetZ", "float", "")]
		public void OffsetZ(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.Offset[2]);
		}

        [NativeFunctionInfo("BeginScene", "void", "")]
        public void BeginScene(ScriptThread thread)
        {
            GraphicsManager.BeginScene();
        }

        [NativeFunctionInfo("FinishScene", "void", "")]
        public void FinishScene(ScriptThread thread)
        {
            GraphicsManager.FinishScene();
        }

        [NativeFunctionInfo("PresentScene", "void", "")]
        public void PresentScene(ScriptThread thread)
        {
            GraphicsManager.PresentScene();
        }

        [NativeFunctionInfo("ClearDepthBuffer", "void", "")]
        public void ClearDepthBuffer(ScriptThread thread)
        {
            GraphicsManager.ClearDepthBuffer();
        }

		[NativeFunctionInfo("ClearScene", "void", "")]
		public void ClearSceneA(ScriptThread thread)
		{
			GraphicsManager.ClearScene();
		}

		[NativeFunctionInfo("ClearScene", "void", "int")]
		public void ClearSceneB(ScriptThread thread)
		{
			int clearColor = GraphicsManager.ClearColor;
			GraphicsManager.ClearColor = thread.GetIntegerParameter(0);
			GraphicsManager.ClearScene();
			GraphicsManager.ClearColor = clearColor;
		}

		[NativeFunctionInfo("ClearRenderState", "void", "")]
		public void ClearRenderState(ScriptThread thread)
		{
			GraphicsManager.ClearRenderState();
		}

		[NativeFunctionInfo("PushRenderState", "void", "")]
		public void PushRenderState(ScriptThread thread)
		{
			GraphicsManager.PushRenderState();
		}

		[NativeFunctionInfo("PopRenderState", "void", "")]
		public void PopRenderState(ScriptThread thread)
		{
			GraphicsManager.PopRenderState();
		}

		[NativeFunctionInfo("RenderImage", "void", "object,float,float,float")]
		public void RenderImageA(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called RenderImage with an invalid object.", LogAlertLevel.Error);
				return;
			}
			GraphicsManager.RenderImage(image, thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3));
		}

		[NativeFunctionInfo("RenderImage", "void", "object,float,float,float,int")]
		public void RenderImageB(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called RenderImage with an invalid object.", LogAlertLevel.Error);
				return;
			}
			GraphicsManager.RenderImage(image, thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3), thread.GetIntegerParameter(4));
		}

		[NativeFunctionInfo("RenderLine", "void", "float,float,float,float,float,float")]
		public void RenderLine(ScriptThread thread)
		{
			GraphicsManager.RenderLine(thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3), thread.GetFloatParameter(4), thread.GetFloatParameter(5));
		}

		[NativeFunctionInfo("RenderOval", "void", "float,float,float,float,float")]
		public void RenderOvalA(ScriptThread thread)
		{
			GraphicsManager.RenderOval(thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3), thread.GetFloatParameter(4));
		}

		[NativeFunctionInfo("RenderOval", "void", "float,float,float,float,float,bool")]
		public void RenderOvalB(ScriptThread thread)
		{
			GraphicsManager.RenderOval(thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3), thread.GetFloatParameter(4), thread.GetBooleanParameter(5));
		}

		[NativeFunctionInfo("RenderRectangle", "void", "float,float,float,float,float")]
		public void RenderRectangleA(ScriptThread thread)
		{
			GraphicsManager.RenderRectangle(thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3), thread.GetFloatParameter(4));
		}

		[NativeFunctionInfo("RenderRectangle", "void", "float,float,float,float,float,bool")]
		public void RenderRectangleB(ScriptThread thread)
		{
			GraphicsManager.RenderRectangle(thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3), thread.GetFloatParameter(4), thread.GetBooleanParameter(5));
		}

		[NativeFunctionInfo("RenderDashedRectangle", "void", "float,float,float,float,float")]
		public void RenderDashedRectangleA(ScriptThread thread)
		{
			GraphicsManager.RenderDashedRectangle(thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3), thread.GetFloatParameter(4));	
		}

		[NativeFunctionInfo("RenderDashedRectangle", "void", "float,float,float,float,float,int")]
		public void RenderDashedRectangleB(ScriptThread thread)
		{
			GraphicsManager.RenderDashedRectangle(thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3), thread.GetFloatParameter(4), thread.GetIntegerParameter(5));
		}

		[NativeFunctionInfo("RenderPixel", "void", "float,float,float")]
		public void RenderPixel(ScriptThread thread)
		{
			GraphicsManager.RenderPixel(thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2));
		}

		[NativeFunctionInfo("RenderPolygon", "void", "float[]")]
		public void RenderPolygonA(ScriptThread thread)
		{
			int arrayIndex = thread.GetArrayParameter(0);
			if (arrayIndex == 0)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called RenderPolygon with an invalid object.", LogAlertLevel.Error);
				return;
			}
			int arrayLength = thread.GetArrayLength(arrayIndex);
			Vertex[] vertexs = new Vertex[(arrayLength / 3)];
			for (int i = 0; i < (arrayLength / 3); i++)
				vertexs[i] = new Vertex(thread.GetFloatArrayElement(arrayIndex, (i * 3)), 
										thread.GetFloatArrayElement(arrayIndex, (i * 3) + 1), 
										thread.GetFloatArrayElement(arrayIndex, (i * 3) + 2));

			GraphicsManager.RenderPolygon(vertexs);
		}

		[NativeFunctionInfo("RenderPolygon", "void", "float[],bool")]
		public void RenderPolygonB(ScriptThread thread)
		{
			int arrayIndex = thread.GetArrayParameter(0);
			if (arrayIndex == 0)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called RenderPolygon with an invalid object.", LogAlertLevel.Error);
				return;
			}
			int arrayLength = thread.GetArrayLength(arrayIndex);
			Vertex[] vertexs = new Vertex[(arrayLength / 3)];
			for (int i = 0; i < (arrayLength / 3); i++)
				vertexs[i] = new Vertex(thread.GetFloatArrayElement(arrayIndex, (i * 3)),
										thread.GetFloatArrayElement(arrayIndex, (i * 3) + 1),
										thread.GetFloatArrayElement(arrayIndex, (i * 3) + 2));

			GraphicsManager.RenderPolygon(vertexs, thread.GetBooleanParameter(1));
		}

        [NativeFunctionInfo("BFCodeSubString", "string", "string,int,int")]
        public void BFCodeSubString(ScriptThread thread)
        {
            string bfCode = thread.GetStringParameter(0);
            int start = thread.GetIntegerParameter(1);
            int length = thread.GetIntegerParameter(2);
            string finished = "";
            int currentLength = 0;
            if (bfCode == "" || bfCode == null)
                return;

            for (int currentPosition = 0; currentPosition < bfCode.Length; currentPosition++)
            {
                char currentCharacter = bfCode[currentPosition];
                if (currentCharacter == '[')
                {
                    // Skip to closing tag.
                    int newPosition = bfCode.IndexOf(']', currentPosition);
                    if (newPosition == -1)
                        continue;

                    finished += bfCode.Substring(currentPosition, (newPosition - currentPosition) + 1);
                    currentPosition = newPosition;
                }
                else if (currentPosition >= start)
                {
                    finished += currentCharacter;
                    currentLength++;
                }
                if (currentLength >= length)
                    break;
            }
            thread.SetReturnValue(finished);
        }

        [NativeFunctionInfo("BFCodeLength", "int", "string")]
        public void BFCodeLength(ScriptThread thread)
        {
            string bfCode = thread.GetStringParameter(0);
            int currentLength = 0;
            if (bfCode == "" || bfCode == null)
                return;

            for (int currentPosition = 0; currentPosition < bfCode.Length; currentPosition++)
            {
                char currentCharacter = bfCode[currentPosition];
                if (currentCharacter == '[')
                {
                    // Skip to closing tag.
                    currentPosition = bfCode.IndexOf(']', currentPosition);
                    if (currentPosition == -1)
                        continue;
                }
                else 
                    currentLength++;
            }

            thread.SetReturnValue(currentLength);
        }

		[NativeFunctionInfo("RenderBitmapText", "void", "string,float,float,float")]
		public void RenderBitmapTextA(ScriptThread thread)
		{
			GraphicsManager.RenderText(thread.GetStringParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3));
		}

		[NativeFunctionInfo("RenderBitmapText", "void", "string,float,float,float,bool")]
		public void RenderBitmapTextB(ScriptThread thread)
		{
			GraphicsManager.RenderText(thread.GetStringParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3), thread.GetBooleanParameter(4));
		}

		[NativeFunctionInfo("SetRotationAngle", "void", "float,float,float")]
		public void SetRotationAngle(ScriptThread thread)
		{
			GraphicsManager.RotationAngle[0] = thread.GetFloatParameter(0);
            GraphicsManager.RotationAngle[1] = thread.GetFloatParameter(1);
            GraphicsManager.RotationAngle[2] = thread.GetFloatParameter(2);
		}

		[NativeFunctionInfo("RotationAngleX", "float", "")]
		public void RotationAngleX(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.RotationAngle[0]);
		}

        [NativeFunctionInfo("RotationAngleY", "float", "")]
        public void RotationAngleY(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.RotationAngle[1]);
        }

        [NativeFunctionInfo("RotationAngleZ", "float", "")]
        public void RotationAngleZ(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.RotationAngle[2]);
        }

		[NativeFunctionInfo("SetScaleFactor", "void", "float,float,float")]
		public void SetScaleFactor(ScriptThread thread)
		{
            GraphicsManager.ScaleFactor = new float[3] { thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2) };
		}

		[NativeFunctionInfo("ScaleFactorX", "float", "")]
		public void ScaleFactorX(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.ScaleFactor[0]);
		}

		[NativeFunctionInfo("ScaleFactorY", "float", "")]
		public void ScaleFactorY(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.ScaleFactor[1]);
		}

        [NativeFunctionInfo("ScaleFactorZ", "float", "")]
        public void ScaleFactorZ(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.ScaleFactor[2]);
        }

        [NativeFunctionInfo("TextCharacterX", "float", "string,int")]
        public void TextCharacterXA(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.TextCharacterPosition(thread.GetStringParameter(0), thread.GetIntegerParameter(1), false).X);
        }
        [NativeFunctionInfo("TextCharacterX", "float", "string,int,bool")]
        public void TextCharacterXB(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.TextCharacterPosition(thread.GetStringParameter(0), thread.GetIntegerParameter(1), thread.GetBooleanParameter(2)).X);
        }

        [NativeFunctionInfo("TextCharacterY", "float", "string,int")]
        public void TextCharacterYA(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.TextCharacterPosition(thread.GetStringParameter(0), thread.GetIntegerParameter(1), false).Y);
        }

        [NativeFunctionInfo("TextCharacterY", "float", "string,int,bool")]
        public void TextCharacterYB(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.TextCharacterPosition(thread.GetStringParameter(0), thread.GetIntegerParameter(1), thread.GetBooleanParameter(2)).Y);
        }

		[NativeFunctionInfo("TextWidth", "int", "string")]
		public void TextWidthA(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.TextWidth(thread.GetStringParameter(0)));
		}

        [NativeFunctionInfo("TextWidth", "int", "string,bool")]
		public void TextWidthB(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.TextWidth(thread.GetStringParameter(0), thread.GetBooleanParameter(1)));
		}

        [NativeFunctionInfo("TextHeight", "int", "string")]
		public void TextHeightA(ScriptThread thread)
		{
            thread.SetReturnValue(GraphicsManager.TextHeight(thread.GetStringParameter(0)));
		}

        [NativeFunctionInfo("TextHeight", "int", "string,bool")]
		public void TextHeightB(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.TextHeight(thread.GetStringParameter(0), thread.GetBooleanParameter(1)));
		}

		[NativeFunctionInfo("TileImage", "void", "object,float,float,float")]
		public void TileImageA(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called TileImage with an invalid object.", LogAlertLevel.Error);
				return;
			}
			GraphicsManager.TileImage(image, thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2));
		}

		[NativeFunctionInfo("TileImage", "void", "object,float,float,float,int")]
		public void TileImageB(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called TileImage with an invalid object.", LogAlertLevel.Error);
				return;
			}
			GraphicsManager.TileImage(image, thread.GetFloatParameter(0), thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetIntegerParameter(3));
		}

		[NativeFunctionInfo("SetViewport", "void", "int,int,int,int")]
		public void SetViewport(ScriptThread thread)
		{
			GraphicsManager.Viewport = new System.Drawing.Rectangle(thread.GetIntegerParameter(0), thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), thread.GetIntegerParameter(3));
		}

		[NativeFunctionInfo("ViewportX", "int", "")]
		public void ViewportX(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.Viewport.X);
		}

		[NativeFunctionInfo("ViewportY", "int", "")]
		public void ViewportY(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.Viewport.Y);
		}

		[NativeFunctionInfo("ViewportWidth", "int", "")]
		public void ViewportWidth(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.Viewport.Width);
		}

		[NativeFunctionInfo("ViewportHeight", "int", "")]
		public void ViewportHeight(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.Viewport.Height);
		}

        [NativeFunctionInfo("ResolutionWidth", "int", "")]
        public void ResolutionWidth(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.Resolution[0]);
        }

        [NativeFunctionInfo("ResolutionHeight", "int", "")]
        public void ResolutionHeight(ScriptThread thread)
        {
            thread.SetReturnValue(GraphicsManager.Resolution[1]);
        }

		[NativeFunctionInfo("SetColor", "void", "int")]
		public void SetColor(ScriptThread thread)
		{
			GraphicsManager.VertexColors.AllVertexs = thread.GetIntegerParameter(0);
		}

		[NativeFunctionInfo("Color", "int", "")]
		public void Color(ScriptThread thread)
		{
			thread.SetReturnValue(GraphicsManager.VertexColors[0]);
		}

		[NativeFunctionInfo("CombineARGB", "int", "int,int,int,int")]
		public void CombineColor(ScriptThread thread)
		{
			thread.SetReturnValue(ColorMethods.CombineColor(ColorFormats.A8R8G8B8, thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), thread.GetIntegerParameter(3), thread.GetIntegerParameter(0))); 
		}

		[NativeFunctionInfo("ExtractAlpha", "int", "int")]
		public void ExtractAlpha(ScriptThread thread)
		{
			int r = 0, g = 0, b = 0, a = 0;
			ColorMethods.SplitColor(ColorFormats.A8R8G8B8, thread.GetIntegerParameter(0), out r, out g, out b, out a);
			thread.SetReturnValue(a);
		}

		[NativeFunctionInfo("ExtractRed", "int", "int")]
		public void ExtractRed(ScriptThread thread)
		{
			int r = 0, g = 0, b = 0, a = 0;
			ColorMethods.SplitColor(ColorFormats.A8R8G8B8, thread.GetIntegerParameter(0), out r, out g, out b, out a);
			thread.SetReturnValue(r);
		}

		[NativeFunctionInfo("ExtractGreen", "int", "int")]
		public void ExtractGreen(ScriptThread thread)
		{
			int r = 0, g = 0, b = 0, a = 0;
			ColorMethods.SplitColor(ColorFormats.A8R8G8B8, thread.GetIntegerParameter(0), out r, out g, out b, out a);
			thread.SetReturnValue(g);
		}

		[NativeFunctionInfo("ExtractBlue", "int", "int")]
		public void ExtractBlue(ScriptThread thread)
		{
			int r = 0, g = 0, b = 0, a = 0;
			ColorMethods.SplitColor(ColorFormats.A8R8G8B8, thread.GetIntegerParameter(0), out r, out g, out b, out a);
			thread.SetReturnValue(b);
		}

		[NativeFunctionInfo("SaveImage", "void", "object,string,int,int")]
		public void SaveImageA(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SaveImage with an invalid object.", LogAlertLevel.Error);
				return;
			}
			GraphicsManager.SaveImage(thread.GetStringParameter(1), image, (PixelMapSaveFlags)thread.GetIntegerParameter(2), thread.GetIntegerParameter(3));
		}

		[NativeFunctionInfo("SaveImage", "void", "object,string")]
		public void SaveImageB(ScriptThread thread)
		{
			Image image = ((NativeObject)thread.GetObjectParameter(0)).Object as Image;
			if (image == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SaveImage with an invalid object.", LogAlertLevel.Error);
				return;
			}
			GraphicsManager.SaveImage(thread.GetStringParameter(1), image);
		}

        [NativeFunctionInfo("GrabImage", "object", "int,int,int,int")]
        public void GrabImage(ScriptThread thread)
        {
            thread.SetReturnValue(new ImageScriptObject(new Image(GraphicsManager.GrabPixelmap(thread.GetIntegerParameter(0), thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), thread.GetIntegerParameter(3)), ImageFlags.Dynamic)));
        }

        [NativeFunctionInfo("RenderSceneGraph", "void", "")]
        public void RenderSceneGraphA(ScriptThread thread)
        {
            Engine.GlobalInstance.Map.SceneGraph.Render();
        }

        [NativeFunctionInfo("RenderSceneGraph", "void", "int")]
        public void RenderSceneGraphB(ScriptThread thread)
        {
            Engine.GlobalInstance.Map.SceneGraph.Render(thread.GetIntegerParameter(0));
        }

        [NativeFunctionInfo("SceneGraphLayers", "int[]", "")]
        public void SceneGraphLayers(ScriptThread thread)
        {
            int[] layers = SceneGraph.Layers;
            int arrayIndex = thread.AllocateArray(DataType.Int, SceneGraph.Layers.Length);
            for (int i = 0; i < SceneGraph.Layers.Length; i++)
                thread.SetArrayElement(arrayIndex, i, layers[i]);
            thread.SetReturnValueArray(arrayIndex);
        }

        [NativeFunctionInfo("RenderEntity", "void", "object,float,float,float")]
        public void RenderEntity(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called RenderEntity with an invalid object.", LogAlertLevel.Error);
                return;
            }
            Transformation transformation = entity.CalculateTransformation();
            entity.Render(new Transformation(thread.GetFloatParameter(1) - transformation.X, thread.GetFloatParameter(2) - transformation.Y, thread.GetFloatParameter(3), 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f), null, entity.DepthLayer);
        }
	}

}