namespace Ti84App.Operator;

public class NegationOperator : UnaryMathOperator
{
    public override string[] Ids => ["-"];
    protected override RationalOrDecimal Operate(RationalOrDecimal input)
    {
        return -1*input;
    }
}