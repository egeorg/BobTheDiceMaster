using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public interface IDiceRoll<T>
  {
    public int DiceAmount { get; }
    public T Reroll(IReadOnlyCollection<int> diceToReroll, IDie die);
    public T ApplyReroll(int[] diceToReroll, IDiceRoll<T> rerollResult);
    public int this[int i] { get; }
  }
}
