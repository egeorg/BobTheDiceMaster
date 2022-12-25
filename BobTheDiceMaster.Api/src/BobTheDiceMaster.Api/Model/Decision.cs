namespace BobTheDiceMaster.Api.Model
{
  public class Decision
  {
    public Reroll? Reroll { get; set; }
    public Score? Score { get; set; }
    public CrossOut? CrossOut { get; set; }

    public Decision(BobTheDiceMaster.Decision decision)
    {
      switch (decision)
      {
        case BobTheDiceMaster.Reroll reroll:
          Reroll = new Reroll
          {
            ValuesToReroll = reroll.DiceValuesToReroll.ToArray()
          };
          break;
        case BobTheDiceMaster.Score score:
          Score = new Score
          {
            Combination = score.CombinationToScore
          };
          break;
        case BobTheDiceMaster.CrossOut crossOut:
          CrossOut = new CrossOut
          {
            Combination = crossOut.Combination
          };
          break;
        default:
          throw new ArgumentException(
            $"Unexpected decision to convert. " +
            $"Expected only {typeof(BobTheDiceMaster.Reroll)}, " +
            $"{typeof(BobTheDiceMaster.Score)} or " +
            $"{typeof(BobTheDiceMaster.CrossOut)}, " +
            $"but was {decision.GetType} ({decision}).");
      };
    }
  }
}
