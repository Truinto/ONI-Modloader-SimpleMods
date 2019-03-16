using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace FastTravelTube
{
	[HarmonyPatch(typeof(Game), "OnPrefabInit")]
	internal class FastTravelTubeMod_Game_OnPrefabInit
	{
		private static void Postfix(Game __instance)
		{
			if (!FastTravelTubeState.StateManager.State.Enabled) return;

			Debug.Log(" === FastTravelTubeMod_Game_OnPrefabInit Postfix === ");

		}
    }

    /*[HarmonyPatch(typeof(BipedTransitionLayer), "BipedTransitionLayer")]
    internal class FastTravelTube_BipedTransitionLayer_BipedTransitionLayer
    {
        private static void Postfix(ref float ___tubeSpeed)
        {
            ___tubeSpeed = 54f;
        }
    }*/

    [HarmonyPatch(typeof(BipedTransitionLayer), "BeginTransition")]
    internal class FastTravelTube_BipedTransitionLayer_BeginTransition
    {
        private static void Prefix(ref float ___tubeSpeed)
        {
            ___tubeSpeed = FastTravelTubeState.StateManager.State.TubeSpeed;
        }
    }
}
