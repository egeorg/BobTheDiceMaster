using System;
using System.Linq;

using BobTheDiceMaster;

namespace BobTheDiceMaster.UI.Console
{
  class Program
  {
    static void Main(string[] args)
    {
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
  }
}
