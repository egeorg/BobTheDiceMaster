namespace BobTheDiceMaster
{
  public class GameOfSchoolContext
  {
    public CombinationTypes AvailableCombinations { get; set; }
    public DiceRoll DiceRoll { get; set; }
    public int RollsLeft { get; set; }

    public GameOfSchoolContext(CombinationTypes availableCombinations, DiceRoll diceRoll, int rollsLeft)
    {
      AvailableCombinations = availableCombinations;
      DiceRoll = diceRoll;
      RollsLeft = rollsLeft;
    }

    public override string ToString()
    {
      return $"GameOfDiceState(AvailableCombinations={AvailableCombinations};DiceRoll={DiceRoll};RollsLeft={RollsLeft})";
    }
  }
}
