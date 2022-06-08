using BitcaskDraftAPI;
using System.Collections.Concurrent;
using DamienG.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace Bitcask
{
    public class Bitcask<TKey, TValue> : BitcaskGenericBase<TKey, TValue>
    {
        ConcurrentDictionary<byte[], MemdirEntry> Memdir = new ConcurrentDictionary<byte[], MemdirEntry>() { };
        Stream stream;
        internal string PATH_PROP { get; set; }        // Path to the Direcory
        internal string ACTIV_FILE            // Active File
        {
            get
            {
                return updateActivFile();
            }
        }
        internal string FULL_PATH
        {
            get
            {
                return PATH_PROP + @"\" + ACTIV_FILE;
            }
        }

        #region Interface
        public override string Author { get; } = "Martin Schindler\nE-Mail:\tmartin@schindler-it.com";    // Author's name and contact info7
        /// <summary>
        /// Memdir.Count weil das Memdir alle Aktiven Daten Sätze beinhaltet. Also Alle aktiven Zeilen.
        /// </summary>
        public override int Count { get { return Memdir.Count; } }    // Total number of active rows in bitcask
        /// <summary>
        /// Gibt eine Liste von allen Data File Namen in diesem Directory zurück.
        /// </summary>
        public override IEnumerable<string> DataFiles   // Return List of datafiles in chronological order of creation
        {
            get
            {
                DirectoryInfo info = new DirectoryInfo(PATH_PROP);
                FileInfo[] files = info.GetFiles("*.data").OrderBy(p => p.CreationTime).ToArray();
                List<string> data = new List<string>();
                foreach (var item in files)
                {
                    data.Add(item.Name);
                }
                return data;
            }
        }
        public override string Version { get; } = "v0.2";       // Version number major.minor

        public override void Clear()                            // Clear content of files, delete all non-active files (all data is lost)
        {
            stream.Close();
            Memdir.Clear();
            string[] files = Directory.GetFiles(PATH_PROP);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] != ACTIV_FILE)
                {
                    File.Delete(PATH_PROP + @"\" + files[i]);
                }
            }
            File.WriteAllText(FULL_PATH, string.Empty);
        }
        public override void Close()                            // Close all files in open bitcask directory (Schliest streams Activ FIle -> inactiv)
        {
            stream.Close();
            Memdir.Clear();
        }

        /// <summary>
        /// Überprüft ob der SChlüssen forhanden ist. Dazu müste man eigendlich nur schauen ob der Key im DIrektory drinnen ist, aber ich finde es besser wenn man auch überprüft ob der Datensatz auch vorhanden ist.
        /// </summary>
        /// <param name="key">Ist der zu überprüfende Key</param>
        /// <returns>Gibt zurück ob der Key im Directory vorhanden ist.</returns>
        public override bool ContainsKey(TKey key)              // Check if key exists ()
        {
            if (key != null)
            {
                if (Memdir.ContainsKey(tools.ObjectToByteArray(key)) && Read(key) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public override void Delete(TKey key)                   // Flag row with given key as deleted (Set Tomb stone)
        {
            MemdirEntry mementry = Memdir[tools.ObjectToByteArray(key)];
            Memdir.Remove(tools.ObjectToByteArray(key), out mementry);

            Entry<TKey, TValue> entry = new Entry<TKey, TValue>(DateTime.Now, tools.ObjectToByteArray(key).Length, key);
            stream.Write(tools.ObjectToByteArray(entry), mementry.Value_Position, mementry.Value_Size);
        }
        /// <summary>
        /// Das sieht irgend wie nicht nach einer guten idee aus. Kann man da nicht einfach nur das Aktive Directory Löschen?
        /// </summary>
        /// <param name="path">Das zu löschende Directory</param>
        public override void DeleteBitcask(string path)         // Delete bitcask directory and all files (all data is lost) (Ordner mit Bitcask Files löschen)
        {
            stream.Close();
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true); ;
            }
            Memdir.Clear();
        }
        public override void Open(string path)                  // Open or create a directory holding bitcask file group (Ordner mit Bitcask Files Offnen oder Ertellen)
        {

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            PATH_PROP = path;
            FillDirectory();
            stream = BitcaskFileStream.Create(ACTIV_FILE, StorageType.OnDisc);
            OpenFile();
        }
        public override TValue Read(TKey key)                   // Return row with given key (Look up in Memdir -> Binärfile Lesen und Wert zurück geben)
        {
            byte[] data = new byte[Memdir[tools.ObjectToByteArray(key)].Value_Size];

            stream.Read(data, Memdir[tools.ObjectToByteArray(key)].Value_Position, Memdir[tools.ObjectToByteArray(key)].Value_Size);


            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                Entry<TKey, TValue> entry = (Entry<TKey, TValue>)obj;
                Crc32 crc32 = new Crc32();
                if (entry.CRC == BitConverter.ToInt32(crc32.ComputeHash(tools.ObjectToByteArray(entry)))) throw new Exception("The Data is corupted.");
                return entry.value.Value;
            }
        }
        #endregion

        public override IEnumerable<(TKey, TValue)> ReadAll()   // Return IEnumerable of (key,value) tuple of all rows (no sort specified)
        {
            List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>();
            foreach (KeyValuePair<byte[], Bitcask.MemdirEntry> kvp in Memdir)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(kvp.Key))
                {
                    object obj = bf.Deserialize(ms);
                    TKey keyobj = (TKey)obj;

                    list.Add(new KeyValuePair<TKey, TValue>(keyobj, Read(keyobj)));
                }
            }
            return (IEnumerable<(TKey, TValue)>)list;
        }
        public override void Write(TKey key, TValue record)     // Write row with given key (Append)
        {
            //34,359,738,368
            long size = new System.IO.FileInfo(FULL_PATH).Length + tools.ObjectToByteArray(key).Length + tools.ObjectToByteArray(record).Length;
            if (size! > 34_359_738_368)
            {
                OpenFile();
                updateActivFile();
            }

            var lineCount = File.ReadLines(@"C:\file.txt").Count() + 1;

            stream.Write(tools.ObjectToByteArray(record), lineCount, tools.ObjectToByteArray(record).Length);
            tools tool = new tools();
            Memdir.AddOrUpdate(tools.ObjectToByteArray(key), new MemdirEntry(tool.GetFileID(FULL_PATH), tools.ObjectToByteArray(record).Length, lineCount), (key, oldValue) => new MemdirEntry(tool.GetFileID(FULL_PATH), tools.ObjectToByteArray(record).Length, lineCount));
        }

        #region internalClasses

        /// <summary>
        /// Detects the Active File
        /// </summary>
        /// <returns>Returns the Active File</returns>
        internal string updateActivFile()
        {
            return DataFiles.First();
        }

        /// <summary>
        /// Opens a new File
        /// </summary>
        internal void OpenFile()
        {
            string pfad = PATH_PROP;
            string newpath = DateOnly.FromDateTime(DateTime.Now).ToString("dd.MM.yyyy").Replace(".", "") + ".data";
            string[] filelist = Directory.GetFiles(PATH_PROP);
            int flnr = 0;
            bool exists = false;
            do
            {
                exists = false;
                for (int i = 0; i < filelist.Length; i++)
                {
                    if (newpath == filelist[i])
                    {
                        exists = true;
                    }
                }
                if (exists)
                {
                    //Console.WriteLine("Esist: "+newpath);
                    newpath = Regex.Replace(newpath, "\\d", String.Empty) + DateOnly.FromDateTime(DateTime.Now).ToString("dd.MM.yyyy").Replace(".", "") + (flnr + 1);

                    //Console.WriteLine($"Schlaufe: Lenge: {flnr.ToString().Length}\nFNLR: {flnr}");
                }
                flnr++;
            } while (exists);
            newpath = newpath + ".data";
            File.Create(PATH_PROP + @"\" + newpath);
            updateActivFile();
        }

        internal void FillDirectory()
        {
            string[] filePaths = Directory.GetFiles(PATH_PROP, "*.data");
            int position = 0;
            for (int i = 0; i < filePaths.Length; i++)
            {
                string FilePath = PATH_PROP + @"\" + filePaths[i];
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        byte[] data = Encoding.ASCII.GetBytes(line);
                        if (data == null)
                        {
                            throw new Exception("Memdir was unable to be build because the data was corupted.");
                            return;
                        }
                        BinaryFormatter bf = new BinaryFormatter();
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            object obj = bf.Deserialize(ms);
                            Entry<TKey, TValue> entry = (Entry<TKey, TValue>)obj;
                            if (!KeyExists(entry.value.Key))
                            {
                                tools tool = new tools();
                                Memdir.AddOrUpdate(tools.ObjectToByteArray(entry.value.Key), new MemdirEntry(tool.GetFileID(filePaths[i]), entry.value.Value_Size, position), (key, oldValue) => new MemdirEntry(tool.GetFileID(FilePath), entry.value.Value_Size, position));
                            }
                        }
                        position++;
                    }
                }
                position = 0;
            }
        }

        internal bool KeyExists(TKey key)              // Check if key exists ()
        {
            if (key != null)
            {
                if (Memdir.ContainsKey(tools.ObjectToByteArray(key)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}