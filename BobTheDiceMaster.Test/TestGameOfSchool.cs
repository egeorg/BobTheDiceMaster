using Moq;

namespace BobTheDiceMaster.Test
{
  public class TestGameOfSchool
  {
    [Fact]
    public void AllowedCombinations_AreOnlyAllGrades_InTheBeginningOfTheGame()
    {
      var game = new GameOfSchool();

      CombinationTypes[] expectedInitialAllowedCombinationTypes =
        new[] {
          CombinationTypes.Grade1,
          CombinationTypes.Grade2,
          CombinationTypes.Grade3,
          CombinationTypes.Grade4,
          CombinationTypes.Grade5,
          CombinationTypes.Grade6,
        };

      Assert.Equal(expectedInitialAllowedCombinationTypes, game.AllowedCombinationTypes);
    }

    [Fact]
    public void State_IsIdle_InTheBeginningOfTheGame()
    {
      var game = new GameOfSchool();

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void Score_Is0_InTheBeginningOfTheGame()
    {
      var game = new GameOfSchool();

      Assert.Equal(0, game.Score);
    }

    private void ScoreGrades456(GameOfSchool game)
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
      var game = new GameOfSchool();

      ScoreGrades456(game);

      game.Reset();

      CombinationTypes[] expectedInitialAllowedCombinationTypes =
        new[] {
          CombinationTypes.Grade1,
          CombinationTypes.Grade2,
          CombinationTypes.Grade3,
          CombinationTypes.Grade4,
          CombinationTypes.Grade5,
          CombinationTypes.Grade6,
        };

      Assert.Equal(expectedInitialAllowedCombinationTypes, game.AllowedCombinationTypes);
    }

    [Fact]
    public void State_IsIdle_AfterReset()
    {
      var game = new GameOfSchool();

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 5 }));

      game.Reset();

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void Score_Is0_AfterReset()
    {
      var game = new GameOfSchool();

      ScoreGrades456(game);

      game.Reset();

      Assert.Equal(0, game.Score);
    }

    [Fact]
    public void SetRoll_Fails_InRolledState()
    {
      var game = new GameOfSchool();

      int[] rollResult = new[] { 1, 2, 3, 4, 5 };

      game.SetCurrentRoll(new DiceRollDistinct(rollResult));

      Assert.Throws<InvalidOperationException>(() => game.SetCurrentRoll(new DiceRollDistinct(rollResult)));
    }

    [Fact]
    public void SetRoll_ChangesStateToRolled()
    {
      var game = new GameOfSchool();

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 5 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);
    }

    [Fact]
    public void SetRoll_SetsCurrentRollToItsArgument()
    {
      var game = new GameOfSchool();

      var newRoll = new DiceRollDistinct(new[] { 6, 2, 2, 1, 6 });

      game.SetCurrentRoll(newRoll);

      Assert.Equal(newRoll, game.CurrentRoll);
    }

    [Fact]
    public void SetRoll_SetsRerollsLeftTo2()
    {
      var game = new GameOfSchool();

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 6, 2, 2, 1, 6 }));

      Assert.Equal(2, game.RerollsLeft);
    }

    [Fact]
    public void ApplyReroll_Fails_InIdleState()
    {
      var game = new GameOfSchool();

      Assert.Throws<InvalidOperationException>(() => game.ApplyRerollToDiceAtIndexes(new[] { 1, 5, 2 }, new[] { 1, 2, 3 }));
    }

    [Fact]
    public void ApplyReroll_YieldsCorrectResult_InRolledState()
    {
      var diceMock = TestHelper.GetDiceMock();

      var game = new GameOfSchool();
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 6, 1, 2, 2, 6 }));
      game.ApplyRerollToDiceAtIndexes(new[] { 1, 5, 2 }, new[] { 1, 2, 3 });

      var expectedResult = new DiceRollDistinct(new[] { 6, 1, 5, 2, 6 });
      Assert.Equal(expectedResult, game.CurrentRoll);
    }

    [Fact]
    public void ApplyDecision_Fails_InIdleState()
    {
      var game = new GameOfSchool();

      Assert.Throws<InvalidOperationException>(() => game.ApplyRerollToDiceAtIndexes(new[] { 1, 5, 2 }, new[] { 1, 2, 3 }));
    }

    [Fact]
    public void ApplyDecisionReroll_Fails_WhenRerollAttemptedThirdTime()
    {
      var game = new GameOfSchool();

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 6, 1, 2, 2, 6 }));
      game.ApplyRerollToDiceAtIndexes(new[] { 1, 5, 2 }, new[] { 1, 2, 3 });
      game.ApplyRerollToDiceAtIndexes(new[] { 1, 5, 2 }, new[] { 1, 2, 3 });

      Assert.Throws<InvalidOperationException>(() => game.ApplyRerollToDiceAtIndexes(new[] { 1, 5, 2 }, new[] { 1, 2, 3 }));
    }

    [Fact]
    public void ApplyDecisionScore_Fails_WhenCombinationIsNotAllowed()
    {
      var game = new GameOfSchool();

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 6, 1, 2, 2, 6 }));

      // Pair is not allowed since it's only start of the game and only grades are allowed
      Assert.DoesNotContain(CombinationTypes.Pair, game.AllowedCombinationTypes);
      Assert.Throws<InvalidOperationException>(() => game.ScoreCombination(CombinationTypes.Pair));
    }

    [Fact]
    public void ApplyDecisionScore_Fails_WhenCombinationDoesNotFitRoll()
    {
      var game = new GameOfSchool();

      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 6 }));

      Assert.Contains(CombinationTypes.Pair, game.AllowedCombinationTypes);
      Assert.DoesNotContain(CombinationTypes.Pair, game.ScoreCombinationTypes);
      Assert.Throws<InvalidOperationException>(() => game.ScoreCombination(CombinationTypes.Pair));
    }

    [Fact]
    public void ApplyDecisionScore_AddsDoubleScore_WhenNonGradeCombinationScoredOnFirstRoll()
    {
      var game = new GameOfSchool();

      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 6, 6 }));

      int initialScore = game.Score;

      game.ScoreCombination(CombinationTypes.Pair);

      Assert.Equal(24, game.Score - initialScore);
    }

    [Fact]
    public void ApplyDecisionScore_AddsSingleScore_WhenGradeCombinationScoredOnFirstRoll()
    {
      var game = new GameOfSchool();
      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 6, 6 }));

      int initialScore = game.Score;

      game.ScoreCombination(CombinationTypes.Grade1);

      Assert.Equal(-2, game.Score - initialScore);
    }

    [Fact]
    public void ApplyDecisionScore_AddsSingleScore_WhenCombinationScoredNotOnFirstRoll()
    {
      var game = new GameOfSchool();

      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 2, 4, 6 }));

      int initialScore = game.Score;

      game.ApplyRerollToDiceAtIndexes(new[] { 1, 2, 3, 6 }, new[] { 0, 1, 2, 3 });

      game.ScoreCombination(CombinationTypes.Pair);

      Assert.Equal(12, game.Score - initialScore);
    }

    [Fact]
    public void ApplyDecisionScore_RemovesScoredCombinationFromAllowed()
    {
      var game = new GameOfSchool();

      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 2, 4, 6 }));

      game.ApplyRerollToDiceAtIndexes(new[] { 1, 2, 3, 6 }, new[] { 0, 1, 2, 3 });

      Assert.Contains(CombinationTypes.Pair, game.AllowedCombinationTypes);

      game.ScoreCombination(CombinationTypes.Pair);

      Assert.DoesNotContain(CombinationTypes.Pair, game.AllowedCombinationTypes);
    }

    [Fact]
    public void ApplyDecisionScore_ChangesGameStateToIdle()
    {
      var game = new GameOfSchool();

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 1, 1 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);

      game.ScoreCombination(CombinationTypes.Grade1);

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void ApplyDecisionCrossOut_ChangesGameStateToIdle()
    {
      var game = new GameOfSchool();

      ScoreGrades456(game);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 2, 3, 1, 2 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);

      game.CrossOutCombination(CombinationTypes.Poker);

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void ApplyDecision_ChangesGameStateToGameOverWhenNoCombinationsLeft()
    {
      var game = new GameOfSchool();

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
      var game = new GameOfSchool();

      Assert.Equal(new[]
        {
          CombinationTypes.Grade1,
          CombinationTypes.Grade2,
          CombinationTypes.Grade3,
          CombinationTypes.Grade4,
          CombinationTypes.Grade5,
          CombinationTypes.Grade6
        },
        game.AllowedCombinationTypes);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 1, 1, 1, 1, 1 }));
      game.ScoreCombination(CombinationTypes.Grade1);

      Assert.Equal(new[]
        {
          CombinationTypes.Grade2,
          CombinationTypes.Grade3,
          CombinationTypes.Grade4,
          CombinationTypes.Grade5,
          CombinationTypes.Grade6
        },
        game.AllowedCombinationTypes);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 2, 2, 2, 2, 2 }));
      game.ScoreCombination(CombinationTypes.Grade2);

      Assert.Equal(new[]
        {
          CombinationTypes.Grade3,
          CombinationTypes.Grade4,
          CombinationTypes.Grade5,
          CombinationTypes.Grade6
        },
        game.AllowedCombinationTypes);

      game.SetCurrentRoll(new DiceRollDistinct(new[] { 3, 3, 3, 3, 3 }));
      game.ScoreCombination(CombinationTypes.Grade3);

      Assert.Equal(new[]
        {
          CombinationTypes.Grade4,
          CombinationTypes.Grade5,
          CombinationTypes.Grade6,
          CombinationTypes.Pair,
          CombinationTypes.Set,
          CombinationTypes.TwoPairs,
          CombinationTypes.Full,
          CombinationTypes.Care,
          CombinationTypes.LittleStraight,
          CombinationTypes.BigStraight,
          CombinationTypes.Poker,
          CombinationTypes.Trash,
        },
        game.AllowedCombinationTypes);
    }
  }
}