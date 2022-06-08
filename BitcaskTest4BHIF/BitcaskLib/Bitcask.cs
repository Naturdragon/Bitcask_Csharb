using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitcaskLib.BaseClasses;

namespace BitcaskLib
{
    /// <summary>
    /// Generic Bitcask Implementation
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Bitcask<TKey, TValue> : BitcaskGenericBase<TKey, TValue>
    {
        public override string Author => "Herbert Feichtinger";

        public override int Count => throw new NotImplementedException();

        public override IEnumerable<string> DataFiles => throw new NotImplementedException();

        public override string Version => "V0.1";

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public override void Delete(TKey key)
        {
            throw new NotImplementedException();
        }

        public override void DeleteBitcask(string path)
        {
            throw new NotImplementedException();
        }

        public override void Open(string path)
        {
            //throw new NotImplementedException();
        }

        public override TValue Read(TKey key)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<(TKey, TValue)> ReadAll()
        {
            throw new NotImplementedException();
        }

        public override void Write(TKey key, TValue record)
        {
            //throw new NotImplementedException();
        }
    }
}
