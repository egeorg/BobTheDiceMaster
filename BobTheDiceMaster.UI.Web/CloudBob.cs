using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BobTheDiceMaster.UI.Web
{
  public class CloudBob : IPlayer
  {
    private readonly string bobApiBaseAddress;

    public CloudBob(string bobApiBaseAddress)
    {
      this.bobApiBaseAddress = bobApiBaseAddress;
    }

    public async Task<Decision> DecideOnRollAsync(CombinationTypes availableCombinations, DiceRoll currentRoll, int rerollsLeft)
    {
      DateTime startRequestTime = DateTime.UtcNow;
      using HttpClient httpClient = new HttpClient();
      var bobClient = new Api.SwaggerClient.BobApiClient(bobApiBaseAddress, httpClient);

      var gameContext = new Api.SwaggerClient.GameOfSchoolContext();
      gameContext.AvailableCombinations = (Api.SwaggerClient.CombinationTypes)availableCombinations;
      gameContext.DiceRoll = Enumerable.Range(0, currentRoll.DiceAmount)
          .Select(n => currentRoll[n]).ToArray();
      gameContext.RerollsLeft = rerollsLeft;

      Api.SwaggerClient.Decision apiDecision = await bobClient.DefaultBobDecideOnRollAsync(gameContext);

      return ConvertSwaggerToCore(apiDecision);
    }


    private Decision ConvertSwaggerToCore(Api.SwaggerClient.Decision apiDecision)
    {
      if (apiDecision.Reroll != null)
      {
        return new Reroll(apiDecision.Reroll.ValuesToReroll);
      }
      else if (apiDecision.Score != null)
      {
        return new Score((CombinationTypes)apiDecision.Score.Combination);
      }
      else if (apiDecision.CrossOut != null)
      {
        return new CrossOut((CombinationTypes)apiDecision.CrossOut.Combination);
      }
      else
      {
        throw new InvalidOperationException(
          $"All {typeof(BobTheDiceMaster.Api.SwaggerClient.Decision)} fields are null");
      }
    }
  }
}
