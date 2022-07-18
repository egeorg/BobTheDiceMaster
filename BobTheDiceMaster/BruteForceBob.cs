using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobTheDiceMaster
{
  class BruteForceBob : IPlayer
  {
    public IDecision DecideOnRoll(CombinationTypes availableCombinations, DiceRoll currentRoll, int rerloosLeft)
    {
      double averageProfit = 0;

      Dictionary<DiceRoll, double> secondRollScoreCache = new Dictionary<DiceRoll, double>();

      foreach (DiceRoll firstRoll in DiceRoll.Roll5Results)
      {
        double firstRollScore = firstRoll.Score(CombinationTypes.Trash);

        //if ((CombinationType & CombinationTypes.School) != CombinationType)
        //{
        //  firstRollScore *= 2;
        //}

        int[] bestFirstReroll = null;

        foreach (int[] firstReroll in DiceRoll.Rerolls)
        {
          if (firstReroll.Length == 0)
          {
            continue;
          }
          IReadOnlyList<DiceRoll> firstRerollResults = DiceRoll.RollResults[firstReroll.Length - 1];
          double firstRerollAverage = 0;

          foreach (var firstRerollResult in firstRerollResults)
          {
            int[] secondRollDice = new int[DiceRoll.MaxDiceAmount];
            int firstRerollCounter = 0;

            for (int i = 0; i < DiceRoll.MaxDiceAmount; ++i)
            {
              // Indices in DiceRoll.Rerolls are in ascending order
              if (firstRerollCounter < firstReroll.Length && firstReroll[firstRerollCounter] == i)
              {
                secondRollDice[i] = firstRerollResult[firstRerollCounter++];
              }
              else
              {
                secondRollDice[i] = firstRoll[i];
              }
            }
            DiceRoll secondRoll = new DiceRoll(secondRollDice);

            // The best score that can be achieved on the secondRoll result, including optimal reroll.
            // i.e. if first reroll yields firstRerollResult, it's the best.
            double secondRollScore;

            if (secondRollScoreCache.ContainsKey(secondRoll))
            {
              secondRollScore = secondRollScoreCache[secondRoll];
            }
            else
            {
              secondRollScore = secondRoll.Score(CombinationTypes.Trash);

              // null indicates that current score is better than any reroll
              int[] bestSecondReroll = null;

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
                  int[] thirdRollDice = new int[DiceRoll.MaxDiceAmount];
                  int secondRerollCounter = 0;
                  for (int i = 0; i < DiceRoll.MaxDiceAmount; ++i)
                  {
                    // Indices in DiceRoll.Rerolls are in ascending order
                    if (secondRerollCounter < secondReroll.Length && secondReroll[secondRerollCounter] == i)
                    {
                      thirdRollDice[i] = secondRerollResult[secondRerollCounter++];
                    }
                    else
                    {
                      thirdRollDice[i] = secondRoll[i];
                    }
                  }
                  DiceRoll thirdRoll = new DiceRoll(thirdRollDice);
                  secondRerollAverage += secondRerollResult.GetProbability() * thirdRoll.Score(CombinationTypes.Trash);
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

      return new Score(CombinationTypes.Trash);
    }

    private CombinationTypes GetBestCombination(DiceRoll roll)
    {
      throw new NotImplementedException();
    }
  }
}
