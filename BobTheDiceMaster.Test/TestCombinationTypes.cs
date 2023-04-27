namespace BobTheDiceMaster.Test
{
  public class TestCombinationTypes
  {

    [Fact]
    public void GetElementaryCombinations_ReturnsConstituents_ForSimpleDisjunction()
    {
      CombinationTypes[] constituents =
      {
        CombinationTypes.Grade6,
        CombinationTypes.Full,
        CombinationTypes.Trash
      };
      CombinationTypes disjunction = constituents.Aggregate(
        CombinationTypes.None,
        (x, y) => x | y);
      Assert.Equal(constituents, disjunction.GetElementaryCombinationTypes());
    }

    [Theory]
    [InlineData(CombinationTypes.Grade1)]
    [InlineData(CombinationTypes.Grade2)]
    [InlineData(CombinationTypes.Grade3)]
    [InlineData(CombinationTypes.Grade4)]
    [InlineData(CombinationTypes.Grade5)]
    [InlineData(CombinationTypes.Grade6)]
    [InlineData(CombinationTypes.Pair)]
    [InlineData(CombinationTypes.Set)]
    [InlineData(CombinationTypes.TwoPairs)]
    [InlineData(CombinationTypes.Full)]
    [InlineData(CombinationTypes.Care)]
    [InlineData(CombinationTypes.LittleStraight)]
    [InlineData(CombinationTypes.BigStraight)]
    [InlineData(CombinationTypes.Poker)]
    [InlineData(CombinationTypes.Trash)]
    public void IsElementary_ReturnsTrue_ForElementaryCombinations(CombinationTypes combination)
    {
      Assert.True(combination.IsElementary());
    }

    [Theory]
    [InlineData(CombinationTypes.School)]
    [InlineData(CombinationTypes.All)]
    [InlineData(CombinationTypes.AllButSchool)]
    [InlineData(CombinationTypes.Grade6 | CombinationTypes.Full)]
    public void IsElementary_ReturnsFalse_ForNonElementaryCombinations(CombinationTypes combination)
    {
      Assert.False(combination.IsElementary());
    }

    [Theory]
    [InlineData(CombinationTypes.Grade1)]
    [InlineData(CombinationTypes.Grade2)]
    [InlineData(CombinationTypes.Grade3)]
    [InlineData(CombinationTypes.Grade4)]
    [InlineData(CombinationTypes.Grade5)]
    [InlineData(CombinationTypes.Grade6)]
    public void IsFromSchool_ReturnsTrue_ForSchoolCombinations(CombinationTypes combination)
    {
      Assert.True(combination.IsFromSchool());
    }

    [Theory]
    [InlineData(CombinationTypes.Pair)]
    [InlineData(CombinationTypes.Set)]
    [InlineData(CombinationTypes.TwoPairs)]
    [InlineData(CombinationTypes.Full)]
    [InlineData(CombinationTypes.Care)]
    [InlineData(CombinationTypes.LittleStraight)]
    [InlineData(CombinationTypes.BigStraight)]
    [InlineData(CombinationTypes.Poker)]
    [InlineData(CombinationTypes.Trash)]
    public void IsFromSchool_ReturnsFalse_ForNonSchoolCombinations(CombinationTypes combination)
    {
      Assert.False(combination.IsFromSchool());
    }
  }
}
