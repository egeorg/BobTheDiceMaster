namespace BobTheDiceMaster.GameOfSchool
{
    /// <summary>
    /// State of a <see cref="GameOfSchoolWithEnvironment"/> finite state machine.
    /// </summary>
    public enum GameOfSchoolState
    {
        None = 0,
        Idle,
        Rolled,
        GameOver
    }
}
