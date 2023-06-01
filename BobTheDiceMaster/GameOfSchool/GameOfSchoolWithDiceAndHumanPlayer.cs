﻿namespace BobTheDiceMaster.GameOfSchool
{
    /// <summary>
    /// A game of school for a single player with decisions as input parameters
    /// and dice values generated by an <see cref="IDie"/> passed to the constructor.
    /// </summary>
    public class GameOfSchoolWithDiceAndHumanPlayer : GameOfSchoolWithDice
  {
    public GameOfSchoolWithDiceAndHumanPlayer(IGameOfSchool game, IDie dice) : base(game, dice)
    {
      this.dice = dice;
    }

    /// <summary>
    /// Score a combination <paramref name="combination"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    public void ScoreCombination(CombinationTypes combination)
    {
      game.ScoreCombination(combination);
    }

    /// <summary>
    /// Cross out a combination <paramref name="combination"/>.
    /// Only possible in <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    public void CrossOutCombination(CombinationTypes combination)
    {
      game.CrossOutCombination(combination);
    }

    /// <summary>
    /// Generate new values for dice at indexes <paramref name="diceIndexes" />.
    /// Only possible in the <see cref="GameOfSchoolState.Rolled"/> game state.
    /// </summary>
    public void RerollDiceAtIndexes(int[] diceIndexes)
    {
      game.ApplyRerollToDiceAtIndexes(dice.Roll(diceIndexes.Length), diceIndexes);
    }
  }
}
