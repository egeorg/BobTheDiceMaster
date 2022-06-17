using System;
using BobTheDiceMaster.Combinations;

namespace BobTheDiceMaster
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Starting a game...");
      Grade1 g1 = new Grade1();
      double complement = Math.Pow(5, 5) / Math.Pow(6, 5);

      // Yields 1
      Console.WriteLine(
        g1.NDiceRollKSuccessProbability(5, 0)
        + g1.NDiceRollKSuccessProbability(5, 1)
        + g1.NDiceRollKSuccessProbability(5, 2)
        + g1.NDiceRollKSuccessProbability(5, 3)
        + g1.NDiceRollKSuccessProbability(5, 4)
        + g1.NDiceRollKSuccessProbability(5, 5));

      // Yields 1
      Console.WriteLine(g1.FirstRollNSuccessProbability(0)
        + g1.FirstRollNSuccessProbability(1)
        + g1.FirstRollNSuccessProbability(2)
        + g1.FirstRollNSuccessProbability(3)
        + g1.FirstRollNSuccessProbability(4)
        + g1.FirstRollNSuccessProbability(5));


      Console.WriteLine(g1.SecondRollNSuccessProbability(0)
        + g1.SecondRollNSuccessProbability(1)
        + g1.SecondRollNSuccessProbability(2)
        + g1.SecondRollNSuccessProbability(3)
        + g1.SecondRollNSuccessProbability(4)
        + g1.SecondRollNSuccessProbability(5));

      Console.WriteLine(g1.ThirdRollNSuccessProbability(0)
        + g1.ThirdRollNSuccessProbability(1)
        + g1.ThirdRollNSuccessProbability(2)
        + g1.ThirdRollNSuccessProbability(3)
        + g1.ThirdRollNSuccessProbability(4)
        + g1.ThirdRollNSuccessProbability(5));

      //Console.WriteLine(Combinatorics.Cnk(6, 0));
      //Console.WriteLine(Combinatorics.Cnk(6, 1));
      //Console.WriteLine(Combinatorics.Cnk(6, 2));
      //Console.WriteLine(Combinatorics.Cnk(6, 3));
      //Console.WriteLine(Combinatorics.Cnk(6, 4));
      //Console.WriteLine(Combinatorics.Cnk(6, 5));
      //Console.WriteLine(Combinatorics.Cnk(6, 6));
      //Console.WriteLine(
      //  Combinatorics.Cnk(6, 0)
      //  + Combinatorics.Cnk(6, 1)
      //  + Combinatorics.Cnk(6, 2)
      //  + Combinatorics.Cnk(6, 3)
      //  + Combinatorics.Cnk(6, 4)
      //  + Combinatorics.Cnk(6, 5)
      //  + Combinatorics.Cnk(6, 6));

      GameOfSchool game = new GameOfSchool(new HumanPlayer());
      while (!game.IsOver)
      {
        Console.WriteLine("Performing next step");
        game.NextStep();
        Console.WriteLine($"Scored. Current score: {game.Score}");
      }
    }
  }
}
