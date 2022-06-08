using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Bitcask
{
    class tools
    {
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null) return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        /*
        public static MemdirEntry FromByteArray(byte[] data)
        {
            if (data == null)
                throw new Exception("Memdir was unable to be build because the data was corupted.");
                return;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                Entry<TKey,TValue> entry = (Entry<TKey, TValue>)obj;
                return new MemdirEntry()
            }
        }*/


        #region Get_File_ID
        public struct BY_HANDLE_FILE_INFORMATION
        {
            public uint FileAttributes;
            public uint VolumeSerialNumber;
            public uint FileSizeHigh;
            public uint FileSizeLow;
            public uint NumberOfLinks;
            public uint FileIndexHigh;
            public uint FileIndexLow;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetFileInformationByHandle(IntPtr hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        public ulong GetFileID(string path)
        {
            BY_HANDLE_FILE_INFORMATION objectFileInfo = new BY_HANDLE_FILE_INFORMATION();

            FileInfo fi = new FileInfo(path);
            FileStream fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            GetFileInformationByHandle(fs.Handle, out objectFileInfo);

            fs.Close();

            ulong fileIndex = ((ulong)objectFileInfo.FileIndexHigh << 32) + (ulong)objectFileInfo.FileIndexLow;

            return fileIndex;
        }
        #endregion
    }
}
