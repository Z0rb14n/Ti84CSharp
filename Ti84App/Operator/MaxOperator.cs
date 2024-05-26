namespace Ti84App.Operator;

public class VarargMaxOperator : AbstractOperator
{
    public override string[] Ids => ["max"];
    public override int Params => 1;

    private readonly UnaryMaxOperator _unaryMaxOperator = new();
    private readonly BinaryMaxOperator _binaryMaxOperator = new();

    public override void Evaluate(Stack<object> values)
    {
        throw new Exception("need parameter count");
    }
    
    public override void Evaluate(Stack<object> values, int paramCount)
    {
        if (values.Count < Params || values.Count < paramCount) throw new Exception($"Need {paramCount} items in Stack; got {values.Count}");
        if (paramCount > 2) throw new Exception($"Have more than 2 params {paramCount}");
        object[] arr = new object[paramCount];
        for (int i = arr.Length - 1; i >= 0; i--) arr[i] = values.Pop();
        object ret = EvaluateUnchecked(arr);
        values.Push(ret);
    }

    public override object EvaluateUnchecked(object[] values)
    {
        return values.Length switch
        {
            2 => _binaryMaxOperator.EvaluateUnchecked(values),
            1 => _unaryMaxOperator.EvaluateUnchecked(values),
            _ => throw new Exception("Vararg Max Operator received unchecked invalid count")
        };
    }
}

internal class UnaryMaxOperator : AbstractOperator
{
    public override string[] Ids => ["max"];
    public override int Params => 1;
    public override object EvaluateUnchecked(object[] values)
    {
        if (values[0] is RationalOrDecimal[] arr)
        {
            return arr.Max();
        }

        throw new Exception("Unary max requires array; receives " + values[0]);
    }
}

internal class BinaryMaxOperator : BinaryMathOperator
{
    public override string[] Ids => ["max"];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l >= r ? l : r;
    }
}