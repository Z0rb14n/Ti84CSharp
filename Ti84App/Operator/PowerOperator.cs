namespace Ti84App.Operator;

public class PowerOperator : BinaryMathOperator
{
    public override string[] Ids => ["^"];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return RationalOrDecimal.Pow(l, r);
    }
}