/* 
 * File: Audio.cs
 *
 * This source file contains the declaration of the AudioFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using BinaryPhoenix.Fusion.Audio;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Scripting;

namespace BinaryPhoenix.Fusion.Engine.ScriptingFunctions
{

    /// <summary>
    ///     Wraps a sound object used in a script.
    /// </summary>
    public class SoundScriptObject : NativeObject
    {
        public SoundScriptObject(Sound sound)
        {
            _nativeObject = sound;
        }

        public SoundScriptObject() { }
    }

    /// <summary>
    ///     Wraps a sound channel object used in a script.
    /// </summary>
    public class ChannelScriptObject : NativeObject
    {
        public ChannelScriptObject(ISampleBuffer buffer)
        {
            _nativeObject = buffer;
        }
    }

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class AudioFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("LoadSound", "object", "string,int")]
		public void LoadSound(ScriptThread thread)
		{
            thread.SetReturnValue(new SoundScriptObject(AudioManager.LoadSound(thread.GetStringParameter(0), (SoundFlags)thread.GetIntegerParameter(1))));
		}

		[NativeFunctionInfo("PlaySound", "object", "object")]
		public void PlaySound(ScriptThread thread)
		{
            Sound sound = ((NativeObject)thread.GetObjectParameter(0)).Object as Sound;
			if (sound == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called PlaySound with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(new ChannelScriptObject(sound.Play()));
		}

        [NativeFunctionInfo("PauseChannel", "void", "object")]
        public void PauseChannel(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called PauseChannel with an invalid object.", LogAlertLevel.Error);
                return;
            }
            sound.Paused = true;
        }

        [NativeFunctionInfo("StopChannel", "void", "object")]
        public void StopChannel(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called StopChannel with an invalid object.", LogAlertLevel.Error);
                return;
            }
            sound.Stop();
        }

        [NativeFunctionInfo("ResumeChannel", "void", "object")]
        public void ResumeChannel(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ResumeChannel with an invalid object.", LogAlertLevel.Error);
                return;
            }
            sound.Paused = false;
        }

        [NativeFunctionInfo("ChannelPaused", "bool", "object")]
        public void ChannelPaused(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelPaused with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Paused);
        }

		[NativeFunctionInfo("ChannelFinished", "bool", "object")]
		public void ChannelFinished(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelFinished with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Finished);
		}

		[NativeFunctionInfo("ChannelLength", "int", "object")]
        public void ChannelLength(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelLength with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Length);
		}

		[NativeFunctionInfo("ChannelPosition", "int", "object")]
		public void ChannelPosition(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelPosition with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Position);
		}

        [NativeFunctionInfo("SetChannelLooping", "void", "object,bool")]
        public void SetChannelLooping(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetChannelLooping with an invalid object.", LogAlertLevel.Error);
                return;
            }
            sound.Looping = thread.GetBooleanParameter(1);
		}

        [NativeFunctionInfo("ChannelLooping", "bool", "object")]
        public void ChannelLooping(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelLooping with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Looping);
		}

        [NativeFunctionInfo("SetChannelFrequency", "void", "object,int")]
        public void SetChannelFrequency(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetChannelFrequency with an invalid object.", LogAlertLevel.Error);
                return;
            }
            sound.Frequency = thread.GetIntegerParameter(1);
		}

        [NativeFunctionInfo("ChannelFrequency", "int", "object")]
        public void ChannelFrequency(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelFrequency with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Frequency);
		}

        [NativeFunctionInfo("SetChannelPan", "void", "object,float")]
        public void SetChannelPan(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetChannelPan with an invalid object.", LogAlertLevel.Error);
                return;
            }
            sound.Pan = thread.GetFloatParameter(1);
		}

        [NativeFunctionInfo("ChannelPan", "float", "object")]
        public void ChannelPan(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelPan with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Pan);
		}

        [NativeFunctionInfo("SetChannelVolume", "void", "object,float")]
        public void SetChannelVolume(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetChannelVolume with an invalid object.", LogAlertLevel.Error);
                return;
            }
            sound.Volume = thread.GetFloatParameter(1);
		}

        [NativeFunctionInfo("ChannelVolume", "float", "object")]
        public void ChannelVolume(ScriptThread thread)
		{
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelVolume with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Volume);
		}

        [NativeFunctionInfo("SetChannel3DPosition", "void", "object,float,float,float")]
        public void SetChannel3DVolume(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetChannel3DPosition with an invalid object.", LogAlertLevel.Error);
                return;
            }
            sound.Position3D = new Vector(thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3));
        }

        [NativeFunctionInfo("ChannelX", "float", "object")]
        public void ChannelX(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelX with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Position3D.X);
        }

        [NativeFunctionInfo("ChannelY", "float", "object")]
        public void ChannelY(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelY with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Position3D.Y);
        }

        [NativeFunctionInfo("ChannelZ", "float", "object")]
        public void ChannelZ(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelZ with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.Position3D.Z);
        }

        [NativeFunctionInfo("SetChannelRadius", "void", "object,float,float")]
        public void SetChannelRadius(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called SetChannelRadius with an invalid object.", LogAlertLevel.Error);
                return;
            }
            sound.InnerRadius = thread.GetFloatParameter(1);
            sound.OuterRadius = thread.GetFloatParameter(2);
        }

        [NativeFunctionInfo("ChannelInnerRadius", "float", "object")]
        public void ChannelInnerRadius(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelInnerRadius with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.InnerRadius);
        }

        [NativeFunctionInfo("ChannelOuterRadius", "float", "object")]
        public void ChannelOuterRadius(ScriptThread thread)
        {
            ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ChannelOuterRadius with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(sound.OuterRadius);
        }

	}

}