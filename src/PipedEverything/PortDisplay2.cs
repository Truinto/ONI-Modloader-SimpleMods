using UnityEngine;
using Common;
using static LogicPorts;
using System.Linq;
using System.Collections.Generic;

namespace PipedEverything
{
    [SkipSaveFileSerialization]
    public class PortDisplay2 : KMonoBehaviour
    {
        private static List<(Sprite sprite, Color color, Color background, Color border)> sprites_in = new();
        private static List<(Sprite sprite, Color color, Color background, Color border)> sprites_out = new();

        private GameObject portObject;

        private int lastUtilityCell = -1;

        [SerializeField]
        public ConduitType type;

        [SerializeField]
        public CellOffset offset;

        [SerializeField]
        public bool input;

        [SerializeField]
        public Color32 color;

        [SerializeField]
        public Sprite sprite;

        [SerializeField]
        public SimHashes[] filter;

        [SerializeField]
        public Tag[] tags;

        [SerializeField]
        public int storageIndex;

        [SerializeField]
        public float storageCapacity;

        private Storage storage;

        public Storage Storage => storage ??= GetComponents<Storage>()[this.storageIndex];

        public void AssignPort(PortDisplayInfo port)
        {
            this.type = port.type;
            this.offset = port.offset;
            this.input = port.input;
            this.color = port.color;
            this.sprite = GetSprite(port);
            this.filter = port.filters;
            this.tags = port.filterTags;
            this.storageIndex = port.StorageIndex;
            this.storageCapacity = port.StorageCapacity;
        }

        private Sprite GetSprite(PortDisplayInfo port)
        {
            // if there is no color mixing, we can just use the old method
            var sprite_base = this.input ? BuildingCellVisualizerResources.Instance().liquidInputIcon : BuildingCellVisualizerResources.Instance().liquidOutputIcon;
            if (port.background == Color.black && port.color == port.border)
                return sprite_base;

            // otherwise we do not use tint and instead pre-render the sprite in the colors we need
            this.color = Color.white;

            var sprites = this.input ? sprites_in : sprites_out;
            var sprite_colored = sprites.Find(f => f.color == port.color && f.background == port.background && f.border == port.border).sprite;
            if (sprite_colored == null)
            {
                sprite_colored = CreateSpriteColor(sprite_base, port.color, port.background, port.border);
                sprites.Add((sprite_colored, port.color, port.background, port.border));
            }

            return sprite_colored;
        }

        private Sprite CreateSpriteColor(Sprite sprite_base, Color color, Color background, Color border)
        {
            Texture2D source_texture = MakeTextureReadable(sprite_base.texture);

            /*var path = @"D:\Users\Fumihiko\Desktop\spritemap.png";
            if (!System.IO.File.Exists(path))
            {
                var bmp = new System.Drawing.Bitmap(texture.width, texture.height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                for (int i = 0; i < texture.width; i++)
                {
                    for (int j = 0; j < texture.height; j++)
                    {
                        var p1 = texture.GetPixel(i, j);

                        //Helpers.PrintDebug($"pixel {i},{j}: {p1.a} - {p1.r} - {p1.g} - {p1.b}");

                        var p2 = System.Drawing.Color.FromArgb(
                            (int)255,
                            (int)(p1.r * 255),
                            (int)(p1.g * 255),
                            (int)(p1.b * 255)
                        );
                        bmp.SetPixel(i, j, p2);
                    }
                }
                bmp.Save(path);
            }*/

            int x = (int)sprite_base.textureRect.x;
            int y = (int)sprite_base.textureRect.y;
            int w = (int)sprite_base.textureRect.width;
            int h = (int)sprite_base.textureRect.height;
            int ml = 20;
            int mh = w - 20;
            var target_texture = new Texture2D(w, h);
            target_texture.filterMode = FilterMode.Point;
            target_texture.wrapMode = TextureWrapMode.Clamp;

            //Helpers.PrintDebug($"x={x} y={y} w={w} h={h}");
            //Helpers.PrintDebug($"tx={sprite_base.textureRect.x} ty={sprite_base.textureRect.y} tw={sprite_base.textureRect.width} th={sprite_base.textureRect.height}");
            //Helpers.PrintDebug($"ox={sprite_base.textureRectOffset.x} oy={sprite_base.textureRectOffset.y}");

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    var p = source_texture.GetPixel(i + x, j + y);

                    if (p.r < 0.5f)     // if color is 50% dark, invert it and tint with background                    
                        target_texture.SetPixel(i, j, p.Invert() * background);                    
                    else if (i < ml || i > mh || j < ml || j > mh)
                        target_texture.SetPixel(i, j, p * border);     // if in border, tint with border
                    else
                        target_texture.SetPixel(i, j, p * color);      // otherwise, tint with color
                }
            }

            target_texture.Apply();
            return Sprite.Create(target_texture, new(0, 0, w, h), new(0.5f, 0.5f));
        }

        private static Texture2D MakeTextureReadable(Texture2D source)
        {
            //https://stackoverflow.com/questions/44733841/how-to-make-texture2d-readable-via-script
            RenderTexture render = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, render);
            var previous = RenderTexture.active;
            RenderTexture.active = render;
            var readable_texture = new Texture2D(source.width, source.height);
            readable_texture.ReadPixels(new Rect(0, 0, render.width, render.height), 0, 0);
            //readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(render);
            return readable_texture;
        }

        public void Draw(GameObject obj, BuildingCellVisualizer visualizer, bool force)
        {
            int utilityCell = visualizer.building.GetCellWithOffset(this.offset);

            // redraw if anything changed
            if (force || utilityCell != this.lastUtilityCell)
            {
                this.lastUtilityCell = utilityCell;
                visualizer.DrawUtilityIcon(utilityCell, this.sprite, ref portObject, color/*Color.white*/);
            }
        }

        public void DisableIcons()
        {
            if (this.portObject != null)
            {
                if (this.portObject != null && this.portObject.activeInHierarchy)
                {
                    this.portObject.SetActive(false);
                }
            }
        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
            if (this.portObject != null)
            {
                UnityEngine.Object.Destroy(this.portObject);
            }
        }

        public bool IsConnected()
        {
            if (this.lastUtilityCell < 0)
                this.lastUtilityCell = this.GetCellWithOffset(this.offset);

            int layer = this.type == ConduitType.Gas ? (int)ObjectLayer.GasConduit : this.type == ConduitType.Liquid ? (int)ObjectLayer.LiquidConduit : (int)ObjectLayer.SolidConduit;
            return Grid.Objects[this.lastUtilityCell, layer] != null;
        }

        public float GetCapacity(SimHashes element)
        {
            if (!filter.Contains(element) && !filter.Contains(SimHashes.Void))
                return 0f;

            float capacityElement = this.storageCapacity;
            float capacityStorage = this.Storage.capacityKg;
            foreach (var item in this.Storage.items)
            {
                if (item == null)
                    continue;

                var element2 = item.GetComponent<PrimaryElement>();
                capacityStorage -= element2.Mass;
                if (element == element2.ElementID)
                    capacityElement -= element2.Mass;
            }

            return Mathf.Min(capacityElement, capacityStorage);
        }

        public bool IsBlocked()
        {
            if (!IsConnected())
                return false;

            float capacityElement = this.storageCapacity;
            foreach (var item in this.Storage.items)
            {
                if (item == null)
                    continue;

                var element2 = item.GetComponent<PrimaryElement>();
                if ((this.filter.Contains(SimHashes.Void) && element2.Element.GetConduitType() == this.type || this.filter.Contains(element2.ElementID)) && element2.Mass > capacityElement)
                    return true;
            }
            return false;
        }

        public bool TryStore(Element element, float mass, float temperature)
        {
            //if (GetCapacity(element.id) < 0)
            //    return false;

            if (element.IsGas)
                this.Storage.AddGasChunk(element.id, mass, temperature, 0, 0, keep_zero_mass: true);
            else if (element.IsLiquid)
                this.Storage.AddLiquid(element.id, mass, temperature, 0, 0, keep_zero_mass: true);

            return true;
        }
    }
}
