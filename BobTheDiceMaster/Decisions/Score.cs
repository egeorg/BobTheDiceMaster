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
    public CombinationTypes Combination { get; }

    public Score(
      CombinationTypes combination,
      IEnumerable<DecisionInfoVerbose> decisionInfos = null)
      : base(decisionInfos)
    {
      Combination = combination;
    }

    public override string ToString()
    {
      return $"Score({Combination})";
    }

    /// <summary>
    /// Two <see cref="Score"/>s are considered equal if corresponding combinations are equal.
    /// </summary>
    public override bool Equals(object obj)
    {
      return obj is Score && ((Score)obj).Combination == Combination;
    }

    public override int GetHashCode()
    {
      return Combination.GetHashCode();
    }
  }
}
