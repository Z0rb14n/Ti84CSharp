namespace Ti84App;

public static class EntryMain
{
    private static readonly TerminalEmulator _emulator = new();
    private static void Main(string[] args)
    {
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