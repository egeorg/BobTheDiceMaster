using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public class Score : Decision
  {
    public CombinationTypes CombinationToScore { get; }

    public Score(
      CombinationTypes combinationToScore,
      IEnumerable<DecisionInfoVerbose> decisionInfos = null)
      : base(decisionInfos)
    {
      CombinationToScore = combinationToScore;
    }

    public override string ToString()
    {
      return $"Score({CombinationToScore})";
    }
  }
}
