using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobTheDiceMaster
{
  public class Combinatorics
  {
    public static long Cnk(int n, int k)
    {
      long result = 1;
      // Calculate n!/k!
      for (int i = n; i > k; --i)
      {
        result *= i;
      }

      return result / Factorial(n - k);
    }

    public static long Factorial(int n)
    {
      long result = 1;
      for (int i = n; i > 0; --i)
      {
        result *= i;
      }
      return result;
    }
  }
}
