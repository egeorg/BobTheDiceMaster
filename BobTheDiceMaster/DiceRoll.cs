using System;
using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class DiceRoll
  {
    #region private fields
    private static Random rng = new Random();

    private int[] dice = new int[DiceAmount];
    #endregion

    #region private methods
    private static int RollSingleDice()
    {
      return rng.Next(1, DieMaxValue + 1);
    }
    #endregion

    #region public constants and properties
    public int this[int i]
    {
      get {
        return dice[i];
      }
    }

    public const int DiceAmount = 5;

    public const int DieMaxValue = 6;
    #endregion

    #region public methods
    public DiceRoll(int[] dice)
    {
      if (dice.Length != DiceAmount)
      {
        throw new ArgumentException(
          $"Exactly {DiceAmount} dice expected, but was '{dice.Length}'");
      }

      for (int i = 0; i < DiceAmount; ++i)
      {
        if (dice[i] < 1 || dice[i] > DieMaxValue)
        {
          throw new ArgumentException(
            $"Die value has to be between 1 and {DieMaxValue}, but {i}-th value was '{dice[i]}'");
        }
        this.dice[i] = dice[i];
      }
    }

    public int Sum()
    {
      return dice.Sum();
    }

    public static DiceRoll GenerateNew()
    {
      int[] dice = new int[DiceAmount];

      for (int i = 0; i < DiceAmount; ++i)
      {
        dice[i] = RollSingleDice();
      }

      return new DiceRoll(dice);
    }

    public void Reroll(IReadOnlyCollection<int> diceToReroll)
    {
      if (diceToReroll.Count > DiceAmount)
      {
        throw new ArgumentException($"Can't reroll more than {DiceAmount} dice.");
      }

      foreach (var dieNumber in diceToReroll)
      {
        if (dieNumber < 0 || dieNumber >= DiceAmount)
        {
          throw new ArgumentException(
            $"Can't reroll die {dieNumber}. Die number has to be between 0 and {DiceAmount - 1} inclusively.");
        }
        dice[dieNumber] = RollSingleDice();
      }
    }
    #endregion
  }
}
