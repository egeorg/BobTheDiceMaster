using System.Collections.Generic;

namespace BobTheDiceMaster
{
  public class RecursiveBruteForceBob : IPlayer
  {
    Dictionary<CombinationTypes, Dictionary<int, Dictionary<DiceRoll, DecisionInfo>>> decisionCache
       = new Dictionary<CombinationTypes, Dictionary<int, Dictionary<DiceRoll, DecisionInfo>>>();

    public IDecision DecideOnRoll(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft)
    {
      DecisionInfo bestDecisionInfo = GetBestDecision(
        availableCombinations,
        currentRoll,
        rerollsLeft);

      if (bestDecisionInfo.Reroll != null)
      {
        return new Reroll(bestDecisionInfo.Reroll);
      }

      if (currentRoll.Score(bestDecisionInfo.Combination) != null)
      {
        return new Score(bestDecisionInfo.Combination);
      }

      return new CrossOut(bestDecisionInfo.Combination);
    }

    private DecisionInfo GetBestDecision(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft)
    {
      InitializeCache(availableCombinations, rerollsLeft);

      if (decisionCache[availableCombinations][rerollsLeft].ContainsKey(currentRoll))
      {
        return decisionCache[availableCombinations][rerollsLeft][currentRoll];
      }

      DecisionInfo noRerollBestDecision = GetNoRerollBestDecision(
        availableCombinations,
        currentRoll,
        isFirstReroll: rerollsLeft == 3);

      if (rerollsLeft == 1)
      {
        decisionCache[availableCombinations][rerollsLeft].Add(currentRoll, noRerollBestDecision);
        return noRerollBestDecision;
      }

      DecisionInfo bestRerollDecision =
        new DecisionInfo(double.NegativeInfinity, CombinationTypes.None, null);

      foreach (var reroll in DiceRoll.Rerolls)
      {
        if (reroll.Length == 0)
        {
          continue;
        }

        double rerollScore = 0;
        DecisionInfo bestNextRerollDecision =
          new DecisionInfo(double.NegativeInfinity, CombinationTypes.None, null);

        foreach (var rerollResult in DiceRoll.RollResults[reroll.Length - 1])
        {
          DiceRoll nextRoll = currentRoll.ApplyReroll(reroll, rerollResult);

          DecisionInfo nextRollDecision =
            GetBestDecision(availableCombinations, nextRoll, rerollsLeft - 1);

          rerollScore += rerollResult.GetProbability() * nextRollDecision.Value;

          if (nextRollDecision.Value > bestNextRerollDecision.Value)
          {
            bestNextRerollDecision = nextRollDecision;
          }
        }

        if (rerollScore > bestRerollDecision.Value)
        {
          bestRerollDecision =
            new DecisionInfo(rerollScore, bestNextRerollDecision.Combination, reroll);
        }
      }

      if (noRerollBestDecision.Value > bestRerollDecision.Value)
      {
        decisionCache[availableCombinations][rerollsLeft].Add(currentRoll, noRerollBestDecision);
        return noRerollBestDecision;
      }

      decisionCache[availableCombinations][rerollsLeft].Add(currentRoll, bestRerollDecision);
      return bestRerollDecision;
    }

    private DecisionInfo GetNoRerollBestDecision(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      bool isFirstReroll)
    {
      double bestScore = double.NegativeInfinity;
      CombinationTypes bestCombination = CombinationTypes.None;

      foreach (var combination in availableCombinations.GetElementaryCombinationTypes())
      {
        double? currentRollScore = currentRoll.Score(combination);
        if (currentRollScore.HasValue && isFirstReroll && !combination.IsFromSchool())
        {
          currentRollScore *= 2;
        }

        //TODO[GE]: Does AverageScore depend on availableCombinations?
        double combinationScore = (currentRollScore ?? 0)
          - DiceRoll.AverageScore(combination);

        if (combinationScore > bestScore)
        {
          bestScore = combinationScore;
          bestCombination = combination;
        }
      }

      return new DecisionInfo(bestScore, bestCombination);
    }

    private void InitializeCache(
      CombinationTypes availableCombinations,
      int rerollsLeft)
    {
      if (!decisionCache.ContainsKey(availableCombinations))
      {
        decisionCache.Add(availableCombinations, new Dictionary<int, Dictionary<DiceRoll, DecisionInfo>>());
      }
      if (!decisionCache[availableCombinations].ContainsKey(rerollsLeft))
      {
        decisionCache[availableCombinations].Add(rerollsLeft, new Dictionary<DiceRoll, DecisionInfo>());
      }
    }
  }
}
