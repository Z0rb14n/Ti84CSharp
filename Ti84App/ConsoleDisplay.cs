using System.Text;

namespace Ti84App;

public class ConsoleDisplay
{
    // ReSharper disable twice IdentifierTypo
    private readonly List<StringBuilder> _homeBuf = [new StringBuilder()];
    private readonly List<StringBuilder> _menuBuf = [];

    private int _cursorLeft;
    private int _cursorTop;

    // ReSharper disable once SuggestBaseTypeForParameter
    private static void EnsureExists(int row, int col, List<StringBuilder> sbs)
    {
        while (sbs.Count < row) sbs.Add(new StringBuilder());
        if (sbs[row - 1].Length < col) sbs[row - 1].Append(' ', col - sbs[row - 1].Length);
    }
    
    public void ClearHome()
    {
        _homeBuf.Clear();
        _homeBuf.Add(new StringBuilder());
        _cursorLeft = 0;
        _cursorTop = 0;
        UpdateDisplay();
    }

    private void UpdateDisplay(bool home = true)
    {
        List<StringBuilder> sbs = home ? _homeBuf : _menuBuf;
        Console.Clear();
        foreach (StringBuilder sb in sbs) Console.WriteLine(sb.ToString());

        Console.CursorLeft = _cursorLeft;
        Console.CursorTop = _cursorTop;
    }

    public void Output(int row, int col, string str)
    {
        EnsureExists(row, col, _homeBuf);
        StringBuilder sb = _homeBuf[row - 1];
        if (sb.Length >= col)
            sb.Remove(col - 1, Math.Min(sb.Length - col + 1, str.Length));
        if (sb.Length < col)
            sb.Append(' ', col - sb.Length-1);
        Write(str, false);
    }

    public int Menu(string title, string[] labels)
    {
        while (_menuBuf.Count < labels.Length+1) _menuBuf.Add(new StringBuilder());
        _menuBuf[0].Clear().Append(title);
        for (int i = 0; i < labels.Length; i++)
        {
            _menuBuf[i + 1].Clear().Append(i + 1).Append(':').Append(labels[i]);
        }

        int selected = 0;
        _cursorLeft = 0;
        _cursorTop = 1;

        while (true)
        {
            UpdateDisplay(false);
            ConsoleKeyInfo info = Console.ReadKey(true);
            if (info.Key == ConsoleKey.DownArrow)
            {
                selected = (selected + 1) % labels.Length;
                _cursorTop = selected+1;
            } else if (info.Key == ConsoleKey.UpArrow)
            {
                selected = selected == 0 ? labels.Length - 1 : selected - 1;
                _cursorTop = selected+1;
            } else if (info.Key == ConsoleKey.Enter)
            {
                return selected;
            }
            UpdateDisplay(false);
        }
    }

    public void Write(string str, bool updateCursor = true)
    {
        _homeBuf.Last().Append(str);
        if (updateCursor)
        {
            _cursorLeft = _homeBuf.Last().Length;
            _cursorTop = _homeBuf.Count-1;
        }
        UpdateDisplay();
    }

    public void WriteLine(string str, bool updateCursor = true)
    {
        _homeBuf.Last().Append(str);
        _homeBuf.Add(new StringBuilder());
        if (updateCursor)
        {
            _cursorLeft = _homeBuf.Last().Length;
            _cursorTop = _homeBuf.Count-1;
        }
        UpdateDisplay();
    }

    public static void WaitForEnterPress()
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

        _homeBuf.Last().Append(line);
        _homeBuf.Add(new StringBuilder());

        return line;
    }
}