using Microsoft.AspNetCore.Mvc;
using BobTheDiceMaster.Api.Model;

namespace BobTheDiceMaster.Api.Controllers
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
    [HttpPost("{bobVersion}", Name = nameof(DecideOnRoll))]
    [Produces("application/json")]
    public ActionResult<Model.Decision> DecideOnRoll(BobSelector bobVersion, [FromBody] GameOfSchoolContext gameContext)
    {
      IPlayer bob = bobVersion switch
      {
        BobSelector.Verbose => new VerboseBruteForceBob(),
        BobSelector.Parallel => new ParallelVerboseBruteForceBob(),
        BobSelector.Recursive => new RecursiveBruteForceBob(),
        BobSelector.Precomputed => new PrecomputedBob(),
        _ => new RecursiveBruteForceBob(),
      };

      Decision decision = bob.DecideOnRoll(
        gameContext.AvailableCombinations, new DiceRoll(gameContext.DiceRoll), gameContext.RerollsLeft);

      return new Model.Decision(decision);
    }

    // POST /bob
    [HttpPost(Name = nameof(DefaultBobDecideOnRoll))]
    [Produces("application/json")]
    public ActionResult<Model.Decision> DefaultBobDecideOnRoll([FromBody] GameOfSchoolContext gameContext)
    {
      IPlayer aiPlayer = new RecursiveBruteForceBob();
      Decision decision = aiPlayer.DecideOnRoll(
        gameContext.AvailableCombinations, new DiceRoll(gameContext.DiceRoll), gameContext.RerollsLeft);
      return new Model.Decision(decision);
    }
  }
}