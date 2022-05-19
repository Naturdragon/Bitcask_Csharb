using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bitcask
{
    internal class Bitcask<TKey, TValue> : BitcaskGenericBase<TKey, TValue>
    {
        Dictionary<byte[], MemdirEntry> Memdir = new Dictionary<byte[], MemdirEntry>();
        internal void AddOrUpdate(byte[] key, MemdirEntry entry)
        {
            Memdir.Add(key, entry);
        }


        public override string Author { get; } = "Martin Schindler";    // Author's name and contact info
        public override int Count { get; }                      // Total number of active rows in bitcask
        public override IEnumerable<string> DataFiles { get; }  // Return List of datafiles in chronological order of creation
        public override string Version { get; } = "v0.1";       // Version number major.minor

        public override void Clear()                            // Clear content of files, delete all non-active files (all data is lost)
        {

        }
        public override void Close()                            // Close all files in open bitcask directory 
        {

        }
        public override bool ContainsKey(TKey key)              // Check if key exists ()
        {

        }
        public override void Delete(TKey key)                   // Flag row with given key as deleted (Set Tomb stone)
        {

        }
        public override void DeleteBitcask(string path)         // Delete bitcask directory and all files (all data is lost) (Ordner mit Bitcask Files löschen)
        {

        }
        public override void Open(string path)                  // Open or create a directory holding bitcask file group (Ordner mit Bitcask Files Offnen oder Ertellen)
        {

        }
        public override TValue Read(TKey key)                   // Return row with given key (Look up in Memdir -> Binärfile Lesen und Wert zurück geben)
        {

        }
        public override IEnumerable<(TKey, TValue)> ReadAll()   // Return IEnumerable of (key,value) tuple of all rows (no sort specified)
        {

        }
        public override void Write(TKey key, TValue record)     // Write row with given key (Append)
        {

        }

        public BitcaskGenericBase()
        {

        }

        /// <summary>
        /// Detects the Active File
        /// </summary>
        /// <returns>Returns the Active File</returns>
        internal static string getActiveFile()
        {

        }

        /// <summary>
        /// Writes to an Binary File
        /// </summary>
        /// <typeparam name="T">Accepts Generic values</typeparam>
        /// <param name="path">Path to which to wirte to</param>
        /// <param name="entry">Data to be written</param>
        internal void WiriteToBinaryFile<T>(string path, Entry<T> entry)
        {


            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine();
            }
            long fileid = BitcaskHelper.GetFileID(path);
            Memdir.Add(entry.Key, new MemdirEntry(BitcaskHelper.GetFileID(path), entry.Value_Size, Count + 1));

            

        }

        /// <summary>
        /// Reads one Line of Data from an Binary File
        /// </summary>
        /// <typeparam name="T">Returns an Generic Entry Object</typeparam>
        /// <param name="key">The Key with which to find the file</param>
        /// <returns>Returns an Generic Entry Object</returns>
        internal static Entry<T> ReadDataFromBinaryFile<T>(byte[] key)
        {
            return null;
        }

        /// <summary>
        /// Opens a new File
        /// </summary>
        internal static void OpenFile()
        {

        }


    }
}
