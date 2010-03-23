// WaveMediaStreamSource.cs
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace Translate.Services.Audio {

    /// <summary>
    /// A Media Stream Source implemented to play WAVE files
    /// </summary>
    internal class WaveMediaStreamSource : MediaStreamSource, IDisposable {

        /// <summary>
        /// The stream that we're playing back
        /// </summary>
        private Stream stream;

        /// <summary>
        /// The WavParser that can extract the data
        /// </summary>
        private WavParser wavParser;

        /// <summary>
        /// The stream description
        /// </summary>
        private MediaStreamDescription audioDesc;

        /// <summary>
        /// The current position in the stream.
        /// </summary>
        private long currentPosition;

        /// <summary>
        /// The start position of the data in the stream
        /// </summary>
        private long startPosition;

        /// <summary>
        /// The current timestamp
        /// </summary>
        private long currentTimeStamp;

        /// <summary>
        /// The sample attributes (not used so empty)
        /// </summary>
        private Dictionary<MediaSampleAttributeKeys, string> emptySampleDict = new Dictionary<MediaSampleAttributeKeys, string>();

        /// <summary>
        /// Initializes a new instance of the WaveMediaStreamSource class.
        /// </summary>
        /// <param name="stream">The stream the will contain the data to playback</param>
        public WaveMediaStreamSource(Stream stream) {
            this.stream = stream;
        }

        /// <summary>
        /// Implement the Dispose method to release the resources
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementation of the IDisposable pattern
        /// </summary>
        /// <param name="disposing">Are we being destroyed</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (this.wavParser != null) {
                    this.wavParser.Dispose();
                    this.wavParser = null;
                }
            }
        }

        /// <summary>
        /// Open the media.
        /// Create the structures.
        /// </summary>
        protected override void OpenMediaAsync() {
            // Create a parser
            this.wavParser = new WavParser(this.stream);

            // Parse the header
            this.wavParser.ParseWaveHeader();

            this.wavParser.WaveFormatEx.ValidateWaveFormat();

            this.startPosition = this.currentPosition = this.wavParser.DataPosition;

            // Init
            Dictionary<MediaStreamAttributeKeys, string> streamAttributes = new Dictionary<MediaStreamAttributeKeys, string>();
            Dictionary<MediaSourceAttributesKeys, string> sourceAttributes = new Dictionary<MediaSourceAttributesKeys, string>();
            List<MediaStreamDescription> availableStreams = new List<MediaStreamDescription>();

            // Stream Description
            streamAttributes[MediaStreamAttributeKeys.CodecPrivateData] = this.wavParser.WaveFormatEx.ToHexString();
            MediaStreamDescription msd = new MediaStreamDescription(MediaStreamType.Audio, streamAttributes);

            this.audioDesc = msd;
            availableStreams.Add(this.audioDesc);

            sourceAttributes[MediaSourceAttributesKeys.Duration] = this.wavParser.Duration.ToString();
            ReportOpenMediaCompleted(sourceAttributes, availableStreams);
        }

        /// <summary>
        /// Close the media. Release the resources.
        /// </summary>
        protected override void CloseMedia() {
            // Close the stream
            this.startPosition = this.currentPosition = 0;
            this.wavParser = null;
            this.audioDesc = null;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="diagnosticKind">The diagnostic kind</param>
        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the next sample requested
        /// </summary>
        /// <param name="mediaStreamType">The stream type that we are getting a sample for</param>
        protected override void GetSampleAsync(MediaStreamType mediaStreamType) {
            // Start with one second of data, rounded up to the nearest block.
            uint bufferSize = (uint)AlignUp(
                this.wavParser.WaveFormatEx.AvgBytesPerSec,
                this.wavParser.WaveFormatEx.BlockAlign);

            // Figure out how much data we have left in the chunk compared to the
            // data that we need.
            bufferSize = Math.Min(bufferSize, (uint)this.wavParser.BytesRemainingInChunk);
            if (bufferSize > 0) {
                this.wavParser.ProcessDataFromChunk(bufferSize);

                // Send out the next sample
                MediaStreamSample sample = new MediaStreamSample(
                    this.audioDesc,
                    this.stream,
                    this.currentPosition,
                    bufferSize,
                    this.currentTimeStamp,
                    this.emptySampleDict);

                // Move our timestamp and position forward
                this.currentTimeStamp += this.wavParser.WaveFormatEx.AudioDurationFromBufferSize(bufferSize);
                this.currentPosition += bufferSize;

                /* Uncomment to loop forever
                // If there are no more bytes in the chunk, start again from the beginning
                if (this.wavParser.BytesRemainingInChunk == 0)
                {
                    this.wavParser.MoveToStartOfChunk();
                    this.currentPosition = this.startPosition;
                }
                */

                ReportGetSampleCompleted(sample);
            }
            else {
                // Report EOS
                ReportGetSampleCompleted(new MediaStreamSample(this.audioDesc, null, 0, 0, 0, this.emptySampleDict));
            }
        }

        /// <summary>
        /// Called when asked to seek to a new position
        /// </summary>
        /// <param name="seekToTime">the time to seek to</param>
        protected override void SeekAsync(long seekToTime) {
            if (seekToTime > this.wavParser.Duration) {
                throw new InvalidOperationException("The seek position is beyond the length of the stream");
            }

            this.currentPosition = this.wavParser.WaveFormatEx.BufferSizeFromAudioDuration(seekToTime) + this.startPosition;
            this.currentTimeStamp = seekToTime;
            ReportSeekCompleted(seekToTime);
        }

        /// <summary>
        /// Stream media stream.
        /// Not implemented
        /// </summary>
        /// <param name="mediaStreamDescription">The mediaStreamDescription that we want to switch to</param>
        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Helper function to align a block
        /// </summary>
        /// <param name="a">The value we want to align</param>
        /// <param name="b">The alignment value</param>
        /// <returns>A new aligned value</returns>
        private static int AlignUp(int a, int b) {
            int tmp = a + b - 1;
            return tmp - (tmp % b);
        }
    }
}
