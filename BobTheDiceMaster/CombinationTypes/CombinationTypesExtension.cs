using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public static class CombinationTypesExtension
  {
    public static IReadOnlyList<CombinationTypes> ElementaryCombinations =>
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

    public static IEnumerable<CombinationTypes> GetElementaryCombinationTypes(
      this CombinationTypes combinations)
    {
      foreach (var elementaryCombination in ElementaryCombinations)
      {
        if ((combinations & elementaryCombination) == elementaryCombination)
        {
          yield return elementaryCombination;
        }
      }
    }

    public static bool IsElementary(this CombinationTypes combination)
    {
      return ElementaryCombinations.Any(x => x == combination);
    }

    public static bool IsFromSchool(this CombinationTypes combination)
    {
      return (combination & CombinationTypes.School) == combination;
    }
  }
}
