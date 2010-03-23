// StreamEventArgs.cs
//

using System;
using System.IO;

namespace Translate {

    public sealed class StreamEventArgs : EventArgs {

        private Stream _stream;

        public StreamEventArgs(Stream s) {
            _stream = s;
        }

        public Stream Stream {
            get {
                return _stream;
            }
        }
    }
}
