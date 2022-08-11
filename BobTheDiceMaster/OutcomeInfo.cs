using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobTheDiceMaster
{
  public class OutcomeInfo
  {
    public double Value { get; private set; }
    public double? Probability { get; }
    public CombinationTypes Combination { get; }

    public OutcomeInfo(double value, CombinationTypes combination, double? probability = null)
    {
      Value = value;
      Combination = combination;
      Probability = probability;
    }

    public void IncreaseValue(double increment)
    {
      Value += increment;
    }

    public override string ToString()
    {
      if (Probability.HasValue)
      {
        return $"{Combination}({Value:G4},{Probability:G4})";
      }
      return $"{Combination}({Value:G4})";
    }
  }
}
