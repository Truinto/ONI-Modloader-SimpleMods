using System.Collections.Generic;

namespace CustomizePlants
{
    public class PlantData
    {
        private int hash;
        public string id;
        public string fruitId;
        public float? fruit_grow_time;
        public int? fruit_amount;
        public Dictionary<string, float> irrigation;
        public float? illumination;
        public string[] safe_elements;
        public float[] temperatures;
        public float[] pressures;
        public int? decor_value;
        public int? decor_radius;
        public float? submerged_threshold;
        public bool? can_tinker;
        public bool? require_solid_tile;
        public float? max_age;
        public string disease;
        public int? disease_amount;
        public string input_element;
        public float? input_rate;
        public string output_element;
        public float? output_rate;
        public float? radiation;
        public int? radiation_radius;
        public float? radiation_threshold_min;
        public float? radiation_threshold_max;

        /// <summary>
        /// Holds settings for one plant.
        /// </summary>
        /// <param name="id">Required. Defines which plant is affected.</param>
        /// <param name="fruitId">Must be valid Tag or listed in SpecialCropSettings.</param>
        /// <param name="fruit_grow_time">Time for the crop to grow in seconds.</param>
        /// <param name="fruit_amount">Number of objects or amount in kg, depends on Tag.</param>
        /// <param name="irrigation">List of irrigation needed. May be any liquid or solid element. Amount in kg per cycle</param>
        /// <param name="illumination">If equal 0 removes existing component. If less than 0 requires darkness. Otherwise number is light threshold required.</param>
        /// <param name="safe_elements">List of gas elements plant has to be in. If empty all elements are suitable.</param>
        /// <param name="temperatures">Array of temperatures in Kelvin. 1) death if lower 2) wilt if lower 3) wilt if higher 4) death if higher; entries after 4 are ignored; may have less than 4 entries</param>
        /// <param name="pressures">Array of pressures in kg. 1) death if lower 2) wilt if lower 3) wilt if higher 4) death if higher; entries after 4 are ignored; may have less than 4 entries</param>
        /// <param name="decor_value">Decor score.</param>
        /// <param name="decor_radius">Range at which the decor score is applied to.</param>
        /// <param name="submerged_threshold">If equal 0 ignores water. If less than 0 hates water. If higher than 0 needs water. Plant will wilt in bad conditions.</param>
        /// <param name="can_tinker">Whenever plant can be interacted with farming station.</param>
        /// <param name="require_solid_tile">Not sure...</param>
        /// <param name="max_age">If 0 or less, will never auto-harvest itself. Otherwise time in seconds for the plant to auto-harvest and plays bristled animation when at 50%+.</param>
        /// <param name="disease">Type of disease the plant spreads around it. May be: FoodPoisoning, SlimeLung, PollenGerms, or ZombieSpores.</param>
        /// <param name="disease_amount">How much disease is spread around it.</param>
        /// <param name="input_element">Type of gas or liquid plant absorbs from environment.</param>
        /// <param name="input_rate">Amount absorbed per second.</param>
        /// <param name="output_element">Type of gas or liquid plant expels per second.</param>
        /// <param name="output_rate">Amount expelled per second.</param>
        /// <param name="radiation">Amount of radiation emitted.</param>
        /// <param name="radiation_radius">Radius of radiation (if any).</param>
        /// <param name="radiation_threshold_min">Minimum radiation threshold.</param>
        /// <param name="radiation_threshold_max">Maximum radiation threshold.</param>
        public PlantData(string id, string fruitId = null, float? fruit_grow_time = null, int? fruit_amount = null, Dictionary<string, float> irrigation = null, float? illumination = null, string[] safe_elements = null, float[] temperatures = null, float[] pressures = null, int? decor_value = null, int? decor_radius = null, float? submerged_threshold = null, bool? can_tinker = null, bool? require_solid_tile = null, float? max_age = null, string disease = null, int? disease_amount = null, string input_element = null, float? input_rate = null, string output_element = null, float? output_rate = null, float? radiation = null, int? radiation_radius = null, float? radiation_threshold_min = null, float? radiation_threshold_max = null)
        {
            this.id = id;
            this.fruitId = fruitId;
            this.fruit_grow_time = fruit_grow_time;
            this.fruit_amount = fruit_amount;
            this.irrigation = irrigation;
            this.illumination = illumination;
            this.safe_elements = safe_elements;
            this.temperatures = temperatures;
            this.pressures = pressures;
            this.decor_value = decor_value;
            this.decor_radius = decor_radius;
            this.submerged_threshold = submerged_threshold;
            this.can_tinker = can_tinker;
            this.require_solid_tile = require_solid_tile;
            this.max_age = max_age;
            this.disease = disease;
            this.disease_amount = disease_amount;
            this.input_element = input_element;
            this.input_rate = input_rate;
            this.output_element = output_element;
            this.output_rate = output_rate;
            this.radiation = radiation;
            this.radiation_radius = radiation_radius;
            this.radiation_threshold_min = radiation_threshold_min;
            this.radiation_threshold_max = radiation_threshold_max;
        }

        public PlantData()
        { }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == (obj as PlantData)?.GetHashCode();
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
