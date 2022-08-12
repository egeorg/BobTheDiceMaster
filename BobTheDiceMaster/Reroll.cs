using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class Reroll : IDecision
  {
    public SortedSet<DecisionInfoVerbose> RatedDecisionInfo { get; }

    public List<int> DiceToReroll { get; }

    public Reroll(
      IEnumerable<int> diceToReroll,
      IEnumerable<DecisionInfoVerbose> ratedDecisionInfo = null)
    {
      RatedDecisionInfo = new SortedSet<DecisionInfoVerbose>(
        ratedDecisionInfo,
        new DecisionInfoInverseByValueComparer());
      DiceToReroll = diceToReroll.ToList();
    }

    public override string ToString()
    {
      return $"Reroll({string.Join(", ", DiceToReroll)})";
    }
  }
}
