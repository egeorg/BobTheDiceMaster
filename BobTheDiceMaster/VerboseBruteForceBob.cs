﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class VerboseBruteForceBob : IPlayer
  {
    Dictionary<CombinationTypes, Dictionary<int, Dictionary<DiceRoll, SortedSet<DecisionInfoVerbose>>>> ratedDecisionsCache
       = new Dictionary<CombinationTypes, Dictionary<int, Dictionary<DiceRoll, SortedSet<DecisionInfoVerbose>>>>();

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
      SortedSet<DecisionInfoVerbose> ratedDecisionsInfo = GetRatedDecisions(
        availableCombinations,
        currentRoll,
        rerollsLeft);

      Console.WriteLine(
        $"Best combinations are: {Environment.NewLine}" +
        $"{string.Join(Environment.NewLine, ratedDecisionsInfo.Take(3))}");

      DecisionInfoVerbose bestDecisionInfo = ratedDecisionsInfo.First();

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

    private SortedSet<DecisionInfoVerbose> GetRatedDecisions(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft)
    {
      InitializeCache(availableCombinations, rerollsLeft);

      if (ratedDecisionsCache[availableCombinations][rerollsLeft].ContainsKey(currentRoll))
      {
        return ratedDecisionsCache[availableCombinations][rerollsLeft][currentRoll];
      }

      SortedSet<DecisionInfoVerbose> noRerollRatedDecisions = GetNoRerollRatedDecisions(
        availableCombinations,
        currentRoll,
        isFirstReroll: rerollsLeft == 3);

      if (rerollsLeft == 1)
      {
        ratedDecisionsCache[availableCombinations][rerollsLeft].Add(currentRoll, noRerollRatedDecisions);
        return noRerollRatedDecisions;
      }

      SortedSet<DecisionInfoVerbose> ratedDecisions =
        new SortedSet<DecisionInfoVerbose>(new DecisionInfoInverseByValueComparer());

      ratedDecisions.Add(noRerollRatedDecisions.First());

      foreach (var reroll in DiceRoll.Rerolls)
      {
        if (reroll.Length == 0)
        {
          continue;
        }

        double rerollScore = 0;
        //TODO[GE]: use noRerollRatedDecisions?
        // bestNextRerollDecision used only to get most valuable combination
        DecisionInfo bestNextRerollDecision =
          new DecisionInfo(double.NegativeInfinity, CombinationTypes.None, null);
        List<OutcomeInfo> outcomes = new List<OutcomeInfo>();

        foreach (var rerollResult in DiceRoll.RollResults[reroll.Length - 1])
        {
          DiceRoll nextRoll = currentRoll.ApplyReroll(reroll, rerollResult);

          DecisionInfoVerbose nextRollDecision =
            GetRatedDecisions(availableCombinations, nextRoll, rerollsLeft - 1).First();

          double rerollResultScore = rerollResult.GetProbability() * nextRollDecision.Value;

          rerollScore += rerollResultScore;

          OutcomeInfo outcome =
            outcomes.FirstOrDefault(x => x.Combination == nextRollDecision.Combination);
          if (outcome == null)
          {
            outcomes.Add(new OutcomeInfo(rerollResultScore, nextRollDecision.Combination));
          }
          else
          {
            outcome.IncreaseValue(rerollResultScore);
          }

          if (nextRollDecision.Value > bestNextRerollDecision.Value)
          {
            bestNextRerollDecision = nextRollDecision;
          }
        }

        ratedDecisions.Add(
          new DecisionInfoVerbose(rerollScore, bestNextRerollDecision.Combination, outcomes, reroll));
      }

      ratedDecisionsCache[availableCombinations][rerollsLeft].Add(currentRoll, ratedDecisions);

      return ratedDecisions;
    }

    private SortedSet<DecisionInfoVerbose> GetNoRerollRatedDecisions(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      bool isFirstReroll)
    {
      SortedSet<DecisionInfoVerbose> ratedDecisions =
        new SortedSet<DecisionInfoVerbose>(new DecisionInfoInverseByValueComparer());

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

        ratedDecisions.Add(
          new DecisionInfoVerbose(
            combinationScore,
            combination,
            new [] { new OutcomeInfo(combinationScore, combination) } ));
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
          new Dictionary<int, Dictionary<DiceRoll, SortedSet<DecisionInfoVerbose>>>());
      }
      if (!ratedDecisionsCache[availableCombinations].ContainsKey(rerollsLeft))
      {
        ratedDecisionsCache[availableCombinations].Add(
          rerollsLeft,
          new Dictionary<DiceRoll, SortedSet<DecisionInfoVerbose>>());
      }
    }
  }
}
