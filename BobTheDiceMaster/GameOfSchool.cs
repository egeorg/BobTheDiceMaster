﻿using System;
using System.Linq;

namespace BobTheDiceMaster
{
  public class GameOfSchool
  {
    #region private fields
    private IPlayer player;
    private IDie die;
    private CombinationTypes allowedCombinationTypes;
    private bool isSchoolFinished;
    private int totalScore;
    private const int RollsPerTurn = 3;
    #endregion

    #region private methods
    private bool AreThreeGradesFinished(CombinationTypes combinationTypes)
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
    #endregion

    #region public properties
    public int Score => totalScore;

    public bool IsOver { get; private set; } = false;
    #endregion

    #region public methods
    public GameOfSchool(IPlayer player, IDie die)
    {
      this.player = player;
      this.die = die;
      Reset();
    }

    public void Reset()
    {
      allowedCombinationTypes = CombinationTypes.School;
      isSchoolFinished = false;
      totalScore = 0;
    }

    public void NextStep()
    {
      if (allowedCombinationTypes == CombinationTypes.None)
      {
        IsOver = true;
        return;
      }
      DiceRoll roll = DiceRoll.GenerateNew(die);
      int rollsLeft = RollsPerTurn;
      while (rollsLeft > 0)
      {
        Console.WriteLine($"Considering roll {roll}. Waiting for a decision.");
        Decision decision = player.DecideOnRoll(allowedCombinationTypes, roll, rollsLeft);

        Console.WriteLine(
          $"Best combinations are: {Environment.NewLine}" +
          $"{string.Join(Environment.NewLine, decision.RatedDecisionInfo.Take(3))}");

        switch (decision)
        {
          case Reroll reroll:
            --rollsLeft;
            roll.Reroll(reroll.DiceToReroll, die);
            break;
          case Score score:
            if (!allowedCombinationTypes.HasFlag(score.CombinationToScore))
            {
              throw new InvalidOperationException($"Combination {score.CombinationToScore} is already used");
            }
            if (roll.Score(score.CombinationToScore) == null)
            {
              throw new InvalidOperationException($"Combination {score.CombinationToScore} can't be scored for roll {roll}");
            }
            if (rollsLeft == RollsPerTurn
              && !score.CombinationToScore.IsFromSchool())
            {
              totalScore += roll.Score(score.CombinationToScore).Value * 2;
            }
            else
            {
              totalScore += roll.Score(score.CombinationToScore).Value;
            }
            allowedCombinationTypes -= score.CombinationToScore;
            rollsLeft = 0;
            break;
          case CrossOut crossOut:
            if (!allowedCombinationTypes.HasFlag(crossOut.Combination))
            {
              throw new InvalidOperationException($"Combination {crossOut.Combination} is already used");
            }
            if (crossOut.Combination.IsFromSchool())
            {
              throw new InvalidOperationException($"Can't cross out combination {crossOut.Combination}. Can't cross out combinations from school");
            }
            allowedCombinationTypes -= crossOut.Combination;
            rollsLeft = 0;
            break;
        }
        if (!isSchoolFinished && AreThreeGradesFinished(allowedCombinationTypes))
        {
          allowedCombinationTypes |= CombinationTypes.AllButSchool;
        }
      }
    }
    #endregion
  }
}
