using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace CustomizeGeyser
{
    public class GeyserStruct
    {
        private int hash;
        public string id;
        public string element;
        public string anim;
        [JsonConverter(typeof(StringEnumConverter))]
        public GeyserConfigurator.GeyserShape? shape;
        public int? width;
        public int? height;
        public float? temperature;
        public float? minRatePerCycle;
        public float? maxRatePerCycle;
        public float? maxPressure;
        public float? minIterationLength;
        public float? maxIterationLength;
        public float? minIterationPercent;
        public float? maxIterationPercent;
        public float? minYearLength;
        public float? maxYearLength;
        public float? minYearPercent;
        public float? maxYearPercent;
        public string Name;
        public string Description;
        public string Disease;
        public int? DiseaseCount;
        public bool? IsGeneric;

        public GeyserStruct(
            string id,
            string element = null,
            string anim = null,
            int? width = null,
            int? height = null,
            float? temperature = null,
            float? minRatePerCycle = null,
            float? maxRatePerCycle = null,
            float? maxPressure = null,
            float? minIterationLength = null,
            float? maxIterationLength = null,
            float? minIterationPercent = null,
            float? maxIterationPercent = null,
            float? minYearLength = null,
            float? maxYearLength = null,
            float? minYearPercent = null,
            float? maxYearPercent = null,
            string Name = null,
            string Description = null,
            string Disease = null,
            int? DiseaseCount = null,
            bool? IsGeneric = null)
        {
            this.id = id;
            this.element = element;
            this.anim = anim;
            this.width = width;
            this.height = height;
            this.temperature = temperature;
            this.minRatePerCycle = minRatePerCycle;
            this.maxRatePerCycle = maxRatePerCycle;
            this.maxPressure = maxPressure;
            this.minIterationLength = minIterationLength;
            this.maxIterationLength = maxIterationLength;
            this.minIterationPercent = minIterationPercent;
            this.maxIterationPercent = maxIterationPercent;
            this.minYearLength = minYearLength;
            this.maxYearLength = maxYearLength;
            this.minYearPercent = minYearPercent;
            this.maxYearPercent = maxYearPercent;
            this.Name = Name;
            this.Description = Description;
            this.Disease = Disease;
            this.DiseaseCount = DiseaseCount;
            this.IsGeneric = IsGeneric;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == (obj as GeyserStruct)?.GetHashCode();
        }

        public override int GetHashCode()
        {
            if (hash == 0)
                hash = Hash.SDBMLower(this.id);
            return hash;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}