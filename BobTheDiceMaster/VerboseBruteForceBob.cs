using System;
using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class VerboseBruteForceBob : IPlayer
  {
    Dictionary<CombinationTypes, Dictionary<int, Dictionary<DiceRoll, SortedSet<DecisionInfo>>>> ratedDecisionsCache
       = new Dictionary<CombinationTypes, Dictionary<int, Dictionary<DiceRoll, SortedSet<DecisionInfo>>>>();

    public IDecision DecideOnRoll(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft)
    {
      return DecideOnRollRatedDecisions(
        availableCombinations,
        currentRoll,
        rerollsLeft);
    }

    public IDecision DecideOnRollRatedDecisions(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft)
    {
      SortedSet<DecisionInfo> ratedDecisionsInfo = GetRatedDecisions(
        availableCombinations,
        currentRoll,
        rerollsLeft);

      Console.WriteLine(
        $"Best combinations are: {Environment.NewLine}" +
        $"{string.Join(Environment.NewLine, ratedDecisionsInfo.Take(3))}");

      DecisionInfo bestDecisionInfo = ratedDecisionsInfo.First();

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

    private SortedSet<DecisionInfo> GetRatedDecisions(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft)
    {
      InitializeCache(availableCombinations, rerollsLeft);

      if (ratedDecisionsCache[availableCombinations][rerollsLeft].ContainsKey(currentRoll))
      {
        return ratedDecisionsCache[availableCombinations][rerollsLeft][currentRoll];
      }

      SortedSet<DecisionInfo> noRerollRatedDecisions = GetNoRerollRatedDecisions(
        availableCombinations,
        currentRoll,
        isFirstReroll: rerollsLeft == 3);

      if (rerollsLeft == 1)
      {
        ratedDecisionsCache[availableCombinations][rerollsLeft].Add(currentRoll, noRerollRatedDecisions);
        return noRerollRatedDecisions;
      }

      SortedSet<DecisionInfo> ratedDecisions =
        new SortedSet<DecisionInfo>(new DecisionInfoInverseByValueComparer());

      ratedDecisions.Add(noRerollRatedDecisions.First());

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
            GetRatedDecisions(availableCombinations, nextRoll, rerollsLeft - 1).First();

          rerollScore += rerollResult.GetProbability() * nextRollDecision.Value;

          if (nextRollDecision.Value > bestNextRerollDecision.Value)
          {
            bestNextRerollDecision = nextRollDecision;
          }
        }

        ratedDecisions.Add(
          new DecisionInfo(rerollScore, bestNextRerollDecision.Combination, reroll));
      }

      ratedDecisionsCache[availableCombinations][rerollsLeft].Add(currentRoll, ratedDecisions);

      return ratedDecisions;
    }

    private SortedSet<DecisionInfo> GetNoRerollRatedDecisions(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      bool isFirstReroll)
    {
      SortedSet<DecisionInfo> ratedDecisions =
        new SortedSet<DecisionInfo>(new DecisionInfoInverseByValueComparer());

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

        ratedDecisions.Add(new DecisionInfo(combinationScore, combination));
      }

      return ratedDecisions;
    }

    private void InitializeCache(
      CombinationTypes availableCombinations,
      int rerollsLeft)
    {
      if (!ratedDecisionsCache.ContainsKey(availableCombinations))
      {
        ratedDecisionsCache.Add(
          availableCombinations,
          new Dictionary<int, Dictionary<DiceRoll, SortedSet<DecisionInfo>>>());
      }
      if (!ratedDecisionsCache[availableCombinations].ContainsKey(rerollsLeft))
      {
        ratedDecisionsCache[availableCombinations].Add(
          rerollsLeft,
          new Dictionary<DiceRoll, SortedSet<DecisionInfo>>());
      }
    }
  }
}
