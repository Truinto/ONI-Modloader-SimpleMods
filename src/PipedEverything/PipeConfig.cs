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
        public string? Id;
        public bool Input;
        public int OffsetX;
        public int OffsetY;
        public string[] Filter = [];

        public Color32? Color;
        public Color32? ColorBackground;
        public Color32? ColorBorder;
        public int? StorageIndex;
        public float? StorageCapacity;
        public bool? RemoveMaxAtmosphere;
        public Port? OriginalPort;

        public PipeConfig() { }

        public PipeConfig(string id, bool input, int x, int y, params string[] filter)
        {
            this.Id = id;
            this.Input = input;
            this.OffsetX = x;
            this.OffsetY = y;
            this.Filter = filter;
        }

        public PipeConfig(string id, bool input, int x, int y, params SimHashes[] filter)
        {
            this.Id = id;
            this.Input = input;
            this.OffsetX = x;
            this.OffsetY = y;
            this.Filter = filter.Select(s => s.ToString()).ToArray();
        }
    }

    public enum Port
    {
        Extra1,
        Extra2,
        Extra3,
        Extra4,
        Utility = 100,
    }
}
