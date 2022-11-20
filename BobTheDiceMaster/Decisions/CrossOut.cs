using System.Collections.Generic;

namespace BobTheDiceMaster
{
    public class CrossOut : Decision
  {
    /// <remarks>
    /// Public setter is required only to make it serializable, otherwise setter can be removed.
    /// </remarks>
    public CombinationTypes Combination { get; set; }

    /// <remarks>
    /// Parameterless constructor is required only to make it serializable.
    /// </remarks>
    public CrossOut()
    { }

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
