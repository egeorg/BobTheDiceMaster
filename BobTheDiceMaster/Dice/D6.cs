using System;

namespace BobTheDiceMaster
{
  public class D6 : IDie
  {
    private Random rng;

    public const int MaxValue = 6;

    public D6()
    {
      rng = new Random();
    }
    public D6(int seed)
    {
      rng = new Random(seed);
    }

    public int Roll()
    {
      return rng.Next(1, D6.MaxValue + 1);
    }
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
