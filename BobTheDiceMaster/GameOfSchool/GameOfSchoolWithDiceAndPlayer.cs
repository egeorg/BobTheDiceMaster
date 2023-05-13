﻿using System;
using System.Collections.Generic;

namespace BobTheDiceMaster
{
  /// <summary>
  /// A game of school for a single player with decisions generated by
  /// an <see cref="IPlayer"/> passed to the constructor and dice values
  /// generated by an <see cref="IDie"/> passed to the constructor.
  /// </summary>
  public class GameOfSchoolWithDiceAndPlayer : GameOfSchoolBase
  {
    private IDie dice;
    private IPlayer aiPlayer;

    public GameOfSchoolWithDiceAndPlayer(IDie dice, IPlayer aiPlayer)
    {
      this.dice = dice;
      this.aiPlayer = aiPlayer;
    }

    /// <summary>
    /// Generate and return a new <see cref="CurrentRoll"/> and
    /// change game state to <see cref="GameOfSchoolState.Rolled"/>.
    /// Only possible in <see cref="GameOfSchoolState.Idle"/> game state.
    /// </summary>
    public DiceRollDistinct GenerateRoll()
    {
      DiceRollDistinct newRoll = new DiceRollDistinct(
          dice.Roll(DiceRoll.MaxDiceAmount));
      SetCurrentRollProtected(newRoll);

      return newRoll;
    }

    /// <summary>
    /// Generate a decision using a player passed through a constructor
    /// and apply it.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    /// <returns>
    /// The decision that was generated and applied.
    /// </returns>
    public Decision GenerateAndApplyDecision()
    {
      Decision decision = aiPlayer.DecideOnRoll(
        allowedCombinationTypes, CurrentRoll.Roll, RerollsLeft);
      switch (decision)
      {
        case Reroll reroll:
          int[] diceIndexesToReroll = GetDiceIndexesToReroll(reroll.DiceValuesToReroll);
          ApplyRerollToDiceAtIndexesProtected(dice.Roll(diceIndexesToReroll.Length), diceIndexesToReroll);
          break;
        case Score score:
          ScoreCombinationProtected(score.Combination);
          break;
        case CrossOut crossOut:
          CrossOutCombinationProtected(crossOut.Combination);
          break;
      }
      return decision;
    }

    private int[] GetDiceIndexesToReroll(IReadOnlyCollection<int> diceValuesToReroll)
    {
      int[] diceToReroll = new int[diceValuesToReroll.Count];
      int rerollCounter = 0;
      bool[] dieUsed = new bool[DiceRoll.MaxDiceAmount];
      foreach (int dieValue in diceValuesToReroll)
      {
        for (int i = 0; i < DiceRoll.MaxDiceAmount; ++i)
        {
          if (!dieUsed[i] && CurrentRoll[i] == dieValue)
          {
            diceToReroll[rerollCounter++] = i;
            dieUsed[i] = true;
            break;
          }
        }
      }

      if (rerollCounter < diceValuesToReroll.Count)
      {
        throw new InvalidOperationException(
          $"Current roll {CurrentRoll} does not contain all of the reroll values: {String.Join(",", diceValuesToReroll)}");
      }

      return diceToReroll;
    }
  }
}