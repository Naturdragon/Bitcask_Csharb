using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcask
{
    internal class Entry<T>
    {
        public int CRC { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Key_Size { get; set; }
        public int Value_Size { get; set; }
        public T Key { get; set; }
        public T Value { get; set; }
        public bool TombStone { get; set; }

        public Entry(int crc, DateTime timestamp, int keysize, int valuesize, T key, T value)
        {
            CRC = crc;
            TimeStamp = timestamp;
            Key_Size = keysize;
            Value_Size = valuesize;
            Key = key;
            Value = value;
        }
    }
}
