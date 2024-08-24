using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.JsonNS
{
    /// <summary>
    /// Serialization with System.Text.Json.
    /// </summary>
    /// <remarks>
    /// [JsonInclude] [JsonIgnore]
    /// </remarks>
    public static class JsonTool
    {
        /// <summary>
        /// Serialization settings.
        /// </summary>
        public static JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            IncludeFields = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters = { new JsonStringEnumConverter() },
        };

        /// <summary>
        /// Serialization settings with $ref.
        /// </summary>
        public static JsonSerializerOptions JsonOptionsRef = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            IncludeFields = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters = { new JsonStringEnumConverter() },
        };
    }
}