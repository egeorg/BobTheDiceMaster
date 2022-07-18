using System;

namespace BobTheDiceMaster.Combinations
{
  public abstract class GradeCombinatoric
  {
    protected abstract int GradeNumber { get; }

    public double GetAverageProfit()
    {
      return GradeNumber * (5 * FirstRollNSuccessProbability(5)
        + 5 * SecondRollNSuccessProbability(5)
        + 5 * ThirdRollNSuccessProbability(5)
        + 4 * ThirdRollNSuccessProbability(4)
        + 3 * ThirdRollNSuccessProbability(3)
        + 2 * ThirdRollNSuccessProbability(2)
        + ThirdRollNSuccessProbability(1));
    }

    public double SingleRerollAverageProfit(DiceRoll roll)
    {
      int wrongDiceAmount = 0;
      for (int i = 0; i < DiceRoll.MaxDiceAmount; ++i)
      {
        if (roll[i] != GradeNumber)
        {
          ++wrongDiceAmount;
        }
      }

      int correctDiceAmount = DiceRoll.MaxDiceAmount - wrongDiceAmount;

      double result = 0;
      for (int i = 0; correctDiceAmount + i <= DiceRoll.MaxDiceAmount; ++i)
      {
        result += GradeNumber * (correctDiceAmount + i) * NDiceRollKSuccessProbability(wrongDiceAmount, i);
      }

      return result;
    }

    public double SingleRerollAverageProfit(DiceRoll roll, int[] diceToReroll)
    {
      int correctDiceAmount = 0;
      for (int i = 0; i < DiceRoll.MaxDiceAmount; ++i)
      {
        if (roll[i] == GradeNumber)
        {
          ++correctDiceAmount;
        }
      }

      for (int i = 0; i < diceToReroll.Length; ++i)
      {
        if (roll[diceToReroll[i]] == GradeNumber)
        {
          --correctDiceAmount;
        }
      }

      double result = 0;
      for (int i = 0; i <= diceToReroll.Length; ++i)
      {
        result += GradeNumber * (correctDiceAmount + i) * NDiceRollKSuccessProbability(diceToReroll.Length, i);
      }

      return result;
    }

    public double TwoRerollAverageProfit(DiceRoll roll, int[] diceToReroll)
    {
      int correctDiceAmount = 0;
      for (int i = 0; i < DiceRoll.MaxDiceAmount; ++i)
      {
        if (roll[i] == GradeNumber)
        {
          ++correctDiceAmount;
        }
      }

      for (int i = 0; i < diceToReroll.Length; ++i)
      {
        if (roll[diceToReroll[i]] == GradeNumber)
        {
          --correctDiceAmount;
        }
      }

      // Situation 1: all dice are GradeNumber after a single reroll.
      double result = GradeNumber * (correctDiceAmount + diceToReroll.Length) * NDiceRollKSuccessProbability(diceToReroll.Length, diceToReroll.Length);

      // Situation 2: i dice rerolled as GradeNumber after a single reroll,
      // j dice rerolled as GradeNumber after a second reroll.
      for (int i = 0; i < diceToReroll.Length; ++i)
      {
        for (int j = 0; i + j <= diceToReroll.Length; ++j)
        {
          result += GradeNumber * (correctDiceAmount + i + j)
            * NDiceRollKSuccessProbability(diceToReroll.Length, i)
            * NDiceRollKSuccessProbability(diceToReroll.Length - i, j);
        }
      }

      return result;
    }

    public double NSuccessProbability(int n)
    {
      return FirstRollNSuccessProbability(n)
        + SecondRollNSuccessProbability(n)
        + ThirdRollNSuccessProbability(n);
    }

    public double FirstRollNSuccessProbability(int n)
    {
      return NDiceRollKSuccessProbability(DiceRoll.MaxDiceAmount, n);
    }

    public double SecondRollNSuccessProbability(int n)
    {
      double p = 0;
      // Inequality is not strict to make sure that only second roll yields to n success
      for (int firstRollSuccessDice = 0;
        firstRollSuccessDice < n;
        ++firstRollSuccessDice)
      {
        int secondRollSuccessDice = n - firstRollSuccessDice;
        p += NDiceRollKSuccessProbability(DiceRoll.MaxDiceAmount, firstRollSuccessDice)
          * NDiceRollKSuccessProbability(
              DiceRoll.MaxDiceAmount - firstRollSuccessDice,
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
        if (firstRollSuccessDice == DiceRoll.MaxDiceAmount)
        {
          continue;
        }
        for (int secondRollSuccessDice = 0;
          secondRollSuccessDice <= n - firstRollSuccessDice;
          ++secondRollSuccessDice)
        {

          if (secondRollSuccessDice == DiceRoll.MaxDiceAmount - firstRollSuccessDice)
          {
            continue;
          }
          int thirdRollSuccessDice = n - firstRollSuccessDice - secondRollSuccessDice;
          p += NDiceRollKSuccessProbability(DiceRoll.MaxDiceAmount, firstRollSuccessDice)
            * NDiceRollKSuccessProbability(
              DiceRoll.MaxDiceAmount - firstRollSuccessDice,
              secondRollSuccessDice)
            * NDiceRollKSuccessProbability(
              DiceRoll.MaxDiceAmount - firstRollSuccessDice - secondRollSuccessDice,
              thirdRollSuccessDice);
        }
      }
      return p;
    }

    public double ExactSecondRollNSuccessProbability(int n)
    {
      double p = 0;
      // Inequality is not strict to make sure that only second roll yields to n success
      for (int firstRollSuccessDice = 0;
        firstRollSuccessDice < n;
        ++firstRollSuccessDice)
      {
        int secondRollSuccessDice = n - firstRollSuccessDice;
        p += NDiceRollKSuccessProbability(DiceRoll.MaxDiceAmount, firstRollSuccessDice)
          * NDiceRollKSuccessProbability(
            DiceRoll.MaxDiceAmount - firstRollSuccessDice,
            secondRollSuccessDice);
      }
      return p;
    }

    public double ExactThirdRollNSuccessProbability(int n)
    {
      double p = 0;
      for (int firstRollSuccessDice = 0;
        firstRollSuccessDice < n;
        ++firstRollSuccessDice)
      {
        for (int secondRollSuccessDice = 0;
          secondRollSuccessDice < n - firstRollSuccessDice;
          ++secondRollSuccessDice)
        {
          int thirdRollSuccessDice = n - firstRollSuccessDice - secondRollSuccessDice;
          p += NDiceRollKSuccessProbability(DiceRoll.MaxDiceAmount, firstRollSuccessDice)
            * NDiceRollKSuccessProbability(
              DiceRoll.MaxDiceAmount - firstRollSuccessDice,
              secondRollSuccessDice)
            * NDiceRollKSuccessProbability(
              DiceRoll.MaxDiceAmount - firstRollSuccessDice - secondRollSuccessDice,
              thirdRollSuccessDice);
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
