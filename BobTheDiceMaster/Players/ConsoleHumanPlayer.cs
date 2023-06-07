using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BobTheDiceMaster
{
  /// <summary>
  /// A <see cref="GameOfSchool"/> player where decisions are read from user using
  /// a <see cref="Console"/> input.
  /// A relevant game context is displayed before requesting a decision.
  /// </summary>
  public class ConsoleHumanPlayer : IPlayer
  {
    /// <summary>
    /// Request and read users decision from a <see cref="Console"/> input.
    /// Firstly, a decision type is read. User has to enter:
    /// <ul>
    /// <li>r for reroll;</li>
    /// <li>s for score;</li>
    /// <li>c for cross out.</li>
    /// </ul>
    /// Then user is requested and has to enter a decision payload:
    /// <ul>
    /// <li>Dice values separated by spaces for a reroll;</li>
    /// <li>Combination type string representation for score or cross out.</li>
    /// </ul>
    /// </summary>
    public async Task<Decision> DecideOnRollAsync(CombinationTypes availableCombinations, DiceRoll currentRoll, int rerollsLeft)
    {
      await Task.Yield();
      Decision inputDecision = null;
      while (inputDecision == null)
      {
        string decisionsAvailable = String.Empty;

        if (rerollsLeft > 0)
        {
          decisionsAvailable += " reroll (r);";
        }

        List<CombinationTypes> combinationsThatCanBeScored =
          availableCombinations
          .GetElementaryCombinationTypes()
          .Where(combination => currentRoll.Score(combination) != null)
          .ToList();

        List<CombinationTypes> combinationsThatCanBeCrossedOut =
          (availableCombinations & ~CombinationTypes.School)
          .GetElementaryCombinationTypes()
          .ToList();

        if (combinationsThatCanBeScored.Any())
        {
          decisionsAvailable += " score (s);";
        }

        if (combinationsThatCanBeCrossedOut.Any())
        {
          decisionsAvailable += " cross out (c);";
        }

        Console.WriteLine($"Select what to do:{decisionsAvailable}");

        string input = Console.ReadLine();
        switch (input)
        {
          case "r":
            if (rerollsLeft == 0)
              break;
            List<int> diceValuesToReroll = null;
            while (diceValuesToReroll == null)
            {
              Console.WriteLine("Enter dice values to reroll (1-6, may be repeated), separated by space");
              string diceValuesToRerollString = Console.ReadLine();
              try
              {
                diceValuesToReroll = diceValuesToRerollString.Split(" ").Select(x => Int32.Parse(x)).ToList();
              }
              catch (Exception e) when (e is FormatException || e is OverflowException)
              {
                Console.WriteLine($"Dice value must be an integer between 1 and 6");
                continue;
              }
              if (diceValuesToReroll.Count == 0)
              {
                Console.WriteLine("Enter more than zero dice values");
                continue;
              }
              foreach (var dieValuesToReroll in diceValuesToReroll)
              {
                if (dieValuesToReroll < 0 || dieValuesToReroll > 6)
                {
                  diceValuesToReroll = null;
                  Console.WriteLine(
                    $"Invalid dice value: {dieValuesToReroll}. Dice number must be an integer between 1 and 6");
                }
              }
            }
            inputDecision = new Reroll(diceValuesToReroll);
            break;
          case "s":
            {
              if (!combinationsThatCanBeScored.Any())
                break;
              bool isInputCorrect = false;
              while (!isInputCorrect)
              {
                Console.WriteLine(
                  $"Enter a combination type to score ({String.Join(", ", combinationsThatCanBeScored)})");
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
                isInputCorrect = true;
                inputDecision = new Score((CombinationTypes)result);
              }
            }
            break;
          case "c":
            {
              if (!combinationsThatCanBeCrossedOut.Any())
                break;
              bool isInputCorrect = false;
              while (!isInputCorrect)
              {
                Console.WriteLine(
                  $"Enter a combination type to cross out ({String.Join(", ", combinationsThatCanBeCrossedOut)})");
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
                isInputCorrect = true;
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
