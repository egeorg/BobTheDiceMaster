namespace BobTheDiceMaster.GameOfSchool
{
    public abstract class GameOfSchoolWithDice : GameOfSchoolWithEnvironment
  {
    protected IDie dice;

    protected GameOfSchoolWithDice(IGameOfSchool game, IDie dice) : base(game)
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
      game.SetCurrentRoll(newRoll);

      return newRoll;
    }
  }
}
