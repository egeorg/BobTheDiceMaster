using Microsoft.AspNetCore.Mvc;
using BobTheDiceMaster;
using BobTheDiceMaster.Decisions;

namespace BobTheDiceMasterAwsLambdaApi.Controllers
{
  [Route("[controller]")]
  public class BobController : ControllerBase
  {
    // GET /bob
    [HttpGet]
    public IActionResult Get()
    {
      return Ok("BobTheDiceMaster AWS Lambda API endpoint");
    }

    // POST /bob/{bobVersion}
    [HttpPost("{bobVersion}")]
    public DecisionWrapper Post(BobSelector bobVersion, [FromBody] GameOfSchoolContext gameContext)
    {
      IPlayer bob = bobVersion switch
      {
        BobSelector.Verbose => new VerboseBruteForceBob(),
        BobSelector.Parallel => new ParallelVerboseBruteForceBob(),
        BobSelector.Recursive => new RecursiveBruteForceBob(),
        _ => new RecursiveBruteForceBob(),
      };

      Decision decision = bob.DecideOnRoll(
        gameContext.AvailableCombinations, gameContext.DiceRoll, gameContext.RollsLeft);
      return new DecisionWrapper { Decision = decision };
    }

    // POST /bob
    [HttpPost]
    public DecisionWrapper Post([FromBody] GameOfSchoolContext gameContext)
    {
      IPlayer aiPlayer = new RecursiveBruteForceBob();
      Decision decision = aiPlayer.DecideOnRoll(
        gameContext.AvailableCombinations, gameContext.DiceRoll, gameContext.RollsLeft);
      return new DecisionWrapper { Decision = decision };
    }
  }
}