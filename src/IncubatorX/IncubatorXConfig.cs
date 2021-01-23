using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class IncubatorXConfig : IBuildingConfig
{
    public const string ID = "IncubatorX";
    public static List<Tag> EGGS = new List<Tag>() { GameTags.Egg };

    public override BuildingDef CreateBuildingDef()
    {
        string id = "IncubatorX";
        int width = 2;
        int height = 3;
        string anim = "incubator_kanim";
        int hitpoints = 30;
        float construction_time = 120f;
        float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
        string[] refinedMetals = MATERIALS.REFINED_METALS;
        float melting_point = 1600f;
        BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tieR3, refinedMetals, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER0, none, 0.2f);
        buildingDef.AudioCategory = "Metal";
        buildingDef.RequiresPowerInput = true;
        buildingDef.EnergyConsumptionWhenActive = 240f;
        buildingDef.ExhaustKilowattsWhenActive = 0.5f;
        buildingDef.SelfHeatKilowattsWhenActive = 4f;
        buildingDef.OverheatTemperature = 363.15f;
        buildingDef.SceneLayer = Grid.SceneLayer.Building;
        buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        Prioritizable.AddRef(go);
        BuildingTemplates.CreateDefaultStorage(go, false).SetDefaultStoredItemModifiers(EggIncubatorConfig.IncubatorStorage); //do not change

        Storage storage = go.AddOrGet<Storage>();
        storage.showInUI = true;
        storage.allowItemRemoval = true;
        storage.showDescriptor = true;
        storage.storageFilters = IncubatorXConfig.EGGS; //new List<Tag>() { GameTags.Egg };
        storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
        storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
        storage.capacityKg = 20f;

        go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
        go.AddOrGet<IncubatorX>();
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        go.AddOrGet<EnergyConsumer>();
    }
}