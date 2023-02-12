using System.Collections.Generic;

namespace BobTheDiceMaster
{
  /// <summary>
  /// A comparer for <see cref="OutcomeInfo"/> is required
  /// to store them in a <see cref="SortedSet{T}"/>.
  /// When the <see cref="OutcomeInfo"/> are sorted inversely, the most
  /// valuable decision from a <see cref="SortedSet{T}"/> can be retrieved
  /// by a .First() LINQ method.
  /// </summary>
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
