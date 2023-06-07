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
    public async Task<ActionResult<Model.Decision>> DecideOnRoll(BobSelector bobVersion, [FromBody] GameOfSchoolContext gameContext)
    {
      IPlayer bob = bobVersion switch
      {
        BobSelector.Verbose => new VerboseBruteForceBob(),
        BobSelector.Parallel => new ParallelVerboseBruteForceBob(),
        BobSelector.Recursive => new BruteForceBob(),
        BobSelector.Precomputed => new PrecomputedBob(),
        _ => new BruteForceBob(),
      };

      Decision decision = await bob.DecideOnRollAsync(
        gameContext.AvailableCombinations, new DiceRoll(gameContext.DiceRoll), gameContext.RerollsLeft);

      return new Model.Decision(decision);
    }

    // POST /bob
    [HttpPost(Name = nameof(DefaultBobDecideOnRoll))]
    [Produces("application/json")]
    public async Task<ActionResult<Model.Decision>> DefaultBobDecideOnRoll([FromBody] GameOfSchoolContext gameContext)
    {
      IPlayer aiPlayer = new BruteForceBob();
      Decision decision = await aiPlayer.DecideOnRollAsync(
        gameContext.AvailableCombinations, new DiceRoll(gameContext.DiceRoll), gameContext.RerollsLeft);
      return new Model.Decision(decision);
    }
  }
}