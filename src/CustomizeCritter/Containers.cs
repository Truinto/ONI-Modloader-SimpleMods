using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Klei.AI;
using FumLib;

namespace CustomizeCritter
{
    public class CritterContainer
    {
        public string id;
        public string name;
        public string desc;
        public string anim_file;
        public bool? is_baby;
        public bool? is_adult;
        public string traitId;
        public string override_prefix;
        public int? space_requirement;
        public float? lifespan;
        public float? mass;
        public float? width;
        public float? height;
        public EffectorValues? decor;
        public string navGridName;
        public string navi;
        public float? moveSpeed;
        public string[] dropOnDeath;
        public bool? canDrown;
        public bool? canCrushed;
        public bool? canBurrow;
        public bool? canTunnel;
        public bool? canFall;
        public bool? canHoverOverWater;
        public float? tempLowDeath;
        public float? tempLowWarning;
        public float? tempBorn;
        public float? tempHighWarning;
        public float? tempHighDeath;
        public bool? pickup_only_from_top;
        public bool? pickup_allow_mark;
        public bool? pickup_use_gun;
        public string[] tags;
        public string faction;
        public string species;
        public string[] lures;
        public float? attackValue;
        public ChoreContainer chore_table;

        public string adultId;
        public string eggId;
        public string babyId;
        public string dropOnMature;
        public float? fertility_cycles;
        public List<KeyValuePair<string, float>> egg_chances;
        public bool? is_ranchable;
        public bool? canBeNamed;

        //public float? calories_per_KG;  //deprecated
        //public float? min_poop_KG;  //deprecated
        public float? min_poop_calories;
        public List<DietContainer> diet_list;
        public bool? feedsOnPickupables;

        public Light light;
        public Scales scales;
        public Expulsion expulsion;

        public CritterContainer Defaults()
        {
            id = "HatchHard";
            name = "Stone Hatch";
            desc = "Stone Hatches excrete solid Coal as waste and enjoy burrowing into the ground.";
            anim_file = "hatch_kanim";
            is_baby = false;
            traitId = "HatchHardBaseTrait";
            override_prefix = "hvy_";
            space_requirement = HatchTuning.PEN_SIZE_PER_CREATURE;
            lifespan = 100f;
            mass = 100f;
            width = 1;
            height = 1;
            decor = TUNING.DECOR.BONUS.TIER0;
            navGridName = !is_baby.Value ? "WalkerNavGrid1x1" : "WalkerBabyNavGrid";
            navi = NavType.Floor.ToString();
            moveSpeed = 2f;
            dropOnDeath = new string[] { "Meat" };
            canDrown = true;
            canCrushed = false;
            canBurrow = false;
            canTunnel = false;
            canFall = true;
            canHoverOverWater = false;
            tempLowDeath = 243.15f;
            tempLowWarning = 283.15f;
            tempBorn = 293f;
            tempHighWarning = 293.15f;
            tempHighDeath = 343.15f;
            pickup_only_from_top = true;
            pickup_allow_mark = true;
            pickup_use_gun = false;
            tags = new string[] { GameTags.Creatures.Walker.ToString() };
            faction = FactionManager.FactionID.Pest.ToString();
            species = GameTags.Creatures.Species.HatchSpecies.ToString();
            lures = null;
            attackValue = 1f;
            chore_table = null;

            adultId = "HatchHard";
            eggId = "HatchHardEgg";
            babyId = "HatchHardBaby";
            dropOnMature = null;
            fertility_cycles = 60f;
            egg_chances = new List<KeyValuePair<string, float>>();
            egg_chances.Add(new KeyValuePair<string, float>("HatchHardEgg", 1f));
            is_ranchable = true;
            
            diet_list = new List<DietContainer>();
            diet_list.Add(new DietContainer(100f, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f, "Carbon", "SedimentaryRock", "IgneousRock"));
            feedsOnPickupables = true;
            canBeNamed = false;

            light = new Light()
            {
                color = Color.blue,
                lux = 1800,
                range = 5,
            };

            scales = new Scales()
            {
                atmosphere = "Void",
                drop = "Dirt",
                mass = 5f,
                growthRate = 1f / 600f,
                levelCount = 5,
            };

            expulsion = new Expulsion()
            {
                element = "Copper",
                massPerDirt = 5f,
                cellTargetMass = 5f,
                probability = 100f,
                onDeath = 3f,
                diseaseId = "SlimeLung",
                diseaseAmount = 1000f,
            };

            return this;
        }
        
    }

    public class EggContainer
    {
        public string eggId;
        public string eggName;
        public string eggDesc;
        public string override_prefix;
        public string anim_egg;
        public float? egg_mass;
        public string babyId;

        public float? incubation_cycles;
        public int? egg_sortorder;
        public bool? is_fish;
        public float? egg_scale;

        public EggContainer Defaults()
        {
            eggId = "HatchHardEgg";
            eggName = "Stone Hatchling Egg";
            eggDesc = "A Stone Hatchling Egg";
            override_prefix = "hvy_";
            anim_egg = "egg_hatch_kanim";
            egg_mass = 2f;
            babyId = "HatchHardBaby";
            incubation_cycles = 20f;
            egg_sortorder = 2;
            is_fish = false;
            egg_scale = 1f;
            return this;
        }
    }

    public class TraitContainer
    {
        public string traidId;
        public string name;
        public List<Attribute> attributes;

        public TraitContainer Defaults()
        {
            traidId = "HatchHardBaseTrait";
            name = "Stone Hatch";
            attributes = new List<Attribute>();
            Add("HitPointsMax", 99f);
            Add("AgeMax", 999f);
            return this;
        }

        public TraitContainer Add(string id, float value, bool multiplier = false)
        {
            if (this.attributes == null) this.attributes = new List<Attribute>();
            this.attributes.Add(new Attribute(id, value, multiplier));
            return this;
        }

        public override bool Equals(object obj)
        {
            return this.traidId == (obj as TraitContainer)?.traidId;
        }

        public override int GetHashCode()
        {
            return Hash.SDBMLower(this.traidId);
        }

        public class Attribute
        {
            public string id;
            public float value;
            public bool? multiplier;

            public Attribute() {}

            public Attribute(string id, float value, bool? multiplier = null)
            {
                this.id = id;
                this.value = value;
                this.multiplier = multiplier;
            }

            public override bool Equals(object obj)
            {
                return this.id == (obj as Attribute)?.id;
            }

            public override int GetHashCode()
            {
                return Hash.SDBMLower(this.id);
            }
        }
    }

    public class DietContainer
    {
        public HashSet<string> consumedTags = new HashSet<string>();
        public string producedTag;
        public float caloriesPerKg;
        public float producedConversionRate;
        public string diseaseId;
        public float diseasePerKgProduced;
        public bool produceSolidTile;
        public bool eatsPlantsDirectly;

        public DietContainer()
        { }

        public DietContainer(float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced, string poopTag, params string[] consumeTags)
        {
            this.producedTag = poopTag;
            this.caloriesPerKg = caloriesPerKg;
            this.producedConversionRate = producedConversionRate;
            this.diseaseId = diseaseId;
            this.diseasePerKgProduced = diseasePerKgProduced;
            foreach (var consume in consumeTags) consumeTags.Add(consume);
        }

        public DietContainer(Diet.Info info)
        {
            this.producedTag = info.producedElement.IsValid ? info.producedElement.ToString() : null;
            this.caloriesPerKg = info.caloriesPerKg;
            this.producedConversionRate = info.producedConversionRate;
            this.diseaseId = info.diseaseIdx.ToDiseaseId();
            this.diseasePerKgProduced = info.diseasePerKgProduced;
            this.consumedTags = new HashSet<string>(info.consumedTags.Select(x => x.ToString()));
        }

        public static List<DietContainer> Convert(Diet.Info[] infos)
        {
            if (infos == null || infos.Length < 1)
                return null;

            var result = new List<DietContainer>();

            foreach (var info in infos)
                result.Add(new DietContainer(info));

            return result;
        }

        public static Diet.Info[] Convert(List<DietContainer> c)
        {
            Diet.Info[] result = new Diet.Info[c.Count];

            for (int i = 0; i < c.Count; i++)
            {
                result[i] = new Diet.Info(new HashSet<Tag>(c[i].consumedTags.Select(x => x.ToTag())), c[i].producedTag, c[i].caloriesPerKg, c[i].producedConversionRate, c[i].diseaseId, c[i].diseasePerKgProduced, c[i].produceSolidTile, c[i].eatsPlantsDirectly);
            }

            return result;
        }
    }

    public class ChoreContainer
    {
        public bool automatically_populate;

        public bool can_attack;
        public bool can_flee;
        public bool can_sleep;
        public bool can_call;

        public float? bag_escape_time;
        public string nesting_poop;

        public string copy_from_ID; // idea: copy chore_table from another critter, instead of making a new one
        public List<StateMachine.BaseDef> chore_table; // TODO: fix exception with constructor

        public void PopulateDefs(GameObject go)
        {
            if (go == null)
                Debug.LogError("PopulateDefs go shouldn't be null!");

            //always
            Add(new DeathStates.Def());
            Add(new AnimInterruptStates.Def());
            Add(new BaggedStates.Def() { escapeTime = this.bag_escape_time ?? 300f });
            Add(new DebugGoToStates.Def());

            //has baby or adult form
            if (go.GetDef<BabyMonitor.Def>() != null || go.GetDef<FertilityMonitor.Def>() != null)
            {
                Add(new GrowUpStates.Def());
                Add(new IncubatingStates.Def());
                Add(new LayEggStates.Def());
            }

            //can fly
            if (go.HasTag(GameTags.Creatures.Flyer))//(go.GetDef<CreatureFallMonitor.Def>() != null)
            {
            }
            else
            {
                Add(new TrappedStates.Def());
                if (!go.HasTag(GameTags.SwimmingCreature)) Add(new FallStates.Def()); //special case: fish
            }

            //is fish
            if (go.HasTag(GameTags.SwimmingCreature))
            {
                Add(new FallStates.Def { getLandAnim = new Func<FallStates.Instance, string>(GetLandAnim) });
                Add(new FlopStates.Def());
                Add(new FixedCaptureStates.Def());
            }
            else
            {
                Add(new StunnedStates.Def());
                Add(new FixedCaptureStates.Def());
                Add(new RanchedStates.Def());
            }

            //can drown
            if (go.GetComponent<DrowningMonitor>() != null)
                Add(new DrowningStates.Def());

            //eats solid
            if (go.GetDef<SolidConsumerMonitor.Def>() != null)
                Add(new EatStates.Def());

            //eats gas/liquids
            if (go.GetDef<GasAndLiquidConsumerMonitor.Def>() != null)
                Add(new InhaleStates.Def());

            //can get lured
            if (go.GetDef<LureableMonitor.Def>() != null)
                Add(new MoveToLureStates.Def());

            //can burrow
            if (go.GetDef<BurrowMonitor.Def>() != null && go.GetDef<FertilityMonitor.Def>() != null)
            {
                Add(new ExitBurrowStates.Def());

                //only adult
                if (go.GetDef<BabyMonitor.Def>() == null)
                {
                    Add(new PlayAnimsStates.Def(GameTags.Creatures.Burrowed, true, "idle_mound", STRINGS.CREATURES.STATUSITEMS.BURROWED.NAME, STRINGS.CREATURES.STATUSITEMS.BURROWED.TOOLTIP));
                    Add(new PlayAnimsStates.Def(GameTags.Creatures.WantsToEnterBurrow, false, "hide", STRINGS.CREATURES.STATUSITEMS.BURROWING.NAME, STRINGS.CREATURES.STATUSITEMS.BURROWING.TOOLTIP));
                }
            }

            //can tunnel
            if (go.GetDef<DiggerMonitor.Def>() != null)
                Add(new DiggerStates.Def());

            //can expulse
            if (go.GetDef<ElementDropperMonitor.Def>() != null)
                Add(new DropElementStates.Def());

            if (this.can_attack)
                Add(new AttackStates.Def());

            if (this.can_flee)
                Add(new FleeStates.Def());

            if (this.can_sleep)
                Add(new CreatureSleepStates.Def());

            if (this.can_call)
                Add(new CallAdultStates.Def());

            //species dependend!
            Tag species = go.GetComponent<CreatureBrain>().species;

            if (species == GameTags.Creatures.Species.MooSpecies || species == GameTags.Creatures.Species.PuftSpecies)
                Add(new IdleStates.Def { customIdleAnim = new IdleStates.Def.IdleAnimCallback(CustomIdleAnim_Moo_Puft) });
            else if (species == GameTags.Creatures.Species.DreckoSpecies)
                Add(new IdleStates.Def { customIdleAnim = new IdleStates.Def.IdleAnimCallback(CustomIdleAnim_Drecko) });
            else if (species == GameTags.Creatures.Species.MoleSpecies)
                Add(new IdleStates.Def { customIdleAnim = new IdleStates.Def.IdleAnimCallback(CustomIdleAnim_Mole) });
            else
                Add(new IdleStates.Def());

            //Crabs
            if (species == GameTags.Creatures.Species.CrabSpecies)
                Add(new DefendStates.Def());

            //Squirrels
            if (species == GameTags.Creatures.Species.SquirrelSpecies)
            {
                Add(new TreeClimbStates.Def());
                Add(new SeedPlantingStates.Def());
            }

            //Pufts
            if (species == GameTags.Creatures.Species.PuftSpecies)
                Add(new UpTopPoopStates.Def());

            //Oilfloaters
            if (species == GameTags.Creatures.Species.OilFloaterSpecies)
                Add(new SameSpotPoopStates.Def());

            //Moles
            if (species == GameTags.Creatures.Species.MoleSpecies)
            {
                try
                {
                    Add((StateMachine.BaseDef)Activator.CreateInstance(Type.GetType("NestingPoopState+Def, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"), nesting_poop != null ? nesting_poop.ToTag() : new Tag(0)));
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[CustomizeCritter] Critical error while generating NestingPoopState+Def " + e.Message);
                }
            }

            if (species == GameTags.Robots.Models.SweepBot)
            {
                Add(new SweepBotTrappedStates.Def());
                Add(new DeliverToSweepLockerStates.Def());
                Add(new ReturnToChargeStationStates.Def());
                Add(new SweepStates.Def());
            }

            //poop general rule: must NOT have InhaleStates; must have diet with output
            var diet = go.GetDef<CreatureCalorieMonitor.Def>()?.diet?.infos;
            if (ElementLoader.elements != null && diet != null && diet.Length > 0 && diet[0].producedElement.IsValid)
            {
                switch (ElementLoader.FindElementByName(diet[0].producedElement.ToString()).state)
                {
                    case Element.State.Solid:
                        //Mole + Crab + Drecko + Hatch OR Pacu
                        Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, go.HasTag(GameTags.SwimmingCreature) ? "lay_egg_pre" : "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP));
                        break;
                    case Element.State.Liquid:
                        //Moo
                        Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_GAS.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_GAS.TOOLTIP));
                        break;
                }
            }
            else if (ElementLoader.elements == null)
                Debug.Log("Wups, elements are not loaded yet.");
        }

        //makes sure not to add duplicates
        public void Add(StateMachine.BaseDef def)
        {
            if (chore_table == null)
                chore_table = new List<StateMachine.BaseDef>();

            bool doAdd = true;

            if (chore_table.Any(s => s.GetType() == def.GetType()))
                doAdd = false;                  // don't add if def Type is already added

            var play_tag = (def as PlayAnimsStates.Def)?.tag;
            if (play_tag != null && play_tag.Value.IsValid)
            {
                doAdd = !chore_table.Any(s =>   // don't add if any PlayAnimsState matches the tag already
                {
                    var x = s as PlayAnimsStates.Def;
                    return (x != null && x.tag == play_tag);
                });
            }

            if (doAdd)
                chore_table.Add(def);
        }

        public ChoreTable.Builder Get(GameObject go)
        {
            if (go == null || chore_table == null) return null;

            if (automatically_populate)
                PopulateDefs(go);

            var result = new ChoreTable.Builder();

            foreach (var def in chore_table)
                result.Add(def);

            return result;
        }

        public ChoreContainer Set(GameObject go)
        {
            if (go == null) return null;

            var chore = go.GetComponent<ChoreConsumer>();
            if (chore == null || chore.choreTable == null) return null;

            var entries = (ChoreTable.Entry[])AccessTools.Field(typeof(ChoreTable), "entries").GetValue(chore.choreTable);
            if (entries == null) return null;

            if (chore_table == null)
                chore_table = new List<StateMachine.BaseDef>();

            if (CustomizeCritterState.StateManager.State.debug)
            {
                Debug.Log("Printout BaseDefs");
                foreach (var entry in entries)
                    Debug.Log(entry.stateMachineDef.GetType().AssemblyQualifiedName);
            }

            if (CustomizeCritterState.StateManager.State.print_verbose)
                foreach (var entry in entries)
                    chore_table.Add(entry.stateMachineDef);
            else
                automatically_populate = true;

            can_attack = entries.Any(s => s.stateMachineDef is AttackStates.Def);
            can_flee = entries.Any(s => s.stateMachineDef is FleeStates.Def);
            can_sleep = entries.Any(s => s.stateMachineDef is CreatureSleepStates.Def);
            can_call = entries.Any(s => s.stateMachineDef is CallAdultStates.Def);

            if (go.HasTag("Mole")) nesting_poop = "Regolith";
            
            return this;
        }

        private static Func<FallStates.Instance, string> _pacu_fall = (Func<FallStates.Instance, string>)Delegate.CreateDelegate(typeof(Func<FallStates.Instance, string>), AccessTools.Method(typeof(BasePacuConfig), "GetLandAnim"));
        private static string GetLandAnim(FallStates.Instance smi)
        {
            if (smi.GetSMI<CreatureFallMonitor.Instance>().CanSwimAtCurrentLocation(true))
            {
                return "idle_loop";
            }
            return "flop_loop";
        }
        private static HashedString CustomIdleAnim_Drecko(IdleStates.Instance smi, ref HashedString pre_anim)
        {
            CellOffset offset = new CellOffset(0, -1);
            bool facing = smi.GetComponent<Facing>().GetFacing();
            NavType currentNavType = smi.GetComponent<Navigator>().CurrentNavType;
            if (currentNavType != NavType.Floor)
            {
                if (currentNavType == NavType.Ceiling)
                {
                    offset = (facing ? new CellOffset(1, 1) : new CellOffset(-1, 1));
                }
            }
            else
            {
                offset = (facing ? new CellOffset(1, -1) : new CellOffset(-1, -1));
            }
            HashedString result = "idle_loop";
            int num = Grid.OffsetCell(Grid.PosToCell(smi), offset);
            if (Grid.IsValidCell(num) && !Grid.Solid[num])
            {
                pre_anim = "idle_loop_hang_pre";
                result = "idle_loop_hang";
            }
            return result;
        }
        private static HashedString CustomIdleAnim_Moo_Puft(IdleStates.Instance smi, ref HashedString pre_anim)
        {
            CreatureCalorieMonitor.Instance smi2 = smi.GetSMI<CreatureCalorieMonitor.Instance>();
            return (smi2 != null && smi2.stomach.IsReadyToPoop()) ? "idle_loop_full" : "idle_loop";
        }
        private static HashedString CustomIdleAnim_Mole(IdleStates.Instance smi, ref HashedString pre_anim)
        {
            if (smi.gameObject.GetComponent<Navigator>().CurrentNavType == NavType.Solid)
            {
                int num = UnityEngine.Random.Range(0, _solidIdleAnims.Length);
                return _solidIdleAnims[num];
            }
            if (smi.gameObject.GetDef<BabyMonitor.Def>() != null && UnityEngine.Random.Range(0, 100) >= 90)
            {
                return "drill_fail";
            }
            return "idle_loop";
        }
        private static readonly string[] _solidIdleAnims = new string[] { "idle1", "idle2", "idle3", "idle4" };
    }

    public class Scales
    {
        public string drop;
        public float? mass;
        public float? growthRate;//growth gained per second, is full when growth=1
        public string atmosphere = "Void";
        public int? levelCount;
    }

    ///<field=probability>0..100</field>
    public class Expulsion
    {
        public string element;
        public float? probability;//0..100
        public float? cellTargetMass;
        public float? massPerDirt;
        public float? onDeath;
        public string diseaseId;
        public float? diseaseAmount;
    }

    public class Light
    {
        public Color? color;
        public float? range;
        public int? lux;
    }

    public class EggModifiers
    {
        public string id;
        public string eggTag;
        public float weight;

        public string[] foodTags;

        public string nearbyCreature;

        public float? minTemperature;
        public float? maxTemperature;

        public bool alsoInvert;

        public static float multiplicator = 10000000f;
        public static float multiplicator2 = 0.0000001f;

        #region constructors
        public EggModifiers()
        {
        }

        public EggModifiers(string id, string eggTag, float weight, params string[] foodTags)
        {
            this.id = id;
            this.eggTag = eggTag;
            this.weight = weight * multiplicator;
            this.foodTags = foodTags;
        }

        public EggModifiers(string id, string eggTag, float weight, string nearbyCreature, bool alsoInvert)
        {
            this.id = id;
            this.eggTag = eggTag;
            this.weight = weight * multiplicator;
            this.nearbyCreature = nearbyCreature;
            this.alsoInvert = alsoInvert;
        }

        public EggModifiers(string id, string eggTag, float weight, float minTemperature, float maxTemperature)
        {
            this.id = id;
            this.eggTag = eggTag;
            this.weight = weight * multiplicator;
            this.minTemperature = minTemperature;
            this.maxTemperature = maxTemperature;
        }
        #endregion constructors

        public System.Action Convert()
        {
            if (id == null)
                throw new InvalidOperationException("[CustomizeCritter] EggModifiers must set id");
            if (eggTag == null)
                throw new InvalidOperationException("[CustomizeCritter] EggModifiers must set eggTag");
            if (weight == 0f)
                Debug.Log("[CustomizeCritter] Warning: EggModifiers weight is 0");
            if ((foodTags != null ? 1 : 0) + (nearbyCreature != null ? 1 : 0) + (minTemperature != null || maxTemperature != null ? 1 : 0) > 1)
                Debug.Log("[CustomizeCritter] Warning: EggModifiers has set multiple modifiers. Only one will be applied!");

            if (foodTags != null && foodTags.Length > 0)
                return (System.Action)CreateDietaryModifier.Invoke(null, new object[] { id, eggTag.ToTag(), new TagBits(foodTags.Select(x => x.ToTag()).ToArray()), weight * multiplicator2 }); //string id, Tag eggTag, TagBits foodTags, float modifierPerCal
            else if (nearbyCreature != null && nearbyCreature != "")
                return (System.Action)CreateNearbyCreatureModifier.Invoke(null, new object[] { id, eggTag.ToTag(), nearbyCreature.ToTag(), weight * multiplicator2, alsoInvert }); //string id, Tag eggTag, Tag nearbyCreature, float modifierPerSecond, bool alsoInvert
            else if (maxTemperature != null && minTemperature != null)
                return (System.Action)CreateTemperatureModifier.Invoke(null, new object[] { id, eggTag.ToTag(), minTemperature.Value, maxTemperature.Value, weight * multiplicator2, alsoInvert }); //string id, Tag eggTag, float minTemp, float maxTemp, float modifierPerSecond, bool alsoInvert

            Debug.Log("[CustomizeCritter] Warning: Empty EggModifier");
            return Empty;
        }

        public static MethodInfo CreateDietaryModifier = AccessTools.Method(typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS), "CreateDietaryModifier", new Type[] { typeof(string), typeof(Tag), typeof(TagBits), typeof(float) });
        public static MethodInfo CreateNearbyCreatureModifier = AccessTools.Method(typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS), "CreateNearbyCreatureModifier");
        public static MethodInfo CreateTemperatureModifier = AccessTools.Method(typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS), "CreateTemperatureModifier");

        public static System.Action Empty = delegate () { };

        public static List<EggModifiers> Defaults()
        {
            var list = new List<EggModifiers>();

            list.Add(new EggModifiers("HatchHard", "HatchHardEgg", 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE, "SedimentaryRock"));
            list.Add(new EggModifiers("HatchVeggie", "HatchVeggieEgg", 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE, "Dirt"));
            list.Add(new EggModifiers("HatchMetal", "HatchMetalEgg", 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE, "Cuprite", "GoldAmalgam", "IronOre", "Wolframite"));
            list.Add(new EggModifiers("LightBugOrange", "LightBugOrangeEgg", 0.00125f, "GrilledPrickleFruit"));
            list.Add(new EggModifiers("LightBugPurple", "LightBugPurpleEgg", 0.00125f, "FriedMushroom"));
            list.Add(new EggModifiers("LightBugPink", "LightBugPinkEgg", 0.00125f, "SpiceBread"));
            list.Add(new EggModifiers("LightBugBlue", "LightBugBlueEgg", 0.00125f, "Salsa"));
            list.Add(new EggModifiers("LightBugBlack", "LightBugBlackEgg", 0.00125f, "Phosphorus"));
            list.Add(new EggModifiers("LightBugCrystal", "LightBugCrystalEgg", 0.00125f, "CookedMeat"));
            list.Add(new EggModifiers("DreckoPlastic", "DreckoPlasticEgg", 0.025f / DreckoTuning.STANDARD_CALORIES_PER_CYCLE, "BasicSingleHarvestPlant"));

            list.Add(new EggModifiers("PuftAlphaBalance", "PuftAlphaEgg", -0.00025f, "PuftAlpha", true));
            list.Add(new EggModifiers("PuftAlphaNearbyOxylite", "PuftOxyliteEgg", 8.333333E-05f, "PuftAlpha", false));
            list.Add(new EggModifiers("PuftAlphaNearbyBleachstone", "PuftBleachstoneEgg", 8.333333E-05f, "PuftAlpha", false));

            list.Add(new EggModifiers("OilFloaterHighTemp", "OilfloaterHighTempEgg", 8.333333E-05f, 373.15f, 523.15f));
            list.Add(new EggModifiers("OilFloaterDecor", "OilfloaterDecorEgg", 8.333333E-05f, 293.15f, 333.15f));
            list.Add(new EggModifiers("PacuTropical", "PacuTropicalEgg", 8.333333E-05f, 308.15f, 353.15f));
            list.Add(new EggModifiers("PacuCleaner", "PacuCleanerEgg", 8.333333E-05f, 243.15f, 278.15f));

            return list;
        }
    }

}