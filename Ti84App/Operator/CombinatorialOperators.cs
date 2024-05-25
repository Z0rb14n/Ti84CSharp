namespace Ti84App.Operator;

public class NChooseROperator : BinaryMathOperator
{
    public override string[] Ids => [" nCr "];

    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        if (!l.IsInteger || !r.IsInteger)
        {
            throw new Exception("nCr must have integers");
        }

        int leftInt = l.ToInt32();
        int rightInt = r.ToInt32();
        return Util.Factorial(leftInt) / (Util.Factorial(rightInt) * Util.Factorial(leftInt - rightInt));
    }
}

public class NPermuteROperator : BinaryMathOperator
{
    public override string[] Ids => [" nPr "];

    public override RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r)
    {
        if (!l.IsInteger || !r.IsInteger)
        {
            throw new Exception("nCr must have integers");
        }

        int leftInt = l.ToInt32();
        int rightInt = r.ToInt32();
        return Util.Factorial(leftInt) / Util.Factorial(leftInt - rightInt);
    }
}