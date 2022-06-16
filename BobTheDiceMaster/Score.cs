namespace BobTheDiceMaster
{
  public class Score : IDecision
  {
    public CombinationTypes CombinationToScore { get; }

    public Score(CombinationTypes combinationToScore)
    {
      CombinationToScore = combinationToScore;
    }
  }
}
