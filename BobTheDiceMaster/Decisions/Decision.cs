using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public class Decision
  {
    public SortedSet<DecisionInfoVerbose> RatedDecisionInfo { get; }

    public Decision(IEnumerable<DecisionInfoVerbose> decisionInfos)
    {
      if (decisionInfos != null)
      {
        RatedDecisionInfo = new SortedSet<DecisionInfoVerbose>(
          decisionInfos, new DecisionInfoInverseByValueComparer());
      }
    }
  }
}
