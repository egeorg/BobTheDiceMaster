using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobTheDiceMaster
{
  static class CombinationTypesExtension
  {
    private static List<CombinationTypes> elementaryCombinations =
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
        CombinationTypes.SmallStreet,
        CombinationTypes.BigStreet,
        CombinationTypes.Trash
      };
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
  }
}
