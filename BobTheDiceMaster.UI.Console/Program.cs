using System;
using System.Linq;

using BobTheDiceMaster;

namespace BobTheDiceMaster.UI.Console
{
  class Program
  {
    static void Main(string[] args)
    {
      //foreach (var combination in CombinationTypesExtension.ElementaryCombinations)
      //{
      //  Console.WriteLine($"{{{combination}, {DiceRoll.AverageScore(combination):R}}}");
      //}
      TestVerboseBob();
      System.Console.ReadLine();
      System.Console.WriteLine("Starting a game...");
      GameOfSchool game = new GameOfSchool(new VerboseBruteForceBob(), new D6());

      while (!game.IsGameOver)
      {
        System.Console.WriteLine();
        System.Console.WriteLine("Performing next step");
        PerformNextStep(game);
        if (!game.IsGameOver)
        {
          System.
          Console.WriteLine($"Scored. Current score: {game.Score}");
        }
        else
        {
          System.Console.WriteLine($"Game over! Score is {game.Score}");
        }
        System.Console.ReadKey();
      }
    }

    public static void PerformNextStep(GameOfSchool game)
    {
      DiceRoll roll = game.GenerateRoll().Roll;
      while (!game.IsTurnOver)
      {
        System.Console.WriteLine($"Considering roll {roll}. Waiting for a decision.");

        Decision decision = game.GenerateAndApplyDecision();

        System.Console.WriteLine(
          Environment.NewLine +
          $"Decision is {decision}" + Environment.NewLine +
          $"Best combinations are:" + Environment.NewLine +
          $"{string.Join(Environment.NewLine, decision.RatedDecisionInfo.Take(3))}");
        if (decision is Reroll)
        {
          game.GenerateAndApplyReroll();
        }
      }
    }

    static void TestRollProbability()
    {
      for (int i = 1; i < DiceRoll.MaxDiceAmount; ++i)
      {
        double pSum = 0;
        foreach (var reroll in DiceRoll.RollResults[i])
        {
          pSum += reroll.GetProbability();
        }
        System.Console.WriteLine($"{i} dice rerolls probability sum: {pSum}");
      }
    }

    static void TestDiceRollEquals()
    {
      DiceRoll r1 = new DiceRoll(new[] { 1, 1, 6, 6, 6 });
      DiceRoll r2 = new DiceRoll(new[] { 6, 1, 6, 1, 6 });
      DiceRoll r3 = new DiceRoll(new[] { 1, 2, 3, 4, 5 });
      System.Console.WriteLine(r1.Equals(r2));
      System.Console.WriteLine(r1.Equals(r3));
    }

    static void TestVerboseBob()
    {
      VerboseBruteForceBob bob = new VerboseBruteForceBob();

      Decision decision = bob.DecideOnRoll(
        availableCombinations: (CombinationTypes)(CombinationTypes.All
        - CombinationTypes.Grade1
        - CombinationTypes.Grade2
        - CombinationTypes.Grade3
        - CombinationTypes.Grade6
        - CombinationTypes.Grade4
        - CombinationTypes.Poker
        - CombinationTypes.Full
        - CombinationTypes.TwoPairs
        - CombinationTypes.Set
        - CombinationTypes.Trash),
        currentRoll: new DiceRoll(new[] { 2, 2, 4, 4, 6 }),
        rerollsLeft: 3);

      System.Console.WriteLine(
        $"Best combinations are: {Environment.NewLine}" +
        $"{string.Join(Environment.NewLine, decision.RatedDecisionInfo)}");

      System.Console.WriteLine(decision);
    }
  }
}
