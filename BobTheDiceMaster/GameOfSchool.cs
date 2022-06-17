using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobTheDiceMaster
{
  public class GameOfSchool
  {
    #region private fields
    private IPlayer player;
    private CombinationTypes allowedCombinationTypes;
    private bool isSchoolFinished;
    private int totalScore;
    private const int RollsPerTurn = 3;
    #endregion

    #region private methods
    private bool AreThreeGradesFinished(CombinationTypes combinationTypes)
    {
      if ((combinationTypes & ~CombinationTypes.School) != 0)
      {
        throw new ArgumentException($"Only grade combinations expected, but was {combinationTypes}");
      }
      uint gradesLeft = 0;
      uint combinationTypesUint = (uint)combinationTypes;
      while (combinationTypesUint != 0)
      {
        gradesLeft += combinationTypesUint % 2;
        combinationTypesUint = combinationTypesUint >> 1;
      }

      return gradesLeft < 3;
    }
    #endregion

    #region public properties
    public int Score => totalScore;
    #endregion

    #region public methods
    public GameOfSchool(IPlayer player)
    {
      this.player = player;
      Reset();
    }

    public void Reset()
    {
      allowedCombinationTypes = CombinationTypes.School;
      isSchoolFinished = false;
      totalScore = 0;
    }

    public void NextStep()
    {
      DiceRoll roll = DiceRoll.GenerateNew();
      int rollsLeft = RollsPerTurn;
      while (rollsLeft > 0)
      {
        IDecision decision = player.DecideOnRoll(allowedCombinationTypes, roll, rollsLeft);

        switch (decision)
        {
          case Reroll reroll:
            --rollsLeft;
            roll.Reroll(reroll.DiceToReroll);
            break;
          case Score score:
            if (rollsLeft == RollsPerTurn)
            {
              totalScore += roll.Sum() * 2;
            }
            else
            {
              totalScore += roll.Sum();
            }
            allowedCombinationTypes -= score.CombinationToScore;
            rollsLeft = 0;
            break;
          case CrossOut crossOut:
            allowedCombinationTypes -= crossOut.Combination;
            rollsLeft = 0;
            break;
        }
        if (!isSchoolFinished && AreThreeGradesFinished(allowedCombinationTypes))
        {
          allowedCombinationTypes |= CombinationTypes.AllButSchool;
        }
      }
    }
    #endregion
  }
}
