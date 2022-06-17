using System.Collections.Generic;
using System;

namespace BobTheDiceMaster
{
  public abstract class Combination
  {
    #region private methods
    public virtual double GetAverageProfit()
    {
      //int sum = SuccessfulThrows.Sum(x => x.Sum());
      //return sum * (FirstTrySuccessProbability() * 2 + SecondTrySuccessProbability() + ThirdTrySuccessProbability());
      throw new NotImplementedException();
    }
    #endregion

    #region public properties
    public double AverageProfit { get; }

    abstract public CombinationTypes CombinationType { get; }

    //abstract public IReadOnlyList<DiceRoll> SuccessfulThrows { get; }
    #endregion

    //#region protected methods
    //abstract protected double FirstTrySuccessProbability();

    //abstract protected double SecondTrySuccessProbability();

    //abstract protected double ThirdTrySuccessProbability();
    //#endregion

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
