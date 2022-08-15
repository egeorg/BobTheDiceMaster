using System.Collections.Generic;

namespace BobTheDiceMaster
{
  class OutcomeInfoInverseByValueComparer : IComparer<OutcomeInfo>
  {
    public int Compare(OutcomeInfo x, OutcomeInfo y)
    {
      int compareResult = Comparer<double>.Default.Compare(y.Value, x.Value);
      // Eliminate 0 to make sure duplicates are not removed.
      // Order of equal values does not mater
      return compareResult == 0 ? 1 : compareResult;
    }
  }
}
