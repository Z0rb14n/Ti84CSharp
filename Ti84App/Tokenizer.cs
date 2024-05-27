using System.Collections.Frozen;
using Ti84App.Variable;

namespace Ti84App;

public static class Tokenizer
{
    private static readonly FrozenSet<string> Operators =
    FrozenSet.ToFrozenSet([
        "^", " nPr ", " nCr ", "*", "/", "+", "—", ">", "<", "=",
        "\u2260", // neq
        "\u2265", // geq
        "\u2264", // leq
        " xor ", " and ", " or "
    ]);

    public const char EChar = 'e';
    public const char PiChar = 'π';

    private static readonly FrozenSet<char> LeftUnaryOps = FrozenSet.ToFrozenSet(['-']);
    private static readonly FrozenSet<char> RightUnaryOps = FrozenSet.ToFrozenSet(['!','\u00b2']);
    private static readonly FrozenSet<char> NumericalConsts = FrozenSet.ToFrozenSet([EChar, PiChar]);
    private static readonly List<string> Functions = ["not", "max"];
    private static readonly FrozenSet<char> SingleCharOps;

    static Tokenizer()
    {
        SingleCharOps = Operators.Where(op => op.Length == 1).Select(key => key[0])
            .ToFrozenSet();
    }

    public enum TokenType
    {
        Invalid,
        Number,
        String,
        Variable,
        Function,
        BinaryOperator,
        Comma,
        LeftParen,
        RightParen,
        CurlyLeftParen,
        CurlyRightParen,
        RightUnaryOperator,
        LeftUnaryOperator
    }

    public struct Token
    {
        public TokenType type;
        public string data;
        public int count;

        public override string ToString()
        {
            return $"(type:{type}, data:{data})";
        }
    }

    private class TokenNode
    {
        public bool tokenized;
        public Token token;
        public string? data;
        public int startIndex;
        public TokenNode? next;
    }

    private static void Split(TokenNode prev, List<TokenNode> todo, Token token, int left, int right)
    {
        // 3 cases:
        // range occupies entirety of start
        //   subcase: range occupies end as well
        // range occupies entirety of end
        // range is firmly in the middle
        if (left == prev.startIndex)
        {
            prev.tokenized = true;
            prev.token = token;
            todo.Remove(prev);
            if (right == prev.data!.Length)
                prev.data = "";
            else
            {
                TokenNode newNode = new()
                {
                    tokenized = false,
                    data = prev.data[(right)..],
                    next = prev.next
                };
                prev.next = newNode;
                todo.Add(newNode);
            }
        }
        else if (right == prev.startIndex + prev.data!.Length)
        {
            TokenNode newNode = new()
            {
                tokenized = true,
                token = token,
                next = prev.next
            };
            prev.data = prev.data[..(left - prev.startIndex)];
            prev.next = newNode;
        }
        else
        {
            string before = prev.data[..(left - prev.startIndex)];
            string after = prev.data[(right - prev.startIndex)..];
            prev.data = before;
            TokenNode middleNode = new()
            {
                tokenized = true,
                token = token
            };

            TokenNode afterNode = new()
            {
                tokenized = false,
                data = after,
                next = prev.next
            };
            middleNode.next = afterNode;
            prev.next = middleNode;
            todo.Add(afterNode);
        }
    }
    
    // TODO OPTIMIZE LMFAO
    public static List<Token> GetTokens(string inputString)
    {
        TokenNode head = new()
        {
            tokenized = false,
            data = inputString,
            startIndex = 0
        };
        List<TokenNode> todo = [head];
        // get strings first
        for (int index = 0; index < inputString.Length;)
        {
            char c = inputString[index];
            if (c != '"')
            {
                index++;
                continue;
            }
            int next;
            for (next = index + 1; next < inputString.Length && inputString[next] != '"'; next++)
            {
            }

            if (next >= inputString.Length || inputString[next] != '"')
                throw new Exception("Unmatched quote: " + inputString);
            Split(head, todo, new Token() { data = inputString[(index + 1)..(next)], type = TokenType.String }, index,
                next + 1);
            index = next + 1;
        }
        
        while (true)
        {
            bool didDoSomething = false;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < todo.Count; i++)
            {
                TokenNode entry = todo[i];
                foreach (string str in Operators)
                {
                    int index = entry.data!.IndexOf(str, StringComparison.Ordinal);
                    if (index == -1) continue;
                    didDoSomething = true;
                    Split(entry, todo, new Token() { data = str, type = TokenType.BinaryOperator },
                        entry.startIndex + index, entry.startIndex + index + str.Length);
                    if (entry.tokenized)
                    {
                        break;
                    }
                }

                if (entry.tokenized) continue;
                for (int j = 0; j < entry.data!.Length; j++)
                {
                    if (IsCharToken(entry.data[j], out Token charToken))
                    {
                        Split(entry, todo, charToken, entry.startIndex+j, entry.startIndex+j+1);
                        didDoSomething = true;
                        if (entry.tokenized) break;
                    }
                }

                if (entry.tokenized) continue;
                if (entry.next is { tokenized: true, token.type: TokenType.LeftParen })
                {
                    foreach (string func in Functions)
                    {
                        if (!entry.data.EndsWith(func)) continue;
                        Token funcToken = new()
                        {
                            type = TokenType.Function,
                            data = func
                        };
                        int end = entry.startIndex + entry.data.Length;
                        Split(entry, todo, funcToken, end - func.Length,end);
                        didDoSomething = true;
                        break;
                    }
                }
            }

            if (!didDoSomething || todo.Count == 0) break;
        }

        for (int i = 0; i < todo.Count; i++)
        {
            TokenNode node = todo[i];
            if (node.data!.Contains(' '))
                throw new Exception($"Node contains illegal spaces: {node.data}" + "," + inputString);
            int startNumLen = StartNumberLength(node.data);
            if (startNumLen != 0)
            {
                Split(node, todo, new Token(){type = TokenType.Number, data = node.data},node.startIndex, node.startIndex+startNumLen);
                i--;
                continue;
            }
            // parse as variable
            char c = node.data[0];
            if (node.data.StartsWith("Str"))
            {
                if (node.data.Length < 4) throw new Exception("Str variable with no identifier: " + node.data + "," + inputString);
                Split(node, todo, new Token(){type=TokenType.Variable, data = node.data[..4]}, node.startIndex, node.startIndex+4);
                i--;
                continue;
            }

            if (node.data.Length >= 2 && c == 'L' && ListVariable.SubscriptSet.Contains(node.data[1]))
            {
                Split(node, todo, new Token(){type=TokenType.Variable, data = node.data[..2]}, node.startIndex, node.startIndex+2);
                i--;
                continue;
            }

            if (c == ListVariable.SmallL)
            {
                if (node.data.Length == 1) throw new Exception("Invalid 0-len list variable name: " + inputString);
                int maxIter = node.data.Length-1 > 5 ? 5 : node.data.Length-1;
                if (node.data[1] != RealVariable.Theta && node.data[1] < 'A' && node.data[1] > 'Z')
                    throw new Exception($"Invalid list variable name {node.data}; " + inputString);
                int j = 2;
                for (; j <= maxIter; j++)
                {
                    char d = node.data[j];
                    if (d is not (>= 'A' and <= 'Z' or RealVariable.Theta or ':')) break;
                }
                Split(node, todo, new Token(){type=TokenType.Variable, data = node.data[..j]}, node.startIndex, node.startIndex+j);
                i--;
            }
            else
            {
                Split(node, todo, new Token(){type=TokenType.Variable, data = $"{c}"}, node.startIndex, node.startIndex+1);
                i--;
            }
        }

        List<Token> tokens = new();
        TokenNode? curr = head;
        while (curr != null)
        {
            tokens.Add(curr.token);
            curr = curr.next;
        }

        // add * between things
        for (int i = 0; i < tokens.Count-1; i++)
        {
            if (tokens[i].type is TokenType.RightParen or TokenType.Number or TokenType.Variable && 
                (tokens[i+1].type is TokenType.LeftParen or TokenType.Number or TokenType.Variable ||
                 (i < tokens.Count-2 && tokens[i+1].type is TokenType.LeftUnaryOperator
                                     && tokens[i+2].type is TokenType.LeftParen or TokenType.Number or TokenType.Variable)))
            {
                tokens.Insert(i+1,new Token()
                {
                    data = "*",
                    type = TokenType.BinaryOperator
                });
                i ++;
            }
        }

        return tokens;
    }

    private static bool IsCharToken(char c, out Token token)
    {
        switch (c)
        {
            case '(':
                token = new Token() { type = TokenType.LeftParen, data = "(" };
                return true;
            case ')':
                token = new Token() { type = TokenType.RightParen, data = ")" };
                return true;
            case ',':
                token = new Token() { type = TokenType.Comma, data = "," };
                return true;
            case '{':
                token = new Token() { type = TokenType.CurlyLeftParen, data = "{" };
                return true;
            case '}':
                token = new Token() { type = TokenType.CurlyRightParen, data = "}" };
                return true;
        }

        if (NumericalConsts.Contains(c))
        {
            token = new Token() { type = TokenType.Number, data = $"{c}" };
            return true;
        }

        if (SingleCharOps.Contains(c))
        {
            token = new Token() { type = TokenType.BinaryOperator, data = $"{c}" };
            return true;
        }

        if (RightUnaryOps.Contains(c))
        {
            token = new Token() { type = TokenType.RightUnaryOperator, data = $"{c}" };
            return true;
        }

        if (LeftUnaryOps.Contains(c))
        {
            token = new Token() { type = TokenType.LeftUnaryOperator, data = $"{c}" };
            return true;
        }

        token = new Token() { type = TokenType.Invalid, data = "" };
        return false;
    }

    private static int StartNumberLength(string input)
    {
        bool hasSeenNegative = false;
        bool hasSeenPeriod = false;
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            switch (c)
            {
                case '-':
                    if (hasSeenNegative) throw new Exception("Received two negatives: " + input);
                    if (hasSeenPeriod) throw new Exception("Negative after period: " + input);
                    hasSeenNegative = true;
                    break;
                case '.':
                    if (hasSeenPeriod) throw new Exception("Received two periods");
                    hasSeenPeriod = true;
                    break;
                default:
                    if (c is >= '0' and <= '9') continue;
                    if ((i == 1 && (hasSeenPeriod || hasSeenNegative)) ||
                        (i == 2 && hasSeenPeriod && hasSeenNegative))
                        throw new Exception("Period/Negative and no digits");
                    return i; //alphabetic character; return
            }
        }
        return input.Length;
    }
}