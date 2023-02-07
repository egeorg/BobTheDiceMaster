using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobTheDiceMaster.Test
{
  public static class TestHelper
  {
    public const double Tolerance = 0.000001;

    public static Mock<IDie> GetDiceMock(int[] results)
    {
      var dieMock = new Mock<IDie>();
      dieMock.Setup(die => die.Roll(results.Length))
        .Returns(results);

      return dieMock;
    }
  }
}
