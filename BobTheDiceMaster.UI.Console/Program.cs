using System;
using System.Linq;

using BobTheDiceMaster.GameOfSchool;

namespace BobTheDiceMaster.UI.Console
{
  class Program
  {
    static void Main(string[] args)
    {
      System.Console.WriteLine("Starting a game...");
      GameOfSchoolWithDiceAndAIPlayer game =
        new GameOfSchoolWithDiceAndAIPlayer(
          new GameOfSchool.GameOfSchool(),
          new D6(),
          new ConsoleHumanPlayer());

      while (!game.IsGameOver)
      {
        PerformNextStep(game);
        if (!game.IsGameOver)
        {
          System.Console.WriteLine($"Scored. Current score: {game.Score}");
        }
        else
        {
          System.Console.WriteLine($"Game over! Score is {game.Score}");
        }
        System.Console.WriteLine($"Press any key to continue...");
        System.Console.ReadKey();
        System.Console.Clear();
      }
    }

    public static void PerformNextStep(GameOfSchoolWithDiceAndAIPlayer game)
    {
      game.GenerateRoll();
      while (!game.IsTurnOver)
      {
        System.Console.WriteLine($"Available combinations: {String.Join(", ", game.AllowedCombinationTypes)}");
        System.Console.WriteLine($"Rerolls left: {game.RerollsLeft}");
        System.Console.WriteLine($"Current roll is {game.CurrentRoll.Roll}. Waiting for a decision.");

        Decision decision = game.GenerateAndApplyDecision();

        System.Console.WriteLine(
          Environment.NewLine +
          $"Decision is {decision}" + Environment.NewLine);

        if (decision.RatedDecisionInfo != null)
        {
          System.Console.WriteLine(
            $"Top 3 decisions are:" + Environment.NewLine + 
            $"{string.Join(Environment.NewLine, decision.RatedDecisionInfo.Take(3))}");
        }
      }
    }
  }
}
