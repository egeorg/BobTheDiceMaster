﻿using System;

namespace BobTheDiceMaster
{
  public class D6 : IDie
  {
    #region private fields;
    private Random rng;
    #endregion

    #region public constants
    public const int MaxValue = 6;
    #endregion

    #region public methods
    public D6()
    {
      rng = new Random();
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
    #endregion
  }
}