using static Ti84App.Tokenizer;

namespace Ti84App;

using static Tokenizer;
using static TokenType;

public static class ExpressionParser
{
    public static Dictionary<string, int> Precedence = new()
    {
        ["^"] = 5,
        [" nPr "] = 4,
        [" nCr "] = 4,
        ["*"] = 3,
        ["/"] = 3,
        ["+"] = 2,
        // so NEGATIVES use -
        // while minus uses the em dash — 
        ["—"] = 2,
        [">"] = 1,
        ["<"] = 1,
        ["="] = 1,
        ["\u2260"] = 1, // not equal to
        ["\u2265"] = 1, // geq
        ["\u2264"] = 1, // leq
        [" xor "] = 0,
        [" and "] = 0,
        [" or "] = 0,
    };

    public static HashSet<string> RightAssociative = ["^"];
    public static Queue<Token> Parse(string toRead)
    {
        Queue<Token> outputQueue = new();
        Stack<Token> operatorStack = new();
        IEnumerable<Token> tokens = GetTokens(toRead);
        foreach (Token t in tokens)
        {
            switch (t.type)
            {
                case Number:
                case String:
                case TokenType.Variable:
                    outputQueue.Enqueue(t);
                    break;
                case Function:
                case LeftParen:
                case RightUnaryOperator:
                    operatorStack.Push(t);
                    break;
                
                case BinaryOperator:
                    while (operatorStack.TryPeek(out Token topStack) && topStack.type != LeftParen &&
                           (Precedence[topStack.data] > Precedence[t.data] ||
                            (Precedence[topStack.data] == Precedence[t.data] && !RightAssociative.Contains(t.data))))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    operatorStack.Push(t);
                    break;
                case Comma:
                    while (operatorStack.TryPeek(out Token topStack) && topStack.type != LeftParen)
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    break;
                case RightParen:
                    bool foundLeftParen = false;
                    while (operatorStack.TryPeek(out Token topStack))
                    {
                        if (topStack.type == LeftParen)
                        {
                            foundLeftParen = true;
                            break;
                        }
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    if (!foundLeftParen)
                    {
                        throw new Exception("Mismatched parenthesis:" + toRead);
                    }

                    if (operatorStack.TryPeek(out Token sanity) && sanity.type != LeftParen)
                    {
                        throw new Exception("Sanity test failed: expected left paren but not on the top");
                    }

                    operatorStack.Pop();
                    if (operatorStack.TryPeek(out Token potentialFunc) && potentialFunc.type == Function)
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    break;
            }
        }

        while (operatorStack.TryPop(out Token remaining))
        {
            if (remaining.type == LeftParen)
            {
                throw new Exception("Mismatched parenthesis: " + toRead);
            }
            outputQueue.Enqueue(remaining);
        }

        return outputQueue;
    }
}