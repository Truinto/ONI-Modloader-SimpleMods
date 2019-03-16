using System.Collections.Generic;
using ONI_Common.Json;

namespace FastTravelTube
{
    public class FastTravelTubeState
    {
        public bool Enabled { get; set; } = true;

		public float TubeSpeed { get; set; } = 54f;


		public static BaseStateManager<FastTravelTubeState> StateManager
			= new BaseStateManager<FastTravelTubeState>("FastTravelTube");
	}
}