using System.Collections.Generic;

namespace BobTheDiceMaster
{
  /// <summary>
  /// Base class for a palyer's decision (cross out, score or reroll).
  /// </summary>
  public class Decision
  {
    /// <summary>
    /// Information about possible decisions sorted by their
    /// profit descending, i.e. the first one is optimal.
    /// </summary>
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
