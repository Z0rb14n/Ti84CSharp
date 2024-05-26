namespace Ti84App.Operator;

public class UnaryMaxOperator : AbstractOperator
{
    public override string[] Ids => ["max1"];
    public override int Params => 1;
    public override object EvaluateUnchecked(object[] values)
    {
        if (values[0] is RationalOrDecimal[] arr)
        {
            return arr.Max();
        }

        throw new Exception("Unary max requires array");
    }
}

public class BinaryMaxOperator : BinaryMathOperator
{
    public override string[] Ids => ["max"];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l >= r ? l : r;
    }
}