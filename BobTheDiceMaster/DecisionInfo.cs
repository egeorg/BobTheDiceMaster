namespace BobTheDiceMaster
{
  public class DecisionInfo
  {
    public double Value { get; }
    public CombinationTypes Combination { get; }
    public int[] Reroll { get; }

    public DecisionInfo(double value, CombinationTypes combination, int[] reroll = null)
    {
      Value = value;
      Combination = combination;
      Reroll = reroll;
    }

    public override string ToString()
    {
      string rerollString = Reroll != null ? string.Join(",", Reroll) : "null";
      return $"DecisionInfo(Value={Value},Combination={Combination},Reroll=({rerollString}))";
    }
  }
}
