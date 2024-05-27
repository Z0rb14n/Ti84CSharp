namespace Ti84App;

public static class EntryMain
{
    private static readonly TerminalEmulator _emulator = new();
    private static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            foreach (string str in args)
            {
                if (File.Exists(str)) _emulator.Execute(File.ReadAllText(str));
                else if (File.Exists($"../../../data/{str}.txt")) Execute(str);
                else
                {
                    Console.WriteLine($"File {str} does not exist; skipping.");
                }
            }

            return;
        }
        //Execute("BINOTHEO");
        //Execute("TESTFUNC");
        //Execute("CIRCMOVE");
        //Execute("TESTCURSOR");
        // Execute("DEG2RAD");
        // Execute("ELECTRIC");
        Execute("FGRAVITY");
    }

    private static void Execute(string program)
    {
        _emulator.Execute(File.ReadAllText($"../../../data/{program}.txt"));
    }
}