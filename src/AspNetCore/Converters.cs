using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sequence.AspNetCore
{
    internal sealed class GameIdJsonConverter : JsonConverter<GameId>
    {
        public override GameId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetGuid(out var value))
            {
                return new GameId(value);
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, GameId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }

    internal sealed class PlayerHandleJsonConverter : JsonConverter<PlayerHandle>
    {
        public override PlayerHandle? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (value is null)
            {
                return null;
            }

            return new PlayerHandle(value);
        }

        public override void Write(Utf8JsonWriter writer, PlayerHandle value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }

    internal sealed class PlayerIdJsonConverter : JsonConverter<PlayerId>
    {
        public override PlayerId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetInt32(out var value))
            {
                return new PlayerId(value);
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, PlayerId value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Value);
        }
    }

    internal sealed class TileJsonConverter : JsonConverter<Tile>
    {
        public override Tile? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Tile value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else if (value is (Suit, Rank) tile)
            {
                writer.WriteStartArray();
                writer.WriteStringValue(tile.Suit.ToString());
                writer.WriteStringValue(tile.Rank.ToString());
                writer.WriteEndArray();
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }

    internal sealed class GameEventConverter : JsonConverter<GameEvent>
    {
        private static readonly ImmutableDictionary<Type, string> _keyByType;
        private static readonly ImmutableDictionary<Type, PropertyInfo[]> _propertiesByType;

        static GameEventConverter()
        {
            var gameEventTypes = typeof(IGameEvent).Assembly
                .ExportedTypes
                .Where(typeof(IGameEvent).IsAssignableFrom)
                .ToList();

            _keyByType = gameEventTypes
                .ToImmutableDictionary(
                    type => type,
                    type => new string(type.Name
                        .SelectMany(NameToChars)
                        .ToArray())
                        .Trim('-'));

            var bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            _propertiesByType = gameEventTypes
                .ToImmutableDictionary(
                    type => type,
                    type => type.GetProperties(bindingFlags));
        }

        private static IEnumerable<char> NameToChars(char c)
        {
            if (char.IsUpper(c))
            {
                yield return '-';
                yield return char.ToLowerInvariant(c);
            }
            else
            {
                yield return c;
            }
        }

        public override bool CanConvert(Type objectType) => typeof(IGameEvent).IsAssignableFrom(objectType);

        public override GameEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, GameEvent value, JsonSerializerOptions options)
        {
            var type = value.GetType();
            var name = _keyByType[type];
            var namingPolicy = JsonNamingPolicy.CamelCase;
            var properties = _propertiesByType[type]
                .Select(p => (p.Name, Value: p.GetValue(value)))
                .ToDictionary(
                    p => namingPolicy.ConvertName(p.Name),
                    p => p.Value);

            writer.WriteStartObject();
            writer.WriteString("kind", name);

            foreach (var property in properties)
            {
                writer.WritePropertyName(property.Key);
                JsonSerializer.Serialize(writer, property.Value, type, options);
            }

            writer.WriteEndObject();
        }
    }
}
