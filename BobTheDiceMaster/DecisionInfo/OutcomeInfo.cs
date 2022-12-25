using System;
using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public class OutcomeInfo
  {
    public double Value { get; private set; }
    public double Probability { get; private set; }
    public CombinationTypes Combination { get; }

    /// <summary>
    /// True if the combination is scored (and not crossed out) in this outcome.
    /// </summary>
    public bool IsScored { get; set; }

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

    public override string ToString()
    {
      // G4 to use only 4 digits so that the string representation is more compact.
      // This class is often used in lists.
      return $"{(IsScored ? "S" : "X")} {Combination}({Value:G4},{Probability:G4})";
    }
  }
}
