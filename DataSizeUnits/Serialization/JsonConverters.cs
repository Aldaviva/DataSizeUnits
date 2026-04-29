using Newtonsoft.Json;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
#if !NET5_0_OR_GREATER
using System.Buffers;
#endif

namespace DataSizeUnits.Serialization;

/// <summary>Serialize and deserialize <see cref="DataSize"/> as a number of bits in JSON.</summary>
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
#if NET5_0_OR_GREATER
                    rawString = Utf8.GetString(reader.ValueSequence);
#else
                    rawString = Utf8.GetString(reader.ValueSequence.ToArray());
#endif
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

            // Backward compatibility with DataSizeUnits ≤ 3
            case JsonTokenType.StartObject:
                DataSizeUnit unit           = DataSizeUnit.Byte;
                ulong?       quantityUlong  = null;
                long?        quantityLong   = null;
                double       quantityDouble = 0;

                reader.Read();
                while (reader.TokenType != JsonTokenType.EndObject) {
                    string? propertyName = reader.GetString();

                    reader.Read();
                    switch (propertyName?.ToLowerInvariant()) {
                        case "quantity":
                            if (reader.TryGetUInt64(out ulong q1)) {
                                quantityUlong = q1;
                            } else if (reader.TryGetInt64(out long q2)) {
                                quantityLong = q2;
                            } else {
                                quantityDouble = reader.GetDouble();
                            }
                            break;
                        case "unit":
                            string unitString = reader.GetString() ?? string.Empty;
                            unit =
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                Enum.Parse<DataSizeUnit>(unitString, true);
#else
                                (DataSizeUnit) Enum.Parse(typeof(DataSizeUnit), unitString, true);
#endif
                            break;
                    }
                    reader.Read();
                }
                if (quantityUlong.HasValue) {
                    return new DataSize(quantityUlong.Value, unit);
                } else if (quantityLong.HasValue) {
                    return new DataSize(quantityLong.Value, unit);
                } else {
                    return new DataSize(quantityDouble, unit);
                }
            default:
                throw new JsonException($"DataSize must be encoded in JSON as a number of bits or a string, not {reader.TokenType}");
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

/// <summary>
/// <para>A <see cref="Newtonsoft.Json.JsonConverter"/> that can be used to serialize and deserialize <see cref="DataSize"/> instances using <c>Newtonsoft.Json</c>.</para>
/// <para>You can use this by adding <see cref="Instance"/> to <see cref="Newtonsoft.Json.JsonSerializerSettings.Converters"/>. By default, it is automatically registered with <see cref="Newtonsoft.Json.JsonConvert.DefaultSettings"/>, so it will take effect on all usages of <see cref="Newtonsoft.Json.JsonConvert"/> and <see cref="JsonSerializer.CreateDefault()"/>, but not <see cref="JsonSerializer.Create()"/> or <see cref="Newtonsoft.Json.JsonSerializer()"/>.</para>
/// </summary>
public sealed class DataSizeNewtonsoftJsonConverter: Newtonsoft.Json.JsonConverter<DataSize> {

    /// <summary>Public shared singleton instance of this class, if you want to register it in a non-default <see cref="Newtonsoft.Json.JsonSerializerSettings"/>.</summary>
    public static readonly DataSizeNewtonsoftJsonConverter Instance = new();

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, DataSize value, JsonSerializer serializer) {
        writer.WriteRawValue(value.Bits.ToString("R", CultureInfo.InvariantCulture));
    }

}

internal static class DataSizeNewtonsoftJsonConverterRegistrar {

    private static int _registered;

    /// <exception cref="FileNotFoundException"><c>Newtonsoft.Json</c> dependency is not on the assembly load path</exception>
    public static void Register() {
        if (0 == Interlocked.Exchange(ref _registered, 1)) {
            Func<JsonSerializerSettings>? oldDefaultSettings = JsonConvert.DefaultSettings;
            if (oldDefaultSettings != null) {
                JsonConvert.DefaultSettings = () => {
                    JsonSerializerSettings defaultSettings = oldDefaultSettings();
                    if (!defaultSettings.Converters.OfType<DataSizeNewtonsoftJsonConverter>().Any()) {
                        defaultSettings.Converters.Add(DataSizeNewtonsoftJsonConverter.Instance);
                    }
                    return defaultSettings;
                };
            } else {
                JsonSerializerSettings defaultSettings = new() { Converters = { DataSizeNewtonsoftJsonConverter.Instance } };
                JsonConvert.DefaultSettings = () => defaultSettings;
            }
        }
    }

}