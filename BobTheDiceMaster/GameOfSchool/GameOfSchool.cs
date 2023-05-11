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
    /// Apply a reroll result <paramref name="rerolledDiceValuesAfterReroll"/> to a <see cref="CurrentRoll"/>.
    /// Dice to be rerolled has to be set earlier by
    /// a <see cref="ApplyDecision(Decision)"/> method with an argument of type <see cref="Reroll"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    public void ApplyReroll(int[] rerolledDiceValuesAfterReroll)
    {
      ApplyRerollProtected(rerolledDiceValuesAfterReroll);
    }

    /// <summary>
    /// Apply a decision passed as an argument.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    public void ApplyDecision(Decision decision)
    {
      ApplyDecisionProtected(decision);
    }
  }
}
