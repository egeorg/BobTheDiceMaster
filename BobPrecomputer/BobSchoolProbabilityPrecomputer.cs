using System.Text;

namespace BobTheDiceMaster.Precomputer
{
  /// <summary>
  /// Class used to precompute probability of a combination: ptobability that
  /// a player would get a specific combination if they aim for a specific
  /// combination and choose optimal rerolls.
  /// </summary>
  /// <remarks>
  /// The result is not used anywhere, I wrote it out of curiosity.
  /// </remarks>
  public class BobSchoolProbabilityPrecomputer
  {
    /// <summary>
    /// Returns a string with all the elementary combinations and probability
    /// to roll the specific combinations in a single turn (roll and two rerolls),
    /// formatted like a dictionary collection initializer, i.e. it can be
    /// copy-pasted directly to the code and be used as a dictionary collecion
    /// initializer. Average score is formatted to represent the exact number
    /// when read, without rounding errors.
    /// </summary>
    public string Precompute()
    {
      StringBuilder averageScoresString = new StringBuilder();

      foreach (var elementaryCombination in CombinationTypes.All.GetElementaryCombinationTypes())
      {
        averageScoresString.AppendLine($"CombinationTypes.{{{elementaryCombination}, {GetProbability(elementaryCombination).ToString("R")}}},");
      }

      return averageScoresString.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    public string PrecomputeCombinationProbabilityOnFirstRoll()
    {
      StringBuilder averageScoresString = new StringBuilder();

      foreach (var elementaryCombination in CombinationTypes.All.GetElementaryCombinationTypes())
      {
        averageScoresString.AppendLine($"CombinationTypes.{{{elementaryCombination}, {CombinationProbabilityOnFirstRoll(elementaryCombination).ToString("R")}}},");
      }

      return averageScoresString.ToString();
    }

    private static double GetProbability(CombinationTypes combination)
    {
      if (!combination.IsElementary())
      {
        throw new ArgumentException(
          $"Elementary combination expected, but was {combination}");
      }

      double averageProfit = 0;

      Dictionary<DiceRoll, double> secondRollScoreCache = new Dictionary<DiceRoll, double>();

      foreach (DiceRoll firstRoll in DiceRoll.RollResultsOfAllDice)
      {
        double firstRollScore = 0;

        if (firstRoll.Score(combination) != null)
        {
          firstRollScore = firstRoll.GetProbability();
        }

        int[]? bestFirstReroll = null;

        foreach (int[] firstReroll in DiceRoll.NonEmptyRerolls)
        {
          IReadOnlyList<DiceRoll> firstRerollResults = DiceRoll.RollResultsByDiceAmount[firstReroll.Length - 1];
          double firstRerollAverage = 0;

          foreach (var firstRerollResult in firstRerollResults)
          {
            DiceRoll secondRoll = firstRoll.ApplyReroll(firstReroll, firstRerollResult);

            // The best score that can be achieved on the secondRoll result, including optimal reroll.
            // i.e. if first reroll yields firstRerollResult, it's the best.
            double secondRollScore;

            if (secondRollScoreCache.ContainsKey(secondRoll))
            {
              secondRollScore = secondRollScoreCache[secondRoll];
            }
            else
            {
              secondRollScore = 0;
              if (secondRoll.Score(combination) != null)
              {
                secondRollScore = secondRoll.GetProbability();
              }

              // null indicates that current score is better than any reroll
              int[]? bestSecondReroll = null;

              foreach (int[] secondReroll in DiceRoll.NonEmptyRerolls)
              {
                IReadOnlyList<DiceRoll> secondRerollResults = DiceRoll.RollResultsByDiceAmount[secondReroll.Length - 1];
                double secondRerollAverage = 0;
                double secondRerollTotal = 0;

                foreach (var secondRerollResult in secondRerollResults)
                {
                  DiceRoll thirdRoll = secondRoll.ApplyReroll(secondReroll, secondRerollResult);
                  if (thirdRoll.Score(combination) != null)
                  {
                    secondRerollAverage += secondRerollResult.GetProbability();
                  }
                  secondRerollTotal += secondRerollResult.GetProbability();
                }

                if (secondRerollAverage > secondRollScore)
                {
                  secondRollScore = secondRerollAverage;
                  bestSecondReroll = secondReroll;
                }
              }

              secondRollScoreCache.Add(secondRoll, secondRollScore);
            }

            firstRerollAverage += firstRerollResult.GetProbability() * secondRollScore;
          }

          if (firstRerollAverage > firstRollScore)
          {
            firstRollScore = firstRerollAverage;
            bestFirstReroll = firstReroll;
          }
        }

        averageProfit += firstRoll.GetProbability() * firstRollScore;
      }

      return averageProfit;
    }

    public static double CombinationProbabilityOnFirstRoll(CombinationTypes combination)
    {
      double totalProbability = 0;

      return DiceRoll.RollResultsOfAllDice.Sum(roll =>
        roll.Score(combination) == null ? 0 : roll.GetProbability());
    }
  }
}
