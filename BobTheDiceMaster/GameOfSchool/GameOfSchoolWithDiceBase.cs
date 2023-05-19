namespace BobTheDiceMaster.GameOfSchool
{
  public abstract class GameOfSchoolWithDiceBase : GameOfSchoolBase
  {
    protected IDie dice;

    protected GameOfSchoolWithDiceBase(IDie dice)
    {
      this.dice = dice;
    }

    /// <summary>
    /// Generate and return a new <see cref="CurrentRoll"/> and
    /// change game state to <see cref="GameOfSchoolState.Rolled"/>.
    /// Only possible in <see cref="GameOfSchoolState.Idle"/> game state.
    /// </summary>
    public DiceRollDistinct GenerateRoll()
    {
      DiceRollDistinct newRoll = new DiceRollDistinct(
          dice.Roll(DiceRoll.MaxDiceAmount));
      SetCurrentRollProtected(newRoll);

      return newRoll;
    }
  }
}
