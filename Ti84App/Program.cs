namespace Ti84App;

public static class Program
{
    private static void Main(string[] args)
    {
        TerminalEmulator emulator = new();
        //emulator.Execute(File.ReadAllText(@"C:\Users\Jonathan Nah\RiderProjects\Ti84App\Ti84App\data\BINOTHEO.txt"));
        emulator.Execute(File.ReadAllText(@"C:\Users\Jonathan Nah\RiderProjects\Ti84App\Ti84App\data\CIRCMOVE.txt"));
    }
    
    
}