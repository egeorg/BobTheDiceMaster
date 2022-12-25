namespace BobTheDiceMaster
{
  public class DecisionInfo
  {
    public double Value { get; protected set; }
    public CombinationTypes Combination { get; protected set; }
    public int[] Reroll { get; protected set; }

    public DecisionInfo(double value, CombinationTypes combination, int[] reroll = null)
    {
      Value = value;
      Combination = combination;
      Reroll = reroll;
    }

    public DecisionInfo()
    {
      //do nothing
    }

    public override string ToString()
    {
      string rerollString = Reroll != null ? string.Join(",", Reroll) : "null";
      return $"DecisionInfo(Value={Value},Combination={Combination},Reroll=({rerollString}))";
    }
  }
}
