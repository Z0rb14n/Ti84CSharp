using Ti84App.Variable;

namespace Ti84App;

using static Util;

public class TerminalEmulator
{
    private readonly VariableData _variables = new();
    private readonly ConsoleDisplay _display = new();

    private static string[] SplitArgs(string str)
    {
        List<string> strings = new();
        Stack<char> chars = new();
        bool inString = false;
        int left = 0;
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            switch (c)
            {
                case '(':
                case '{':
                case '[':
                    if (!inString) chars.Push(c);
                    continue;
                case '"':
                    inString = !inString;
                    continue;
                case ')':
                case '}':
                case ']':
                    if (inString) continue;
                    if (!chars.TryPeek(out char top) || top != FlipBracket(c))
                        throw new Exception("Mismatched brackets: " + str);
                    chars.Pop();
                    continue;
                case ',':
                    if (inString) continue;
                    if (chars.Count == 0)
                    {
                        strings.Add(str[left..i]);
                        left = i + 1;
                    }
                    break;
            }
        }

        if (chars.Count != 0) throw new Exception("Mismatched brackets: " + str);
        strings.Add(str[left..]);
        return strings.ToArray();
    }

    private static string? ToDisplay(object? obj)
    {
        if (obj == null) return "null";
        if (obj is RationalOrDecimal[] rat)
        {
            return '{' + string.Join(' ', rat) + '}';
        }

        return obj.ToString();
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

        _display.Output(r.ToInt32(), c.ToInt32(), ToDisplay(strVal) ?? throw new Exception("Null output???"));
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

            ConsoleDisplay.WaitForEnterPress();
        }
        else if (splits.Length == 2)
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
                toWrite = ToDisplay(InterpretExpression(t)) ??
                          throw new Exception("Null on interpret expression from Disp?");
            }

            _display.WriteLine(toWrite);
        }
    }

    private void Input(string? str, string variable)
    {
        if (str == null) _display.Write("?");
        else
        {
            object var = InterpretExpression(str);
            _display.Write(var as string ?? throw new Exception("Input evaluated string is null: " + str));
        }

        _variables.SetVariableValue(variable, InterpretExpression(_display.ReadLineNonEmpty()));
    }

    public void Prompt(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) throw new Exception("Prompt expects an input");
        string[] vars = TrimAll(SplitArgs(line));
        foreach (string var in vars)
        {
            _display.Write(var + "=?");
            _variables.SetVariableValue(var, InterpretExpression(_display.ReadLineNonEmpty()));
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

    public string Menu(string[] param)
    {
        string title = InterpretExpression(param[0]) as string ?? throw new Exception("Menu title evaluated as null: " + param[0]);
        string[] vars = new string[(param.Length - 1) /2];
        string[] labels = new string[(param.Length - 1) /2];
        for (int i = 0; i < vars.Length; i++)
        {
            object var = InterpretExpression(param[1+2*i]);
            vars[i] = var as string ?? throw new Exception("Menu: evaluated string is null: " + param[1+2*i]);
            labels[i] = param[2 + 2 * i];
        }

        return labels[_display.Menu(title, vars)];
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
        RootBlock root = ControlBlockParser.Parse(program);
        Stack<(Block, int)> blocks = new();
        blocks.Push((root, 0));
        while (blocks.Count > 0)
        {
            (Block block, int index) = blocks.Pop();
            if (index < block.Children.Count - 1) blocks.Push((block, index + 1));
            if (block.Children[index] is OneLine oneLine)
            {
                string line = oneLine.Line;
                if (line == "") continue;
                if (line == "ClrHome")
                {
                    _display.ClearHome();
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
                    string trimmedLine = line.Substring(7, line.Length  - (line.EndsWith(')') ? 1 : 0) - 7);
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

                if (line.StartsWith("Menu("))
                {
                    string trimmedLine = line.Substring(5, line.Length - (line.EndsWith(')') ? 1 : 0) - 5);
                    string[] args = TrimAll(SplitArgs(trimmedLine));
                    if (args.Length < 3) throw new Exception($"Menu has {args.Length} args; expected 3+: {line}");
                    if (args.Length % 2 == 0)
                        throw new Exception($"Menu needs odd number of args; received {args.Length}: {line}");
                    blocks = new Stack<(Block, int)>(root.Labels[Menu(args)]);
                    continue;
                }

                if (line == "Stop")
                {
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