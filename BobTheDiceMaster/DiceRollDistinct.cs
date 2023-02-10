﻿using System.Collections.Generic;
using System;
using System.Linq;

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

    public void Reroll(IReadOnlyCollection<int> diceIndexesToReroll, IDie die)
    {
      if (diceIndexesToReroll.Count > dice.Length)
      {
        throw new ArgumentException($"Can't reroll more than {dice.Length} dice for roll {this}.");
      }

      int[] rerollResult = die.Roll(diceIndexesToReroll.Count);

      for (int i = 0; i < diceIndexesToReroll.Count; ++i)
      {
        int dieIndex = diceIndexesToReroll.ElementAt(i);

        if (dieIndex < 0 || dieIndex >= dice.Length)
        {
          throw new ArgumentException(
            $"Can't reroll die {dieIndex}. Die number has to be between 0 and {dice.Length - 1} inclusively for roll {this}.");
        }
        dice[dieIndex] = rerollResult[dieIndex];
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
          $"Dice to reroll and dice result has to be of the same length, but was: diceIndexesToReroll({diceToReroll.Length}), rerollResult({rerollResult.DiceAmount})");
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
          $"Dice to reroll and dice result has to be of the same length, but was: diceIndexesToReroll({diceToReroll.Length}), rerollResult({rerollResult.Length})");
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

    public override bool Equals(object other)
    {
      var otherRoll = (DiceRollDistinct)other;

      if (dice.Length != otherRoll.DiceAmount)
      {
        return false;
      }

      for (int i = 0; i < dice.Length; ++i)
      {
        if (dice[i] != otherRoll[i])
        {
          return false;
        }
      }

      return true;
    }

    public override int GetHashCode()
    {
      int hash = 0;
      for (int i = 0; i < DiceAmount; ++i)
      {
        hash *= 6;
        hash += dice[i];
      }
      return hash;
    }

    public override string ToString()
    {
      return $"{nameof(DiceRollDistinct)}({String.Join(", ", dice)})";
    }
  }
}
