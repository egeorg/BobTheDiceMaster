namespace BobTheDiceMaster
{
  /// <summary>
  /// An interface for classes that represent a die.
  /// </summary>
  public interface IDie
  {
    /// <summary>
    /// A single die roll result.
    /// </summary>
    public int Roll();

    /// <summary>
    /// Results of a roll of <see cref="diceAmount"/> dice.
    /// </summary>
    public int[] Roll(int diceAmount);
  }
}
