namespace Ti84App.Operator;

public class MakeListOperator : AbstractOperator
{
    public const string MakeListID = "MakeList";
    public override string[] Ids => [MakeListID];
    public override int Params => 0;
    
    public override void Evaluate(Stack<object> values)
    {
        throw new Exception("need parameter count");
    }
    
    public override void Evaluate(Stack<object> values, int paramCount)
    {
        if (values.Count < paramCount || values.Count < paramCount) throw new Exception($"Need {Params} items in Stack; got {values.Count}");
        object[] arr = new object[paramCount];
        for (int i = arr.Length - 1; i >= 0; i--) arr[i] = values.Pop();
        object ret = EvaluateUnchecked(arr);
        values.Push(ret);
    }

    public override object EvaluateUnchecked(object[] values)
    {
        RationalOrDecimal[] rats = new RationalOrDecimal[values.Length];
        for (int i = 0; i < rats.Length; i++)
        {
            if (values[i] is RationalOrDecimal rat)
            {
                rats[i] = rat;
            }
            else
            {
                throw new Exception("List must contain real numbers; received " + values[i]);
            }
        }

        return rats;
    }
}