using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public class CrossOut : Decision
  {
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
