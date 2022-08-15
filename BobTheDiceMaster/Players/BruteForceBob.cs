using System.Collections.Generic;

namespace BobTheDiceMaster
{
  class BruteForceBob : IPlayer
  {
    public Decision DecideOnRoll(
      CombinationTypes availableCombinations, DiceRoll currentRoll, int rollsLeft)
    {
      double firstRollScore;
      //TODO[GE]: 3 to constants?
      CombinationTypes bestFirstRollCombination = GetBestCombination(
        availableCombinations, currentRoll, isFirstRoll: rollsLeft == 3, out firstRollScore);

      if (rollsLeft == 1)
      {
        CombinationTypes leastValuableCombination =
          GetLeastValuableCombination(availableCombinations, currentRoll, out double worstScore);
        if (bestFirstRollCombination == CombinationTypes.None || firstRollScore < worstScore)
        {
          if (leastValuableCombination.IsFromSchool())
          {
            return new Score(leastValuableCombination);
          }
          else
          {
            return new CrossOut(leastValuableCombination);
          }
        }
        else
        {
          return new Score(bestFirstRollCombination);
        }
      }

      Dictionary<DiceRoll, double> secondRollScoreCache = new Dictionary<DiceRoll, double>();

      Dictionary<DiceRoll, CombinationTypes> secondRollBestCombinationsCache =
        new Dictionary<DiceRoll, CombinationTypes>();

      int[] bestFirstReroll = null;

      foreach (int[] firstReroll in DiceRoll.Rerolls)
      {
        if (firstReroll.Length == 0)
        {
          continue;
        }
        IReadOnlyList<DiceRoll> firstRerollResults = DiceRoll.RollResults[firstReroll.Length - 1];
        double firstRerollAverage = 0;

        List<(CombinationTypes, double)> combinationsScore = new List<(CombinationTypes, double)>();

        foreach (var firstRerollResult in firstRerollResults)
        {
          DiceRoll secondRoll = currentRoll.ApplyReroll(firstReroll, firstRerollResult);

          // The best average score that can be achieved on the secondRoll result,
          // including optimal reroll. i.e. if first reroll yields firstRerollResult,
          // it's the best.
          // Best available score for secondRoll if no rerolls left.
          double secondRollScore;

          if (rollsLeft == 2)
          {
            secondRollScore = GetBestScore(
              availableCombinations, secondRoll, isFirstRoll: false);
          }
          else
          {
            if (secondRollScoreCache.ContainsKey(secondRoll))
            {
              secondRollScore = secondRollScoreCache[secondRoll];
            }
            else
            {
              secondRollScore = GetBestScore(
                availableCombinations, secondRoll, isFirstRoll: false);

              foreach (int[] secondReroll in DiceRoll.Rerolls)
              {
                if (secondReroll.Length == 0)
                {
                  continue;
                }
                IReadOnlyList<DiceRoll> secondRerollResults = DiceRoll.RollResults[secondReroll.Length - 1];
                double secondRerollAverage = 0;

                foreach (var secondRerollResult in secondRerollResults)
                {
                  DiceRoll thirdRoll = secondRoll.ApplyReroll(secondReroll, secondRerollResult);
                  secondRerollAverage += secondRerollResult.GetProbability()
                    * GetBestScore(
                        availableCombinations, thirdRoll, isFirstRoll: false); ;
                }

                if (secondRerollAverage > secondRollScore)
                {
                  secondRollScore = secondRerollAverage;
                }
              }

              secondRollScoreCache.Add(secondRoll, secondRollScore);
            }
          }

          firstRerollAverage += firstRerollResult.GetProbability() * secondRollScore;
        }

        if (firstRerollAverage > firstRollScore)
        {
          firstRollScore = firstRerollAverage;
          bestFirstReroll = firstReroll;
        }
      }

      if (bestFirstReroll == null)
      {
        return new Score(bestFirstRollCombination);
      }
      else
      {
        return new Reroll(bestFirstReroll);
      }
    }

    private CombinationTypes GetBestCombination(
      CombinationTypes availableCombinations, DiceRoll roll, bool isFirstRoll, out double bestScore)
    {
      bestScore = double.NegativeInfinity;
      CombinationTypes bestCombination = CombinationTypes.None;
      foreach (var combination in availableCombinations.GetElementaryCombinationTypes())
      {
        double rollScore = roll.Score(combination) ?? 0;
        if (rollScore == 0)
        {
          continue;
        }
        if (isFirstRoll && !combination.IsFromSchool())
        {
          rollScore *= 2;
        }
        double combinationScore = rollScore - DiceRoll.AverageScore(combination);
        if (combinationScore > bestScore)
        {
          bestScore = combinationScore;
          bestCombination = combination;
        }
      }
      return bestCombination;
    }

    private CombinationTypes GetLeastValuableCombination(
      CombinationTypes availableCombinations, DiceRoll roll, out double worstScore)
    {
      worstScore = double.PositiveInfinity;
      CombinationTypes leastValuableCombination = CombinationTypes.None;
      foreach (var combination in availableCombinations.GetElementaryCombinationTypes())
      {
        double rollScore;
        if (combination.IsFromSchool())
        {
          rollScore = DiceRoll.AverageScore(combination) - roll.Score(combination) ?? 0;
        }
        else
        {
          rollScore = DiceRoll.AverageScore(combination);
        }
        if (rollScore < worstScore)
        {
          worstScore = rollScore;
          leastValuableCombination = combination;
        }
      }
      return leastValuableCombination;
    }

    private double GetBestScore(
      CombinationTypes availableCombinations, DiceRoll roll, bool isFirstRoll)
    {
      double bestScore = double.NegativeInfinity;
      foreach (var combination in availableCombinations.GetElementaryCombinationTypes())
      {
        double rollScore = roll.Score(combination) ?? 0;
        if (isFirstRoll && !combination.IsFromSchool())
        {
          rollScore *= 2;
        }
        double combinationScore = rollScore - DiceRoll.AverageScore(combination);
        if (combinationScore > bestScore)
        {
          bestScore = combinationScore;
        }
      }
      return bestScore;
    }
  }
}
