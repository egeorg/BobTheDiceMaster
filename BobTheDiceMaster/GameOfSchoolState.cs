namespace BobTheDiceMaster
{
  /// <summary>
  /// State of a <see cref="GameOfSchoolBase"/> finite state machine.
  /// </summary>
  public enum GameOfSchoolState
  {
    None = 0,
    Idle,
    Rolled,
    GameOver
  }
}
