using Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace CustomizeGeyser
{
    public class GeoTunerStruct(string? geyser, string? id)
    {
        [JsonIgnore] public readonly int idHash = Hash.SDBMLower(id);
        [JsonIgnore] public readonly int geyserHash = Hash.SDBMLower(geyser);

        [JsonProperty] public readonly string? id = id;
        [JsonProperty] public readonly string? geyser = geyser;
        [JsonProperty] public string? name;

        [JsonProperty] public string? material;
        [JsonProperty] public float? quantity;
        [JsonProperty] public float? duration;

        [JsonProperty] public string? originID;
        [JsonProperty] public float? massPerCycleModifier;
        [JsonProperty] public float? temperatureModifier;
        [JsonProperty] public float? iterationDurationModifier;
        [JsonProperty] public float? iterationPercentageModifier;
        [JsonProperty] public float? yearDurationModifier;
        [JsonProperty] public float? yearPercentageModifier;
        [JsonProperty] public float? maxPressureModifier;
        [JsonProperty] public string? newElement;

        public bool IsValidForGeyser(Geyser geyser)
        {
            return geyserHash == 0 || geyserHash == geyser.configuration.typeId.hash;
        }

        public static implicit operator GeoTunerConfig.GeotunedGeyserSettings(GeoTunerStruct obj) => new()
        {
            material = obj.material.ToTagSafe(),
            quantity = Math.Max(0f, obj.quantity ?? 50f),
            duration = Math.Max(1f, obj.duration ?? 600f),
            template =
            {
                originID = obj.originID ?? "GeoTuner #cg!",
                massPerCycleModifier = obj.massPerCycleModifier ?? 0f,
                temperatureModifier = obj.temperatureModifier ?? 0f,
                iterationDurationModifier = obj.iterationDurationModifier ?? 0f,
                iterationPercentageModifier = obj.iterationPercentageModifier ?? 0f,
                yearDurationModifier = obj.yearDurationModifier ?? 0f,
                yearPercentageModifier = obj.yearPercentageModifier ?? 0f,
                maxPressureModifier = obj.maxPressureModifier ?? 0f,
                modifyElement = obj.newElement.ToSimHash(0) != 0,
                newElement = obj.newElement.ToSimHash(0)
            }
        };
    }
}
