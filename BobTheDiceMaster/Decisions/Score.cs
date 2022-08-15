using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public class Score : IDecision
  {
    public CombinationTypes CombinationToScore { get; }
    public SortedSet<DecisionInfoVerbose> RatedDecisionInfo { get; }

    public Score(
      CombinationTypes combinationToScore,
      IEnumerable<DecisionInfoVerbose> ratedDecisionInfo = null)
    {
      RatedDecisionInfo = new SortedSet<DecisionInfoVerbose>(
        ratedDecisionInfo,
        new DecisionInfoInverseByValueComparer());
      CombinationToScore = combinationToScore;
    }

    public override string ToString()
    {
      return $"Score({CombinationToScore})";
    }
  }
}
