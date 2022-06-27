using System;
using BobTheDiceMaster.Combinations;

namespace BobTheDiceMaster
{
  class Program
  {
    static void Main(string[] args)
    {
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
  }
}
