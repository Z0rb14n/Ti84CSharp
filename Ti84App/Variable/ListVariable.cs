namespace Ti84App.Variable;

public class ListVariable : IVariable<RationalOrDecimal[]>
{
    public const char SmallL = 'ʟ';
    private static readonly char[] SubscriptChars = ['\u2080','\u2081','\u2082','\u2083','\u2084','\u2085','\u2086'];
    public readonly bool CalculatorDefined;
    public readonly byte CalcId;
    public readonly string UserDefinedName;

    private RationalOrDecimal[]? _entries;

    public RationalOrDecimal[] Value
    {
        get
        {
            if (_entries == null) throw new Exception($"String Variable {Name} is undefined.");
            return _entries;
        }
        set => _entries = value;
    }

    public ListVariable(byte calcId)
    {
        if (calcId is < 1 or > 6) throw new Exception($"CalcID out of range: needed [1,6]; got {calcId}");
        CalculatorDefined = true;
        CalcId = calcId;
        UserDefinedName = "";
    }

    public ListVariable(string userName)
    {
        if (userName.Length > 5) throw new Exception("User defined name too long");
        CalculatorDefined = false;
        CalcId = 0;
        UserDefinedName = userName;
    }

    public string Name
    {
        get
        {
            if (CalculatorDefined) return "L" + SubscriptChars[CalcId];
            return UserDefinedName;
        }
    }

    public override string ToString()
    {
        if (_entries == null) return "undefined";
        return "{" + string.Join(' ', _entries) + "}";
    }
}