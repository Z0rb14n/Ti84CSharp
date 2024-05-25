namespace Ti84App;

public static class Tokenizer
{
    private static readonly HashSet<string> Operators =
    [
        "^", " nPr ", " nCr ", "*", "/", "+", "—", ">", "<", "=",
        "\u2260", // neq
        "\u2265", // geq
        "\u2264", // leq
        " xor ", " and ", " or "
    ];

    public const char EChar = 'e';
    public const char PiChar = 'π';

    private static readonly HashSet<string> RightUnaryOps = ["!","\u00b2"];
    private static readonly HashSet<char> NumericalConsts = [EChar, PiChar];
    private static readonly HashSet<char> SingleCharOps;
    private static readonly HashSet<string> MultiCharOps;

    static Tokenizer()
    {
        SingleCharOps = Operators.Where(op => op.Length == 1).Select(key => key[0])
            .ToHashSet();
        MultiCharOps = Operators.Where(op => op.Length > 1).ToHashSet();
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
        RightUnaryOperator
    }

    public struct Token
    {
        public TokenType type;
        public string data;

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
            if (right == prev.data.Length)
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
        else if (right == prev.startIndex + prev.data.Length)
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
                    int index = entry.data.IndexOf(str);
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
                foreach (string str in RightUnaryOps)
                {
                    int index = entry.data.IndexOf(str);
                    if (index == -1) continue;
                    didDoSomething = true;
                    Split(entry, todo, new Token() { data = str, type = TokenType.RightUnaryOperator },
                        entry.startIndex + index, entry.startIndex + index + str.Length);
                    if (entry.tokenized)
                    {
                        break;
                    }
                }

                if (entry.tokenized) continue;
                for (int j = 0; j < entry.data.Length; j++)
                {
                    if (IsCharToken(entry.data[j], out Token charToken))
                    {
                        Split(entry, todo, charToken, entry.startIndex+j, entry.startIndex+j+1);
                        didDoSomething = true;
                    }
                }
            }

            if (!didDoSomething || todo.Count == 0) break;
        }

        foreach (TokenNode node in todo)
        {
            if (node.data.Contains(' '))
                throw new Exception($"Node contains illegal spaces: {node.data}" + "," + inputString);
            if (float.TryParse(node.data, out _))
            {
                node.tokenized = true;
                node.token = new Token()
                {
                    type = TokenType.Number,
                    data = node.data
                };
                node.data = "";
            }
            else if (node.next is { tokenized: true, token.type: TokenType.LeftParen })
            {
                node.tokenized = true;
                node.token = new Token()
                {
                    type = TokenType.Function,
                    data = node.data
                };
                node.data = "";
            }
            else
            {
                node.tokenized = true;
                node.token = new Token()
                {
                    type = TokenType.Variable,
                    data = node.data
                };
                node.data = "";
            }
        }

        List<Token> tokens = new();
        TokenNode? curr = head;
        while (curr != null)
        {
            tokens.Add(curr.token);
            curr = curr.next;
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

        if (RightUnaryOps.Contains($"{c}"))
        {
            token = new Token() { type = TokenType.RightUnaryOperator, data = $"{c}" };
            return true;
        }

        token = new Token() { type = TokenType.Invalid, data = "" };
        return false;
    }
}