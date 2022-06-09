using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcaskDraftAPI
{
    /// <summary>
    /// A MemoryStream for simultaneus read/write (internal locks).
    /// </summary>
    public class ConcurrentMemoryStream : Stream
    {
        private readonly MemoryStream innerStream;
        private long readPosition;
        private long writePosition;

        public ConcurrentMemoryStream()
        {
            innerStream = new MemoryStream();
        }

        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

        public override void Flush()
        {
            lock (innerStream)
            {
                innerStream.Flush();
            }
        }

        public override long Length
        {
            get
            {
                lock (innerStream)
                {
                    return innerStream.Length;
                }
            }
        }

        public override long Position
        {
            get 
            {
                lock (innerStream)
                {
                    return readPosition;
                }
            }
            set 
            {
                lock (innerStream)
                {
                    if (value >= Length) throw new EndOfStreamException("readPosition >= Length");
                    readPosition = value;
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
                // HMF 17-May-2022
                //if (buffer == null) throw new ArgumentNullException("buffer is null");
                //if ((offset < 0) || (count < 0)) throw new ArgumentOutOfRangeException("offset or count is less than  0");
                //if (Length - offset < count) throw new EndOfStreamException("Length-offset < count");
                //if (readPosition >= Length) throw new EndOfStreamException("readPosition >= Length");

                lock (innerStream)
                {
                    innerStream.Position = readPosition;
                    int red = innerStream.Read(buffer, offset, count);
                    readPosition = innerStream.Position;

                    return red;
                }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (innerStream)
            {
                innerStream.Position = writePosition;
                innerStream.Write(buffer, offset, count);
                writePosition = innerStream.Position;
            }
        }
    }
}
