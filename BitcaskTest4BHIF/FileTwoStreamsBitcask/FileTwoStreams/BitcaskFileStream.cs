using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcaskDraftAPI
{
    public enum StorageType {InMemory, OnDisc} // ... InCloud

    /// <summary>
    /// Stream for accessing the existing active bitcask file on-disc or creating an in-memory file.
    /// </summary>
    /// <remarks>
    /// Usage:
    ///     var stream = BitcaskFileStream.Create("my_active_datafile", StorageType.OnDisc);  
    ///     var stream = BitcaskFileStream.Create("my_active_datafile", StorageType.InMemory); 
    ///     
    ///     Reading:
    ///         var length = bfs.Length; // end of stream
    ///         bfs.Position = ...; // compute position of record to read
    ///         bfs.Read(buffer, 0, buffer.Length); // read record, this moves bfs.Position past the read record
    ///     Writing (Append):
    ///         bfs.Write(buffer, 0, buffer.Length); // append record
    ///
    ///     This handles only the active bitcask file (open all others using a readonly filestream).
    ///     
    ///     Thread-safety:
    ///         As long as there is only one writer thread in the calling application code or only one reader thread,
    ///         this BitcaskFileStream is thread-safe.
    ///     
    ///     Note: 
    ///         RecordIDs, CRCs and other meta-data for a record must be handled in the calling code.
    /// </remarks>
    public class BitcaskFileStream
    {
        private BitcaskFileStream()
        {}

        /// <summary>
        /// Create the stream for managing the existing bitcask data store's active file, thread-safe.
        /// </summary>
        /// <returns></returns>
        public static System.IO.Stream Create(string path, StorageType storage)
        {
            System.IO.Stream stream = null;

            // Create active file on-disk or in-memory
            if (storage == StorageType.InMemory)
            {
                stream = new ConcurrentMemoryStream();
            }
            else if (storage == StorageType.OnDisc)
            {
              stream = new ConcurrentFileStream(path);
            }
            else
                throw new ArgumentException($"Invalid Storage type parameter: {storage}");

            return stream;
        }
    }
}
