using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  /// <summary>
  /// A decision to reroll a number of dice.
  /// </summary>
  public class Reroll : Decision
  {
    /// <summary>
    /// Values of the dice to be rerolled.
    /// </summary>
    public List<int> DiceValuesToReroll { get; }

    public Reroll(
      IEnumerable<int> diceValuesToReroll,
      IEnumerable<DecisionInfoVerbose> decisionInfos = null)
      : base(decisionInfos)
    {
      DiceValuesToReroll = diceValuesToReroll.ToList();
    }

    public override string ToString()
    {
      return $"Reroll({string.Join(", ", DiceValuesToReroll)})";
    }

    /// <summary>
    /// Two <see cref="Reroll"/>s are considered equal if values to reroll are the same (ignoring order).
    /// </summary>
    public override bool Equals(object obj)
    {
      var otherReroll = obj as Reroll;

      if (otherReroll == null)
      {
        return false;
      }

      return !DiceValuesToReroll.Except(otherReroll.DiceValuesToReroll).Any()
        && !otherReroll.DiceValuesToReroll.Except(DiceValuesToReroll).Any();
    }

    public override int GetHashCode()
    {
      return DiceValuesToReroll.GetHashCode();
    }
  }
}
