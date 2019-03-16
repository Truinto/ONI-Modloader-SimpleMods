
using TUNING;
using UnityEngine;

public class LiquidReservoirSmartConfig : IBuildingConfig
{
    public const string ID = "LiquidReservoirSmart";
    private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;
    private const int WIDTH = 2; //redundant?
    private const int HEIGHT = 3; //redundant?
    private static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_DESC, true);

    public override BuildingDef CreateBuildingDef()
    {
        string id = "LiquidReservoirSmart";
        int width = 2;
        int height = 3;
        string anim = "liquidreservoir_kanim";
        int hitpoints = 100;
        float construction_time = 120f;
        float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
        string[] allMetals = MATERIALS.ALL_METALS;
        float melting_point = 800f;
        BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
        EffectorValues none = TUNING.NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tieR4, allMetals, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
        buildingDef.InputConduitType = ConduitType.Liquid;
        buildingDef.OutputConduitType = ConduitType.Liquid;
        buildingDef.Floodable = false;
        buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
        buildingDef.AudioCategory = "HollowMetal";
        buildingDef.UtilityInputOffset = new CellOffset(1, 2);
        buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
        buildingDef.PermittedRotations = PermittedRotations.R360;
        return buildingDef;
    }

    /*public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
    }*/

    public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
    {
        GeneratedBuildings.RegisterLogicPorts(go, LiquidReservoirSmartConfig.OUTPUT_PORT);
    }

    public override void DoPostConfigureUnderConstruction(GameObject go)
    {
        GeneratedBuildings.RegisterLogicPorts(go, LiquidReservoirSmartConfig.OUTPUT_PORT);
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        GeneratedBuildings.RegisterLogicPorts(go, LiquidReservoirSmartConfig.OUTPUT_PORT);
        SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", TUNING.NOISE_POLLUTION.NOISY.TIER1);
        Prioritizable.AddRef(go);

        Storage storage = go.AddOrGet<Storage>();
        storage.showInUI = true;
        storage.allowItemRemoval = true;
        storage.showDescriptor = true;
        storage.storageFilters = STORAGEFILTERS.LIQUIDS;
        storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
        storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
        storage.capacityKg = 100000f;
        storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);

        ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
        conduitConsumer.conduitType = ConduitType.Liquid;
        conduitConsumer.ignoreMinMassCheck = true;
        conduitConsumer.forceAlwaysSatisfied = true;
        conduitConsumer.alwaysConsume = true;
        conduitConsumer.capacityKG = storage.capacityKg;
        ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
        conduitDispenser.conduitType = ConduitType.Liquid;
        conduitDispenser.elementFilter = (SimHashes[])null;

        go.AddOrGet<LiquidReservoirSmart>();
        go.AddOrGetDef<StorageController.Def>();    //plays animations
    }
}
