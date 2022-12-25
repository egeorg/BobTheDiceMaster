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
  }
}
