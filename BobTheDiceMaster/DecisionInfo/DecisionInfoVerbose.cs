using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  /// <summary>
  /// In addition to information presented by <see cref="DecisionInfo">, it
  /// represents information that justifies certain decision. Namely list of
  /// possible outcomes, sorted by their contribution to the decision value.
  /// </summary>
  public class DecisionInfoVerbose : DecisionInfo
  {
    /// <summary>
    /// Sum of probabilities by all outcomes has to be 1.
    /// Sum of values by all outcomes has to be <see cref="Value">.
    /// </summary>
    public SortedSet<OutcomeInfo> Outcomes { get; }

    public DecisionInfoVerbose(
      double value,
      IEnumerable<OutcomeInfo> outcomes,
      int[] reroll = null)
    {
      Value = value;
      DiceValuesToReroll = reroll;
      Outcomes = new SortedSet<OutcomeInfo>(outcomes, new OutcomeInfoInverseByValueComparer());
      Combination = Outcomes.First().Combination;
    }

    public override string ToString()
    {
      if (DiceValuesToReroll != null)
      {
        return
          $"C={Combination};V={Value};R={string.Join(",", DiceValuesToReroll)};O={string.Join(",", Outcomes)}";
      }
      return
        $"C={Combination};V={Value};O={string.Join(",", Outcomes)}";
    }
  }
}
