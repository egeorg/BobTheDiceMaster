﻿using System.Collections.Generic;
using System.Linq;

namespace BobTheDiceMaster
{
  public class Decision
  {
    /// <remarks>
    /// Public setter is required only to make it serializable, otherwise setter can be removed.
    /// </remarks>
    public List<DecisionInfoVerbose> RatedDecisionInfo { get; set; }

    /// <remarks>
    /// Parameterless constructor is required only to make it serializable.
    /// </remarks>
    public Decision()
    { }

    public Decision(IEnumerable<DecisionInfoVerbose> decisionInfos)
    {
      if (decisionInfos != null)
      {
        RatedDecisionInfo = new SortedSet<DecisionInfoVerbose>(
          decisionInfos, new DecisionInfoInverseByValueComparer()).ToList();
      }
    }
  }
}
