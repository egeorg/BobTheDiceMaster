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

    public override bool Equals(object obj)
    {
      return obj is Score && ((Score)obj).CombinationToScore == CombinationToScore;
    }

    public override int GetHashCode()
    {
      return CombinationToScore.GetHashCode();
    }
  }
}
