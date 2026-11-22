#define DEBUG_1

using System;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static Config.PostBootDialog;
using System.Linq;
using Common;

namespace CustomizeGeyser
{
	public class RandomizerTable
	{
		public static HashedString[] Geysers = [];
		public static int[] Weights = [];
		public static HashedString[] IgnoreTypes = [];
		public static int Sum = 0;
		public static string[] IgnoreMissing = ["liquid_sulfur", "molten_niobium", "molten_cobalt", "molten_tungsten", "molten_aluminum",];

		public static void FixErrors()
		{
			List<string> itemsToRemove = new List<string>();

			foreach (string entry in CustomizeGeyserState.Instance.RNGTable.Keys)
			{
				if (!GeyserInfo.GeyserTypes.Any(a => a.id == entry))
					itemsToRemove.Add(entry);

				else if (!GeyserInfo.Config.Any(a => a.id == "GeyserGeneric_" + entry)) //(a => a.geysertype.id == entry)
					itemsToRemove.Add(entry);

				//if (!Assets.Prefabs.Any(a => a.PrefabID() == "GeyserGeneric_" + entry))
				//	itemsToRemove.Add(entry);
			}

			

			foreach (string entry in itemsToRemove)
			{
				CustomizeGeyserState.Instance.RNGTable.Remove(entry);
				if (!IgnoreMissing.Contains(entry))
					Helpers.PrintDialog("GeyserRandomizer: Geyser in config non existent, check spelling: " + entry);
			}
		}

		public static void Initialize()
		{
#if DEBUG_1
			Helpers.Print("Running Initialize()...");
#endif
			if (CustomizeGeyserState.Instance.RNGTable == null)
            {
				CustomizeGeyserState.Instance.RandomizerEnabled = false;
				return;
			}

			if (GeyserInfo.Config == null || GeyserInfo.GeyserTypes == null)
				return;

            FixErrors();

			int count = 0;
			foreach (KeyValuePair<string, int> entry in CustomizeGeyserState.Instance.RNGTable)
			{
				if (entry.Value > 0) count++;
			}

			Geysers = new HashedString[count];
			Weights = new int[count];
			IgnoreTypes = new HashedString[CustomizeGeyserState.Instance.RNGTable.Count - count];

            Sum = 0; int i = 0; int j = 0;
			foreach (KeyValuePair<string, int> entry in CustomizeGeyserState.Instance.RNGTable)
			{
				if (entry.Value > 0)
				{
					Geysers[i] = (HashedString)entry.Key;
					Weights[i] = entry.Value;
					Sum += entry.Value;
					i++;
				}
				else
				{
					IgnoreTypes[j++] = (HashedString)entry.Key;
				}
			}
		}
		
		public static void Reinitialize()
		{
			Initialize();
		}
	
		public static string GetRandomGeyserType(Transform transform)
        {
			System.Random rng = transform != null ? GetRNG(transform) : new System.Random();

			int random = rng.Next(Sum);  // 0 <= random < sum
			int pick = 0;
			for (int j = 0; j <= random;) j += Weights[pick++];
			return GeyserConfigurator.FindType(Geysers[pick - 1]).id;
		}

		public static System.Random GetRNG(Transform transform)
        {
			return new System.Random(SaveLoader.Instance.clusterDetailSave.globalWorldSeed + (int)transform.GetPosition().x + (int)transform.GetPosition().y);
		}
	}

	/// Only for debug.
	[HarmonyPatch(typeof(Geyser), "OnSpawn")]
	public class Geyser_OnSpawn
	{
		public static bool IsFirst = true;

		public static void Prefix(Geyser __instance)
		{
#if DEBUG_1
			Helpers.Print("Geyser.OnSpawn " + __instance.GetComponent<GeyserConfigurator>().presetType.ConvertGeyserId());
#endif
//			if (RandomizerTable.geysers == null) RandomizerTable.Initialize();
//
//			if (CustomizeGeyserState.Instance.RandomizerEnabled && (__instance.configuration == null || __instance.configuration.typeId == HashedString.Invalid))
//			{
//				var config = __instance.GetComponent<GeyserConfigurator>();
//
//				config.presetType = RandomizerTable.GetRandomGeyserType(CustomizeGeyserState.Instance.RandomizerUsesMapSeed ? __instance.transform : null);
//
//				if (isFirst)
//					config.presetType = GeyserInfo.GeyserTypes.Find(x => x.id == CustomizeGeyserState.Instance.RandomizerSetFirstGeyser)?.id ?? config.presetType;
//#if DEBUG_1
//				Helpers.Print("Changed geyser to: " + __instance.GetComponent<GeyserConfigurator>().presetType.ConvertGeyserId());
//#endif
//				if (CustomizeGeyserState.Instance.RandomizerPopupGeyserDiscoveryInfo)
//				{
//					KMod.Manager.Dialog(null, "Geysers discovered", "You just discovered a geyser: " + __instance.GetComponent<GeyserConfigurator>().presetType.ConvertGeyserId());
//				}
//			}
//
//			isFirst = false;
		}
	}

	/// <summary> Patches the randomization of the geyers parameters. NOT the geyser type. </summary>
	[HarmonyPatch(typeof(GeyserConfigurator), "CreateRandomInstance")]
	public class GeyserConfigurator_CreateRandomInstance
	{
		public static bool Prepare()
		{
			return CustomizeGeyserState.Instance.RandomizerEnabled && CustomizeGeyserState.Instance.RNGTable != null;
		}

		public static void Postfix(GeyserConfigurator __instance, ref GeyserConfigurator.GeyserInstanceConfiguration __result)
		{
			if (CustomizeGeyserState.Instance.RandomizerRerollsCycleRate)
			{
				System.Random rng;
				if (CustomizeGeyserState.Instance.RandomizerUsesMapSeed)
					rng = new System.Random(SaveLoader.Instance.clusterDetailSave.globalWorldSeed + (int)__instance.transform.GetPosition().x + (int)__instance.transform.GetPosition().y);
				else
					rng = new System.Random();

				__result.rateRoll = (float)rng.NextDouble();
				__result.iterationLengthRoll = (float)rng.NextDouble();
				__result.iterationPercentRoll = (float)rng.NextDouble();
				__result.yearLengthRoll = (float)rng.NextDouble();
				__result.yearPercentRoll = (float)rng.NextDouble();
			}
		}
	}

    /// <summary> Attachs Init function to GeyserGeneric that manages the geyser type randomization process.
    /// If no weight is left, GeyserGeneric does not get replaced at all (because it usually get replaced be the choice of geyser picked). </summary>
	[HarmonyPatch(typeof(GeyserGenericConfig), "CreatePrefabs")]
	public class GeyserGenericConfig_CreatePrefabs
	{
        public const string GeyserGenericDescription = "This is highlander mode and you are out of geysers to spawn. I am a placeholder and will not output anything. If you decide to disable the mod or the highlander mode, I will be transformed into a real geyser.";
        public const string GeyserGenericKAnim = "gravestone_pet_kanim"; // geyser_liquid_water_slush_kanim
        public const string GeyserGenericInitialState = "closed"; //inactive

        public static bool Prepare()
		{
			return CustomizeGeyserState.Instance.RandomizerEnabled && CustomizeGeyserState.Instance.RNGTable != null;
		}

        [HarmonyPriority(410)] //slightly higher priority since it clears events and other might want to hook in as well
        public static void Postfix(GeyserGenericConfig __instance, ref List<GameObject> __result)
        {
            GameObject geyserGenericOrg = __result.Find(x => x.name == "GeyserGeneric");
            if (geyserGenericOrg == null)
            {
				Helpers.Print("RandomizerPatch critical error: Did not find GeyserGeneric");
                return;
            }

            GameObject geyserGeneric = EntityTemplates.CreatePlacedEntity(id: "GeyserGeneric", name: "Random Geyser Spawner", desc: GeyserGenericDescription, mass: 2000f,
                anim: Assets.GetAnim(GeyserGenericKAnim), initialAnim: GeyserGenericInitialState, sceneLayer: Grid.SceneLayer.BuildingBack,
                width: 3, height: 3, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER1, noise: TUNING.NOISE_POLLUTION.NOISY.TIER6,
                element: SimHashes.Katairite, additionalTags: null, defaultTemperature: 372.15f);
            geyserGeneric.AddOrGet<SaveLoadRoot>();
            geyserGeneric.AddOrGet<KPrefabID>().prefabInitFn += PrefabInitGenericGeysers;
                
            __result.Remove(geyserGenericOrg);
            __result.Add(geyserGeneric);

#if DEBUG_1
			Helpers.Print("Attached to GeyserGeneric");
#endif

			if (RandomizerTable.Geysers == null) RandomizerTable.Initialize();
		}

        public static void PrefabInitGenericGeysers(GameObject go)
		{
#if DEBUG_1
			Helpers.Print("PrefabInitGenericGeysers");
#endif
            if (RandomizerTable.Sum > 0)
			{
				string geyserTag = RandomizerTable.GetRandomGeyserType(go.transform);

				if (CustomizeGeyserState.Instance.RandomizerPopupGeyserDiscoveryInfo)
					KMod.Manager.Dialog(null, "Geysers discovered", "You just discovered a geyser: " + geyserTag);
				Helpers.Print("Discovered a new geyser: " + geyserTag);

				GameUtil.KInstantiate(Assets.GetPrefab(
					(Tag)("GeyserGeneric_" + geyserTag)	 //change Tag to whatever you want to spawn; Tag is "GeyserGeneric_" + geyserType.id
				), go.transform.GetPosition(), Grid.SceneLayer.BuildingBack, null, 0).SetActive(true);
				go.DeleteObject();
			}
			else
			{
				Helpers.Print("RandomizerTable was empty...");
			}
		}
	}



    /// <summary> Attachs Init function to all Geysers, except GeyserGeneric. When called reduces weight of that geyser.
    /// May also retransform to GeyserGeneric if RandomizerHighlanderRetroactive option is enabled. </summary>
    //[HarmonyPatch(typeof(GeyserGenericConfig), "CreateGeyser")]
    public class GeyserHighlander
    {
        public static bool Prepare()
        {
            return CustomizeGeyserState.Instance.RandomizerEnabled && CustomizeGeyserState.Instance.RNGTable != null;
        }

        public static void Postfix(GameObject __result)
        {
            __result.AddOrGet<KPrefabID>().prefabInitFn += PrefabInitIndividualGeyser;
#if DEBUG_1
			Helpers.Print("Attached to geyser type " + __result.name);
#endif
		}

		public static void PrefabInitIndividualGeyser(GameObject go)
		{
#if DEBUG_1
			Helpers.Print("Running PrefabInitIndividualGeyser");
#endif
			try
			{
				if (CustomizeGeyserState.Instance.RandomizerHighlanderMode || SaveGame.Instance?.BaseName?.Contains("Highlander") is true)
				{
					HashedString id = go.GetComponent<GeyserConfigurator>()?.presetType ?? "none";
					int i;
					bool flag = false;
					for (i = 0; i < RandomizerTable.Geysers.Length; i++)
					{
						if (RandomizerTable.Geysers[i] == id)
						{
							flag = true;
							break;
						}
					}
#if DEBUG_1
					Helpers.Print("Attached to geyser [" + i + "] id: " + id + " flag is " + flag);
#endif

					if (flag && RandomizerTable.Weights[i] >= 1)
					{
						RandomizerTable.Weights[i]--;
						RandomizerTable.Sum--;
#if DEBUG_1
						Helpers.Print("Reduced weight of [" + i + "] to " + RandomizerTable.Weights[i]);
#endif
					}
					else if (CustomizeGeyserState.Instance.RandomizerHighlanderRetroactive)
					{
#if DEBUG_1
						Helpers.Print("Weight of [" + i + "] is 0, changing back to GeyserGeneric");
#endif
						ChangeGeyserElement(go);    //changes geyser back to GeyserGeneric, which in turn rerolls again unless sum is 0
					}
				}
			}
			catch (Exception e)
			{
				Helpers.PrintDialog(e.ToString());
			}
		}

		public static bool ChangeGeyserElement(GameObject go, string? new_id = null)
        {
            return GeyserMorph.ChangeGeyserElement(go, new_id);
        }
    }
	


	[HarmonyPatch(typeof(SaveLoader), "Load", typeof(string))]
	public class OnSaveLoad
	{
		public static bool Prepare()
		{
			return CustomizeGeyserState.Instance.RandomizerEnabled && CustomizeGeyserState.Instance.RNGTable != null;
		}
		
		public static void Prefix()
		{
			Helpers.Print("Re-initializing RandomizerTable...");
			RandomizerTable.Reinitialize();
		}
    }

    [HarmonyPatch(typeof(SaveLoader), "LoadFromWorldGen")]
    public class OnWorldGen
    {
        public static bool Prepare()
        {
            return CustomizeGeyserState.Instance.RandomizerEnabled && CustomizeGeyserState.Instance.RNGTable != null;
        }

        public static void Prefix()
        {
			Helpers.Print("Re-initializing RandomizerTable...");
            RandomizerTable.Reinitialize();
        }
    }
}