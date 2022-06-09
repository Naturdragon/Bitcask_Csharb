using BitcaskDraftAPI;
using System.Collections.Concurrent;
using DamienG.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace Bitcask
{
    public class Bitcask<TKey, TValue> : BitcaskGenericBase<TKey, TValue>
    {
        ConcurrentDictionary<byte[], MemdirEntry> Memdir = new ConcurrentDictionary<byte[], MemdirEntry>(new MyEqualityComparer()) { };
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
                try
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
                catch (Exception ex)
                {
                    throw new Exception("Path not set.");
                }
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
            OpenFile();
            FillDirectory();
            stream = BitcaskFileStream.Create(ACTIV_FILE, StorageType.OnDisc);
        }
        public override TValue Read(TKey key)                   // Return row with given key (Look up in Memdir -> Binärfile Lesen und Wert zurück geben)
        {
            try
            {
                byte[] test = tools.ObjectToByteArray(key);
                MemdirEntry mementry = Memdir[test];

                byte[] data = new byte[mementry.Value_Size];

                //TODO: Reat richtig setzen
                //Memdir[tools.ObjectToByteArray(key)].FileID;

                stream.Read(data, Memdir[test].Value_Position, Memdir[test].Value_Size);
                stream.Flush();

                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(data))
                {
                    object obj = bf.Deserialize(ms);
                    Entry<TKey, TValue> entry = (Entry<TKey, TValue>)obj;
                    //Entry<TKey, TValue> entry = Deserialize<Entry<TKey,TValue>>(data);

                    Crc32 crc32 = new Crc32();
                    if (entry.CRC == BitConverter.ToInt32(crc32.ComputeHash(tools.ObjectToByteArray(entry)))) throw new Exception("The Data is corupted.");
                    return entry.value.Value;
                }
            }catch (Exception ex)
            {
                throw new Exception($"Error: Unable to read.\n{ex}");
            }
        }
        #endregion

        private T Deserialize<T>(byte[] param)
        {
            using (MemoryStream ms = new MemoryStream(param))
            {
                IFormatter br = new BinaryFormatter();
                return (T)br.Deserialize(ms);
            }
        }


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
            Entry<TKey, TValue> entry = new Entry<TKey, TValue>(DateTime.Now, tools.ObjectToByteArray(key).Length, tools.ObjectToByteArray(record).Length, key, record);
            byte[] bytearr = tools.ObjectToByteArray(entry);
            //34,359,738,368
            long size = new System.IO.FileInfo(FULL_PATH).Length + bytearr.Length;
            if (size! > 34_359_738_368)
            {
                OpenFile();
                updateActivFile();
            }

            var lineCount = File.ReadLines(FULL_PATH).Count();   // Kann später zu Problemen Führen                                                                                                                                  
            byte[] test = tools.ObjectToByteArray(key);



            stream.Write(bytearr, lineCount, bytearr.Length);
            stream.Flush();
            //byte[] b = new byte[bytearr.Length];
            //stream.Read(b, 0, bytearr.Length);

            MemdirEntry mementry = new MemdirEntry(ACTIV_FILE.Split(".")[0], bytearr.Length, lineCount);
            Memdir.AddOrUpdate(tools.ObjectToByteArray(key), mementry, (key, oldValue) => mementry);
        }

        #region internalClasses

        /// <summary>
        /// Detects the Active File
        /// </summary>
        /// <returns>Returns the Active File</returns>
        internal string updateActivFile()
        {
            return DataFiles.FirstOrDefault();
        }

        /// <summary>
        /// Opens a new File
        /// </summary>
        internal void OpenFile()
        {
            string pfad = PATH_PROP;
            //string newpath = DateOnly.FromDateTime(DateTime.Now).ToString("dd.MM.yyyy").Replace(".", "") + ".data";
            string newpath = Guid.NewGuid().ToString("N") + ".data";
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
            //newpath = newpath + ".data";
            File.Create(PATH_PROP + @"\" + newpath).Close();
            updateActivFile();
        }

        internal void FillDirectory()
        {
            string[] filePaths = Directory.GetFiles(PATH_PROP, "*.data");
            int position = 0;
            for (int i = 0; i < filePaths.Length; i++)
            {
                string FilePath = filePaths[i];
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
                                Memdir.AddOrUpdate(tools.ObjectToByteArray(entry.value.Key), new MemdirEntry(FilePath.Split(".")[0], tools.ObjectToByteArray( entry).Length, position), (key, oldValue) => new MemdirEntry(FilePath.Split(".")[0], tools.ObjectToByteArray(entry).Length, position));
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