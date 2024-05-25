namespace Ti84App.Operator;

public class GreaterThanOperator : BinaryMathOperator
{
    public override string[] Ids => [">"];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l > r ? 1 : 0;
    }
}

public class LessThanOperator : BinaryMathOperator
{
    public override string[] Ids => ["<"];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l < r ? 1 : 0;
    }
}

public class GeqOperator : BinaryMathOperator
{
    public override string[] Ids => ["\u2265"];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l >= r ? 1 : 0;
    }
}

public class LeqOperator : BinaryMathOperator
{
    public override string[] Ids => ["\u2264"];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l <= r ? 1 : 0;
    }
}