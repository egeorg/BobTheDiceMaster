using Moq;

namespace BobTheDiceMaster.Test
{
  public class TestGameOfSchoolWithDice
  {
    [Fact]
    public void GenerateAndApplyRerollWithDiceIndexesArgument_FailsInIdleState()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });

      var game = new GameOfSchoolWithDice(diceMock.Object);

      Assert.Throws<InvalidOperationException>(() => game.RerollDiceAtIndexes(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceIndexesArgument_YieldsCorrectResult_InRolledState()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });

      var game = new GameOfSchoolWithDice(diceMock.Object);
      game.GenerateRoll();
      game.RerollDiceAtIndexes(new[] { 1, 2, 3 });

      var expectedResult = new DiceRollDistinct(new[] { 6, 1, 5, 2, 6 });
      Assert.Equal(expectedResult, game.CurrentRoll);
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceIndexesArgument_DecrementsRerollsLeft()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });

      var game = new GameOfSchoolWithDice(diceMock.Object);
      game.GenerateRoll();
      int rerollsLeftBeforeReroll = game.RerollsLeft;
      game.RerollDiceAtIndexes(new[] { 1, 2, 3 });

      Assert.Equal(rerollsLeftBeforeReroll - 1, game.RerollsLeft);
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceIndexesArgument_FailsOnThirdReroll()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });
      var game = new GameOfSchoolWithDice(diceMock.Object);
      int rerollsLeftBeforeReroll = game.RerollsLeft;
      game.GenerateRoll();
      game.RerollDiceAtIndexes(new[] { 1, 2, 3 });
      game.RerollDiceAtIndexes(new[] { 1, 2, 3 });

      Assert.Throws<InvalidOperationException>(() => game.RerollDiceAtIndexes(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void GenerateAndApplyReroll_RetainsDiceOrder()
    {
      Mock<IDie> diceMock = TestHelper.GetDiceMock(new[] { 1, 1, 1, 1, 1 }, new[] { 5, 2 });

      var game = new GameOfSchoolWithDice(diceMock.Object);

      game.GenerateRoll();

      game.RerollDiceAtIndexes(new[] { 1, 3 });

      Assert.Equal(game.CurrentRoll, new DiceRollDistinct(new[] { 1, 5, 1, 2, 1 }));
    }

    [Fact]
    public void GenerateRoll_Fails_InRolledState()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 });

      var game = new GameOfSchoolWithDice(diceMock.Object);

      game.GenerateRoll();

      Assert.Throws<InvalidOperationException>(() => game.GenerateRoll());
    }

    [Fact]
    public void GenerateRoll_ChangesStateToRolled()
    {
      Mock<IDie> diceMock = TestHelper.GetDiceMock(new[] { 1, 2, 3, 4, 5 });

      var game = new GameOfSchoolWithDice(diceMock.Object);

      game.GenerateRoll();

      Assert.Equal(GameOfSchoolState.Rolled, game.State);
    }

    [Fact]
    public void GenerateRoll_SetsCurrentRollToGeneratedByDice()
    {
      // Dice order is not ascending to ensure that order is persisted.
      int[] diceValues = new[] { 6, 2, 2, 1, 6 };

      Mock<IDie> diceMock = TestHelper.GetDiceMock(diceValues);

      var game = new GameOfSchoolWithDice(diceMock.Object);

      DiceRollDistinct generatedRoll = game.GenerateRoll();

      var expectedRoll = new DiceRollDistinct(diceValues);

      Assert.Equal(expectedRoll, generatedRoll);

      Assert.Equal(expectedRoll, game.CurrentRoll);
    }

    [Fact]
    public void GenerateRoll_SetsRerollsLeftTo2()
    {
      int[] diceValues = new[] { 6, 2, 2, 1, 6 };

      Mock<IDie> diceMock = TestHelper.GetDiceMock(diceValues);

      var game = new GameOfSchoolWithDice(diceMock.Object);

      game.GenerateRoll();

      Assert.Equal(2, game.RerollsLeft);
    }
  }
}
