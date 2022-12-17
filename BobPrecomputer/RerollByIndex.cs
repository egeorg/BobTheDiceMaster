using BobTheDiceMaster;

namespace BobPrecomputer
{
  /// <summary>
  /// Used only for output of precomputed decisions
  /// </summary>
  public class RerollByIndex
  {
    private readonly byte rerollByteRepresentation = (byte)0b00100000;

    public RerollByIndex(DiceRoll roll, Reroll reroll)
    {
      List<int> valuesToRerollSorted = new List<int>(reroll.DiceValuesToReroll);
      valuesToRerollSorted.Sort();
      int rollIndex = 0;
      foreach (int value in valuesToRerollSorted)
      {
        if (rollIndex == roll.DiceAmount)
        {
          throw new ArgumentException(
            $"DiceRoll '{roll}' does not conatain all the values from Reroll '{reroll}'");
        }
        while (roll[rollIndex] != value)
        {
          rollIndex++;
          if (rollIndex == roll.DiceAmount)
          {
            throw new ArgumentException(
              $"DiceRoll '{roll}' does not conatain all the values from Reroll '{reroll}'");
          }
        }

        rerollByteRepresentation |= (byte)(1 << rollIndex);
        rollIndex++;
      }
    }

    public byte ByteSerialize()
    {
      return rerollByteRepresentation;
    }
  }
}
