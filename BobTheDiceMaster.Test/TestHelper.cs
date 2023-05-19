using Moq;

namespace BobTheDiceMaster.Test
{
  public static class TestHelper
  {
    public const double Tolerance = 0.000001;

    public static Mock<IDie> GetDiceMock(params int[][] results)
    {
      var diceMock = new Mock<IDie>();

      foreach (int[] result in results)
      {
        diceMock.Setup(die => die.Roll(result.Length))
          .Returns(result);
      }

      return diceMock;
    }
    public static Mock<IDie> ConfigureDiceMock(Mock<IDie> diceMock, params int[][] results)
    {
      foreach (int[] result in results)
      {
        diceMock.Setup(die => die.Roll(result.Length))
          .Returns(result);
      }

      return diceMock;
    }
  }
}
