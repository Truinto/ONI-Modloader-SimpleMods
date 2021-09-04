using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomizeRecipe
{
    /// need [Id] or [Building, Inputs, Outputs]
    /// note that you always need Id, if you are changing any elements of existing recipes
    /// buildings must be able to handle HEP, otherwise values higher than 0 will cause a crash when recipe is selected
    public class RecipeData
    {
        public string Id;
        public string Building;
        public List<RecipeElement> Inputs;
        public List<RecipeElement> Outputs;
        public float? Time;
        public int? HEP;
        public string Description;

        public class RecipeElement
        {
            public string material;
            public float amount;
            [JsonConverter(typeof(StringEnumConverter))]
            public TemperatureOperation? temperatureOperation;
            public bool? storeElement;
            public bool? inheritElement;

            public RecipeElement() { }

            public RecipeElement(string material, float amount)
            {
                this.material = material;
                this.amount = amount;
            }

            public static implicit operator ComplexRecipe.RecipeElement(RecipeElement recipe)
            {
                return new ComplexRecipe.RecipeElement(recipe.material, recipe.amount, (ComplexRecipe.RecipeElement.TemperatureOperation)(recipe.temperatureOperation ?? 0), recipe.storeElement ?? false)
                {
                    inheritElement = recipe.inheritElement ?? false
                };
            }

            public static implicit operator RecipeElement(ComplexRecipe.RecipeElement recipe)
            {
                var result = new RecipeElement(recipe.material.ToString(), recipe.amount);
                if (recipe.temperatureOperation != 0)
                    result.temperatureOperation = (TemperatureOperation)recipe.temperatureOperation;
                if (recipe.storeElement)
                    result.storeElement = true;
                if (recipe.inheritElement)
                    result.inheritElement = true;
                return result;
            }
        }

        public enum TemperatureOperation
        {
            AverageTemperature = 0,
            Heated = 1,
            Melted = 2
        }

        public RecipeData() { }

        public RecipeData(string Id = null, string Building = null, float? Time = null, int? HEP = null, string Description = null, ComplexRecipe.RecipeElement[] ingredients = null, ComplexRecipe.RecipeElement[] results = null)
        {
            this.Id = Id;
            this.Building = Building;
            this.Time = Time;
            this.HEP = HEP;
            this.Description = Description;

            if (ingredients != null)
            {
                this.Inputs = new List<RecipeElement>();
                foreach (var ingredient in ingredients)
                    this.Inputs.Add(ingredient);
            }

            if (results != null)
            {
                this.Outputs = new List<RecipeElement>();
                foreach (var result in results)
                    this.Outputs.Add(result);
            }
        }

        public RecipeData FindId(string fabricator, SimHashes[] inputs, SimHashes[] outputs)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(fabricator);
            stringBuilder.Append("_I");
            foreach (var input in inputs)
            {
                stringBuilder.Append("_");
                stringBuilder.Append(input.ToString());
            }
            stringBuilder.Append("_O");
            foreach (var output in outputs)
            {
                stringBuilder.Append("_");
                stringBuilder.Append(output.ToString());
            }
            this.Id = stringBuilder.ToString();
            return this;
        }

        public RecipeData In(string material, float amount)
        {
            if (this.Inputs == null)
                this.Inputs = new List<RecipeElement>();
            this.Inputs.Add(new RecipeElement(material, amount));
            return this;
        }

        public RecipeData Out(string material, float amount)
        {
            if (this.Outputs == null)
                this.Outputs = new List<RecipeElement>();
            this.Outputs.Add(new RecipeElement(material, amount));
            return this;
        }

        public RecipeData In(SimHashes material, float amount)
        {
            return In(material.ToString(), amount);
        }

        public RecipeData Out(SimHashes material, float amount)
        {
            return Out(material.ToString(), amount);
        }

        [JsonIgnore]
        public List<ComplexRecipe.RecipeElement> InputsList => Inputs.Select(s => (ComplexRecipe.RecipeElement)s).ToList();
        [JsonIgnore]
        public List<ComplexRecipe.RecipeElement> OutputsList => Outputs.Select(s => (ComplexRecipe.RecipeElement)s).ToList();
        [JsonIgnore]
        public ComplexRecipe.RecipeElement[] InputsArray => Inputs.Select(s => (ComplexRecipe.RecipeElement)s).ToArray();
        [JsonIgnore]
        public ComplexRecipe.RecipeElement[] OutputsArray => Outputs.Select(s => (ComplexRecipe.RecipeElement)s).ToArray();
    }
}
