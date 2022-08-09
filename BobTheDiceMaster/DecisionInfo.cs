namespace BobTheDiceMaster
{
  class DecisionInfo
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
  }
}
