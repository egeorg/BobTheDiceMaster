namespace BobTheDiceMaster
{
  public interface IPlayer
  {
    Decision DecideOnRoll(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerollsLeft);
  }
}
