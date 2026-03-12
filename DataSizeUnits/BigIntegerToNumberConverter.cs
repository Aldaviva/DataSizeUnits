using Newtonsoft.Json;
using System.Buffers;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace DataSizeUnits;

internal sealed class DataSizeJsonConverter: System.Text.Json.Serialization.JsonConverter<DataSize> {

    private static readonly Encoding Utf8 = Encoding.UTF8;

    /// <exception cref="System.Text.Json.JsonException">Unsupported token type</exception>
    public override DataSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        string?            rawString = null;
        ReadOnlySpan<char> rawSpan   = default;

        switch (reader.TokenType) {
            case JsonTokenType.Number:
                if (reader.HasValueSequence) {
                    rawString = Utf8.GetString(reader.ValueSequence.ToArray());
                } else {
#if NET7_0_OR_GREATER
                    int    charCount = Utf8.GetCharCount(reader.ValueSpan);
                    char[] chars     = new char[charCount];
                    charCount = Utf8.GetChars(reader.ValueSpan, chars);
                    rawSpan   = chars.AsSpan(..charCount);
#else
                    rawString = Utf8.GetString(reader.ValueSpan.ToArray());
#endif
                }
                break;
            case JsonTokenType.String:
                rawString = reader.GetString()!;
                break;
            default:
                throw new JsonException($"DataSize must be encoded in JSON as a number or a string, not {reader.TokenType}");
        }

        BigInteger bits;
        try {
            bits =
#if NET7_0_OR_GREATER
                rawSpan != default ?
                    BigInteger.Parse(rawSpan, CultureInfo.InvariantCulture) :
#endif
                    BigInteger.Parse(rawString!, CultureInfo.InvariantCulture);
        } catch (FormatException e) {
            throw new JsonException($"Failed to parse \"{rawString ?? rawSpan.ToString()}\" as a {nameof(BigInteger)}", e);
        }

        return new DataSize(bits);
    }

    public override void Write(Utf8JsonWriter writer, DataSize value, JsonSerializerOptions options) {
        writer.WriteRawValue(value.Bits.ToString("R"));
    }

}

internal sealed class DataSizeNewtonsoftJsonConverter: Newtonsoft.Json.JsonConverter<DataSize> {

    /// <exception cref="Newtonsoft.Json.JsonException">Unsupported token type</exception>
    public override DataSize ReadJson(JsonReader reader, Type objectType, DataSize existingValue, bool hasExistingValue, JsonSerializer serializer) {
        object? readerValue = reader.Value;
        BigInteger bigInteger = readerValue switch {
            string s => BigInteger.Parse(s, CultureInfo.InvariantCulture),
            long l   => l,
            _        => throw new Newtonsoft.Json.JsonException($"Failed to parse {readerValue?.GetType().Name ?? "null"} \"{readerValue}\" as a {nameof(BigInteger)}")
        };
        return new DataSize(bigInteger);
    }

    public override void WriteJson(JsonWriter writer, DataSize value, JsonSerializer serializer) {
        writer.WriteRawValue(value.Bits.ToString("R", CultureInfo.InvariantCulture));
    }

}