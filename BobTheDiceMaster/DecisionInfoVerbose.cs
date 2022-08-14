using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  /// <summary>
  /// Represents information that justifies certain decision. Namely:
  /// - Average value that the decision yields.
  /// - List of possible outcomes, sorted by their contribution to the decision value.
  /// </summary>
  public class DecisionInfoVerbose : DecisionInfo
  {
    public DecisionInfoVerbose(
      double value,
      IEnumerable<OutcomeInfo> outcomes,
      int[] reroll = null)
    {
      Value = value;
      Reroll = reroll;
      Outcomes = new SortedSet<OutcomeInfo>(outcomes, new OutcomeInfoInverseByValueComparer());
      Combination = Outcomes.First().Combination;
    }

    /// <summary>
    /// Sum of probabilities by all outcomes has to be 1.
    /// Sum of values by all outcomes has to be <see cref="Value">.
    /// </summary> 
    public SortedSet<OutcomeInfo> Outcomes { get; }

    public override string ToString()
    {
      if (Reroll != null)
      {
        return
          $"C={Combination};V={Value};R={string.Join(",", Reroll)};O={string.Join(",", Outcomes)}";
      }
      return
        $"C={Combination};V={Value};O={string.Join(",", Outcomes)}";
    }
  }
}
