using System.Collections.Frozen;
using Ti84App.Operator;
using static Ti84App.Tokenizer;

namespace Ti84App;

using static Tokenizer;
using static TokenType;

public static class ExpressionParser
{
    private static readonly FrozenDictionary<string, int> Precedence = new Dictionary<string, int>
    {
        ["\u00b2"] = 7,
        ["!"] = 7,
        ["-"] = 6,
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
    }.ToFrozenDictionary();

    private static readonly FrozenSet<string> RightAssociative = FrozenSet.ToFrozenSet(["^"]);
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
                case LeftUnaryOperator:
                case CurlyLeftParen:
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
                    while (operatorStack.TryPeek(out Token topStack) && topStack.type is not (LeftParen or CurlyLeftParen))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    if (operatorStack.TryPeek(out Token testParen) && testParen.type is LeftParen or CurlyLeftParen)
                    {
                        testParen.count++;
                        operatorStack.Pop();
                        operatorStack.Push(testParen);
                    }

                    break;
                case CurlyRightParen:
                    bool foundCurlyLeftParen = false;
                    Token curlyLeftParen;
                    while (operatorStack.TryPeek(out curlyLeftParen))
                    {
                        if (curlyLeftParen.type == LeftParen)
                        {
                            throw new Exception("Mismatched parenthesis (expected {, got ( ): " + toRead);
                        }

                        if (curlyLeftParen.type == CurlyLeftParen)
                        {
                            foundCurlyLeftParen = true;
                            break;
                        }
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    if (!foundCurlyLeftParen)
                    {
                        throw new Exception("Mismatched parenthesis:" + toRead);
                    }

                    if (operatorStack.TryPeek(out Token curlySanity) && curlySanity.type != CurlyLeftParen)
                    {
                        throw new Exception("Sanity test failed: expected left paren but not on the top");
                    }

                    operatorStack.Pop();
                    
                    outputQueue.Enqueue(new Token() {type=Function, data=MakeListOperator.MakeListID, count = curlyLeftParen.count+1});
                    break;
                case RightParen:
                    bool foundLeftParen = false;
                    Token leftParen;
                    while (operatorStack.TryPeek(out leftParen))
                    {
                        if (leftParen.type == LeftParen)
                        {
                            foundLeftParen = true;
                            break;
                        }

                        if (leftParen.type == CurlyLeftParen)
                        {
                            throw new Exception("Mismatched parenthesis (expected (, got {): " + toRead);
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
                        potentialFunc.count = leftParen.count+1;
                        operatorStack.Pop();
                        outputQueue.Enqueue(potentialFunc);
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