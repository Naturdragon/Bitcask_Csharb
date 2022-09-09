ReadMe BitcaskTest4BHIF   in ARBEIT!
=======================
V1.0, 29-May-2022, Herbert Feichtinger
V1.1, 30-May-2022: first version shipped, supports menu item 0 and 1.

Idea: 
Every student in the 4BHIF of 2021/2022 had to write an implementation of Bitcask based on my InMemory/OnDisk library.
This is the test program to verify the correct implementation of IBitcaskGeneric.

Usage: 
- Replace the Bitcask class in BitcaskLib by your implementation.
- In MenuConsole Program in line 20 replace the Bitcask class by your class.
- Start the MenuConsole and do the tests.
- Note:
	The Bitcask class internally must verify the integrity of a record using a checksum (CRC or Hash), the key, the timestamp
	and all other internals.

The payload is a byte array with random numbers, containing both the key (at the beginning) and a hash checksum (at the end).
The key is a byte array created from an int number 0,1,2,...