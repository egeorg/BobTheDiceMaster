using System.Collections.Generic;

namespace BobTheDiceMaster
{
  //TODO[GE]: to abstract class
  public interface IDecision
  {
    public SortedSet<DecisionInfoVerbose> RatedDecisionInfo { get; }
  }
}
