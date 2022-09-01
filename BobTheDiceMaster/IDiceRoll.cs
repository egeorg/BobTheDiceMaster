using System;
using System.Collections.Generic;
using System.Text;

namespace BobTheDiceMaster
{
  public interface IDiceRoll<T>
  {
    public int DiceAmount { get; }
    public void Reroll(IReadOnlyCollection<int> diceToReroll, IDie die);
    public T ApplyReroll(int[] diceToReroll, IDiceRoll<T> rerollResult);
    public int this[int i] { get; }
  }
}
