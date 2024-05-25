namespace Ti84App.Operator;

public class FactorialOperator : UnaryMathOperator
{
    public override string[] Ids => ["!"];

    protected override RationalOrDecimal Operate(RationalOrDecimal input)
    {
        if (!input.IsInteger)throw new Exception("Factorial requires integer types");
        return Util.Factorial(input.ToInt32());
    }
}