using System.Text;

namespace BobTheDiceMaster.Precomputer
{
  public class BobSchoolProbabilityPrecomputer
  {
    public string Precompute()
    {
      StringBuilder averageScoresString = new StringBuilder();

      foreach (var elementaryCombination in CombinationTypes.All.GetElementaryCombinationTypes())
      {
        averageScoresString.AppendLine($"{{{elementaryCombination}, {GetProbability(elementaryCombination).ToString("R")}}},");
      }

      return averageScoresString.ToString();
    }
    public string PrecomputeCombinationProbabilityOnFirstRoll()
    {
      StringBuilder averageScoresString = new StringBuilder();

      foreach (var elementaryCombination in CombinationTypes.All.GetElementaryCombinationTypes())
      {
        averageScoresString.AppendLine($"{{{elementaryCombination}, {CombinationProbabilityOnFirstRoll(elementaryCombination).ToString("R")}}},");
      }

      return averageScoresString.ToString();
    }

    public static double GetProbability(CombinationTypes combination)
    {
      if (!combination.IsElementary())
      {
        throw new ArgumentException(
          $"Elementary combination expected, but was {combination}");
      }

      double averageProfit = 0;

      Dictionary<DiceRoll, double> secondRollScoreCache = new Dictionary<DiceRoll, double>();

      foreach (DiceRoll firstRoll in DiceRoll.Roll5Results)
      {
        double firstRollScore = 0;

        if (firstRoll.Score(combination) != null)
        {
          firstRollScore = firstRoll.GetProbability();
        }

        int[]? bestFirstReroll = null;

        foreach (int[] firstReroll in DiceRoll.NonEmptyRerolls)
        {
          IReadOnlyList<DiceRoll> firstRerollResults = DiceRoll.RollResults[firstReroll.Length - 1];
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
                IReadOnlyList<DiceRoll> secondRerollResults = DiceRoll.RollResults[secondReroll.Length - 1];
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

      return DiceRoll.Roll5Results.Sum(roll =>
        roll.Score(combination) == null ? 0 : roll.GetProbability());
    }
  }
}
