namespace Ti84App;

public static class Program
{
    private static void Main(string[] args)
    {
        TerminalEmulator emulator = new();
        //emulator.Execute(File.ReadAllText("../../../data/BINOTHEO.txt"));
        emulator.Execute(File.ReadAllText("../../../data/TESTFUNC.txt"));
        //emulator.Execute(File.ReadAllText("../../../data/CIRCMOVE.txt"));
    }
    
    
}