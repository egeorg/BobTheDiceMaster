using BobTheDiceMaster.GameOfSchool;
using Moq;

namespace BobTheDiceMaster.Test
{
  public class TestGameOfSchoolWithDiceAndHumanPlayer
  {
    private GameOfSchoolWithDiceAndHumanPlayer game;
    private Mock<IDie> diceMock;
    private Mock<IGameOfSchool> underlayingGameMock;

    public TestGameOfSchoolWithDiceAndHumanPlayer()
    {
      diceMock = new Mock<IDie>();
      underlayingGameMock = new Mock<IGameOfSchool>();
      game = new GameOfSchoolWithDiceAndHumanPlayer(underlayingGameMock.Object, diceMock.Object);
    }

    [Fact]
    public void RerollDiceAtIndexes_CallsUnderlayingGameReroll()
    {
      int[] newDiceValues = new[] { 1, 5, 2 };
      int[] diceIndexesToReroll = new[] { 1, 2, 3 };
      TestHelper.ConfigureDiceMock(diceMock, new[] { 6, 1, 2, 2, 6 }, newDiceValues);
      game.RerollDiceAtIndexes(diceIndexesToReroll);

      underlayingGameMock.Verify(x => x.ApplyRerollToDiceAtIndexes(newDiceValues, diceIndexesToReroll), Times.Once);
    }

    [Fact]
    public void ScoreCombination_CallsUnderlayingGameScoreCombination()
    {
      CombinationTypes combinationToScore = CombinationTypes.Grade5;

      game.ScoreCombination(combinationToScore);

      underlayingGameMock.Verify(x => x.ScoreCombination(combinationToScore), Times.Once);
    }

    [Fact]
    public void CrossOutCombination_CallsUnderlayingGameCrossOutCombinationl()
    {
      CombinationTypes combinationToCrossOut = CombinationTypes.Pair;

      game.CrossOutCombination(combinationToCrossOut);

      underlayingGameMock.Verify(x => x.CrossOutCombination(combinationToCrossOut), Times.Once);
    }
  }
}
