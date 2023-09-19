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
        public bool Input;
        public int OffsetX;
        public int OffsetY;
        public string[] Filter;

        public Color32? Color;
        public int? StorageIndex;
        public float? StorageCapacity;

        public PipeConfig() { }

        public PipeConfig(string id, bool input, int x, int y, SimHashes filter)
        {
            this.Id = id;
            this.Input = input;
            this.OffsetX = x;
            this.OffsetY = y;
            this.Filter = filter == 0 ? Array.Empty<string>() : new string[] { filter.ToString() };
        }

        public PipeConfig(string id, bool input, int x, int y, string filter)
        {
            this.Id = id;
            this.Input = input;
            this.OffsetX = x;
            this.OffsetY = y;
            this.Filter = new string[] { filter.ToString() };
        }

        public PipeConfig(string id, bool input, int x, int y, params SimHashes[] filter)
        {
            this.Id = id;
            this.Input = input;
            this.OffsetX = x;
            this.OffsetY = y;
            this.Filter = filter.Select(s => s.ToString()).ToArray();
        }

        public PipeConfig(string id, bool input, int x, int y, IEnumerable<Element> filter)
        {
            this.Id = id;
            this.Input = input;
            this.OffsetX = x;
            this.OffsetY = y;
            this.Filter = filter.Select(s => s.id.ToString()).ToArray();
        }
    }
}
