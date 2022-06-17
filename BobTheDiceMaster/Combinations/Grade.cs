using System;

namespace BobTheDiceMaster.Combinations
{
  public abstract class Grade : Combination
  {
    protected abstract int GradeNumber { get; }

    public override double GetAverageProfit()
    {
      return GradeNumber * (5 * NSuccessProbability(5)
        + 4 * NSuccessProbability(4)
        + 3 * NSuccessProbability(3)
        + 2 * NSuccessProbability(2)
        + NSuccessProbability(1));
    }

    public double NSuccessProbability(int n)
    {
      return FirstRollNSuccessProbability(n)
        + SecondRollNSuccessProbability(n)
        + ThirdRollNSuccessProbability(n);
    }

    public double FirstRollNSuccessProbability(int n)
    {
      return NDiceRollKSuccessProbability(DiceRoll.DiceAmount, n);
    }

    public double SecondRollNSuccessProbability(int n)
    {
      double p = 0;
      for (int firstRollSuccessDice = 0;
        firstRollSuccessDice <= n;
        ++firstRollSuccessDice)
      {
        int secondRollSuccessDice = n - firstRollSuccessDice;
        p += NDiceRollKSuccessProbability(DiceRoll.DiceAmount, firstRollSuccessDice)
          * NDiceRollKSuccessProbability(
            DiceRoll.DiceAmount - firstRollSuccessDice,
            secondRollSuccessDice);
      }
      return p;
    }

    public double ThirdRollNSuccessProbability(int n)
    {
      double p = 0;
      for (int firstRollSuccessDice = 0;
        firstRollSuccessDice <= n;
        ++firstRollSuccessDice)
      {
        for (int secondRollSuccessDice = 0;
          secondRollSuccessDice <= n - firstRollSuccessDice;
          ++secondRollSuccessDice)
        {
          int thirdRollSuccessDice = n - firstRollSuccessDice - secondRollSuccessDice;
          p += NDiceRollKSuccessProbability(DiceRoll.DiceAmount, firstRollSuccessDice)
            * NDiceRollKSuccessProbability(
              DiceRoll.DiceAmount - firstRollSuccessDice,
              secondRollSuccessDice)
            * NDiceRollKSuccessProbability(
              DiceRoll.DiceAmount - firstRollSuccessDice - secondRollSuccessDice,
              thirdRollSuccessDice) ;
        }
      }
      return p;
    }

    public double NDiceRollKSuccessProbability(int n, int k)
    {
      return Combinatorics.Cnk(n, k)
        * Math.Pow(1.0 / 6, k)
        * Math.Pow(5.0 / 6, n - k);
    }
  }
}
