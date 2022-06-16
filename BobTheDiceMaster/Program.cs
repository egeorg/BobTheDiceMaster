using System;

namespace BobTheDiceMaster
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Stargin a game with Bob...");

      GameWithBob game = new GameWithBob();
      game.Start();
    }
  }
}
