using Microsoft.AspNetCore.Mvc;
using BobTheDiceMaster;
using BobTheDiceMaster.Decisions;

namespace BobTheDiceMasterAwsLambdaApi.Controllers
{
  [Route("api/[controller]")]
  public class BobTheDiceMasterController : ControllerBase
  {
    // GET api/values
    [HttpGet]
    public IActionResult Get()
    {
      return Ok("BobTheDiceMaster AWL Lambda API endpoint");
    }

    // POST api/values
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