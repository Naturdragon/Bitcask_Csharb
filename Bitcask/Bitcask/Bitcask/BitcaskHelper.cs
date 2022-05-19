using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bitcask
{
    internal class BitcaskHelper
    {
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
        /// <remarks>The Adding to the Memdir is after the Wirte, because if an error schoud acoure, the Data is "Lost" in the sence that it is written to the files, but it does not exist in the MemDir. There for the Data was "Lost".
        /// The otherway around we could have an Entry in the Memdir but the data does not exist.</remarks>
        internal static void WiriteToBinaryFile<T>(string path, Entry<T> entry)
        {
            

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine();
            }
            AddOrUpdate(entry.Key,new MemdirEntry(Get, entry.Value_Size, C))
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
