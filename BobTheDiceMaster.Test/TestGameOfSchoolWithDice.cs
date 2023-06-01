using BobTheDiceMaster.GameOfSchool;
using Moq;

namespace BobTheDiceMaster.Test
{
  public class TestGameOfSchoolWithDice
  {
    private GameOfSchoolWithDice game;
    private Mock<IDie> diceMock;
    private Mock<IGameOfSchool> underlayingGameMock;

    public TestGameOfSchoolWithDice()
    {
      diceMock = new Mock<IDie>();
      underlayingGameMock = new Mock<IGameOfSchool>();
      // Use the least sofisticated GameOfSchoolWithDice implementation to
      // test the functionality implemented in the GameOfSchoolWithDice itself, i.e.
      // not virtual and not supposed to be overriden.
      game = new GameOfSchoolWithDiceAndHumanPlayer(underlayingGameMock.Object, diceMock.Object);
    }

    [Fact]
    public void GenerateRoll_CallsUnderlayingGameSetRoll()
    {
      int[] generatedDiceValues = new[] { 6, 1, 2, 2, 6 };
      TestHelper.ConfigureDiceMock(diceMock, generatedDiceValues);

      game.GenerateRoll();

      underlayingGameMock.Verify(x => x.SetCurrentRoll(new DiceRollDistinct(generatedDiceValues)), Times.Once);
    }
  }
}
