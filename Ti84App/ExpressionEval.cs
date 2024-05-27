using System.Diagnostics;
using Ti84App.Operator;
using Ti84App.Variable;

namespace Ti84App;

using static Tokenizer.TokenType;

public static class ExpressionEval
{
    private static readonly AbstractOperator?[] Operators =
    [
        new Addition(),
        new Subtraction(),
        new Equals(),
        new NotEquals(),
        new GreaterThanOperator(),
        new LessThanOperator(),
        new GeqOperator(),
        new LeqOperator(),
        new NChooseROperator(),
        new NPermuteROperator(),
        new PowerOperator(),
        new Multiplication(),
        new Division(),
        new LogicalAndOperator(),
        new LogicalOrOperator(),
        new LogicalXorOperator(),
        new LogicalNotOperator(),
        new FactorialOperator(),
        new SquareOperator(),
        new VarargMaxOperator(),
        new MakeListOperator(),
        new NegationOperator()
    ];
    public static Dictionary<string, AbstractOperator> ops;

    static ExpressionEval()
    {
        ops = new Dictionary<string, AbstractOperator>();
        foreach (AbstractOperator? op in Operators) {
        {
            if (op == null)
            {
                Debug.WriteLine("Found null operator? skipping.");
                continue;
            }
            foreach (string id in op.Ids) ops.Add(id, op);
        }}
    }
    
    public static object Evaluate(Queue<Tokenizer.Token> tokens, in VariableData variables)
    {
        Stack<object> valueStack = new();
        foreach (Tokenizer.Token t in tokens)
        {
            switch (t.type)
            {
                case Number:
                    valueStack.Push( RationalOrDecimal.Parse(t.data));
                    break;
                case String:
                    valueStack.Push(t.data);
                    break;
                case Tokenizer.TokenType.Variable:
                    if (variables.TryGetVariableValue(t.data, out object? val) && val != null) valueStack.Push(val);
                    else throw new Exception($"Variable {t.data} not present.");
                    break;
                case LeftParen:
                case RightParen:
                case Comma:
                case Invalid:
                    throw new Exception("Cannot evaluate this token type: " + t);
                case Function:
                case BinaryOperator:
                case RightUnaryOperator:
                case LeftUnaryOperator:
                    if (ops.TryGetValue(t.data, out AbstractOperator? op)) op.Evaluate(valueStack, t.count);
                    else throw new Exception("Don't have this function/operator: " + t);
                    break;
            }
        }

        if (valueStack.Count != 1) throw new Exception("Resulting value stack is not single entry");
        return valueStack.Pop();
    }
}