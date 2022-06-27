using System;

namespace BobTheDiceMaster
{
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
    School = Grade1 | Grade2 | Grade3 | Grade4 | Grade5 | Grade6,
    Pair = 0x40,
    Set = 0x80,
    TwoPairs = 0x100,
    Full = 0x200,
    Care = 0x400,
    SmallStreet = 0x800,
    BigStreet = 0x1000,
    Poker = 0x2000,
    Trash = 0x4000,
    All = 0x7fff,
    AllButSchool = All - School
  }
}
