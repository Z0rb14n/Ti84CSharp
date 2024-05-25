namespace Ti84App.Variable;

public class RealVariable : IVariable<RationalOrDecimal>
{
    public const char Theta = 'θ';
    public readonly char VarName;

    public string Name => VarName.ToString();

    public RationalOrDecimal Value { get; set; } = 0;

    public RealVariable(char c)
    {
        if (c != Theta && c is < 'A' or > 'Z')
        {
            throw new Exception("Invalid Real variable Name: must be A-Z or theta (θ): " + c);
        }

        VarName = c;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}