/* 
 * File: IO.cs
 *
 * This source file contains the declaration of the IOFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Runtime.Scripting.ScriptingFunctions
{

    /// <summary>
    ///     Wraps a stream object used in a script.
    /// </summary>
    public class StreamScriptObject : NativeObject
    {
        public StreamScriptObject(ScriptStream stream)
        {
            _nativeObject = stream;
        }

        public StreamScriptObject() { }
    }

	/// <summary>
	///		Stores details on a stream object that can be used within a script.
	/// </summary>
	public class ScriptStream
	{
		#region Members
		#region Variables

		private Stream _stream = null;
		private BinaryReader _reader = null;
		private BinaryWriter _writer = null;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the stream this class is storing details on.
		/// </summary>
		public Stream Stream
		{
			get { return _stream; }
			set { _stream = value; }
		}

		/// <summary>
		///		Gets or sets the BinaryReader that this class is storing in conjunction with the stream.
		/// </summary>
		public BinaryReader Reader
		{
			get { return _reader; }
			set { _reader = value; }
		}

		/// <summary>
		///		Gets or sets the BinaryWriter that this class is storing in conjunction with the stream.
		/// </summary>
		public BinaryWriter Writer
		{
			get { return _writer; }
			set { _writer = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Deinitializes this class and closes all streams.
		/// </summary>
		~ScriptStream()
		{
			Close();
		}

		/// <summary>
		///		Closes all streams, readers and writers associated with this class instance.
		/// </summary>
		public void Close()
		{
			_reader.Close();
			_writer.Close();
			_stream.Close();
		}

		/// <summary>
		///		Creates a new instance of this class with the given values.
		/// </summary>
		/// <param name="stream">Stream that this class should store details on.</param>
		public ScriptStream(Stream stream)
		{
			_stream = stream;
			_reader = new BinaryReader(stream);
			_writer = new BinaryWriter(stream);
		}

		#endregion
	}

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class IOFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("OpenStream", "object", "string,int")]
		public void OpenStream(ScriptThread thread)
		{
			Stream stream = StreamFactory.RequestStream(thread.GetStringParameter(0), (StreamMode)thread.GetIntegerParameter(1));
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called OpenStream with an unreachable url.", LogAlertLevel.Error);
				return;
			}
            thread.SetReturnValue(new StreamScriptObject(new ScriptStream(stream)));
		}

		[NativeFunctionInfo("CloseStream", "void", "object")]
		public void CloseStream(ScriptThread thread)
		{
            ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null) 
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script")  + " called CloseStream with an invalid object.",LogAlertLevel.Error);
				return;
			}
			stream.Close();
		}

		[NativeFunctionInfo("SeekStream", "void", "object,int")]
		public void SeekStream(ScriptThread thread)
		{
            ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SeekStream with an invalid object.", LogAlertLevel.Error);
				return;
			}
			stream.Stream.Position = thread.GetIntegerParameter(1);
		}

		[NativeFunctionInfo("StreamPosition", "int", "object")]
		public void StreamPosition(ScriptThread thread)
		{
            ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamPosition with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(stream.Stream.Position);
		}

		[NativeFunctionInfo("StreamLength", "int", "object")]
		public void StreamLength(ScriptThread thread)
		{
            ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamLength with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(stream.Stream.Length);
		}

		[NativeFunctionInfo("StreamWrite", "void", "object,bool")]
		public void StreamWriteA(ScriptThread thread)
		{
            ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamWrite with an invalid object.", LogAlertLevel.Error);
				return;
			}
			stream.Writer.Write(thread.GetBooleanParameter(1));
		}

		[NativeFunctionInfo("StreamWrite", "void", "object,byte")]
		public void StreamWriteB(ScriptThread thread)
		{
            ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamWrite with an invalid object.", LogAlertLevel.Error);
				return;
			}
			stream.Writer.Write(thread.GetByteParameter(1));
		}

		[NativeFunctionInfo("StreamWrite", "void", "object,string")]
		public void StreamWriteC(ScriptThread thread)
		{
            ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamWrite with an invalid object.", LogAlertLevel.Error);
				return;
			}
			stream.Writer.Write(thread.GetStringParameter(1));
		}

		[NativeFunctionInfo("StreamWrite", "void", "object,int")]
		public void StreamWriteD(ScriptThread thread)
		{
            ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamWrite with an invalid object.", LogAlertLevel.Error);
				return;
			}
			stream.Writer.Write(thread.GetIntegerParameter(1));
		}

		[NativeFunctionInfo("StreamWrite", "void", "object,short")]
		public void StreamWriteE(ScriptThread thread)
		{
            ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamWrite with an invalid object.", LogAlertLevel.Error);
				return;
			}
			stream.Writer.Write(thread.GetShortParameter(1));
		}

		[NativeFunctionInfo("StreamWrite", "void", "object,float")]
		public void StreamWriteF(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamWrite with an invalid object.", LogAlertLevel.Error);
				return;
			}
			stream.Writer.Write(thread.GetFloatParameter(1));
		}

		[NativeFunctionInfo("StreamWrite", "void", "object,long")]
		public void StreamWriteG(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamWrite with an invalid object.", LogAlertLevel.Error);
				return;
			}
			stream.Writer.Write(thread.GetLongParameter(1));
		}

		[NativeFunctionInfo("StreamWrite", "void", "object,double")]
		public void StreamWriteH(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamWrite with an invalid object.", LogAlertLevel.Error);
				return;
			}
			stream.Writer.Write(thread.GetDoubleParameter(1));
		}

		[NativeFunctionInfo("StreamReadBool", "bool", "object")]
		public void StreamReadBool(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamReadBool with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(stream.Reader.ReadBoolean());
		}

		[NativeFunctionInfo("StreamReadByte", "byte", "object")]
		public void StreamReadByte(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamReadByte with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(stream.Reader.ReadByte());
		}

		[NativeFunctionInfo("StreamReadString", "string", "object")]
		public void StreamReadString(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamReadString with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(stream.Reader.ReadString());
		}

		[NativeFunctionInfo("StreamReadInt", "int", "object")]
		public void StreamReadInt(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamReadInt with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(stream.Reader.ReadInt32());
		}

		[NativeFunctionInfo("StreamReadShort", "short", "object")]
		public void StreamReadShort(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamReadShort with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(stream.Reader.ReadInt16());
		}

		[NativeFunctionInfo("StreamReadFloat", "float", "object")]
		public void StreamReadFloat(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamReadFloat with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(stream.Reader.ReadSingle());
		}

		[NativeFunctionInfo("StreamReadLong", "long", "object")]
		public void StreamReadLong(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamReadLong with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(stream.Reader.ReadInt64());
		}

		[NativeFunctionInfo("StreamReadDouble", "double", "object")]
		public void StreamReadDouble(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamReadDouble with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(stream.Reader.ReadDouble());
		}

		[NativeFunctionInfo("StreamPeekBool", "bool", "object")]
		public void StreamPeekBool(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamPeekBool with an invalid object.", LogAlertLevel.Error);
				return;
			}
			long position = stream.Stream.Position;
			bool value = stream.Reader.ReadBoolean();
			stream.Stream.Position = position;
			thread.SetReturnValue(value);
		}

		[NativeFunctionInfo("StreamPeekByte", "byte", "object")]
		public void StreamPeekByte(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamPeekByte with an invalid object.", LogAlertLevel.Error);
				return;
			}
			long position = stream.Stream.Position;
			byte value = stream.Reader.ReadByte();
			stream.Stream.Position = position;
			thread.SetReturnValue(value);
		}

		[NativeFunctionInfo("StreamPeekString", "string", "object")]
		public void StreamPeekString(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamPeekString with an invalid object.", LogAlertLevel.Error);
				return;
			}
			long position = stream.Stream.Position;
			string value = stream.Reader.ReadString();
			stream.Stream.Position = position;
			thread.SetReturnValue(value);
		}

		[NativeFunctionInfo("StreamPeekInt", "int", "object")]
		public void StreamPeekInt(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamPeekInt with an invalid object.", LogAlertLevel.Error);
				return;
			}
			long position = stream.Stream.Position;
			int value = stream.Reader.ReadInt32();
			stream.Stream.Position = position;
			thread.SetReturnValue(value);
		}

		[NativeFunctionInfo("StreamPeekShort", "short", "object")]
		public void StreamPeekShort(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamPeekShort with an invalid object.", LogAlertLevel.Error);
				return;
			}
			long position = stream.Stream.Position;
			short value = stream.Reader.ReadInt16();
			stream.Stream.Position = position;
			thread.SetReturnValue(value);
		}

		[NativeFunctionInfo("StreamPeekFloat", "float", "object")]
		public void StreamPeekFloat(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamPeekFloat with an invalid object.", LogAlertLevel.Error);
				return;
			}
			long position = stream.Stream.Position;
			float value = stream.Reader.ReadSingle();
			stream.Stream.Position = position;
			thread.SetReturnValue(value);
		}

		[NativeFunctionInfo("StreamPeekLong", "long", "object")]
		public void StreamPeekLong(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamPeekLong with an invalid object.", LogAlertLevel.Error);
				return;
			}
			long position = stream.Stream.Position;
			long value = stream.Reader.ReadInt64();
			stream.Stream.Position = position;
			thread.SetReturnValue(value);
		}

		[NativeFunctionInfo("StreamPeekDouble", "double", "object")]
		public void StreamPeekDouble(ScriptThread thread)
		{
			ScriptStream stream = ((NativeObject)thread.GetObjectParameter(0)).Object as ScriptStream;
			if (stream == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StreamPeekDouble with an invalid object.", LogAlertLevel.Error);
				return;
			}
			long position = stream.Stream.Position;
			double value = stream.Reader.ReadDouble();
			stream.Stream.Position = position;
			thread.SetReturnValue(value);
		}

        [NativeFunctionInfo("FileExists", "bool", "string")]
        public void FileExists(ScriptThread thread)
        {
            thread.SetReturnValue(File.Exists(thread.GetStringParameter(0)));
        }

        [NativeFunctionInfo("DeleteFile", "void", "string")]
        public void DeleteFile(ScriptThread thread)
        {
            File.Delete(thread.GetStringParameter(0));
        }

        [NativeFunctionInfo("CopyFile", "void", "string,string")]
        public void CopyFile(ScriptThread thread)
        {
            File.Copy(thread.GetStringParameter(0), thread.GetStringParameter(1), true);
        }
	}

}