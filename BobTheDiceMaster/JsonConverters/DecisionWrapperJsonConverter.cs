using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using BobTheDiceMaster.Decisions;

namespace BobTheDiceMaster.JsonConverters
{
  internal class DecisionWrapperJsonConverter : JsonConverter<DecisionWrapper>
  {
    enum TypeDiscriminator
    {
      None = 0,
      Reroll = 1,
      Score = 2,
      CrossOut = 3
    }

    public override void Write(Utf8JsonWriter writer, DecisionWrapper value, JsonSerializerOptions options)
    {
      writer.WriteStartObject();

      if (value.Decision is Reroll reroll)
      {
        writer.WriteNumber("typeDiscriminator", (int)TypeDiscriminator.Reroll);
        writer.WritePropertyName("decisionTypeValue");
        JsonSerializer.Serialize(writer, reroll, options);
      }
      else if (value.Decision is Score score)
      {
        writer.WriteNumber("typeDiscriminator", (int)TypeDiscriminator.Score);
        writer.WritePropertyName("decisionTypeValue");
        JsonSerializer.Serialize(writer, score, options);
      }
      else if (value.Decision is CrossOut crossOut)
      {
        writer.WriteNumber("typeDiscriminator", (int)TypeDiscriminator.CrossOut);
        writer.WritePropertyName("decisionTypeValue");
        JsonSerializer.Serialize(writer, crossOut, options);
      }

      writer.WriteEndObject();
    }

    public override DecisionWrapper Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType != JsonTokenType.StartObject)
      {
        throw new JsonException();
      }

      if (!reader.Read()
              || reader.TokenType != JsonTokenType.PropertyName
              || reader.GetString() != "typeDiscriminator")
      {
        throw new JsonException();
      }
      if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
      {
        throw new JsonException();
      }

      TypeDiscriminator typeDiscriminator = (TypeDiscriminator)reader.GetInt32();

      if (!reader.Read() || reader.GetString() != "decisionTypeValue")
      {
        throw new JsonException();
      }
      //if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
      //{
      //    throw new JsonException();
      //}

      Decision decision;

      JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions(options);
      jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

      switch (typeDiscriminator)
      {
        case TypeDiscriminator.Reroll:
          decision = (Reroll)JsonSerializer.Deserialize(
              ref reader, typeof(Reroll), jsonSerializerOptions);
          break;
        case TypeDiscriminator.Score:
          decision = (Score)JsonSerializer.Deserialize(
              ref reader, typeof(Score), jsonSerializerOptions);
          break;
        case TypeDiscriminator.CrossOut:
          decision = (CrossOut)JsonSerializer.Deserialize(
              ref reader, typeof(CrossOut), jsonSerializerOptions);
          break;
        default:
          throw new JsonException();
      }

      if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
      {
        throw new JsonException();
      }

      return new DecisionWrapper() { Decision = decision };
    }
  }
}
