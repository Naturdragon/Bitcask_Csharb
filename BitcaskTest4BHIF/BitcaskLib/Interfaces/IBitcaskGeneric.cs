using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcaskLib.Interfaces
{
    /// <summary>
    /// Bitcask key/value append only library Generic API
    /// </summary>
    /// <remarks>
    /// modeled after Basho's original bitcask white paper https://riak.com/assets/bitcask-intro.pdf (2010)
    /// V1.1, 16-May-2022
    /// Owner: FEIH - please contact for any changes
    /// 
    /// Vorteil der generischen Variante: kann auch Memory<T> und Span<T>
    /// 
    /// geplante Erweiterungen: 
    ///     async/await
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
        IEnumerable<(TKey,TValue)> ReadAll(); // Return IEnumerable of (key,value) tuple of all rows (no sort specified)
        void Write(TKey key, TValue record);  // Write row with given key
        #endregion
    }
}
