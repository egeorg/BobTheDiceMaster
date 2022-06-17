using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobTheDiceMaster
{
  public class D6
  {
    #region private fields;
    private Random rng;
    #endregion

    #region public constants
    public const int MaxValue = 6;
    #endregion

    #region public methods
    public D6()
    {
      rng = new Random();
    }

    public int Roll()
    {
      return rng.Next(1, D6.MaxValue + 1);
    }
    #endregion
  }
}
