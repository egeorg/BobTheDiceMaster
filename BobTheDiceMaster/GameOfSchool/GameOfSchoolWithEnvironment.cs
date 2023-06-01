using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster.GameOfSchool
{
    /// <summary>
    /// A base class for a game of school for a single player.
    /// A game for multiple players can be constructed using several
    /// <see cref="GameOfSchoolWithEnvironment"/> descendants instances in parallel, it does not
    /// have any state shared across different instances.
    /// </summary>
    public abstract class GameOfSchoolWithEnvironment
  {
    /// <summary>
    /// Latest result of a dice roll.
    /// </summary>
    public DiceRollDistinct CurrentRoll => game.CurrentRoll;

    /// <summary>
    /// All the combinations that are left (not scored or crossed out).
    /// </summary>
    public IEnumerable<CombinationTypes> AllowedCombinationTypes =>
      game.AllowedCombinationTypes.GetElementaryCombinationTypes();

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
      AllowedCombinationTypes.Where(x => game.CurrentRoll.Roll.Score(x) != null);

    /// <summary>
    /// Current state of game turn.
    /// </summary>
    public GameOfSchoolState State => game.State;

    /// <summary>
    /// Current game score.
    /// </summary>
    public int Score => game.Score;

    /// <summary>
    /// True iff game is over.
    /// </summary>
    public bool IsGameOver => game.State == GameOfSchoolState.GameOver;

    /// <summary>
    /// True iff turn a is not in progress: player has rolled the dice, but did not scored or crossed out any combination.
    /// </summary>
    public bool IsTurnOver =>
      game.State == GameOfSchoolState.Idle
      || game.State == GameOfSchoolState.GameOver;

    /// <summary>
    /// How many rerolls are left.
    /// </summary>
    public int RerollsLeft => game.RerollsLeft;

    protected IGameOfSchool game;

    public GameOfSchoolWithEnvironment(IGameOfSchool game)
    {
      this.game = game;
    }
  }
}
