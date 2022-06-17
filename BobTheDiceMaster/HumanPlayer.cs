using System;
using System.Linq;
using System.Collections.Generic;

namespace BobTheDiceMaster
{
  internal class HumanPlayer : IPlayer
  {
    public IDecision DecideOnRoll(CombinationTypes availableCombinations, DiceRoll currentRoll, int rerloosLeft)
    {
      Console.WriteLine(
        $"Current roll: [1]={currentRoll[0]}, [2]={currentRoll[1]}, [3]={currentRoll[2]}, [4]={currentRoll[3]}, [5]={currentRoll[4]}");
      Console.WriteLine($"Available combinations: {availableCombinations}");
      IDecision inputDecision = null;
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
              //TODO[GE]: handle non-numbers
              diceToReroll = diceToRerollString.Split(" ").Select(x => Int32.Parse(x) - 1).ToList();
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
                    $"Invalid dice number: {dieToReroll + 1}. It has to be a whole number from 1 to 5");
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
                //TODO[GE]: list only available combinations
                Console.WriteLine("Enter a combination type to score (Grade1, Grade2, Grade3, Grade4, Grade5, Grade6, Pair, Three, TwoPairs, Full, Care, SmallStreet, BigStreet, Trash)");
                string combinationString = Console.ReadLine();
                isInputCorrect = Enum.TryParse(typeof(CombinationTypes), combinationString, out object? result);
                if (isInputCorrect)
                {
                  inputDecision = new Score((CombinationTypes)result);
                }
                //TODO[GE]: check if combination is available
              }
            }
            break;
          case "c":
            {
              bool isInputCorrect = false;
              while (!isInputCorrect)
              {
                //TODO[GE]: list only available combinations
                Console.WriteLine("Enter a combination type to cross out (Grade1, Grade2, Grade3, Grade4, Grade5, Grade6, Pair, Three, TwoPairs, Full, Care, SmallStreet, BigStreet, Trash)");
                string combinationString = Console.ReadLine();
                isInputCorrect = Enum.TryParse(typeof(CombinationTypes), combinationString, out object? result);
                if (isInputCorrect)
                {
                  inputDecision = new CrossOut((CombinationTypes)result);
                }
                //TODO[GE]: check if combination is available
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
