namespace BobTheDiceMaster.Test
{
  public class TestCombinatorics
  {
    [Theory]
    [InlineData(1, 1)]
    [InlineData(720, 6)]
    [InlineData(3628800, 10)]
    public void Factorial_IsCorrect_ForPositiveValues(long expectedFactorial, int factorialArgument)
    {
      Assert.Equal(expectedFactorial, Combinatorics.Factorial(factorialArgument));
    }

    [Fact]
    public void Factorial_Is1_For0()
    {
      Assert.Equal(1, Combinatorics.Factorial(0));
    }

    [Fact]
    public void Factorial_DoesNotOverflow_ForMaximalAllowedArgument()
    {
      Assert.Equal(2432902008176640000, Combinatorics.Factorial(20));
    }

    [Fact]
    public void Factorial_ThrowsArgumentException_ForNegativeArgument()
    {
      Assert.Throws<ArgumentException>(() => Combinatorics.Factorial(-1));
    }

    [Fact]
    public void Factorial_ThrowsArgumentException_ForMinimalForbiddenArgument()
    {
      Assert.Throws<ArgumentException>(() => Combinatorics.Factorial(21));
    }

    [Theory]
    [InlineData(3, 3, 2)]
    [InlineData(9, 9, 1)]
    [InlineData(252, 10, 5)]
    public void NumberOfCombinations_IsCorrect_ForPositiveValues(int expectedResult, int n, int k)
    {
      Assert.Equal(expectedResult, Combinatorics.Cnk(n, k));
    }

    /// <summary>
    /// The results are not so big to be worried about, but big numbers may appear during
    /// internal calculations if the algorithm is not smart enough.
    /// </summary>
    [Theory]
    [InlineData(1, 20, 0)]
    [InlineData(1, 20, 20)]
    [InlineData(184756, 20, 10)]
    public void NumberOfCombinaitons_DoesNotOverflow_ForMaximalAllowedArguments(int expectedResult, int n, int k)
    {
      Assert.Equal(expectedResult, Combinatorics.Cnk(n, k));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(11)]
    public void NumberOfCombinations_Is1_ForKEqual0(int n)
    {
      Assert.Equal(1, Combinatorics.Cnk(n, 0));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(12)]
    public void NumberOfCombinations_Is1_ForKEqualN(int n)
    {
      Assert.Equal(1, Combinatorics.Cnk(n, n));
    }

    [Fact]
    public void NumberOfCombinaitons_ThrowsArgumentException_ForMinimalForbiddenN()
    {
      Assert.Throws<ArgumentException>(() => Combinatorics.Cnk(21, 0));
    }

    [Fact]
    public void NumberOfCombinaitons_ThrowsArgumentException_ForNegativeN()
    {
      Assert.Throws<ArgumentException>(() => Combinatorics.Cnk(-1, 0));
    }

    [Fact]
    public void NumberOfCombinaitons_ThrowsArgumentException_ForNegativeK()
    {
      Assert.Throws<ArgumentException>(() => Combinatorics.Cnk(0, -1));
    }
  }
}
