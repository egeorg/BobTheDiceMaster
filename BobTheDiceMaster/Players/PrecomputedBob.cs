using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace BobTheDiceMaster
{
  /// <summary>
  /// Artificial intelligence implementation of an <see cref="IPlayer"/>
  /// sped up using precomputed results.
  /// Faster than other implementations for bigger number of rerolls left,
  /// slower for situations when <paramref name="rerollsLeft" /> == 0.
  /// </summary>
  public class PrecomputedBob : IPlayer
  {
    public Decision DecideOnRoll(CombinationTypes availableCombinations, DiceRoll currentRoll, int rerollsLeft)
    {

      using (ZipArchive precomputedDecisionsArchive =
        new ZipArchive(
          Assembly.GetAssembly(
            GetType()).GetManifestResourceStream(precomputedDecisionsResource)))
      {
        var entry = precomputedDecisionsArchive.GetEntry(
          $"{rerollsLeft}_{String.Join("", Enumerable.Range(0, 5).Select(i => currentRoll[i]))}.bin");
        using (var stream = entry.Open())
        {
          // It's not possible to read the exact byte in DeflateStream.
          // Read exactly necessary amount of bytes.
          byte[] decisionByte = new byte[(int)availableCombinations];
          stream.Read(decisionByte, 0, (int)availableCombinations);
          return ExtractDecision(decisionByte[(int)availableCombinations - 1], currentRoll);
        }
      }
    }

    // Precomputed using a BobTheDiceMaster.Precomputer tool, it's in this solution as well.
    private const string precomputedDecisionsResource = "BobTheDiceMaster.Players.precomputedDecisions.zip";

    // 6-th and 7-th bytes encode decision type, 1-5-th encode payload (reroll or combination).
    private const int DecisionTypeMask = 0b01100000;
    private const int DecisionPayloadMask = 0b00011111;

    private const int RerollDecisionBits = 0b00100000;
    private const int ScoreDecisionBits = 0b01000000;
    private const int CrossOutDecisionBits = 0b01100000;

    private Decision ExtractDecision(byte decisionByte, DiceRoll currentRoll)
    {
      switch (decisionByte & DecisionTypeMask)
      {
        case RerollDecisionBits:
          return new Reroll(GetDiceIndexesToRerollDecisionPayload(decisionByte, currentRoll));
        case ScoreDecisionBits:
          return new Score(GetCombinationTypesDecisionPayload(decisionByte));
        case CrossOutDecisionBits:
          return new CrossOut(GetCombinationTypesDecisionPayload(decisionByte));
        default:
          throw new ArgumentException(
            $"Unknown decision type in '{Convert.ToString(decisionByte, 2).PadLeft(8, '0')}'");
      }
    }

    private CombinationTypes GetCombinationTypesDecisionPayload(byte decisionByte)
    {
      return (CombinationTypes)(1 << ((decisionByte & DecisionPayloadMask) - 1));
    }

    private List<int> GetDiceIndexesToRerollDecisionPayload(byte decisionByte, DiceRoll currentRoll)
    {
      List<int> reroll = new List<int>();
      for (int i = 0; i < DiceRoll.MaxDiceAmount; ++i)
      {
        if ((decisionByte & (1 << i)) != 0)
        {
          reroll.Add(currentRoll[i]);
        }
      }
      return reroll;
    }
  }
}
