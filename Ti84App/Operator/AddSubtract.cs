namespace Ti84App.Operator;

public class Addition : BinaryMathOperator
{
    public override string[] Ids => ["+"];
    public override object EvaluateUnchecked(object[] values)
    {
        object right = values[1];
        object left = values[0];

        if (right is string rs && left is string ls)
        {
            return ls + rs;
        }

        return base.EvaluateUnchecked(values);
    }

    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l + r;
    }
}

public class Subtraction : BinaryMathOperator
{
    public override string[] Ids => ["—"];

    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l - r;
    }
}