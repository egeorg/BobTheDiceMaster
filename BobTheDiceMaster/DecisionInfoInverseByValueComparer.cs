using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public class DecisionInfoInverseByValueComparer : IComparer<DecisionInfo>
  {
    public int Compare(DecisionInfo x, DecisionInfo y)
    {
      return Comparer<double>.Default.Compare(y.Value, x.Value);
    }
  }
}
