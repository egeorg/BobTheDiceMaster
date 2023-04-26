using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  /// <summary>
  /// Static class with a <see cref="CombinationTypes"/> extension methods.
  /// </summary>
  /// <remarks>
  /// It has to be a separate class since <see cref="CombinationTypes"/> is an <see langword="Enum" />.
  /// </remarks>
  public static class CombinationTypesExtension
  {
    /// <summary>
    /// Returns a set of elementary combinations that constitute a <paramref name="combinations">.
    /// </summary>
    public static IEnumerable<CombinationTypes> GetElementaryCombinationTypes(
      this CombinationTypes combinations)
    {
      foreach (var elementaryCombination in elementaryCombinations)
      {
        if ((combinations & elementaryCombination) == elementaryCombination)
        {
          yield return elementaryCombination;
        }
      }
    }

    /// <summary>
    /// True iff a <paramref name="combination"> is an elementary combination.
    /// </summary>
    public static bool IsElementary(this CombinationTypes combination)
    {
      return elementaryCombinations.Any(x => x == combination);
    }

    /// <summary>
    /// True iff a <paramref name="combination"> is a grade elementary combination or a set of
    /// grade elementary combinations.
    /// </summary>
    public static bool IsFromSchool(this CombinationTypes combination)
    {
      return (combination & CombinationTypes.School) == combination;
    }

    private static IReadOnlyList<CombinationTypes> elementaryCombinations =>
      new List<CombinationTypes>
      {
        CombinationTypes.Grade1,
        CombinationTypes.Grade2,
        CombinationTypes.Grade3,
        CombinationTypes.Grade4,
        CombinationTypes.Grade5,
        CombinationTypes.Grade6,
        CombinationTypes.Pair,
        CombinationTypes.Set,
        CombinationTypes.TwoPairs,
        CombinationTypes.Full,
        CombinationTypes.Care,
        CombinationTypes.LittleStraight,
        CombinationTypes.BigStraight,
        CombinationTypes.Poker,
        CombinationTypes.Trash
      };
  }
}
