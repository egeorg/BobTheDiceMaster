namespace BobTheDiceMaster
{
  public interface IPlayer
  {
    IDecision DecideOnRoll(
      CombinationTypes availableCombinations,
      DiceRoll currentRoll,
      int rerloosLeft);
  }
}
