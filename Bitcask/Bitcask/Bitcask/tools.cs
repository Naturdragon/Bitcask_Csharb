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
            if (obj.GetType().Equals(  typeof (byte[]))) return (byte[])obj;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            // test
            byte[] result= ms.ToArray();
            //MemoryStream ms2 = new MemoryStream(result);

           // object o =bf.Deserialize(ms2);

            return result;

        }

        /*
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
        */
    }
}
