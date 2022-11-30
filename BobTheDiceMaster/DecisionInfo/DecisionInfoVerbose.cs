using System;
using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  /// <summary>
  /// Represents information that justifies certain decision. Namely:
  /// - Average value that the decision yields.
  /// - List of possible outcomes, sorted by their contribution to the decision value.
  /// </summary>
  /// <remarks>
  /// IComparable interface is required only to make it serializable.
  /// </remarks>
  public class DecisionInfoVerbose : DecisionInfo, IComparable<DecisionInfoVerbose>
  {
    /// <remarks>
    /// Parameterless constructor is required only to make it serializable.
    /// </remarks>
    public DecisionInfoVerbose()
    { }

    public DecisionInfoVerbose(
      double value,
      IEnumerable<OutcomeInfo> outcomes,
      int[] reroll = null)
    {
      Value = value;
      Reroll = reroll;
      Outcomes = new SortedSet<OutcomeInfo>(outcomes, new OutcomeInfoInverseByValueComparer());
      Combination = Outcomes.First().Combination;
      //Combination = CombinationTypes.Grade1;//outcomes.First().Combination;
    }

    /// <summary>
    /// TODO[GE]: remove DecisionInfoInverseByValueComparer?
    /// </summary>
    /// <remarks>
    /// Used only by AWS deserializer.
    /// </remarks>
    public int CompareTo(DecisionInfoVerbose other)
    {
      int doubleCompareResult = Comparer<double>.Default.Compare(other.Value, Value);
      // Eliminate 0 to make sure duplicates are not removed.
      // Order of equal values does not mater
      return doubleCompareResult == 0 ? 1 : doubleCompareResult;
    }

    /// <summary>
    /// Sum of probabilities by all outcomes has to be 1.
    /// Sum of values by all outcomes has to be <see cref="Value">.
    /// </summary>
    /// <remarks>
    /// Public setter is required only to make it serializable, otherwise setter can be removed.
    /// </remarks>
    public SortedSet<OutcomeInfo> Outcomes { get; set; }

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
