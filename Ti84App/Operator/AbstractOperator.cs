namespace Ti84App.Operator;

public abstract class AbstractOperator
{
    public abstract string[] Ids { get; }
    public abstract int Params { get; }

    public virtual void Evaluate(Stack<object> values)
    {
        if (values.Count < Params) throw new Exception($"Need {Params} items in Stack; got {values.Count}");
        object[] arr = new object[Params];
        for (int i = arr.Length - 1; i >= 0; i--) arr[i] = values.Pop();
        object ret = EvaluateUnchecked(arr);
        values.Push(ret);
    }
    
    public virtual void Evaluate(Stack<object> values, int count)
    {
        Evaluate(values);
    }

    public abstract object EvaluateUnchecked(object[] values);
}