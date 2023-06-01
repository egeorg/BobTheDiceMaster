using System;
using System.Collections.Generic;
using System.Text;

namespace BobTheDiceMaster.GameOfSchool
{
  public interface IGameOfSchool
  {
    /// <summary>
    /// Latest result of a dice roll.
    /// </summary>
    public DiceRollDistinct CurrentRoll { get; }

    /// <summary>
    /// TODO
    /// </summary>
    public CombinationTypes AllowedCombinationTypes { get; }

    /// <summary>
    /// Current state of game turn.
    /// </summary>
    public GameOfSchoolState State { get; }

    /// <summary>
    /// Current game score.
    /// </summary>
    public int Score { get; }

    /// <summary>
    /// How many rerolls are left.
    /// </summary>
    public int RerollsLeft { get; }

    /// <summary>
    /// Reset the game: make it as if it did not even start.
    /// </summary>
    public void Reset();

    /// <summary>
    /// Set <see cref="CurrentRoll"/> to <paramref name="roll"/> and
    /// change game state to <see cref="GameOfSchoolState.Rolled"/>.
    /// Only possible in <see cref="GameOfSchoolState.Idle"/> game state.
    /// </summary>
    public void SetCurrentRoll(DiceRollDistinct roll);

    /// <summary>
    /// Apply a reroll result <paramref name="newDiceValues"/> to
    /// <see cref="CurrentRoll"/> dice at indexes <paramref name="diceIndexes"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    public void ApplyRerollToDiceAtIndexes(int[] newDiceValues, int[] diceIndexes);

    /// <summary>
    /// Score a combination <paramref name="combination"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    public void ScoreCombination(CombinationTypes combination);

    /// <summary>
    /// Cross out a combination <paramref name="combination"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    public void CrossOutCombination(CombinationTypes combination);
  }
}
