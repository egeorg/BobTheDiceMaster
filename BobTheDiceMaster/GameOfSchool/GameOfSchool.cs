using System;

namespace BobTheDiceMaster.GameOfSchool
{
  public class GameOfSchool : IGameOfSchool
  {
    /// <summary>
    /// Latest result of a dice roll.
    /// </summary>
    public DiceRollDistinct CurrentRoll => currentRoll;

    /// <summary>
    /// All the combinations that are left (not scored or crossed out).
    /// </summary>
    public CombinationTypes AllowedCombinationTypes => allowedCombinationTypes;

    /// <summary>
    /// Current state of game turn.
    /// </summary>
    public GameOfSchoolState State => state;

    /// <summary>
    /// Current game score.
    /// </summary>
    public int Score => totalScore;

    /// <summary>
    /// How many rerolls are left.
    /// </summary>
    public int RerollsLeft => rerollsLeft;

    public GameOfSchool()
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
    public void SetCurrentRoll(DiceRollDistinct roll)
    {
      VerifyState(GameOfSchoolState.Idle);
      rerollsLeft = rerollsPerTurn;
      this.currentRoll = roll;
      state = GameOfSchoolState.Rolled;
    }

    /// <summary>
    /// Apply a reroll result <paramref name="newDiceValues"/> to
    /// <see cref="CurrentRoll"/> dice at indexes <paramref name="diceIndexes"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    public void ApplyRerollToDiceAtIndexes(int[] newDiceValues, int[] diceIndexes)
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
    public void ScoreCombination(CombinationTypes combination)
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
      if (rerollsLeft == rerollsPerTurn
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
    public void CrossOutCombination(CombinationTypes combination)
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

    private const int rerollsPerTurn = 2;

    private int totalScore;
    private bool isSchoolFinished;
    private GameOfSchoolState state;
    private int rerollsLeft;
    private DiceRollDistinct currentRoll;
    private CombinationTypes allowedCombinationTypes;

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

      throw new InvalidOperationException($"Unexpected game state: {state}, but expected {requiredState}.");
    }
  }
}
