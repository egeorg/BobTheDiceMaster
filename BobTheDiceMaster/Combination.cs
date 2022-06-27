using System.Collections.Generic;
using System;

namespace BobTheDiceMaster
{
  public abstract class Combination
  {
    #region private methods
    public abstract double GetAverageProfit();
    public abstract double SingleRerollAverageProfit(DiceRoll roll, int[] diceToReroll);
    public abstract double TwoRerollAverageProfit(DiceRoll roll, int[] diceToReroll);
    #endregion

    #region public properties
    public double AverageProfit { get; }

    abstract public CombinationTypes CombinationType { get; }
    #endregion

    #region public methods
    public Combination()
    {
      AverageProfit = GetAverageProfit();
    }

    //abstract public double SecondTrySuccessProbability(DiceRoll current);

    //abstract public double ThirdTrySuccessProbability(DiceRoll current);
    #endregion
  }
}
