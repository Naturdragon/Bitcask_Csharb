using Bitcask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProgram
{
    internal class Tests
    {
        public static void testingPropsNoPath()
        {

            Bitcask<byte[], byte[]> bitcask = new Bitcask<byte[], byte[]>();
            Console.WriteLine($"Author: {bitcask.Author}");
            Console.WriteLine($"Count: {bitcask.Count}");
            Console.WriteLine($"DataFiles: ");
            try
            {
                foreach (var item in bitcask.DataFiles)
                {
                    Console.WriteLine(item);
                }
            }
            catch
            {
                Console.WriteLine("Der Pfad wurde nicht gesetzt.\n");
            }
            Console.WriteLine($"Version: {bitcask.Version}");
        }

        public static void testingPropsWithPath()
        {
            Bitcask<byte[], byte[]> bitcask = new Bitcask<byte[], byte[]>();
            Console.WriteLine($"Author: {bitcask.Author}");
            Console.WriteLine($"Count: {bitcask.Count}");
            bitcask.Open(@"..\..\..\BitcaskData");
            Console.WriteLine($"DataFiles: ");
            try
            {
                foreach (var item in bitcask.DataFiles)
                {
                    Console.WriteLine(item);
                }
            }
            catch
            {
                Console.WriteLine("Der Pfad wurde nicht gesetzt.\n");
            }
            Console.WriteLine($"Version: {bitcask.Version}");
            bitcask.DeleteBitcask(@"..\..\..\BitcaskData");
        }

        /// <summary>
        /// Write and Read wird hier getestet.
        /// </summary>
        public static void testingBasicBitcask()
        {
            Bitcask<byte[], byte[]> bitcask = new Bitcask<byte[], byte[]>();
            bitcask.Open(@"..\..\..\BitcaskData");
            //bitcask.Write(new byte[] { 0,1,0,0,1,0,1,1, 0,1,1,0,0,1,0,1, 0,1,1,1,1,0,0,1 }, new byte[] { 0,1,0,1,0,1,1,0, 0,1,1,0,0,0,0,1, 0,1,1,0,1,1,0,0, 0,1,1,1,0,1,0,1, 0,1,1,0,0,1,0,1 });
            //Console.WriteLine("Geschrieben: 01001011 01100101 01111001 and 01010110 01100001 01101100 01110101 01100101");
            //Console.WriteLine(bitcask.Read(new byte[] { 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 1, 1, 1, 0, 0, 1 }));

            bitcask.Write(new byte[] { 0, 1, 0}, new byte[] { 1, 0, 1 });
            Console.WriteLine("Geschrieben: 01001011 01100101 01111001 and 01010110 01100001 01101100 01110101 01100101");
            byte[] result = bitcask.Read(new byte[] { 0, 1, 0 });
            Console.WriteLine($"Byte Array is: {string.Join(" ", result)}");

        }
    }
}
