using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcask
{
    internal class Entry<T>
    {
        int CRC { get; set; }
        DateTime TimeStamp { get; set; }
        int Key_Size { get; set; }
        int Value_Size { get; set; }
        T Key { get; set; }
        T Value { get; set; }

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
