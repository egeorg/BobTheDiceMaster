using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BobTheDiceMaster
{
  public class ParallelVerboseBruteForceBob : IPlayer
  {
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
        1,
        doParallelization: true);

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

    private ConcurrentDictionary<CombinationTypes, ConcurrentDictionary<int, ConcurrentDictionary<DiceRoll, SortedSet<DecisionInfoVerbose>>>> ratedDecisionsCache
     = new ConcurrentDictionary<CombinationTypes, ConcurrentDictionary<int, ConcurrentDictionary<DiceRoll, SortedSet<DecisionInfoVerbose>>>>();

    private int[] redundantCalculations = new int[4];
    private int[] cacheAdds = new int[4];

    private SortedSet<DecisionInfoVerbose> GetRatedDecisions(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft,
      double currentRollProbability,
      bool doParallelization)
    {
      if (doParallelization)
      {
        Console.WriteLine($"[{DateTime.UtcNow}] Start GetRatedDecisions paralleled");
      }
      try
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
          ratedDecisionsCache[availableCombinations][rerollsLeft].TryAdd(currentRoll, noRerollRatedDecisions);
          return noRerollRatedDecisions;
        }

        SortedSet<DecisionInfoVerbose> ratedDecisions =
          new SortedSet<DecisionInfoVerbose>(new DecisionInfoInverseByValueComparer());

        ratedDecisions.Add(noRerollRatedDecisions.First());

        if (doParallelization)
        {
          ConcurrentBag<DecisionInfoVerbose> decisionInfos = new ConcurrentBag<DecisionInfoVerbose>();
          //DiceRoll.NonEmptyRerolls.AsParallel()
          //  .WithDegreeOfParallelism(31)
          //  .ForAll(reroll =>
          //    decisionInfos.Add(GetDecisionInfoForReroll(
          //      currentRoll,
          //      currentRollProbability,
          //      availableCombinations,
          //      rerollsLeft,
          //      reroll,
          //      verbose: true)));

          //System.Threading.Tasks.Parallel.ForEach(
          //  DiceRoll.NonEmptyRerolls,
          //  reroll =>
          //    decisionInfos.Add(GetDecisionInfoForReroll(
          //      currentRoll,
          //      currentRollProbability,
          //      availableCombinations,
          //      rerollsLeft,
          //      reroll,
          //      verbose: true))
          //  );

          List<Task> taskList = new List<Task>();

          foreach (var reroll in DiceRoll.NonEmptyRerolls)
          {
            Task task = new Task(() =>
              decisionInfos.Add(GetDecisionInfoForReroll(
                currentRoll,
                currentRollProbability,
                availableCombinations,
                rerollsLeft,
                reroll,
                verbose: true)));
            taskList.Add(task);
            task.Start();
          }
          Task.WaitAll(taskList.ToArray());

          foreach (var decisionInfo in decisionInfos)
          {
            ratedDecisions.Add(decisionInfo);
          }
        }
        else
        {
          foreach (var reroll in DiceRoll.NonEmptyRerolls)
          {
            if (reroll.Length == 0)
            {
              continue;
            }

            ratedDecisions.Add(
              GetDecisionInfoForReroll(
                currentRoll, currentRollProbability, availableCombinations, rerollsLeft, reroll));
          }
        }

        return ratedDecisions;
      }
      finally
      {
        if (doParallelization)
        {
          Console.WriteLine($"[{DateTime.UtcNow}] End GetRatedDecisions paralleled");
        }
      }
    }

    private void AddDecisionToCache(
      CombinationTypes availableCombinations,
      int rerollsLeft,
      DiceRoll currentRoll,
      SortedSet<DecisionInfoVerbose> ratedDecisions)
    {
      if (!ratedDecisionsCache[availableCombinations][rerollsLeft].TryAdd(currentRoll, ratedDecisions))
      {
        redundantCalculations[rerollsLeft]++;
      }
      cacheAdds[rerollsLeft]++;
    }

    private DecisionInfoVerbose GetDecisionInfoForReroll(
      DiceRoll currentRoll,
      double currentRollProbability,
      CombinationTypes availableCombinations,
      int rerollsLeft,
      int[] reroll,
      bool verbose = false)
    {
      //TODO[ge]: not used without verbose
      DateTime startCalculationTime = DateTime.UtcNow;

      double rerollScore = 0;
      List<OutcomeInfo> outcomes = new List<OutcomeInfo>();

      //Can be paralleled here
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
            nextRollOverallProbability,
            doParallelization: false)
          .First();

        double rerollResultScore = rerollResultProbability * nextRollDecision.Value;

        rerollScore += rerollResultScore;

        //Can be paralleled here
        foreach (OutcomeInfo nextRollOutcome in nextRollDecision.Outcomes)
        {
          double nextRollOutcomeScore =
            rerollResultProbability * nextRollOutcome.Value;
          double nextRollOutcomeProbability =
            rerollResultProbability * nextRollOutcome.Probability;

          OutcomeInfo outcome =
            outcomes.FirstOrDefault(x =>
              x.Combination == nextRollOutcome.Combination
              && x.IsScored == nextRollOutcome.IsScored);

          if (outcome == null)
          {
            outcomes.Add(new OutcomeInfo(
              nextRollOutcomeScore,
              nextRollOutcome.Combination,
              nextRollOutcomeProbability,
              nextRollOutcome.IsScored));
          }
          else
          {
            outcome.IncreaseValue(nextRollOutcomeScore);
            outcome.IncreaseProbability(nextRollOutcomeProbability);
          }
        }
      }

      if (verbose)
      {
        Console.WriteLine(
          $"[{DateTime.UtcNow}] DecisionInfo calculated on thread [{Environment.CurrentManagedThreadId}] in '{DateTime.UtcNow - startCalculationTime}'");
      }

      return new DecisionInfoVerbose(rerollScore, outcomes, reroll);
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
            new[] {
              new OutcomeInfo(
                value: combinationScore,
                combination: combination,
                probability: 1,
                isScored: currentRollScore.HasValue)
            }));
      }

      return ratedDecisions;
    }

    private void InitializeCache(
      CombinationTypes availableCombinations,
      int rerollsLeft)
    {
      if (!ratedDecisionsCache.ContainsKey(availableCombinations))
      {
        ratedDecisionsCache.TryAdd(
          availableCombinations,
          new ConcurrentDictionary<int, ConcurrentDictionary<DiceRoll, SortedSet<DecisionInfoVerbose>>>());
      }
      if (!ratedDecisionsCache[availableCombinations].ContainsKey(rerollsLeft))
      {
        ratedDecisionsCache[availableCombinations].TryAdd(
          rerollsLeft,
          new ConcurrentDictionary<DiceRoll, SortedSet<DecisionInfoVerbose>>());
      }
    }
  }
}

