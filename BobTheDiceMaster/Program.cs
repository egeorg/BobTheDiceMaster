using System;
using System.Linq;

namespace BobTheDiceMaster
{
  class Program
  {
    static void Main(string[] args)
    {
      //foreach (var combination in CombinationTypesExtension.ElementaryCombinations)
      //{
      //  Console.WriteLine($"{combination}: {DiceRoll.AverageScore(combination)}");
      //}

      Console.WriteLine("Starting a game...");
      //GameOfSchool game = new GameOfSchool(new HumanPlayer());
      GameOfSchool game = new GameOfSchool(new VerboseBruteForceBob());

      while (!game.IsGameOver)
      {
        Console.WriteLine();
        Console.WriteLine("Performing next step");
        PerformNextStep(game);
        if (!game.IsGameOver)
        {
          Console.WriteLine($"Scored. Current score: {game.Score}");
        }
        else
        {
          Console.WriteLine($"Game over! Score is {game.Score}");
        }
        Console.ReadKey();
      }
    }

    public static void PerformNextStep(GameOfSchool game)
    {
      DiceRoll roll = game.GenerateRoll();
      while (!game.IsTurnOver)
      {
        Console.WriteLine($"Considering roll {roll}. Waiting for a decision.");

        Decision decision = game.GenerateAndApplyDecision();

        Console.WriteLine();
        Console.WriteLine($"Decision is {decision}.");
        Console.WriteLine(
          $"Best combinations are: {Environment.NewLine}" +
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
        Console.WriteLine($"{i} dice rerolls probability sum: {pSum}");
      }
    }

    static void TestDiceRollEquals()
    {
      DiceRoll r1 = new DiceRoll(new[] { 1, 1, 6, 6, 6 });
      DiceRoll r2 = new DiceRoll(new[] { 6, 1, 6, 1, 6 });
      DiceRoll r3 = new DiceRoll(new[] { 1, 2, 3, 4, 5 });
      Console.WriteLine(r1.Equals(r2));
      Console.WriteLine(r1.Equals(r3));
    }

    static void TestVerboseBob()
    {
      VerboseBruteForceBob bob = new VerboseBruteForceBob();

      Decision decision = bob.DecideOnRoll(
        availableCombinations: (CombinationTypes)(CombinationTypes.All - CombinationTypes.Grade2),
        currentRoll: new DiceRoll(new[] { 1, 1, 2, 5, 6 }),
        rerollsLeft: 3);

      Console.WriteLine(
        $"Best combinations are: {Environment.NewLine}" +
        $"{string.Join(Environment.NewLine, decision.RatedDecisionInfo.Take(3))}");

      Console.WriteLine(decision);
    }
  }
}
