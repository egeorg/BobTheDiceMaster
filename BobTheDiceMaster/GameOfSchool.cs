using System;

namespace BobTheDiceMaster
{
  public class GameOfSchool
  {
    #region private fields
    private IPlayer player;
    private D6 d6;
    private const int RollsPerTurn = 3;

    #region current game state
    private DiceRoll currentRoll;
    private int[] diceToReroll;
    private int rollsLeft;
    private int totalScore;
    private CombinationTypes allowedCombinationTypes;
    private bool isSchoolFinished;
    #endregion

    private GameOfSchoolState state;

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

    #endregion

    #region public properties
    public int Score => totalScore;

    public bool IsGameOver => state == GameOfSchoolState.GameOver;

    public bool IsTurnOver =>
      state == GameOfSchoolState.Idle
      || state == GameOfSchoolState.GameOver;

    public int RollsLeft => rollsLeft;
    #endregion

    #region public methods
    public GameOfSchool(IPlayer player)
    {
      this.player = player;
      d6 = new D6();
      Reset();
    }

    public void Reset()
    {
      allowedCombinationTypes = CombinationTypes.School;
      isSchoolFinished = false;
      totalScore = 0;
      state = GameOfSchoolState.Idle;
    }

    public DiceRoll GenerateRoll()
    {
      VerifyState(GameOfSchoolState.Idle);
      rollsLeft = RollsPerTurn;
      currentRoll = DiceRoll.GenerateNew(d6);
      state = GameOfSchoolState.Rolled;
      return currentRoll;
    }

    public void SetRoll(DiceRoll roll)
    {
      VerifyState(GameOfSchoolState.Idle);
      this.currentRoll = roll;
      state = GameOfSchoolState.Rolled;
    }

    public DiceRoll GenerateAndApplyReroll()
    {
      VerifyState(GameOfSchoolState.Rolled);
      //TODO[GE]: Make DiceRoll immutable
      currentRoll.Reroll(diceToReroll, d6);
      return currentRoll;
    }

    public void ApplyReroll(DiceRoll roll)
    {
      VerifyState(GameOfSchoolState.Rolled);
      currentRoll = currentRoll.ApplyReroll(diceToReroll, roll);
    }

    public Decision GenerateAndApplyDecision()
    {
      VerifyState(GameOfSchoolState.Rolled);
      Decision decision = player.DecideOnRoll(allowedCombinationTypes, currentRoll, rollsLeft);
      ApplyDecision(decision);
      return decision;
    }

    public void ApplyDecision(Decision decision)
    {
      VerifyState(GameOfSchoolState.Rolled);
      switch (decision)
      {
        case Reroll reroll:
          --rollsLeft;
          diceToReroll = reroll.DiceToReroll.ToArray();
          break;
        case Score score:
          if (!allowedCombinationTypes.HasFlag(score.CombinationToScore))
          {
            throw new InvalidOperationException(
              $"Combination {score.CombinationToScore} is already used");
          }
          if (currentRoll.Score(score.CombinationToScore) == null)
          {
            throw new InvalidOperationException(
              $"Combination {score.CombinationToScore} can't be scored for roll {currentRoll}");
          }
          if (rollsLeft == RollsPerTurn
            && !score.CombinationToScore.IsFromSchool())
          {
            totalScore += currentRoll.Score(score.CombinationToScore).Value * 2;
          }
          else
          {
            totalScore += currentRoll.Score(score.CombinationToScore).Value;
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
    #endregion
  }
}
