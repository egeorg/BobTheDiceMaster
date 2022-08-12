using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class DecisionInfoVerbose : DecisionInfo
  {
    public DecisionInfoVerbose(
      double value,
      IEnumerable<OutcomeInfo> outcomes,
      int[] reroll = null)
    {
      Value = value;
      Reroll = reroll;
      Outcomes = new SortedSet<OutcomeInfo>(outcomes, new OutcomeInfoInverseByValueComparer());
      Combination = Outcomes.First().Combination;
    }

    public SortedSet<OutcomeInfo> Outcomes { get; }

    public override string ToString()
    {
      if (Reroll != null)
      {
        return
          $"C={Combination};V={Value};R={string.Join(",", Reroll)};O={string.Join(",", Outcomes)}";
      }
      return
        $"C={Combination};V={Value};O={string.Join(",", Outcomes)}";
    }
  }
}
