using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Klei.AI;
using FumLib;

public class Test
{
    public void test()
    {
        // configurations
        string id = "HatchHard";
        string name = "Stone Hatch";
        string desc = "Stone Hatches excrete solid Coal as waste and enjoy burrowing into the ground.";
        string anim_file = "hatch_kanim";
        bool is_baby = false;
        string traitId = "HatchHardBaseTrait";
        string override_prefix = "hvy_";
        int space_requirement = HatchTuning.PEN_SIZE_PER_CREATURE;
        float lifespan = 100f;
        float mass = 100f;
        int width = 1;
        int height = 1;
        EffectorValues decor = TUNING.DECOR.BONUS.TIER0;
        string navGridName = !is_baby ? "WalkerNavGrid1x1" : "WalkerBabyNavGrid";
        NavType navi = NavType.Floor;
        float moveSpeed = 2f;
        string dropOnDeath = "Meat";
        int dropCount = 2;
        bool canDrown = true;
        bool canCrushed = false;
        float tempLowDeath = 243.15f;
        float tempLowWarning = 283.15f;
        float tempBorn = 293f;
        float tempHighWarning = 293.15f;
        float tempHighDeath = 343.15f;
        bool pickup_only_from_top = true;
        bool pickup_allow_mark = true;
        bool pickup_use_gun = false;
        Tag tag = GameTags.Creatures.Walker;
        Tag species = GameTags.Creatures.Species.HatchSpecies;
        float attackValue = 1f;
        ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new ExitBurrowStates.Def(), !is_baby).Add(new PlayAnimsStates.Def(GameTags.Creatures.Burrowed, true, "idle_mound", STRINGS.CREATURES.STATUSITEMS.BURROWED.NAME, STRINGS.CREATURES.STATUSITEMS.BURROWED.TOOLTIP), !is_baby).Add(new GrowUpStates.Def(), true).Add(new TrappedStates.Def(), true).Add(new IncubatingStates.Def(), true).Add(new BaggedStates.Def(), true).Add(new FallStates.Def(), true).Add(new StunnedStates.Def(), true).Add(new DrowningStates.Def(), true).Add(new DebugGoToStates.Def(), true).Add(new FleeStates.Def(), true).Add(new AttackStates.Def(), !is_baby).PushInterruptGroup().Add(new CreatureSleepStates.Def(), true).Add(new FixedCaptureStates.Def(), true).Add(new RanchedStates.Def(), true).Add(new PlayAnimsStates.Def(GameTags.Creatures.WantsToEnterBurrow, false, "hide", STRINGS.CREATURES.STATUSITEMS.BURROWING.NAME, STRINGS.CREATURES.STATUSITEMS.BURROWING.TOOLTIP), !is_baby).Add(new LayEggStates.Def(), true).Add(new EatStates.Def(), true).Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true).Add(new CallAdultStates.Def(), true).PopInterruptGroup().Add(new IdleStates.Def(), true);

        string adultId = "HatchHard";
        string eggId = "HatchHardBaby";
        string eggName = "Stone Hatchling Egg";
        string anim_egg = "egg_hatch_kanim";
        float egg_mass = 2f;
        string dropOnMature = null;
        float fertility_cycles = 60f;
        float incubation_cycles = 20f;
        int egg_sortorder = 2;
        List<FertilityMonitor.BreedingChance> egg_chances = new List<FertilityMonitor.BreedingChance>();
        egg_chances.Add(BreedingChance("HatchHardEgg", 1f));
        bool is_ranchable = true;
        bool is_fish = false;
        float egg_scale = 1f;

        float calories_per_KG = 700000f / 140f;
        float min_poop_KG = 25;
        List<Diet.Info> diet_list = BaseHatchConfig.HardRockDiet(SimHashes.Carbon.CreateTag(), calories_per_KG, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f);
        // end of configurations

        // basic generation
        object obj = Activator.CreateInstance(typeof(HatchHardConfig));
        KPrefabID obj_kPrefabID = (obj as HatchHardConfig).CreatePrefab().GetComponent<KPrefabID>();
        obj_kPrefabID.prefabInitFn += (obj as HatchHardConfig).OnPrefabInit;
        obj_kPrefabID.prefabSpawnFn += (obj as HatchHardConfig).OnSpawn;
        Assets.AddPrefab(obj_kPrefabID);

        // advanced generation
        // critter = BaseHatchConfig.BaseHatch(id, name, desc, anim_file, traitId, is_baby, override_prefix);
        GameObject critter;
        critter = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, width, height, decor, default(EffectorValues), SimHashes.Creature, null, tempBorn);
        EntityTemplates.ExtendEntityToBasicCreature(critter, FactionManager.FactionID.Pest, traitId, navGridName, navi, 32, moveSpeed, dropOnDeath, dropCount, canDrown, canCrushed, tempLowWarning, tempHighWarning, tempLowDeath, tempHighDeath);
        if (override_prefix != null) critter.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), override_prefix, null, 0);
        critter.AddOrGet<Trappable>();
        critter.AddOrGetDef<CreatureFallMonitor.Def>();
        critter.AddOrGetDef<BurrowMonitor.Def>();
        critter.AddOrGetDef<WorldSpawnableMonitor.Def>().adjustSpawnLocationCb = (int cell) => { return cell; };//new Func<int, int>(BaseHatchConfig.AdjustSpawnLocationCB);
        critter.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
        critter.AddWeapon(attackValue, attackValue, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
        EntityTemplates.CreateAndRegisterBaggedCreature(critter, pickup_only_from_top, pickup_allow_mark, pickup_use_gun);
        KPrefabID prefabID = critter.GetComponent<KPrefabID>();
        prefabID.AddTag(tag, false);
        prefabID.prefabInitFn += delegate (GameObject inst) { inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost); };
        EntityTemplates.AddCreatureBrain(critter, chore_table, species, override_prefix);

        EntityTemplates.ExtendEntityToWildCreature(critter, space_requirement, lifespan);

        Diet diet = new Diet(diet_list.ToArray());
        CreatureCalorieMonitor.Def calorieMonitor = critter.AddOrGetDef<CreatureCalorieMonitor.Def>();
        calorieMonitor.diet = diet;
        calorieMonitor.minPoopSizeInCalories = calories_per_KG * min_poop_KG;
        critter.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;

        if (is_baby)
            EntityTemplates.ExtendEntityToBeingABaby(critter, adultId, dropOnMature);
        else
            EntityTemplates.ExtendEntityToFertileCreature(critter, eggId, eggName, desc, anim_egg,
                                                          egg_mass, eggId, fertility_cycles,
                                                          incubation_cycles, egg_chances,
                                                          egg_sortorder, is_ranchable, is_fish, true, egg_scale);
    }

    public FertilityMonitor.BreedingChance BreedingChance(string tag, float weight)
    {
        return new FertilityMonitor.BreedingChance()
        {
            egg = tag.ToTag(),
            weight = weight
        };
    }

    public void test2()
    {
        var load = TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS;

        var modifiers = Db.Get().FertilityModifiers;

#pragma warning disable 219
        Diet diet = new Diet();
        GameObject gameObject = null;
        FactionManager.FactionID faction;
        string navGridName;
        Tag creatureTag;
        Tag species;
        NavType navType;
        Color lightColor = Color.blue;
#pragma warning restore 219

        //unique components
        //hatch
        navGridName = "WalkerNavGrid1x1";
        navGridName = "WalkerBabyNavGrid";
        faction = FactionManager.FactionID.Pest;
        creatureTag = GameTags.Creatures.Walker;
        species = GameTags.Creatures.Species.HatchSpecies;
        navType = NavType.Floor;
        gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
        gameObject.AddOrGetDef<BurrowMonitor.Def>();

        //lightbug
        navGridName = "FlyerNavGrid1x1";
        faction = FactionManager.FactionID.Prey;
        creatureTag = GameTags.Creatures.Flyer;
        species = GameTags.Creatures.Species.LightBugSpecies;
        navType = NavType.Hover;
        gameObject.AddOrGetDef<SubmergedMonitor.Def>();
        gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[] { GameTags.Phosphorite };

        //oilfloater
        navGridName = "FloaterNavGrid";
        faction = FactionManager.FactionID.Pest;
        creatureTag = GameTags.Creatures.Hoverer;
        species = GameTags.Creatures.Species.OilFloaterSpecies;
        navType = NavType.Hover;
        gameObject.AddOrGetDef<CreatureFallMonitor.Def>().canSwim = true;

        //drecko
        navGridName = "DreckoNavGrid";
        navGridName = "DreckoBabyNavGrid";
        faction = FactionManager.FactionID.Pest;
        creatureTag = GameTags.Creatures.Walker;
        species = GameTags.Creatures.Species.DreckoSpecies;
        navType = NavType.Floor;

        //puft
        navGridName = "FlyerNavGrid1x1";
        faction = FactionManager.FactionID.Prey;
        creatureTag = GameTags.Creatures.Flyer;
        species = GameTags.Creatures.Species.PuftSpecies;
        navType = NavType.Hover;

        //pacu
        navGridName = "SwimmerNavGrid";
        faction = FactionManager.FactionID.Prey;
        creatureTag = GameTags.SwimmingCreature;
        creatureTag = GameTags.Creatures.Swimmer;
        species = GameTags.Creatures.Species.PacuSpecies;
        navType = NavType.Swim;

        //moo
        navGridName = "FlyerNavGrid2x2";
        faction = FactionManager.FactionID.Prey;
        creatureTag = GameTags.Creatures.Flyer;
        species = GameTags.Creatures.Species.MooSpecies;
        navType = NavType.Hover;

        //mole
        navGridName = "DiggerNavGrid";
        faction = FactionManager.FactionID.Pest;
        creatureTag = GameTags.Creatures.Walker;
        species = GameTags.Creatures.Species.MoleSpecies;
        navType = NavType.Floor;
        gameObject.AddOrGetDef<DiggerMonitor.Def>().depthToDig = MoleTuning.DEPTH_TO_HIDE;

        //squirrel
        navGridName = "SquirrelNavGrid";
        navGridName = "DreckoBabyNavGrid";
        faction = FactionManager.FactionID.Pest;
        creatureTag = GameTags.Creatures.Walker;
        species = GameTags.Creatures.Species.SquirrelSpecies;
        navType = NavType.Floor;

        //crab
        navGridName = "WalkerNavGrid1x2";
        navGridName = "WalkerBabyNavGrid";
        faction = FactionManager.FactionID.Pest;
        creatureTag = GameTags.Creatures.Walker;
        species = GameTags.Creatures.Species.CrabSpecies;
        navType = NavType.Floor;

        //morb
        navGridName = "WalkerNavGrid1x1";
        faction = FactionManager.FactionID.Pest;
        creatureTag = GameTags.Creatures.Walker;
        species = GameTags.Creatures.Species.GlomSpecies;
        navType = NavType.Floor;
        gameObject.AddOrGet<DiseaseSourceVisualizer>().alwaysShowDisease = "SlimeLung";

        //sweepbot
        navGridName = "WalkerBabyNavGrid";
        faction = FactionManager.FactionID.Pest;
        creatureTag = GameTags.Creatures.Walker;
        species = GameTags.Robots.Models.SweepBot;
        navType = NavType.Floor;
        gameObject.AddComponent<Storage>();
        gameObject.AddComponent<Storage>().capacityKg = 500f;
        gameObject.AddOrGet<UserNameable>();

        //scales
        ScaleGrowthMonitor.Def def_scale = gameObject.AddOrGetDef<ScaleGrowthMonitor.Def>();
        def_scale.defaultGrowthRate = 1f / DreckoConfig.SCALE_GROWTH_TIME_IN_CYCLES / 600f;
        def_scale.dropMass = DreckoConfig.FIBER_PER_CYCLE * DreckoConfig.SCALE_GROWTH_TIME_IN_CYCLES;
        def_scale.itemDroppedOnShear = DreckoConfig.EMIT_ELEMENT;
        def_scale.levelCount = 6;
        def_scale.targetAtmosphere = SimHashes.Hydrogen;

        //gases (works also with liquids/solids)
        ElementDropperMonitor.Def def_morb = gameObject.AddOrGetDef<ElementDropperMonitor.Def>();
        def_morb.dirtyEmitElement = SimHashes.ContaminatedOxygen;
        def_morb.dirtyProbabilityPercent = 25f;
        def_morb.dirtyCellToTargetMass = 1f;
        def_morb.dirtyMassPerDirty = 0.2f;
        def_morb.dirtyMassReleaseOnDeath = 3f;
        def_morb.emitDiseaseIdx = Db.Get().Diseases.GetIndex("SlimeLung");
        def_morb.emitDiseasePerKg = 1000f;

        //light
        if (lightColor != Color.black)
        {
            Light2D light2D = gameObject.AddOrGet<Light2D>();
            light2D.Color = lightColor;
            light2D.overlayColour = TUNING.LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
            light2D.Range = 5f;
            light2D.Angle = 0f;
            light2D.Direction = TUNING.LIGHT2D.LIGHTBUG_DIRECTION;
            light2D.Offset = TUNING.LIGHT2D.LIGHTBUG_OFFSET;
            light2D.shape = LightShape.Circle;
            light2D.drawOverlay = true;
            light2D.Lux = 1800;
            gameObject.AddOrGet<LightSymbolTracker>().targetSymbol = "snapTo_light_locator";
            gameObject.AddOrGetDef<CreatureLightToggleController.Def>();
        }

        //consume solids on ground/feeder
        gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;

        //consume gas/liquid around it
        gameObject.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>().diet = diet;

        // TODO: test out Tag GameTags.Creatures.ScalesGrown and GameTags.Creatures.CanMolt
        // idea: pufts get diet for polluted oxygen, but gain no calories; requires alternative diet, like slime/dirt; OR IsHungry is always true?
    }

    /// Creates new critters which were not already loaded by EntityLoad ...
    public static void CreateNewCritters()
    {
        foreach (var setting in CustomizeCritterState.StateManager.State.critter_settings)
        {
            if (setting?.id == null) continue;

            if (Assets.Prefabs.Find(a => a.gameObject?.name == setting.id) == null)
            {
                // configurations
                string id = setting.id;
                string name = setting.name ?? "Stone Hatch";
                string desc = setting.desc ?? "Stone Hatches excrete solid Coal as waste and enjoy burrowing into the ground.";
                string anim_file = setting.anim_file ?? "hatch_kanim";
                bool is_baby = setting.is_baby ?? false;
                string traitId = setting.traitId ?? "HatchHardBaseTrait";
                string override_prefix = setting.override_prefix;
                int space_requirement = setting.space_requirement ?? HatchTuning.PEN_SIZE_PER_CREATURE;
                float lifespan = setting.lifespan ?? 100f;
                float mass = setting.mass ?? 100f;
                float width = setting.width ?? 1f;
                float height = setting.height ?? 1f;
                EffectorValues decor = setting.decor ?? TUNING.DECOR.BONUS.TIER0;
                string navGridName = setting.navGridName ?? (!is_baby ? "WalkerNavGrid1x1" : "WalkerBabyNavGrid");
                NavType navi; Enum.TryParse(setting.navi, out navi);// ?? NavType.Floor;
                float moveSpeed = setting.moveSpeed ?? 2f;
                string[] dropOnDeath = setting.dropOnDeath ?? new string[] { "Meat", "Meat" };
                bool canDrown = setting.canDrown ?? true;
                bool canCrushed = setting.canCrushed ?? false;
                float tempLowDeath = setting.tempLowDeath ?? 243.15f;
                float tempLowWarning = setting.tempLowWarning ?? 283.15f;
                float tempBorn = setting.tempBorn ?? 293f;
                float tempHighWarning = setting.tempHighWarning ?? 293.15f;
                float tempHighDeath = setting.tempHighDeath ?? 343.15f;
                bool pickup_only_from_top = setting.pickup_only_from_top ?? true;
                bool pickup_allow_mark = setting.pickup_allow_mark ?? true;
                bool pickup_use_gun = setting.pickup_use_gun ?? false;
                string[] tags = setting.tags ?? new string[] { GameTags.Creatures.Walker.ToString() };
                Tag species = setting.species ?? GameTags.Creatures.Species.HatchSpecies;
                float attackValue = setting.attackValue ?? 1f;
                ChoreTable.Builder chore_table = setting.chore_table ?? new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new ExitBurrowStates.Def(), !is_baby).Add(new PlayAnimsStates.Def(GameTags.Creatures.Burrowed, true, "idle_mound", STRINGS.CREATURES.STATUSITEMS.BURROWED.NAME, STRINGS.CREATURES.STATUSITEMS.BURROWED.TOOLTIP), !is_baby).Add(new GrowUpStates.Def(), true).Add(new TrappedStates.Def(), true).Add(new IncubatingStates.Def(), true).Add(new BaggedStates.Def(), true).Add(new FallStates.Def(), true).Add(new StunnedStates.Def(), true).Add(new DrowningStates.Def(), true).Add(new DebugGoToStates.Def(), true).Add(new FleeStates.Def(), true).Add(new AttackStates.Def(), !is_baby).PushInterruptGroup().Add(new CreatureSleepStates.Def(), true).Add(new FixedCaptureStates.Def(), true).Add(new RanchedStates.Def(), true).Add(new PlayAnimsStates.Def(GameTags.Creatures.WantsToEnterBurrow, false, "hide", STRINGS.CREATURES.STATUSITEMS.BURROWING.NAME, STRINGS.CREATURES.STATUSITEMS.BURROWING.TOOLTIP), !is_baby).Add(new LayEggStates.Def(), true).Add(new EatStates.Def(), true).Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true).Add(new CallAdultStates.Def(), true).PopInterruptGroup().Add(new IdleStates.Def(), true);

                string adultId = setting.adultId ?? "HatchHard";
                string eggId = setting.eggId ?? "HatchHardEgg";
                string babyId = setting.babyId ?? "HatchHardBaby";
                string dropOnMature = setting.dropOnMature ?? null;
                float fertility_cycles = setting.fertility_cycles ?? 60f;
                List<FertilityMonitor.BreedingChance> egg_chances = new List<FertilityMonitor.BreedingChance>();
                egg_chances.Add(BreedingChance("HatchHardEgg", 1f));
                bool is_ranchable = setting.is_ranchable ?? true;

                float calories_per_KG = setting.calories_per_KG ?? (700000f / 140f);
                float min_poop_KG = setting.min_poop_KG ?? 25f;
                List<Diet.Info> diet_list = setting.diet_list ?? BaseHatchConfig.HardRockDiet(SimHashes.Carbon.CreateTag(), calories_per_KG, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f);
                // end of configurations

                GameObject critter;
                critter = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, (int)width, (int)height, decor, default(EffectorValues), SimHashes.Creature, null, tempBorn);
                EntityTemplates.ExtendEntityToBasicCreature(critter, FactionManager.FactionID.Pest, traitId, navGridName, navi, 32, moveSpeed, "Meat", 2, canDrown, canCrushed, tempLowWarning, tempHighWarning, tempLowDeath, tempHighDeath);
                critter.AddOrGet<Butcherable>().SetDrops(dropOnDeath);
                if (override_prefix != null) critter.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), override_prefix, null, 0);
                critter.AddOrGet<Trappable>();
                critter.AddOrGetDef<CreatureFallMonitor.Def>();
                critter.AddOrGetDef<BurrowMonitor.Def>();
                critter.AddOrGetDef<WorldSpawnableMonitor.Def>().adjustSpawnLocationCb = (int cell) => { return cell; };//new Func<int, int>(BaseHatchConfig.AdjustSpawnLocationCB);
                critter.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
                if (attackValue > 0f)
                    critter.AddWeapon(attackValue, attackValue, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
                EntityTemplates.CreateAndRegisterBaggedCreature(critter, pickup_only_from_top, pickup_allow_mark, pickup_use_gun);
                KPrefabID prefabID = critter.GetComponent<KPrefabID>();
                prefabID.ProcessTags(tags);
                prefabID.prefabInitFn += delegate (GameObject inst) { inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost); };
                EntityTemplates.AddCreatureBrain(critter, chore_table, species, override_prefix);

                EntityTemplates.ExtendEntityToWildCreature(critter, space_requirement, lifespan);

                Diet diet = new Diet(diet_list.ToArray());
                CreatureCalorieMonitor.Def calorieMonitor = critter.AddOrGetDef<CreatureCalorieMonitor.Def>();
                calorieMonitor.diet = diet;
                calorieMonitor.minPoopSizeInCalories = calories_per_KG * min_poop_KG;
                critter.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;

                if (is_baby)
                    EntityTemplates.ExtendEntityToBeingABaby(critter, adultId, dropOnMature);
                else
                {
                    FertilityMonitor.Def def = critter.AddOrGetDef<FertilityMonitor.Def>();
                    def.baseFertileCycles = fertility_cycles;
                    def.eggPrefab = new Tag(eggId);
                    def.initialBreedingWeights = egg_chances;
                    KPrefabID creature_prefab_id = critter.GetComponent<KPrefabID>();
                    creature_prefab_id.prefabSpawnFn += delegate (GameObject inst)
                    {
                        WorldInventory.Instance.Discover(eggId.ToTag(), WorldInventory.GetCategoryForTags(egg_tags));
                        WorldInventory.Instance.Discover(babyId.ToTag(), WorldInventory.GetCategoryForTags(creature_prefab_id.Tags));
                    };
                    if (is_ranchable)
                    {
                        critter.AddOrGetDef<RanchableMonitor.Def>();
                    }
                    critter.AddOrGetDef<FixedCapturableMonitor.Def>();
                }

                if (setting.scales != null)
                {
                    ScaleGrowthMonitor.Def def_scale = critter.AddOrGetDef<ScaleGrowthMonitor.Def>();
                    def_scale.defaultGrowthRate = setting.scales.growthRate ?? (setting.scales.growthRate ?? (1f / DreckoConfig.SCALE_GROWTH_TIME_IN_CYCLES / 600f));
                    def_scale.dropMass = setting.scales.mass ?? (DreckoConfig.FIBER_PER_CYCLE * DreckoConfig.SCALE_GROWTH_TIME_IN_CYCLES);
                    def_scale.itemDroppedOnShear = setting.scales.drop ?? "Dirt";
                    def_scale.levelCount = setting.scales.levelCount ?? 6;
                    def_scale.targetAtmosphere = (setting.scales.atmosphere ?? "Void").ToSimHash();
                }

                if (setting.expulsion != null)
                {
                    ElementDropperMonitor.Def def_morb = critter.AddOrGetDef<ElementDropperMonitor.Def>();
                    def_morb.dirtyEmitElement = (setting.expulsion.element ?? "Dirt").ToSimHash();
                    def_morb.dirtyProbabilityPercent = setting.expulsion.probability ?? 25f;
                    def_morb.dirtyCellToTargetMass = setting.expulsion.cellTargetMass ?? 1f;
                    def_morb.dirtyMassPerDirty = setting.expulsion.massPerDirt ?? 0.2f;
                    def_morb.dirtyMassReleaseOnDeath = setting.expulsion.onDeath ?? 3f;
                    def_morb.emitDiseaseIdx = Db.Get().Diseases.GetIndex(setting.expulsion.diseaseId ?? "");
                    def_morb.emitDiseasePerKg = setting.expulsion.diseaseAmount ?? 0f;
                }

                if (setting.light != null)
                {
                    if (setting.light.color != Color.black)
                    {
                        Light2D light2D = critter.AddOrGet<Light2D>();
                        light2D.Color = setting.light.color ?? Color.white;
                        light2D.overlayColour = TUNING.LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
                        light2D.Range = setting.light.range ?? 5f;
                        light2D.Angle = 0f;
                        light2D.Direction = TUNING.LIGHT2D.LIGHTBUG_DIRECTION;
                        light2D.Offset = TUNING.LIGHT2D.LIGHTBUG_OFFSET;
                        light2D.shape = LightShape.Circle;
                        light2D.drawOverlay = true;
                        light2D.Lux = setting.light.lux ?? 1800;
                        critter.AddOrGet<LightSymbolTracker>().targetSymbol = "snapTo_light_locator";
                        critter.AddOrGetDef<CreatureLightToggleController.Def>();
                    }
                }

            }
        }
    }

    public static void CreateNewEggs()
    {
        foreach (var setting in CustomizeCritterState.StateManager.State.egg_settings)
        {
            if (setting?.eggId == null) continue;    //eggId isn't allowed to be null

            if (Assets.Prefabs.Find(a => a.gameObject?.name == setting.eggId) == null)
            {
                if (CustomizeCritterState.StateManager.State.debug)
                    Debug.Log("[CustomizeCritter] Adding new egg type: " + setting.eggId);
                // configurations
                string eggId = setting.eggId;
                string eggName = setting.eggName ?? "No Name";
                string eggDesc = setting.eggDesc ?? "No Description";
                string egg_anim = setting.anim_egg ?? "egg_hatch_kanim";
                string baby_id = setting.babyId ?? "HatchHardBaby";
                float egg_mass = setting.egg_mass ?? 2f;
                int eggSortOrder = setting.egg_sortorder ?? 0;
                float base_incubation_rate = 100f / (600f * setting.incubation_cycles ?? 20f);
                float egg_anim_scale = setting.egg_scale ?? 1f;
                bool is_fish = setting.is_fish ?? false;
                string symbolPrefix = setting.override_prefix;
                // end of configurations

                GameObject egg = EggConfig.CreateEgg(eggId, eggName, eggDesc, baby_id, egg_anim, egg_mass, eggSortOrder, base_incubation_rate);

                if (egg_anim_scale != 1f)
                {
                    KBatchedAnimController component = egg.GetComponent<KBatchedAnimController>();
                    component.animWidth = egg_anim_scale;
                    component.animHeight = egg_anim_scale;
                }

                if (is_fish)
                    egg.AddOrGetDef<FishOvercrowdingMonitor.Def>();

                if (!string.IsNullOrEmpty(symbolPrefix))
                    SymbolOverrideControllerUtil.AddToPrefab(egg).ApplySymbolOverridesByAffix(Assets.GetAnim(egg_anim), symbolPrefix, null, 0);
            }
        }
    }



}



//[HarmonyPatch(typeof(Assets), nameof(Assets.AddPrefab))]
public class Patch_EntityLoad
{
    public static void Prefix(KPrefabID prefab)
    {
        GameObject parent = prefab?.gameObject;

        if (parent == null) return;

        if (CustomizeCritterState.StateManager.State.debug)
            Debug.Log("Loading GameObject: " + parent.name);

        PatchCritters(prefab, parent);
        PatchEggs(prefab, parent);
    }

    public static HashSet<Tag> egg_tags = new HashSet<Tag>() { GameTags.Egg, GameTags.IncubatableEgg, GameTags.PedestalDisplayable };

    /// Modifies critters as they are loaded
    /// missing parameters: is_baby, override_prefix, lifespan, diet_list
    /// babyId: only reveals material, actually object is defined in egg
    public static void PatchCritters(KPrefabID prefab, GameObject critter)
    {
        CritterContainer setting = CustomizeCritterState.StateManager.State.critter_settings.Find(s => s.id == critter.name);
        if (setting == null) return;

        Debug.Log("DEBUG HELLO 1");

        if (setting.name != null)
        {
            prefab.PrefabTag = TagManager.Create(setting.id, setting.name);
            critter.GetComponent<KSelectable>()?.SetName(setting.name);
            critter.AddOrGet<DecorProvider>().overrideName = setting.name;
        }

        Debug.Log("DEBUG HELLO 2");

        if (setting.desc != null)
        {
            critter.AddOrGet<InfoDescription>().description = setting.desc;
        }

        Debug.Log("DEBUG HELLO 3");

        if (setting.anim_file != null)
        {
            KBatchedAnimController kbatchedAnimController = critter.AddOrGet<KBatchedAnimController>();
            kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim(setting.anim_file) };
            //kbatchedAnimController.sceneLayer = sceneLayer;
            //kbatchedAnimController.initialAnim = initialAnim;
        }

        Debug.Log("DEBUG HELLO 4");

        if (setting.is_baby != null)
        {
            Debug.Log(setting.id + ".is_baby: This cannot be changed on existing critters.");
        }

        Debug.Log("DEBUG HELLO 5");

        if (setting.traitId != null)
        {
            Modifiers modifiers = critter.AddOrGet<Modifiers>();
            if (setting.traitId != "")
                modifiers.initialTraits = new string[] { setting.traitId };
            else
                modifiers.initialTraits = null;
            //modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
        }

        Debug.Log("DEBUG HELLO 6");

        if (setting.override_prefix != null)
        {
            Debug.Log(setting.id + ".override_prefix: This cannot be changed on existing critters.");
        }

        Debug.Log("DEBUG HELLO 7");

        if (setting.space_requirement != null)
        {
            critter.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = setting.space_requirement.Value;
        }

        Debug.Log("DEBUG HELLO 8");

        if (setting.lifespan != null)
        {
            Debug.Log(setting.id + ".lifespan: This value is not read from the game. It exists only as artifact. Use traits instead!");
            //trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name, false, false, true));
        }

        Debug.Log("DEBUG HELLO 9");

        if (setting.mass != null)
        {
            critter.AddOrGet<PrimaryElement>().Mass = setting.mass.Value;
        }

        Debug.Log("DEBUG HELLO 10");

        if (setting.width != null || setting.height != null)
        {
            float width = setting.width ?? 1f;
            float height = setting.height ?? 1f;
            KBoxCollider2D kboxCollider2D = critter.AddOrGet<KBoxCollider2D>();
            kboxCollider2D.size = new Vector2f(width, height);
            float num = 0.5f * (float)((width + 1) % 2);
            kboxCollider2D.offset = new Vector2f(num, (float)height / 2f);
            critter.GetComponent<KBatchedAnimController>().Offset = new Vector3(num, 0f, 0f);
            critter.AddOrGet<OccupyArea>().OccupiedCellsOffsets = EntityTemplates.GenerateOffsets((int)Math.Round(width, 0), (int)Math.Round(height, 0));
        }

        Debug.Log("DEBUG HELLO 11");

        if (setting.decor != null)
        {
            DecorProvider decorProvider = critter.AddOrGet<DecorProvider>();
            decorProvider.SetValues(setting.decor.Value);
        }

        Debug.Log("DEBUG HELLO 12");

        if (setting.navGridName != null)
        {
            critter.AddOrGet<Navigator>().NavGridName = setting.navGridName;
        }

        Debug.Log("DEBUG HELLO 13");

        if (setting.navi != null)
        {
            critter.AddOrGet<Navigator>().CurrentNavType = setting.navi.Value;
        }

        Debug.Log("DEBUG HELLO 14");

        if (setting.moveSpeed != null)
        {
            critter.AddOrGet<Navigator>().defaultSpeed = setting.moveSpeed.Value;
        }

        Debug.Log("DEBUG HELLO 15");

        if (setting.dropOnDeath != null)
        {
            critter.AddOrGet<Butcherable>().SetDrops(setting.dropOnDeath);
        }

        Debug.Log("DEBUG HELLO 16");

        if (setting.canDrown != null)
        {
            if (setting.canDrown.Value)
                critter.AddOrGet<DrowningMonitor>();
            else
                critter.RemoveComponents<DrowningMonitor>();
        }

        Debug.Log("DEBUG HELLO 17");

        if (setting.canCrushed != null)
        {
            if (setting.canCrushed.Value)
                critter.AddOrGet<EntombVulnerable>();
            else
                critter.RemoveComponents<EntombVulnerable>();
        }

        Debug.Log("DEBUG HELLO 18");

        if (setting.tempLowDeath != null)
        {
            critter.AddOrGet<TemperatureVulnerable>().internalTemperatureLethal_Low = setting.tempLowDeath.Value;
        }

        Debug.Log("DEBUG HELLO 19");

        if (setting.tempLowWarning != null)
        {
            critter.AddOrGet<TemperatureVulnerable>().internalTemperatureWarning_Low = setting.tempLowWarning.Value;
        }

        Debug.Log("DEBUG HELLO 20");

        if (setting.tempHighWarning != null)
        {
            critter.AddOrGet<TemperatureVulnerable>().internalTemperatureWarning_High = setting.tempHighWarning.Value;
        }

        Debug.Log("DEBUG HELLO 21");

        if (setting.tempHighDeath != null)
        {
            critter.AddOrGet<TemperatureVulnerable>().internalTemperatureLethal_High = setting.tempHighDeath.Value;
        }

        Debug.Log("DEBUG HELLO 22");

        if (setting.tempBorn != null)
        {
            critter.AddOrGet<PrimaryElement>().Temperature = setting.tempBorn.Value;
        }

        Debug.Log("DEBUG HELLO 23");

        if (setting.pickup_only_from_top != null)
        {
            critter.AddOrGet<Baggable>().mustStandOntopOfTrapForPickup = setting.pickup_only_from_top.Value;
        }

        Debug.Log("DEBUG HELLO 24");

        if (setting.pickup_allow_mark != null)
        {
            critter.AddOrGet<Capturable>().allowCapture = setting.pickup_allow_mark.Value;
        }

        Debug.Log("DEBUG HELLO 25");

        if (setting.pickup_use_gun != null)
        {
            critter.AddOrGet<Baggable>().useGunForPickup = setting.pickup_use_gun.Value;
        }

        Debug.Log("DEBUG HELLO 26");

        if (setting.tags != null)
        {
            prefab.ProcessTags(setting.tags);
        }

        Debug.Log("DEBUG HELLO 27");

        if (setting.species != null)
        {
            critter.AddOrGet<CreatureBrain>().species = setting.species;
        }

        Debug.Log("DEBUG HELLO 28");

        if (setting.attackValue != null)
        {
            if (setting.attackValue.Value > 0f)
                critter.AddWeapon(setting.attackValue.Value, setting.attackValue.Value, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
            else
                critter.RemoveComponents<Weapon>();
        }

        Debug.Log("DEBUG HELLO 29");

        if (setting.chore_table != null)
        {
            ChoreConsumer chore_consumer = critter.AddOrGet<ChoreConsumer>();
            chore_consumer.choreTable = setting.chore_table.CreateTable();
            // prefab.instantiateFn += delegate(GameObject go) { go.GetComponent<ChoreConsumer>().choreTable = chore_consumer.choreTable; };
        }

        Debug.Log("DEBUG HELLO 30");

        if (setting.adultId != null)
        {
            critter.AddOrGetDef<BabyMonitor.Def>().adultPrefab = setting.adultId;
        }

        Debug.Log("DEBUG HELLO 31");

        if (setting.dropOnMature != null)
        {
            critter.AddOrGetDef<BabyMonitor.Def>().onGrowDropID = setting.dropOnMature;
        }

        Debug.Log("DEBUG HELLO 32");

        if (setting.fertility_cycles != null)
        {
            critter.AddOrGetDef<FertilityMonitor.Def>().baseFertileCycles = setting.fertility_cycles.Value;
        }

        Debug.Log("DEBUG HELLO 33");

        if (setting.egg_chances != null)
        {
            critter.AddOrGetDef<FertilityMonitor.Def>().initialBreedingWeights = setting.egg_chances;
        }

        Debug.Log("DEBUG HELLO 34");

        if (setting.is_ranchable != null)
        {
            if (setting.is_ranchable.Value)
                critter.AddOrGetDef<RanchableMonitor.Def>();
            else
                critter.RemoveDef<RanchableMonitor.Def>();
        }

        Debug.Log("DEBUG HELLO 35");

        if (setting.calories_per_KG != null)
        {
            critter.AddOrGetDef<CreatureCalorieMonitor.Def>();
        }

        Debug.Log("DEBUG HELLO 36");

        if (setting.min_poop_KG != null)
        {
            critter.AddOrGetDef<CreatureCalorieMonitor.Def>().minPoopSizeInCalories = setting.min_poop_KG.Value * setting.calories_per_KG ?? 1f;
        }

        Debug.Log("DEBUG HELLO 37");

        if (setting.diet_list != null)
        {
            //Diet diet = new Diet(setting.diet_list.ToArray());
            //critter.AddOrGetDef<CreatureCalorieMonitor.Def>().diet = diet;
            //critter.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
        }

        Debug.Log("DEBUG HELLO 38");

        if (setting.eggId != null)
        {
            critter.AddOrGetDef<FertilityMonitor.Def>().eggPrefab = setting.eggId;
            prefab.prefabSpawnFn += delegate (GameObject inst)
            {
                WorldInventory.Instance.Discover(setting.eggId.ToTag(), WorldInventory.GetCategoryForTags(egg_tags));
            };
        }

        Debug.Log("DEBUG HELLO 39");

        if (setting.babyId != null)
        {
            prefab.prefabSpawnFn += delegate (GameObject inst)
            {
                WorldInventory.Instance.Discover(setting.babyId.ToTag(), WorldInventory.GetCategoryForTags(prefab.Tags));
            };
        }

        Debug.Log("DEBUG HELLO 40");

        if (setting.light != null)
        {
            if (setting.light.color != Color.black)
            {
                Light2D light2D = critter.AddOrGet<Light2D>();
                if (setting.light.color != null)
                    light2D.Color = setting.light.color.Value;
                light2D.overlayColour = TUNING.LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
                if (setting.light.range != null)
                    light2D.Range = setting.light.range.Value;
                light2D.Angle = 0f;
                light2D.Direction = TUNING.LIGHT2D.LIGHTBUG_DIRECTION;
                light2D.Offset = TUNING.LIGHT2D.LIGHTBUG_OFFSET;
                light2D.shape = LightShape.Circle;
                light2D.drawOverlay = true;
                if (setting.light.lux != null)
                    light2D.Lux = setting.light.lux.Value;
                critter.AddOrGet<LightSymbolTracker>().targetSymbol = "snapTo_light_locator";
                critter.AddOrGetDef<CreatureLightToggleController.Def>();
            }
            else
            {
                critter.RemoveComponents<Light2D>();
                critter.RemoveComponents<LightSymbolTracker>();
                critter.RemoveDef<CreatureLightToggleController.Def>();
            }
        }

        Debug.Log("DEBUG HELLO 41");

        if (setting.expulsion != null)  //works also with gasses/liquids/solids
        {
            ElementDropperMonitor.Def def_morb = critter.AddOrGetDef<ElementDropperMonitor.Def>();
            if (setting.expulsion.element != null)
                def_morb.dirtyEmitElement = setting.expulsion.element.ToSimHash();
            if (setting.expulsion.probability != null)
                def_morb.dirtyProbabilityPercent = setting.expulsion.probability.Value;
            if (setting.expulsion.cellTargetMass != null)
                def_morb.dirtyCellToTargetMass = setting.expulsion.cellTargetMass.Value;
            if (setting.expulsion.massPerDirt != null)
                def_morb.dirtyMassPerDirty = setting.expulsion.massPerDirt.Value;
            if (setting.expulsion.onDeath != null)
                def_morb.dirtyMassReleaseOnDeath = setting.expulsion.onDeath.Value;
            if (setting.expulsion.diseaseId != null)
                def_morb.emitDiseaseIdx = Db.Get().Diseases.GetIndex(setting.expulsion.diseaseId);
            if (setting.expulsion.diseaseAmount != null)
                def_morb.emitDiseasePerKg = setting.expulsion.diseaseAmount.Value;
        }

        Debug.Log("DEBUG HELLO 42");

        if (setting.scales != null)
        {
            ScaleGrowthMonitor.Def def_scale = critter.AddOrGetDef<ScaleGrowthMonitor.Def>();
            if (setting.scales.growthRate != null)
                def_scale.defaultGrowthRate = setting.scales.growthRate.Value;
            if (setting.scales.mass != null)
                def_scale.dropMass = setting.scales.mass.Value;
            if (setting.scales.drop != null)
                def_scale.itemDroppedOnShear = setting.scales.drop;
            if (setting.scales.levelCount != null)
                def_scale.levelCount = setting.scales.levelCount.Value;
            if (setting.scales.atmosphere != null)
                def_scale.targetAtmosphere = setting.scales.atmosphere.ToSimHash();
        }

        Debug.Log("DEBUG HELLO 43");
    }

    /// Modifies eggs as they are loaded
    /// missing parameters: override_prefix
    /// egg_mass: recipe is fixed in EntityFinish
    public static void PatchEggs(KPrefabID prefab, GameObject egg)
    {
        EggContainer setting = CustomizeCritterState.StateManager.State.egg_settings.Find(s => s.eggId == egg.name);
        if (setting == null) return;

        if (setting.eggName != null)
        {
            prefab.PrefabTag = TagManager.Create(setting.eggId, setting.eggName);
            egg.GetComponent<KSelectable>()?.SetName(setting.eggName);
            egg.AddOrGet<DecorProvider>().overrideName = setting.eggName;
        }

        if (setting.eggDesc != null)
        {
            egg.AddOrGet<InfoDescription>().description = setting.eggDesc;
        }

        if (setting.override_prefix != null && setting.anim_egg != null)
        {
            // THIS CODE ONLY WORKS ON CREATURES, NOT EGGS!
            //SymbolOverrideController symbol_override_controller = SymbolOverrideControllerUtil.AddToPrefab(egg);
            //string symbolPrefix = prefab.GetComponent<CreatureBrain>().symbolPrefix;
            //if (!string.IsNullOrEmpty(symbolPrefix))
            //{
            //    symbol_override_controller.ApplySymbolOverridesByAffix(Assets.GetAnim(setting.anim_egg), symbolPrefix, null, 0);
            //}
        }

        if (setting.anim_egg != null)
        {
            KBatchedAnimController kbatchedAnimController = egg.AddOrGet<KBatchedAnimController>();
            kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim(setting.anim_egg) };
            //kbatchedAnimController.sceneLayer = sceneLayer;
            //kbatchedAnimController.initialAnim = initialAnim;
        }

        if (setting.egg_mass != null)
        {
            egg.AddOrGet<PrimaryElement>().MassPerUnit = setting.egg_mass.Value;
        }

        if (setting.babyId != null)
        {
            egg.AddOrGetDef<IncubationMonitor.Def>().spawnedCreature = setting.babyId;
        }

        if (setting.incubation_cycles != null)
        {
            float base_incubation_rate = 100f / (600f * setting.incubation_cycles.Value);
            egg.AddOrGetDef<IncubationMonitor.Def>().baseIncubationRate = base_incubation_rate;
        }

        if (setting.egg_sortorder != null)
        {
            egg.AddOrGet<Pickupable>().sortOrder = TUNING.SORTORDER.EGGS + setting.egg_sortorder.Value;
        }

        if (setting.is_fish != null)
        {
            if (setting.is_fish.Value)
                egg.AddOrGetDef<FishOvercrowdingMonitor.Def>();
            else
                egg.RemoveDef<FishOvercrowdingMonitor.Def>();
        }

        if (setting.egg_scale != null)
        {
            KBatchedAnimController component = egg.GetComponent<KBatchedAnimController>();
            component.animWidth = setting.egg_scale.Value;
            component.animHeight = setting.egg_scale.Value;
        }
    }
}
