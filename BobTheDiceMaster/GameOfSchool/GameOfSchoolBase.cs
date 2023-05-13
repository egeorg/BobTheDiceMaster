﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  /// <summary>
  /// A base class for a game of school for a single player.
  /// A game for multiple players can be constructed using several
  /// <see cref="GameOfSchoolBase"/> instances in parallel, it does not
  /// have any state shared across different instances.
  /// </summary>
  public abstract class GameOfSchoolBase
  {
    /// <summary>
    /// Latest result of a dice roll.
    /// </summary>
    public DiceRollDistinct CurrentRoll => currentRoll;

    /// <summary>
    /// All the combinations that are left (not scored or crossed out).
    /// </summary>
    public IEnumerable<CombinationTypes> AllowedCombinationTypes =>
      allowedCombinationTypes.GetElementaryCombinationTypes();

    /// <summary>
    /// All the available combinations that can be crossed out.
    /// </summary>
    public IEnumerable<CombinationTypes> CrossOutCombinationTypes =>
      AllowedCombinationTypes.Where(x => !x.IsFromSchool());

    /// <summary>
    /// All the available combinations that can be scored
    /// given actual <see cref="CurrentRoll"/> value.
    /// </summary>
    public IEnumerable<CombinationTypes> ScoreCombinationTypes =>
      AllowedCombinationTypes.Where(x => currentRoll.Roll.Score(x) != null);

    /// <summary>
    /// Current state of game turn.
    /// </summary>
    public GameOfSchoolState State => state;

    /// <summary>
    /// Current game score.
    /// </summary>
    public int Score => totalScore;

    /// <summary>
    /// True iff game is over.
    /// </summary>
    public bool IsGameOver => state == GameOfSchoolState.GameOver;

    /// <summary>
    /// True iff turn a is not in progress: player has rolled the dice, but did not scored or crossed out any combination.
    /// </summary>
    public bool IsTurnOver =>
      state == GameOfSchoolState.Idle
      || state == GameOfSchoolState.GameOver;

    /// <summary>
    /// How many rerolls are left.
    /// </summary>
    public int RerollsLeft => rerollsLeft;

    public GameOfSchoolBase()
    {
      Reset();
    }

    /// <summary>
    /// Reset the game: make it as if it did not even start.
    /// </summary>
    public void Reset()
    {
      allowedCombinationTypes = CombinationTypes.School;
      isSchoolFinished = false;
      totalScore = 0;
      state = GameOfSchoolState.Idle;
    }

    /// <summary>
    /// Set <see cref="CurrentRoll"/> to <paramref name="roll"/> and
    /// change game state to <see cref="GameOfSchoolState.Rolled"/>.
    /// Only possible in <see cref="GameOfSchoolState.Idle"/> game state.
    /// </summary>
    protected void SetCurrentRollProtected(DiceRollDistinct roll)
    {
      VerifyState(GameOfSchoolState.Idle);
      rerollsLeft = RerollsPerTurn;
      this.currentRoll = roll;
      state = GameOfSchoolState.Rolled;
    }

    /// <summary>
    /// Apply a reroll result <paramref name="newDiceValues"/> to
    /// <see cref="CurrentRoll"/> dice at indexes <paramref name="diceIndexes"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    protected void ApplyRerollToDiceAtIndexesProtected(int[] newDiceValues, int[] diceIndexes)
    {
      VerifyState(GameOfSchoolState.Rolled);
      
      if (rerollsLeft == 0)
      {
        throw new InvalidOperationException("No rerolls are left, can't reroll the dice");
      }
      --rerollsLeft;

      currentRoll = currentRoll.ApplyRerollAtIndexes(
        diceIndexes, new DiceRollDistinct(newDiceValues));
    }

    /// <summary>
    /// Score a combination <paramref name="combination"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    protected void ScoreCombinationProtected(CombinationTypes combination)
    {
      VerifyState(GameOfSchoolState.Rolled);
      if (!allowedCombinationTypes.HasFlag(combination))
      {
        throw new InvalidOperationException(
          $"Combination {combination} is already used");
      }
      if (currentRoll.Roll.Score(combination) == null)
      {
        throw new InvalidOperationException(
          $"Combination {combination} can't be scored for roll {currentRoll}");
      }
      if (rerollsLeft == RerollsPerTurn
        && !combination.IsFromSchool())
      {
        totalScore += currentRoll.Roll.Score(combination).Value * 2;
      }
      else
      {
        totalScore += currentRoll.Roll.Score(combination).Value;
      }
      allowedCombinationTypes -= combination;

      if (allowedCombinationTypes == CombinationTypes.None)
      {
        state = GameOfSchoolState.GameOver;
        return;
      }

      if (!isSchoolFinished && AreThreeGradesFinished(allowedCombinationTypes))
      {
        allowedCombinationTypes |= CombinationTypes.AllButSchool;
      }

      state = GameOfSchoolState.Idle;
    }

    /// <summary>
    /// Cross out a combination <paramref name="combination"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    protected void CrossOutCombinationProtected(CombinationTypes combination)
    {
      VerifyState(GameOfSchoolState.Rolled);
      if (!allowedCombinationTypes.HasFlag(combination))
      {
        throw new InvalidOperationException($"Combination {combination} is already used");
      }
      if (combination.IsFromSchool())
      {
        throw new InvalidOperationException($"Can't cross out combination {combination}. Can't cross out combinations from school");
      }
      allowedCombinationTypes -= combination;

      if (allowedCombinationTypes == CombinationTypes.None)
      {
        state = GameOfSchoolState.GameOver;
        return;
      }

      state = GameOfSchoolState.Idle;
    }

    private const int RerollsPerTurn = 2;

    private int totalScore;
    private bool isSchoolFinished;
    private GameOfSchoolState state;
    private int rerollsLeft;
    private DiceRollDistinct currentRoll;

    //TODO[GE]: move to appropriate location, private fields too
    protected CombinationTypes allowedCombinationTypes;

    protected bool AreThreeGradesFinished(CombinationTypes combinationTypes)
    {
      if (!combinationTypes.IsFromSchool())
      {
        throw new ArgumentException($"Only grade combinations expected, but was {combinationTypes}");
      }
      uint gradesLeft = 0;
      uint combinationTypesUint = (uint)combinationTypes;
      while (combinationTypesUint != 0)
      {
        gradesLeft += combinationTypesUint % 2;
        combinationTypesUint = combinationTypesUint >> 1;
      }

      if (gradesLeft > 3)
      {
        return false;
      }
      isSchoolFinished = true;
      return true;
    }

    protected void VerifyState(GameOfSchoolState requiredState)
    {
      if (state == requiredState)
      {
        return;
      }

      throw new InvalidOperationException($"Unexpected game state: {state}, but expected {requiredState}.");
    }
  }
}
