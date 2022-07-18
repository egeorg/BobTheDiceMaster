using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobTheDiceMaster
{
  static class CombinationTypesExtension
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
        CombinationTypes.SmallStreet,
        CombinationTypes.BigStreet,
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
  }
}
