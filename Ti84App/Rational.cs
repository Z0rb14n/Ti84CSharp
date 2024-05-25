using System.Globalization;
using System.Numerics;

namespace Ti84App;

public struct Rational : INumber<Rational>
{
    public BigInteger Numerator { get; private set; }
    public BigInteger Denominator { get; private set; }

    public static implicit operator Rational(int i)
    {
        return new Rational
        {
            Numerator = i,
            Denominator = BigInteger.One
        };
    }

    public static implicit operator Rational(BigInteger i)
    {
        return new Rational()
        {
            Numerator = i,
            Denominator = BigInteger.One
        };
    }

    public static explicit operator decimal(Rational rat)
    {
        return (decimal)rat.Numerator / (decimal)rat.Denominator;
    }

    public static explicit operator double(Rational rat)
    {
        return (double)rat.Numerator / (double)rat.Denominator;
    }
    
    public int CompareTo(object? obj)
    {
        if (obj is not Rational other) throw new ArgumentException(null, nameof(obj));
        return CompareTo(other);
    }

    public int CompareTo(Rational other)
    {
        return ((double)Numerator / (double)Denominator).CompareTo((double)other.Numerator / (double)other.Denominator);
    }

    public bool Equals(Rational other) => Numerator.Equals(other.Numerator) && Denominator.Equals(other.Denominator);

    public Rational Reduced
    {
        get
        {
            BigInteger gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);
            BigInteger numerator = BigInteger.Divide(Numerator, gcd);
            BigInteger denominator = BigInteger.Divide(Denominator, gcd);
            if (denominator.Sign < 0)
            {
                numerator *= BigInteger.MinusOne;
                denominator *= BigInteger.MinusOne;
            }
            return new Rational()
            {
                Numerator = numerator,
                Denominator = denominator
            };
        }
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        throw new NotImplementedException();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Rational Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Rational result)
    {
        throw new NotImplementedException();
    }

    public static Rational Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Rational result)
    {
        throw new NotImplementedException();
    }

    public static Rational operator +(Rational left, Rational right)
    {
        return new Rational
        {
            Numerator = (left.Numerator * right.Denominator) + (right.Numerator * left.Denominator),
            Denominator = left.Denominator * right.Denominator
        }.Reduced;
    }

    public static Rational AdditiveIdentity => Zero;
    public static bool operator ==(Rational left, Rational right)
    {
        Rational thisReduced = left.Reduced;
        Rational otherReduced = right.Reduced;

        return thisReduced.Numerator == otherReduced.Numerator && thisReduced.Denominator == otherReduced.Denominator;
    }

    public static bool operator !=(Rational left, Rational right)
    {
        Rational thisReduced = left.Reduced;
        Rational otherReduced = right.Reduced;

        return thisReduced.Numerator != otherReduced.Numerator || thisReduced.Denominator != otherReduced.Denominator;
    }

    public static bool operator >(Rational left, Rational right) => left.CompareTo(right) > 0;

    public static bool operator >=(Rational left, Rational right) => left.CompareTo(right) >= 0;

    public static bool operator <(Rational left, Rational right) => left.CompareTo(right) < 0;

    public static bool operator <=(Rational left, Rational right) => left.CompareTo(right) <= 0;

    public static Rational operator --(Rational value)
    {
        return value - One;
    }

    public static Rational operator /(Rational left, Rational right)
    {
        return new Rational()
        {
            Numerator = left.Numerator * right.Denominator,
            Denominator = left.Denominator * right.Numerator
        }.Reduced;
    }

    public static Rational operator ++(Rational value)
    {
        return value + One;
    }

    public static Rational operator %(Rational left, Rational right)
    {
        throw new NotImplementedException();
    }

    public static Rational MultiplicativeIdentity => One;
    public static Rational operator *(Rational left, Rational right)
    {
        BigInteger newNum = left.Numerator * right.Numerator;
        BigInteger newDenom = left.Denominator * right.Denominator;
        return new Rational()
        {
            Numerator = newNum,
            Denominator = newDenom
        }.Reduced;
    }

    public static Rational operator -(Rational left, Rational right)
    {
        return new Rational
        {
            Numerator = (left.Numerator * right.Denominator) - (right.Numerator * left.Denominator),
            Denominator = left.Denominator * right.Denominator
        }.Reduced;
    }

    public static Rational operator -(Rational value)
    {
        return (value with { Numerator = -value.Numerator }).Reduced;
    }

    public static Rational operator +(Rational value) => value;

    public static Rational Abs(Rational value)
    {
        return new Rational
        {
            Numerator = BigInteger.Abs(value.Numerator),
            Denominator = BigInteger.Abs(value.Denominator)
        };
    }

    public static bool IsCanonical(Rational value)
    {
        throw new NotImplementedException();
    }

    public static bool IsComplexNumber(Rational value)
    {
        return false;
    }

    public static bool IsEvenInteger(Rational value)
    {
        Rational reduced = value.Reduced;
        return reduced.Denominator == BigInteger.One && BigInteger.IsEvenInteger(reduced.Numerator);
    }

    public static bool IsFinite(Rational value)
    {
        return value.Denominator != BigInteger.Zero;
    }

    public static bool IsImaginaryNumber(Rational value)
    {
        return false;
    }

    public static bool IsInfinity(Rational value)
    {
        return value.Denominator == BigInteger.Zero && value.Numerator != BigInteger.Zero;
    }

    public static bool IsInteger(Rational value)
    {
        Rational reduced = value.Reduced;
        return reduced.Denominator == BigInteger.One;
    }

    public static bool IsNaN(Rational value)
    {
        return value.Denominator == BigInteger.Zero && value.Numerator == BigInteger.Zero;
    }

    public static bool IsNegative(Rational value)
    {
        return value.Denominator.Sign * value.Numerator.Sign < 0;
    }

    public static bool IsNegativeInfinity(Rational value)
    {
        return value.Denominator == BigInteger.Zero && value.Numerator < 0;
    }

    public static bool IsNormal(Rational value)
    {
        throw new NotImplementedException();
    }

    public static bool IsOddInteger(Rational value)
    {
        Rational reduced = value.Reduced;
        return reduced.Denominator == BigInteger.One && BigInteger.IsOddInteger(reduced.Numerator);
    }

    public static bool IsPositive(Rational value)
    {
        return value.Denominator.Sign * value.Numerator.Sign > 0;
    }

    public static bool IsPositiveInfinity(Rational value)
    {
        return value.Denominator == BigInteger.Zero && value.Numerator > 0;
    }

    public static bool IsRealNumber(Rational value)
    {
        return true;
    }

    public static bool IsSubnormal(Rational value)
    {
        throw new NotImplementedException();
    }

    public static bool IsZero(Rational value)
    {
        return value.Numerator == BigInteger.Zero && value.Denominator != BigInteger.Zero;
    }

    public static Rational MaxMagnitude(Rational x, Rational y)
    {
        throw new NotImplementedException();
    }

    public static Rational MaxMagnitudeNumber(Rational x, Rational y)
    {
        return MaxMagnitude(x, y);
    }

    public static Rational MinMagnitude(Rational x, Rational y)
    {
        throw new NotImplementedException();
    }

    public static Rational MinMagnitudeNumber(Rational x, Rational y)
    {
        return MinMagnitude(x, y);
    }

    public static Rational Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Rational Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromChecked<TOther>(TOther value, out Rational result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, out Rational result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, out Rational result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToChecked<TOther>(Rational value, out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToSaturating<TOther>(Rational value, out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToTruncating<TOther>(Rational value, out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Rational result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Rational result)
    {
        throw new NotImplementedException();
    }

    public static Rational One =>
        new()
        {
            Numerator = BigInteger.One,
            Denominator = BigInteger.One
        };

    public static int Radix => 10;

    public static Rational Zero => new()
    {
        Numerator = BigInteger.Zero,
        Denominator = BigInteger.One
    };

    public static Rational NegativeOne => new()
    {
        Numerator = BigInteger.MinusOne,
        Denominator = BigInteger.One
    };

    public static int ToInt32(Rational rationalValue)
    {
        return Decimal.ToInt32(Math.Round((decimal)rationalValue.Numerator/(decimal)rationalValue.Denominator));
    }

    public override string ToString()
    {
        return ((double)this).ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is Rational rational && Equals(rational);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Numerator, Denominator);
    }
}