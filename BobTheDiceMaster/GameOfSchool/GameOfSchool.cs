namespace BobTheDiceMaster
{
  /// <summary>
  /// A game of school for a single player with decisions and dice values as input parameters.
  /// </summary>
  public class GameOfSchool : GameOfSchoolBase
  {
    /// <summary>
    /// Set value of <see cref="CurrentRoll"/> to <paramref name="roll"/>.
    /// Only possible in <see cref="GameOfSchoolState.Idle"/> game state.
    /// </summary>
    public void SetCurrentRoll(DiceRollDistinct roll)
    {
      SetCurrentRollProtected(roll);
    }

    /// <summary>
    /// Apply a reroll result <paramref name="newDiceValues"/> to
    /// <see cref="CurrentRoll"/> dice at indexes <paramref name="diceIndexes"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    public void ApplyRerollToDiceAtIndexes(int[] newDiceValues, int[] diceIndexes)
    {
      ApplyRerollToDiceAtIndexesProtected(newDiceValues, diceIndexes);
    }

    public void ScoreCombination(CombinationTypes combinationToScore)
    {
      ScoreCombinationProtected(combinationToScore);
    }

    public void CrossOutCombination(CombinationTypes combinationToCrossOut)
    {
      CrossOutCombinationProtected(combinationToCrossOut);
    }
  }
}
