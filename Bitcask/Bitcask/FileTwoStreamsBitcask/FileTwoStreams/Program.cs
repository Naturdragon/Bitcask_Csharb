//#define InMemory
#define OnDisk

#define Concurrent
//#define Sequential

using BitcaskDraftAPI;

const string TITLE = "FileTwoStreamsBitcask";
Console.Title = TITLE + " " + Environment.UserName + " " + Environment.MachineName;
Console.WriteLine(TITLE);
Console.WindowWidth = 150;

const int LOOP_COUNT = 100_000; // how many reads & writes
const int RECORD_LENGTH = 64*1024; // => duration about 8min 30sec with delay timeouts

//const string DATA_DIR = @"..\..\..\Data";
const string ACTIVE_DATA_FILE = @"..\..\..\Data\ActiveFile.data";

// Display filesize
Console.WriteLine($"Using {(LOOP_COUNT * (RECORD_LENGTH/1024.0))/(1024.0):N2} MBytes on disc.");

// Stopwatch
var stpw = new Stopwatch();

// Lock object
//var lockObj = new Object();

try
{
    // Open file with two filestreams
    Console.WriteLine($"Opening file with two filestreams ... ");
    var path = ACTIVE_DATA_FILE;
#if InMemory
    var bfs = BitcaskFileStream.Create(path, StorageType.InMemory);
#endif
#if OnDisk
    Console.WriteLine($"Using file {Path.GetFullPath(path)}");
    if (File.Exists(path)) File.Delete(path);
    var bfs = BitcaskFileStream.Create(path, StorageType.OnDisc);
#endif

    // Define threads
    var t0 = new Thread(WriterThread);
    t0.Name = "WriterThread";

    var t1 = new Thread(ReaderThread);
    t1.Name = "ReaderThread";

    // Start threads
    stpw.Start();   // start stopwatch

#if Concurrent
    t0.Start(bfs);    // writer
    t1.Start(bfs);      // reader

    t0.Join();
    t1.Join();
#endif

#if Sequential
    t0.Start(bfs);    // writer
    t0.Join();

    //t1.Start(bfs);      // reader
    //t1.Join();

    ReaderThread(bfs);
#endif

    stpw.Stop();   // stop stopwatch
    bfs.Dispose();

    //File.Delete(path);  // this lead to errors

    Console.WriteLine($"{2*LOOP_COUNT/ (stpw.ElapsedMilliseconds / 1000.0):N2} file accesses per second; " +
        $"combined throughput: {((2*LOOP_COUNT * (RECORD_LENGTH / 1024.0)) / (1024.0))/(stpw.ElapsedMilliseconds/1000.0):N2} MB/sec");
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}\n\t{ex.InnerException?.Message} ");
}

Console.WriteLine("\nHit Enter to Exit.");
Console.ReadLine();

// --------------------------------------------------------------------

/// <summary>
/// WriterThread appends to file
/// </summary>
/// <remarks>
/// The WriterThread worker appends a record to the existing file. The record contains the recordId (1st four bytes, int32)
/// and a random byte pattern.
/// </remarks>
void WriterThread(object obj)
{
    var bfs = (Stream)obj;
    //Console.WriteLine($"Writer thread started.");
    var buffer = new byte[RECORD_LENGTH];  // fixed length records
    var rndSize = new Random();
    var rndTime = new Random();

    try
    {
        // optimized for append-only operation!

            rndSize.NextBytes(buffer); // fill array with random numbers

            for (int i = 0; i < LOOP_COUNT; i++)
            {
                // Override the first 4 bytes with the record Id which is the loop count i (int32)
                var recordId = BitConverter.GetBytes(i);
                Buffer.BlockCopy(recordId, 0, buffer, 0, 4);
                var Id2 = BitConverter.ToInt32(buffer, 0);
                if (Id2 != i)
                {
                    Console.WriteLine($"Writer: invalid id.");
                }

            // Append to file 
            //lock (lockObj)
            //{
                bfs.Write(buffer, 0, buffer.Length); 
            //}

            //if (i % 1000 == 0)
            //{
                //Console.WriteLine($"WriterThread: appended recordId {i}.");
            //}

            // Delay a random period
            Thread.Sleep(rndTime.Next(1,101));
            //Thread.Sleep(1000);
        }
        Console.WriteLine($"Writer thread terminated successfully.");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"WriterThread Exception: {ex.Message}\n\t{ex.InnerException?.Message}");
    }
    finally
    {
    }
}

/// <summary>
/// Read random record from file and check its integrity
/// </summary>
void ReaderThread(object obj)
{
    var bfs = (Stream)obj;

    //Console.WriteLine($"Reader thread started.");

    var buffer = new Byte[RECORD_LENGTH];
    var rnd = new Random();
    var rndTime = new Random();

    try
    {
        // optimized for read-only random-access operation!

            // Wait for the write stream to append at least one record
            while (bfs.Length == 0) Thread.Sleep(500); // manchmal kommt Länge = 0

            for (int i = 0; i < LOOP_COUNT; i++)
            {
            // Compute a random recordId - we need to know the current number of records
            //lock (lockObj)
            //{
                var length = bfs.Length;
                var highestRecordId = ((int)(length / RECORD_LENGTH)) - 1; // recordID starts with 0
                var randomRecordId = rnd.Next(0, highestRecordId + 1);
                var position = (long)randomRecordId * RECORD_LENGTH; // position must be long!
                //Console.WriteLine($"\nReading recordId {randomRecordId} at position {position}");
                bfs.Position = position;

                // Read the record at random position
                bfs.Read(buffer, 0, buffer.Length); // this moves bfs.Position past the read record
                //Console.WriteLine($"fresh: {buffer[3]} {buffer[2]} {buffer[1]} {buffer[0]}");
                // Get the recordId from the first four bytes and verify it
                var recordId = BitConverter.ToInt32(buffer, 0);

                //Console.WriteLine($"recordId random: {randomRecordId}, recordId read: {recordId}, position computed:{position}");
                //Console.WriteLine($"Position read: {bfs.Position - RECORD_LENGTH}");
                //if (recordId == randomRecordId) Console.WriteLine($"\tCorrect recordId");
                if (recordId != randomRecordId) 
                {
                    Console.WriteLine($"\tIncorrect recordId {recordId} instead of {randomRecordId} at position {position}");
                }
            //}

            // Delay a random period
            Thread.Sleep(rndTime.Next(1, 101));
            //Thread.Sleep(1000);
        }

        Console.WriteLine($"Reader thread terminated successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ReaderThread Exception: {ex.Message}\n\t{ex.InnerException?.Message}");
    }
    finally
    {
    }
}