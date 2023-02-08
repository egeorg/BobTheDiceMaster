using System;
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

    public GameOfSchool(IPlayer player, IDie d6)
    {
      this.player = player;
      this.d6 = d6;
      Reset();
    }

    public void Reset()
    {
      //TODO: setting roll after reset does not makes sense if game does not generate rolls itself.
      // Also state is idle, but current roll is set, does it makes sense?
      //currentRoll = new DiceRollDistinct(d6.Roll(DiceRoll.MaxDiceAmount));
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
      //TODO: does it affect anything? Seems that everything worked without it.
      rerollsLeft = RerollsPerTurn;
      this.currentRoll = roll;
      state = GameOfSchoolState.Rolled;
    }

    public void GenerateAndApplyReroll()
    {
      VerifyState(GameOfSchoolState.Rolled);
      int[] reroll = d6.Roll(diceIndexesToReroll.Length);
      //TODO[GE]: Make DiceRoll immutable
      currentRoll = currentRoll.ApplyReroll(diceIndexesToReroll, reroll);
    }

    /// <summary>
    /// Shorcut, it's basically the same as calling
    /// <see cref="ApplyDecision(Reroll)"> and then <see cref="GenerateAndApplyReroll">.
    /// TODO: check that ApplyDecision(Reroll) translated correctly in a generated document
    /// </summary>
    public void GenerateAndApplyReroll(int[] diceIndexesToReroll)
    {
      VerifyState(GameOfSchoolState.Rolled);
      DecrementRerollsLeft();
      int[] reroll = d6.Roll(diceIndexesToReroll.Length);
      currentRoll = currentRoll.ApplyReroll(diceIndexesToReroll, reroll);
    }


    public void ApplyReroll(int[] reroll)
    {
      //TODO: ensure that it's not called too many times.
      VerifyState(GameOfSchoolState.Rolled);
      currentRoll = currentRoll.ApplyReroll(diceIndexesToReroll, reroll);
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
          DecrementRerollsLeft();
          diceIndexesToReroll = GetDiceIndexesToReroll(reroll.DiceValuesToReroll);
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
    private IDie d6;
    private const int RerollsPerTurn = 2;

    private DiceRollDistinct currentRoll;
    private int[] diceIndexesToReroll;
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

    private int[] GetDiceIndexesToReroll(IReadOnlyCollection<int> diceValuesToReroll)
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

    private void DecrementRerollsLeft()
    {
      if (rerollsLeft == 0)
      {
        throw new InvalidOperationException("No rerolls are left, can't reroll the dice");
      }
      --rerollsLeft;
    }
  }
}
