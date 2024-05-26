namespace Ti84App;

public class ConsoleDisplay
{
    public void ClearHome()
    {
        Console.Clear();
    }

    public void Output(int row, int col, string str)
    {
        Console.SetCursorPosition(col - 1, row - 1); // one-indexed
        Write(str);
    }

    public void Write(string str)
    {
        Console.Write(str);
    }

    public void WriteLine(string str)
    {
        Console.WriteLine(str);
    }

    public void WaitForEnterPress()
    {
        _ = Console.ReadLine();
    }

    public string ReadLineNonEmpty()
    {
        int left = Console.CursorLeft;
        int top = Console.CursorTop;
        string? line = Console.ReadLine();
        while (string.IsNullOrEmpty(line))
        {
            Console.SetCursorPosition(left, top);
            line = Console.ReadLine();
        }

        return line;
    }
}