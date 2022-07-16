using System;
using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class DiceRoll
  {
    #region private fields
    private static D6 die = new D6();

    private int[] dice;
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

    public int DiceAmount => dice.Length;

    public const int MaxDiceAmount = 5;
    #endregion

    #region public methods
    public DiceRoll(int[] dice)
    {
      this.dice = (int[])dice.Clone();

      if (dice.Length < 1 || dice.Length > MaxDiceAmount)
      {
        throw new ArgumentException(
          $"Between 1 and {MaxDiceAmount} dice expected, but was '{dice.Length}'");
      }

      for (int i = 0; i < dice.Length; ++i)
      {
        if (dice[i] < 1 || dice[i] > D6.MaxValue)
        {
          throw new ArgumentException(
            $"Die value has to be between 1 and {D6.MaxValue}, but {i}-th value was '{dice[i]}'");
        }
      }
    }

    public int Sum()
    {
      return dice.Sum();
    }

    public static DiceRoll GenerateNew()
    {
      int[] dice = new int[MaxDiceAmount];

      for (int i = 0; i < MaxDiceAmount; ++i)
      {
        dice[i] = RollSingleDice();
      }

      return new DiceRoll(dice);
    }

    public void Reroll(IReadOnlyCollection<int> diceToReroll)
    {
      if (diceToReroll.Count > dice.Length)
      {
        throw new ArgumentException($"Can't reroll more than {dice.Length} dice for roll {this}.");
      }

      foreach (var dieNumber in diceToReroll)
      {
        if (dieNumber < 0 || dieNumber >= dice.Length)
        {
          throw new ArgumentException(
            $"Can't reroll die {dieNumber}. Die number has to be between 0 and {dice.Length - 1} inclusively for roll {this}.");
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
      return $"DiceRoll({ String.Join(", ", dice) }";
    }

    private int GradeScore(int grade)
    {
      int rollScore = 0;//  -(grade * MaxDiceAmount);
      for (int i = 0; i < MaxDiceAmount; ++i)
      {
        if (dice[i] == grade)
        {
          rollScore += grade;
        }
      }
      return rollScore;
    }

    public double GetProbability()
    {
      int[] diceHist = new int[D6.MaxValue];
      for (int i = 0; i < dice.Length; ++i)
      {
        ++diceHist[dice[i] - 1];
      }

      long numberOfThrows = Combinatorics.Factorial(dice.Length);

      for (int i = 0; i < D6.MaxValue; ++i)
      {
        numberOfThrows /= Combinatorics.Factorial(diceHist[i]);
      }

      return numberOfThrows / Math.Pow(6, dice.Length);
    }

    private int PairScore()
    {
      int[] valuesCount = new int[D6.MaxValue];

      for (int i = 0; i < MaxDiceAmount; ++i)
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

      for (int i = 0; i < MaxDiceAmount; ++i)
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

      for (int i = 0; i < MaxDiceAmount; ++i)
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

      for (int i = 0; i < MaxDiceAmount; ++i)
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

    static DiceRoll() {
      InitRollResults();
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

    private static List<DiceRoll> roll5Results = new List<DiceRoll>();
    private static List<DiceRoll> roll4Results = new List<DiceRoll>();
    private static List<DiceRoll> roll3Results = new List<DiceRoll>();
    private static List<DiceRoll> roll2Results = new List<DiceRoll>();
    private static List<DiceRoll> roll1Results = new List<DiceRoll>();

    public static IReadOnlyList<DiceRoll> Roll5Results => roll5Results;
    public static IReadOnlyList<DiceRoll> Roll4Results => roll4Results;
    public static IReadOnlyList<DiceRoll> Roll3Results => roll3Results;
    public static IReadOnlyList<DiceRoll> Roll2Results => roll2Results;
    public static IReadOnlyList<DiceRoll> Roll1Results => roll1Results;

    public static IReadOnlyList<DiceRoll>[] RollResults => new [] { roll1Results, roll2Results, roll3Results, roll4Results, roll5Results };

    public static void InitRollResults()
    {
      for (int i1 = 1; i1 <= 6; ++i1)
      {
        roll1Results.Add(new DiceRoll(new[] { i1 }));
        for (int i2 = i1; i2 <= 6; ++i2)
        {
          roll2Results.Add(new DiceRoll(new[] { i1, i2 }));
          for (int i3 = i2; i3 <= 6; ++i3)
          {
            roll3Results.Add(new DiceRoll(new[] { i1, i2, i3 }));
            for (int i4 = i3; i4 <= 6; ++i4)
            {
              roll4Results.Add(new DiceRoll(new[] { i1, i2, i3, i4 }));
              for (int i5 = i4; i5 <= 6; ++i5)
              {
                roll5Results.Add(new DiceRoll(new[] { i1, i2, i3, i4, i5 }));
              }
            }
          }
        }
      }
    }
    #endregion
  }
}
