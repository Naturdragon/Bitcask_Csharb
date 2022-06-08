ReadMe.txt FileTwoStreamsBitcask funktioniert
================================
V1.0, 19-May-2022, Herbert Feichtinger
	THIS IS A MODIFIED VERSION OF  FileTwoStreams Project. It uses the Bitcask file stream 
	for in-memory or on-disc operations.
	Aufwand: 150 LOC
	Use two filestreams, one for reading, one for writing a file.
	Prove that this is thread-safe for an append-only file, as long as there is only a single read and a single write thread.
V1.1, 24-May-2022, comments added

Die FileStreams sind für Random-Read und Append-Only-Write optimiert.

Da Read und Write getrennte FileStreams verwenden, brauche ich kein Lock. 
Damit wäre dieser Code als Library aber nicht thread-safe, da ja Read und Write mehrfach gleichzeitig aus verschiedenen Threads aufgerufen werden können.
Es gibt zwei Möglichkeiten:
a) Der aufrufende application code muss sich seine Locks selber machen, wenn es mehr als einen Write Thread oder mehr 
   als einen Read Thread gibt.
b) Ich baue einen ReadLock (nur für die Reads) und einen WriteLock (nur für die Writes) ein. Oder jeweils eine "einfachere Lock" Variante (SlimLock). 

This is a series of PoC Tests for a Bitcask replica:
	C:\DotNetCore\FilePerformance
	C:\DotNetCore\FileTwoStreams
	C:\DotNetCore\FileTwoStreamsAsync
	C:\DotNetCore\FileSizePerformance

MemoryStream Performance:
25.929,13 file accesses per second; combined throughput: 1.620,57 MB/sec  bei 64kB record length

FileStream Performance:
8.813,68 file accesses per second; combined throughput: 550,85 MB/sec (mit locks)
10.383,14 file accesses per second; combined throughput: 648,95 MB/sec (ohne locks)

=> MemoryStream ist ca. 3mal schneller
