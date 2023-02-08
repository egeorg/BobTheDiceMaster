using Moq;

namespace BobTheDiceMaster.Test
{
  public static class TestHelper
  {
    public const double Tolerance = 0.000001;

    public static Mock<IDie> GetDiceMock(params int[][] results)
    {
      var dieMock = new Mock<IDie>();

      foreach (int[] result in results)
      {
        dieMock.Setup(die => die.Roll(result.Length))
          .Returns(result);
      }

      return dieMock;
    }
  }
}
