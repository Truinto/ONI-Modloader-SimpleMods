using System;
using Harmony;
using System.Collections.Generic;
using UnityEngine;
using static BootDialog.PostBootDialog;

namespace CustomizeGeyser
{
    internal class RandomizerTable
    {
        internal static HashedString[] geysers = null;
        internal static int[] weights = null;
        internal static HashedString[] ignoreTypes = new HashedString[0];
        internal static int sum = 0;

        private static void FixErrors()
        {
            List<GeyserConfigurator.GeyserType> geyserTypes = (List<GeyserConfigurator.GeyserType>)AccessTools.Field(typeof(GeyserConfigurator), "geyserTypes").GetValue(null);

            List<string> itemsToRemove = new List<string>();

            foreach (string entry in CustomizeGeyserState.StateManager.State.RNGTable.Keys)
            {
                GeyserConfigurator.GeyserType geyserType = geyserTypes.Find((Predicate<GeyserConfigurator.GeyserType>)(t => t.id == entry));

                if (geyserType == null) itemsToRemove.Add(entry);
            }

            foreach (string entry in itemsToRemove)
            {
                CustomizeGeyserState.StateManager.State.RNGTable.Remove(entry);
                Debug.LogWarning(ToDialog("GeyserRandomizer: Geyser in config non existent, check spelling: " + entry));
            }
        }

        internal static void Initialize()
        {
            FixErrors();

            int count = 0;
            foreach (KeyValuePair<string, int> entry in CustomizeGeyserState.StateManager.State.RNGTable)
            {
                if (entry.Value > 0) count++;
            }

            if (count < 1)  //if the config has no allowed geysers, disable the mod
            {
                CustomizeGeyserState.StateManager.State.RandomizerEnabled = false;
                return;
            }

            geysers = new HashedString[count];
            weights = new int[count];
            ignoreTypes = new HashedString[CustomizeGeyserState.StateManager.State.RNGTable.Count - count];

            int i = 0; int j = 0;
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
    }

    //[HarmonyPatch(typeof(GeyserConfigurator), "CreateRandomInstance")]
    internal class GeyserConfigurator_CreateRandomInstance
    {

        internal static bool Prepare()
        {
            return CustomizeGeyserState.StateManager.State.RandomizerEnabled && CustomizeGeyserState.StateManager.State.RNGTable != null;
        }

        internal static void Postfix(GeyserConfigurator __instance, ref GeyserConfigurator.GeyserInstanceConfiguration __result)
        {
            //if ( GeyserConfigurator.FindType(__result.typeId).id.StartsWith("GeyserGeneric_") ) Note: "GeyserGeneric_" is added later and never true here

            //if ( Array.IndexOf(RandomizerTable.ignoreTypes, __result.typeId) < 0 )	//if typeId is not in ignoreTypes
            if (CustomizeGeyserState.StateManager.State.RandomizerRerollsCycleRate)
            {
                System.Random RNG;
                if (CustomizeGeyserState.StateManager.State.RandomizerUsesMapSeed)
                    RNG = new System.Random(SaveLoader.Instance.worldDetailSave.globalWorldSeed + (int)__instance.transform.GetPosition().x + (int)__instance.transform.GetPosition().y);
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

    [HarmonyPatch(typeof(GeyserGenericConfig), "CreatePrefabs")]
    internal class GeyserGenericConfig_CreatePrefabs
    {
        internal static bool Prepare()
        {
            return CustomizeGeyserState.StateManager.State.RandomizerEnabled && CustomizeGeyserState.StateManager.State.RNGTable != null;
        }
        
        internal static bool Prefix(GeyserGenericConfig __instance, ref List<GameObject> __result)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            List<GeyserGenericConfig.GeyserPrefabParams> configs = (List<GeyserGenericConfig.GeyserPrefabParams>) AccessTools.Method(typeof(GeyserGenericConfig), "GenerateConfigs").Invoke(__instance, null);
            foreach (GeyserGenericConfig.GeyserPrefabParams geyserPrefabParams in configs)
                gameObjectList.Add(__instance.CreateGeyser(geyserPrefabParams.id, geyserPrefabParams.anim, geyserPrefabParams.width, geyserPrefabParams.height, Strings.Get(geyserPrefabParams.nameStringKey), Strings.Get(geyserPrefabParams.descStringKey), geyserPrefabParams.geyserType.idHash));
            GameObject entity = EntityTemplates.CreateEntity("GeyserGeneric", "Random Geyser Spawner", true);
            entity.AddOrGet<SaveLoadRoot>();
            entity.GetComponent<KPrefabID>().prefabInitFn += (go =>
            {
                PrefabInitForGeysers(go);
            });
            gameObjectList.Add(entity);
            __result = gameObjectList;

            if (RandomizerTable.geysers == null) RandomizerTable.Initialize();

            return false;
        }

        internal static void PrefabInitForGeysers(GameObject go)
        {
            System.Random RNG;
            if (CustomizeGeyserState.StateManager.State.RandomizerUsesMapSeed)
            {
                RNG = new System.Random(SaveLoader.Instance.worldDetailSave.globalWorldSeed + (int)go.transform.GetPosition().x + (int)go.transform.GetPosition().y);
            }
            else
                RNG = new System.Random();

            int random = RNG.Next(RandomizerTable.sum);  // 0 <= random < sum
            int pick = 0;
            for (int j = 0; j <= random;) j += RandomizerTable.weights[pick++];
            string geyserTag = GeyserConfigurator.FindType(RandomizerTable.geysers[pick - 1]).id;

            if (CustomizeGeyserState.StateManager.State.RandomizerPopupGeyserDiscoveryInfo)
                KMod.Manager.Dialog(null, "Geysers discovered", "You just discovered a geyser: " + geyserTag);

            GameUtil.KInstantiate(Assets.GetPrefab(
                (Tag)("GeyserGeneric_" + geyserTag)  //change Tag to whatever you want to spawn; Tag is "GeyserGeneric_" + geyserType.id
            ), go.transform.GetPosition(), Grid.SceneLayer.BuildingBack, (string)null, 0).SetActive(true);
            go.DeleteObject();
        }

        internal static void NotPostfix(ref List<GameObject> __result)
        {
            for (int i = 0; i < __result.Count; i++)
            {
                GameObject entity = __result[i];
                KPrefabID prefab = entity.GetComponent<KPrefabID>();
                //prefab.prefabInitFn = null;
                prefab.prefabInitFn += (KPrefabID.PrefabFn)(go =>
                {
                    PrefabInitForGeysers(go);
                });
            }
        }
    }
}