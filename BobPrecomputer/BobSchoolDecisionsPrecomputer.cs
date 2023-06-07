using BobTheDiceMaster;

namespace BobPrecomputer
{
  /// <summary>
  /// Produces a precomputed set of decisions for the dice game called School.
  /// How to use: create a new <see cref="BobSchoolDecisionsPrecomputer"/> instance, pass relevant
  /// <see cref="IPlayer"/> instance and existing directory for the precomputed output to the
  /// constructor. Then call <see cref="Precompute()"/> to produce a precomputed set of decisions.
  /// </summary>
  public class BobSchoolDecisionsPrecomputer
  {
    /// <summary>
    /// Directory path used to output precomputed decisions if custom output path is not passed to the constructor.
    /// </summary>
    public const string DefaultOutputPath = "C:\\precomputedDecisions";

    /// <summary>
    /// Create a new <see cref="BobSchoolDecisionsPrecomputer"/> that uses
    /// <paramref name="bob"/> player to precompute all decisions for all
    /// possible game contexts, serialize and output them to
    /// <paramref name="outputPath"/>. If <paramref name="outputPath"/> is not
    /// set or is null, then the <see cref="DefaultOutputPath"/> is used instead.
    /// </summary>
    public BobSchoolDecisionsPrecomputer(IPlayer bob, string outputPath = null)
    {
      this.bob = bob;
      if (outputPath != null)
      {
        this.outputPath = outputPath;
      }
      else
      {
        this.outputPath = DefaultOutputPath;
      }
    }

    /// <summary>
    /// Precompute decisions for all possible game contexts, serialize and
    /// output them to the path passed to the constructor or
    /// <see cref="DefaultOutputPath"/> if it was not specified in the constructor.
    /// </summary>
    public void Precompute()
    {
      for (int rerollsLeft = 0; rerollsLeft <= 2; ++rerollsLeft)
      {
        Console.WriteLine($"Computing decisions for {rerollsLeft} roll[s] left...");
        foreach (DiceRoll roll in DiceRoll.RollResultsOfAllDice)
        {
          Console.WriteLine($"Computing decisions for reroll {roll}.");
          string fileName =
            $"{rerollsLeft}_{String.Join("", Enumerable.Range(0, 5).Select(i => roll[i]))}.bin";
          File.WriteAllBytes(Path.Combine(outputPath, fileName), Precompute(roll, rerollsLeft));
        }
      }
    }

    private IPlayer bob;
    private readonly string outputPath;

    private byte[] Precompute(DiceRoll roll, int rerollsLeft)
    {
      int availableCombinationsAmount = (1 << 15) - 1;
      byte[] decisions = new byte[availableCombinationsAmount];
      int decisionsCounter = 0;
      for (uint combinationsUint = 1; combinationsUint < availableCombinationsAmount; ++combinationsUint)
      {
        Task<Decision> decideOnRollTask = bob.DecideOnRollAsync((CombinationTypes)combinationsUint, roll, rerollsLeft);

        Decision decision = decideOnRollTask.Result;

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
      return (byte)(GetIndex(score.Combination) | 0b01000000);
    }

    private static byte ByteSerialize(CrossOut crossOut)
    {
      return (byte)(GetIndex(crossOut.Combination) | 0b01100000);
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
