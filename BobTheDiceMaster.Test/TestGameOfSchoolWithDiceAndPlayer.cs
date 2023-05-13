using Moq;

namespace BobTheDiceMaster.Test
{
  public class TestGameOfSchoolWithDiceAndPlayer
  {
    [Fact]
    public void GenerateAndApplyDecision_FailsInIdleState()
    {
      var defaultPlayerMock = new Mock<IPlayer>();
      var defaultDieMock = TestHelper.GetDiceMock(new int[] { });
      var game = new GameOfSchoolWithDiceAndPlayer(defaultDieMock.Object, defaultPlayerMock.Object);

      Assert.Throws<InvalidOperationException>(() => game.GenerateAndApplyDecision());
    }

    [Fact]
    public void GenerateAndApplyReroll_YieldsCorrectResult_InRolledState()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 6, 5, 6 });
      var playerMock = new Mock<IPlayer>();
      playerMock.Setup(
        player => player.DecideOnRoll(
          It.IsAny<CombinationTypes>(), It.IsAny<DiceRoll>(), It.IsAny<int>()))
        .Returns(new Reroll(new[] { 1, 2, 2 }));

      var game = new GameOfSchoolWithDiceAndPlayer(diceMock.Object, playerMock.Object);
      game.GenerateRoll();
      game.GenerateAndApplyDecision();

      var expectedResult = new DiceRollDistinct(new[] { 6, 6, 5, 6, 6 });
      Assert.Equal(expectedResult, game.CurrentRoll);
    }
  }
}
