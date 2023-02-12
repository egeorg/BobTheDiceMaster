using System;

namespace BobTheDiceMaster
{
  /// <summary>
  /// Static class with combinatorial functions.
  /// </summary>
  public static class Combinatorics
  {
    private const int factorialMaxArgument = 20;

    /// <summary>
    /// Calculate number of <paramref name="k"/>-combinations of <paramref name="n"/s>
    /// elements, aka binomial coefficients.
    /// This method is rather dumb and it's not suitable for big numbers.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Arguments are too big and internal computation may result in an overflow.
    /// </exception>
    public static long Cnk(int n, int k)
    {
      if (n > factorialMaxArgument)
      {
        // There are some situations when it's fine to have big n (e.g. if n == k), but
        // they should not appear in a program like this, it's fine to have a restriction like this.
        throw new ArgumentException(
          $"{nameof(n)} has to be less or equal to {factorialMaxArgument}, otherwise internal {nameof(Cnk)} internal computation may result in an overflow",
          nameof(n));
      }

      if (n < 0)
      {
        throw new ArgumentException(
          $"{nameof(n)} has to be positive, otherwise {nameof(Cnk)} does not makes sense.",
          nameof(n));
      }

      if (k < 0 || k > n)
      {
        throw new ArgumentException(
          $"{nameof(k)} has to be between 0 and {nameof(n)}, otherwise {nameof(Cnk)} does not makes sense.",
          nameof(k));
      }

      long result = 1;

      // Calculate n!/k!
      for (int i = n; i > k; --i)
      {
        result *= i;
      }

      return result / Factorial(n - k);
    }

    /// <summary>
    /// Calculate <paramref name="n"/> factorial.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Argument is too big and internal computation may result in an overflow.
    /// </exception>
    public static long Factorial(int n)
    {
      if (n < 0)
      {
        throw new ArgumentException(
          $"{nameof(n)} has to be positive, otherwise {nameof(Factorial)} does not makes sense.",
          nameof(n));
      }

      if (n > factorialMaxArgument)
      {
        throw new ArgumentException(
          $"{nameof(n)} was {n}, but it has to be less or equal to {factorialMaxArgument}, otherwise its result would overflow a \"long\" .NET type",
          nameof(n));
      }

      long result = 1;
      for (int i = n; i > 0; --i)
      {
        result *= i;
      }
      return result;
    }
  }
}
