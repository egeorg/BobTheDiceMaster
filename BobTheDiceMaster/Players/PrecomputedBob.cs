using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace BobTheDiceMaster
{
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

    private const string precomputedDecisionsResource = "BobTheDiceMaster.Players.precomputedDecisions.zip";

    private Decision ExtractDecision(byte decisionByte, DiceRoll currentRoll)
    {
      // 6-th and 7-th bytes encode decision type, 1-5-th encode payload (reroll or combination).
      switch (decisionByte & 0b01100000)
      {
        case 0b00100000:
          List<int> reroll = new List<int>();
          //TODO: 5 to const?
          for (int i = 0; i < 5; ++i)
          {
            if ((decisionByte & (1 << i)) != 0)
            {
              reroll.Add(currentRoll[i]);
            }
          }
          return new Reroll(reroll);
        case 0b01000000:
          return new Score((CombinationTypes)(1 << ((decisionByte & 0b00001111) - 1)));
        case 0b01100000:
          return new CrossOut((CombinationTypes)(1 << ((decisionByte & 0b00001111) - 1)));
        default:
          throw new ArgumentException(
            $"Unknown decision type in '{Convert.ToString(decisionByte, 2).PadLeft(8, '0')}'");
      }
    }
  }
}
