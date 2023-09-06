using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PipedEverything
{
    public class PipeConfig
    {
        public string Id;
        public int OffsetX;
        public int OffsetY;
        public string[] Filter;

        public Color32? Color;
        public int? StorageIndex;
        public int? StorageCapacity;

        public PipeConfig() { }
    }
}
