using Moq;

namespace BobTheDiceMaster.Test
{
  public class TestDiceRoll
  {
    [Fact]
    public void Sum_ReturnsCorrectResult()
    {
      DiceRoll roll = new DiceRoll(new [] { 1, 6, 2, 6, 3 });

      Assert.Equal(18, roll.Sum());
    }

    [Fact]
    public void GenerateNew_ReturnsCorrectResult()
    {
      var dieMock = TestHelper.GetDiceMock(new[] { 2, 4, 4, 5, 6 });

      DiceRoll generatedRoll = DiceRoll.GenerateNew(dieMock.Object);

      DiceRoll expectedRoll = new DiceRoll(new[] { 2, 4, 4, 5, 6 });

      Assert.Equal(expectedRoll, generatedRoll);
    }

    [Fact]
    public void Reroll_YieldsCorrectResult()
    {
      var dieMock = TestHelper.GetDiceMock(new[] { 5, 6 });

      DiceRoll initialRoll = new DiceRoll(new[] { 1, 3, 6, 6, 6 });

      int[] diceIndexesToReroll = new[] { 0, 1 };

      DiceRoll newRoll = initialRoll.RerollIndexes(diceIndexesToReroll, dieMock.Object);

      DiceRoll expectedRoll = new DiceRoll(new[] { 5, 6, 6, 6, 6 });

      Assert.Equal(expectedRoll, newRoll);
    }

    [Fact]
    public void RerollByValue_YieldsCorrectResult()
    {
      var dieMock = TestHelper.GetDiceMock(new[] { 5, 6 });

      DiceRoll initialRoll = new DiceRoll(new[] { 1, 3, 6, 6, 6 });

      int[] diceValuesToReroll = new[] { 1, 3 };

      DiceRoll newRoll = initialRoll.RerollValues(diceValuesToReroll, dieMock.Object);

      DiceRoll expectedRoll = new DiceRoll(new[] { 5, 6, 6, 6, 6 });

      Assert.Equal(expectedRoll, newRoll);
    }

    [Fact]
    public void ApplyReroll_ReturnsCorrectResult()
    {
      DiceRoll initialRoll = new DiceRoll(new[] { 1, 1, 1, 1, 1 });

      int[] diceIndexesToReroll = new[] { 0, 1 };

      DiceRoll rerollResult = new DiceRoll(new[] { 6, 6 });

      DiceRoll resultRoll = initialRoll.ApplyRerollAtIndexes(diceIndexesToReroll, rerollResult);

      DiceRoll expectedRoll = new DiceRoll(new[] { 1, 1, 1, 6, 6 });

      Assert.Equal(expectedRoll, resultRoll);
    }

    [Fact]
    public void ApplyRerollByValue_ReturnsCorrectResult()
    {
      DiceRoll initialRoll = new DiceRoll(new[] { 2, 3, 3, 4, 5 });

      int[] diceValuesToReroll = new[] { 3, 3 };

      DiceRoll rerollResult = new DiceRoll(new[] { 6, 6 });

      DiceRoll resultRoll = initialRoll.ApplyRerollForValues(diceValuesToReroll, rerollResult);

      DiceRoll expectedRoll = new DiceRoll(new[] { 2, 4, 5, 6, 6 });

      Assert.Equal(expectedRoll, resultRoll);
    }

    [Theory]
    [InlineData(new[] { 1, 2, 6, 6, 6 }, -12, CombinationTypes.Grade6)]
    [InlineData(new[] { 2, 2, 2, 4, 4 }, 8,   CombinationTypes.Pair)]
    [InlineData(new[] { 2, 2, 2, 4, 4 }, 6,   CombinationTypes.Set)]
    [InlineData(new[] { 4, 6, 6, 6, 6 }, 24,  CombinationTypes.Care)]
    [InlineData(new[] { 1, 1, 1, 1, 1 }, 5,   CombinationTypes.Poker)]
    [InlineData(new[] { 2, 2, 2, 5, 5 }, 14,  CombinationTypes.TwoPairs)]
    [InlineData(new[] { 2, 2, 2, 2, 5 }, 8,   CombinationTypes.TwoPairs)]
    [InlineData(new[] { 2, 2, 2, 5, 5 }, 16, CombinationTypes.Full)]
    [InlineData(new[] { 2, 2, 5, 5, 5 }, 19, CombinationTypes.Full)]
    [InlineData(new[] { 5, 5, 5, 5, 5 }, 25,  CombinationTypes.Full)]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 15,  CombinationTypes.LittleStraight)]
    [InlineData(new[] { 2, 3, 4, 5, 6 }, 20,  CombinationTypes.BigStraight)]
    [InlineData(new[] { 3, 4, 5, 5, 6 }, 23,  CombinationTypes.Trash)]
    public void Score_ReturnsCorrectResult_ForSpecificCombinaiton(
      int[] testedRollDice, int expectedScore, CombinationTypes combination)
    {
      DiceRoll testedRoll = new DiceRoll(testedRollDice);

      Assert.Equal(expectedScore, testedRoll.Score(combination));
    }

    [Theory]
    [InlineData(new[] { 1, 3, 4, 5, 6 }, CombinationTypes.Pair)]
    [InlineData(new[] { 2, 2, 3, 4, 4 }, CombinationTypes.Set)]
    [InlineData(new[] { 4, 4, 4, 6, 6 }, CombinationTypes.Care)]
    [InlineData(new[] { 2, 1, 1, 1, 1 }, CombinationTypes.Poker)]
    [InlineData(new[] { 1, 2, 3, 5, 5 }, CombinationTypes.TwoPairs)]
    [InlineData(new[] { 2, 2, 3, 5, 5 }, CombinationTypes.Full)]
    [InlineData(new[] { 2, 2, 3, 4, 5 }, CombinationTypes.LittleStraight)]
    [InlineData(new[] { 2, 3, 4, 5, 5 }, CombinationTypes.BigStraight)]
    public void Score_IsNotCalculated_ForInaproppriateCombination(
      int[] testedRollDice, CombinationTypes combination)
    {
      DiceRoll testedRoll = new DiceRoll(testedRollDice);

      Assert.Null(testedRoll.Score(combination));
    }

    private static IEnumerable<IEnumerable<int>> GetAllRerollResults(int diceAmount)
    {
      IEnumerable<IEnumerable<int>> allRolls = new[] { Enumerable.Empty<int>() };
      for (int i = 0; i < diceAmount; ++i)
      {
        allRolls = allRolls.Select(allRollsWithOneDiceLess =>
          Enumerable.Range(1, 6)
            .Select(dieValue => allRollsWithOneDiceLess.Append(dieValue)))
            .SelectMany(rolls => rolls);
      }
      return allRolls;
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void RollResults_ContainAllDiceRolls(int rollResultsIndex)
    {
      int diceAmount = rollResultsIndex + 1;

      IEnumerable<IEnumerable<int>> allRolls = GetAllRerollResults(diceAmount);

      var rollResultsSet = new HashSet<DiceRoll>(DiceRoll.RollResultsByDiceAmount[rollResultsIndex]);

      foreach (IEnumerable<int> roll in allRolls)
      {
        Assert.Contains(new DiceRoll(roll.ToArray()), rollResultsSet);
      }
    }

    [Fact]
    public void NonEmptyRerolls_ContainsAllNonEmptyRerolls()
    {

      int[][] nonEmptyDiceSubsets = GetAllNonEmptyDiceSubsetsOrderedAscending().ToArray();

      Assert.Equal(nonEmptyDiceSubsets.Length, DiceRoll.NonEmptyRerolls.Count);

      var nonEmptyRerollsSet = new HashSet<int[]>(DiceRoll.NonEmptyRerolls);

      foreach (var diceSubset in DiceRoll.NonEmptyRerolls)
      {
        Assert.Contains(diceSubset, nonEmptyRerollsSet);
      }
    }

    private IEnumerable<int[]> GetAllNonEmptyDiceSubsetsOrderedAscending()
    {
      IEnumerable<IEnumerable<int>> allDiceSubsets = new IEnumerable<int>[1 << DiceRoll.MaxDiceAmount];
      for (int subsetIndex = 1; subsetIndex < 1 << DiceRoll.MaxDiceAmount; ++subsetIndex)
      {
        yield return Enumerable.Range(0, DiceRoll.MaxDiceAmount)
          .Where(bitIndex => (subsetIndex & (1 << bitIndex)) != 0)
          .ToArray();
      }
    }

    [Theory]
    // (a, b, c, d, e)
    // 5! / 6^5
    [InlineData(new[] { 1, 2, 3, 4, 5 }, (5 * 4 * 3 * 2) / (double)(6 * 6 * 6 * 6 * 6))]
    // (a, a, a, a, a)
    // 1 / 6^5
    [InlineData(new[] { 6, 6, 6, 6, 6 }, 1 / (double)(6 * 6 * 6 * 6 * 6))]
    // (a, b, c, d, d)
    // (5! / 2!) / 6^5
    [InlineData(new[] { 1, 2, 6, 6, 6 }, ((5 * 4 * 3 * 2) / (3 * 2)) / (double)(6 * 6 * 6 * 6 * 6))]
    // (a, a, a, b, b)
    // (5! / (3! * 2!)) / 6^5
    [InlineData(new[] { 5, 5, 5, 6, 6 }, ((5 * 4 * 3 * 2) / ((3 * 2) * 2)) / (double)(6 * 6 * 6 * 6 * 6))]
    // (a, b, b, c, c)
    // (5! / (2! * 2!)) / 6^5
    [InlineData(new[] { 1, 5, 5, 6, 6 }, ((5 * 4 * 3 * 2) / (2 * 2)) / (double)(6 * 6 * 6 * 6 * 6))]
    public void GetProbability_ReturnsCorrectResult(int[] diceValues, double probability)
    {
      var roll = new DiceRoll(diceValues);

      Assert.Equal(probability, roll.GetProbability(), TestHelper.Tolerance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void GetProbability_SumIs1_AcrossAllRollResults(int rollResultsIndex)
    {
      double pSum = 0;
      foreach (var reroll in DiceRoll.RollResultsByDiceAmount[rollResultsIndex])
      {
        pSum += reroll.GetProbability();
      }
      Assert.Equal(1, pSum, TestHelper.Tolerance);
    }

    [Fact]
    public void Equals_IsTrue_ForSameDiceRollsWithDifferentDiceOrder()
    {
      DiceRoll roll1 = new DiceRoll(new[] { 1, 1, 6, 6, 6 });
      DiceRoll roll2 = new DiceRoll(new[] { 6, 1, 6, 1, 6 });
      Assert.Equal(roll1, roll2);
    }

    [Fact]
    public void Equals_IsFalse_ForDifferentDiceRolls()
    {
      DiceRoll roll1 = new DiceRoll(new[] { 1, 1, 6, 6, 6 });
      DiceRoll roll2 = new DiceRoll(new[] { 1, 1, 1, 6, 6 });
      Assert.NotEqual(roll1, roll2);
    }

    [Fact]
    public void GetHashCode_IsPerfectHash()
    {
      HashSet<int> hashCodes = new HashSet<int>();
      foreach (IEnumerable<DiceRoll> rolls in DiceRoll.RollResultsByDiceAmount)
      {
        foreach (DiceRoll roll in rolls)
        {
          int hashCode = roll.GetHashCode();
          Assert.DoesNotContain(hashCode, hashCodes);
          hashCodes.Add(hashCode);
        }
      }
    }
  }
}
