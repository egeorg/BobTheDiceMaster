using System;

namespace BobTheDiceMaster
{
  /// <summary>
  /// Flag enum that represents a number of combination types used by a game of school.
  /// </summary>
  /// <remarks>
  /// Ordered by probability to roll a combination in a single roll (without rerolls).
  /// </remarks>
  [Flags]
  public enum CombinationTypes : uint
  {
    None = 0,
    Grade1 = 0x1,
    Grade2 = 0x2,
    Grade3 = 0x4,
    Grade4 = 0x8,
    Grade5 = 0x10,
    Grade6 = 0x20,
    Trash = 0x40,
    Pair = 0x80,
    TwoPairs = 0x100,
    Set = 0x200,
    Full = 0x400,
    Care = 0x800,
    LittleStraight = 0x1000,
    BigStraight = 0x2000,
    Poker = 0x4000,
    All = 0x7fff,
    School = Grade1 | Grade2 | Grade3 | Grade4 | Grade5 | Grade6,
    AllButSchool = All - School
  }
}
