using System.Threading.Tasks;

namespace BobTheDiceMaster
{
  /// <summary>
  /// An interface for a <see cref="GameOfSchool"/> player.
  /// It has only a single method to select a decision given a game context:
  /// available combination types, current roll result and how many rerolls are left.
  /// </summary>
  public interface IPlayer
  {
    /// <summary>
    /// Get a decision given a game context: available combination types,
    /// current roll result and how many rerolls are left.
    /// All the heavyweight computation must be done asynchronously.
    /// </summary>
    Task<Decision> DecideOnRollAsync(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft);
  }
}
