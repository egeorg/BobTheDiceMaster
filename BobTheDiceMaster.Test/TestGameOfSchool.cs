using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace BobTheDiceMaster.Test
{
  public class TestGameOfSchool
  {
    private Mock<IPlayer> defaultPlayerMock;

    private Mock<IDie> defaultDieMock;

    public TestGameOfSchool()
    {
      defaultPlayerMock = new Mock<IPlayer>();
      defaultDieMock = TestHelper.GetDiceMock(new int[] { });
    }

    [Fact]
    public void AllowedCombinations_AreOnlyAllGrades_InTheBeginningOfTheGame()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

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
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void Score_Is0_InTheBeginningOfTheGame()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      Assert.Equal(0, game.Score);
    }

    private void ScoreGrades456(GameOfSchool game)
    {
      game.SetRoll(new DiceRollDistinct(new[] { 1, 6, 6, 6, 6 }));
      game.ApplyDecision(new Score(CombinationTypes.Grade6));
      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 5, 5, 5 }));
      game.ApplyDecision(new Score(CombinationTypes.Grade5));
      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 1, 4, 4 }));
      game.ApplyDecision(new Score(CombinationTypes.Grade4));
    }

    [Fact]
    public void AllowedCombinations_AreOnlyAllGrades_AfterReset()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

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
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 5 }));

      game.Reset();

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void Score_Is0_AfterReset()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      ScoreGrades456(game);

      game.Reset();

      Assert.Equal(0, game.Score);
    }

    [Fact]
    public void GenerateRoll_Fails_InRolledState()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 5 }));

      Assert.Throws<InvalidOperationException>(() => game.GenerateRoll());
    }

    [Fact]
    public void GenerateRoll_ChangesStateToRolled()
    {
      Mock<IDie> dieMock = TestHelper.GetDiceMock(new[] { 1, 2, 3, 4, 5 });

      var game = new GameOfSchool(defaultPlayerMock.Object, dieMock.Object);

      game.GenerateRoll();

      Assert.Equal(GameOfSchoolState.Rolled, game.State);
    }

    [Fact]
    public void GenerateRoll_SetsCurrentRollToGeneratedByDice()
    {
      // Dice order is not ascending to ensure that order is persisted.
      int[] diceValues = new[] { 6, 2, 2, 1, 6 };

      Mock<IDie> dieMock = TestHelper.GetDiceMock(diceValues);

      var game = new GameOfSchool(defaultPlayerMock.Object, dieMock.Object);

      DiceRollDistinct generatedRoll = game.GenerateRoll();

      var expectedRoll = new DiceRollDistinct(diceValues);

      Assert.Equal(expectedRoll, generatedRoll);

      Assert.Equal(expectedRoll, game.CurrentRoll);
    }

    [Fact]
    public void GenerateRoll_SetsRerollsLeftTo2()
    {
      int[] diceValues = new[] { 6, 2, 2, 1, 6 };

      Mock<IDie> dieMock = TestHelper.GetDiceMock(diceValues);

      var game = new GameOfSchool(defaultPlayerMock.Object, dieMock.Object);

      game.GenerateRoll();

      Assert.Equal(2, game.RerollsLeft);
    }

    [Fact]
    public void SetRoll_Fails_InRolledState()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      int[] rollResult = new[] { 1, 2, 3, 4, 5 };

      game.SetRoll(new DiceRollDistinct(rollResult));

      Assert.Throws<InvalidOperationException>(() => game.SetRoll(new DiceRollDistinct(rollResult)));
    }

    [Fact]
    public void SetRoll_ChangesStateToRolled()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 5 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);
    }

    [Fact]
    public void SetRoll_SetsCurrentRollToItsArgument()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      var newRoll = new DiceRollDistinct(new[] { 6, 2, 2, 1, 6 });

      game.SetRoll(newRoll);

      Assert.Equal(newRoll, game.CurrentRoll);
    }

    [Fact]
    public void SetRoll_SetsRerollsLeftTo2()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      game.SetRoll(new DiceRollDistinct(new[] { 6, 2, 2, 1, 6 }));

      Assert.Equal(2, game.RerollsLeft);
    }

    [Fact]
    public void GenerateAndApplyReroll_FailsInIdleState()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      Assert.Throws<InvalidOperationException>(() => game.GenerateAndApplyReroll());
    }

    [Fact]
    public void GenerateAndApplyReroll_YieldsCorrectResult_InRolledState()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 6, 5, 6 });

      var game = new GameOfSchool(defaultPlayerMock.Object, diceMock.Object);
      game.GenerateRoll();
      game.ApplyDecision(new Reroll(new[] { 1, 2, 2 }));
      game.GenerateAndApplyReroll();

      var expectedResult = new DiceRollDistinct(new[] { 6, 6, 5, 6, 6 });
      Assert.Equal(expectedResult, game.CurrentRoll);
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceValuesArgument_FailsInIdleState()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      Assert.Throws<InvalidOperationException>(() => game.GenerateAndApplyReroll(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceValuesArgument_YieldsCorrectResult_InRolledState()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });

      var game = new GameOfSchool(defaultPlayerMock.Object, diceMock.Object);
      game.GenerateRoll();
      game.GenerateAndApplyReroll(new[] { 1, 2, 3 });

      var expectedResult = new DiceRollDistinct(new[] { 6, 1, 5, 2, 6 });
      Assert.Equal(expectedResult, game.CurrentRoll);
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceValuesArgument_DecrementsRerollsLeft()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });

      var game = new GameOfSchool(defaultPlayerMock.Object, diceMock.Object);
      game.GenerateRoll();
      int rerollsLeftBeforeReroll = game.RerollsLeft;
      game.GenerateAndApplyReroll(new[] { 1, 2, 3 });

      Assert.Equal(rerollsLeftBeforeReroll - 1, game.RerollsLeft);
    }

    [Fact]
    public void GenerateAndApplyRerollWithDiceValuesArgument_FailsWhenNoRerollsLeft()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });
      var game = new GameOfSchool(defaultPlayerMock.Object, diceMock.Object);
      int rerollsLeftBeforeReroll = game.RerollsLeft;
      game.GenerateRoll();
      game.GenerateAndApplyReroll(new[] { 1, 2, 3 });
      game.GenerateAndApplyReroll(new[] { 1, 2, 3 });

      Assert.Throws<InvalidOperationException>(() => game.GenerateAndApplyReroll(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void ApplyReroll_Fails_InIdleState()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      Assert.Throws<InvalidOperationException>(() => game.ApplyReroll(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void ApplyReroll_YieldsCorrectResult_InRolledState()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 });

      var game = new GameOfSchool(defaultPlayerMock.Object, diceMock.Object);
      game.GenerateRoll();
      game.ApplyDecision(new Reroll(new[] { 1, 2, 2 }));
        
      game.ApplyReroll(new[] { 1, 5, 2 });

      var expectedResult = new DiceRollDistinct(new[] { 6, 1, 5, 2, 6 });
      Assert.Equal(expectedResult, game.CurrentRoll);
    }

    [Fact]
    public void ApplyDecision_Fails_InIdleState()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      Assert.Throws<InvalidOperationException>(() => game.ApplyDecision(new Reroll(new int[] { 1 })));
    }

    [Fact]
    public void ApplyDecisionReroll_Fails_WhenRerollAttemptedThirdTime()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });

      var game = new GameOfSchool(defaultPlayerMock.Object, diceMock.Object);

      game.GenerateRoll();
      game.ApplyDecision(new Reroll(new[] { 1, 2, 2 }));
      game.ApplyReroll(new[] { 1, 5, 2 });
      game.ApplyDecision(new Reroll(new[] { 1, 5, 2 }));
      game.ApplyReroll(new[] { 1, 5, 2 });

      Assert.Throws<InvalidOperationException>(() => game.ApplyDecision(new Reroll(new[] { 1, 5, 2 })));
    }

    [Fact]
    public void ApplyDecisionScore_Fails_WhenCombinationIsNotAllowed()
    {
      var diceMock = TestHelper.GetDiceMock(new[] { 6, 1, 2, 2, 6 }, new[] { 1, 5, 2 });

      var game = new GameOfSchool(defaultPlayerMock.Object, diceMock.Object);

      game.GenerateRoll();

      Assert.DoesNotContain(CombinationTypes.Pair, game.AllowedCombinationTypes);
      Assert.Throws<InvalidOperationException>(() => game.ApplyDecision(new Score(CombinationTypes.Pair)));
    }

    [Fact]
    public void ApplyDecisionScore_Fails_WhenCombinationDoesNotFitRoll()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      ScoreGrades456(game);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 6 }));

      Assert.Contains(CombinationTypes.Pair, game.AllowedCombinationTypes);
      Assert.DoesNotContain(CombinationTypes.Pair, game.ScoreCombinationTypes);
      Assert.Throws<InvalidOperationException>(() => game.ApplyDecision(new Score(CombinationTypes.Pair)));
    }

    [Fact]
    public void ApplyDecisionScore_AddsDoubleScore_WhenNonGradeCombinationScoredOnFirstRoll()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      ScoreGrades456(game);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 2, 3, 6, 6 }));

      int initialScore = game.Score;

      game.ApplyDecision(new Score(CombinationTypes.Pair));

      Assert.Equal(24, game.Score - initialScore);
    }

    [Fact]
    public void ApplyDecisionScore_AddsSingleScore_WhenGradeCombinationScoredOnFirstRoll()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);
      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 1, 6, 6 }));

      int initialScore = game.Score;

      game.ApplyDecision(new Score(CombinationTypes.Grade1));

      Assert.Equal(-2, game.Score - initialScore);
    }

    [Fact]
    public void ApplyDecisionScore_AddsSingleScore_WhenCombinationScoredNotOnFirstRoll()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      ScoreGrades456(game);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 2, 4, 6 }));

      int initialScore = game.Score;

      game.ApplyDecision(new Reroll(new[] { 1, 1, 2, 4 }));
      game.ApplyReroll(new[] { 1, 2, 3, 6 });

      game.ApplyDecision(new Score(CombinationTypes.Pair));

      Assert.Equal(12, game.Score - initialScore);
    }

    [Fact]
    public void ApplyDecisionScore_RemovesScoredCombinationFromAllowed()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      ScoreGrades456(game);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 2, 4, 6 }));

      game.ApplyDecision(new Reroll(new[] { 1, 1, 2, 4 }));
      game.ApplyReroll(new[] { 1, 2, 3, 6 });

      Assert.Contains(CombinationTypes.Pair, game.AllowedCombinationTypes);

      game.ApplyDecision(new Score(CombinationTypes.Pair));

      Assert.DoesNotContain(CombinationTypes.Pair, game.AllowedCombinationTypes);
    }

    [Fact]
    public void ApplyDecisionScore_ChangesGameStateToIdle()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 1, 1, 1 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);

      game.ApplyDecision(new Score(CombinationTypes.Grade1));

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void ApplyDecisionCrossOut_ChangesGameStateToIdle()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      ScoreGrades456(game);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 2, 3, 1, 2 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);

      game.ApplyDecision(new CrossOut(CombinationTypes.Poker));

      Assert.Equal(GameOfSchoolState.Idle, game.State);
    }

    [Fact]
    public void ApplyDecisionReroll_DoesNotChangeGameStateToIdle()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      ScoreGrades456(game);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 2, 3, 1, 2 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);

      game.ApplyDecision(new Reroll(new[] { 1, 1, 2, 2, 3 }));

      Assert.Equal(GameOfSchoolState.Rolled, game.State);
    }

    [Fact]
    public void ApplyDecision_ChangesGameStateToGameOverWhenNoCombinationsLeft()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

      ScoreGrades456(game);

      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 1, 1, 1 }));
      game.ApplyDecision(new Score(CombinationTypes.Grade1));
      game.SetRoll(new DiceRollDistinct(new[] { 2, 2, 2, 2, 2 }));
      game.ApplyDecision(new Score(CombinationTypes.Grade2));
      game.SetRoll(new DiceRollDistinct(new[] { 3, 3, 3, 3, 3 }));
      game.ApplyDecision(new Score(CombinationTypes.Grade3));
      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 1, 6, 6 }));
      game.ApplyDecision(new Score(CombinationTypes.Pair));
      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 1, 6, 6 }));
      game.ApplyDecision(new Score(CombinationTypes.Set));
      game.SetRoll(new DiceRollDistinct(new[] { 1, 5, 5, 6, 6 }));
      game.ApplyDecision(new Score(CombinationTypes.TwoPairs));
      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 1, 6, 6 }));
      game.ApplyDecision(new Score(CombinationTypes.Full));
      game.SetRoll(new DiceRollDistinct(new[] { 1, 6, 6, 6, 6 }));
      game.ApplyDecision(new Score(CombinationTypes.Care));
      game.SetRoll(new DiceRollDistinct(new[] { 1, 2, 3, 4, 5 }));
      game.ApplyDecision(new Score(CombinationTypes.LittleStraight));
      game.SetRoll(new DiceRollDistinct(new[] { 2, 3, 4, 5, 6 }));
      game.ApplyDecision(new Score(CombinationTypes.BigStraight));
      game.SetRoll(new DiceRollDistinct(new[] { 3, 3, 3, 3, 3 }));
      game.ApplyDecision(new Score(CombinationTypes.Poker));
      game.SetRoll(new DiceRollDistinct(new[] { 3, 4, 5, 3, 6 }));
      game.ApplyDecision(new Score(CombinationTypes.Trash));

      Assert.Equal(GameOfSchoolState.GameOver, game.State);
    }

    [Fact]
    public void AllowedCombinations_ContainsOnlyGrades_UnlessThreeGradesScored()
    {
      var game = new GameOfSchool(defaultPlayerMock.Object, defaultDieMock.Object);

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

      game.SetRoll(new DiceRollDistinct(new[] { 1, 1, 1, 1, 1 }));
      game.ApplyDecision(new Score(CombinationTypes.Grade1));

      Assert.Equal(new[]
        {
          CombinationTypes.Grade2,
          CombinationTypes.Grade3,
          CombinationTypes.Grade4,
          CombinationTypes.Grade5,
          CombinationTypes.Grade6
        },
        game.AllowedCombinationTypes);

      game.SetRoll(new DiceRollDistinct(new[] { 2, 2, 2, 2, 2 }));
      game.ApplyDecision(new Score(CombinationTypes.Grade2));

      Assert.Equal(new[]
        {
          CombinationTypes.Grade3,
          CombinationTypes.Grade4,
          CombinationTypes.Grade5,
          CombinationTypes.Grade6
        },
        game.AllowedCombinationTypes);

      game.SetRoll(new DiceRollDistinct(new[] { 3, 3, 3, 3, 3 }));
      game.ApplyDecision(new Score(CombinationTypes.Grade3));

      Assert.Equal(new[]
        {
          CombinationTypes.Grade4,
          CombinationTypes.Grade5,
          CombinationTypes.Grade6,
          CombinationTypes.Trash,
          CombinationTypes.Pair,
          CombinationTypes.TwoPairs,
          CombinationTypes.Set,
          CombinationTypes.Full,
          CombinationTypes.Care,
          CombinationTypes.LittleStraight,
          CombinationTypes.BigStraight,
          CombinationTypes.Poker,
        },
        game.AllowedCombinationTypes);
    }
  }
}