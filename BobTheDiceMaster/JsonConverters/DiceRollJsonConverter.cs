using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BobTheDiceMaster.JsonConverters
{
  internal class DiceRollJsonConverter : JsonConverter<DiceRoll>
  {
    public override void Write
        (Utf8JsonWriter writer, DiceRoll diceRollToConvert, JsonSerializerOptions options)
    {
      int[] diceValues = new int[diceRollToConvert.DiceAmount];
      for (int i = 0; i < diceValues.Length; ++i)
      {
        diceValues[i] = diceRollToConvert[i];
      }
      writer.WriteStringValue(JsonSerializer.Serialize(diceValues));
    }

    public override DiceRoll Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      int[] diceValues = JsonSerializer.Deserialize<int[]>(reader.GetString());
      return new DiceRoll(diceValues);
    }
  }
}
