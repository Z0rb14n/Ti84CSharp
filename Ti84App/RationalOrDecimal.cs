using System.Numerics;

namespace Ti84App;

public struct RationalOrDecimal : IComparable<RationalOrDecimal>
{

    public bool IsRational;
    public Rational RationalValue;
    public decimal NumericValue;

    public static RationalOrDecimal operator *(RationalOrDecimal r1, RationalOrDecimal r2)
    {
        if (r1.IsRational && r2.IsRational)
        {
            return new RationalOrDecimal()
            {
                IsRational = true,
                RationalValue = r1.RationalValue * r2.RationalValue
            };
        }
        return new RationalOrDecimal()
        {
            IsRational = false,
            NumericValue = (r1.IsRational ? (decimal)r1.RationalValue : r1.NumericValue) *
                           (r2.IsRational ? (decimal)r2.RationalValue : r2.NumericValue)
        };
    }
    public static RationalOrDecimal operator /(RationalOrDecimal r1, RationalOrDecimal r2)
    {
        if (r1.IsRational && r2.IsRational)
        {
            return new RationalOrDecimal()
            {
                IsRational = true,
                RationalValue = r1.RationalValue / r2.RationalValue
            };
        }
        return new RationalOrDecimal()
        {
            IsRational = false,
            NumericValue = (r1.IsRational ? (decimal)r1.RationalValue : r1.NumericValue) /
                           (r2.IsRational ? (decimal)r2.RationalValue : r2.NumericValue)
        };
    }

    public static RationalOrDecimal operator +(RationalOrDecimal r1, RationalOrDecimal r2)
    {
        if (r1.IsRational && r2.IsRational)
            return new RationalOrDecimal()
            {
                IsRational = true,
                RationalValue = r1.RationalValue + r2.RationalValue
            };
        return new RationalOrDecimal()
        {
            IsRational = false,
            NumericValue = (r1.IsRational ? (decimal)r1.RationalValue : r1.NumericValue) +
                           (r2.IsRational ? (decimal)r2.RationalValue : r2.NumericValue)
        };
    }
    
    public static RationalOrDecimal operator -(RationalOrDecimal r1, RationalOrDecimal r2)
    {
        if (r1.IsRational && r2.IsRational)
            return new RationalOrDecimal()
            {
                IsRational = true,
                RationalValue = r1.RationalValue - r2.RationalValue
            };
        return new RationalOrDecimal()
        {
            IsRational = false,
            NumericValue = (r1.IsRational ? (decimal)r1.RationalValue : r1.NumericValue) -
                           (r2.IsRational ? (decimal)r2.RationalValue : r2.NumericValue)
        };
    }

    public static bool operator ==(RationalOrDecimal r1, RationalOrDecimal r2)
    {
        return r1.IsRational switch
        {
            true when r2.IsRational => r1.RationalValue == r2.RationalValue,
            false when !r2.IsRational => r1.NumericValue == r2.NumericValue,
            _ => false
        };
    }

    public static bool operator !=(RationalOrDecimal r1, RationalOrDecimal r2)
    {
        return !(r1 == r2);
    }

    public static bool operator==(RationalOrDecimal rat, int i)
    {
        if (rat.IsRational) return rat.RationalValue == i;
        return rat.NumericValue == i;
    }

    public static bool operator !=(RationalOrDecimal rat, int i)
    {
        return !(rat == i);
    }

    public static bool operator >=(RationalOrDecimal r1, RationalOrDecimal r2)
    {
        return r1 == r2 || r1 > r2;
    }

    public static bool operator <=(RationalOrDecimal r1, RationalOrDecimal r2)
    {
        return r1 == r2 || r1 < r2;
    }

    public static bool operator >(RationalOrDecimal r1, RationalOrDecimal r2)
    {
        return (r1.IsRational ? (decimal)r1.RationalValue : r1.NumericValue) >
               (r2.IsRational ? (decimal)r2.RationalValue : r2.NumericValue);
    }

    public static bool operator <(RationalOrDecimal r1, RationalOrDecimal r2)
    {
        return (r1.IsRational ? (decimal)r1.RationalValue : r1.NumericValue) <
               (r2.IsRational ? (decimal)r2.RationalValue : r2.NumericValue);
    }

    public static implicit operator RationalOrDecimal(int i)
    {
        return new RationalOrDecimal()
        {
            IsRational = true,
            RationalValue = i
        };
    }

    public static implicit operator RationalOrDecimal(Rational rat)
    {
        return new RationalOrDecimal()
        {
            IsRational = true,
            RationalValue = rat
        };
    }
    
    public static implicit operator RationalOrDecimal(BigInteger bi)
    {
        return new RationalOrDecimal()
        {
            IsRational = true,
            RationalValue = bi
        };
    }
    
    public static implicit operator RationalOrDecimal(decimal d)
    {
        return new RationalOrDecimal()
        {
            IsRational = false,
            NumericValue = d
        };
    }

    public static explicit operator decimal(RationalOrDecimal rat)
    {
        if (rat.IsRational) return (decimal)rat.RationalValue;
        return rat.NumericValue;
    }

    public bool IsInteger => IsRational ? Rational.IsInteger(RationalValue) : Decimal.IsInteger(NumericValue);

    public int ToInt32()
    {
        return IsRational ? Rational.ToInt32(RationalValue) : Decimal.ToInt32(NumericValue);
    }

    public override string ToString()
    {
        return IsRational ? RationalValue.ToString() : NumericValue.ToString();
    }

    public bool Equals(RationalOrDecimal other)
    {
        return IsRational == other.IsRational && RationalValue.Equals(other.RationalValue) && NumericValue == other.NumericValue;
    }

    public int CompareTo(RationalOrDecimal other)
    {
        return ((decimal)this).CompareTo((decimal)other);
    }

    public override bool Equals(object? obj)
    {
        return obj is RationalOrDecimal other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsRational, RationalValue, NumericValue);
    }
    public static RationalOrDecimal Parse(string str)
    {
        if (BigInteger.TryParse(str, out BigInteger res))
            return new RationalOrDecimal { IsRational = true, RationalValue = res };
        if (str == $"{Tokenizer.EChar}") return new RationalOrDecimal() { IsRational = false, NumericValue = (decimal)Math.E };
        if (str == $"{Tokenizer.PiChar}") return new RationalOrDecimal() { IsRational = false, NumericValue = (decimal)Math.PI };
        return new RationalOrDecimal { IsRational = false, NumericValue = decimal.Parse(str) };
    }

    public static RationalOrDecimal Pow(RationalOrDecimal b, RationalOrDecimal exp)
    {
        // TODO SUPPORT RATIONAL EXPONENTIATION
        return (RationalOrDecimal)Math.Pow(b.IsRational ? (double)b.RationalValue : (double)b.NumericValue,
            exp.IsRational ? (double)exp.RationalValue : (double) exp.NumericValue);
    }
}