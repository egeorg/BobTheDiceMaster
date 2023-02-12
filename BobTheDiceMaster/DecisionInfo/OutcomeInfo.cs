namespace BobTheDiceMaster
{
  /// <summary>
  /// Information about a possible outcome of a player's single turn. A single
  /// outcome represents all the situations when a specific elementary combinaiton
  /// is optimal to score or cross out, how much value all those situations yield
  /// on average and probability that dice would be rolled so that this specific
  /// combination is scored or crossed out.
  /// 
  /// For example, if <see cref="Combination"/> is <see cref="CombinationTypes.Poker"/>
  /// and <see cref="IsScored"/> equals true, then <see cref="Probability"/> is probability
  /// of a situation when it is optimal to score <see cref="CombinationTypes.Poker"/>,
  /// and <see cref="Value"/> is average profit in this case. <see cref="Value"/> and 
  /// <see cref="Probability"/> are calculated for the case of optimal decisions across
  /// all possible reroll outcomes if there are rerolls left in current game context.
  /// 
  /// Given a set of possible outcomes for specific decision, you can understand
  /// motivation behind the decision.
  /// </summary>
  public class OutcomeInfo
  {
    /// <summary>
    /// Average profit of the situation when <see cref="Combination"/>
    /// is optimal (to score or cross out or aim at depending on <see cref="IsScored"/>).
    /// </summary>
    public double Value { get; private set; }

    /// <summary>
    /// Probability of the situation when <see cref="Combination"/>
    /// is optimal to score of cross out depending on <see cref="IsScored"/>.
    /// </summary>
    public double Probability { get; private set; }

    /// <summary>
    /// A combination to be scored or crossed out.
    /// </summary>
    public CombinationTypes Combination { get; }

    /// <summary>
    /// True iff the combination is scored (and not crossed out) in this outcome.
    /// </summary>
    public bool IsScored { get; set; }

    public OutcomeInfo(
      double value,
      CombinationTypes combination,
      double probability,
      bool isScored)
    {
      Value = value;
      Combination = combination;
      Probability = probability;
      IsScored = isScored;
    }

    /// <summary>
    /// Increase <see cref="Value"/> by <paramref name="increment"/>.
    /// </summary>
    public void IncreaseValue(double increment)
    {
      Value += increment;
    }

    /// <summary>
    /// Increase <see cref="Probability"/> by <paramref name="increment"/>.
    /// </summary>
    public void IncreaseProbability(double increment)
    {
      Probability += increment;
    }

    public override string ToString()
    {
      // G4 to use only 4 digits so that the string representation is more compact.
      // This class is often used in lists.
      return $"{(IsScored ? "S" : "X")} {Combination}({Value:G4},{Probability:G4})";
    }
  }
}
