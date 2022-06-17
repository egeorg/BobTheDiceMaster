using System;

namespace BobTheDiceMaster
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Stargin a game with Bob...");

      GameOfSchool game = new GameOfSchool(new HumanPlayer());
      while (true)
      {
        Console.WriteLine("Performing next step");
        game.NextStep();
        Console.WriteLine($"Scored. Current score: {game.Score}");
      }
    }
  }
}
