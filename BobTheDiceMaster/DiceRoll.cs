using System;
using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class DiceRoll
  {
    #region private fields
    private int[] dice;

    private static Dictionary<CombinationTypes, double> averageScore =
      new Dictionary<CombinationTypes, double>();
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
    static DiceRoll()
    {
      InitRollResults();
      foreach (var elementaryCombination in CombinationTypesExtension.ElementaryCombinations)
      {
        averageScore.Add(elementaryCombination, AverageScore(elementaryCombination));
      }
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

    public int Sum()
    {
      return dice.Sum();
    }

    public static DiceRoll GenerateNew(IDie die)
    {
      return new DiceRoll(die.Roll(MaxDiceAmount));
    }

    public void Reroll(IReadOnlyCollection<int> diceToReroll, IDie die)
    {
      if (diceToReroll.Count > dice.Length)
      {
        throw new ArgumentException($"Can't reroll more than {dice.Length} dice for roll {this}.");
      }

      int[] dieRollResult = die.Roll(diceToReroll.Count);
      int dieRollResultCounter = 0;

      foreach (var dieNumber in diceToReroll)
      {
        if (dieNumber < 0 || dieNumber >= dice.Length)
        {
          throw new ArgumentException(
            $"Can't reroll die {dieNumber}. Die number has to be between 0 and {dice.Length - 1} inclusively for roll {this}.");
        }
        dice[dieNumber] = dieRollResult[dieRollResultCounter++];
      }
      Array.Sort(dice);
    }

    public DiceRoll ApplyReroll(int[] diceToReroll, DiceRoll rerollResult)
    {
      if (diceToReroll.Length != rerollResult.DiceAmount)
      {
        throw new ArgumentException(
          $"Dice to reroll and dice result has to be of the same length, but was: diceToReroll({diceToReroll.Length}), rerollResult({rerollResult.DiceAmount})");
      }
      int[] newRollDice = new int[MaxDiceAmount];
      int rerollCounter = 0;
      for (int i = 0; i < MaxDiceAmount; ++i)
      {
        // Indices in DiceRoll.Rerolls are in ascending order
        if (rerollCounter < diceToReroll.Length && diceToReroll[rerollCounter] == i)
        {
          newRollDice[i] = rerollResult[rerollCounter++];
        }
        else
        {
          newRollDice[i] = dice[i];
        }
      }
      return new DiceRoll(newRollDice);
    }

    public static double AverageScore(CombinationTypes combination)
    {
      if (!combination.IsElementary())
      {
        throw new ArgumentException(
          $"Elementary combination expected, but was {combination}");
      }
      if (averageScore.ContainsKey(combination))
      {
        return averageScore[combination];
      }

      double averageProfit = 0;

      Dictionary<DiceRoll, double> secondRollScoreCache = new Dictionary<DiceRoll, double>();

      foreach (DiceRoll firstRoll in DiceRoll.Roll5Results)
      {
        double firstRollScore = firstRoll.Score(combination) ?? 0;

        if (!combination.IsFromSchool())
        {
          firstRollScore *= 2;
        }

        int[] bestFirstReroll = null;

        foreach (int[] firstReroll in DiceRoll.Rerolls)
        {
          if (firstReroll.Length == 0)
          {
            continue;
          }
          IReadOnlyList<DiceRoll> firstRerollResults = DiceRoll.RollResults[firstReroll.Length - 1];
          double firstRerollAverage = 0;

          foreach (var firstRerollResult in firstRerollResults)
          {
            int[] secondRollDice = new int[DiceRoll.MaxDiceAmount];
            int firstRerollCounter = 0;

            for (int i = 0; i < DiceRoll.MaxDiceAmount; ++i)
            {
              // Indices in DiceRoll.Rerolls are in ascending order
              if (firstRerollCounter < firstReroll.Length && firstReroll[firstRerollCounter] == i)
              {
                secondRollDice[i] = firstRerollResult[firstRerollCounter++];
              }
              else
              {
                secondRollDice[i] = firstRoll[i];
              }
            }
            DiceRoll secondRoll = new DiceRoll(secondRollDice);

            // The best score that can be achieved on the secondRoll result, including optimal reroll.
            // i.e. if first reroll yields firstRerollResult, it's the best.
            double secondRollScore;

            if (secondRollScoreCache.ContainsKey(secondRoll))
            {
              secondRollScore = secondRollScoreCache[secondRoll];
            }
            else
            {
              secondRollScore = secondRoll.Score(combination) ?? 0;

              // null indicates that current score is better than any reroll
              int[] bestSecondReroll = null;

              foreach (int[] secondReroll in DiceRoll.Rerolls)
              {
                if (secondReroll.Length == 0)
                {
                  continue;
                }
                IReadOnlyList<DiceRoll> secondRerollResults = DiceRoll.RollResults[secondReroll.Length - 1];
                double secondRerollAverage = 0;

                foreach (var secondRerollResult in secondRerollResults)
                {
                  int[] thirdRollDice = new int[DiceRoll.MaxDiceAmount];
                  int secondRerollCounter = 0;
                  for (int i = 0; i < DiceRoll.MaxDiceAmount; ++i)
                  {
                    // Indices in DiceRoll.Rerolls are in ascending order
                    if (secondRerollCounter < secondReroll.Length && secondReroll[secondRerollCounter] == i)
                    {
                      thirdRollDice[i] = secondRerollResult[secondRerollCounter++];
                    }
                    else
                    {
                      thirdRollDice[i] = secondRoll[i];
                    }
                  }
                  DiceRoll thirdRoll = new DiceRoll(thirdRollDice);
                  secondRerollAverage += secondRerollResult.GetProbability() * (thirdRoll.Score(combination) ?? 0);
                }

                if (secondRerollAverage > secondRollScore)
                {
                  secondRollScore = secondRerollAverage;
                  bestSecondReroll = secondReroll;
                }
              }

              secondRollScoreCache.Add(secondRoll, secondRollScore);
            }

            firstRerollAverage += firstRerollResult.GetProbability() * secondRollScore;
          }

          if (firstRerollAverage > firstRollScore)
          {
            firstRollScore = firstRerollAverage;
            bestFirstReroll = firstReroll;
          }
        }

        averageProfit += firstRoll.GetProbability() * firstRollScore;
      }

      return averageProfit;
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
        case CombinationTypes.SmallStreet:
          return SmallStreetScore();
        case CombinationTypes.BigStreet:
          return BigStreetScore();
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
      return $"DiceRoll({ String.Join(", ", dice) })";
    }

    public override int GetHashCode()
    {
      int[] diceHist = new int[D6.MaxValue];
      for (int i = 0; i < dice.Length; ++i)
      {
        ++diceHist[dice[i] - 1];
      }
      int hash = 0;
      int basePower = 1;
      for (int i = 0; i < D6.MaxValue; ++i)
      {
        hash += basePower * diceHist[i];
        basePower *= MaxDiceAmount;
      }
      return hash;
    }

    public override bool Equals(object obj)
    {
      DiceRoll other = (DiceRoll)obj;
      int[] diceHist = new int[D6.MaxValue];
      int[] otherDiceHist = new int[D6.MaxValue];
      for (int i = 0; i < dice.Length; ++i)
      {
        ++diceHist[dice[i] - 1];
        ++otherDiceHist[other[i] - 1];
      }
      for (int i = 0; i < D6.MaxValue; ++i)
      {
        if (diceHist[i] != otherDiceHist[i])
        {
          return false;
        }
      }
      return true;
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

    private int? SmallStreetScore()
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
        return 20;
      }
      return null;
    }

    private int? BigStreetScore()
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
        return 30;
      }
      return null;
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
