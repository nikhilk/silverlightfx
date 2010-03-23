//-----------------------------------------------------------------------
// <copyright file="WavParser.cs" company="Gilles Khouzam">
// (c) Copyright Gilles Khouzam
// This source is subject to the Microsoft Public License (Ms-PL)
// All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;

namespace Translate.Services.Audio {

    /// <summary>
    /// Class WavParser
    /// Parses a standard WAVE file
    /// </summary>
    internal sealed class WavParser : RiffParser {

        /// <summary>
        /// The minimum size of the format structure
        /// </summary>
        private const uint MinFormatSize = WaveFormatEx.SizeOf - sizeof(short);

        private WaveFormatEx waveFormat;
        private long duration;

        /// <summary>
        /// Initializes a new instance of the WavParser class.
        /// </summary>
        /// <param name="stream">A stream that contains the Wave data</param>
        public WavParser(Stream stream)
            : base(stream, FourCC.Riff, 0) {
            if (RiffType != FourCC.Wave) {
                throw new InvalidOperationException("File is not a WAV file");
            }
        }

        /// <summary>
        /// Gets the WAVEFORMATEX structure
        /// </summary>
        public WaveFormatEx WaveFormatEx {
            get {
                return this.waveFormat;
            }
        }

        /// <summary>
        /// Gets the duration of the Wave file
        /// </summary>
        public long Duration {
            get {
                return this.duration;
            }
        }

        /// <summary>
        /// Parses the RIFF WAVE header.
        /// .wav files should look like this:           
        /// RIFF ('WAVE'                                      
        /// 'fmt ' = WAVEFORMATEX structure             
        /// 'data' = audio data                         
        ///       )                     
        /// </summary>
        public void ParseWaveHeader() {
            bool foundData = false;

            try {
                while (!foundData) {
                    // Go through each chunk and look for the WavFmt which
                    // contains the format information
                    if (Chunk.FCC == FourCC.WavFmt) {
                        this.ReadFormatBlock();
                    }
                    else if (Chunk.FCC == FourCC.WavData || Chunk.FCC == FourCC.Wavdata) {
                        // Found the Wave data.
                        foundData = true;
                        break;
                    }

                    MoveToNextChunk();
                }
            }
            catch (Exception e) {
                if (this.waveFormat == null || !foundData) {
                    throw new InvalidOperationException("Invalid file format", e);
                }
            }

            // Now that we have a chunk with the data from the file,
            // calculate the duration of the file based on the size
            // of the buffer.
            this.duration = this.waveFormat.AudioDurationFromBufferSize(Chunk.Size);
        }

        /// <summary>
        /// Read the format block from the file and construct the WAVEFORMATEX
        /// structure
        /// </summary>
        private void ReadFormatBlock() {
            try {
                Debug.Assert(Chunk.FCC == FourCC.WavFmt, "This is not a WavFmt chunk");
                Debug.Assert(this.waveFormat == null, "The waveformat structure should have been set before");

                // Some .wav files do not include the cbSize field of the WAVEFORMATEX 
                // structure. For uncompressed PCM audio, field is always zero. 
                uint formatSize = 0;
                if (Chunk.Size < MinFormatSize) {
                    throw new InvalidOperationException("File is not a WAV file");
                }

                // Allocate a buffer for the WAVEFORMAT structure.
                formatSize = Chunk.Size;

                this.waveFormat = new WaveFormatEx();

                // Read the format from the current chunk in the file
                byte[] data = ReadDataFromChunk(formatSize);

                // Copy the read data into our WAVFORMATEX
                this.waveFormat.SetFromByteArray(data);
            }
            catch (Exception) {
                this.waveFormat = null;
                throw;
            }
        }
    }
}
