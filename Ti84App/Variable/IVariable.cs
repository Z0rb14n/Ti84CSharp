namespace Ti84App.Variable;

public interface IVariable<out T>
{
    public string Name { get; }
    public T Value { get; }
}