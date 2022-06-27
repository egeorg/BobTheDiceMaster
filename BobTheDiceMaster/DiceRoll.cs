using System;
using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class DiceRoll
  {
    #region private fields
    private static D6 die = new D6();

    private int[] dice = new int[DiceAmount];
    #endregion

    #region private methods
    private static int RollSingleDice()
    {
      return die.Roll();
    }
    #endregion

    #region public constants and properties
    public int this[int i]
    {
      get {
        return dice[i];
      }
    }

    public const int DiceAmount = 5;
    #endregion

    #region public methods
    public DiceRoll(int[] dice)
    {
      if (dice.Length != DiceAmount)
      {
        throw new ArgumentException(
          $"Exactly {DiceAmount} dice expected, but was '{dice.Length}'");
      }

      for (int i = 0; i < DiceAmount; ++i)
      {
        if (dice[i] < 1 || dice[i] > D6.MaxValue)
        {
          throw new ArgumentException(
            $"Die value has to be between 1 and {D6.MaxValue}, but {i}-th value was '{dice[i]}'");
        }
        this.dice[i] = dice[i];
      }
    }

    public int Sum()
    {
      return dice.Sum();
    }

    public static DiceRoll GenerateNew()
    {
      int[] dice = new int[DiceAmount];

      for (int i = 0; i < DiceAmount; ++i)
      {
        dice[i] = RollSingleDice();
      }

      return new DiceRoll(dice);
    }

    public void Reroll(IReadOnlyCollection<int> diceToReroll)
    {
      if (diceToReroll.Count > DiceAmount)
      {
        throw new ArgumentException($"Can't reroll more than {DiceAmount} dice.");
      }

      foreach (var dieNumber in diceToReroll)
      {
        if (dieNumber < 0 || dieNumber >= DiceAmount)
        {
          throw new ArgumentException(
            $"Can't reroll die {dieNumber}. Die number has to be between 0 and {DiceAmount - 1} inclusively.");
        }
        dice[dieNumber] = RollSingleDice();
      }
    }

    public int Score(CombinationTypes combination)
    {
      switch (combination)
      {
        case CombinationTypes.Grade1:
          return GradeScore(1);
        case CombinationTypes.Grade2:
          return GradeScore(2);
        case CombinationTypes.Grade3:
          return GradeScore(3);
        case CombinationTypes.Grade4:
          return GradeScore(4);
        case CombinationTypes.Grade5:
          return GradeScore(5);
        case CombinationTypes.Grade6:
          return GradeScore(6);
        case CombinationTypes.Pair:
          return PairScore();
        case CombinationTypes.Set:
          return ThreeScore();
        case CombinationTypes.TwoPairs:
          return TwoPairsScore();
        case CombinationTypes.Care:
          return CareScore();
        case CombinationTypes.Full:
        case CombinationTypes.SmallStreet:
        case CombinationTypes.BigStreet:
        case CombinationTypes.Trash:
        case CombinationTypes.Poker:
          return Sum();
        default:
          throw new ArgumentException(
            $"Only primitive combinations can be scored, but was {combination}");
      }
    }

    public override string ToString()
    {
      //TODO: use DiceAmount?
      return $"{dice[0]} {dice[1]} {dice[2]} {dice[3]} {dice[4]}";
    }

    private int GradeScore(int grade)
    {
      int rollScore = -(grade * DiceAmount);
      for (int i = 0; i < DiceAmount; ++i)
      {
        if (dice[i] == grade)
        {
          rollScore += grade;
        }
      }
      return rollScore;
    }

    private int PairScore()
    {
      int[] valuesCount = new int[D6.MaxValue];

      for (int i = 0; i < DiceAmount; ++i)
      {
        ++valuesCount[dice[i] - 1];
      }

      for (int i = D6.MaxValue; i > 0; --i)
      {
        if (valuesCount[i - 1] >= 2)
        {
          return 2 * i;
        }
      }
      throw new InvalidOperationException($"No pairs in roll {this}");
    }

    private int ThreeScore()
    {
      int[] valuesCount = new int[D6.MaxValue];

      for (int i = 0; i < DiceAmount; ++i)
      {
        ++valuesCount[dice[i] - 1];
      }

      for (int i = D6.MaxValue; i > 0; --i)
      {
        if (valuesCount[i - 1] >= 3)
        {
          return 3 * i;
        }
      }

      throw new InvalidOperationException($"No threes found in roll {this}");
    }

    private int CareScore()
    {
      int[] valuesCount = new int[D6.MaxValue];

      for (int i = 0; i < DiceAmount; ++i)
      {
        ++valuesCount[dice[i] - 1];
      }

      for (int i = D6.MaxValue; i > 0; --i)
      {
        if (valuesCount[i - 1] >= 4)
        {
          return 4 * i;
        }
      }

      throw new InvalidOperationException($"No care found in roll {this}");
    }

    private int TwoPairsScore()
    {
      int[] valuesCount = new int[D6.MaxValue];

      for (int i = 0; i < DiceAmount; ++i)
      {
        ++valuesCount[dice[i] - 1];
      }

      int score = 0;
      for (int i = D6.MaxValue; i > 0; --i)
      {
        // Situation 1: two pairs of single value
        // (basically same as care, but for some reason we want to score it as two pairs)
        if (valuesCount[i - 1] == 4 && score == 0)
        {
          return 4 * i;
        }

        // Situation 2: first pair found
        if (valuesCount[i - 1] >= 2 && score == 0)
        {
          score += 2 * i;
        }
        // Situation 3: second pair found
        else if (valuesCount[i - 1] >= 2 && score > 0)
        {
          return score + 2 * i;
        }
      }

      throw new InvalidOperationException($"No two pairs found in roll {this}");
    }

    public static readonly IReadOnlyList<int[]> Rerolls = new List<int[]>()
    {
      Array.Empty<int>(),
      new [] { 0 }, new [] { 1 }, new [] { 2 }, new [] { 3 }, new [] { 4 },
      new [] { 0, 1 }, new [] { 0, 2 }, new [] { 0, 3 }, new [] { 0, 4 },
      new [] { 1, 2 }, new [] { 1, 3 }, new [] { 1, 4 },
      new [] { 2, 3 }, new [] { 2, 4 },
      new [] { 3, 4 },
      new [] { 0, 1, 2}, new [] { 0, 1, 3}, new [] { 0, 1, 4},
      new [] { 0, 2, 3}, new [] { 0, 2, 4},
      new [] { 0, 3, 4},
      new [] { 1, 2, 3}, new [] { 1, 2, 4},
      new [] { 1, 3, 4},
      new [] { 2, 3, 4},
      new [] { 0, 1, 2, 3 }, new [] { 0, 1, 2, 4 },
      new [] { 0, 1, 3, 4 },
      new [] { 0, 2, 3, 4 },
      new [] { 1, 2, 3, 4 },
      new [] { 0, 1, 2, 3, 4 },
    };
    #endregion
  }
}
