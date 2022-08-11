using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public class DecisionInfoVerbose : DecisionInfo
  {
    public DecisionInfoVerbose(
      double value,
      CombinationTypes combination,
      IEnumerable<OutcomeInfo> outcomes,
      int[] reroll = null)
      : base(value, combination, reroll)
    {
      Outcomes = new SortedSet<OutcomeInfo>(outcomes, new OutcomeInfoInverseByValueComparer());
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
