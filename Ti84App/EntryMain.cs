namespace Ti84App;

public static class EntryMain
{
    private static TerminalEmulator _emulator = new();
    private static void Main(string[] args)
    {
        //Execute("BINOTHEO.txt");
        //Execute("TESTFUNC.txt");
        Execute("CIRCMOVE.txt");
        //Execute("TESTCURSOR.txt");
    }

    private static void Execute(string program)
    {
        _emulator.Execute(File.ReadAllText($"../../../data/{program}"));
    }
    
    
}