//
// Copyright (c) 2008-2011, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

namespace MRPDiskLib.Wim
{
    using System;
    using System.IO;
    using MRPDiskLib.Compression;

    /// <summary>
    /// Converts a byte stream into a bit stream.
    /// </summary>
    /// <remarks>Note the precise read-ahead behaviour of this stream is critical.
    /// Some data is read directly from the underlying stream when decoding an Xpress
    /// stream - so it's critical the underlying stream position is in the correct
    /// location.</remarks>
    internal class XpressBitStream : BitStream
    {
        private Stream _byteStream;

        private uint _buffer;
        private int _bufferAvailable;

        private byte[] _readBuffer = new byte[2];

        public XpressBitStream(Stream byteStream)
        {
            _byteStream = byteStream;
        }

        public override int MaxReadAhead
        {
            get { return 16; }
        }

        public override uint Read(int count)
        {
            if (count > 16)
            {
                throw new ArgumentOutOfRangeException("count", count, "Maximum 16 bits can be read");
            }

            EnsureBufferFilled();

            _bufferAvailable -= count;

            uint mask = (uint)((1 << count) - 1);

            return (uint)((_buffer >> _bufferAvailable) & mask);
        }

        public override uint Peek(int count)
        {
            EnsureBufferFilled();

            uint mask = (uint)((1 << count) - 1);

            return (uint)((_buffer >> (_bufferAvailable - count)) & mask);
        }

        public override void Consume(int count)
        {
            EnsureBufferFilled();

            _bufferAvailable -= count;
        }

        private void EnsureBufferFilled()
        {
            if (_bufferAvailable < 16)
            {
                _readBuffer[0] = 0;
                _readBuffer[1] = 0;
                _byteStream.Read(_readBuffer, 0, 2);

                _buffer = (uint)((uint)(_buffer << 16) | (uint)(_readBuffer[1] << 8) | (uint)_readBuffer[0]);
                _bufferAvailable += 16;
            }
        }
    }
}
