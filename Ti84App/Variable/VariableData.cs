namespace Ti84App.Variable;

public class VariableData
{
    public readonly Dictionary<char, RealVariable> RealVars = new();
    public readonly Dictionary<int, StringVariable> StringVars = new();
    public readonly Dictionary<string, ListVariable> ListVars = new();

    public bool HasVariable(string name)
    {
        if (name.Length == 1) return RealVars.ContainsKey(name[0]);

        if (!name.StartsWith("Str")) return ListVars.ContainsKey(name);
        return name.Length == 4 && StringVars.ContainsKey(name[3] - '0');
    }

    public object GetVariableValue(string name)
    {
        if (name.Length == 1) return RealVars[name[0]].Value;
        if (!name.StartsWith("Str")) return ListVars[name].Value;
        if (name.Length == 4) return StringVars[name[3] - '0'].Value;
        throw new Exception("Variable name " + name + " not valid");
    }

    public bool TryGetVariableValue(string name, out object? value)
    {
        value = null;
        if (name.Length == 1)
        {
            bool has = RealVars.TryGetValue(name[0], out RealVariable rv);
            if (has) value = rv.Value;
            return has;
        }

        if (!name.StartsWith("Str"))
        {
            bool has = ListVars.TryGetValue(name, out ListVariable lv);
            if (has) value = lv.Value;
            return has;
        }

        if (name.Length == 4)
        {
            bool has = StringVars.TryGetValue(name[3] - '0', out StringVariable sv);
            if (has) value = sv.Value;
            return has;
        }

        return false;
    }

    public void SetVariableValue(string name, in object? value)
    {
        bool success = TrySetVariableValue(name, value);
        if (!success) throw new Exception($"Couldn't set variable {name} with value {value}");
    }

    public bool TrySetVariableValue(string name, in object? value)
    {
        if (name.Length == 1)
        {
            bool has = RealVars.TryGetValue(name[0], out RealVariable rv);
            if (!has) return false;
            if (value is RationalOrDecimal rat) rv.Value = rat;
            // reals are fine with setting invalid stuff apparently???
            if (value is RationalOrDecimal[] arr)
            {
                bool isValid = ListVariable.TryGetName(name, out string lvname);
                if (!isValid) return false;
                _ = ListVars.TryAdd(lvname, new ListVariable(name));
                ListVars[lvname].Value = arr;
            }
            return true;
        }

        if (name.StartsWith("Str"))
        {
            if (name.Length != 4) return false;
            bool has = StringVars.TryGetValue(name[3] - '0', out StringVariable sv);
            if (!has || value is not string str) return false;
            sv.Value = str;
            return true;
        }
        
        if (value is RationalOrDecimal[] rats)
        {
            bool isValid = ListVariable.TryGetName(name, out string lvname);
            if (!isValid) return false;
            _ = ListVars.TryAdd(lvname, new ListVariable(name));
            ListVars[name].Value = rats;
            return true;
        }

        return false;
    }

    public IVariable<T> GetVariable<T>(string name)
    {
        if (name.Length == 1)
        {
            if (typeof(T) == typeof(RationalOrDecimal)) return (RealVars[name[0]] as IVariable<T>)!;
            throw new Exception("Tried to get non-numeric value of likely numeric variable " + name);
        }

        if (name.StartsWith("Str"))
        {
            if (typeof(T) == typeof(string)) return (StringVars[name[3] - '0'] as IVariable<T>)!;
            throw new Exception("Tried to get non-string value of likely string variable " + name);
        }

        if (typeof(T) == typeof(RationalOrDecimal[])) return (ListVars[name] as IVariable<T>)!;
        throw new Exception("Tried to get non-list value of variable " + name);
    }

    public VariableData()
    {
        RealVars[RealVariable.Theta] = new RealVariable(RealVariable.Theta);
        for (char c = 'A'; c <= 'Z'; c++) RealVars[c] = new RealVariable(c);
        for (byte b = 0; b <= 9; b++) StringVars[b] = new StringVariable(b);
        for (byte b = 1; b <= 6; b++)
        {
            ListVariable lv = new(b);
            ListVars[lv.Name] = lv;
        }
    }
}