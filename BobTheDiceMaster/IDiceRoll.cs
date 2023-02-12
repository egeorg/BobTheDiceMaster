using System.Collections.Generic;

namespace BobTheDiceMaster
{
  /// <summary>
  /// An interface for a dice roll. Dice roll is a result of rolling
  /// a <see cref="DiceAmount"/> number of dice.
  /// </summary>
  /// <remarks>
  /// The type parameter is necessary to make methods like
  /// <see cref="Reroll(int[], IDie)"/> and <see cref="ApplyReroll(int[], T)"/>
  /// that return the <see cref="IDiceRoll{T}"/>
  /// implementation itself.
  /// </remarks>
  public interface IDiceRoll<T> where T : IDiceRoll<T>
  {
    /// <summary>
    /// Number of dice in the roll.
    /// </summary>
    public int DiceAmount { get; }

    /// <summary>
    /// Replace dice at the indexes <paramref name="diceIndexesToReroll"/>
    /// by the result of the <paramref name="die"/> roll.
    /// </summary>
    public T Reroll(int[] diceIndexesToReroll, IDie die);

    /// <summary>
    /// Replace dice at the indexes <paramref name="diceIndexesToReroll"/>
    /// by the <paramref name="rerollResult"/>.
    /// </summary>
    public T ApplyReroll(int[] diceIndexesToReroll, T rerollResult);

    /// <summary>
    /// Public way to access dice values without being able to affect them.
    /// <paramref name="i">-th dice value.
    /// </summary>
    public int this[int i] { get; }
  }
}
