namespace BobTheDiceMaster.Test
{
  /// <summary>
  /// Unit tests for different Bobs.
  /// Test for specific bob can be implemented by inheriting this class
  /// and calling a <see cref="TestBobBase(IPlayer)"/> in
  /// a parameterless default constructor.
  /// Note: all available combinations has to be either grades only or they
  /// have to contain no more than 3 grades, otherwise bob is not guaranteed
  /// to return any result since it's not a valid situation in game of school.
  /// </summary>
  public abstract class TestBobBase
  {
    IPlayer bob;

    public TestBobBase(IPlayer bob)
    {
      this.bob = bob;
    }

    private const CombinationTypes pokerCareAndFull = CombinationTypes.Poker | CombinationTypes.Full | CombinationTypes.Care;
    private const CombinationTypes grades456 = CombinationTypes.Grade4 | CombinationTypes.Grade5 | CombinationTypes.Grade6;
    private const CombinationTypes grades456PokerCareAndFull = pokerCareAndFull | grades456;
    private const CombinationTypes grades123PokerCareAndFull = pokerCareAndFull | CombinationTypes.Grade1 | CombinationTypes.Grade2 | CombinationTypes.Grade3;
    private const CombinationTypes allButGrades123PokerCareAndFull = CombinationTypes.All & ~grades123PokerCareAndFull;
    private const CombinationTypes allButGrades456PokerCareAndFull = CombinationTypes.All & ~grades456PokerCareAndFull;
    private const CombinationTypes grades136PokerStraightsCareAndFull = pokerCareAndFull | CombinationTypes.Grade3 | CombinationTypes.Grade1 | CombinationTypes.Grade6 | CombinationTypes.LittleStraight | CombinationTypes.BigStraight;
    private const CombinationTypes allButGrades136PokerStraightsCareAndFull = CombinationTypes.All & ~grades136PokerStraightsCareAndFull;
    private const CombinationTypes schoolPokerStraightsCareFullAndPair = grades136PokerStraightsCareAndFull | CombinationTypes.School | CombinationTypes.Pair;
    private const CombinationTypes allButSchoolPokerStraightsCareFullAndPair = CombinationTypes.All & ~schoolPokerStraightsCareFullAndPair;
    private const CombinationTypes allButGrades456 = CombinationTypes.All & ~(grades456);

    [Theory]
    [InlineData(CombinationTypes.Grade1,         new[] { 1, 1, 1, 1, 1 }, allButGrades456PokerCareAndFull)]
    [InlineData(CombinationTypes.Grade2,         new[] { 2, 2, 2, 2, 2 }, allButGrades456PokerCareAndFull)]
    [InlineData(CombinationTypes.Grade3,         new[] { 3, 3, 3, 3, 3 }, allButGrades456PokerCareAndFull)]
    [InlineData(CombinationTypes.Grade4,         new[] { 4, 4, 4, 4, 4 }, allButGrades123PokerCareAndFull)]
    [InlineData(CombinationTypes.Grade5,         new[] { 5, 5, 5, 5, 5 }, allButGrades123PokerCareAndFull)]
    [InlineData(CombinationTypes.Grade6,         new[] { 6, 6, 6, 6, 6 }, allButGrades123PokerCareAndFull)]
    [InlineData(CombinationTypes.Pair,           new[] { 3, 4, 5, 6, 6 }, allButGrades136PokerStraightsCareAndFull)]
    [InlineData(CombinationTypes.TwoPairs,       new[] { 1, 5, 5, 6, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.Set,            new[] { 1, 2, 6, 6, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.Full,           new[] { 5, 5, 6, 6, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.Care,           new[] { 5, 6, 6, 6, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.LittleStraight, new[] { 1, 2, 3, 4, 5 }, allButGrades456)]
    [InlineData(CombinationTypes.BigStraight,    new[] { 2, 3, 4, 5, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.Poker,          new[] { 6, 6, 6, 6, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.Trash,          new[] { 3, 4, 5, 6, 6 }, allButSchoolPokerStraightsCareFullAndPair)]
    public void CorrectCombination_IsScored_WhenNoRerollsLeft(
      CombinationTypes expectedCombination, int[] diceValues, CombinationTypes availableCombinations)
    {
      DiceRoll roll = new DiceRoll(diceValues);
      Decision decision = bob.DecideOnRoll(availableCombinations, roll, 0);

      Assert.Equal(new Score(expectedCombination), decision);
    }

    private const CombinationTypes allButGrades456TwoPairsSetAndCare = CombinationTypes.All & ~(grades456 | CombinationTypes.TwoPairs | CombinationTypes.Set | CombinationTypes.Care);

    [Theory]
    [InlineData(CombinationTypes.Grade1,         new[] { 1, 1, 1, 1, 1 }, CombinationTypes.School)]
    [InlineData(CombinationTypes.Grade2,         new[] { 2, 2, 2, 2, 2 }, CombinationTypes.School)]
    [InlineData(CombinationTypes.Grade3,         new[] { 3, 3, 3, 3, 3 }, CombinationTypes.School)]
    [InlineData(CombinationTypes.Grade4,         new[] { 4, 4, 4, 4, 4 }, CombinationTypes.School)]
    [InlineData(CombinationTypes.Grade5,         new[] { 5, 5, 5, 5, 5 }, CombinationTypes.School)]
    [InlineData(CombinationTypes.Grade6,         new[] { 6, 6, 6, 6, 6 }, CombinationTypes.School)]
    [InlineData(CombinationTypes.Pair,           new[] { 1, 2, 3, 6, 6 }, allButGrades456TwoPairsSetAndCare)]
    [InlineData(CombinationTypes.TwoPairs,       new[] { 1, 5, 5, 6, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.Set,            new[] { 1, 2, 6, 6, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.Full,           new[] { 5, 5, 6, 6, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.Care,           new[] { 5, 6, 6, 6, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.LittleStraight, new[] { 1, 2, 3, 4, 5 }, allButGrades456)]
    [InlineData(CombinationTypes.Poker,          new[] { 6, 6, 6, 6, 6 }, allButGrades456)]
    [InlineData(CombinationTypes.Trash,          new[] { 3, 4, 5, 6, 6 }, CombinationTypes.Trash)]
    public void CorrectCombination_IsScored_WhenRolledOnFirstRoll(
      CombinationTypes expectedCombination, int[] diceValues, CombinationTypes availableCombinations)
    {
      DiceRoll roll = new DiceRoll(diceValues);
      Decision decision = bob.DecideOnRoll(availableCombinations, roll, 2);

      Assert.Equal(new Score(expectedCombination), decision);
    }

    [Fact]
    public void PokerCrossedOut_WhenRollFitsOnlyPairAndCorrespondingGradeAbsent()
    {
      DiceRoll roll = new DiceRoll(new[] { 1, 2, 3, 6, 6 });
      Decision decision = bob.DecideOnRoll(allButGrades456, roll, 0);

      Assert.Equal(new CrossOut(CombinationTypes.Poker), decision);
    }

    [Theory]
    [InlineData(new[] { 2, 2, 6, 6, 6 }, 2)]
    [InlineData(new[] { 2, 2, 6, 6, 6 }, 1)]
    public void DiceRerolledForSchool_IfPossibleAndRerollsLeft(int[] diceValues, int rerollsLeft)
    {
      DiceRoll roll = new DiceRoll(diceValues);
      Decision decision = bob.DecideOnRoll(CombinationTypes.School, roll, rerollsLeft);

      Assert.Equal(new Reroll(new[] { 2, 2 }), decision);
    }

    [Fact]
    public void FirstValueIsRerolled_WhenIts1WithFour6()
    {
      DiceRoll roll = new DiceRoll(new[] { 1, 6, 6, 6, 6 });
      Decision decision = bob.DecideOnRoll(CombinationTypes.School, roll, 2);

      Assert.Equal(new Reroll(new[] { 1 }), decision);
    }

    [Theory]
    [InlineData(new[] { 1, 2, 4, 5, 6 }, 1)]
    [InlineData(new[] { 2, 2, 4, 5, 6 }, 2)]
    [InlineData(new[] { 2, 3, 3, 5, 6 }, 3)]
    [InlineData(new[] { 2, 3, 4, 4, 6 }, 4)]
    [InlineData(new[] { 2, 3, 4, 5, 5 }, 5)]
    public void AnyDiceIndexRerollIsPossible_ForBigStraigt(int[] diceValues, int expectedDieValueToReroll)
    {
      DiceRoll roll = new DiceRoll(diceValues);
      Decision decision = bob.DecideOnRoll(CombinationTypes.BigStraight, roll, 2);

      Assert.Equal(new Reroll(new[] { expectedDieValueToReroll }), decision);
    }

    [Theory]
    [InlineData(new[] { 1, 1, 3, 4, 5 }, 1)]
    [InlineData(new[] { 1, 2, 2, 4, 5 }, 2)]
    [InlineData(new[] { 1, 2, 3, 3, 5 }, 3)]
    [InlineData(new[] { 1, 2, 3, 4, 4 }, 4)]
    [InlineData(new[] { 1, 2, 3, 4, 6 }, 6)]
    public void AnyDiceIndexRerollIsPossible_ForSmallStraight(int[] diceValues, int expectedDieValueToReroll)
    {
      DiceRoll roll = new DiceRoll(diceValues);
      Decision decision = bob.DecideOnRoll(CombinationTypes.LittleStraight, roll, 2);

      Assert.Equal(new Reroll(new[] { expectedDieValueToReroll }), decision);
    }

    [Fact]
    public void AllDiceCanBeRerolled()
    {
      DiceRoll roll = new DiceRoll(new[] { 1, 2, 3, 4, 5 });
      Decision decision = bob.DecideOnRoll(CombinationTypes.Grade6, roll, 2);

      Assert.Equal(new Reroll(new[] { 1, 2, 3, 4, 5 }), decision);
    }
  }
}
