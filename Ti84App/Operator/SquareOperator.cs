namespace Ti84App.Operator;

public class SquareOperator : UnaryMathOperator
{
    public override string[] Ids => ["\u00b2"];
    protected override RationalOrDecimal Operate(RationalOrDecimal input)
    {
        return input * input;
    }
}