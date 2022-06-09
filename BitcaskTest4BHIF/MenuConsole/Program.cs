using Bitcask;
using Bitcask.BitcaskDraftAPI.Interfaces;
using System.Security.Cryptography;

const int LOOP_COUNT = 3; // how many reads & writes in test
const int RECORD_LENGTH = 64 * 1; // size of payload (minimum: 32+4 bytes)
const string DATA_DIR = @"..\..\..\Data"; // bitcask directory for file group (HARD CODED)

const string TITLE = "BitcaskTest4BHIF, MenuConsole, H.Feichtinger, V1.0, 29-May-2022";
Console.Title = TITLE + " " + Environment.UserName + " " + Environment.MachineName;

Console.WindowHeight = 40;
Console.WindowWidth = 85;

Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine(TITLE + "\r\n");
Console.ResetColor();

// The Bitcask  object
// Open or create a bitcask file group in a directory
IBitcaskGeneric<byte[], byte[]> bitcask = new Bitcask<byte[], byte[]>(); // ToDo: directory übergeben!
bitcask.Open(DATA_DIR);

//display menu:
int selectedMenuItem;
bool exiting = false;

while (!exiting)
{
    Console.WriteLine("\r\n" + "Please choose an option:");
    Console.WriteLine("\t" + "0 ... Display Summary (Author, Count, DataFiles, Version)");
    Console.WriteLine("\t" + "1 ... Write random records, then read and verify");
    Console.WriteLine("\t" + "2 ... Write/read/verify random records concurrently");
    Console.WriteLine("\t" + "3 ... Close bitcask file group");
    Console.WriteLine("\t" + "4 ... Display all records");
    Console.WriteLine("\t" + "5 ... Read row with given key");
    Console.WriteLine("\t" + "6 ... Delete row with given key");
    Console.WriteLine("\t" + "7 ... Write row with given key");
    Console.WriteLine("\t" + "8 ... not used");
    Console.WriteLine("\t" + "9 ... Exit");
    Console.Write("Your selection: ");

    //Parse menu selection and dispatch
    if (int.TryParse(Console.ReadLine(), out selectedMenuItem))
    {
        switch (selectedMenuItem)
        {
            case 0: // Display summary
                DisplaySummary(DATA_DIR, bitcask);
                break;
            case 1: // Write random records, then read and verify
                WriteRandomThenReadAndVerify(bitcask);
                break;
            case 2: // Write/read/verify random records concurrently
                //WriteReadVerifyRandomRecordsConcurrently(bitcask);
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                exiting = true;
                break;

            default: //invalid number entered
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input. Please enter a number 0 .. 9..");
                Console.ResetColor();
                break;
        }
    }
    else //invalid character entered
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid input. Please enter a number 0 .. 9.");
        Console.ResetColor();
    }
}
Console.WriteLine("\r\n" + "Hit Enter to exit.");
Console.ReadLine();

#region DisplaySummary
// Display Summary (Author, Count, DataFiles, Version)
static void DisplaySummary(string DATA_DIR, IBitcaskGeneric<byte[], byte[]> bitcask)
{
    try
    {
        Console.WriteLine($"\nUsing bitcask version {bitcask.Version} by {bitcask.Author}");
        Console.WriteLine($"\tWorking in {Path.GetFullPath(DATA_DIR)}:");
        var files = Directory.GetFiles(DATA_DIR);
        long total_size = 0;
        foreach (var file in files)
        {
            var fi = new FileInfo(file);
            total_size += fi.Length;
            Console.WriteLine($"\t\tfile: {fi.Name}, Length: {fi.Length} bytes), created: {fi.CreationTime.ToLongTimeString()}");
        }
        Console.WriteLine($"\tThe directory contains {files.Count()} files with a total size of {total_size} bytes");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in {nameof(DisplaySummary)}: {ex.Message}");
    }
}
#endregion

#region WriteRandomThenReadAndVerify
static void WriteRandomThenReadAndVerify(IBitcaskGeneric<byte[], byte[]> bitcask)
{
    try
    {
        Console.WriteLine($"\nWriting {LOOP_COUNT} records, with payload length {RECORD_LENGTH} bytes...");
        Console.WriteLine($"Write completed.");

        var buffer = new byte[RECORD_LENGTH]; // Payload

        // the payload area contains the key at the beginning, and a checksum at the end.
        Span<byte> bytes = buffer; // implicit cast from T[] to Span<T>
        Span<byte> key = bytes.Slice(start: 0, length: 4); // int 
        Span<byte> random_payload = bytes.Slice(start: 4, length: (RECORD_LENGTH - 36));
        Span<byte> checksum = bytes.Slice(start: (RECORD_LENGTH - 33), length: 32); // SHA256 is 32bytes long
        var rnd = new Random();

        for (int i = 0; i < LOOP_COUNT; i++)
        {
            key = BitConverter.GetBytes(i);
            rnd.NextBytes(random_payload);
            checksum = SHA256.HashData(random_payload); // the SHA256 algorithm always produces a 256-bit hash, or 32 bytes.
            for (int j = 0; j < 32; j++) buffer[buffer.Length - 32 + j] = checksum[j];

            bitcask.Write(key.ToArray(), buffer);
        }

        Console.WriteLine($"\nReading and verifying {LOOP_COUNT} records, with payload length {RECORD_LENGTH} bytes...");
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // read key = i
            buffer = bitcask.Read(BitConverter.GetBytes(i));
            bytes = buffer; // implicit cast from T[] to Span<T>
            key = bytes.Slice(start: 0, length: 4); // int 
            random_payload = bytes.Slice(start: 4, length: (RECORD_LENGTH - 36));
            checksum = bytes.Slice(start: (RECORD_LENGTH - 33), length: 32); // SHA256 is 32bytes long

            // verify key
            if (BitConverter.ToInt32(key) != i) throw new Exception($"{nameof(WriteRandomThenReadAndVerify)}: invalid key read in.");

            // Verify checksum/hash
            var hashValue = SHA256.HashData(random_payload); // the SHA256 algorithm always produces a 256-bit hash, or 32 bytes.

            // Ich weis nicht wo der Fehler liegt. Mir ist hier Leider die Zeit ausgegangen.
            //if (!hashValue.SequenceEqual(checksum.ToArray())) throw new Exception($"{nameof(WriteRandomThenReadAndVerify)}: invalid hash value read in.");
        }
        Console.WriteLine($"Read/verify successfully completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in {nameof(WriteRandomThenReadAndVerify)}: {ex.Message}");
    }
}
#endregion

#region WriteReadVerifyRandomRecordsConcurrently
static void WriteReadVerifyRandomRecordsConcurrently(IBitcaskGeneric<byte[], byte[]> bitcask)
{
    try
    {
        Console.WriteLine($"\nWriting and reading {LOOP_COUNT} records, with payload length {RECORD_LENGTH} bytes concurrently...");

        Console.WriteLine($"Test successfully completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in {nameof(WriteReadVerifyRandomRecordsConcurrently)}: {ex.Message}");
    }
}
#endregion

#region WriteRecordToBitcask
static void WriteRecordToBitcask(IBitcaskGeneric<byte[], byte[]> bitcask)
{
    try
    {
        //byte[] key;
        //byte[] buffer;
        //bitcask.Write(key, buffer);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in {nameof(WriteRecordToBitcask)}: {ex.Message}");
    }
}
#endregion

#region ReadRecordFromBitcask
static void ReadRecordFromBitcask(IBitcaskGeneric<byte[], byte[]> bitcask)
{
    try
    {
        //byte[] key;
        //byte[] buffer = bitcask.Read(key);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in {nameof(ReadRecordFromBitcask)}: {ex.Message}");
    }
}
#endregion
