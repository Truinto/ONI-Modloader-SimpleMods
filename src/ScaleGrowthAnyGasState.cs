using System.Collections.Generic;
using ONI_Common.Json;

namespace ScaleGrowthAnyGas
{
    public class ScaleGrowthAnyGasState
    {
        public bool Enabled { get; set; } = true;

		//public int Parameter { get; set; } = 512;


		public static BaseStateManager<ScaleGrowthAnyGasState> StateManager
			= new BaseStateManager<ScaleGrowthAnyGasState>("Template");
	}
}