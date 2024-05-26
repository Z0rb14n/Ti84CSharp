namespace Ti84App;

public static class Program
{
    private static TerminalEmulator _emulator = new();
    private static void Main(string[] args)
    {
        //Execute("BINOTHEO.txt");
        Execute("TESTFUNC.txt");
        //Execute("CIRCMOVE.txt");
    }

    private static void Execute(string program)
    {
        _emulator.Execute(File.ReadAllText($"../../../data/{program}"));
    }
    
    
}