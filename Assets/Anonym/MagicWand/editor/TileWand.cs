using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Util
{
    using Isometric;

    [CreateAssetMenu(fileName = "New Tile Wand", menuName = "Anonym/Magic Wand/Tile", order = 998)]
    public class TileWand : MagicWand
    {
        [SerializeField]
        public GameObject Prefab;

        [SerializeField]
        public TmpTexture2D textureForGUI = new TmpTexture2D();

        [SerializeField, HideInInspector]
        private Sprite[] sprites = null;

        [SerializeField, HideInInspector]
        private Color[] colors = null;

        public IsoTile Tile { get { return Prefab == null ? null : Prefab.GetComponent<IsoTile>(); } }

#if UNITY_EDITOR

        public override ParamType[] Params
        {
            get
            {
                return new ParamType[] {ParamType.New, ParamType.Position, ParamType.Parts, ParamType.AutoIsoLight , ParamType.IsoBulk};
            }
        }

        SpriteRenderer[] GetSpriteRenderers()
        {
            if (Prefab != null)
                return Prefab.GetComponentsInChildren<SpriteRenderer>();
            return null;
        }

        public override Texture[] GetTextures()
        {
            var sprr = GetSpriteRenderers();
            if (sprr != null)
                return sprr.Select(s => s.sprite.texture).ToArray();
            return null;
        }

        public override Color[] GetColors()
        {
            var sprr = GetSpriteRenderers();
            if (sprr != null)
                return sprr.Select(s => s.color).ToArray();
            return null;
        }
        private void OnEnable()
        {
            textureForGUI.bCorrupted = true;
        }

        protected override void OnCustomGUI(Rect rect)
        {
            if (textureForGUI.bCorrupted == true)
                MakeIcon(this);

            textureForGUI.DrawRect(rect);
            return;
        }

        public override GameObject TargetGameObject(GameObject target)
        {
            IsoTile tile = IsoTile.Find(target);
            return tile != null ? tile.gameObject : null;
        }

        public override bool MakeUp(ref GameObject target, params object[] values)
        {
            bool bAutoCreation = (bool)values[0];
            Vector3 vAt = (Vector3)values[1];
            bool bBody = (bool)values[2];
            bool bAttachments = (bool)values[3];
            bool bAutoIsoLight = (bool)values[4];
            IsoTile refTile = Tile;
            IsoTileBulk refBulk = (IsoTileBulk)values[5];

            if (target == null)
            {
                if (!bAutoCreation)
                    return false;

                return TileControlWand.Tile_Create(ref target, vAt, refTile, bBody, bAttachments, bAutoIsoLight, refBulk);
            }

            IsoTile TargetTile = IsoTile.Find(target);

            if (refTile == null || TargetTile == null)
                return false;

            // Undo.Record() code is aleady in Copycat()
            TargetTile.Copycat(refTile, bBody, bAttachments, true);
            target = TargetTile.gameObject;
            return true;
        }

        public static bool MakeIcon(TileWand tileWand)
        {
            if (tileWand != null && tileWand.textureForGUI != null)
            {
                tileWand.sprites = tileWand.GetSpriteRenderers().Select(s => s.sprite).ToArray();
                tileWand.colors = tileWand.GetColors().ToArray();

                Texture2D _tex = tileWand.textureForGUI.MakeIcon(ref tileWand.sprites, ref tileWand.colors);
                if (_tex == null)
                {
                    tileWand.textureForGUI.bCorrupted = false;
                    return true;
                }

                if (tileWand.textureForGUI.texture == null || !AssetDatabase.Contains(tileWand.textureForGUI.texture))
                {
                    AssetDatabase.AddObjectToAsset(_tex, tileWand);
                    tileWand.textureForGUI.texture = _tex;
                }
                else
                {
                    EditorUtility.CopySerialized(_tex, tileWand.textureForGUI.texture);
                }
                tileWand.textureForGUI.bCorrupted = false;

                EditorUtility.SetDirty(tileWand.textureForGUI.texture);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return true;
            }
            return false;
        }
        public static TileWand CreateAsset(string path, IsoTile tile)
        {
            TileWand newTileWand = ScriptableObject.CreateInstance<TileWand>();

            string _path = AssetDatabase.GenerateUniqueAssetPath(path + ".asset");
            AssetDatabase.CreateAsset(newTileWand, _path);

            GameObject tileGameObject = tile.gameObject;
            if (!PrefabHelper.IsPrefab(tileGameObject))
                tileGameObject = PrefabHelper.CreatePrefab(AssetDatabase.GenerateUniqueAssetPath(path + ".prefab"), tileGameObject);

            // Revert All Lightings
            tile = IsoTile.Find(tileGameObject);
            tile.LightRecivers_RevertAll();
            tile.LightRecivers_RemoveAll(false);

            IsometricSortingOrder iso = tile.GetComponent<IsometricSortingOrder>();
            iso.Clear_Backup(false);

            newTileWand.Prefab = tileGameObject;

            MakeIcon(newTileWand);

            if (newTileWand != null)
            {
                // AssetDatabase.AddObjectToAsset(tilePrefab, newTileWand);            
                EditorUtility.SetDirty(newTileWand);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (newTileWand == null)
                newTileWand = AssetDatabase.LoadAssetAtPath<TileWand>(_path);

            return newTileWand;
        }
#endif
    }
}