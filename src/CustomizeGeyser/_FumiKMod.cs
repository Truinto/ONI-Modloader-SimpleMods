using Common;
using HarmonyLib;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeGeyser
{
    public class FumiKMod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            Helpers.ModName = "CustomizeGeyser";
            CustomStrings.LoadStrings();
            Helpers.StringsAddClass(typeof(GeyserStruct));
#if LOCALE
            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.MOLTEN_STEEL.NAME"] = "Steel Volcano";
            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.MOLTEN_STEEL.DESC"] = "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Steel", "MOLTENSTEEL") + ".";

            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.MOLTEN_GLASS.DESC"] = "Glass Volcano";
            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.MOLTEN_GLASS.DESC"] = "A large volcano that periodically erupts with molten " + STRINGS.UI.FormatAsLink("Glass", "MOLTENGLASS") + ".";

            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.LIQUID_COOLANT.DESC"] = "Super Coolant Geyser";
            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.LIQUID_COOLANT.DESC"] = "A highly pressurized geyser that periodically erupts with hot " + STRINGS.UI.FormatAsLink("Super Coolant", "SUPERCOOLANT") + ".";

            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.LIQUID_ETHANOL.DESC"] = "Ethanol Geyser";
            Helpers.StringsDic["STRINGS.CREATURES.SPECIES.GEYSER.LIQUID_ETHANOL.DESC"] = "A highly pressurized geyser that periodically erupts with boiling " + STRINGS.UI.FormatAsLink("Ethanol", "ETHANOL") + ".";
            Helpers.StringsPrint();
#else
            Helpers.StringsLoad();
#endif

            new POptions().RegisterOptions(this, typeof(CustomizeGeyserState));

            CustomizeGeyserState.OnLoad();
            base.OnLoad(harmony);
        }
    }
}