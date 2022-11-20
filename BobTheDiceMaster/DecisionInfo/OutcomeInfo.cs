using System;
using System.Collections.Generic;

namespace BobTheDiceMaster
{
  /// <remarks>
  /// IComparable interface is required only to make it serializable.
  /// </remarks>
  public class OutcomeInfo : IComparable<OutcomeInfo>
  {
    public double Value { get; private set; }
    public double Probability { get; private set; }
    public CombinationTypes Combination { get; }
    /// <summary>
    /// True if the combination is scored (and not crossed out) in this outcome.
    /// </summary>
    public bool IsScored { get; }

    /// <remarks>
    /// Parameterless constructor is required only to make it serializable.
    /// </remarks>
    public OutcomeInfo()
    {
      //do nothing
    }

    public OutcomeInfo(
      double value,
      CombinationTypes combination,
      double probability,
      bool isScored)
    {
      Value = value;
      Combination = combination;
      Probability = probability;
      IsScored = isScored;
    }

    public void IncreaseValue(double increment)
    {
      Value += increment;
    }

    public void IncreaseProbability(double increment)
    {
      Probability += increment;
    }

    /// <summary>
    /// TODO[GE]: remove OutcomeInfoInverseByValueComparer?
    /// </summary>
    /// <remarks>
    /// Used only by AWS deserializer.
    /// </remarks>
    public int CompareTo(OutcomeInfo other)
    {
        int compareResult = Comparer<double>.Default.Compare(other.Value, Value);
        // Eliminate 0 to make sure duplicates are not removed.
        // Order of equal values does not mater
        return compareResult == 0 ? 1 : compareResult;
    }

    public override string ToString()
    {
      // G4 to use only 4 digits so that the string representation is more compact.
      // This class is often used in lists.
      return $"{(IsScored ? "S" : "X")} {Combination}({Value:G4},{Probability:G4})";
    }
  }
}
