using Bitcask.BitcaskDraftAPI.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcask
{
    /*
    internal interface BitcaskInterface
    {
        /// <summary>
        /// Bitcask key/value append only library API
        /// </summary>
        /// <remarks>
        /// modeled after Basho's original bitcask white paper https://riak.com/assets/bitcask-intro.pdf (2010)
        /// V1.1, 16-May-2022
        /// Owner: FEIH - please contact for any changes
        /// </remarks>
        public interface IBitcask : IEnumerable
        {
            #region Properties
            public string Author { get; }          // Author's name and contact info
            int Count { get; }                     // Total number of active rows in bitcask
            IEnumerable<string> DataFiles { get; }// Return List of datafiles in chronological order of creation
            string Version { get; }                // Version number major.minor
            #endregion

            #region Methods
            void Clear();                         // Clear content of files, delete all non-active files (all data is lost)
            void Close();                         // Close all files in open bitcask directory
            bool ContainsKey(byte[] key);         // Check if key exists
            void Delete(byte[] key);              // Flag row with given key as deleted
            void DeleteBitcask(string path);      // Delete bitcask directory and all files (all data is lost)
            void Open(string path);               // Open or create a directory holding bitcask file group
            byte[] Read(byte[] key);              // Return row with given key
            IEnumerable<(byte[], byte[])> ReadAll(); // Return IEnumerable of (key, value) tuples of all rows (no sort specified)
            void Write(byte[] key, byte[] record);// Write row with given key
            #endregion
        }
    }
*/

        
    //Generic:


    namespace BitcaskDraftAPI.Interfaces
    {
        /// <summary>
        /// Bitcask key/value append only library Generic API
        /// </summary>
        /// <remarks>
        /// modeled after Basho's original bitcask white paper https://riak.com/assets/bitcask-intro.pdf (2010)
        /// V1.1, 16-May-2022
        /// Owner: FEIH - please contact for any changes
        /// </remarks>
        public interface IBitcaskGeneric<TKey, TValue>
        {
            #region Properties
            public string Author { get; }          // Author's name and contact info
            int Count { get; }                     // Total number of active rows in bitcask
            IEnumerable<string> DataFiles { get; } // Return List of datafiles in chronological order of creation
            string Version { get; }                // Version number major.minor
            #endregion

            #region Methods
            void Clear();                         // Clear content of files, delete all non-active files (all data is lost)
            void Close();                         // Close all files in open bitcask directory
            bool ContainsKey(TKey key);           // Check if key exists
            void Delete(TKey key);                // Flag row with given key as deleted
            void DeleteBitcask(string path);      // Delete bitcask directory and all files (all data is lost)
            void Open(string path);               // Open or create a directory holding bitcask file group
            TValue Read(TKey key);                // Return row with given key
            IEnumerable<(TKey, TValue)> ReadAll(); // Return IEnumerable of (key,value) tuple of all rows (no sort specified)
            void Write(TKey key, TValue record);  // Write row with given key
            #endregion
        }
    }

    /// <summary>
    /// Bitcask append-only file class. This is an abstract class.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class BitcaskGenericBase<TKey, TValue> : IBitcaskGeneric<TKey, TValue>
    {
        public abstract string Author { get; }
        public abstract int Count { get; }
        public abstract IEnumerable<string> DataFiles { get; }
        public abstract string Version { get; }

        public abstract void Clear();
        public abstract void Close();
        public abstract bool ContainsKey(TKey key);
        public abstract void Delete(TKey key);
        public abstract void DeleteBitcask(string path);
        public abstract void Open(string path);
        public abstract TValue Read(TKey key);
        public abstract IEnumerable<(TKey, TValue)> ReadAll();
        public abstract void Write(TKey key, TValue record);

        public BitcaskGenericBase()
        {

        }
    }
}