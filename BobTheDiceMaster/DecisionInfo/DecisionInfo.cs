namespace BobTheDiceMaster
{
  /// <summary>
  /// Class that represents information required to make a decision given a game
  /// of school context (available combinaitons and current dice roll result) and
  /// a double value that can be used to compare two corresponding decisions.
  /// Used in Bob algorithms to compare different options and to construct a final decision.
  /// </summary>
  public class DecisionInfo
  {
    /// <summary>
    /// A number that represents profit of the decision.
    /// A decision with the highest value is considered optimal.
    /// </summary>
    public double Value { get; protected set; }

    /// <summary>
    /// A combination to score for <see cref="Score"/> decision (if current dice roll
    /// from a game context can be scored as the combination) or a combination to cross out
    /// for a <see cref="CrossOut"/> decision (if current dice roll
    /// from the game context can't be scored as the combination).
    /// If reroll is not null, it is a combination that Bob aims to score after
    /// reroll[s]. If it aims at more than one combination, it is the one that yields
    /// the highest score.
    /// </summary>
    public CombinationTypes Combination { get; protected set; }

    /// <summary>
    /// Dice to reroll if the optimal decision is <see cref="Reroll"/>.
    /// </summary>
    public int[] DiceValuesToReroll { get; protected set; }

    public DecisionInfo(double value, CombinationTypes combination, int[] diceValuesToReroll = null)
    {
      Value = value;
      Combination = combination;
      DiceValuesToReroll = diceValuesToReroll;
    }

    public override string ToString()
    {
      string rerollString = DiceValuesToReroll != null
        ? string.Join(",", DiceValuesToReroll)
        : "null";
      return $"DecisionInfo(Value={Value},Combination={Combination},Reroll=({rerollString}))";
    }

    /// <summary>
    /// Used by descendants constructor.
    /// </summary>
    protected DecisionInfo()
    {
      //do nothing
    }
  }
}
