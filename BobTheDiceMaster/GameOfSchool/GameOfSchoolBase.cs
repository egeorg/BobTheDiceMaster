using System;
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
    /// Apply a reroll result <paramref name="rerolledDiceValuesAfterReroll"/> to a <see cref="CurrentRoll"/>.
    /// Dice to be rerolled has to be set earlier by
    /// a <see cref="ApplyDecisionProtected(Decision)"/> method with an argument of type <see cref="Reroll"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    protected void ApplyRerollProtected(int[] rerolledDiceValuesAfterReroll)
    {
      VerifyState(GameOfSchoolState.Rolled);
      currentRoll = currentRoll.ApplyRerollAtIndexes(
        diceIndexesToReroll, new DiceRollDistinct(rerolledDiceValuesAfterReroll));
    }

    /// <summary>
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    protected void ApplyDecisionProtected(Decision decision)
    {
      VerifyState(GameOfSchoolState.Rolled);
      switch (decision)
      {
        case Reroll reroll:
          DecrementRerollsLeftAndThrowIfNoRerollsLeft();
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

    private const int RerollsPerTurn = 2;

    private int totalScore;
    private bool isSchoolFinished;
    private GameOfSchoolState state;
    
    //TODO[GE]: move to appropriate location, private fields too
    protected int[] diceIndexesToReroll;
    protected DiceRollDistinct currentRoll;
    protected CombinationTypes allowedCombinationTypes;
    protected int rerollsLeft;

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

    protected int[] GetDiceIndexesToReroll(IReadOnlyCollection<int> diceValuesToReroll)
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

    protected void DecrementRerollsLeftAndThrowIfNoRerollsLeft()
    {
      if (rerollsLeft == 0)
      {
        throw new InvalidOperationException("No rerolls are left, can't reroll the dice");
      }
      --rerollsLeft;
    }
  }
}
