namespace Ti84App.Variable;

public class StringVariable(byte id) : IVariable<string>
{
    /// <summary>
    /// ID within range 0-9: i.e. Str0
    /// </summary>
    private readonly byte _id = id;

    private string? _value;

    public string Value
    {
        get
        {
            if (_value == null) throw new Exception($"String Variable {Name} is undefined.");
            return _value;
        }
        set => _value = value;
    }

    public string Name => $"Str{_id}";

    public override string ToString()
    {
        return _value ?? "undefined";
    }
}