﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class GameOfSchool
  {
    public DiceRollDistinct CurrentRoll => currentRoll;
    public IEnumerable<CombinationTypes> AllowedCombinationTypes =>
      allowedCombinationTypes.GetElementaryCombinationTypes();
    public IEnumerable<CombinationTypes> CrossOutCombinationTypes =>
      AllowedCombinationTypes.Where(x => !x.IsFromSchool());
    public IEnumerable<CombinationTypes> ScoreCombinationTypes =>
      AllowedCombinationTypes.Where(x => currentRoll.Roll.Score(x) != null);

    public GameOfSchoolState State => state;

    public int Score => totalScore;

    public bool IsGameOver => state == GameOfSchoolState.GameOver;

    public bool IsTurnOver =>
      state == GameOfSchoolState.Idle
      || state == GameOfSchoolState.GameOver;

    public int RerollsLeft => rerollsLeft;

    public GameOfSchool(IPlayer player)
    {
      this.player = player;
      d6 = new D6();
      Reset();
    }

    public void Reset()
    {
      currentRoll = new DiceRollDistinct(d6.Roll(DiceRoll.MaxDiceAmount));
      allowedCombinationTypes = CombinationTypes.School;
      isSchoolFinished = false;
      totalScore = 0;
      state = GameOfSchoolState.Idle;
    }

    public DiceRollDistinct GenerateRoll()
    {
      VerifyState(GameOfSchoolState.Idle);
      rerollsLeft = RerollsPerTurn;
      currentRoll = new DiceRollDistinct(d6.Roll(DiceRoll.MaxDiceAmount));
      state = GameOfSchoolState.Rolled;
      return currentRoll;
    }

    public void SetRoll(DiceRollDistinct roll)
    {
      VerifyState(GameOfSchoolState.Idle);
      this.currentRoll = roll;
      state = GameOfSchoolState.Rolled;
    }

    public int[] GenerateAndApplyReroll()
    {
      VerifyState(GameOfSchoolState.Rolled);
      int[] reroll = d6.Roll(diceToReroll.Length);
      //TODO[GE]: Make DiceRoll immutable
      currentRoll = currentRoll.ApplyReroll(diceToReroll, reroll);
      return reroll;
    }

    public void GenerateAndApplyReroll(int[] diceToReroll)
    {
      VerifyState(GameOfSchoolState.Rolled);
      int[] reroll = d6.Roll(diceToReroll.Length);
      currentRoll = currentRoll.ApplyReroll(diceToReroll, reroll);
      --rerollsLeft;
    }

    public void ApplyReroll(int[] reroll)
    {
      VerifyState(GameOfSchoolState.Rolled);
      currentRoll = currentRoll.ApplyReroll(diceToReroll, reroll);
    }

    public Decision GenerateAndApplyDecision()
    {
      VerifyState(GameOfSchoolState.Rolled);
      Decision decision = player.DecideOnRoll(
        allowedCombinationTypes, currentRoll.Roll, rerollsLeft);
      ApplyDecision(decision);
      return decision;
    }

    public void ApplyDecision(Decision decision)
    {
      VerifyState(GameOfSchoolState.Rolled);
      switch (decision)
      {
        case Reroll reroll:
          --rerollsLeft;
          diceToReroll = GetDiceToReroll(reroll.DiceValuesToReroll);
          break;
        case Score score:
          if (!allowedCombinationTypes.HasFlag(score.CombinationToScore))
          {
            throw new InvalidOperationException(
              $"Combination {score.CombinationToScore} is already used");
          }
          if (currentRoll.Roll.Score(score.CombinationToScore) == null)
          {
            throw new InvalidOperationException(
              $"Combination {score.CombinationToScore} can't be scored for roll {currentRoll}");
          }
          if (rerollsLeft == RerollsPerTurn
            && !score.CombinationToScore.IsFromSchool())
          {
            totalScore += currentRoll.Roll.Score(score.CombinationToScore).Value * 2;
          }
          else
          {
            totalScore += currentRoll.Roll.Score(score.CombinationToScore).Value;
            //TODO[GE]: move somewhere
            switch (score.CombinationToScore)
            {
              case CombinationTypes.Grade1:
                totalScore -= 5;
                break;
              case CombinationTypes.Grade2:
                totalScore -= 10;
                break;
              case CombinationTypes.Grade3:
                totalScore -= 15;
                break;
              case CombinationTypes.Grade4:
                totalScore -= 20;
                break;
              case CombinationTypes.Grade5:
                totalScore -= 25;
                break;
              case CombinationTypes.Grade6:
                totalScore -= 30;
                break;
            }
          }
          allowedCombinationTypes -= score.CombinationToScore;
          state = GameOfSchoolState.Idle;
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
          state = GameOfSchoolState.Idle;
          break;
      }
      if (!isSchoolFinished && AreThreeGradesFinished(allowedCombinationTypes))
      {
        allowedCombinationTypes |= CombinationTypes.AllButSchool;
      }

      if (allowedCombinationTypes == CombinationTypes.None)
      {
        state = GameOfSchoolState.GameOver;
      }
    }

    private IPlayer player;
    private D6 d6;
    private const int RerollsPerTurn = 2;

    private DiceRollDistinct currentRoll;
    private int[] diceToReroll;
    private int rerollsLeft;
    private int totalScore;
    private CombinationTypes allowedCombinationTypes;
    private bool isSchoolFinished;
    private GameOfSchoolState state;

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

    private void VerifyState(GameOfSchoolState requiredState)
    {
      if (state == requiredState)
      {
        return;
      }

      switch (state)
      {
        case GameOfSchoolState.Idle:
          throw new InvalidOperationException(
            "Dice were not rolled yet. Use GenerateRoll or SetRoll");
        case GameOfSchoolState.Rolled:
          throw new InvalidOperationException(
            "Previous turn is not over yet. Use GenerateReroll, ApplyDecision or GenerateAndApplyDecision");
        case GameOfSchoolState.GameOver:
          throw new InvalidOperationException("Game is over. Start a new game.");
        default:
          throw new InvalidOperationException($"Unexpected game state: {state}.");
      }
    }

    private int[] GetDiceToReroll(IReadOnlyCollection<int> diceValuesToReroll)
    {
      int[] diceToReroll = new int[diceValuesToReroll.Count];
      int rerollCounter = 0;
      bool[] dieUsed = new bool[DiceRoll.MaxDiceAmount];
      foreach (int dieValue in diceValuesToReroll)
      {
        for (int i = 0; i < DiceRoll.MaxDiceAmount; ++i)
        {
          if (!dieUsed[i] && currentRoll[i] == dieValue)
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
          $"Current roll {currentRoll} does not contain all of the reroll values: {String.Join(",", diceValuesToReroll)}");
      }

      return diceToReroll;
    }


  }
}
