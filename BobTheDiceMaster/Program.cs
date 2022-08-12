using System;

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

      Console.WriteLine("Press any key to start a game...");

      Console.ReadKey();
      Console.WriteLine("Starting a game...");
      //GameOfSchool game = new GameOfSchool(new HumanPlayer());
      GameOfSchool game = new GameOfSchool(new VerboseBruteForceBob());

      while (!game.IsOver)
      {
        Console.WriteLine();
        Console.WriteLine("Performing next step");
        game.NextStep();
        if (!game.IsOver)
        {
          Console.WriteLine($"Scored. Current score: {game.Score}");
        }
        Console.ReadKey();
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

      IDecision decision = bob.DecideOnRoll(
        availableCombinations: CombinationTypes.School,
        currentRoll: new DiceRoll(new[] { 2, 3, 5, 5, 5 }),
        rerollsLeft: 3);

      Console.WriteLine(decision);
    }
  }
}
