using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class Reroll : Decision
  {
    public List<int> DiceToReroll { get; }

    public Reroll(
      IEnumerable<int> diceToReroll,
      IEnumerable<DecisionInfoVerbose> decisionInfos = null)
      : base(decisionInfos)
    {
      DiceToReroll = diceToReroll.ToList();
    }

    public override string ToString()
    {
      return $"Reroll({string.Join(", ", DiceToReroll)})";
    }
  }
}
