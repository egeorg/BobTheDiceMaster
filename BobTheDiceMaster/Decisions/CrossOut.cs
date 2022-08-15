using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public class CrossOut : IDecision
  {
    public CombinationTypes Combination { get; }
    public SortedSet<DecisionInfoVerbose> RatedDecisionInfo { get; }

    public CrossOut(
      CombinationTypes combination,
      IEnumerable<DecisionInfoVerbose> ratedDecisionInfo = null)
    {
      RatedDecisionInfo = new SortedSet<DecisionInfoVerbose>(
        ratedDecisionInfo,
        new DecisionInfoInverseByValueComparer());
      Combination = combination;
    }

    public override string ToString()
    {
      return $"CrossOut({Combination})";
    }
  }
}
