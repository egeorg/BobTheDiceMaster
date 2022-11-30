using System.Collections.Generic;
using System;

namespace BobTheDiceMaster
{
  public class DiceRollDistinct : IDiceRoll<DiceRollDistinct>
  {
    public DiceRoll Roll { get; private set; }

    public int DiceAmount => dice.Length;

    public int this[int i] => dice[i];

    private int[] dice;

    public DiceRollDistinct(int[] dice)
    {
      this.dice = (int[])dice.Clone();
      Roll = new DiceRoll(dice);
    }

    public void Reroll(IReadOnlyCollection<int> diceToReroll, IDie die)
    {
      if (diceToReroll.Count > dice.Length)
      {
        throw new ArgumentException($"Can't reroll more than {dice.Length} dice for roll {this}.");
      }

      foreach (var dieNumber in diceToReroll)
      {
        if (dieNumber < 0 || dieNumber >= dice.Length)
        {
          throw new ArgumentException(
            $"Can't reroll die {dieNumber}. Die number has to be between 0 and {dice.Length - 1} inclusively for roll {this}.");
        }
        dice[dieNumber] = die.Roll();
      }

      Roll = new DiceRoll(dice);
    }

    public DiceRollDistinct ApplyReroll(
      int[] diceToReroll, IDiceRoll<DiceRollDistinct> rerollResult)
    {
      int[] diceNew = (int[])dice.Clone();

      if (diceToReroll.Length != rerollResult.DiceAmount)
      {
        throw new ArgumentException(
          $"Dice to reroll and dice result has to be of the same length, but was: diceToReroll({diceToReroll.Length}), rerollResult({rerollResult.DiceAmount})");
      }

      for (int rerollCounter = 0; rerollCounter < diceToReroll.Length; rerollCounter++)
      {
        int dieNumber = diceToReroll[rerollCounter];
        if (dieNumber < 0 || dieNumber >= diceNew.Length)
        {
          throw new ArgumentException(
            $"Can't reroll die {dieNumber}. Die number has to be between 0 and {diceNew.Length - 1} inclusively for roll {this}.");
        }
        diceNew[dieNumber] = rerollResult[rerollCounter];
      }

      return new DiceRollDistinct(diceNew);
    }

    public DiceRollDistinct ApplyReroll(
      int[] diceToReroll, int[] rerollResult)
    {
      int[] diceNew = (int[])dice.Clone();

      if (diceToReroll.Length != rerollResult.Length)
      {
        throw new ArgumentException(
          $"Dice to reroll and dice result has to be of the same length, but was: diceToReroll({diceToReroll.Length}), rerollResult({rerollResult.Length})");
      }

      for (int rerollCounter = 0; rerollCounter < diceToReroll.Length; rerollCounter++)
      {
        int dieNumber = diceToReroll[rerollCounter];
        if (dieNumber < 0 || dieNumber >= diceNew.Length)
        {
          throw new ArgumentException(
            $"Can't reroll die {dieNumber}. Die number has to be between 0 and {diceNew.Length - 1} inclusively for roll {this}.");
        }
        diceNew[dieNumber] = rerollResult[rerollCounter];
      }

      return new DiceRollDistinct(diceNew);
    }

    public override string ToString()
    {
      return Roll.ToString();
    }
  }
}
