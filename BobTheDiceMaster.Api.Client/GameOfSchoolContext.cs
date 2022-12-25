namespace BobTheDiceMaster.Api.Client
{
  public class GameOfSchoolContext
  {
    public CombinationTypes AvailableCombinations { get; set; }
    public int[] DiceRoll { get; set; }
    public int RollsLeft { get; set; }
  }
}
