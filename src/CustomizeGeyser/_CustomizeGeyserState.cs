using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace CustomizeGeyser
{
    [ConfigFile("CustomizeGeyser.json", true, true, typeof(Config.TranslationResolver))]
    [RestartRequired]
    public class CustomizeGeyserState : IManualConfig
    {
        public static void OnLoad()
        {
            try
            {
                string temp = Path.Combine(Config.PathHelper.AssemblyDirectory, "templates");
                string temp2 = Path.Combine(Config.PathHelper.AssemblyDirectory, "templates2");

                if (CustomizeGeyserState.StateManager.State.RandomizerEnabled)
                {
                    if (Directory.Exists(temp2))
                    {
                        Directory.Move(temp2, temp);
                    }
                }
                else
                {
                    if (Directory.Exists(temp))
                    {
                        Directory.Move(temp, temp2);
                    }
                }
            } catch (Exception e)
            {
                Helpers.Print(e.ToString());
            }
        }

        #region Buttons
        [Option("CustomizeGeyser.LOCSTRINGS.ResetToCustomDefault_Title", "CustomizeGeyser.LOCSTRINGS.ResetToCustomDefault_ToolTip", "Buttons", null)]
        [JsonIgnore]
        public System.Action<object> ResetToCustomDefault => delegate (object nix)
        {
            StateManager.TrySaveConfigurationState(new CustomizeGeyserState());
            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last?.CheckForRestart();
            OptionsDialog.Last = null;
        };

        [Option("CustomizeGeyser.LOCSTRINGS.ResetToAllOff_Title", "CustomizeGeyser.LOCSTRINGS.ResetToAllOff_ToolTip", "Buttons", null)]
        [JsonIgnore]
        public System.Action<object> ResetToAllOff => delegate (object nix)
        {
            StateManager.State.Geysers.Clear();
            StateManager.State.RNGTable.Clear();
            foreach (var idBase in GeyserInfo.IdsBaseGame)
                StateManager.State.RNGTable.Add(idBase, 1);

            StateManager.State.RandomizerEnabled = false;
            StateManager.State.RandomizerUsesMapSeed = true;
            StateManager.State.RandomizerRerollsCycleRate = false;
            StateManager.State.RandomizerPopupGeyserDiscoveryInfo = false;
            StateManager.State.RandomizerHighlanderMode = false;
            StateManager.State.RandomizerHighlanderRetroactive = false;
            StateManager.State.RandomizerEnabled = false;
            StateManager.State.GeyserMorphEnabled = false;

            StateManager.TrySaveConfigurationState();
            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last?.CheckForRestart();
            OptionsDialog.Last = null;
        };

        [Option("CustomizeGeyser.LOCSTRINGS.PresetNoDormancy_Title", "CustomizeGeyser.LOCSTRINGS.PresetNoDormancy_ToolTip", "Buttons", null)]
        [JsonIgnore]
        public System.Action<object> PresetNoDormancy => delegate (object nix)
        {
            foreach (var geyser in GeyserInfo.IdsBaseGame)
            {
                if (!StateManager.State.Geysers.Any(a => a.id == geyser))
                    StateManager.State.Geysers.Add(new GeyserStruct(geyser));
            }

            foreach (var geyser in StateManager.State.Geysers)
            {
                geyser.minYearPercent = 1f;
                geyser.maxYearPercent = 1f;
            }

            StateManager.TrySaveConfigurationState();
            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last?.CheckForRestart();
            OptionsDialog.Last = null;
        };


        [Option("CustomizeGeyser.LOCSTRINGS.PresetCopy_Title", "CustomizeGeyser.LOCSTRINGS.PresetCopy_ToolTip", "Buttons", null)]
        [JsonIgnore]
        public System.Action<object> PresetCopy => delegate (object nix)
        {
            StateManager.State.Geysers.Clear();
            foreach (var config in GeyserInfo.Config)
            {
                var geyserType = config.geyserType;
                StateManager.State.Geysers.Add(new GeyserStruct(
                    id: geyserType.id,
                    element: geyserType.element.ToString(),
                    anim: config.anim,
                    width: config.width,
                    height: config.height,
                    temperature: geyserType.temperature,
                    minRatePerCycle: geyserType.minRatePerCycle,
                    maxRatePerCycle: geyserType.maxRatePerCycle,
                    maxPressure: geyserType.maxPressure,
                    minIterationLength: geyserType.minIterationLength,
                    maxIterationLength: geyserType.maxIterationLength,
                    minIterationPercent: geyserType.minIterationPercent,
                    maxIterationPercent: geyserType.maxIterationPercent,
                    minYearLength: geyserType.minYearLength,
                    maxYearLength: geyserType.maxYearLength,
                    minYearPercent: geyserType.minYearPercent,
                    maxYearPercent: geyserType.maxYearPercent,
                    Name: Strings.Get(config.nameStringKey),
                    Description: Strings.Get(config.descStringKey),
                    Disease: geyserType.diseaseInfo.idx == byte.MaxValue ? null : Db.Get().Diseases[geyserType.diseaseInfo.idx].Id,
                    DiseaseCount: geyserType.diseaseInfo.count == 0 ? null : geyserType.diseaseInfo.count,
                    IsGeneric: config.isGenericGeyser,
                    shape: geyserType.shape
                ));
            }

            StateManager.TrySaveConfigurationState();
        };

        [Option("CustomizeGeyser.LOCSTRINGS.EnableAllGeysers_Title", "CustomizeGeyser.LOCSTRINGS.EnableAllGeysers_ToolTip", "Buttons", null)]
        [JsonIgnore]
        public System.Action<object> EnableAllGeysers => delegate (object nix)
        {
            foreach (var config in GeyserInfo.Config)
            {
                if (!StateManager.State.RNGTable.ContainsKey(config.geyserType.id))
                {
                    StateManager.State.RNGTable[config.geyserType.id] = 1;
                }
            }

            //foreach (var geyser in StateManager.State.Geysers)
            //    if (!StateManager.State.RNGTable.ContainsKey(geyser.id))
            //        StateManager.State.RNGTable[geyser.id] = 1;

            foreach (var key in StateManager.State.RNGTable.Keys.ToArray())
            {
                StateManager.State.RNGTable[key] = 1;
            }

            StateManager.TrySaveConfigurationState();
            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last?.CheckForRestart();
            OptionsDialog.Last = null;
        };
        #endregion

        #region Fields
        public int version { get; set; } = 10;
        [Option("CustomizeGeyser.LOCSTRINGS.Enabled_Title", "CustomizeGeyser.LOCSTRINGS.Enabled_ToolTip", "Fields", null)]
        public bool Enabled { get; set; } = true;

        public HashSet<GeyserStruct> Geysers { get; set; } = new HashSet<GeyserStruct>() {
            new GeyserStruct(id: "steam", temperature: 378.15f),
            new GeyserStruct(id: "slimy_po2", temperature: 378.15f, Disease: "ZombieSpores", DiseaseCount: 5000),
            new GeyserStruct(id: "molten_tungsten", anim: "geyser_molten_tungsten_kanim", element: "MoltenTungsten",
                Name: "Tungsten Volcano", Description: "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Tungsten", "MOLTENTUNGSTEN") + ".",
                temperature: 3773.15f, minRatePerCycle: 200f, maxRatePerCycle: 400f, maxPressure: 150f, minIterationLength: 480f,
                maxIterationLength: 1080f, minIterationPercent: 0.02f, maxIterationPercent: 0.1f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f),
            new GeyserStruct(id: "molten_aluminum", anim: "geyser_molten_aluminum_kanim", element: "MoltenAluminum",
                Name: "Aluminum Volcano", Description: "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Aluminum", "MOLTENALUMINUM") + ".",
                temperature: 2273.15f, minRatePerCycle: 200f, maxRatePerCycle: 400f, maxPressure: 150f, minIterationLength: 480f,
                maxIterationLength: 1080f, minIterationPercent: 0.02f, maxIterationPercent: 0.1f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f),
            new GeyserStruct(id: "molten_steel", anim: "geyser_molten_iron_kanim", element: "MoltenSteel",
                Name: "Steel Volcano", Description: "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Steel", "MOLTENSTEEL") + ".",
                temperature: 2773.15f, minRatePerCycle: 200f, maxRatePerCycle: 400f, maxPressure: 150f, minIterationLength: 480f,
                maxIterationLength: 1080f, minIterationPercent: 0.02f, maxIterationPercent: 0.1f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f),
            new GeyserStruct(id: "molten_glass", anim: "geyser_molten_iron_kanim", element: "MoltenGlass",
                Name: "Glass Volcano", Description: "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Glass", "MOLTENGLASS") + ".",
                temperature: 2273.15f, minRatePerCycle: 200f, maxRatePerCycle: 400f, maxPressure: 150f, minIterationLength: 480f,
                maxIterationLength: 1080f, minIterationPercent: 0.02f, maxIterationPercent: 0.1f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f),
            new GeyserStruct(id: "liquid_coolant", anim: "geyser_liquid_water_slush_kanim", element: "SuperCoolant",
                Name: "Super Coolant Geyser", Description: "A highly pressurized geyser that periodically erupts with hot " + STRINGS.UI.FormatAsLink("Super Coolant", "SUPERCOOLANT") + ".",
                temperature: 673.15f, minRatePerCycle: 200f, maxRatePerCycle: 400f, maxPressure: 500f, minIterationLength: 60f,
                maxIterationLength: 1140f, minIterationPercent: 0.1f, maxIterationPercent: 0.9f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f),
            new GeyserStruct(id: "liquid_ethanol", anim: "geyser_liquid_water_filthy_kanim", element: "Ethanol",
                Name: "Ethanol Geyser", Description: "A highly pressurized geyser that periodically erupts with boiling " + STRINGS.UI.FormatAsLink("Ethanol", "ETHANOL") + ".",
                temperature: 343.15f, minRatePerCycle: 2000f, maxRatePerCycle: 4000f, maxPressure: 500f, minIterationLength: 60f,
                maxIterationLength: 1140f, minIterationPercent: 0.1f, maxIterationPercent: 0.9f, minYearLength: 15000f, maxYearLength: 135000f,
                minYearPercent: 0.4f, maxYearPercent: 0.8f, Disease: "PollenGerms", DiseaseCount: 50)
        };

        [Option("CustomizeGeyser.LOCSTRINGS.RandomizerEnabled_Title", "CustomizeGeyser.LOCSTRINGS.RandomizerEnabled_ToolTip", "Fields", null)]
        public bool RandomizerEnabled { get; set; } = true;
        [Option("CustomizeGeyser.LOCSTRINGS.RandomizerUsesMapSeed_Title", "CustomizeGeyser.LOCSTRINGS.RandomizerUsesMapSeed_ToolTip", "Fields", null)]
        public bool RandomizerUsesMapSeed { get; set; } = false;
        [Option("CustomizeGeyser.LOCSTRINGS.RandomizerRerollsCycleRate_Title", "CustomizeGeyser.LOCSTRINGS.RandomizerRerollsCycleRate_ToolTip", "Fields", null)]
        public bool RandomizerRerollsCycleRate { get; set; } = true;
        [Option("CustomizeGeyser.LOCSTRINGS.RandomizerPopupGeyserDiscoveryInfo_Title", "CustomizeGeyser.LOCSTRINGS.RandomizerPopupGeyserDiscoveryInfo_ToolTip", "Fields", null)]
        public bool RandomizerPopupGeyserDiscoveryInfo { get; set; } = true;
        public bool RandomizerHighlanderMode { get; set; } = false;
        public bool RandomizerHighlanderRetroactive { get; set; } = false;
        public Dictionary<string, int> RNGTable { get; set; } = new Dictionary<string, int>() {
            { "steam", 1 },
            { "hot_steam", 1 },
            { "hot_water", 1 },
            { "slush_water", 1 },
            { "filthy_water", 1 },
            { "slush_salt_water", 1 },
            { "salt_water", 1 },
            { "small_volcano", 1 },
            { "big_volcano", 1 },
            { "liquid_co2", 1 },
            { "hot_co2", 1 },
            { "hot_hydrogen", 1 },
            { "hot_po2", 1 },
            { "slimy_po2", 1 },
            { "chlorine_gas", 1 },
            { "methane", 1 },
            { "molten_copper", 1 },
            { "molten_iron", 1 },
            { "molten_gold", 1 },
            { "oil_drip", 1 },
            { "liquid_sulfur", 1 },
            { "molten_cobalt", 1 },
            { "molten_niobium", 1 },
            { "molten_tungsten", 1 },
            { "molten_aluminum", 1 },
            { "molten_glass", 1 },
            { "liquid_ethanol", 1 },
            { "molten_steel", 0 },
            { "liquid_coolant", 0 }
        };

        [Option("CustomizeGeyser.LOCSTRINGS.GeyserMorphEnabled_Title", "CustomizeGeyser.LOCSTRINGS.GeyserMorphEnabled_ToolTip", "Fields", null)]
        public bool GeyserMorphEnabled { get; set; } = true;
        [Option("CustomizeGeyser.LOCSTRINGS.GeyserMorphWorktime_Title", "CustomizeGeyser.LOCSTRINGS.GeyserMorphWorktime_ToolTip", "Fields", null)]
        public int GeyserMorphWorktime { get; set; } = 300;

        [Option("CustomizeGeyser.LOCSTRINGS.GeyserTeleportEnabled_Title", "CustomizeGeyser.LOCSTRINGS.GeyserTeleportEnabled_ToolTip", "Fields", null)]
        public bool GeyserTeleportEnabled { get; set; } = true;
        #endregion

        #region _implementation

        public static Config.Manager<CustomizeGeyserState> StateManager = null!;
        public static bool OnUpdate(CustomizeGeyserState state)
        {
            return true;
        }
        public object ReadSettings()
        {
            return StateManager.State;
        }

        public void WriteSettings(object settings)
        {
            if (settings is CustomizeGeyserState state)
                StateManager.TrySaveConfigurationState(state);
            else
                StateManager.TrySaveConfigurationState();
        }

        public string GetConfigPath()
        {
            return GetStaticConfigPath();
        }

        public static string GetStaticConfigPath()
        {
            string path = FumiKMod.ModName;
            //if (Helpers.ActiveLocale.NotEmpty() && Helpers.ActiveLocale != "en")
            //    path += "_" + Helpers.ActiveLocale;
            return Config.PathHelper.CreatePath(path);
        }

        #endregion
    }

    public class CustomStrings
    {
        public static void LoadStrings()
        {





            #region Buttons
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.ResetToCustomDefault_Title", "Reset: Custom Default");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.ResetToCustomDefault_ToolTip", "Re-installs mod's default settings.");

            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.ResetToAllOff_Title", "Reset: All Off");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.ResetToAllOff_ToolTip", "Clears all geyser settings and turns off all options.");

            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.PresetNoDormancy_Title", "Preset: No Dormancy");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.PresetNoDormancy_ToolTip", "Adds all geysers with no dormancy period.");

            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.PresetCopy_Title", "Preset: Copy");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.PresetCopy_ToolTip", "Adds all geysers with all options copied from current game.");

            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.EnableAllGeysers_Title", "Preset: Enable All");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.EnableAllGeysers_ToolTip", "Enables all geyser types, even the overpowered ones. Doesn't affect existing saves and they are generally still rare.");
            #endregion
            #region Fields
            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.version", "version");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.Enabled", "Enabled");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.Enabled_Title", "Enable Mod");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.Enabled_ToolTip", "");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.Geysers", "Geysers");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.RandomizerEnabled", "RandomizerEnabled");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.RandomizerEnabled_Title", "Randomizer Enabled");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.RandomizerEnabled_ToolTip", "if set to false will disable all other randomize settings");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.RandomizerUsesMapSeed", "RandomizerUsesMapSeed");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.RandomizerUsesMapSeed_Title", "Randomizer Uses Map Seed");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.RandomizerUsesMapSeed_ToolTip", "if set to true, the same geysers will spawn for a certain set of settings; you still get different results when you change the weights or add new geyser types; if set to false, reloading and rediscovery a geyser may reveal a different geyser");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.RandomizerRerollsCycleRate", "RandomizerRerollsCycleRate");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.RandomizerRerollsCycleRate_Title", "Randomizer Rerolls Cycle Rate");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.RandomizerRerollsCycleRate_ToolTip", "if set to true and RandomizerUsesMapSeed set to false will also change the percentage of cycle output; otherwise output stays consistent");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.RandomizerPopupGeyserDiscoveryInfo", "RandomizerPopupGeyserDiscoveryInfo");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.RandomizerPopupGeyserDiscoveryInfo_Title", "Randomizer Popup Geyser Discovery Info");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.RandomizerPopupGeyserDiscoveryInfo_ToolTip", "generates a popup whenever a geyser is discovered; useful for rerolling/testing Randomize settings do not affect pre-defined geysers, which are some steam, methane, and oil geysers");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.RandomizerHighlanderMode", "RandomizerHighlanderMode");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.RandomizerHighlanderRetroactive", "RandomizerHighlanderRetroactive");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.RNGTable", "RNGTable");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.GeyserMorphEnabled", "GeyserMorphEnabled");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.GeyserMorphEnabled_Title", "Geyser Morph Enabled");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.GeyserMorphEnabled_ToolTip", "When enabled, shows two buttons in the geyser menu. The first will request a scientist dupe to work on it, the second will define which geyser it should be morphed into. Click the second button to cycle through the options.");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.GeyserMorphWorktime", "GeyserMorphWorktime");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.GeyserMorphWorktime_Title", "Geyser Morph Worktime");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.GeyserMorphWorktime_ToolTip", "How long a scientist dupe will need to work on the geyser to morph it.");

            Helpers.StringsAddProperty("CustomizeGeyser.PROPERTY.GeyserTeleportEnabled", "GeyserTeleportEnabled");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.GeyserTeleportEnabled_Title", "Geyser Teleport Enabled");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.GeyserTeleportEnabled_ToolTip", "Teleport selected geyser to mouse cursor with DebugTeleport key (default: ALT+Q)");
            #endregion
        }

        public static void ExtraStrings()
        {
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.Cancel_morphing", "Cancel morphing");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.Cancel_morphing_tooltip", "Cancel this morphing order");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.Morph_into", "Morph geyser into:");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.Morph_into_tooltip", "Morph geyser into another type");
            Helpers.StringsAdd("CustomizeGeyser.LOCSTRINGS.Next_geyser_tooltip", "Click for next geyser type");

            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_steam", "geyser_steam", "Steam Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_hot_steam", "geyser_hot_steam", "Hot Steam Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_hot_water", "geyser_hot_water", "Hot Water Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_slush_water", "geyser_slush_water", "Slush Water Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_filthy_water", "geyser_filthy_water", "Filthy Water Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_salt_water", "geyser_salt_water", "Salt Water Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_small_volcano", "geyser_small_volcano", "Small Volcano");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_big_volcano", "geyser_big_volcano", "Big Volcano");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_liquid_co2", "geyser_liquid_co2", "Liquid CO2 Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_hot_co2", "geyser_hot_co2", "Hot CO2 Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_hot_hydrogen", "geyser_hot_hydrogen", "Hydrogen Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_hot_po2", "geyser_hot_po2", "Poluted Oxygen Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_slimy_po2", "geyser_slimy_po2", "Toxic Oxygen Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_chlorine_gas", "geyser_chlorine_gas", "Chlorine Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_methane", "geyser_methane", "Methane Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_molten_copper", "geyser_molten_copper", "Copper Volcano");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_molten_iron", "geyser_molten_iron", "Iron Volcano");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_molten_gold", "geyser_molten_gold", "Gold Volcano");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_oil_drip", "geyser_oil_drip", "Oil Drip");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_slush_salt_water", "geyser_slush_salt_water", "Slush Salt Water Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_molten_aluminum", "geyser_molten_aluminum", "Aluminum Volcano");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_molten_tungsten", "geyser_molten_tungsten", "Tungsten Volcano");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_molten_niobium", "geyser_molten_niobium", "Niobium Volcano");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_molten_cobalt", "geyser_molten_cobalt", "Cobalt Volcano");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_liquid_sulfur", "geyser_liquid_sulfur", "Sulfur Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_molten_glass", "geyser_molten_glass", "Glass Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_liquid_ethanol", "geyser_liquid_ethanol", "Ethanol Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_molten_steel", "geyser_molten_steel", "Steel Geyser");
            Helpers.StringsTag("CustomizeGeyser.TAG.geyser_liquid_coolant", "geyser_liquid_coolant", "Coolant Geyser");
#if LOCALE
            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.MOLTEN_STEEL.NAME"] = "Steel Volcano";
            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.MOLTEN_STEEL.DESC"] = "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Steel", "MOLTENSTEEL") + ".";

            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.MOLTEN_GLASS.NAME"] = "Glass Volcano";
            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.MOLTEN_GLASS.DESC"] = "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Glass", "MOLTENGLASS") + ".";

            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.LIQUID_COOLANT.NAME"] = "Super Coolant Geyser";
            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.LIQUID_COOLANT.DESC"] = "A highly pressurized geyser that periodically erupts with hot " + STRINGS.UI.FormatAsLink("Super Coolant", "SUPERCOOLANT") + ".";

            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.LIQUID_ETHANOL.NAME"] = "Ethanol Geyser";
            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.LIQUID_ETHANOL.DESC"] = "A highly pressurized geyser that periodically erupts with boiling " + STRINGS.UI.FormatAsLink("Ethanol", "ETHANOL") + ".";
#endif
        }
    }
}
