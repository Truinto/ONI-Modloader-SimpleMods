using TUNING;
using UnityEngine;

public class AutomaticCompostConfig : IBuildingConfig
{
    public static readonly Tag COMPOST_TAG = GameTags.Compostable;
    public const string ID = "AutomaticCompost";
    public const float POWER_CONSUMPTION = 240f;
    public const float COMPOST_PER_SECOND = 400f / 600f; //0.1f;
    public const float DIRT_OUTPUT_TEMP = 348.15f;
    public const float INPUT_CAPACITY = 360f;
    private const SimHashes OUTPUT_ELEMENT = SimHashes.Dirt;

    public override BuildingDef CreateBuildingDef()
    {
        string id = ID;
        int width = 2;
        int height = 2;
        string anim = "compost_kanim";
        int hitpoints = 30;
        float construction_time = 30f;
        float[] tieR5 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
        string[] rawMinerals = MATERIALS.RAW_MINERALS;
        float melting_point = 800f;
        BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tieR5, rawMinerals, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER3, none, 0.2f);
        buildingDef.AudioCategory = "HollowMetal";
        buildingDef.ExhaustKilowattsWhenActive = 0.5f;
        buildingDef.SelfHeatKilowattsWhenActive = 4f;
        buildingDef.Overheatable = false;
        buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
        buildingDef.RequiresPowerInput = true;
        buildingDef.EnergyConsumptionWhenActive = POWER_CONSUMPTION;
        buildingDef.PowerInputOffset = new CellOffset(0, 0);
        buildingDef.InputConduitType = ConduitType.Solid;
        //buildingDef.OutputConduitType = ConduitType.Solid;
        buildingDef.UtilityInputOffset = new CellOffset(0, 0);
        //buildingDef.UtilityOutputOffset = new CellOffset(1, 1);
        //SoundEventVolumeCache.instance.AddVolume("anim_interacts_compost_kanim", "Compost_shovel_in", NOISE_POLLUTION.NOISY.TIER2);
        //SoundEventVolumeCache.instance.AddVolume("anim_interacts_compost_kanim", "Compost_shovel_out", NOISE_POLLUTION.NOISY.TIER2);
        //GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, this.ID);
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        Storage storage = go.AddOrGet<Storage>();
        storage.capacityKg = 650f;
        storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);

        ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
        manualDeliveryKg.SetStorage(storage);
        manualDeliveryKg.requestedItemTag = AutomaticCompostConfig.COMPOST_TAG;
        manualDeliveryKg.capacity = INPUT_CAPACITY;
        manualDeliveryKg.refillMass = 60f;
        manualDeliveryKg.minimumMass = 60f;
        manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FarmFetch.IdHash;

        ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
        elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
        {
            new ElementConverter.ConsumedElement(AutomaticCompostConfig.COMPOST_TAG, COMPOST_PER_SECOND)
        };
        elementConverter.outputElements = new ElementConverter.OutputElement[1] 
        {
            new ElementConverter.OutputElement(COMPOST_PER_SECOND, SimHashes.Dirt, 348.15f, false, true, 0.0f, 0.5f, 1f, byte.MaxValue, 0)
        };

        ElementDropper elementDropper = go.AddComponent<ElementDropper>();
        elementDropper.emitMass = 300f;
        elementDropper.emitTag = SimHashes.Dirt.CreateTag();
        elementDropper.emitOffset = new Vector3(0.5f, 1f, 0.0f);

        go.AddOrGet<SolidConduitConsumer>();
        //go.AddOrGet<SolidConduitDispenser>();

        go.AddOrGet<Operational>();

        Prioritizable.AddRef(go);
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        //GeneratedBuildings.MakeBuildingAlwaysOperational(go);
    }
}