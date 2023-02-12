using System;

namespace BobTheDiceMaster
{
  /// <summary>
  /// D6 - 6-sided die with each face represented by an integer from interval [1,6].
  /// Roll results are generated randomly.
  /// A seed for a random number generater can be optionally set using a
  /// <see cref="D6(int)">corresponding constructor</see>.
  /// </summary>
  public class D6 : IDie
  {
    private Random rng;

    public const int MaxValue = 6;

    /// <summary>
    /// A constructor with random seed.
    /// </summary>
    public D6()
    {
      rng = new Random();
    }

    /// <summary>
    /// A constructor with seed equal <paramref name="seed"/>
    /// </summary>
    public D6(int seed)
    {
      rng = new Random(seed);
    }

    /// <summary>
    /// Get a random integer from [1,6].
    /// </summary>
    public int Roll()
    {
      return rng.Next(1, MaxValue + 1);
    }


    /// <summary>
    /// Get an array of <paramref name="diceAmount"/> random integers from [1,6].
    /// </summary>
    public int[] Roll(int diceAmount)
    {
      int[] result = new int[diceAmount];
      for (int i = 0; i < diceAmount; ++i)
      {
        result[i] = Roll();
      }
      return result;
    }
  }
}
