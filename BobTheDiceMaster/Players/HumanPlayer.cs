using System;
using System.Linq;
using System.Collections.Generic;

namespace BobTheDiceMaster
{
  internal class HumanPlayer : IPlayer
  {
    public Decision DecideOnRoll(CombinationTypes availableCombinations, DiceRoll currentRoll, int rerloosLeft)
    {
      Console.WriteLine(
        $"Current roll: [1]={currentRoll[0]}, [2]={currentRoll[1]}, [3]={currentRoll[2]}, [4]={currentRoll[3]}, [5]={currentRoll[4]}");
      Console.WriteLine($"Available combinations: {availableCombinations}");
      Decision inputDecision = null;
      while (inputDecision == null)
      {
        Console.WriteLine("Select what to do next: roll (r), score (s) or cross our (c)");
        string input = Console.ReadLine();
        switch (input)
        {
          case "r":
            List<int> diceToReroll = null;
            while (diceToReroll == null)
            {
              Console.WriteLine("Enter dice to reroll (their numbers, not values: 1-5), separated by space");
              Console.WriteLine("For example: 2 3 4");
              string diceToRerollString = Console.ReadLine();
              try
              {
                diceToReroll = diceToRerollString.Split(" ").Select(x => Int32.Parse(x) - 1).ToList();
              }
              catch (Exception e) when (e is FormatException || e is OverflowException)
              {
                Console.WriteLine($"Dice number must be an integer between 1 and 5");
                continue;
              }
              if (diceToReroll.Count == 0)
              {
                Console.WriteLine("Enter more than zero dice numbers");
                continue;
              }
              foreach (var dieToReroll in diceToReroll)
              {
                if (dieToReroll < 0 || dieToReroll >= 5)
                {
                  diceToReroll = null;
                  Console.WriteLine(
                    $"Invalid dice number: {dieToReroll + 1}. Dice number must be an integer between 1 and 5");
                }
              }
            }
            inputDecision = new Reroll(diceToReroll);
            break;
          case "s":
            {
              bool isInputCorrect = false;
              while (!isInputCorrect)
              {
                Console.WriteLine(
                  $"Enter a combination type to score ({String.Join(", ", availableCombinations.GetElementaryCombinationTypes())})");
                string combinationString = Console.ReadLine();
                if (!Enum.TryParse(typeof(CombinationTypes), combinationString, out object result))
                {
                  Console.WriteLine($"Can't parse string '{combinationString}' as a combination.");
                  continue;
                }
                if (!availableCombinations.HasFlag((CombinationTypes)result))
                {
                  Console.WriteLine(
                    $"Combination '{combinationString}' is not available (it's already scored or crossed out).");
                  continue;
                }
                if (!currentRoll.Score((CombinationTypes)result).HasValue)
                {
                  Console.WriteLine(
                    $"Combination '{combinationString}' can't be scored for for roll '{currentRoll}'");
                  continue;
                }
                inputDecision = new Score((CombinationTypes)result);
              }
            }
            break;
          case "c":
            {
              bool isInputCorrect = false;
              while (!isInputCorrect)
              {
                Console.WriteLine(
                  $"Enter a combination type to cross out ({String.Join(", ", availableCombinations.GetElementaryCombinationTypes())})");
                string combinationString = Console.ReadLine();
                if (!Enum.TryParse(typeof(CombinationTypes), combinationString, out object result))
                {
                  Console.WriteLine($"Can't parse string '{combinationString}' as a combination.");
                  continue;
                }
                if (!availableCombinations.HasFlag((CombinationTypes)result))
                {
                  Console.WriteLine(
                    $"Combination '{combinationString}' is not available (it's already scored or crossed out).");
                  continue;
                }
                inputDecision = new CrossOut((CombinationTypes)result);
              }
            }
            break;
          default:
            Console.WriteLine($"input not recognized: '{input}'");
            break;
        }
      }
      return inputDecision;
    }
  }
}
