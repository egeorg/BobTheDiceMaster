using System;

namespace BobTheDiceMaster
{
  public class D6Console : IDie
  {
    public int Roll()
    {
      while (true)
      {
        Console.WriteLine($"Enter a single result of D6 roll. It has to be an integer between 1 and {D6.MaxValue}");
        string result = Console.ReadLine().Trim();

        if (Int32.TryParse(result, out int intResult) && intResult >= 1 && intResult <= D6.MaxValue)
        {
          return intResult;
        }
        Console.WriteLine($"D6 result has to be an integer between 1 and {D6.MaxValue}, but was '{intResult}'");
      }
    }

    public int[] Roll(int diceAmount)
    {
      if (diceAmount < 1)
      {
        throw new ArgumentOutOfRangeException($"diceAmount has to be non-negative, but was '{diceAmount}'");
      }

      bool inputCorrect;
      int[] intResult = new int[diceAmount];

      do
      {
        inputCorrect = true;
        Console.WriteLine(
          $"Enter {diceAmount} results of D6 roll separated by spaces. Every result has to be an integer between 1 and {D6.MaxValue}");
        string stringResult = Console.ReadLine().Trim();

        // null means any whitespace characte is treated as separator
        string[] stringResults = stringResult.Split(null);

        if (stringResults.Length != diceAmount)
        {
          Console.WriteLine(
            $"It has to be {diceAmount} results, but was {stringResults.Length} ({String.Join(",", stringResults)})");
          continue;
        }

        for (int i = 0; i < diceAmount; ++i)
        {
          if (!Int32.TryParse(stringResults[i], out intResult[i]) && intResult[i] >= 1 && intResult[i] <= D6.MaxValue)
          {
            inputCorrect = false;
            Console.WriteLine($"{i}-th D6 result has to be an integer between 1 and {D6.MaxValue}, but was '{intResult}'");
          }
        }
      } while (!inputCorrect);
      return intResult;
    }
  }
}
