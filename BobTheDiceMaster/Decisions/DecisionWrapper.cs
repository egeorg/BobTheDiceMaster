using BobTheDiceMaster.JsonConverters;
using System.Text.Json.Serialization;

namespace BobTheDiceMaster.Decisions
{
    [JsonConverter(typeof(DecisionWrapperJsonConverter))]
    public class DecisionWrapper
    {
        public Decision Decision { get; set; }
    }
}
