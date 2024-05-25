using System.Numerics;

namespace Ti84App;

public static class Util
{
    public static string[] TrimAll(IEnumerable<string> input) => input.Select(str => str.Trim()).ToArray();
    
    public static BigInteger Factorial(BigInteger n)
    {
        if (n < 0) throw new Exception("Factorial requires non-negative integers; got " + n);
        BigInteger ret = BigInteger.One;
        for (BigInteger i = BigInteger.One; i <= n; i++) ret *= i;
        return ret;
    }
}