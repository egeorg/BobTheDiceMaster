using System.Collections.Generic;

namespace BobTheDiceMaster
{
  /// <summary>
  /// A decision to cross out a combination.
  /// </summary>
  public class CrossOut : Decision
  {
    /// <summary>
    /// A combination to cross out.
    /// </summary>
    public CombinationTypes Combination { get; }

    public CrossOut(
      CombinationTypes combination,
      IEnumerable<DecisionInfoVerbose> decisionInfos = null)
      : base(decisionInfos)
    {
      Combination = combination;
    }

    public override string ToString()
    {
      return $"CrossOut({Combination})";
    }

    /// <summary>
    /// Two <see cref="CrossOut"/>s are considered equal if corresponding combinations are equal.
    /// </summary>
    public override bool Equals(object obj)
    {
      return obj is CrossOut && ((CrossOut)obj).Combination == Combination;
    }

    public override int GetHashCode()
    {
      return Combination.GetHashCode();
    }
  }
}
