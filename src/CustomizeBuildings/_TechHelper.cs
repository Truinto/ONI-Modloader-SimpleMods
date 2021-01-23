using Harmony;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CustomizeBuildings
{
    public class TechHelper
    {
        public const string Base = "Base";
        public const string Oxygen = "Oxygen";
        public const string Power = "Generators";
        public const string Food = "Food";
        public const string Plumbing = "Plumbing Structures";
        public const string Ventilation = "Ventilation Structures";
        public const string Refining = "Refining";
        public const string Medicine = "Medical";
        public const string Furniture = "Decor";
        public const string Stations = "Refining";
        public const string Utilities = "Utilities";
        public const string Automation = "LogicWiring";
        public const string Shipping = "Logistics";
        public const string Rocketry = "Rocketry";

        public const string FarmingTech = "FarmingTech";
        public const string RanchingTech = "Ranching";

        public static void AddBuildingToPlanScreen(HashedString category, string buildingId, string afterBuildingId)
        {
            int index = TUNING.BUILDINGS.PLANORDER.FindIndex(x => x.category == category);
            if (index < 0)
                return;
            IList<string> data = TUNING.BUILDINGS.PLANORDER[index].data as IList<string>;
            if (data == null)
            {
                Debug.LogWarning("Could not add " + buildingId);
            }
            else
            {
                int num = data.IndexOf(afterBuildingId);
                if (num != -1)
                    data.Insert(num + 1, buildingId);
                else
                    data.Add(buildingId);
            }
        }

        public static void AddBuildingToPlanScreen(HashedString category, string buildingId)
        {
            ModUtil.AddBuildingToPlanScreen(category, buildingId);
        }

        public static void AddBuildingToTechnology(string tech, string buildingId)
        {
            //List<string> stringList = new List<string>(Database.Techs.TECH_GROUPING[tech])
            //{
            //    buildingId
            //};
            //Database.Techs.TECH_GROUPING[tech] = stringList.ToArray();

            throw new NotImplementedException();
            //Db.Get().Techs.
        }
    }
    
}