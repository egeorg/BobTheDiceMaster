using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class Reroll : Decision
  {
    public List<int> DiceValuesToReroll { get; }

    public Reroll(
      IEnumerable<int> diceValuesToReroll,
      IEnumerable<DecisionInfoVerbose> decisionInfos = null)
      : base(decisionInfos)
    {
      DiceValuesToReroll = diceValuesToReroll.ToList();
    }

    public override string ToString()
    {
      return $"Reroll({string.Join(", ", DiceValuesToReroll)})";
    }
  }
}
