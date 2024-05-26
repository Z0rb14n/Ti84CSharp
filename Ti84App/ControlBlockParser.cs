using static Ti84App.Util;

namespace Ti84App;

public class ControlBlockParser
{
    private class IfElseParseState(IfElseBlock prev)
    {
        public IfElseBlock prev = prev;
        public bool hasSeenThen;
        public bool hasSeenElse;
        public bool hasParsedBlock;
    }

    public static RootBlock Parse(string program) => Parse(program.Split('\n'));

    public static RootBlock Parse(string[] lines)
    {
        lines = TrimAll(lines);
        Dictionary<string, List<IBlock>> labels = new();
        List<IBlock> root = [];
        int index = 0;
        Stack<IfElseParseState> prev = new();
        while (index < lines.Length)
        {
            IBlock toAdd;
            IfElseParseState? toPush = null;
            if (lines[index].StartsWith("If "))
            {
                IfElseBlock ifelse = new(lines[index][3..]);
                toPush = new IfElseParseState(ifelse)
                {
                    hasSeenThen = false,
                    hasSeenElse = false,
                    hasParsedBlock = false,
                };
                toAdd = ifelse;
            } else if (lines[index] == "Then")
            {
                if (prev.Count == 0) throw new Exception("Received Then before If/Else");
                if (prev.Peek().hasSeenThen) throw new Exception("Received Then twice");
                if (prev.Peek().hasParsedBlock) throw new Exception("Received Then after end of if block");
                prev.Peek().hasSeenThen = true;
                index++;
                continue;
            } else if (lines[index] == "Else")
            {
                if (prev.Count == 0) throw new Exception("Received Else before If");
                if (prev.Peek().hasSeenElse) throw new Exception("Received Else twice");
                prev.Peek().prev.ElseBlock = new Block();
                prev.Peek().hasSeenElse = true;
                prev.Peek().hasParsedBlock = false;
                prev.Peek().hasSeenThen = false;
                index++;
                continue;
            } else if (lines[index] == "End")
            {
                if (prev.Count == 0) throw new Exception("End does not proceed If block");
                prev.Pop();
                index++;
                continue;
            } else if (lines[index].StartsWith("Lbl "))
            {
                string label = lines[index][4..];
                if (label.Contains(' ')) throw new Exception("Label has extra space: " + lines[index]);
                bool canAdd = labels.TryAdd(label, prev.Select(ifelse => ifelse.prev).Cast<IBlock>().ToList());
                if (!canAdd) throw new Exception("Can't add??? duplicate label: " + lines[index]);
                index++;
                continue;
            }
            else
            {
                toAdd = new OneLine(lines[index]);
                if (prev.Count > 0)
                {
                    // there is no Then
                    if (!prev.Peek().hasSeenThen)
                    {
                        // no preceding line (i.e. just after if/else)
                        if (prev.Peek().hasParsedBlock)
                        {
                            prev.Pop();
                        }
                        else
                        {
                            prev.Peek().hasParsedBlock = true;
                        }
                    }
                }
            }

            if (prev.Count == 0) root.Add(toAdd);
            else
            {
                if (prev.Peek().hasSeenElse) ((Block)prev.Peek().prev.ElseBlock!).Children.Add(toAdd);
                else ((Block)prev.Peek().prev.IfBlock).Children.Add(toAdd);
            }
            if (toPush != null) prev.Push(toPush);
            index++;
        }
        
        return new RootBlock()
        {
            Children = root.ToArray()
        };
    }
}

public interface IBlock;

public class IfElseBlock(string cond) : IBlock
{
    public string Condition = cond;
    public Block IfBlock = new Block();
    public Block? ElseBlock;
}

public class RootBlock : Block
{
    public Dictionary<string, List<IBlock>> Labels = new();
}

public class Block : IBlock
{
    public IList<IBlock> Children = new List<IBlock>();
}

public class OneLine(string line) : IBlock
{
    public string Line = line;
}