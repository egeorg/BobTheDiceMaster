using System.Collections.Generic;

namespace BobTheDiceMaster
{
  /// <summary>
  /// A decision to score a combination.
  /// </summary>
  public class Score : Decision
  {
    /// <summary>
    /// A combination to score.
    /// </summary>
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

    /// <summary>
    /// Two <see cref="Score"/>s are considered equal if corresponding combinations are equal.
    /// </summary>
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
