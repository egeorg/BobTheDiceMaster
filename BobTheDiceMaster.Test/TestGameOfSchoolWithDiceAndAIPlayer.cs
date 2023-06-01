using Moq;
using BobTheDiceMaster.GameOfSchool;

namespace BobTheDiceMaster.Test
{
  public class TestGameOfSchoolWithDiceAndAIPlayer
  {
    private GameOfSchoolWithDiceAndAIPlayer game;
    private Mock<IDie> diceMock;
    private Mock<IPlayer> playerMock;
    private Mock<IGameOfSchool> underlayingGameMock;

    public TestGameOfSchoolWithDiceAndAIPlayer()
    {
      diceMock = new Mock<IDie>();
      playerMock = new Mock<IPlayer>();
      underlayingGameMock = new Mock<IGameOfSchool>();
      game = new GameOfSchoolWithDiceAndAIPlayer(underlayingGameMock.Object, diceMock.Object, playerMock.Object);
    }

    [Fact]
    public void GenerateAndApplyDecision_CallsUnderlayingGameReroll_WhenPlayerRerolls()
    {
      int[] initialRoll = new[] { 6, 1, 2, 2, 6 };
      int[] newDiceValues = new[] { 6, 5, 6 };
      int[] diceValuesToReroll = new[] { 1, 2, 2 };
      int[] diceIndexesToReroll = new[] { 1, 2, 3 };
      TestHelper.ConfigureDiceMock(diceMock, initialRoll, newDiceValues);

      playerMock.Setup(
        player => player.DecideOnRoll(
          It.IsAny<CombinationTypes>(), It.IsAny<DiceRoll>(), It.IsAny<int>()))
        .Returns(new Reroll(diceValuesToReroll));

      underlayingGameMock.Setup(x => x.CurrentRoll).Returns(new DiceRollDistinct(initialRoll));

      game.GenerateAndApplyDecision();

      underlayingGameMock.Verify(x => x.ApplyRerollToDiceAtIndexes(newDiceValues, diceIndexesToReroll), Times.Once);
    }

    //TODO: replace long setups by mock setup
    [Fact]
    public void GenerateAndApplyDecision_ScoresCorrectly()
    {
      underlayingGameMock
        .Setup(x => x.CurrentRoll)
        .Returns(new DiceRollDistinct(new[] { 6, 6, 6, 6, 6 }));
      CombinationTypes combinationToScore = CombinationTypes.Grade6;
      playerMock.Setup(
        player => player.DecideOnRoll(
          It.IsAny<CombinationTypes>(), It.IsAny<DiceRoll>(), It.IsAny<int>()))
        .Returns(new Score(combinationToScore));

      game.GenerateAndApplyDecision();

      underlayingGameMock.Verify(x => x.ScoreCombination(combinationToScore), Times.Once);
    }

    [Fact]
    public void GenerateAndApplyDecision_CrossesOutCorrectly()
    {
      underlayingGameMock
        .Setup(x => x.CurrentRoll)
        .Returns(new DiceRollDistinct(new[] { 6, 6, 6, 6, 6 }));
      CombinationTypes combinationToCrossOut = CombinationTypes.LittleStraight;
      playerMock.Setup(
        player => player.DecideOnRoll(
          It.IsAny<CombinationTypes>(), It.IsAny<DiceRoll>(), It.IsAny<int>()))
        .Returns(new CrossOut(combinationToCrossOut));

      game.GenerateAndApplyDecision();

      underlayingGameMock.Verify(x => x.CrossOutCombination(combinationToCrossOut), Times.Once);
    }
  }
}
