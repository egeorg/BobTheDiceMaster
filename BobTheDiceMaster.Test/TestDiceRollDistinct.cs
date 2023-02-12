using Moq;

namespace BobTheDiceMaster.Test
{
  public class TestDiceRollDistinct
  {
    [Fact]
    public void Reroll_IsAppliedCorrectlyAndWithoutDiceReorder()
    {
      Mock<IDie> dieMock = TestHelper.GetDiceMock(new[] { 3, 1 });

      DiceRollDistinct initialRoll = new DiceRollDistinct(new[] { 2, 2, 4, 4, 6 });

      DiceRollDistinct newRoll = initialRoll.Reroll(new[] { 0, 2 }, dieMock.Object);

      Assert.Equal(new DiceRollDistinct(new[] { 3, 2, 1, 4, 6 }), newRoll);
    }

    [Fact]
    public void ApplyReroll_IsAppliedCorrectlyAndWithoutDiceReorder()
    {
      DiceRollDistinct roll = new DiceRollDistinct(new[] { 2, 2, 4, 4, 6 });

      DiceRollDistinct rerollResult = new DiceRollDistinct(new[] { 3, 1 });

      DiceRollDistinct newRoll = roll.ApplyReroll(new[] { 0, 2 }, rerollResult);

      Assert.Equal(new DiceRollDistinct(new[] { 3, 2, 1, 4, 6 }), newRoll);
    }

    [Fact]
    public void Equals_ReturnsTrue_WhenDiceValuesAndOrderIsSimilar()
    {
      DiceRollDistinct roll1 = new DiceRollDistinct(new[] { 1, 5, 2, 2, 6 });
      DiceRollDistinct roll2 = new DiceRollDistinct(new[] { 1, 5, 2, 2, 6 });

      Assert.Equal(roll1, roll2);
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenDiceValuesSimilarButOrderIsDifferent()
    {
      DiceRollDistinct roll1 = new DiceRollDistinct(new[] { 1, 2, 2, 5, 6 });
      DiceRollDistinct roll2 = new DiceRollDistinct(new[] { 1, 2, 2, 6, 5 });

      Assert.NotEqual(roll1, roll2);
    }
  }
}
