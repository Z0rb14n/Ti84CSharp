namespace Ti84App.Operator;

public class Equals : BinaryMathOperator
{
    public override string[] Ids => ["="];
    public override object EvaluateUnchecked(object[] values)
    {
        object v1 = values[1];
        object v0 = values[0];

        if (v1 is string rs && v0 is string ls)
        {
            return (RationalOrDecimal)(rs == ls ? 1 : 0);
        }

        return base.EvaluateUnchecked(values);
    }

    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l == r ? 1 : 0;
    }
}

public class NotEquals : BinaryMathOperator
{
    public override string[] Ids => ["\u2260"];
    public override object EvaluateUnchecked(object[] values)
    {
        object v1 = values[1];
        object v0 = values[0];

        if (v1 is string rs && v0 is string ls)
        {
            return (RationalOrDecimal)(rs != ls ? 1 : 0);
        }

        return base.EvaluateUnchecked(values);
    }

    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l != r ? 1 : 0;
    }
}