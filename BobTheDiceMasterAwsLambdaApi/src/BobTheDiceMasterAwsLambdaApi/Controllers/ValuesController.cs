using Microsoft.AspNetCore.Mvc;
using BobTheDiceMaster;
using BobTheDiceMaster.Decisions;

namespace BobTheDiceMasterAwsLambdaApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public SortedSet<int> Get()
        {
            return new SortedSet<int>() { 1, 11, 9, 5, 13 };
        }

        // POST api/values
        [HttpPost]
        public DecisionWrapper Post([FromBody] GameOfSchoolContext gameContext)
        {
            IPlayer aiPlayer = new VerboseBruteForceBob();
            Decision decision = aiPlayer.DecideOnRoll(
                gameContext.AvailableCombinations, gameContext.DiceRoll, gameContext.RollsLeft);
            decision.SortedInts = new SortedSet<int>() { 1, 11, 9, 5, 13 };
            return new DecisionWrapper { Decision = decision };
        }
    }
}