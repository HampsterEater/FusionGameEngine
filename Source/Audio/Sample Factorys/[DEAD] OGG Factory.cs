/* 
 * File: OGG Factory.cs
 *
 * This source file contains the declaration of the OGGFactory class, which is
 * used to load audio samples from vorbis audio files file's (.ogg).
 * 
 * *** Audiere needs to be removed and a homebrew version added ***
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System.Collections;
using System.IO;
using System;
using BinaryPhoenix.Fusion.Audio;
using BinaryPhoenix.Fusion.Runtime;
using ManagedAudiere;

namespace BinaryPhoenix.Fusion.Audio.SampleFactorys
{

	/// <summary>
	///     The OGGFactory class is used to load audio samples from 
	///     vorbis audio file's (.OGG).
	/// </summary>
	public sealed class OGGFactory : SampleFactory
	{
		#region Methods

		public OGGFactory()
		{
			Register();
		}

		/// <summary>
		///     This method is called when Sample save is requested, if it returns true
		///		the calling method will stop illiterating through the SampleFactorys and 
		///		return success to the user.
		/// </summary>
		/// <param name="path">File path or object of the image to load.</param>
		/// <param name="sample">Sample to save.</param>
		/// <param name="flags">Bitmask of flags defining how the sample should be saved.</param>
		/// <returns>True if the save was successfull else false.</returns>
		protected override bool RequestSave(object path, Sample sample, SampleSaveFlags flags)
		{
			if (path.ToString().ToLower().EndsWith(".ogg") == false) return false;

			throw new Exception("OGG save factory currently unfinished and as such unsupported.");
		}

		/// <summary>
		///     This method is called when Sample load is requested, if you return a Sample
		///     it will return it from the user else it will keep trying all the other SampleLoaders
		///     until it does get one
		/// </summary>
		/// <param name="path">File path of the sound to load.</param>
        /// <param name="streamed">If set to true the sound will be loaded in piece by piece rather than all at once.</param>
		/// <returns>New Sample instance or NULL if this factory can't load the given audio sample file format.</returns>
		protected override Sample RequestLoad(object path, bool streamed)
		{
			// Load in the audio file's data.
			Stream stream = StreamFactory.RequestStream(path, StreamMode.Open);
			if (stream == null) return null;
			if (stream.ReadByte() != 'O' || stream.ReadByte() != 'g' || stream.ReadByte() != 'g' || stream.ReadByte() != 'S')
			{
				stream.Close();
				return null;
			}
			stream.Position = 0;

			byte[] data = new byte[stream.Length];
			stream.Read(data, 0, (int)stream.Length);
			stream.Close();

			// Create an audiere memory file to place our data into.
			ManagedAudiere.File file = Audiere.CreateMemoryFile(new MemoryFileBuffer(data, data.Length));
			SampleSource audiereSample = Audiere.OpenSampleSource(file);
			SampleFormatData sampleFormat = audiereSample.GetFormat();

			int size = audiereSample.Length * (Audiere.GetSampleSize(sampleFormat.sample_format) * sampleFormat.channel_count);
			MemoryFileBuffer audiereBuffer = new MemoryFileBuffer(new byte[size], size);
			audiereSample.Read(audiereSample.Length, audiereBuffer);
			
			// Put the audiere audio buffer into a new sample buffer.
			byte[] pcmBuffer = audiereBuffer.GetBuffer();
			Sample sample = null;

			if (sampleFormat.channel_count == 1)
				sample = new Sample(SampleFormat.MONO16LE, pcmBuffer.Length);
			else
				sample = new Sample(SampleFormat.STEREO16LE, pcmBuffer.Length);

			sample.SampleRate = sampleFormat.sample_rate;
			sample.ChannelCount = sampleFormat.channel_count;
			sample.BlockAlign = Audiere.GetSampleSize(sampleFormat.sample_format) * sampleFormat.channel_count;
			sample.ByteRate = sample.SampleRate * sample.BlockAlign;
			sample.BitsPerSample = Audiere.GetSampleSize(sampleFormat.sample_format) * 8;

			for (int i = 0; i < pcmBuffer.Length; i++)
				sample.Data[i] = pcmBuffer[i];

			return sample;
		}
		#endregion
	}

}