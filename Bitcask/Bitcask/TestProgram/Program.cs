using TestProgram;

bool exit = false;
while (!exit)
{
    Console.WriteLine("\n0\tTesting Properties (Without Path)\n1\tTesting Properties (with Path)\n2\tTesting Basic Bitcask (Write and Read)\n9\tExit\n\n");
    string answer = Console.ReadLine();
    switch (answer)
    {
        case "0":
            Tests.testingPropsNoPath();
            break;
        case "1":
            Tests.testingPropsWithPath();
            break;
        case "2":Tests.testingBasicBitcask();
            break;
        case "9":
            exit = true;
            break;
        default:
            break;
    }
}