using System.Collections.Generic;
using System;
using System.Linq;

using BobTheDiceMaster.Combinations;

namespace BobTheDiceMaster
{
  public class BobTheDiceMaster : IPlayer
  {
    //Dictionary<CombinationTypes, Combination> combinations = new Dictionary<CombinationTypes, Combination>()
    //{
    //  { CombinationTypes.Grade1, new Grade1() },
    //  { CombinationTypes.Grade2, new Grade2() },
    //  { CombinationTypes.Grade3, new Grade3() },
    //  { CombinationTypes.Grade4, new Grade4() },
    //  { CombinationTypes.Grade5, new Grade5() },
    //  { CombinationTypes.Grade6, new Grade6() }
    //};

    #region public methods
    public BobTheDiceMaster()
    {
      // TODO
    }

    private static void Log(string message)
    {
      Console.WriteLine(message);
    }

    public IDecision DecideOnRoll(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rollsLeft)
    {
      //Log($"Roll: {currentRoll}");
      //int rerollsLeft = rollsLeft - 1;
      //double? currentBestScore = null;
      //CombinationTypes currentBestCombination = CombinationTypes.None;
      //foreach (var availableCombination in availableCombinations.GetElementaryCombinationTypes())
      //{
      //  Combination combination = combinations[availableCombination];
      //  int? combinationCurrentScore = currentRoll.Score(availableCombination);
      //  if (combinationCurrentScore.HasValue)
      //  {
      //    double currentScore = combinationCurrentScore.Value - combinations[availableCombination].AverageProfit;
      //    if (rerollsLeft == 2)
      //    {
      //      currentScore *= 2;
      //    }
      //    if (!currentBestScore.HasValue || currentScore > currentBestScore)
      //    {
      //      currentBestScore = currentScore;
      //      currentBestCombination = availableCombination;
      //    }
      //  }
      //}

      //if (rerollsLeft == 0)
      //{
      //  if (currentBestScore.HasValue && currentBestScore > 0)
      //  {
      //    return new Score(currentBestCombination);
      //  }
      //  else
      //  {
      //    // TODO: choose wisely between cross out and 1 at school.

      //    // School can't be crossed out according to the game rules.
      //    if ((availableCombinations & ~CombinationTypes.School) == CombinationTypes.None)
      //    {
      //      return new Score(currentBestCombination);
      //    }
      //    return new CrossOut(availableCombinations.GetElementaryCombinationTypes().Last());
      //  }
      //}

      //double? potentialBestScore = null;
      //int[] bestReroll = null;
      //foreach (var diceToReroll in DiceRoll.Rerolls)
      //{
      //  foreach (var availableCombination in availableCombinations.GetElementaryCombinationTypes())
      //  {
      //    Combination combination = combinations[availableCombination];
      //    double score = 0;
      //    if (rerollsLeft == 1)
      //    {
      //      score = combination.SingleRerollAverageProfit(currentRoll, diceToReroll)
      //        - combination.AverageProfit;
      //    }
      //    else if (rerollsLeft == 2)
      //    {
      //      score = combination.TwoRerollAverageProfit(currentRoll, diceToReroll)
      //        - combination.AverageProfit;
      //    }
      //    else
      //    {
      //      throw new ArgumentException($"rerollsLeft can be in [0,2], but was {rerollsLeft}");
      //    }

      //    if (!potentialBestScore.HasValue || score > potentialBestScore.Value)
      //    {
      //      bestReroll = diceToReroll;
      //      potentialBestScore = score;
      //    }
      //  }
      //}

      //Log($"Rerolling dice: {String.Join(", ", bestReroll)}");
      ////TODO: proove that best reroll always exist?
      //return new Reroll(bestReroll);
      throw new NotImplementedException();
    }
    #endregion
  }
}
