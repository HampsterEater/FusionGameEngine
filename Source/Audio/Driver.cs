/* 
 * File: Driver.cs
 *
 * This source file contains the declaration of the IAudioDriver interface which is
 * used as an intermediary between the audio driver (DirectSound, OpenAL, ...) and the
 * actual audio API calls.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Audio
{

	/// <summary>
	///		The IAudioDriver interface is used as an abstracted intermediary between
	///		the calls to the audio driver (DirectSound, OpenAL, ...) and the actual
	///		audio API calls.
	/// </summary>
	public interface IAudioDriver
	{
		ISampleBuffer CreateSampleBuffer(Sample sample, SoundFlags flags);
		void AttachToControl(Control control);
        Vector ListenerPosition { get; set; }
	}

}