namespace BobTheDiceMaster.Api.Model
{
  public class GameOfSchoolContext
  {
    public CombinationTypes AvailableCombinations { get; set; }
    public int[] DiceRoll { get; set; } = new int[0];
    public int RerollsLeft { get; set; }
  }
}
