using System.Collections.Frozen;

namespace Ti84App.Variable;

public class ListVariable : IVariable<RationalOrDecimal[]>
{
    public const char SmallL = 'ʟ';
    private static readonly char[] SubscriptChars = ['\u2080','\u2081','\u2082','\u2083','\u2084','\u2085','\u2086'];
    public static readonly FrozenSet<char> SubscriptSet;
    private readonly bool _calculatorDefined;
    private readonly byte _calcId;
    private readonly string _userDefinedName;

    static ListVariable()
    {
        SubscriptSet = SubscriptChars.ToFrozenSet();
    }

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
        _calculatorDefined = true;
        _calcId = calcId;
        _userDefinedName = "";
    }

    public ListVariable(string userName)
    {
        if (userName.Length > 5) throw new Exception("User defined name too long");
        _calculatorDefined = false;
        _calcId = 0;
        _userDefinedName = userName;
    }

    public string Name
    {
        get
        {
            if (_calculatorDefined) return "L" + SubscriptChars[_calcId];
            return _userDefinedName;
        }
    }

    public override string ToString()
    {
        if (_entries == null) return "undefined";
        return "{" + string.Join(' ', _entries) + "}";
    }

    public static bool TryGetName(string name, out string ret)
    {
        ret = "";
        if (name.StartsWith(SmallL))
        {
            bool isValid = name.Length <= 6;
            if (isValid) ret = name;
            return isValid;
        }

        if (name.Length == 2 && name[0] == 'L')
        {
            char c = name[1];

            if (c >= SubscriptChars[1] && c <= SubscriptChars[6])
            {
                ret = name;
                return true;
            }

            ret = SmallL + name;
            return true;
        }

        if (name.Length <= 5) ret = SmallL + name;
        return name.Length <= 5;
    }
}