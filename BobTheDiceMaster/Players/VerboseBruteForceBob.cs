using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class VerboseBruteForceBob : IPlayer
  {
    Dictionary<CombinationTypes, Dictionary<int, Dictionary<DiceRoll, SortedSet<DecisionInfoVerbose>>>> ratedDecisionsCache
       = new Dictionary<CombinationTypes, Dictionary<int, Dictionary<DiceRoll, SortedSet<DecisionInfoVerbose>>>>();

    public Decision DecideOnRoll(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft)
    {
      return DecideOnRollRatedDecisions(
        availableCombinations,
        currentRoll,
        rerollsLeft);
    }

    public Decision DecideOnRollRatedDecisions(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft)
    {
      SortedSet<DecisionInfoVerbose> ratedDecisionsInfo = GetRatedDecisions(
        availableCombinations,
        currentRoll,
        rerollsLeft,
        1);

      DecisionInfoVerbose bestDecisionInfo = ratedDecisionsInfo.First();

      if (bestDecisionInfo.Reroll != null)
      {
        int[] diceToReroll = new int[bestDecisionInfo.Reroll.Length];
        for (int i = 0; i < bestDecisionInfo.Reroll.Length; ++i)
        {
          diceToReroll[i] = currentRoll[bestDecisionInfo.Reroll[i]];
        }
        return new Reroll(diceToReroll, ratedDecisionsInfo);
      }

      if (currentRoll.Score(bestDecisionInfo.Combination) != null)
      {
        return new Score(bestDecisionInfo.Combination, ratedDecisionsInfo);
      }

      return new CrossOut(bestDecisionInfo.Combination, ratedDecisionsInfo);
    }

    private SortedSet<DecisionInfoVerbose> GetRatedDecisions(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft,
      double currentRollProbability)
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
        List<OutcomeInfo> outcomes = new List<OutcomeInfo>();

        foreach (var rerollResult in DiceRoll.RollResults[reroll.Length - 1])
        {
          DiceRoll nextRoll = currentRoll.ApplyReroll(reroll, rerollResult);

          double rerollResultProbability = rerollResult.GetProbability();
          double nextRollOverallProbability = currentRollProbability * rerollResultProbability;

          DecisionInfoVerbose nextRollDecision =
            GetRatedDecisions(
              availableCombinations,
              nextRoll,
              rerollsLeft - 1,
              nextRollOverallProbability)
            .First();

          double rerollResultScore = rerollResultProbability * nextRollDecision.Value;

          rerollScore += rerollResultScore;

          //foreach (OutcomeInfo nextRollOutcome in nextRollDecision.Outcomes)
          //{
          //  double nextRollOutcomeScore =
          //    rerollResultProbability * nextRollOutcome.Value;
          //  double nextRollOutcomeProbability =
          //    rerollResultProbability * nextRollOutcome.Probability;

          //  OutcomeInfo outcome =
          //    outcomes.FirstOrDefault(x =>
          //      x.Combination == nextRollOutcome.Combination
          //      && x.IsScored == nextRollOutcome.IsScored);

          //  if (outcome == null)
          //  {
          //    outcomes.Add(new OutcomeInfo(
          //      nextRollOutcomeScore,
          //      nextRollOutcome.Combination,
          //      nextRollOutcomeProbability,
          //      nextRollOutcome.IsScored));
          //  }
          //  else
          //  {
          //    outcome.IncreaseValue(nextRollOutcomeScore);
          //    outcome.IncreaseProbability(nextRollOutcomeProbability);
          //  }
          //}
        }

        ratedDecisions.Add(
          new DecisionInfoVerbose(rerollScore, outcomes, reroll));
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
            new [] {
              new OutcomeInfo(
                value: combinationScore,
                combination: combination,
                probability: 1,
                isScored: currentRollScore.HasValue)
            } ));
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
