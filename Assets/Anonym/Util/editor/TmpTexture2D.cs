using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Util
{    
    [System.Serializable]
    public class TmpTexture2D
    {
        [SerializeField]
        public Texture2D texture = null;

        public bool bCorrupted = false;

        public void SetIcon(Texture2D newIcon)
        {
            texture = newIcon;
        }

        public Texture2D MakeIcon(ref Sprite[] sprites, ref Color[] colors)
        {
            string conditions = sprites.Select(s => s.name).Concat(colors.Select(c => c.ToString())).Aggregate((i, j) => i + j);
            if (texture == null || !texture.name.Equals(conditions))
            {
                Texture2D _texture = null;
                bool bAccumulate = false;
                for (int i = 0; i < sprites.Length; ++i)
                {
                    Sprite sprite = sprites[i];
                    if (sprite == null)
                        continue;

                    Color color = colors != null && colors.Length > i ? colors[i] : Color.white;
                    bAccumulate |= OverrideColoredTexture2D(sprite.texture, sprite.textureRect, ref _texture, color, bAccumulate);
                }
                _texture.name = conditions;
                return _texture;
            }
            return null;
        }

        public void DrawRect(Rect rect)
        {
            if (texture != null)
                GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true);
        }

        private static bool OverrideColoredTexture2D(Texture2D source, Rect sourceRect, ref Texture2D destination, Color color, bool bAccumulate)
        {
            int s_x = Mathf.RoundToInt(sourceRect.x);
            int s_y = Mathf.RoundToInt(sourceRect.y);
            int s_width = Mathf.RoundToInt(sourceRect.width);
            int s_height = Mathf.RoundToInt(sourceRect.height);

            if (destination == null)
            {
                destination = new Texture2D(s_width, s_height, TextureFormat.RGBA32, false);
                destination.alphaIsTransparency = true;
                destination.filterMode = FilterMode.Point;
            }
            else
            {
                s_width = Mathf.Min(s_width, destination.width);
                s_height = Mathf.Min(s_height, destination.height);
            }

            int d_width = destination.width;
            int d_height = destination.height;
            int d_x = (d_width - s_width) / 2;
            int d_y = (d_height - s_height) / 2;

            Texture2D sourceForRead = source;
            // if (source is compressed)
            {
                RenderTexture rt = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB32);
                RenderTexture.active = rt;
                Graphics.Blit(source, rt);

                sourceForRead = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
                sourceForRead.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
            }

            var rawdata = sourceForRead.GetRawTextureData();
            Color32[] destColor = bAccumulate ? destination.GetPixels32() : null;
            Color32[] newcolors = new Color32[s_width * s_height];
            Color col;
            int index_new_color;
            int index_source_raw;
            int index_dest_raw;

            for (int _y = 0; _y < s_height; ++_y)
            {
                for (int _x = 0; _x < s_width; _x++)
                {
                    index_new_color = _x + _y * s_width;
                    index_source_raw = 4 * ((s_x + _x) + (s_y + _y) * sourceForRead.width);
                    index_dest_raw = (d_x + _x) + (d_y + _y) * d_width;

                    col = new Color32(rawdata[index_source_raw], rawdata[index_source_raw + 1], rawdata[index_source_raw + 2], rawdata[index_source_raw + 3]) * color;
                    newcolors[index_new_color] = bAccumulate ? CustomEditorGUI.Color_AlphaBlend(col, destColor[index_dest_raw]) : col;
                }
            }

            destination.SetPixels32(d_x, d_y, s_width, s_height, newcolors);
            destination.Apply();
            return newcolors.Length > 0;
        }
    }
}