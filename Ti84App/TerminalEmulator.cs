using System.Diagnostics;
using Ti84App.Variable;

namespace Ti84App;

using static Util;

public class TerminalEmulator
{
    private readonly VariableData _variables = new();

    private static string[] SplitArgs(string str)
    {
        return str.Split(',');
    }

    private static void ClearHome()
    {
        Console.Clear();
    }

    private static void Output(int row, int col, string str)
    {
        Console.SetCursorPosition(col - 1, row - 1); // one-indexed
        Console.Write(str);
    }

    private void Output(string param1, string param2, string str)
    {
        object row = InterpretExpression(param1);
        object col = InterpretExpression(param2);
        object strVal = InterpretExpression(str);
        if (row is not RationalOrDecimal r || col is not RationalOrDecimal c)
        {
            throw new Exception("Output expects numeric values: " + param1 + "," + param2);
        }

        if (!r.IsInteger || !c.IsInteger)
        {
            throw new Exception("Output expects integer values");
        }

        Output(r.ToInt32(), c.ToInt32(), strVal.ToString() ?? throw new Exception("Null output???"));
    }

    private void Pause(string param)
    {
        string[] splits = TrimAll(SplitArgs(param));
        if (splits.Length <= 1)
        {
            if (splits.Length == 1)
            {
                Disp(splits[0]);
            }
            _ = Console.ReadLine();
        } else if (splits.Length == 2)
        {
            Disp(splits[0]);
            object var = InterpretExpression(splits[1]);
            if (var is RationalOrDecimal rat)
                Thread.Sleep((int)Math.Round((decimal)rat * 1000));
            else throw new Exception("Sleep time is not numeric");
        }
    }

    private void Disp(params string[] str)
    {
        if (str.Length == 0) return;
        foreach (string t in str)
        {
            string toWrite = "";
            if (t.Length != 0)
            {
                toWrite = InterpretExpression(t).ToString() ?? throw new Exception("Null on interpret expression from Disp?");
            }
            Console.WriteLine(toWrite);
        }
    }

    private void Input(string? str, string variable)
    {
        if (str == null) Console.Write("?");
        else
        {
            object var = InterpretExpression(str);
            Debug.Assert(var is string);
            Console.Write(var.ToString());
        }

        int left = Console.CursorLeft;
        int top = Console.CursorTop;
        string? line = Console.ReadLine();
        while (string.IsNullOrEmpty(line))
        {
            Console.SetCursorPosition(left, top);
            line = Console.ReadLine();
        }

        _variables.SetVariableValue(variable, InterpretExpression(line));
    }

    public void Prompt(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) throw new Exception("Prompt expects an input");
        string[] vars = TrimAll(SplitArgs(line));
        foreach (string var in vars)
        {
            Console.Write(var + "=?");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            string? input = Console.ReadLine();
            while (string.IsNullOrEmpty(input))
            {
                Console.SetCursorPosition(left, top);
                input = Console.ReadLine();
            }
            _variables.SetVariableValue(var, InterpretExpression(input));
        }
    }

    public void Store(string line)
    {
        char arrow = '\u2192';
        string[] splits = line.Split(arrow);
        if (splits.Length > 2) throw new Exception("Multiple stores in one store-line");
        string name = splits[1];
        Queue<Tokenizer.Token> tokens = ExpressionParser.Parse(name);
        if (tokens.Count > 1) throw new Exception("Name has multiple tokens");
        if (tokens.Peek().type != Tokenizer.TokenType.Variable) throw new Exception("Name is not a variable token");
        _variables.SetVariableValue(name, InterpretExpression(splits[0]));
    }

    private object InterpretExpression(string inputString)
    {
        if (inputString[0] == '"' && inputString.Last() == '"')
        {
            return inputString.Substring(1, inputString.Length - 2);
        }
        // so NEGATIVES use -
        // while minus uses the em dash — 
        if (Int32.TryParse(inputString, out int value))
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return new RationalOrDecimal
            {
                IsRational = true,
                RationalValue = value
            };
        }

        return ExpressionEval.Evaluate(ExpressionParser.Parse(inputString), _variables);
    }

    public void Execute(string program)
    {
        Block root = ControlBlockParser.Parse(program);
        Stack<(Block, int)> blocks = new();
        blocks.Push((root, 0));
        while (blocks.Count > 0)
        {
            (Block block, int index) = blocks.Peek();
            blocks.Pop();
            if (index < block.Children.Count-1) blocks.Push((block, index+1));
            if (block.Children[index] is OneLine oneLine)
            {
                string line = oneLine.Line;
                if (line == "") continue;
                if (line == "ClrHome")
                {
                    ClearHome();
                    continue;
                }

                if (line.StartsWith("Pause ") || line == "Pause")
                {
                    Pause(line[5..]);
                    continue;
                }
                

                if (line.StartsWith("Prompt ") || line == "Prompt")
                {
                    Prompt(line[6..]);
                    continue;
                }

                if (line.StartsWith("Disp ") || line == "Disp")
                {
                    if (line.Length <= 5) continue;
                    string[] splits = SplitArgs(line[5..]);
                    Disp(splits);
                    continue;
                }

                if (line.StartsWith("Output("))
                {
                    if (!line.EndsWith(')'))
                    {
                        throw new Exception("Output lacks closing brace: " + line);
                    }

                    string trimmedLine = line.Substring(7, line.Length - 1 - 7);
                    string[] args = TrimAll(SplitArgs(trimmedLine));
                    if (args.Length != 3) throw new Exception($"Output has {args.Length} args, expected 3: " + line);
                    Output(args[0], args[1], args[2]);
                    continue;
                }

                if (line == "Input") throw new Exception("Expected argument to Input");

                if (line.StartsWith("Input "))
                {
                    string trimmedLine = line.Substring(6, line.Length - 6);
                    string[] args = TrimAll(SplitArgs(trimmedLine));
                    switch (args.Length)
                    {
                        case 1:
                            Input(null, args[0]);
                            break;
                        case 2:
                            Input(args[0], args[1]);
                            break;
                        default:
                            throw new Exception($"Input expects 1-2 args, received {args.Length}: " + line);
                    }

                    continue;
                }

                if (line == "Stop")
                {
                    Debug.WriteLine("Reached Stop. Stopping...");
                    return;
                }

                if (line.Contains('→'))
                {
                    Store(line);
                    continue;
                }

                throw new NotImplementedException("Unknown line: " + line);
            }
            else if (block.Children[index] is IfElseBlock ifBlock)
            {
                object val = InterpretExpression(ifBlock.Condition);
                if (val is not RationalOrDecimal rat)
                {
                    throw new Exception($"Expected numeric in if statement, got {val.GetType()}: " + ifBlock.Condition);
                }

                if (rat != 0)
                {
                    blocks.Push((ifBlock.IfBlock, 0));
                }
                else
                {
                    if (ifBlock.ElseBlock != null) blocks.Push((ifBlock.ElseBlock, 0));
                }
            }
        }
    }
}