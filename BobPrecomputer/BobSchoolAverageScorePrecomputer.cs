﻿using System.Text;

namespace BobTheDiceMaster.Precomputer
{
  public class BobSchoolAverageScorePrecomputer
  {
    public string Precompute()
    {
      StringBuilder averageScoresString = new StringBuilder();

      foreach (var elementaryCombination in CombinationTypes.All.GetElementaryCombinationTypes())
      {
        averageScoresString.AppendLine($"{{{elementaryCombination}, {AverageScore(elementaryCombination).ToString("R")}}},");
      }

      return averageScoresString.ToString();
    }

    public static double AverageScore(CombinationTypes combination)
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
        double firstRollScore = firstRoll.Score(combination) ?? 0;

        if (!combination.IsFromSchool())
        {
          firstRollScore *= 2;
        }

        int[]? bestFirstReroll = null;

        foreach (int[] firstReroll in DiceRoll.NonEmptyRerolls)
        {
          IReadOnlyList<DiceRoll> firstRerollResults = DiceRoll.RollResultsByDiceAmount[firstReroll.Length - 1];
          double firstRerollAverage = 0;

          foreach (var firstRerollResult in firstRerollResults)
          {
            DiceRoll secondRoll = firstRoll.ApplyRerollAtIndexes(firstReroll, firstRerollResult);

            // The best score that can be achieved on the secondRoll result, including optimal reroll.
            // i.e. if first reroll yields firstRerollResult, it's the best.
            double secondRollScore;

            if (secondRollScoreCache.ContainsKey(secondRoll))
            {
              secondRollScore = secondRollScoreCache[secondRoll];
            }
            else
            {
              secondRollScore = secondRoll.Score(combination) ?? 0;

              // null indicates that current score is better than any reroll
              int[]? bestSecondReroll = null;

              foreach (int[] secondReroll in DiceRoll.NonEmptyRerolls)
              {
                IReadOnlyList<DiceRoll> secondRerollResults = DiceRoll.RollResultsByDiceAmount[secondReroll.Length - 1];
                double secondRerollAverage = 0;

                foreach (var secondRerollResult in secondRerollResults)
                {
                  DiceRoll thirdRoll = secondRoll.ApplyRerollAtIndexes(secondReroll, secondRerollResult);
                  secondRerollAverage += secondRerollResult.GetProbability() * (thirdRoll.Score(combination) ?? 0);
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
  }
}
