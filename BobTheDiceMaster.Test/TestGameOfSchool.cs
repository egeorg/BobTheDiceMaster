using BobTheDiceMaster.GameOfSchool;

namespace BobTheDiceMaster.Test
{
  public class TestGameOfSchool
  {
    private GameOfSchool.GameOfSchool game;

    public TestGameOfSchool()
    {
      game = new GameOfSchool.GameOfSchool();
    }

    [Fact]
    public void AllowedCombinations_AreOnlyAllGrades_InTheBeginningOfTheGame()
    {
      Assert.Equal(CombinationTypes.School, game.AllowedCombinationTypes);
    }

    [Fact]
    public void State_IsIdle_InTheBeginningOfTheGame()
    {
      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void Score_Is0_InTheBeginningOfTheGame()
    {
      Assert.Equal(0, game.Score);
    }

    private void ScoreGrades456(GameOfSchool.GameOfSchool game)
    {
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 6, 6, 6, 6 }));
      game.ScoreCombination(CombinationTypes.Grade6);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 5, 5, 5 }));
      game.ScoreCombination(CombinationTypes.Grade5);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 4, 4 }));
      game.ScoreCombination(CombinationTypes.Grade4);
    }

    [Fact]
    public void AllowedCombinations_AreOnlyAllGrades_AfterReset()
    {
      ScoreGrades456(game);

      game.Reset();

      Assert.Equal(CombinationTypes.School, game.AllowedCombinationTypes);
    }

    [Fact]
    public void State_IsIdle_AfterReset()
    {
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 5 }));

      game.Reset();

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void Score_Is0_AfterReset()
    {
      ScoreGrades456(game);

      game.Reset();

      Assert.Equal(0, game.Score);
    }

    [Fact]
    public void SetCurrentRoll_Fails_InRolledState()
    {
      int[] rollResult = new[] { 1, 2, 3, 4, 5 };

      game.SetCurrentRoll(new DiceRollDistinct(rollResult));

      Assert.Throws<InvalidOperationException>(() => game.SetCurrentRoll(new DiceRollDistinct(rollResult)));
    }

    [Fact]
    public void SetCurrentRoll_ChangesStateToRolled()
    {
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 5 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);
    }

    [Fact]
    public void SetCurrentRoll_SetsCurrentRollToItsArgument()
    {
      var newRoll = new DiceRollDistinct(new[] { 6, 2, 2, 1, 6 });

      game.SetCurrentRoll(newRoll);

      Assert.Equal(newRoll, game.CurrentRoll);
    }

    [Fact]
    public void SetCurrentRoll_SetsRerollsLeftTo2()
    {
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 6, 2, 2, 1, 6 }));

      Assert.Equal(2, game.RerollsLeft);
    }

    [Fact]
    public void ApplyRerollToDiceAtIndexes_YieldsCorrectResult_InRolledState()
    {
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 6, 1, 2, 2, 6 }));

      game.ApplyRerollToDiceAtIndexes(new[] { 1, 5, 2 }, new[] { 1, 2, 3 });

      var expectedResult = new DiceRollDistinct(new[] { 6, 1, 5, 2, 6 });
      Assert.Equal(expectedResult, game.CurrentRoll);
    }

    [Fact]
    public void ApplyRerollToDiceAtIndexes_Fails_InIdleState()
    {
      Assert.Throws<InvalidOperationException>(() => game.ApplyRerollToDiceAtIndexes(new[] { 6 }, new[] { 1 }));
    }

    [Fact]
    public void ApplyRerollToDiceAtIndexes_Fails_WhenRerollAttemptedThirdTime()
    {
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 6, 1, 2, 2, 6 }));
      game.ApplyRerollToDiceAtIndexes(new[] { 1, 2, 2 }, new[] { 1, 2, 3 });
      game.ApplyRerollToDiceAtIndexes(new[] { 1, 5, 2 }, new[] { 1, 2, 3 });

      Assert.Throws<InvalidOperationException>(() => game.ApplyRerollToDiceAtIndexes(new[] { 1, 5, 2 }, new[] { 1, 2, 3 }));
    }

    [Fact]
    public void ScoreCombination_Fails_WhenCombinationIsNotAllowed()
    {
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 6, 1, 2, 2, 6 }));

      // Pair is not allowed since it's only start of the game and only grades are allowed
      Assert.False(game.AllowedCombinationTypes.HasFlag(CombinationTypes.Pair));
      Assert.Throws<InvalidOperationException>(() => game.ScoreCombination(CombinationTypes.Pair));
    }

    [Fact]
    public void ScoreCombination_Fails_WhenCombinationDoesNotFitRoll()
    {
      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 6 }));

      Assert.True(game.AllowedCombinationTypes.HasFlag(CombinationTypes.Pair));
      Assert.Throws<InvalidOperationException>(() => game.ScoreCombination(CombinationTypes.Pair));
    }

    [Fact]
    public void ScoreCombination_AddsDoubleScore_WhenNonGradeCombinationScoredOnFirstRoll()
    {
      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 6, 6 }));

      int initialScore = game.Score;

      game.ScoreCombination(CombinationTypes.Pair);

      Assert.Equal(24, game.Score - initialScore);
    }

    [Fact]
    public void ScoreCombination_AddsSingleScore_WhenGradeCombinationScoredOnFirstRoll()
    {
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 6, 6 }));

      int initialScore = game.Score;

      game.ScoreCombination(CombinationTypes.Grade1);

      Assert.Equal(-2, game.Score - initialScore);
    }

    [Fact]
    public void ScoreCombination_AddsSingleScore_WhenCombinationScoredNotOnFirstRoll()
    {
      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 2, 4, 6 }));

      int initialScore = game.Score;

      game.ApplyRerollToDiceAtIndexes(new[] { 1, 2, 3, 6 }, new[] { 0, 1, 2, 3 });

      game.ScoreCombination(CombinationTypes.Pair);

      Assert.Equal(12, game.Score - initialScore);
    }

    [Fact]
    public void ScoreCombination_RemovesScoredCombinationFromAllowed()
    {
      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 2, 4, 6 }));

      game.ApplyRerollToDiceAtIndexes(new[] { 1, 2, 3, 6 }, new[] { 0, 1, 2, 3 });

      Assert.True(game.AllowedCombinationTypes.HasFlag(CombinationTypes.Pair));

      game.ScoreCombination(CombinationTypes.Pair);

      Assert.False(game.AllowedCombinationTypes.HasFlag(CombinationTypes.Pair));
    }

    [Fact]
    public void ScoreCombination_ChangesGameStateToIdle()
    {
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 1, 1 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);

      game.ScoreCombination(CombinationTypes.Grade1);

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void CrossOutCombination_ChangesGameStateToIdle()
    {
      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 1, 2 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);

      game.CrossOutCombination(CombinationTypes.Poker);

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void ScoreCombination_ChangesGameStateToGameOver_WhenNoCombinationsLeft()
    {
      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 1, 1 }));
      game.ScoreCombination(CombinationTypes.Grade1);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 2, 2, 2, 2, 2 }));
      game.ScoreCombination(CombinationTypes.Grade2);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 3, 3, 3, 3, 3 }));
      game.ScoreCombination(CombinationTypes.Grade3);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 6, 6 }));
      game.ScoreCombination(CombinationTypes.Pair);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 6, 6 }));
      game.ScoreCombination(CombinationTypes.Set);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 5, 5, 6, 6 }));
      game.ScoreCombination(CombinationTypes.TwoPairs);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 6, 6 }));
      game.ScoreCombination(CombinationTypes.Full);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 6, 6, 6, 6 }));
      game.ScoreCombination(CombinationTypes.Care);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 5 }));
      game.ScoreCombination(CombinationTypes.LittleStraight);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 2, 3, 4, 5, 6 }));
      game.ScoreCombination(CombinationTypes.BigStraight);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 3, 3, 3, 3, 3 }));
      game.ScoreCombination(CombinationTypes.Poker);
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 3, 4, 5, 3, 6 }));
      game.ScoreCombination(CombinationTypes.Trash);

      Assert.Equal(GameOfSchoolState.GameOver, game.State);
    }

    [Fact]
    public void AllowedCombinations_ContainsOnlyGrades_UnlessThreeGradesScored()
    {
      Assert.Equal(CombinationTypes.School, game.AllowedCombinationTypes);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 1, 1 }));
      game.ScoreCombination(CombinationTypes.Grade1);

      Assert.Equal(
        (CombinationTypes)(CombinationTypes.School
          - CombinationTypes.Grade1),
        game.AllowedCombinationTypes);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 2, 2, 2, 2, 2 }));
      game.ScoreCombination(CombinationTypes.Grade2);

      Assert.Equal(
        (CombinationTypes)(CombinationTypes.School
          - CombinationTypes.Grade1
          - CombinationTypes.Grade2),
        game.AllowedCombinationTypes);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 3, 3, 3, 3, 3 }));
      game.ScoreCombination(CombinationTypes.Grade3);

      Assert.Equal(
        (CombinationTypes)(CombinationTypes.All
          - CombinationTypes.Grade1
          - CombinationTypes.Grade2
          - CombinationTypes.Grade3),
        game.AllowedCombinationTypes);
    }
  }
}