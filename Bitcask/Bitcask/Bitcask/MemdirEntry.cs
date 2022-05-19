﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcask
{
    internal class MemdirEntry
    {
        /// <summary>
        /// FileID: System ID From the File the Entry is saved in.
        /// Value_Size: The Size of the stored Value
        /// Value_Position: The Position of the Value in the Binary File
        /// TimeStamp: TimeStamp
        /// </summary>
        public long FileID { get; set; }
        public int Value_Size { get; set; }
        public int Value_Position { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        public MemdirEntry(int fieldid, int value_size, int value_position)
        {
            FileID = fieldid;
            Value_Size = value_size;
            Value_Position = value_position;
        }
    }
}