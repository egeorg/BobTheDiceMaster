using BobTheDiceMaster;

namespace BobPrecomputer
{
  /// <summary>
  /// Produces a precomputed set of decisions for the dice game called School.
  /// How to use: create a new <see cref="BobSchoolPrecomputer"> instance, pass relevant
  /// <see cref="IPlayer"> instance and existing directory for the precomputed output to the
  /// constructor. Then call <see cref="Precompute()"> to produce a precomputed set of decisions.
  /// </summary>
  public class BobSchoolPrecomputer
  {
    public BobSchoolPrecomputer(IPlayer bob, string outputPath = null)
    {
      this.bob = bob;
      if (outputPath != null)
      {
        this.outputPath = outputPath;
      }
    }

    public void Precompute()
    {
      for (int rerollsLeft = 0; rerollsLeft <= 2; ++rerollsLeft)
      {
        Console.WriteLine($"Computing decisions for {rerollsLeft} roll[s] left...");
        foreach (DiceRoll roll in DiceRoll.Roll5Results)
        {
          Console.WriteLine($"Computing decisions for reroll {roll}.");
          string fileName =
            $"{rerollsLeft}_{String.Join("", Enumerable.Range(0, 5).Select(i => roll[i]))}.bin";
          File.WriteAllBytes(Path.Combine(outputPath, fileName), Precompute(roll, rerollsLeft));
        }
      }
    }

    private IPlayer bob;
    private string outputPath = "C:\\precomputedDecisions";

    private byte[] Precompute(DiceRoll roll, int rerollsLeft)
    {
      int availableCombinationsAmount = (1 << 15) - 1;
      byte[] decisions = new byte[availableCombinationsAmount];
      int decisionsCounter = 0;
      for (uint combinationsUint = 1; combinationsUint < availableCombinationsAmount; ++combinationsUint)
      {
        Decision decision = bob.DecideOnRoll((CombinationTypes)combinationsUint, roll, rerollsLeft);

        switch (decision)
        {
          case Score score:
            decisions[decisionsCounter] = ByteSerialize(score);
            break;
          case CrossOut crossOut:
            decisions[decisionsCounter] = ByteSerialize(crossOut);
            break;
          case Reroll rerollByValue:
            decisions[decisionsCounter] = ByteSerialize(roll, rerollByValue);
            break;
        }
        decisionsCounter++;
      }

      return decisions;
    }

    private static byte ByteSerialize(Score score)
    {
      return (byte)(GetIndex(score.CombinationToScore) | 0b01000000);
    }

    private static byte ByteSerialize(CrossOut crossOut)
    {
      return (byte)(GetIndex(crossOut.Combination) | 0b01000000);
    }

    private static byte ByteSerialize(DiceRoll roll, Reroll rerollByValue)
    {
      RerollByIndex rerollByIndex = new RerollByIndex(roll, rerollByValue);
      return rerollByIndex.ByteSerialize();
    }

    private static byte GetIndex(CombinationTypes combination)
    {
      if (!combination.IsElementary())
      {
        throw new ArgumentException(
          $"Index can be calculated only for elementary combinations, not for '{combination}'");
      }
      uint backingUint = (uint)combination;
      byte index = 0;
      while ((backingUint & ((uint)1 << index)) == 0)
      {
        index++;
      }

      return ++index;
    }
  }
}
