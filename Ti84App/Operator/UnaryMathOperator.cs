namespace Ti84App.Operator;

public abstract class UnaryMathOperator : AbstractOperator
{
    public override int Params => 1;

    public override object EvaluateUnchecked(object[] values)
    {
        return HandleNumeric(values[0]);
    }

    protected virtual object HandleNumeric(object left)
    {
        return left switch
        {
            RationalOrDecimal l => Operate(l),
            RationalOrDecimal[] larr => larr.Select(Operate).ToArray(),
            _ => throw new Exception("Operands must be numeric")
        };
    }

    protected abstract RationalOrDecimal Operate(RationalOrDecimal input);
}