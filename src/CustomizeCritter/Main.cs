//#define DLC1

using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Klei.AI;
using FumLib;

namespace CustomizeCritter
{
    //[HarmonyPatch(typeof(SweepBotConfig), "OnSpawn")]
    public class test
    {
        public static bool Prefix(GameObject inst)
        {
            StorageUnloadMonitor.Instance smi = inst.GetSMI<StorageUnloadMonitor.Instance>();
            smi.sm.internalStorage.Set(inst.GetComponents<Storage>()[1], smi);
            inst.GetComponent<OrnamentReceptacle>();
            //inst.GetSMI<CreatureFallMonitor.Instance>().anim = "idle_loop";
            return false;
        }
    }

    [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.AddCreatureBrain))]
    public class Patch_AddCreatureBrain
    {
        // remember all the ChoreTable.Builder, since they are not preserved normally
        public static Dictionary<string, ChoreTable.Builder> CreatureBrains = new Dictionary<string, ChoreTable.Builder>();

        public static void Prefix(GameObject prefab, ChoreTable.Builder chore_table)
        {
            if (CreatureBrains.ContainsKey(prefab.name))
            {
                Debug.Log("[CustomizeCritter] CreatureBrain was added twice? " + prefab.name);
            }
            else
            {
                CreatureBrains[prefab.name] = chore_table;
            }
        }
    }

    [HarmonyPatch(typeof(EntityConfigManager), nameof(EntityConfigManager.LoadGeneratedEntities))]
    [HarmonyPriority(Priority.VeryLow)]
    public class Patch_EntityFinish
    {
        public static void Postfix()
        {
            PrintEntitiesToSettings();

            ChangeTraits();
            ChangeEggs();
            ChangeCritters();
            FixEggRecipies();

            if (CustomizeCritterState.StateManager.State.acceleratedLifecycle)
                AccessTools.Property(typeof(Klei.GenericGameSettings), nameof(Klei.GenericGameSettings.acceleratedLifecycle)).SetValue(Klei.GenericGameSettings.instance, true, null);

#if DEBUG
            if (CustomizeCritterState.StateManager.State.debug)
            {
                Debug.Log("Prefabs list:");
                foreach (var prefab in Assets.Prefabs)
                {
                    GameObject go = prefab.gameObject;
                    if (go != null)
                    {
                        Debug.Log($"prefab.name={prefab.name}, GetProperName={go.GetProperName()}, GetComponents={go.GetComponents(typeof(Component)).Select(x => x.ToString()).Join()}");
                    }
                    else
                    {
                        Debug.Log("go is null for: " + prefab.name);
                    }
                }
            }
#endif

            //var sweepbot = Assets.Prefabs.Find(s => s.name == "SweepBot");
            //Debug.Log("HELLO SweepBot Tags: " + sweepbot.Tags.Select(x => x.ToString()).Join());

        }

        /// Extracts all creatures, eggs, and traits into the settings file
        /// critter missing parameters: lifespan, babyId
        /// egg missing parameters: override_prefix
        public static void PrintEntitiesToSettings()
        {
            if (CustomizeCritterState.StateManager.State.print_all)
            {
                CustomizeCritterState.StateManager.State.critter_settings.Clear();
                CustomizeCritterState.StateManager.State.egg_settings.Clear();
                CustomizeCritterState.StateManager.State.trait_settings.Clear();
                CustomizeCritterState.StateManager.State.egg_modifiers.Clear();

                foreach (var prefab in Assets.Prefabs)
                {
                    GameObject go = prefab.gameObject;

                    if (go.GetComponent<CreatureBrain>() != null)   // is creature
                    {
                        Debug.Log("Printing creature: " + prefab.name);
                        var container = new CritterContainer();

                        container.id = go.name;
                        container.name = go.GetProperName();
                        container.desc = go.GetComponent<InfoDescription>()?.description;
                        container.anim_file = go.GetComponent<KBatchedAnimController>()?.AnimFiles?[0]?.name;
                        container.is_baby = go.GetDef<BabyMonitor.Def>() != null;
                        container.is_adult = go.GetDef<FertilityMonitor.Def>() != null;
                        container.traitId = go.GetComponent<Modifiers>()?.initialTraits?[0];
                        container.override_prefix = go.GetComponent<CreatureBrain>()?.symbolPrefix;
                        container.space_requirement = go.GetDef<OvercrowdingMonitor.Def>()?.spaceRequiredPerCreature;
                        container.lifespan = go.GetDef<AgeMonitor.Def>() != null ? null : new float?(-1f);
                        container.mass = go.GetComponent<PrimaryElement>()?.Mass;
                        container.width = go.GetComponent<KBoxCollider2D>()?.size.x;
                        container.height = go.GetComponent<KBoxCollider2D>()?.size.y;
                        var decor = go.GetComponent<DecorProvider>(); if (decor != null) container.decor = new EffectorValues((int)decor.baseDecor, (int)decor.baseRadius);
                        container.navGridName = go.GetComponent<Navigator>()?.NavGridName;
                        container.navi = go.GetComponent<Navigator>()?.CurrentNavType.ToString();
                        container.moveSpeed = go.GetComponent<Navigator>()?.defaultSpeed;
                        container.dropOnDeath = go.GetComponent<Butcherable>()?.Drops;
                        container.canDrown = go.GetComponent<DrowningMonitor>() != null;
                        container.canCrushed = go.GetComponent<EntombVulnerable>() != null;
                        container.canBurrow = go.GetDef<BurrowMonitor.Def>() != null;
                        container.canTunnel = go.GetDef<DiggerMonitor.Def>() != null;
                        container.canFall = go.GetDef<CreatureFallMonitor.Def>() != null;
                        container.canHoverOverWater = go.GetDef<CreatureFallMonitor.Def>()?.canSwim ?? false;
                        container.tempLowDeath = go.GetComponent<TemperatureVulnerable>()?.internalTemperatureLethal_Low;
                        container.tempLowWarning = go.GetComponent<TemperatureVulnerable>()?.internalTemperatureWarning_Low;
                        container.tempBorn = go.GetComponent<PrimaryElement>()?.Temperature;
                        container.tempHighWarning = go.GetComponent<TemperatureVulnerable>()?.internalTemperatureWarning_High;
                        container.tempHighDeath = go.GetComponent<TemperatureVulnerable>()?.internalTemperatureLethal_High;
                        container.pickup_only_from_top = go.GetComponent<Baggable>()?.mustStandOntopOfTrapForPickup;
                        container.pickup_allow_mark = go.GetComponent<Capturable>()?.allowCapture;
                        container.pickup_use_gun = go.GetComponent<Baggable>()?.useGunForPickup;
                        container.tags = prefab.Tags.Select(x => x.ToString()).ToArray();
                        container.faction = go.GetComponent<FactionAlignment>()?.Alignment.ToString();
                        container.species = go.GetComponent<CreatureBrain>()?.species.ToString();
                        container.lures = go.GetDef<LureableMonitor.Def>()?.lures?.Select(x => x.ToString())?.ToArray();
                        container.attackValue = go.GetComponent<Weapon>()?.properties.base_damage_min;

                        container.chore_table = new ChoreContainer().Set(go);

                        container.adultId = go.GetDef<BabyMonitor.Def>()?.adultPrefab.ToString();
                        container.eggId = go.GetDef<FertilityMonitor.Def>()?.eggPrefab.ToString();
                        //container.babyId;
                        container.dropOnMature = go.GetDef<BabyMonitor.Def>()?.onGrowDropID;
                        container.fertility_cycles = go.GetDef<FertilityMonitor.Def>()?.baseFertileCycles;

                        var breedingWeights = go.GetDef<FertilityMonitor.Def>()?.initialBreedingWeights;
                        if (breedingWeights != null)
                        {
                            container.egg_chances = new List<KeyValuePair<string, float>>();
                            foreach (var breedingWeight in breedingWeights)
                                container.egg_chances.Add(new KeyValuePair<string, float>(breedingWeight.egg.ToString(), breedingWeight.weight));
                        }

                        container.is_ranchable = go.GetDef<RanchableMonitor.Def>() != null;
                        container.canBeNamed = go.GetComponent<UserNameable>() != null;

                        //deprecated container.calories_per_KG = (go.GetDef<CreatureCalorieMonitor.Def>()?.minPoopSizeInCalories ?? 0f) / 25f;
                        //deprecated container.min_poop_KG = 25f;
                        container.min_poop_calories = go.GetDef<CreatureCalorieMonitor.Def>()?.minPoopSizeInCalories;
                        container.diet_list = DietContainer.Convert(go.GetDef<CreatureCalorieMonitor.Def>()?.diet?.infos);
                        bool feedOnGround = go.GetDef<SolidConsumerMonitor.Def>() != null;
                        bool feedOnSurrounding = go.GetDef<GasAndLiquidConsumerMonitor.Def>() != null;
                        if (feedOnGround && feedOnSurrounding)
                            Debug.Log("[CustomizeCritter] WARNING: found critter with multiple feeding methods");
                        else if (!feedOnGround && !feedOnSurrounding)
                            container.feedsOnPickupables = null;
                        else
                            container.feedsOnPickupables = feedOnGround;

                        var light = go.GetComponent<Light2D>();
                        if (light != null)
                        {
                            container.light = new Light();
                            container.light.color = light.Color;
                            container.light.lux = light.Lux;
                            container.light.range = light.Range;
                        }

                        var scales = go.GetDef<ScaleGrowthMonitor.Def>();
                        if (scales != null)
                        {
                            container.scales = new Scales();
                            container.scales.atmosphere = scales.targetAtmosphere.ToString();
                            container.scales.drop = scales.itemDroppedOnShear.ToString();
                            container.scales.growthRate = scales.defaultGrowthRate;
                            container.scales.levelCount = scales.levelCount;
                            container.scales.mass = scales.dropMass;
                        }

                        var expulsion = go.GetDef<ElementDropperMonitor.Def>();
                        if (expulsion != null)
                        {
                            container.expulsion = new Expulsion();
                            container.expulsion.cellTargetMass = expulsion.dirtyCellToTargetMass;
                            container.expulsion.diseaseAmount = expulsion.emitDiseasePerKg;
                            container.expulsion.diseaseId = expulsion.emitDiseaseIdx.ToDiseaseId();
                            container.expulsion.element = expulsion.dirtyEmitElement.ToString();
                            container.expulsion.massPerDirt = expulsion.dirtyMassPerDirty;
                            container.expulsion.onDeath = expulsion.dirtyMassReleaseOnDeath;
                            container.expulsion.probability = expulsion.dirtyProbabilityPercent;
                        }

                        CustomizeCritterState.StateManager.State.critter_settings.Add(container);
                    }
                    else if (go.name.EndsWith("Egg", StringComparison.Ordinal) && go.name != "CookedEgg" && go.name != "RawEgg") // is egg
                    {
                        Debug.Log("Printing egg: " + prefab.name);
                        var container = new EggContainer();
                        container.eggId = prefab.name;
                        container.eggName = go.GetProperName();
                        container.eggDesc = go.GetComponent<InfoDescription>()?.description;

                        var anim_controller = go.GetComponent<KBatchedAnimController>();
                        if (anim_controller != null && anim_controller.usingNewSymbolOverrideSystem)
                        {
                            var override_controller = go.GetComponent<SymbolOverrideController>();

                            foreach (var over in override_controller.GetSymbolOverrides)
                            {
                                try
                                {
                                    string source = HashCache.Get().Get(over.sourceSymbol.hash);
                                    if (source != null)
                                    {
                                        int index = source.IndexOf('_');
                                        if (index >= 0)
                                        {
                                            string prefix = source.Substring(0, index + 1);
                                            string target = source.Substring(index + 1);

                                            if (target == over.targetSymbol)
                                            {
                                                container.override_prefix = prefix;
                                                break;
                                            }
                                            else
                                            {
                                                Debug.Log($"[CustomizeCritter] {container.eggId} has an override_prefix, but it cannot be extracted.");
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Debug.Log("Exception override_controller: " + e.Message);
                                }
                            }
                        }

                        container.anim_egg = go.GetComponent<KBatchedAnimController>()?.AnimFiles?[0]?.name;
                        container.egg_mass = go.GetComponent<PrimaryElement>()?.MassPerUnit;
                        container.babyId = go.GetDef<IncubationMonitor.Def>()?.spawnedCreature.ToString();
                        container.incubation_cycles = (float)Math.Round(1f / (6f * go.AddOrGetDef<IncubationMonitor.Def>().baseIncubationRate), 5);
                        container.egg_sortorder = go.AddOrGet<Pickupable>().sortOrder - TUNING.SORTORDER.EGGS;
                        container.is_fish = go.GetDef<FishOvercrowdingMonitor.Def>() != null;
                        container.egg_scale = go.GetComponent<KBatchedAnimController>()?.animHeight;

                        CustomizeCritterState.StateManager.State.egg_settings.Add(container);
                    }
                }

                foreach (var trait in Db.Get().traits.resources)
                {
                    var container = new TraitContainer();
                    container.traidId = trait.Id;
                    container.name = trait.Name;
                    foreach (var modifier in trait.SelfModifiers)
                        container.Add(modifier.AttributeId, modifier.Value, modifier.IsMultiplier);

                    if (!CustomizeCritterState.StateManager.State.trait_settings.Contains(container))
                        CustomizeCritterState.StateManager.State.trait_settings.Add(container);
                }

                CustomizeCritterState.StateManager.State.clear_vanilla_egg_modifiers = true;
                CustomizeCritterState.StateManager.State.egg_modifiers = EggModifiers.Defaults();

                CustomizeCritterState.StateManager.State.print_all = false;
                CustomizeCritterState.StateManager.TrySaveConfigurationState();
            }
        }

        /// Fixes mass for egg cracker recipies
        public static void FixEggRecipies()
        {
            if (CustomizeCritterState.StateManager.State.debug)
            {
                Debug.Log("Printout of recepies:");
                Debug.Log(ComplexRecipeManager.Get().recipes.Select(x => x.id).Join());
            }

            foreach (var setting in CustomizeCritterState.StateManager.State.egg_settings)
            {
                if (setting?.eggId == null) continue;

                if (setting.egg_mass != null)
                {
                    string recipeId = "EggCracker_I_" + setting.eggId + "_O_RawEgg_EggShell";
                    Debug.Log("[CustomizeCritter] Custom egg recipe: " + recipeId);
                    var recipe = ComplexRecipeManager.Get().recipes.Find(a => a.id == recipeId);
                    if (recipe != null)
                        recipe.results = new ComplexRecipe.RecipeElement[]
                        {
                            new ComplexRecipe.RecipeElement("RawEgg", 0.5f * setting.egg_mass.Value),
                            new ComplexRecipe.RecipeElement("EggShell", 0.5f * setting.egg_mass.Value)
                        };
                }
            }
        }

        public static void ChangeTraits()
        {
#if DEBUG
            if (CustomizeCritterState.StateManager.State.debug)
            {
                Debug.Log("Full trait list:");
                foreach (var x in Db.Get().traits.resources)
                {
                    Debug.Log("Trait: " + x.Id);
                    foreach (var y in x.SelfModifiers)
                    {
                        Debug.Log($"\t{y.AttributeId}:{y.Value}");
                    }
                }
            }
#endif

            foreach (var trait_data in CustomizeCritterState.StateManager.State.trait_settings)
            {
                if (trait_data?.traidId == null) continue;

                var trait = Db.Get().traits.resources.Find(s => s.Id == trait_data.traidId);
                if (trait == null)
                    trait = Db.Get().CreateTrait(trait_data.traidId, trait_data.name, trait_data.name, null, false, null, true, true);

                if (trait_data.attributes == null) continue;
                else trait.SelfModifiers.Clear();

                foreach (var attribute in trait_data.attributes)
                {
                    if (attribute == null) continue;
                    var a_modifier = trait.SelfModifiers.Find(s => s.AttributeId == attribute.id);
                    if (a_modifier == null)
                        trait.SelfModifiers.Add(new AttributeModifier(attribute.id, attribute.value, trait_data.name, attribute.multiplier ?? false));
                    else
                        a_modifier.SetValue(attribute.value);
                }

            }
        }

        public static FertilityMonitor.BreedingChance BreedingChance(string tag, float weight)
        {
            return new FertilityMonitor.BreedingChance()
            {
                egg = tag.ToTag(),
                weight = weight
            };
        }
        public static HashSet<Tag> egg_tags = new HashSet<Tag>() { GameTags.Egg, GameTags.IncubatableEgg, GameTags.PedestalDisplayable };

        /// Modifies existing critters and adds new ones, if ID is missing
        public static void ChangeCritters()
        {
            foreach (var setting in CustomizeCritterState.StateManager.State.critter_settings)
            {
                if (setting?.id == null) continue;

                KPrefabID kPrefab = Assets.Prefabs.Find(a => a.name == setting.id);
                GameObject go = kPrefab?.gameObject;
                bool isNew = go == null;

                Debug.Log("[CustomizeCritter] Processing " + setting.id);

                if (isNew)  // make sure new entities don't have null settings
                {
                    setting.name = setting.name ?? "Stone Hatch";
                    setting.desc = setting.desc ?? "Stone Hatches excrete solid Coal as waste and enjoy burrowing into the ground.";
                    setting.anim_file = setting.anim_file ?? "hatch_kanim";
                    setting.is_baby = setting.is_baby ?? false;
                    setting.is_adult = setting.is_adult ?? false;
                    setting.traitId = setting.traitId ?? "HatchHardBaseTrait";
                    setting.override_prefix = setting.override_prefix ?? null;
                    setting.space_requirement = setting.space_requirement ?? HatchTuning.PEN_SIZE_PER_CREATURE;
                    setting.lifespan = setting.lifespan ?? 100f;
                    setting.mass = setting.mass ?? 100f;
                    setting.width = setting.width ?? 1f;
                    setting.height = setting.height ?? 1f;
                    setting.decor = setting.decor ?? TUNING.DECOR.BONUS.TIER0;
                    setting.navGridName = setting.navGridName ?? (!setting.is_baby.Value ? "WalkerNavGrid1x1" : "WalkerBabyNavGrid");
                    setting.navi = setting.navi ?? NavType.Floor.ToString();
                    setting.moveSpeed = setting.moveSpeed ?? 2f;
                    setting.dropOnDeath = setting.dropOnDeath ?? new string[] { "Meat", "Meat" };
                    setting.canDrown = setting.canDrown ?? true;
                    setting.canCrushed = setting.canCrushed ?? false;
                    setting.canBurrow = setting.canBurrow ?? false;
                    setting.canTunnel = setting.canTunnel ?? false;
                    setting.canFall = setting.canFall ?? true;
                    setting.canHoverOverWater = setting.canHoverOverWater ?? false;
                    setting.tempLowDeath = setting.tempLowDeath ?? 243.15f;
                    setting.tempLowWarning = setting.tempLowWarning ?? 283.15f;
                    setting.tempBorn = setting.tempBorn ?? 293f;
                    setting.tempHighWarning = setting.tempHighWarning ?? 293.15f;
                    setting.tempHighDeath = setting.tempHighDeath ?? 343.15f;
                    setting.pickup_only_from_top = setting.pickup_only_from_top ?? true;
                    setting.pickup_allow_mark = setting.pickup_allow_mark ?? true;
                    setting.pickup_use_gun = setting.pickup_use_gun ?? false;
                    setting.tags = setting.tags ?? new string[] { GameTags.Creatures.Walker.ToString() };
                    setting.faction = setting.faction ?? FactionManager.FactionID.Pest.ToString();
                    setting.species = setting.species ?? GameTags.Creatures.Species.HatchSpecies.ToString();
                    setting.lures = setting.lures ?? null;
                    setting.attackValue = setting.attackValue ?? -1f;
                    setting.chore_table = setting.chore_table ?? new ChoreContainer();

                    setting.adultId = setting.adultId ?? "HatchHard";
                    setting.eggId = setting.eggId ?? "HatchHardEgg";    // TODO: find actual id
                    setting.babyId = setting.babyId ?? "HatchHardBaby";    // TODO: find actual id
                    setting.dropOnMature = setting.dropOnMature ?? null;
                    setting.fertility_cycles = setting.fertility_cycles ?? 60f;
                    if (setting.egg_chances == null)
                    {
                        setting.egg_chances = new List<KeyValuePair<string, float>>();
                        setting.egg_chances.Add(new KeyValuePair<string, float>("HatchHardEgg", 1f));
                    }
                    setting.is_ranchable = setting.is_ranchable ?? true;
                    setting.canBeNamed = setting.canBeNamed ?? false;
                    setting.min_poop_calories = setting.min_poop_calories ?? null;

                    //if (setting.diet_list == null)
                    //{
                    //    setting.diet_list = new List<DietContainer>();
                    //    setting.diet_list.Add(new DietContainer(1000f, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f, "Carbon", "Dirt"));
                    //}
                    setting.feedsOnPickupables = setting.feedsOnPickupables ?? true;
                    // end of configurations
                }

                if (isNew)  // create basic object
                {
                    Debug.Log("[CustomizeCritter] Adding new critter type " + setting.id);
                    go = EntityTemplates.CreatePlacedEntity(setting.id, setting.name, setting.desc, setting.mass.Value, Assets.GetAnim(setting.anim_file), "idle_loop", Grid.SceneLayer.Creatures, (int)setting.width, (int)setting.height, setting.decor.Value, default(EffectorValues), SimHashes.Creature, null, setting.tempBorn.Value);
                    kPrefab = go.GetComponent<KPrefabID>();
                    if (kPrefab == null) throw new NullReferenceException("kPrefab should not be null here '378'");
                }
                else        // edit basic settings
                {
                    if (setting.name != null)
                    {
                        kPrefab.PrefabTag = TagManager.Create(setting.id, setting.name);
                        go.GetComponent<KSelectable>()?.SetName(setting.name);
                        go.AddOrGet<DecorProvider>().overrideName = setting.name;
                    }

                    if (setting.desc != null) go.AddOrGet<InfoDescription>().description = setting.desc;
                    if (setting.mass != null) go.AddOrGet<PrimaryElement>().Mass = setting.mass.Value;
                    if (setting.anim_file != null) go.AddOrGet<KBatchedAnimController>().AnimFiles = new KAnimFile[] { Assets.GetAnim(setting.anim_file) };
                    if (setting.decor != null) go.AddOrGet<DecorProvider>().SetValues(setting.decor.Value);
                    if (setting.tempBorn != null) go.AddOrGet<PrimaryElement>().Temperature = setting.tempBorn.Value;

                    if (setting.width != null || setting.height != null)
                    {
                        float width = setting.width ?? 1f;
                        float height = setting.height ?? 1f;
                        KBoxCollider2D kboxCollider2D = go.AddOrGet<KBoxCollider2D>();
                        kboxCollider2D.size = new Vector2f(width, height);
                        float num = 0.5f * (float)((width + 1) % 2);
                        kboxCollider2D.offset = new Vector2f(num, (float)height / 2f);
                        go.GetComponent<KBatchedAnimController>().Offset = new Vector3(num, 0f, 0f);
                        go.AddOrGet<OccupyArea>().OccupiedCellsOffsets = EntityTemplates.GenerateOffsets((int)Math.Round(width, 0), (int)Math.Round(height, 0));
                    }

                }

                //start ExtendEntityToBasicCreature
                go.GetComponent<KBatchedAnimController>().isMovable = true;
                kPrefab.AddTag(GameTags.Creature, false);   // note: tags are saved in a HashSet and can set again without issues

                if (setting.traitId != null)
                {
                    Modifiers modifiers = go.AddOrGet<Modifiers>();
                    if (setting.traitId != "")
#if DLC1
                        modifiers.initialTraits = new List<string>() { setting.traitId };
#else
                        modifiers.initialTraits = new string[] { setting.traitId };
#endif
                    else
                        modifiers.initialTraits = null;
                    if (!modifiers.initialAmounts.Contains(Db.Get().Amounts.HitPoints.Id))
                        modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
                }

                if (isNew)
                {
                    go.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity("snapto_pivot", false);
                    go.AddOrGet<Pickupable>();
                    go.AddOrGet<Clearable>().isClearable = false;
                    go.AddOrGet<Traits>();
                    go.AddOrGet<Health>();
                    go.AddOrGet<CharacterOverlay>();
                    go.AddOrGet<RangedAttackable>();
                    go.AddOrGet<Prioritizable>();
                    go.AddOrGet<Effects>();
                    go.AddOrGetDef<CreatureDebugGoToMonitor.Def>();
                    go.AddOrGetDef<DeathMonitor.Def>();
                    go.AddOrGetDef<AnimInterruptMonitor.Def>();
                    SymbolOverrideControllerUtil.AddToPrefab(go);
                }

                if (setting.faction != null && !Enum.TryParse(setting.faction, out go.AddOrGet<FactionAlignment>().Alignment)) Debug.Log("[CustomizeCritter] Invalid FactionID: " + setting.faction);
                if (setting.tempLowDeath != null) go.AddOrGet<TemperatureVulnerable>().internalTemperatureLethal_Low = setting.tempLowDeath.Value;
                if (setting.tempLowWarning != null) go.AddOrGet<TemperatureVulnerable>().internalTemperatureWarning_Low = setting.tempLowWarning.Value;
                if (setting.tempHighWarning != null) go.AddOrGet<TemperatureVulnerable>().internalTemperatureWarning_High = setting.tempHighWarning.Value;
                if (setting.tempHighDeath != null) go.AddOrGet<TemperatureVulnerable>().internalTemperatureLethal_High = setting.tempHighDeath.Value;

                if (setting.canDrown != null)
                {
                    if (setting.canDrown.Value)
                        go.AddOrGet<DrowningMonitor>();
                    else
                        go.RemoveComponents<DrowningMonitor>();
                }

                if (setting.canCrushed != null)
                {
                    if (setting.canCrushed.Value)
                        go.AddOrGet<EntombVulnerable>();
                    else
                        go.RemoveComponents<EntombVulnerable>();
                }

                if (setting.dropOnDeath != null) go.AddOrGet<Butcherable>().SetDrops(setting.dropOnDeath);

                Navigator navigator = go.AddOrGet<Navigator>();
                navigator.updateProber = true;
                navigator.maxProbingRadius = 32;
                navigator.sceneLayer = Grid.SceneLayer.Creatures;
                if (setting.navGridName != null) navigator.NavGridName = setting.navGridName;
                if (setting.navi != null && !Enum.TryParse(setting.navi, out navigator.CurrentNavType)) Debug.Log("[CustomizeCritter] Invalid NavType: " + setting.navi);
                if (setting.moveSpeed != null) navigator.defaultSpeed = setting.moveSpeed.Value;
                // end ExtendEntityToBasicCreature

                if (setting.override_prefix != null)
                {
                    if (setting.override_prefix != "")
                        go.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(go.GetComponent<KBatchedAnimController>().AnimFiles[0], setting.override_prefix, null, 0);
                    else
                        go.RemoveComponents<SymbolOverrideController>();
                }

                if (isNew) go.AddOrGet<Trappable>();//?

                if (setting.canFall != null)
                {
                    if (setting.canFall.Value)
                        go.AddOrGetDef<CreatureFallMonitor.Def>();
                    else
                        go.RemoveDef<CreatureFallMonitor.Def>();
                }

                if (setting.canHoverOverWater != null)
                {
                    if (setting.canHoverOverWater.Value)
                        go.AddOrGetDef<CreatureFallMonitor.Def>().canSwim = true;
                    else if (go.GetDef<CreatureFallMonitor.Def>() != null)
                        go.GetDef<CreatureFallMonitor.Def>().canSwim = false;
                }

                if (setting.canBurrow != null)
                {
                    if (setting.canBurrow.Value)
                        go.AddOrGetDef<BurrowMonitor.Def>();
                    else
                        go.RemoveDef<BurrowMonitor.Def>();
                }

                if (setting.canTunnel != null)
                {
                    if (setting.canTunnel.Value)
                        go.AddOrGetDef<DiggerMonitor.Def>().depthToDig = MoleTuning.DEPTH_TO_HIDE;
                    else
                        go.RemoveDef<DiggerMonitor.Def>();
                }

                if (isNew) go.AddOrGetDef<WorldSpawnableMonitor.Def>().adjustSpawnLocationCb = (int cell) => { return cell; };  //?
                if (isNew) go.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;   //?

                if (setting.attackValue != null)
                {
                    if (setting.attackValue.Value > 0f)
                        go.AddWeapon(setting.attackValue.Value, setting.attackValue.Value, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
                    else
                        go.RemoveComponents<Weapon>();
                }

                //start CreateAndRegisterBaggedCreature
                if (setting.pickup_only_from_top != null) go.AddOrGet<Baggable>().mustStandOntopOfTrapForPickup = setting.pickup_only_from_top.Value;
                if (setting.pickup_use_gun != null) go.AddOrGet<Baggable>().useGunForPickup = setting.pickup_use_gun.Value;
                if (setting.pickup_allow_mark != null) go.AddOrGet<Capturable>().allowCapture = setting.pickup_allow_mark.Value;

                if (isNew)
                {
                    kPrefab.AddTag(GameTags.BagableCreature, false);
                    kPrefab.prefabSpawnFn += delegate (GameObject inst)
                    {
#if DLC1
                        DiscoveredResources.Instance.Discover(kPrefab.PrefabTag, DiscoveredResources.GetCategoryForTags(kPrefab.Tags));
#else
                        WorldInventory.Instance.Discover(kPrefab.PrefabTag, WorldInventory.GetCategoryForTags(kPrefab.Tags));
#endif
                    };
                }
                // end CreateAndRegisterBaggedCreature

                if (setting.tags != null)
                {
                    kPrefab.ProcessTags(setting.tags);
                }

                if (isNew) kPrefab.prefabInitFn += delegate (GameObject inst) { inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost); };

                // start AddCreatureBrain
                if (setting.species != null) go.AddOrGet<CreatureBrain>().species = setting.species;
                if (setting.override_prefix != null) go.AddOrGet<CreatureBrain>().symbolPrefix = setting.override_prefix;

                if (setting.chore_table != null && !kPrefab.HasTag(GameTags.Robots.Models.SweepBot))
                {
                    ChoreConsumer chore_consumer = go.AddOrGet<ChoreConsumer>();
                    chore_consumer.choreTable = setting.chore_table.Get(go).CreateTable();
                    kPrefab.instantiateFn += delegate (GameObject go2) { go2.GetComponent<ChoreConsumer>().choreTable = chore_consumer.choreTable; };
                }

                kPrefab.AddTag(GameTags.CreatureBrain, false);
                // end AddCreatureBrain


                // start ExtendEntityToWildCreature
                if (setting.lifespan == null || setting.lifespan.Value > 0f)
                    go.AddOrGetDef<AgeMonitor.Def>();
                else
                    go.RemoveDef<AgeMonitor.Def>();

                if (isNew) go.AddOrGetDef<HappinessMonitor.Def>();  //?

                if (CustomizeCritterState.StateManager.State.wildEffect != null && CustomizeCritterState.StateManager.State.tameEffect != null)
                {
                    WildnessMonitor.Def wild = go.AddOrGetDef<WildnessMonitor.Def>();
                    wild.wildEffect = new Effect("Wild" + kPrefab.PrefabTag.Name, STRINGS.CREATURES.MODIFIERS.WILD.NAME, STRINGS.CREATURES.MODIFIERS.WILD.TOOLTIP, 0f, true, true, false, null, 0f, null);
                    foreach (var attribute in CustomizeCritterState.StateManager.State.wildEffect)
                    {
                        wild.wildEffect.Add(new AttributeModifier(attribute.id, attribute.value, STRINGS.CREATURES.MODIFIERS.WILD.NAME, attribute.multiplier ?? false));
                    }

                    wild.tameEffect = new Effect("Tame" + kPrefab.PrefabTag.Name, STRINGS.CREATURES.MODIFIERS.TAME.NAME, STRINGS.CREATURES.MODIFIERS.TAME.TOOLTIP, 0f, true, true, false, null, 0f, null);
                    foreach (var attribute in CustomizeCritterState.StateManager.State.tameEffect)
                    {
                        wild.tameEffect.Add(new AttributeModifier(attribute.id, attribute.value, STRINGS.CREATURES.MODIFIERS.TAME.NAME, attribute.multiplier ?? false));
                    }
                }

                if (setting.space_requirement != null) go.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = setting.space_requirement.Value;
                if (isNew) go.AddTag(GameTags.Plant);
                // end ExtendEntityToWildCreature

                Diet diet;
                if (setting.diet_list != null)
                {
                    diet = new Diet(DietContainer.Convert(setting.diet_list));
                    go.AddOrGetDef<CreatureCalorieMonitor.Def>().diet = diet;
                }
                else
                {
                    diet = go.GetDef<CreatureCalorieMonitor.Def>()?.diet;
                }

                if (setting.min_poop_calories != null && diet != null) go.AddOrGetDef<CreatureCalorieMonitor.Def>().minPoopSizeInCalories = setting.min_poop_calories.Value;

                if (setting.feedsOnPickupables != null && diet != null) // note: not sure if both were possible; will only allow one for now
                {
                    if (setting.feedsOnPickupables.Value)
                    {
                        go.RemoveDef<GasAndLiquidConsumerMonitor.Def>();
                        go.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
                    }
                    else
                    {
                        go.RemoveDef<SolidConsumerMonitor.Def>();
                        go.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>().diet = diet;
                    }
                }

                if (setting.is_baby == null && setting.is_baby.Value)
                {
                    // start ExtendEntityToBeingABaby
                    if (setting.adultId != null) go.AddOrGetDef<BabyMonitor.Def>().adultPrefab = setting.adultId;
                    if (setting.dropOnMature != null) go.AddOrGetDef<BabyMonitor.Def>().onGrowDropID = setting.dropOnMature;
                    go.AddOrGetDef<IncubatorMonitor.Def>();
                    go.AddOrGetDef<CreatureSleepMonitor.Def>();
                    go.AddOrGetDef<CallAdultMonitor.Def>();
                    if (setting.lifespan == null || setting.lifespan.Value > 0f) go.AddOrGetDef<AgeMonitor.Def>().maxAgePercentOnSpawn = 0.01f;
                    // end ExtendEntityToBeingABaby
                }

                if (setting.is_adult == null && setting.is_adult.Value)
                {
                    // start ExtendEntityToFertileCreature
                    if (setting.fertility_cycles != null) go.AddOrGetDef<FertilityMonitor.Def>().baseFertileCycles = setting.fertility_cycles.Value;

                    if (setting.eggId != null) go.AddOrGetDef<FertilityMonitor.Def>().eggPrefab = new Tag(setting.eggId);
                    if (setting.egg_chances != null) go.AddOrGetDef<FertilityMonitor.Def>().initialBreedingWeights = setting.egg_chances.Select(x => Helper.BreedingChance(x.Key, x.Value)).ToList();
#if DLC1
                    if (setting.eggId != null) kPrefab.prefabSpawnFn += delegate (GameObject inst) { DiscoveredResources.Instance.Discover(setting.eggId.ToTag(), DiscoveredResources.GetCategoryForTags(egg_tags)); };
                    if (setting.babyId != null) kPrefab.prefabSpawnFn += delegate (GameObject inst) { DiscoveredResources.Instance.Discover(setting.babyId.ToTag(), DiscoveredResources.GetCategoryForTags(kPrefab.Tags)); };
#else
                    if (setting.eggId != null) kPrefab.prefabSpawnFn += delegate (GameObject inst) { WorldInventory.Instance.Discover(setting.eggId.ToTag(), WorldInventory.GetCategoryForTags(egg_tags)); };
                    if (setting.babyId != null) kPrefab.prefabSpawnFn += delegate (GameObject inst) { WorldInventory.Instance.Discover(setting.babyId.ToTag(), WorldInventory.GetCategoryForTags(kPrefab.Tags)); };
#endif
                    if (setting.is_ranchable != null)
                    {
                        if (setting.is_ranchable.Value)
                            go.AddOrGetDef<RanchableMonitor.Def>();
                        else
                            go.RemoveDef<RanchableMonitor.Def>();
                    }

                    if (isNew) go.AddOrGetDef<FixedCapturableMonitor.Def>(); //? if add_fixed_capturable_monitor
                    // end ExtendEntityToFertileCreature
                }

                if (setting.scales != null)
                {
                    ScaleGrowthMonitor.Def def_scale = go.AddOrGetDef<ScaleGrowthMonitor.Def>();
                    def_scale.defaultGrowthRate = setting.scales.growthRate ?? (def_scale.defaultGrowthRate != 0f ? def_scale.defaultGrowthRate : 1f / 8f / 600f);
                    def_scale.dropMass = setting.scales.mass ?? (def_scale.dropMass != 0f ? def_scale.dropMass : 2f);
                    def_scale.itemDroppedOnShear = setting.scales.drop ?? (def_scale.itemDroppedOnShear.IsValid ? def_scale.itemDroppedOnShear : "Dirt");
                    def_scale.levelCount = setting.scales.levelCount ?? def_scale.levelCount;

                    if (setting.scales.atmosphere != null)
                        def_scale.targetAtmosphere = setting.scales.atmosphere.ToSimHash();
                    else if (0 == (int)def_scale.targetAtmosphere)
                        def_scale.targetAtmosphere = "Void".ToSimHash();
                }

                if (setting.expulsion != null)
                {
                    ElementDropperMonitor.Def def_morb = go.AddOrGetDef<ElementDropperMonitor.Def>();
                    def_morb.dirtyProbabilityPercent = setting.expulsion.probability ?? (def_morb.dirtyProbabilityPercent != 0 ? def_morb.dirtyProbabilityPercent : 25f);
                    def_morb.dirtyCellToTargetMass = setting.expulsion.cellTargetMass ?? (def_morb.dirtyCellToTargetMass != 0 ? def_morb.dirtyCellToTargetMass : 1f);
                    def_morb.dirtyMassPerDirty = setting.expulsion.massPerDirt ?? (def_morb.dirtyMassPerDirty != 0 ? def_morb.dirtyMassPerDirty : 0.2f);
                    def_morb.dirtyMassReleaseOnDeath = setting.expulsion.onDeath ?? def_morb.dirtyMassReleaseOnDeath;

                    if (setting.expulsion.diseaseId != null)
                    {
                        byte disease = setting.expulsion.diseaseId.ToDiseaseIdx();
                        def_morb.emitDiseaseIdx = disease != byte.MaxValue || def_morb.emitDiseaseIdx == 0 ? disease : def_morb.emitDiseaseIdx;
                    }
                    def_morb.emitDiseasePerKg = setting.expulsion.diseaseAmount ?? def_morb.emitDiseasePerKg;

                    if (setting.expulsion.element != null)
                        def_morb.dirtyEmitElement = setting.expulsion.element.ToSimHash();
                    else if (0 == (int)def_morb.dirtyEmitElement)
                        def_morb.dirtyEmitElement = "Dirt".ToSimHash();
                }

                if (setting.light != null)
                {
                    if (setting.light.color != Color.black)
                    {
                        Light2D light2D = go.AddOrGet<Light2D>();
                        light2D.Color = setting.light.color ?? light2D.Color;
                        light2D.overlayColour = TUNING.LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
                        light2D.Range = setting.light.range ?? (light2D.Range != 0f ? light2D.Range : 5f);
                        light2D.Angle = 0f;
                        light2D.Direction = TUNING.LIGHT2D.LIGHTBUG_DIRECTION;
                        light2D.Offset = TUNING.LIGHT2D.LIGHTBUG_OFFSET;
                        light2D.shape = LightShape.Circle;
                        light2D.drawOverlay = true;
                        light2D.Lux = setting.light.lux ?? (light2D.Lux != 0 ? light2D.Lux : 1800);
                        go.AddOrGet<LightSymbolTracker>().targetSymbol = "snapTo_light_locator";
                        go.AddOrGetDef<CreatureLightToggleController.Def>();
                    }
                    else
                    {
                        go.RemoveComponents<Light2D>();
                        go.RemoveComponents<LightSymbolTracker>();
                        go.RemoveDef<CreatureLightToggleController.Def>();
                    }
                }

                if (isNew) Assets.AddPrefab(kPrefab);
            }
        }

        /// Modifies existing eggs and adds new ones, if ID is missing
        public static void ChangeEggs()
        {
            foreach (var setting in CustomizeCritterState.StateManager.State.egg_settings)
            {
                if (setting.eggId == null) continue;

                KPrefabID kPrefab = Assets.Prefabs.Find(a => a.name == setting.eggId);
                GameObject go = kPrefab?.gameObject;
                bool isNew = go == null;

                Debug.Log("[CustomizeCritter] Processing " + setting.eggId);

                if (isNew)  // make sure all settings have values
                {
                    setting.eggName = setting.eggName ?? "Missing Name";
                    setting.eggDesc = setting.eggDesc ?? "Missing description";
                    setting.override_prefix = setting.override_prefix ?? null;
                    setting.anim_egg = setting.anim_egg ?? "egg_hatch_kanim";
                    setting.egg_mass = setting.egg_mass ?? 2f;
                    setting.babyId = setting.babyId ?? "HatchHardBaby";
                    setting.incubation_cycles = setting.incubation_cycles ?? 20f;
                    setting.egg_sortorder = setting.egg_sortorder ?? 2;
                    setting.is_fish = setting.is_fish ?? false;
                    setting.egg_scale = setting.egg_scale ?? 1f;
                }

                if (isNew) // create new basic object
                {
                    Debug.Log("[CustomizeCritter] Adding new egg type: " + setting.eggId);
                    go = EggConfig.CreateEgg(setting.eggId, setting.eggName, setting.eggDesc, setting.babyId, setting.anim_egg, setting.egg_mass.Value, setting.egg_sortorder.Value, 100f / (600f * setting.incubation_cycles.Value));
                }
                else       // change basic settings
                {
                    if (setting.eggName != null)
                    {
                        kPrefab.PrefabTag = TagManager.Create(setting.eggId, setting.eggName);
                        go.AddOrGet<KSelectable>().SetName(setting.eggName);
                    }

                    if (setting.eggDesc != null) go.AddOrGet<InfoDescription>().description = setting.eggDesc;
                    if (setting.anim_egg != null) go.AddOrGet<KBatchedAnimController>().AnimFiles = new KAnimFile[] { Assets.GetAnim(setting.anim_egg) };

                    if (setting.egg_mass != null) go.AddOrGet<PrimaryElement>().MassPerUnit = setting.egg_mass.Value;
                    if (setting.babyId != null) go.AddOrGetDef<IncubationMonitor.Def>().spawnedCreature = setting.babyId;
                    if (setting.incubation_cycles != null) go.AddOrGetDef<IncubationMonitor.Def>().baseIncubationRate = 100f / (600f * setting.incubation_cycles.Value);
                    if (setting.egg_sortorder != null) go.AddOrGet<Pickupable>().sortOrder = TUNING.SORTORDER.EGGS + setting.egg_sortorder.Value;
                }

                if (setting.override_prefix != null && setting.anim_egg != null)
                {
                    if (setting.override_prefix != "")
                    {
                        go.AddOrGet<KBatchedAnimController>().usingNewSymbolOverrideSystem = true;
                        go.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(go.GetComponent<KBatchedAnimController>().AnimFiles[0], setting.override_prefix, null, 0);
                    }
                    else
                    {
                        go.AddOrGet<KBatchedAnimController>().usingNewSymbolOverrideSystem = false;
                        go.RemoveComponents<SymbolOverrideController>();
                    }
                }

                if (setting.is_fish != null)
                {
                    if (setting.is_fish.Value)
                        go.AddOrGetDef<FishOvercrowdingMonitor.Def>();
                    else
                        go.RemoveDef<FishOvercrowdingMonitor.Def>();
                }

                if (setting.egg_scale != null)
                {
                    KBatchedAnimController kanim_controller = go.GetComponent<KBatchedAnimController>();
                    kanim_controller.animWidth = setting.egg_scale.Value;
                    kanim_controller.animHeight = setting.egg_scale.Value;
                }
            }
        }
    }

    [Harmony.HarmonyPatch(typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS), MethodType.StaticConstructor)]
    public class Patch_EggChances
    {
        public static bool Prefix()
        {
            if (CustomizeCritterState.StateManager.State.clear_vanilla_egg_modifiers)
            {
                TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS = new List<System.Action>();
                return false;
            }
            return true;
        }

        public static void Postfix()
        {
            foreach (var modifier in CustomizeCritterState.StateManager.State.egg_modifiers)
            {
                TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(modifier.Convert());
            }
        }
    }

}