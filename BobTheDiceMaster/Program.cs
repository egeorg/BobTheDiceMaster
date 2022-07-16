using System;
using BobTheDiceMaster.Combinations;

namespace BobTheDiceMaster
{
  class Program
  {
    static void Main(string[] args)
    {
      //TestRollProbability();
      Grade6 g6 = new Grade6();

      Console.WriteLine(g6.GetAverageProfit());

      Console.WriteLine(g6.BaseGetAverageProfit());

      Console.WriteLine("Starting a game...");
      //GameOfSchool game = new GameOfSchool(new HumanPlayer());
      GameOfSchool game = new GameOfSchool(new BobTheDiceMaster());

      while (!game.IsOver)
      {
        Console.WriteLine("Performing next step");
        game.NextStep();
        Console.WriteLine($"Scored. Current score: {game.Score}");
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
  }
}
