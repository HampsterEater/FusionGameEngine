/* 
 * File: WAV Factory.cs
 *
 * This source file contains the declaration of the WAVFactory class, which is
 * used to load audio samples from a wav file's (.wav).
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System.Collections;
using System.IO;
using System;
using BinaryPhoenix.Fusion.Audio;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Audio.SampleFactorys
{

	/// <summary>
	///     The WAVFactory class is used to load audio samples from 
	///     wav file's (.wav).
	/// </summary>
	public sealed class WAVFactory : SampleFactory
	{
		#region Methods

		public WAVFactory()
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
			if (path.ToString().ToLower().EndsWith(".wav") == false &&
				path.ToString().ToLower().EndsWith(".wave") == false) return false;

			throw new Exception("WAV save factory currently unfinished and as such unsupported.");
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
			if (path.ToString().ToLower().EndsWith(".wav") == false &&
				path.ToString().ToLower().EndsWith(".wave") == false) return null;

			Stream stream = StreamFactory.RequestStream(path, StreamMode.Open);
			if (stream == null) return null;
			BinaryReader reader = new BinaryReader(stream);

			//Check the header is correct.
			if (reader.ReadByte() == 'R' && reader.ReadByte() == 'I' &&
				reader.ReadByte() == 'F' && reader.ReadByte() == 'F')
			{
				// Read the header of the RIFF chunk
				reader.ReadBytes(4); // Chunk length
				reader.ReadBytes(4); // WAVE identifier.

				// Local variabl's used to load chunks.
				int audioFormat, channelCount;
				int sampleRate,  byteRate;
				int blockAlign,  bitsPerSample;
				int chunkIndex = 0;
				bool foundFormatChunk = false;
				bool foundDataChunk = false;
                long dataChunkPosition = 0;
                long dataChunkLength = 0;
				Sample sample  = null;

				// Read in every chunk till we find the end of file chunk or 
				// we read past the end of the stream.
				while (true)
				{
					// Check we are not at the end of the stream.
					if (stream.Position >= stream.Length) break;

					// Read in chunk's data
					byte[] chunkTypeChars = reader.ReadBytes(4);
					int chunkLength       = reader.ReadInt32();
					string chunkType	  = ((char)chunkTypeChars[0]).ToString() + ((char)chunkTypeChars[1]).ToString() +
											((char)chunkTypeChars[2]).ToString() + ((char)chunkTypeChars[3]).ToString();
					long originalPosition = stream.Position;

					switch (chunkType)
					{
						// Format chunk.
						case "fmt ":

							// Check that the format chunk is first.
							if (chunkIndex != 0) throw new Exception("Found out of sequence format chunk while reading wav file.");

							// Check we haven't found multiple occurances.
							if (foundFormatChunk == true) throw new Exception("Found multiple format chunks while reading wav file, only singular chunks are valid.");
							foundFormatChunk = false;

							// Read in the format specifications.
							audioFormat   = reader.ReadInt16();
							channelCount  = reader.ReadInt16();
							sampleRate    = reader.ReadInt32();
							byteRate	  = reader.ReadInt32();
							blockAlign	  = reader.ReadInt16();
							bitsPerSample = reader.ReadInt16();

							// Make sure audio format is PCM as other mode
							// are more or less obselete and unsupported by this loader.
							if (audioFormat != 1) throw new Exception("Encountered unsupported audio format while reading wav file.");

							// Check for extra parameter if format is PCM.
							if (stream.Position - originalPosition < chunkLength)
							{
								int extraParamSize = reader.ReadInt16();
								// Lets just skip the extra parameters for
								// now as we don't need them.
								stream.Position += extraParamSize;
							}

							// Create the audio sample to write data to, based
							// on the number of channels and amount of bits used
							// to store the audio sample.
							if (bitsPerSample == 8 && channelCount == 1)
									sample = new Sample(SampleFormat.MONO8,0);
							if (bitsPerSample == 16 && channelCount == 1)
									sample = new Sample(SampleFormat.MONO16LE,0);
							if (bitsPerSample == 8  && channelCount == 2)
									sample = new Sample(SampleFormat.STEREO8,0);
							if (bitsPerSample == 16 && channelCount == 2)
									sample = new Sample(SampleFormat.STEREO16LE,0);

							// Write the format description into the sample.
							sample.BitsPerSample = bitsPerSample;
							sample.ChannelCount  = channelCount;
							sample.SampleRate    = sampleRate;
							sample.ByteRate		 = byteRate;
							sample.BlockAlign	 = blockAlign;

							break;

						// Audio data chunk.
						case "data":

							// Check we haven't found multiple occurances.
							if (foundDataChunk == true) throw new Exception("Found multiple data chunks while reading wav file, only singular chunks are valid.");
							foundDataChunk = false;

							// Read all the audio data into the sample.
                            dataChunkPosition = stream.Position;
                            dataChunkLength = chunkLength;
                            if (streamed == false)
                                sample.Data = reader.ReadBytes(chunkLength);
                            else
                                stream.Position += chunkLength;

							break;

						default:

							// Probably a good idea to throw up an error
							// here but for the moment lets just skip it :).
							stream.Position += chunkLength;

							break;
					}

					chunkIndex++;
				}

                // If its a streamed sound then link it up the the stream loader.
                if (streamed == true)
                {
                    sample.Streamed = true;
                    sample.DataStream = stream;
                    sample.DataChunkPosition = dataChunkPosition;
                    sample.DataChunkLength = dataChunkLength;
                    sample.StreamedDataRequired += RequestStreamedData;
                }
                else
                    stream.Close();
				
				return sample;
			}
			else // Header check
			{
				reader.Close();
				return null;
			}

		}

        /// <summary>
        ///		Loads a chunk of data from a samples stream.
        /// </summary>
        /// <param name="sample">Sample that streamed data needs to be loaded from.</param>
        /// <param name="size">Size in bytes of data that is required.</param>
        /// <param name="position">The variable used for tracking streamed data.</param>
        /// <param name="wrap">Variable indicating if data can be wrapped (used for looping).</param>
        /// <returns>Newly loaded data.</returns>
        public byte[] RequestStreamedData(Sample sample, long size, ref int position, bool wrap)
        {
            if ((sample.DataStream.Position - sample.DataChunkPosition) >= sample.DataChunkLength)
                return new byte[0];

            // Keep reading and wrapping around until we read all the data that we need
            byte[] data = null;
            long readPosition = sample.DataChunkPosition + position;
            sample.DataStream.Position = readPosition;

            if (wrap == true)
            {
                data = new byte[size];
                long dataPosition = 0;
                while (true)
                {
                    // Read in what is left.
                    long sizeLeft = sample.DataChunkLength - (sample.DataStream.Position - sample.DataChunkPosition);
                    long possibleSize = ((data.Length - dataPosition) >= sizeLeft ? sizeLeft : (data.Length - dataPosition));
                    sample.DataStream.Read(data, (int)dataPosition, (int)possibleSize);
                    dataPosition += possibleSize;

                    // Wrap around to the beginning if neccessary.
                    if (dataPosition >= data.Length)
                        break;
                    else
                        sample.DataStream.Position = sample.DataChunkPosition;
                }
            }
            else
            {
                long sizeLeft = sample.DataChunkLength - (sample.DataStream.Position - sample.DataChunkPosition);
                data = new byte[size > sizeLeft ? sizeLeft : size];
                sample.DataStream.Read(data, 0, data.Length);  
            }

            position = (int)(sample.DataStream.Position - sample.DataChunkPosition);

            return data;
        }

		#endregion
	}

}