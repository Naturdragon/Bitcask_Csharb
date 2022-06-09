using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DamienG.Security.Cryptography;


namespace Bitcask
{
    [Serializable]
    internal class Entry<TKey,TValue>
    {

        public long CRC { get; set; }
        public EntryValues<TKey, TValue> value { get; set; }

        public Entry( DateTime _timestamp, int _keysize, int _valuesize, TKey _key, TValue _value)
        {
            EntryValues<TKey,TValue> entry = new EntryValues<TKey, TValue>(_timestamp,_keysize,_valuesize,_key,_value);

            Crc32 crc32 = new Crc32();
            long crc = BitConverter.ToInt32(crc32.ComputeHash(tools.ObjectToByteArray(entry)));
            CRC = crc;
            value = entry;
        }
        public Entry( DateTime timestamp, int keysize,  TKey key)
        {
            EntryValues<TKey,TValue> entry = new EntryValues<TKey,TValue>(timestamp,keysize,key);

            Crc32 crc32 = new Crc32();
            long crc = BitConverter.ToInt32(crc32.ComputeHash(tools.ObjectToByteArray(entry)));
            CRC = crc;
            value= entry;
        }
    }
}
