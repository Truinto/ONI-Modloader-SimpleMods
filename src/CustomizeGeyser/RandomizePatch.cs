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
		public static HashedString[] geysers = null;
		public static int[] weights = null;
		public static HashedString[] ignoreTypes = new HashedString[0];
		public static int sum = 0;
		public static string[] ignoreMissing = new string[] { "liquid_sulfur", "molten_niobium", "molten_cobalt", "molten_tungsten", "molten_aluminum", };

		public static void FixErrors()
		{
			List<string> itemsToRemove = new List<string>();

			foreach (string entry in CustomizeGeyserState.StateManager.State.RNGTable.Keys)
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
				CustomizeGeyserState.StateManager.State.RNGTable.Remove(entry);
				if (!ignoreMissing.Contains(entry))
					Helpers.PrintDialog("GeyserRandomizer: Geyser in config non existent, check spelling: " + entry);
			}
		}

		public static void Initialize()
		{
#if DEBUG_1
			Helpers.Print("Running Initialize()...");
#endif
			if (CustomizeGeyserState.StateManager.State.RNGTable == null)
            {
				CustomizeGeyserState.StateManager.State.RandomizerEnabled = false;
				return;
			}

			if (GeyserInfo.Config == null || GeyserInfo.GeyserTypes == null)
				return;

            FixErrors();

			int count = 0;
			foreach (KeyValuePair<string, int> entry in CustomizeGeyserState.StateManager.State.RNGTable)
			{
				if (entry.Value > 0) count++;
			}

			geysers = new HashedString[count];
			weights = new int[count];
			ignoreTypes = new HashedString[CustomizeGeyserState.StateManager.State.RNGTable.Count - count];

            sum = 0; int i = 0; int j = 0;
			foreach (KeyValuePair<string, int> entry in CustomizeGeyserState.StateManager.State.RNGTable)
			{
				if (entry.Value > 0)
				{
					geysers[i] = (HashedString)entry.Key;
					weights[i] = entry.Value;
					sum += entry.Value;
					i++;
				}
				else
				{
					ignoreTypes[j++] = (HashedString)entry.Key;
				}
			}
		}
		
		public static void Reinitialize()
		{
			Initialize();
		}
	
		public static string GetRandomGeyserType(Transform transform)
        {
			System.Random RNG = transform != null ? GetRNG(transform) : new System.Random();

			int random = RNG.Next(sum);  // 0 <= random < sum
			int pick = 0;
			for (int j = 0; j <= random;) j += weights[pick++];
			return GeyserConfigurator.FindType(geysers[pick - 1]).id;
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
		public static bool isFirst = true;

		public static void Prefix(Geyser __instance)
		{
#if DEBUG_1
			Helpers.Print("Geyser.OnSpawn " + __instance.GetComponent<GeyserConfigurator>().presetType.ConvertGeyserId());
#endif
//			if (RandomizerTable.geysers == null) RandomizerTable.Initialize();
//
//			if (CustomizeGeyserState.StateManager.State.RandomizerEnabled && (__instance.configuration == null || __instance.configuration.typeId == HashedString.Invalid))
//			{
//				var config = __instance.GetComponent<GeyserConfigurator>();
//
//				config.presetType = RandomizerTable.GetRandomGeyserType(CustomizeGeyserState.StateManager.State.RandomizerUsesMapSeed ? __instance.transform : null);
//
//				if (isFirst)
//					config.presetType = GeyserInfo.GeyserTypes.Find(x => x.id == CustomizeGeyserState.StateManager.State.RandomizerSetFirstGeyser)?.id ?? config.presetType;
//#if DEBUG_1
//				Helpers.Print("Changed geyser to: " + __instance.GetComponent<GeyserConfigurator>().presetType.ConvertGeyserId());
//#endif
//				if (CustomizeGeyserState.StateManager.State.RandomizerPopupGeyserDiscoveryInfo)
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
			return CustomizeGeyserState.StateManager.State.RandomizerEnabled && CustomizeGeyserState.StateManager.State.RNGTable != null;
		}

		public static void Postfix(GeyserConfigurator __instance, ref GeyserConfigurator.GeyserInstanceConfiguration __result)
		{
			if (CustomizeGeyserState.StateManager.State.RandomizerRerollsCycleRate)
			{
				System.Random RNG;
				if (CustomizeGeyserState.StateManager.State.RandomizerUsesMapSeed)
					RNG = new System.Random(SaveLoader.Instance.clusterDetailSave.globalWorldSeed + (int)__instance.transform.GetPosition().x + (int)__instance.transform.GetPosition().y);
				else
					RNG = new System.Random();

				__result.rateRoll = (float)RNG.NextDouble();
				__result.iterationLengthRoll = (float)RNG.NextDouble();
				__result.iterationPercentRoll = (float)RNG.NextDouble();
				__result.yearLengthRoll = (float)RNG.NextDouble();
				__result.yearPercentRoll = (float)RNG.NextDouble();
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
			return CustomizeGeyserState.StateManager.State.RandomizerEnabled && CustomizeGeyserState.StateManager.State.RNGTable != null;
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

			if (RandomizerTable.geysers == null) RandomizerTable.Initialize();
		}

        public static void PrefabInitGenericGeysers(GameObject go)
		{
#if DEBUG_1
			Helpers.Print("PrefabInitGenericGeysers");
#endif
            if (RandomizerTable.sum > 0)
			{
				string geyserTag = RandomizerTable.GetRandomGeyserType(go.transform);

				if (CustomizeGeyserState.StateManager.State.RandomizerPopupGeyserDiscoveryInfo)
					KMod.Manager.Dialog(null, "Geysers discovered", "You just discovered a geyser: " + geyserTag);
				Helpers.Print("Discovered a new geyser: " + geyserTag);

				GameUtil.KInstantiate(Assets.GetPrefab(
					(Tag)("GeyserGeneric_" + geyserTag)	 //change Tag to whatever you want to spawn; Tag is "GeyserGeneric_" + geyserType.id
				), go.transform.GetPosition(), Grid.SceneLayer.BuildingBack, (string)null, 0).SetActive(true);
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
            return CustomizeGeyserState.StateManager.State.RandomizerEnabled && CustomizeGeyserState.StateManager.State.RNGTable != null;
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
				if (CustomizeGeyserState.StateManager.State.RandomizerHighlanderMode || (bool)SaveGame.Instance?.BaseName?.Contains("Highlander"))
				{
					HashedString id = go.GetComponent<GeyserConfigurator>()?.presetType ?? "none";
					int i;
					bool flag = false;
					for (i = 0; i < RandomizerTable.geysers.Length; i++)
					{
						if (RandomizerTable.geysers[i] == id)
						{
							flag = true;
							break;
						}
					}
#if DEBUG_1
					Helpers.Print("Attached to geyser [" + i + "] id: " + id + " flag is " + flag);
#endif

					if (flag && RandomizerTable.weights[i] >= 1)
					{
						RandomizerTable.weights[i]--;
						RandomizerTable.sum--;
#if DEBUG_1
						Helpers.Print("Reduced weight of [" + i + "] to " + RandomizerTable.weights[i]);
#endif
					}
					else if (CustomizeGeyserState.StateManager.State.RandomizerHighlanderRetroactive)
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

		public static bool ChangeGeyserElement(GameObject go, string new_id = null)
        {
            return GeyserMorph.ChangeGeyserElement(go, new_id);
        }
    }
	


	[HarmonyPatch(typeof(SaveLoader), "Load", new Type[] { typeof(string) })]
	public class OnSaveLoad
	{
		public static bool Prepare()
		{
			return CustomizeGeyserState.StateManager.State.RandomizerEnabled && CustomizeGeyserState.StateManager.State.RNGTable != null;
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
            return CustomizeGeyserState.StateManager.State.RandomizerEnabled && CustomizeGeyserState.StateManager.State.RNGTable != null;
        }

        public static void Prefix()
        {
			Helpers.Print("Re-initializing RandomizerTable...");
            RandomizerTable.Reinitialize();
        }
    }
}