using System;

namespace BobTheDiceMaster
{
  [Flags]
  public enum CombinationTypes
  {
    None = 0,
    Grade1 = 0x1,
    Grade2 = 0x2,
    Grade3 = 0x4,
    Grade4 = 0x8,
    Grade5 = 0x10,
    Grade6 = 0x20,
    Pair = 0x40,
    Three = 0x80,
    TwoPairs = 0x100,
    Full = 0x200,
    Care = 0x400,
    SmallStreet = 0x800,
    BigStreet = 0x1000,
    Trash = 0x2000
  }
}
