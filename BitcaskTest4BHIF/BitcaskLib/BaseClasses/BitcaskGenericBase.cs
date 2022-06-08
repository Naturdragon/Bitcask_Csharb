using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitcaskLib.Interfaces;

namespace BitcaskLib.BaseClasses
{
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
