using Microsoft.AspNetCore.Mvc;
using BobTheDiceMaster;

namespace BobTheDiceMasterAwsLambdaApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("This is a VerboseBruteForceBob endpoint.");
        }

        // POST api/values
        [HttpPost]
        public Decision Post([FromBody] GameOfSchoolContext gameContext)
        {
            IPlayer aiPlayer = new VerboseBruteForceBob();
            Decision decision = aiPlayer.DecideOnRoll(
                gameContext.AvailableCombinations, gameContext.DiceRoll, gameContext.RollsLeft);
            return decision;
        }
    }
}