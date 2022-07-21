using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class Reroll : IDecision
  {
    public List<int> DiceToReroll { get; }

    public Reroll(IEnumerable<int> diceToReroll)
    {
      DiceToReroll = diceToReroll.ToList();
    }

    public override string ToString()
    {
      return $"Reroll({string.Join(", ", DiceToReroll)})";
    }
  }
}
