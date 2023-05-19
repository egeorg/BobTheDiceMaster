using Moq;

namespace BobTheDiceMaster.Test
{
  public class TestGameOfSchoolWithDice : TestGameOfSchoolWithDiceBase<GameOfSchoolWithDice>
  {
    public TestGameOfSchoolWithDice()
    {
      game = new GameOfSchoolWithDice(diceMock.Object);
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceIndexesArgument_FailsInIdleState()
    {
      TestHelper.ConfigureDiceMock(diceMock, new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });

      Assert.Throws<InvalidOperationException>(() => game.RerollDiceAtIndexes(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceIndexesArgument_YieldsCorrectResult_InRolledState()
    {
      TestHelper.ConfigureDiceMock(diceMock, new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });

      game.GenerateRoll();
      game.RerollDiceAtIndexes(new[] { 1, 2, 3 });

      var expectedResult = new DiceRollDistinct(new[] { 6, 1, 5, 2, 6 });
      Assert.Equal(expectedResult, game.CurrentRoll);
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceIndexesArgument_DecrementsRerollsLeft()
    {
      TestHelper.ConfigureDiceMock(diceMock, new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });

      game.GenerateRoll();
      int rerollsLeftBeforeReroll = game.RerollsLeft;
      game.RerollDiceAtIndexes(new[] { 1, 2, 3 });

      Assert.Equal(rerollsLeftBeforeReroll - 1, game.RerollsLeft);
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceIndexesArgument_FailsOnThirdReroll()
    {
      TestHelper.ConfigureDiceMock(diceMock, new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });
      
      int rerollsLeftBeforeReroll = game.RerollsLeft;
      game.GenerateRoll();
      game.RerollDiceAtIndexes(new[] { 1, 2, 3 });
      game.RerollDiceAtIndexes(new[] { 1, 2, 3 });

      Assert.Throws<InvalidOperationException>(() => game.RerollDiceAtIndexes(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void GenerateAndApplyReroll_RetainsDiceOrder()
    {
      TestHelper.ConfigureDiceMock(diceMock, new[] { 1, 1, 1, 1, 1 }, new[] { 5, 2 });

      game.GenerateRoll();

      game.RerollDiceAtIndexes(new[] { 1, 3 });

      Assert.Equal(game.CurrentRoll, new DiceRollDistinct(new[] { 1, 5, 1, 2, 1 }));
    }

    [Fact]
    public void GenerateRoll_Fails_InRolledState()
    {
      TestHelper.ConfigureDiceMock(diceMock, new[] { 6, 1, 2, 2, 6 });

      game.GenerateRoll();

      Assert.Throws<InvalidOperationException>(() => game.GenerateRoll());
    }

    [Fact]
    public void GenerateRoll_ChangesStateToRolled()
    {
      TestHelper.ConfigureDiceMock(diceMock, new[] { 1, 2, 3, 4, 5 });

      game.GenerateRoll();

      Assert.Equal(GameOfSchoolState.Rolled, game.State);
    }

    [Fact]
    public void GenerateRoll_SetsCurrentRollToGeneratedByDice()
    {
      // Dice order is not ascending to ensure that order is persisted.
      int[] diceValues = new[] { 6, 2, 2, 1, 6 };

      TestHelper.ConfigureDiceMock(diceMock, diceValues);

      DiceRollDistinct generatedRoll = game.GenerateRoll();

      var expectedRoll = new DiceRollDistinct(diceValues);

      Assert.Equal(expectedRoll, generatedRoll);

      Assert.Equal(expectedRoll, game.CurrentRoll);
    }

    [Fact]
    public void GenerateRoll_SetsRerollsLeftTo2()
    {
      int[] diceValues = new[] { 6, 2, 2, 1, 6 };

      TestHelper.ConfigureDiceMock(diceMock, diceValues);

      game.GenerateRoll();

      Assert.Equal(2, game.RerollsLeft);
    }
  }
}
