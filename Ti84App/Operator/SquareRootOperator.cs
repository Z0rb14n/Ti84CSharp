namespace Ti84App.Operator;

public class SquareRootOperator : UnaryMathOperator
{
    public override string[] Ids => ["\u221a"];
    protected override RationalOrDecimal Operate(RationalOrDecimal input)
    {
        // TODO RATIONAL SQUARE ROOT
        return new RationalOrDecimal
        {
            IsRational = false,
            NumericValue =
                (decimal)Math.Sqrt(input.IsRational ? (double)input.RationalValue : (double)input.NumericValue)
        };
    }
}