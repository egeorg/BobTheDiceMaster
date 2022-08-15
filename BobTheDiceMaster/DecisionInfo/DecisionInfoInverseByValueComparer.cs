using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public class DecisionInfoInverseByValueComparer : IComparer<DecisionInfo>
  {
    public int Compare(DecisionInfo x, DecisionInfo y)
    {
      int doubleCompareResult = Comparer<double>.Default.Compare(y.Value, x.Value);
      // Eliminate 0 to make sure duplicates are not removed.
      // Order of equal values does not mater
      return doubleCompareResult == 0 ? 1 : doubleCompareResult;
    }
  }
}
