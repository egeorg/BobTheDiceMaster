using System;
using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  /// <summary>
  /// TODO: explain why do I need a parameter
  /// </summary>
  public class DiceRoll : IDiceRoll<DiceRoll>
  {
    public int this[int i]
    {
      get
      {
        return dice[i];
      }
    }

    public int DiceAmount => dice.Length;

    public const int MaxDiceAmount = 5;

    static DiceRoll()
    {
      // Note: order matters. InitRollResults() has to be called first because
      // InitProbabilities() uses results of InitRollResults()
      InitRollResults();
      InitProbabilities();

      averageScore = new Dictionary<CombinationTypes, double>
      {
        {CombinationTypes.Grade1, 2.1064814814814756},
        {CombinationTypes.Grade2, 4.2129629629629575},
        {CombinationTypes.Grade3, 6.3194444444444455},
        {CombinationTypes.Grade4, 8.425925925925913},
        {CombinationTypes.Grade5, 10.532407407407392},
        {CombinationTypes.Grade6, 12.6388888888889},
        {CombinationTypes.Pair, 15.865270870457234},
        {CombinationTypes.Set, 11.619899714332302},
        {CombinationTypes.TwoPairs, 16.37926613378522},
        {CombinationTypes.Full, 7.702210020138647},
        {CombinationTypes.Care, 4.853220771449692},
        {CombinationTypes.LittleStraight, 3.183917676890467},
        {CombinationTypes.BigStraight, 4.245223569187295},
        {CombinationTypes.Poker, 0.8804221388169133},
        {CombinationTypes.Trash, 35.100630144032884}
      };
    }

    public DiceRoll(int[] dice)
    {
      this.dice = (int[])dice.Clone();
      Array.Sort(this.dice);

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

    private DiceRoll(DiceRoll roll, int[] diceToReroll, IDiceRoll<DiceRoll> rerollResult)
    {
      dice = new int[roll.DiceAmount];
      int rollCounter = 0;
      int rerollResultCounter = 0;
      int rerollCounter = 0;
      int resultCounter = 0;

      // merge two sorted arrays, ignore diceIndexesToReroll from roll.
      while (resultCounter < roll.DiceAmount)
      {
        while (rerollCounter < diceToReroll.Length && rollCounter == diceToReroll[rerollCounter])
        {
          rollCounter++;
          rerollCounter++;
        }
        if (rollCounter < roll.DiceAmount && (rerollResultCounter >= rerollResult.DiceAmount || roll[rollCounter] < rerollResult[rerollResultCounter]))
        {
          dice[resultCounter] = roll[rollCounter];
          resultCounter++;
          rollCounter++;
        }
        else
        {
          dice[resultCounter] = rerollResult[rerollResultCounter];
          resultCounter++;
          rerollResultCounter++;
        }
      }
    }

    public int Sum()
    {
      return dice.Sum();
    }

    public static DiceRoll GenerateNew(IDie die)
    {
      return new DiceRoll(die.Roll(MaxDiceAmount));
    }

    public DiceRoll Reroll(IReadOnlyCollection<int> diceToReroll, IDie die)
    {
      int[] diceNew = (int[])dice.Clone();

      if (diceToReroll.Count > diceNew.Length)
      {
        throw new ArgumentException($"Can't reroll more than {diceNew.Length} dice for roll {this}.");
      }

      int[] dieRollResult = die.Roll(diceToReroll.Count);
      int dieRollResultCounter = 0;

      foreach (var dieNumber in diceToReroll)
      {
        if (dieNumber < 0 || dieNumber >= diceNew.Length)
        {
          throw new ArgumentException(
            $"Can't reroll die {dieNumber}. Die number has to be between 0 and {diceNew.Length - 1} inclusively for roll {this}.");
        }
        diceNew[dieNumber] = dieRollResult[dieRollResultCounter++];
      }

      return new DiceRoll(diceNew);
    }

    /// <summary>
    /// TODO: is it used?
    /// </summary>
    public DiceRoll RerollByValue(int[] valuesToReroll, IDie die)
    {
      //TODO[GE]: copy?
      Array.Sort(valuesToReroll);

      int[] diceToReroll = new int[valuesToReroll.Length];

      int rollCounter = 0;
      int rerollCounter = 0;

      while (rerollCounter < valuesToReroll.Length
        && rollCounter < MaxDiceAmount)
      {
        if (valuesToReroll[rerollCounter] == dice[rollCounter])
        {
          diceToReroll[rerollCounter] = rollCounter;
          ++rerollCounter;
        }
        ++rollCounter;
      }

      if (rerollCounter < valuesToReroll.Length)
      {
        throw new ArgumentException(
          $"Can't reroll values {{{String.Join(", ", valuesToReroll)}}} for roll {this}, some of the values were not found");
      }

      return Reroll(diceToReroll, die);
    }

    public DiceRoll ApplyReroll(int[] diceToReroll, IDiceRoll<DiceRoll> rerollResult)
    {
      if (diceToReroll.Length != rerollResult.DiceAmount)
      {
        throw new ArgumentException(
          $"Dice to reroll and dice result has to be of the same length, but was: diceIndexesToReroll({diceToReroll.Length}), rerollResult({rerollResult.DiceAmount})");
      }
      //TODO: check if using DiceRoll(3 args) actually makes bob faster
      //int[] newRollDice = new int[MaxDiceAmount];
      //int rerollCounter = 0;
      //for (int i = 0; i < MaxDiceAmount; ++i)
      //{
      //  // Indices in DiceRoll.NonEmptyRerolls elements are in ascending order
      //  if (rerollCounter < diceIndexesToReroll.Length && diceIndexesToReroll[rerollCounter] == i)
      //  {
      //    newRollDice[i] = rerollResult[rerollCounter++];
      //  }
      //  else
      //  {
      //    newRollDice[i] = dice[i];
      //  }
      //}
      //return new DiceRoll(newRollDice);
      return new DiceRoll(this, diceToReroll, rerollResult);
    }

    /// <summary>
    /// TODO: is it used?
    /// </summary>
    public DiceRoll ApplyRerollByValue(int[] valuesToReroll, DiceRoll rerollResult)
    {
      if (valuesToReroll.Length != rerollResult.DiceAmount)
      {
        throw new ArgumentException(
          $"Dice to reroll and dice result has to be of the same length, but was: diceIndexesToReroll({valuesToReroll.Length}), rerollResult({rerollResult.DiceAmount})");
      }

      //TODO[GE]: copy?
      Array.Sort(valuesToReroll);

      int[] diceToReroll = new int[valuesToReroll.Length];

      int rollCounter = 0;
      int rerollCounter = 0;

      int[] newRollDice = (int[])dice.Clone();

      while (rerollCounter < valuesToReroll.Length
        && rollCounter < MaxDiceAmount)
      {
        if (valuesToReroll[rerollCounter] == dice[rollCounter])
        {
          newRollDice[rollCounter] = rerollResult[rerollCounter];
          ++rerollCounter;
        }
        ++rollCounter;
      }

      if (rerollCounter < valuesToReroll.Length)
      {
        throw new ArgumentException(
          $"Can't reroll values {{{String.Join(", ", valuesToReroll)}}} for roll {this}, some of the values were not found");
      }

      //rerollCounter = 0;

      //for (int i = 0; i < MaxDiceAmount; ++i)
      //{
      //  // Indices in DiceRoll.NonEmptyRerolls elements are in ascending order
      //  if (rerollCounter < diceIndexesToReroll.Length && diceIndexesToReroll[rerollCounter] == i)
      //  {
      //    newRollDice[i] = rerollResult[rerollCounter++];
      //  }
      //  else
      //  {
      //    newRollDice[i] = dice[i];
      //  }
      //}
      return new DiceRoll(newRollDice);
    }

    public static double AverageScore(CombinationTypes combination)
    {
      if (!combination.IsElementary())
      {
        throw new ArgumentException(
          $"Elementary combination expected, but was {combination}");
      }

      if (!averageScore.ContainsKey(combination))
      {
        throw new ArgumentException(
          $"Unexpected elementary combination: {combination}");
      }

      return averageScore[combination];
    }

    public int? Score(CombinationTypes combination)
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
          return SetScore();
        case CombinationTypes.TwoPairs:
          return TwoPairsScore();
        case CombinationTypes.Care:
          return CareScore();
        case CombinationTypes.Full:
          return FullScore();
        case CombinationTypes.LittleStraight:
          return LittleStraightScore();
        case CombinationTypes.BigStraight:
          return BigStraightScore();
        case CombinationTypes.Poker:
          return PokerScore();
        case CombinationTypes.Trash:
          return Sum();
        default:
          throw new ArgumentException(
            $"Only primitive combinations can be scored, but was {combination}");
      }
    }

    public override string ToString()
    {
      return $"{nameof(DiceRoll)}({String.Join(", ", dice)})";
    }

    public override int GetHashCode()
    {
      // hash calculated in this way is perfect.
      int hash = 0;
      for (int i = 0; i < DiceAmount; ++i)
      {
        hash *= 6;
        hash += dice[i];
      }
      return hash;
    }

    public override bool Equals(object obj)
    {
      //GetHashCode is a perfect cache, so it can be used for equality check as well.
      return obj is DiceRoll && GetHashCode() == obj.GetHashCode();
    }

    public static readonly IReadOnlyList<int[]> NonEmptyRerolls = new List<int[]>()
    {
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

    static Dictionary<DiceRoll, double> probabilitiesCache = new Dictionary<DiceRoll, double>();

    private static void InitProbabilities()
    {
      //TODO: is it all needed?
      foreach (DiceRoll roll in roll5Results)
      {
        probabilitiesCache.Add(roll, roll.CalculateProbability());
      }
      foreach (DiceRoll roll in roll4Results)
      {
        probabilitiesCache.Add(roll, roll.CalculateProbability());
      }
      foreach (DiceRoll roll in roll3Results)
      {
        probabilitiesCache.Add(roll, roll.CalculateProbability());
      }
      foreach (DiceRoll roll in roll2Results)
      {
        probabilitiesCache.Add(roll, roll.CalculateProbability());
      }
      foreach (DiceRoll roll in roll1Results)
      {
        probabilitiesCache.Add(roll, roll.CalculateProbability());
      }
    }

    /// <summary>
    /// Probability is calculated as number of distinct dice rolls that yeild
    /// the numbers specified devided by total number of distinct dice rolls.
    /// Below are examples for 5 dice rolls, similarly for other number of dice:
    /// Total number of possible roll results is 6^5, 6 possible results for each dice.
    /// Number of distinct dice rolls that yield the specific numbers is calculated as follows:
    /// If all dice values are different, it's 5! (one of 5 dice yeilds the
    /// first number, one of 4 dice that left yields the second number, etc.)
    /// It some dice values are repeated, then for each such dice the 5! should be devided
    /// by x! where x is how many times the value occurs in the dice roll result, for example:
    /// For roll (1, 2, 3, 4, 5), probability is 5! / 6^5.
    /// In roll (6,6,6,6,6), 6 appears 5 times =>
    /// 5! / 5! = 1 possible combination, and probability is 1 / 6^5
    /// In roll (1,2,6,6,6), 6 appears 3 times and the rest numbers only once =>
    /// 5! / 3! = 20 combinations, and probability is 20 / 6^5
    /// In roll (5,5,6,6,6), 6 appears 4 times and 5 appears 2 times =>
    /// 5! / (3! * 2!) = 10 combinations, and probability is 10 / 6^5
    /// </summary>
    public double GetProbability()
    {
      // the probabilitiesCache is filled in an InitProbabilities()
      // method that to be called from the static constructor.
      return probabilitiesCache[this];
    }

    private double CalculateProbability()
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

    private readonly int[] dice;

    private static Dictionary<CombinationTypes, double> averageScore =
      new Dictionary<CombinationTypes, double>();

    private int? PairScore()
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

      return null;
    }

    private int? SetScore()
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

      return null;
    }

    private int? CareScore()
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

      return null;
    }

    private int? TwoPairsScore()
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

      return null;
    }

    private int? FullScore()
    {
      int[] valuesCount = new int[D6.MaxValue];

      for (int i = 0; i < MaxDiceAmount; ++i)
      {
        ++valuesCount[dice[i] - 1];
      }

      bool threeSameFound = false;
      bool twoSameFound = false;

      int score = 0;

      for (int i = D6.MaxValue; i > 0; --i)
      {
        // Situation 1: 2 + 3 of single value
        // (basically same as poker, but for some reason we want to score it as full)
        if (valuesCount[i - 1] == 5)
        {
          return 5 * i;
        }

        if (valuesCount[i - 1] == 3)
        {
          score += 3 * i;
          threeSameFound = true;
          continue;
        }

        if (valuesCount[i - 1] == 2)
        {
          score += 2 * i;
          twoSameFound = true;
        }
      }

      if (threeSameFound && twoSameFound)
      {
        return score;
      }

      return null;
    }

    private int? PokerScore()
    {
      int[] valuesCount = new int[D6.MaxValue];

      for (int i = 0; i < MaxDiceAmount; ++i)
      {
        ++valuesCount[dice[i] - 1];
      }

      for (int i = D6.MaxValue; i > 0; --i)
      {
        if (valuesCount[i - 1] == 5)
        {
          return 5 * i;
        }
      }

      return null;
    }

    private int? LittleStraightScore()
    {
      int[] valuesCount = new int[D6.MaxValue];

      for (int i = 0; i < MaxDiceAmount; ++i)
      {
        ++valuesCount[dice[i] - 1];
      }

      if (valuesCount[0] == 1
        && valuesCount[1] == 1
        && valuesCount[2] == 1
        && valuesCount[3] == 1
        && valuesCount[4] == 1)
      {
        return 15;
      }
      return null;
    }

    private int? BigStraightScore()
    {
      int[] valuesCount = new int[D6.MaxValue];

      for (int i = 0; i < MaxDiceAmount; ++i)
      {
        ++valuesCount[dice[i] - 1];
      }

      if (valuesCount[1] == 1
        && valuesCount[2] == 1
        && valuesCount[3] == 1
        && valuesCount[4] == 1
        && valuesCount[5] == 1)
      {
        return 20;
      }
      return null;
    }

    private static List<DiceRoll> roll5Results = new List<DiceRoll>();
    private static List<DiceRoll> roll4Results = new List<DiceRoll>();
    private static List<DiceRoll> roll3Results = new List<DiceRoll>();
    private static List<DiceRoll> roll2Results = new List<DiceRoll>();
    private static List<DiceRoll> roll1Results = new List<DiceRoll>();

    //TODO[GE]: remove? Or change to results depending on number of dice?
    public static IReadOnlyList<DiceRoll> Roll5Results => roll5Results;

    public static IReadOnlyList<DiceRoll>[] RollResults => new[] { roll1Results, roll2Results, roll3Results, roll4Results, roll5Results };

    private static void InitRollResults()
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
  }
}
