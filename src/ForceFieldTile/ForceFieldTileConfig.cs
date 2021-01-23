using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ForceFieldTileConfig : IBuildingConfig
{
    public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_solid_tops");
    public const string ID = "ForceFieldTile";

    public override BuildingDef CreateBuildingDef()
    {
        string id = "ForceFieldTile";
        int width = 1;
        int height = 1;
        string anim = "ladder_poi_kanim"; //floor_basic_kanim //floor_plastic_kanim
        int hitpoints = 100;
        float construction_time = 3f;
        float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
        string[] rawMinerals = MATERIALS.RAW_MINERALS;
        float melting_point = 1600f;
        BuildLocationRule build_location_rule = BuildLocationRule.Tile;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tieR3, rawMinerals, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER0, none, 0.2f);
        buildingDef.Floodable = false;
        buildingDef.Overheatable = false;
        buildingDef.Entombable = false;
        buildingDef.UseStructureTemperature = false;
        buildingDef.IsFoundation = true;
        buildingDef.TileLayer = ObjectLayer.FoundationTile;
        buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
        buildingDef.AudioCategory = "Metal";
        buildingDef.AudioSize = "small";
        buildingDef.BaseTimeUntilRepair = -1f;
        buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
        buildingDef.isKAnimTile = true;
        buildingDef.isSolidTile = true;
        buildingDef.BlockTileAtlas = Assets.GetTextureAtlas("tiles_solid");
        buildingDef.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_solid_place");
        buildingDef.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
        buildingDef.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_solid_tops_info");
        buildingDef.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_solid_tops_place_info");
        buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
        buildingDef.ReplacementTags = new List<Tag>();
        buildingDef.ReplacementTags.Add(GameTags.FloorTiles);
        buildingDef.DragBuild = true;
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        GeneratedBuildings.MakeBuildingAlwaysOperational(go);
        BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
        SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
        simCellOccupier.doReplaceElement = true;
        simCellOccupier.strengthMultiplier = 1.5f;
        simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.BONUS_2;
        simCellOccupier.notifyOnMelt = true;
        go.AddOrGet<ForceFieldTile>();
        //go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = TileConfig.BlockTileConnectorID;
        go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        GeneratedBuildings.RemoveLoopingSounds(go);
        go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);
    }

    public override void DoPostConfigureUnderConstruction(GameObject go)
    {
        base.DoPostConfigureUnderConstruction(go);
        //go.AddOrGet<KAnimGridTileVisualizer>();
    }
}