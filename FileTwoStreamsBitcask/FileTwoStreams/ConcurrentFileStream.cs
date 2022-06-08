using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcaskDraftAPI
{
    /// <summary>
    /// A FileStream for concurrent read and write to the active Bitcask datafile, thread-safe.
    /// </summary>
    /// <remarks>
    ///     Open all other files like this:
    ///    _anotherReadStream = new FileStream(
    ///            pathNonActiveFile,              // path to non-active file (directory + filename + extension)
    ///            FileMode.Open,                  // open existing file
    ///            FileAccess.Read,                // readonly access
    ///            FileShare.Read,                 // allow subsequent opening for reading
    ///            4096,                           // recommended buffer size
    ///            FileOptions.RandomAccess | FileOptions.Asynchronous);      
    ///                                            // optimize cache for random access
    ///
    ///     
    /// </remarks>
    public class ConcurrentFileStream : Stream, IDisposable
    {
        private readonly FileStream _readStream;
        private readonly FileStream _writeStream;
        private long _writePosition; // position of next Write() = append = cuurent length

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _readStream.Length; //  _writePosition hat nicht funktioniert!

        /// <summary>
        /// Gets and sets the position for Read() operations. Write() is always append.
        /// </summary>
        public override long Position
        {
            get { return _readStream.Position; }  // position of next Read()
            set { _readStream.Position = value; } // position of next Read()
        }

        public ConcurrentFileStream(string pathActiveFile)
        {
            _writeStream = new FileStream(
                pathActiveFile,                 // path to active file (directory + filename + extension)
                FileMode.Append,                // append or create a new file
                FileAccess.Write,               // write access to active file
                FileShare.Read,                 // allow subsequent opening for reading
                4096,                           // recommended buffer size
                FileOptions.SequentialScan | FileOptions.WriteThrough);
                //FileOptions.SequentialScan | FileOptions.WriteThrough | FileOptions.Asynchronous);
                                                // optimize cache for random access

            _writePosition = _writeStream.Length; // position of next Write() = append = cuurent length

            _readStream = new FileStream(
                pathActiveFile,                 // path to active file (directory + filename + extension)
                FileMode.Open,                  // open existing file
                FileAccess.Read,                // readonly access
                FileShare.ReadWrite,            // allow subsequent opening for reading and writing (read only doesn't work)
                4096,                           // recommended buffer size
                FileOptions.RandomAccess);
                //FileOptions.RandomAccess | FileOptions.Asynchronous);      
                                                // optimize cache for random access
        }

        public override void Flush()
        {
            _writeStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //Console.WriteLine($"Length writeStream: {_writeStream.Length}, readStream: {_readStream.Length}");
            //Console.WriteLine($"Position writeStream: {_writeStream.Position}, position readStream: {_readStream.Position}");
            return _readStream.Read(buffer, offset, count);

        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _writeStream.Write(buffer, offset, count);
            //Console.WriteLine($"Length writestream: {_writeStream.Length}, readstream: {_readStream.Length}");
        }

        public new void Dispose()
        {
            _writeStream.Dispose();
            _readStream.Dispose();
        }
    }
}
