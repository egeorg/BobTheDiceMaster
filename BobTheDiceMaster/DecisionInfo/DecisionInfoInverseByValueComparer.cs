using System.Collections.Generic;

namespace BobTheDiceMaster
{
  /// <summary>
  /// A comparer for <see cref="DecisionInfo"> is required
  /// to store them in a <see cref="SortedSet{T}">.
  /// When the <see cref="DecisionInfo"> are sorted inversely, the most
  /// valuable decision from a <see cref="SortedSet{T}"> can be retrieved
  /// by a .First() LINQ method.
  /// </summary>
  internal class DecisionInfoInverseByValueComparer : IComparer<DecisionInfo>
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
