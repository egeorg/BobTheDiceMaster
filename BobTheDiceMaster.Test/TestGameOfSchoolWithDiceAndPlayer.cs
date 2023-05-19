using Moq;

namespace BobTheDiceMaster.Test
{
  public class TestGameOfSchoolWithDiceAndPlayer : TestGameOfSchoolWithDiceBase<GameOfSchoolWithDiceAndPlayer>
  {
    private Mock<IPlayer> playerMock = new Mock<IPlayer>();

    public TestGameOfSchoolWithDiceAndPlayer()
    {
      game = new GameOfSchoolWithDiceAndPlayer(diceMock.Object, playerMock.Object);
    }

    [Fact]
    public void GenerateAndApplyDecision_FailsInIdleState()
    {
      var game = new GameOfSchoolWithDiceAndPlayer(diceMock.Object, playerMock.Object);

      Assert.Throws<InvalidOperationException>(() => game.GenerateAndApplyDecision());
    }

    [Fact]
    public void GenerateAndApplyReroll_YieldsCorrectResult_InRolledState()
    {
      TestHelper.ConfigureDiceMock(diceMock, new[] { 6, 1, 2, 2, 6 }, new[] { 6, 5, 6 });
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
