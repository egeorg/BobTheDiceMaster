namespace BobTheDiceMaster
{
  public class DecisionInfo
  {
    /// <remarks>
    /// Public setter is required only to make it serializable, it can be protected.
    /// </remarks>
    public double Value { get; set; }
    /// <remarks>
    /// Public setter is required only to make it serializable, it can be protected.
    /// </remarks>
    public CombinationTypes Combination { get; set; }
    /// <remarks>
    /// Public setter is required only to make it serializable, it can be protected.
    /// </remarks>
    public int[] Reroll { get; set; }

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
