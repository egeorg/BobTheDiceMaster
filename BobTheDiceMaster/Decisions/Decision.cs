using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public abstract class Decision
  {
    public SortedSet<DecisionInfoVerbose> RatedDecisionInfo { get; }

    public Decision(IEnumerable<DecisionInfoVerbose> decisionInfos)
    {
      RatedDecisionInfo = new SortedSet<DecisionInfoVerbose>(
        decisionInfos, new DecisionInfoInverseByValueComparer());
    }
  }
}
