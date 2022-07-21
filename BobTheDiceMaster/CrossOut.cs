namespace BobTheDiceMaster
{
  public class CrossOut : IDecision
  {
    public CombinationTypes Combination { get; }

    public CrossOut(CombinationTypes combination)
    {
      Combination = combination;
    }

    public override string ToString()
    {
      return $"CrossOut({Combination})";
    }
  }
}
