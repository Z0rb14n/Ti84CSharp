namespace Ti84App.Operator;

public class Multiplication : BinaryMathOperator
{
    public override string[] Ids => ["*"];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l * r;
    }
}

public class Division : BinaryMathOperator
{
    public override string[] Ids => ["/"];
    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        return l / r;
    }
}