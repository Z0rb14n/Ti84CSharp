namespace Ti84App.Operator;

public class LogicalAndOperator : BinaryMathOperator
{
    public override string[] Ids => [" and "];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l != 0 && r != 0 ? 1 : 0;
    }
}

public class LogicalOrOperator : BinaryMathOperator
{
    public override string[] Ids => [" or "];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l != 0 || r != 0 ? 1 : 0;
    }
}

public class LogicalXorOperator : BinaryMathOperator
{
    public override string[] Ids => [" xor "];

    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l != 0 ^ r != 0 ? 1 : 0;
    }
}