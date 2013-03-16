/* 
 * File: Sample.cs
 *
 * This source file contains the declaration of all the classes used to
 * store audio samples, including the SampleFactory declaration.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using BinaryPhoenix.Fusion.Audio;
//using BinaryPhoenix.Fusion.Audio.SampleFactorys;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Audio
{
    /// <summary>
	///		Used to tell a sample loaded that new data is required for a streamed sample.
	/// </summary>
	/// <param name="sample">Sample that streamed data needs to be loaded from.</param>
    /// <param name="size">Size in bytes of data that is required.</param>
    /// <param name="position">The variable used for tracking streamed data.</param>
    /// <param name="wrap">Variable indicating if data can be wrapped (used for looping).</param>
    /// <returns>Newly loaded data.</returns>
	public delegate byte[] RequestStreamedData(Sample sample, long size, ref int position, bool wrap);

	/// <summary>
	///		Used as an intermediary between the driver's sample buffer and
	///		the Sound class.
	/// </summary>
	public interface ISampleBuffer
	{
		IAudioDriver Driver { get; }
		Sample Sample { get; set; }
		bool Paused { get; set; }
		float Pan { get; set; }
		float Volume { get; set; }
		int Position { get; set; }
        Vector Position3D { get; set; }
        float InnerRadius { get; set; }
        float OuterRadius { get; set; }
        int Length { get; }
		bool Finished { get; }
		bool Looping { get; set; }
		int Frequency { get; set; }
		void Play();
		void Stop();
        ISampleBuffer Clone();

        byte[] GetCurrentData();
	}

	/// <summary>
	///		Used to store a single audio sample, which can be used by the audio driver
	///		to create an audio buffer capable of being played back.
	/// </summary>
	public class Sample
	{
		#region Members
		#region Variables

		private SampleFormat _format;
		private byte[] _data;

		private int _bitsPerSample;
		private int _channelCount;
		private int _sampleRate;
		private int _byteRate;
		private int _blockAlign;

        private bool _streamed;
        private Stream _dataStream;
        private long _dataChunkPosition, _dataChunkLength;

        private bool _playedUsingMCI = false;

        private object _url;

		#endregion
        #region Events

        public RequestStreamedData StreamedDataRequired;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets if this sample is streamed.
        /// </summary>
        public bool Streamed
        {
            get { return _streamed; }
            set { _streamed = value; }
        }

        /// <summary>
        ///     Gets or sets the file stream used when streaming data.
        /// </summary>
        public Stream DataStream
        {
            get { return _dataStream; }
            set { _dataStream = value; }
        }

        /// <summary>
        ///     Gets or sets the offset in the file stream of the data chunk.
        /// </summary>
        public long DataChunkPosition
        {
            get { return _dataChunkPosition; }
            set { _dataChunkPosition = value; }
        }

        /// <summary>
        ///     Gets or sets the data chunk length in bytes.
        /// </summary>  
        public long DataChunkLength
        {
            get { return _dataChunkLength; }
            set { _dataChunkLength = value; }
        }

        /// <summary>
		///		Sets or Gets the array that stores the samples audio data.
		/// </summary>
		public byte[] Data
		{
			get { return _data;  }
			set { _data = value; }
		}

		/// <summary>
		///		Gets or Sets the bits used per sample.
		/// </summary>
		public int BitsPerSample
		{
			get { return _bitsPerSample;  }
			set { _bitsPerSample = value; }
		}

		/// <summary>
		///		Gets or Sets the amount of channels used in this sample.
		/// </summary>
		public int ChannelCount
		{
			get { return _channelCount; }
			set { _channelCount = value; }
		}

		/// <summary>
		///		Gets or Sets the sample rate.
		/// </summary>
		public int SampleRate
		{
			get { return _sampleRate; }
			set { _sampleRate = value; }
		}

		/// <summary>
		///		Gets or Sets the byte rate.
		/// </summary>
		public int ByteRate
		{
			get { return _byteRate; }
			set { _byteRate = value; }
		}

		/// <summary>
		///		Gets or Sets the block alignment.
		/// </summary>
		public int BlockAlign
		{
			get { return _blockAlign; }
			set { _blockAlign = value; }
		}

        /// <summary>
        ///     Gets or sets if this sample should be played using MCI.
        /// </summary>
        public bool PlayedUsingMCI
        {
            get { return _playedUsingMCI; }
            set { _playedUsingMCI = value; }
        }

        /// <summary>
        ///     Gets or sets the url this sample was loaded from.
        /// </summary>
        public object URL
        {
            get { return _url; }
            set { _url = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new sample with the given format and data length.
		/// </summary>
		public Sample(SampleFormat format,int length)
		{
			_format = format;
			_data = new byte[length];
		}

		#endregion
	}

	/// <summary>
	///		Used to create a bitmask defining how a sample should be saved.
	/// </summary>
	public enum SampleSaveFlags
	{
		None = 0x00000000
	}

	/// <summary>
	///		Used to define what format the sample data is stored in.
	/// </summary>
	public enum SampleFormat
	{
		MONO8		= 0x00000001,
		MONO16LE	= 0x00000002,
		MONO16BE	= 0x00000004,
		STEREO8		= 0x00000008,
		STEREO16LE	= 0x00000010,
		STEREO16BE	= 0x00000020
	}

	/// <summary>
	///     Used to request a loader for a specific sample file format.
	/// </summary>
	public abstract class SampleFactory
	{
		#region Members
		#region Variables

		private static ArrayList _loaderList = new ArrayList();

		#endregion
		#endregion
		#region Methods
		/// <summary>
		///     This method is called when Sample load is requested, if you return a Sample
		///     it will return it from the user else it will keep trying all the other SampleFactorys.
		///     until it does get one
		/// </summary>
		/// <param name="path">File path or object of the sound to load.</param>
        /// <param name="streamed">If set to true the sound will be loaded in piece by piece rather than all at once.</param>
		/// <returns>A Sample or NULL if this factory can't load the given image file.</returns>
		protected abstract Sample RequestLoad(object path, bool streamed);

		/// <summary>
		///     This method is called when Sample save is requested, if it returns true
		///		the calling method will stop illiterating through the SampleFactorys and 
		///		return success to the user.
		/// </summary>
		/// <param name="path">File path or object of the image to load.</param>
		/// <param name="sample">Sample to save.</param>
		/// <param name="flags">Bitmask of flags defining how the sample should be saved.</param>
		/// <returns>True if the save was successfull else false.</returns>
		protected abstract bool RequestSave(object path, Sample sample, SampleSaveFlags flags);

		/// <summary>
		///     This method is called when a sample load is requested, it illiterates through all
		///     the registered SampleFactory instances to see if there is one capable to loading the
		///     given format.
		/// </summary>
		/// <param name="path">File path or object of the sound to load.</param>
        /// <param name="streamed">If set to true the sound will be loaded in piece by piece rather than all at once.</param>
        /// <returns>A Sample or NULL if it can't find a factory able to load the given audio sample format.</returns>
		public static Sample LoadSample(object path, bool streamed)
		{
            string fullPath = path.ToString();
            if (fullPath.ToString().ToLower().StartsWith(Environment.CurrentDirectory.ToLower()) == false)
                fullPath = Environment.CurrentDirectory + "\\" + fullPath;

			Sample sample;
			foreach (SampleFactory factory in _loaderList)
			{
				sample = factory.RequestLoad(path, streamed);
                if (sample != null)
                {
                    sample.URL = fullPath;
                    return sample;
                }
			}

            // Can't load it natively? Use MCI.
            sample = new Sample(SampleFormat.STEREO16BE, 0);
            sample.URL = fullPath;
            sample.PlayedUsingMCI = true;

			return sample;
		}

		/// <summary>
		///     This method is called when a Sample save is requested, it illiterates through all
		///     the registered SampleFactory instances to see if there is one capable of saving the
		///     given format.
		/// </summary>
		/// <param name="sample">Sample to be saved.</param>
		/// <param name="path">File path or save sample to.</param>
		/// <param name="flags">Bitmask of flags to define how the sample should be saved.</param>
		/// <returns>True if save was successfull else false.</returns>
		public static bool SaveSample(object path, Sample sample, SampleSaveFlags flags)
		{
			foreach (SampleFactory factory in _loaderList)
			{
				if (factory.RequestSave(path, sample, flags) == true) return true;
			}
			return false;
		}
		public static void SaveSample(object path, Sample sample)
		{
			SaveSample(path, sample, 0);
		}

		/// <summary>
		///     Adds (registers) an instance to the LoaderList, so it will be used when a
		///     sample load is requested.
		/// </summary>
		protected void Register()
		{
			_loaderList.Add(this);
		}

		#endregion		
	}

}