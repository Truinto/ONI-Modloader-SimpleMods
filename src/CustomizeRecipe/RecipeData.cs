using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Shared.CollectionNS;

namespace CustomizeRecipe
{
    /// need [Id] or [Building, Inputs, Outputs]
    /// note that you always need Id, if you are changing any elements of existing recipes
    /// buildings must be able to handle HEP, otherwise values higher than 0 will cause a crash when recipe is selected
    public class RecipeData
    {
        public string? Id;
        public string? Building;
        public List<RecipeElement> Inputs = new();
        public List<RecipeElement> Outputs = new();
        public float? Time;
        public int? HEP;
        public int? HEPout;
        public ComplexRecipe.RecipeNameDisplay? NameDisplay;
        public string? CustomName;
        public string? CustomSpritePrefabID;
        public string? Description;

        public class RecipeElement
        {
            public string? material;
            public string[]? materials;
            public float? amount;
            public float[]? amounts;
            [JsonConverter(typeof(StringEnumConverter))]
            public ComplexRecipe.RecipeElement.TemperatureOperation? temperatureOperation;
            public bool? storeElement;
            public bool? inheritElement;
            public string? facadeID;
            public bool? doNotConsume;

            public RecipeElement() { }

            public RecipeElement(string material, float amount, ComplexRecipe.RecipeElement.TemperatureOperation? temperatureOperation = null)
            {
                this.material = material;
                this.amount = amount;
                this.temperatureOperation = temperatureOperation;
            }

            public static implicit operator ComplexRecipe.RecipeElement(RecipeElement recipe)
            {
                var output = new ComplexRecipe.RecipeElement(recipe.material.ToTagSafe(), recipe.amount ?? 0f);
                if (recipe.materials != null && recipe.materials.Length > 0)
                {
                    output.possibleMaterials = new Tag[recipe.materials.Length];
                    for (int i = 0; i < output.possibleMaterials.Length; i++)
                        output.possibleMaterials[i] = recipe.materials[i].ToTagSafe();
                    output.possibleMaterialAmounts = new float[output.possibleMaterials.Length];
                    for (int i = 0; i < output.possibleMaterialAmounts.Length; i++)
                        output.possibleMaterialAmounts[i] = recipe.amounts?.ElementAtOrDefault(i) ?? output.amount;
                }
                output.temperatureOperation = recipe.temperatureOperation ?? 0;
                output.storeElement = recipe.storeElement ?? output.possibleMaterials[0].ToElement().IsLiquid;
                output.inheritElement = recipe.inheritElement ?? false;
                output.facadeID = recipe.facadeID;
                output.doNotConsume = recipe.doNotConsume ?? false;
                return output;
            }

            public static implicit operator RecipeElement(ComplexRecipe.RecipeElement recipe)
            {
                var output = new RecipeElement();
                if (recipe.possibleMaterials.Length == 1) // possibleMaterials is never null
                {
                    output.material = recipe.possibleMaterials[0].ToString();
                    output.amount = recipe.amount != 0f ? recipe.amount : recipe.possibleMaterialAmounts?.ElementAtOrDefault(0) ?? 0f;
                }
                else
                {
                    output.materials = recipe.possibleMaterials.Select(s => s.ToString()).ToArray();
                    output.amounts = recipe.possibleMaterialAmounts ?? new float[output.materials.Length].Fill(0f);
                }
                if (recipe.temperatureOperation != 0)
                    output.temperatureOperation = recipe.temperatureOperation;
                if (recipe.storeElement)
                    output.storeElement = true;
                if (recipe.inheritElement)
                    output.inheritElement = true;
                if (output.facadeID is not null or "")
                    output.facadeID = recipe.facadeID;
                if (recipe.doNotConsume)
                    output.doNotConsume = recipe.doNotConsume;
                return output;
            }
        }

        public RecipeData() { }

        public RecipeData(string? Id = null, string? Building = null, float? Time = null, int? HEP = null, int? HEPout = null, string? Description = null, ComplexRecipe.RecipeElement[]? ingredients = null, ComplexRecipe.RecipeElement[]? results = null)
        {
            this.Id = Id;
            this.Building = Building;
            this.Time = Time;
            this.HEP = HEP;
            this.HEPout = HEPout;
            this.Description = Description;

            if (ingredients != null)
            {
                foreach (var ingredient in ingredients)
                    this.Inputs.Add(ingredient);
            }

            if (results != null)
            {
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
            this.Inputs.Add(new RecipeElement(material, amount));
            return this;
        }

        public RecipeData Out(string material, float amount, ComplexRecipe.RecipeElement.TemperatureOperation? temperatureOperation = null)
        {
            this.Outputs.Add(new RecipeElement(material, amount, temperatureOperation));
            return this;
        }

        public RecipeData In(SimHashes material, float amount)
        {
            return In(material.ToString(), amount);
        }

        public RecipeData Out(SimHashes material, float amount, ComplexRecipe.RecipeElement.TemperatureOperation? temperatureOperation = null)
        {
            return Out(material.ToString(), amount, temperatureOperation);
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
