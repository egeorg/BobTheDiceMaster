using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
    public class Reroll : Decision
  {
    /// <remarks>
    /// Public setter is required only to make it serializable, otherwise setter can be removed.
    /// </remarks>
    public List<int> DiceValuesToReroll { get; set; }

    /// <remarks>
    /// Parameterless constructor is required only to make it serializable.
    /// </remarks>
    public Reroll()
    { }

    public Reroll(
      IEnumerable<int> diceToReroll,
      IEnumerable<DecisionInfoVerbose> decisionInfos = null)
      : base(decisionInfos)
    {
      DiceValuesToReroll = diceToReroll.ToList();
    }

    public override string ToString()
    {
      return $"Reroll({string.Join(", ", DiceValuesToReroll)})";
    }
  }
}
