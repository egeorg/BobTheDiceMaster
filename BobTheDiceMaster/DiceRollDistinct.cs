﻿using System;
using System.Linq;

namespace BobTheDiceMaster
{
  /// <summary>
  /// Represents a single roll of <see cref="DiceAmount"/> number of dice,
  /// up to <see cref="MaxDiceAmount"/>.
  /// Unlike <see cref="DiceRoll"> it keeps dice order and
  /// can be used to compare two rolls taking dice order into acount.
  /// Because of that it's useful for UI where you can bind each dice
  /// in a roll to certain control and they will not get mixed up.
  /// </summary>
  /// <example>
  /// var roll1 = new DiceRollDistinct(new[] {1, 2, 3, 4, 5});
  /// var roll2 = new DiceRollDistinct(new[] {5, 4, 3, 2, 1});
  /// Console.WriteLine(roll1 == roll2); // "false"
  /// </example>
  public class DiceRollDistinct
  {
    /// <summary>
    /// A <see cref="DiceRoll"> with the same values. Can be useful,
    /// for example to calculate <see cref="DiceRoll.Score"/>.
    /// </summary>
    public DiceRoll Roll { get; private set; }

    /// <summary>
    /// Number of dice.
    /// </summary>
    public int DiceAmount => dice.Length;

    /// <inheritdoc/>
    public int this[int i] => dice[i];

    private readonly int[] dice;

    /// <summary>
    /// Create a new roll, order of <paramref name="dice"/> is persisted.
    /// </summary>
    public DiceRollDistinct(int[] dice)
    {
      this.dice = (int[])dice.Clone();
      Roll = new DiceRoll(dice);
    }

    /// <summary>
    /// Create a new roll with dice values from <paramref name="diceIndexesToReroll"/>
    /// replaced by <paramref name="rerollResult"/>.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// <paramref name="diceIndexesToReroll"/> contains values out of interval [0, <see cref="DiceAmount"/> - 1].
    /// <paramref name="diceIndexesToReroll"/> size differs from <paramref name="rerollResult"/> size.
    /// </exception>
    public DiceRollDistinct ApplyRerollAtIndexes(int[] diceIndexesToReroll, DiceRollDistinct rerollResult)
    {
      int[] diceNew = (int[])dice.Clone();

      if (diceIndexesToReroll.Length != rerollResult.DiceAmount)
      {
        throw new ArgumentException(
          $"Dice to reroll and dice result has to be of the same length, but was: diceIndexesToReroll({diceIndexesToReroll.Length}), rerollResult({rerollResult.DiceAmount})");
      }

      for (int rerollCounter = 0; rerollCounter < diceIndexesToReroll.Length; rerollCounter++)
      {
        int dieNumber = diceIndexesToReroll[rerollCounter];
        if (dieNumber < 0 || dieNumber >= DiceAmount)
        {
          throw new ArgumentException(
            $"Can't reroll die {dieNumber}. Die number has to be between 0 and {DiceAmount - 1} inclusively for roll {this}.");
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
