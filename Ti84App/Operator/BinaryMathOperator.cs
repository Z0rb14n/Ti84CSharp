namespace Ti84App.Operator;

public abstract class BinaryMathOperator : AbstractOperator
{
    public override int Params => 2;

    public override object EvaluateUnchecked(object[] values)
    {
        return HandleNumeric(values[0], values[1]);
    }

    protected virtual object HandleNumeric(object left, object right)
    {
        if (left is RationalOrDecimal l && right is RationalOrDecimal r)
        {
            return OpTwoRats(l, r);
        }

        if (left is RationalOrDecimal[] larr && right is RationalOrDecimal[] rarr)
        {
            if (rarr.Length != larr.Length) throw new Exception("Array dimensions must match");
            RationalOrDecimal[] ret = new RationalOrDecimal[rarr.Length];
            for (int i = 0; i < ret.Length; i++) ret[i] = OpTwoRats(larr[i], rarr[i]);
            return ret;
        } 
        if (left is RationalOrDecimal[] lar && right is RationalOrDecimal ri)
        {
            RationalOrDecimal[] ret = new RationalOrDecimal[lar.Length];
            for (int i = 0; i < ret.Length; i++) ret[i] = OpTwoRats(lar[i], ri);
            return ret;
        }

        if (left is RationalOrDecimal le && right is RationalOrDecimal[] rar)
        {
            RationalOrDecimal[] ret = new RationalOrDecimal[rar.Length];
            for (int i = 0; i < ret.Length; i++) ret[i] = OpTwoRats(le, rar[i]);
            return ret;
        }

        throw new Exception("Operands must be numeric");
    }

    public abstract RationalOrDecimal OpTwoRats(RationalOrDecimal l, RationalOrDecimal r);

}