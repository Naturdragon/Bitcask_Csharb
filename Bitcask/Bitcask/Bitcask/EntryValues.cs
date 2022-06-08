using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcask
{
    internal class EntryValues<TKey, TValue>
    {
        public DateTime TimeStamp { get; set; }
        public int Key_Size { get; set; }
        public int Value_Size { get; set; }
        public TKey Key { get; set; }
        public TValue? Value { get; set; }
        public bool TombStone { get; set; }

        public EntryValues(DateTime timestamp, int keysize, int valuesize, TKey key, TValue value)
        {
            TimeStamp = timestamp;
            Key_Size = keysize;
            Value_Size = valuesize;
            Key = key;
            Value = value;
            TombStone = false;
        }
        public EntryValues(DateTime timestamp, int keysize, TKey key)
        {
            TimeStamp = timestamp;
            Key_Size = keysize;
            Value_Size = 0;
            Key = key;
            TombStone = true;
        }
    }
}
