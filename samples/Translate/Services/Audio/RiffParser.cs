// RiffParser.cs
//

using System;
using System.Diagnostics;
using System.IO;

namespace Translate.Services.Audio {

    /// <summary>
    /// The different FourCC codes we know of
    /// </summary>
    internal enum FourCC {

        /// <summary>
        /// FCC.FourCC('W', 'A', 'V', 'E')
        /// </summary>
        Wave = 0x45564157,

        /// <summary>
        /// FCC.FourCC('f', 'm', 't', ' ')
        /// </summary>
        WavFmt = 0x20746d66,

        /// <summary>
        /// FCC.FourCC('D', 'A', 'T', 'A')
        /// </summary>
        WavData = 0x41544144,

        /// <summary>
        /// FCC.FourCC('d', 'a', 't', 'a') 
        /// </summary>
        Wavdata = 0x61746164,

        /// <summary>
        /// FCC.FourCC('R', 'I', 'F', 'F')
        /// </summary>
        Riff = 0x46464952,

        /// <summary>
        /// FCC.FourCC('L', 'I', 'S', 'T')
        /// </summary>
        List = 0x5453494c,

        /// <summary>
        /// FCC.FourCC('A', 'V', 'I', ' ')
        /// </summary>
        Avi = 0x20495641,
    }

    /// <summary>
    /// The structure of a RiffChunk
    /// </summary>
    internal struct RiffChunk {

        /// <summary>
        /// Gets or sets the FourCC code
        /// </summary>
        public FourCC FCC {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data size of this chunk
        /// </summary>
        public uint Size {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the FourCC list
        /// </summary>
        public FourCC FCCList {
            get;
            set;
        }

        /// <summary>
        /// Is this a list chunk
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsList() {
            return this.FCC == FourCC.List;
        }
    }

    /// <summary>
    /// Riff Parser. Parses a Riff style file.
    /// </summary>
    internal class RiffParser : IDisposable {

        /// <summary>
        /// The minimum size of a Riff List
        /// </summary>
        private const uint SizeOfRiffList = 12;

        /// <summary>
        /// The minimum size of a Riff Chunk
        /// </summary>
        private const uint SizeOfRiffChunk = 8;

        /// <summary>
        /// The current chunk
        /// </summary>
        private RiffChunk chunk;

        /// <summary>
        /// The current FourCC code
        /// </summary>
        private FourCC fccId;

        /// <summary>
        /// The current FourCC type
        /// </summary>
        private FourCC fccType;

        /// <summary>
        /// The stream that we are parsing
        /// </summary>
        private Stream stream;

        /// <summary>
        /// The binary reader for the stream
        /// </summary>
        private BinaryReader br;

        /// <summary>
        /// The number of bytes remaining in the stream
        /// </summary>
        private uint bytesRemaining;

        /// <summary>
        /// The size of the current container in bytes
        /// </summary>
        private uint containerSize;

        /// <summary>
        /// The offset of the current container in the stream
        /// </summary>
        private long containerOffset;

        /// <summary>
        /// The offset of the current chunk in the container
        /// </summary>
        private long currentChunkOffset;

        /// <summary>
        /// Initializes a new instance of the RiffParser class.
        /// </summary>
        /// <param name="stream">The stream that we will parse</param>
        /// <param name="id">The primary Riff ID that we need</param>
        /// <param name="startOfContainer">The start offset of the container in the stream</param>
        public RiffParser(Stream stream, FourCC id, long startOfContainer) {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }

            this.fccId = id;
            this.containerOffset = startOfContainer;

            this.stream = stream;
            this.br = new BinaryReader(stream);

            this.ReadRiffHeader();
        }

        /// <summary>
        /// Gets the RiffId
        /// </summary>
        public FourCC RiffId {
            get {
                return this.fccId;
            }
        }

        /// <summary>
        /// Gets the RiffType
        /// </summary>
        public FourCC RiffType {
            get {
                return this.fccType;
            }
        }

        /// <summary>
        /// Gets the current chunk
        /// </summary>
        public RiffChunk Chunk {
            get {
                return this.chunk;
            }
        }

        /// <summary>
        /// Gets the chunk'stream data position in the stream
        /// </summary>
        public long DataPosition {
            get {
                return this.currentChunkOffset + SizeOfRiffChunk;
            }
        }

        /// <summary>
        /// Gets the number of bytes left in the chunk
        /// </summary>
        public uint BytesRemainingInChunk {
            get {
                return this.bytesRemaining;
            }
        }

        /// <summary>
        /// Gets the actual size of a chunk (data + header)
        /// </summary>
        private long ChunkActualSize {
            get {
                return SizeOfRiffChunk + RiffRound(this.chunk.Size);
            }
        }

        /// <summary>
        /// Is the start index aligned
        /// </summary>
        /// <param name="startIndex">the start index</param>
        /// <param name="align">The alignment value</param>
        /// <returns>true or false</returns>
        public static bool IsAligned(int startIndex, int align) {
            return (startIndex % align) == 0;
        }

        /// <summary>
        /// Is the start index aligned
        /// </summary>
        /// <param name="startIndex">the start index</param>
        /// <param name="align">The alignment value</param>
        /// <returns>true or false</returns>
        public static bool IsAligned(long startIndex, int align) {
            return (startIndex % align) == 0;
        }

        /// <summary>
        /// Round a count for Riff chunks
        /// </summary>
        /// <param name="count">The count value that we want to round</param>
        /// <returns>the rounded count</returns>
        public static int RiffRound(int count) {
            return count + (count & 1);
        }

        /// <summary>
        /// Round a count for Riff chunks
        /// </summary>
        /// <param name="count">The count value that we want to round</param>
        /// <returns>the rounded count</returns>
        public static long RiffRound(long count) {
            return count + (count & 1);
        }

        /// <summary>
        /// Format a FourCC code
        /// </summary>
        /// <param name="fcc">The FourCC code</param>
        /// <returns>A string for the FourCC code</returns>
        public static string FormatFourCC(FourCC fcc) {
            uint code = (uint)fcc;
            return string.Format(
                "{0}{1}{2}{3}",
                (char)(code & 0xFF),
                (char)((code >> 8) & 0xFF),
                (char)((code >> 16) & 0xFF),
                (char)((code >> 24) & 0xFF));
        }

        /// <summary>
        /// Advance to the start of the next chunk and read the chunk header
        /// </summary>
        public void MoveToNextChunk() {
            // chunk offset is always bigger than container offset,
            // and both are always non-negative.
            Debug.Assert(this.currentChunkOffset > this.containerOffset, "The chunk cannot be out of bounds of the container");
            Debug.Assert(this.currentChunkOffset >= 0, "The chunk offset must be positive");
            Debug.Assert(this.containerOffset >= 0, "The container offset must be positive");

            long maxChunkSize;

            // Update current chunk offset to the start of the next chunk
            this.currentChunkOffset += this.ChunkActualSize;

            // Are we at the end?
            if (this.currentChunkOffset - this.containerOffset >= this.containerSize) {
                throw new InvalidOperationException("We are at the end of the steam");
            }

            // Seek to the start of the chunk.
            this.stream.Position = this.currentChunkOffset;

            // Read the header.
            this.ReadChunkHeader();

            // This chunk cannot be any larger than (container size - (chunk offset - container offset) )
            maxChunkSize = (long)this.containerSize - (this.currentChunkOffset - this.containerOffset);

            if (maxChunkSize < this.ChunkActualSize) {
                throw new InvalidOperationException("The current chunk is too big");
            }

            this.bytesRemaining = this.chunk.Size;
        }

        /// <summary>
        /// Return a parser for a LIST
        /// </summary>
        /// <returns>A new parser for the list</returns>
        public RiffParser EnumerateChunksInList() {
            if (!this.chunk.IsList()) {
                throw new InvalidOperationException("The current chunk is not a list");
            }

            return new RiffParser(this.stream, FourCC.List, this.currentChunkOffset);
        }

        /// <summary>
        /// Print the chunk information of the current chunk and descendants
        /// </summary>
        /// <param name="indent">The indent level for this chunk</param>
        public void PrintChunkInformation(int indent) {
            try {
                while (true) {
                    for (int i = 0; i < indent; i++) {
                        Console.Write("\t");
                    }

                    Console.WriteLine("{0} ({1} bytes) {2}", FormatFourCC(this.fccId), this.Chunk.Size, FormatFourCC(this.fccType));
                    if (this.chunk.IsList()) {
                        RiffParser listParser = this.EnumerateChunksInList();
                        listParser.PrintChunkInformation(indent + 1);
                    }

                    this.MoveToNextChunk();
                }
            }
            catch (InvalidOperationException) {
            }
        }

        /// <summary>
        /// Move the file pointer to the start of the current chunk
        /// </summary>
        public void MoveToStartOfChunk() {
            this.MoveToChunkOffset(0);
        }

        /// <summary>
        /// Move the file pointer to a byte offset from the start of the
        /// current chunk.
        /// </summary>
        /// <param name="offset">The offset we want to move to</param>
        public void MoveToChunkOffset(uint offset) {
            if (offset > this.chunk.Size) {
                throw new ArgumentException("Offset specified is beyond the chunk");
            }

            this.stream.Position = this.currentChunkOffset + offset + SizeOfRiffChunk;
            this.bytesRemaining = this.chunk.Size - offset;
        }

        /// <summary>
        /// Read data from the current chunk. (Starts at the current file ptr.) 
        /// </summary>
        /// <param name="count">The number of bytes we want to read</param>
        /// <returns>The data read</returns>
        public byte[] ReadDataFromChunk(uint count) {
            if (count > this.bytesRemaining) {
                throw new ArgumentException("Trying to read more than the size of the chunk");
            }

            this.stream.Position = this.currentChunkOffset + this.chunk.Size - this.bytesRemaining + RiffParser.SizeOfRiffChunk;

            byte[] data = this.br.ReadBytes((int)count);
            this.bytesRemaining -= (uint)data.Length;

            return data;
        }

        /// <summary>
        /// Process data from a chunk (just like reading without actually getting the data)
        /// </summary>
        /// <param name="count">The number of bytes we want to skip</param>
        /// <returns>The number of bytes we want to skipped</returns>
        public uint ProcessDataFromChunk(uint count) {
            if (count > this.bytesRemaining) {
                throw new ArgumentException("Trying to process more than the size of the chunk");
            }

            this.bytesRemaining -= count;

            return count;
        }

        #region IDisposable Members
        /// <summary>
        /// Implement the Dispose method to release the resources
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Implementation of the IDisposable pattern
        /// </summary>
        /// <param name="disposing">Are we being destroyed</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (this.br != null) {
                    this.br.Close();
                    this.br = null;
                }
            }
        }

        /// <summary>
        /// Read the container header section. (The 'RIFF' or 'LIST' header.)
        /// This method verifies the header is well-formed and caches the
        /// container'stream FOURCC type.
        /// </summary>
        private void ReadRiffHeader() {
            RiffChunk header = new RiffChunk();

            // Riff chunks must be WORD aligned
            if (!IsAligned(this.containerOffset, 2)) {
                throw new InvalidOperationException("The chunk is not aligned");
            }

            // Offset must be positive.
            if (this.containerOffset < 0) {
                throw new InvalidOperationException("The container offset needs to be positive");
            }

            // Seek to the start of the container.
            this.stream.Position = this.containerOffset;

            // Read the header
            header.FCC = (FourCC)this.br.ReadUInt32();
            header.Size = this.br.ReadUInt32();
            header.FCCList = (FourCC)this.br.ReadUInt32();

            // Make sure the header ID matches what the caller expected.
            if (header.FCC != this.fccId) {
                throw new InvalidOperationException("We don't have the right FourCC code");
            }

            // The size given in the RIFF header does not include the 8-byte header.
            // However, our _containerOffset is the offset from the start of the
            // header. Therefore our container size = listed size + size of header.
            this.containerSize = header.Size + SizeOfRiffChunk;
            this.fccType = header.FCCList;

            this.currentChunkOffset = this.containerOffset + SizeOfRiffList;

            this.ReadChunkHeader();
        }

        private void ReadChunkHeader() {
            this.chunk.FCC = (FourCC)this.br.ReadUInt32();
            this.chunk.Size = this.br.ReadUInt32();
            this.bytesRemaining = this.chunk.Size;
        }
    }
}
