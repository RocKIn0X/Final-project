using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Anonym.Util
{
    using Isometric;

    [System.Serializable]
    public class MasterPaletteWindow : EditorWindow, ISerializationCallbackReceiver
    {
        [System.Serializable]
        class SimpleGridForPalette : CustomEditorGUI.SimpleGrid<MagicWand> { }

        enum TargetType
        {
            MouseOver,
            FixedYAxis,
        }

        #region Member
        [SerializeField]
        AbstractMagicWandPalette selectedPalette;
        [SerializeField]
        AbstractMagicWandPalette defaultPalette;
        [SerializeField]
        AbstractMagicWandPalette defaultTileSetPalette;

        [System.NonSerialized]
        List<AbstractMagicWandPalette> Palettes = new List<AbstractMagicWandPalette>();
        [System.NonSerialized]
        List<SimpleGridForPalette> SimpleGrids = new List<SimpleGridForPalette>();

        [SerializeField]
        List<MagicWand> selection = new List<MagicWand>();
        [SerializeField]
        List<MagicWand> selectionForUse = new List<MagicWand>();

        [SerializeField]
        List<MagicWand.ParamType> paramTypes = new List<MagicWand.ParamType>();

        List<GameObject> makeUpedTargetList = new List<GameObject>();

        // LastTarget
        IsoTile lastTile = null;
        List<GameObject> lastTarget_gameObjects = new List<GameObject>();
        void clearLastTarget()
        {
            lastTarget_gameObjects.Clear();
        }
        void clearLastTargets_Etc()
        {
            var lastOne = lastTarget_gameObjects.Last();
            clearLastTarget();
            lastTarget_gameObjects.Add(lastOne);
        }
        void clearLastTargets_All(Vector3 position)
        {
            clearLastTarget();
            setTargetPos(position);
        }
        void setTargetPos(Vector3 position)
        {
            vTargetCellPos = grid.SnapedPosition(position);
        }

        Grid grid;

        float fSelectedWandCellSize = 125;
        bool bSelectionFoldout = true;
        Vector2 vSelectedWandScrollPos = Vector2.zero;
        [SerializeField]
        Vector3 vTargetCellPos = Vector3.zero;

        [SerializeField]
        bool bBrushMode = false;

        [SerializeField]
        bool bPipetteMode = false;

        [SerializeField]
        TargetType targetType = TargetType.FixedYAxis;

        [SerializeField]
        bool bLockedBulk = false;

        [SerializeField]
        IsoTileBulk cLockedBulk = null;        

        bool bFoldoutOption = true;
        bool bMultipleApply = true;
        bool bAreadyPressed = false;

        bool bShowNativeGrid = false;
        bool bShowNativeBoxCollider = false;
        bool bShowNativeCanvas = false;

#if UNITY_2018_3_OR_NEWER
        Color colorSP = Color.white;
#else
        Color PickedColor = Color.gray;
        EditorWindow colorEditorWindow = null;
        MethodInfo getColor;
        MethodInfo setColor;
#endif
        #region Params
        [SerializeField]
        IsoTile tileParam = null;
        void SetTileParam(IsoTile tile)
        {
            tileParam = tile;
            tileParamProperty();
        }

        float floatParam = 0.15f;
        SerializedProperty drawerSO = null;
        bool bIncludeTileBodyParam = true;
        bool bIncludeTileAttachments = true;
        bool bNewParam = true;
        bool bAutoIsoLight = true;
        Vector3 vPositionParam = Vector3.zero;
        bool bAxisParam_X = false;
        bool bAxisParam_Y = false;
        bool bAxisParam_Z = false;
        bool bPlaneParam_XY = false;
        bool bPlaneParam_YZ = false;
        bool bPlaneParam_XZ = false;
        bool bBulkParam_All = false;

        bool bAxisExpand { get { return bAxisParam_X | bAxisParam_Y | bAxisParam_Z | bPlaneParam_XY | bPlaneParam_YZ | bPlaneParam_XZ; } }
        bool bYAxisExpandable
        {
            get
            {
                if (selectionForUse.Count > 0)
                {
                    var wand = selectionForUse.First() as TileControlWand;
                    if (wand != null && 
                        (wand.type == TileControlWand.Type.Tile_Control_Raise 
                        || wand.type == TileControlWand.Type.Tile_Control_Lower))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        object[] GetParams(MagicWand wand)
        {
            object[] objects = new object[] { };
            var ParamTypes = wand.Params;
            if (ParamTypes != null)
            {
                foreach (var type in ParamTypes)
                {
                    switch (type)
                    {
                        //case MagicWand.ParamType.Axis:
                        //    break;
                        case MagicWand.ParamType.AutoIsoLight:
                            ArrayUtility.Add(ref objects, bAutoIsoLight);
                            break;
                        case MagicWand.ParamType.Position:
                            ArrayUtility.Add(ref objects, vPositionParam);
                            break;
                        case MagicWand.ParamType.Color:
                            ArrayUtility.Add(ref objects, GetColorPicked);
                            ArrayUtility.Add(ref objects, floatParam);
                            ArrayUtility.Add(ref objects, bIncludeTileBodyParam);
                            ArrayUtility.Add(ref objects, bIncludeTileAttachments);
                            break;
                        case MagicWand.ParamType.IsoTile:
                            ArrayUtility.Add(ref objects, tileParam);
                            break;
                        case MagicWand.ParamType.Parts:
                            ArrayUtility.Add(ref objects, bIncludeTileBodyParam);
                            ArrayUtility.Add(ref objects, bIncludeTileAttachments);
                            break;
                        case MagicWand.ParamType.New:
                            ArrayUtility.Add(ref objects, bNewParam);
                            break;
                        case MagicWand.ParamType.IsoBulk:
                            ArrayUtility.Add(ref objects, bLockedBulk ? cLockedBulk : null);
                            break;
                    }
                }
            }
            return objects;
        }

        static void AxisExpand(bool bExpand, IsoTileBulk bulk, Vector3 coordinates, Vector3 direction, ref List<IsoTile> tileList)
        {
            if (bExpand)
            {
                tileList.AddRange(bulk.GetTiles_At(coordinates, direction, true, true));
                tileList.AddRange(bulk.GetTiles_At(coordinates, -direction, true, true));
            }
        }
        void PlaneExpand(bool xPlaneEX, bool yPlaneEX, bool zPlaneEX, IsoTileBulk bulk, Vector3 coordinates, ref List<IsoTile> tileList)
        {
            if (!xPlaneEX && !yPlaneEX && !zPlaneEX)
                return;

            List<IsoTile> tiles = bulk.GetTiles_At(coordinates);
            if (tiles.Count == 0)
                return;

            Vector3 position = bulk.coordinates.CoordinatesToPosition(coordinates);
            Plane planeX = new Plane(Vector3.left, position);
            Plane planeY = new Plane(Vector3.down, position);
            Plane planeZ = new Plane(Vector3.back, position);

            IsoTile[] tileArray = bulk.GetTiles();
            foreach(var tile in tileArray)
            {
                bool bIntersect = false;
                Bounds bounds = tile.GetBounds();
                if (xPlaneEX)
                    bIntersect = bounds.Intersect(planeX);
                if (!bIntersect && yPlaneEX)
                    bIntersect = bounds.Intersect(planeY);
                if (!bIntersect && zPlaneEX)
                    bIntersect = bounds.Intersect(planeZ);
                if (bIntersect)
                    tileList.Add(tile);
            }
        }
        static void AllinBulkExpand(IsoTileBulk bulk, ref List<IsoTile> tileList)
        {
            tileList.AddRange(bulk.GetTiles());
        }

        IEnumerator<IsoTile> GetExpandedTiles(GameObject gameObject)
        {
            IsoTile _tile = findTile(gameObject);
            if (!_tile || !bAxisExpand)
                return null;

            Vector3 _coordinates = _tile.coordinates._xyz;
            IsoTileBulk _bulk = _tile.Bulk;
            List<IsoTile> _tileList = new List<IsoTile>();
            bool _bYAxisExpandable = bYAxisExpandable;

            if (_bYAxisExpandable && bBulkParam_All)
                AllinBulkExpand(_bulk, ref _tileList);
            else
            {
                bool xPlaneEX = (_bYAxisExpandable && bPlaneParam_XY) || bPlaneParam_XZ;
                bool yPlaneEX = _bYAxisExpandable && (bPlaneParam_XY || bPlaneParam_YZ);
                bool zPlaneEX = (_bYAxisExpandable && bPlaneParam_YZ) || bPlaneParam_XZ;

                PlaneExpand(_bYAxisExpandable && bPlaneParam_YZ, bPlaneParam_XZ, _bYAxisExpandable && bPlaneParam_XY, _bulk, _coordinates, ref _tileList);
                AxisExpand(!xPlaneEX && bAxisParam_X, _bulk, _coordinates, Vector3.right, ref _tileList);
                AxisExpand(!yPlaneEX && bAxisParam_Y, _bulk, _coordinates, Vector3.up, ref _tileList);
                AxisExpand(!zPlaneEX && bAxisParam_Z, _bulk, _coordinates, Vector3.forward, ref _tileList);
            }
            _tileList.RemoveAll(t => t == _tile);
            _tileList.Add(_tile);
            return _tileList.Distinct().GetEnumerator();
        }
#endregion

        float fixedYAxisValue = 0;
        int iSceneViewID = -1;

        MagicWandSelection wandPreset;
        // List<MagicWandSelection> wandPresets = new List<MagicWandSelection>();

        // float fTargetY = 0f;
        Grid gridObject;

#region ResourcesForEditor
        const string OptionIconPath = "MW-asset-icon.png";
        const string PipetteOnIconPath = "Pipette On.png";
        const string PipetteOffIconPath = "Pipette Off.png";
        const string CustomCursor_PipettePath = "Pipette.png";

        const string DefaultPath = "Assets/Anonym/MagicWand/custom wand/IsometricBuilder/Default";
        const string DefaultControlPalettePath = DefaultPath + "/" + "Default Control Palette.asset";
        const string DefaultTileSetFolder = "TileWand";
        const string DefaultTileSetFolderPath = DefaultPath + "/" + DefaultTileSetFolder;
        const string DefaultTileWandPathName = DefaultTileSetFolderPath + "/" + "Tile";
        const string DefaultTileSetPath = DefaultTileSetFolderPath + "/" + "Tile Set.asset";

        Texture2D AssetIconTexture;
        Texture2D PipetteOn;
        Texture2D PipetteOff;
        Texture2D CustomCursor_Pipette;
        Texture2D CustomCursorTexture = null;

        const string NullIsoMapMsg = "This Tool needs a IsoMap instance in the Scene!";
#endregion

#region Tag
        [SerializeField]
        List<string> tags = new List<string>();
        void UpdateTags()
        {

        }
        public bool Tag(string _tag)
        {
            return tags.Any(r => r.Equals(_tag, System.StringComparison.CurrentCultureIgnoreCase));
        }
        public bool Tag(List<string> _tags)
        {
            return _tags.Any(r => Tag(r));
        }
#endregion

#endregion

#region ISerialize
        bool bJustSerialized = true;
        public void OnBeforeSerialize()
        {

        }
        public void OnAfterDeserialize()
        {
            bJustSerialized = true;
        }
        void tileParamProperty()
        {
            SerializedObject _so = new SerializedObject(this);
            if (_so != null)
                drawerSO = _so.FindProperty("tileParam");
        }
#endregion

#region MainFeature
        Vector3 vLastTileBound = Vector3.one;
        Vector3 vTopOfTileBound
        {
            get
            {
                if (lastTarget_gameObjects.Count > 0)
                {
                    IsoTile tile = findTile(lastTarget_gameObjects.Last());
                    if (tile != null)
                        vLastTileBound = tile.GetBounds_SideOnly().size;
                }
                return vLastTileBound;
            }
        }
        float fTopOfTileBound { get
            {
                return fixedYAxisValue + (lastTile == null
                    ? (grid != null ? grid.TileSize.y : 1) * 0.5f
                    : lastTile.GetBounds_SideOnly().extents.y);
            }
        }
        bool PositionOnSurface(Camera cam, Vector3 vMousePosition, out Vector3 vResult)
        {
            return TouchUtility.Raycast_Plane(cam, vMousePosition, 
                new Plane(Vector3.down, fTopOfTileBound), out vResult);
        }
        Ray GetRay_MouseToScreen(Vector3 vMousePosition)
        {
            return  SceneView.currentDrawingSceneView.camera.ScreenPointToRay(vMousePosition);
        }
        GameObject GetGameObject_MouseOver(Vector3 vMousePosition)
        {
            RaycastHit hit;
            if (Physics.Raycast(GetRay_MouseToScreen(vMousePosition), out hit, 10000, -1, QueryTriggerInteraction.Collide))
                return hit.collider.gameObject;
            return null;
        }
        void MakeUpAt(Vector3 vMousePosition)
        {
            Camera cam = SceneView.currentDrawingSceneView.camera;
            vMousePosition.y = cam.pixelHeight - vMousePosition.y;

            if (bPipetteMode)
            {
                TogglePipetteMode(false);
                IsoTile tile = findTile(GetGameObject_MouseOver(vMousePosition));
                if (tile)
                    SetTileParam(tile);
            }
            else
            {
                Vector3 vTargetTopPos;
                bool bHitOnSurface = PositionOnSurface(cam, vMousePosition, out vTargetTopPos);
                vTargetTopPos.y = vTargetCellPos.y;
                vPositionParam = vTargetTopPos;

                if (targetType == TargetType.MouseOver || !bAreadyPressed)
                {
                    if (!MakeUpEX(GetGameObject_MouseOver(vMousePosition)))
                        clearLastTargets_All(vTargetTopPos);

                    bAreadyPressed = true;
                }
                else
                {
                    if (bHitOnSurface)
                    {
                        var hits = Physics.RaycastAll(GetRay_MouseToScreen(vMousePosition), 1000, -1, QueryTriggerInteraction.Collide)
                            .Where(h => h.collider != null)
                            .Select(c => findTile(c.collider.gameObject))
                            .Where(t => t != null && t.GetBounds_SideOnly().Contains(vTargetTopPos))
                            .Distinct().OrderBy(t => Vector3.Distance(t.transform.position, vTargetTopPos));

                        bool bResult = false;
                        if (hits.Count() > 0)
                        {
                            if (lastTarget_gameObjects.Count > 0 && hits.All(r => !lastTarget_gameObjects.Contains(r.gameObject)))
                                clearLastTarget();

                            foreach (var hit in hits)
                            {
                                if (bResult |= MakeUpEX(hit.gameObject))
                                    break;
                            }
                        }
                        else
                            bResult = MakeUp(null);
                            
                        if (!bResult)
                            clearLastTargets_All(vTargetTopPos);
                    }
                }
            }
        }
        bool MakeUpEX(GameObject targetObject)
        {
            bool bResult = false;

            if (targetObject != null && bAxisExpand)
            {
                List<GameObject> instantMakeUpedList = new List<GameObject>();
                var tiles = GetExpandedTiles(targetObject);
                while (tiles.MoveNext())
                {
                    var current = tiles.Current;
                    if (current)
                        bResult |= MakeUp(current.gameObject, instantMakeUpedList);
                }
            }
            else
                bResult |= MakeUp(targetObject);

            return bResult;
        }
        bool MakeUp(GameObject targetObject, List<GameObject> instantMakeUpedList = null)
        {
            GameObject currentTarget = targetObject;
            if (isInvalidObject(currentTarget))
                return false;

            IsoTile startTile = IsoTile.Find(currentTarget);
            bool bResult = false, bNullStart = startTile == null;
            Vector3 selectedPos = startTile ? startTile.transform.position : vTargetCellPos;

            var e = selectionForUse.GetEnumerator();
            while (e.MoveNext())
            {
                var current = e.Current as MagicWand;
                var wandTargetObject = current.TargetGameObject(targetObject);

                if (bResult |= (instantMakeUpedList != null && instantMakeUpedList.Contains(wandTargetObject)))
                    continue;

                if (bResult |= bMultiCheck(current, currentTarget, wandTargetObject))
                    continue;

                if (bResult |= current.MakeUp(ref currentTarget, GetParams(current)))
                {
                    AddToMakeUpedList(wandTargetObject);
                    if (instantMakeUpedList != null)
                        instantMakeUpedList.Add(wandTargetObject);
                }
            }

            if (!lastTarget_gameObjects.Contains(currentTarget))
                lastTarget_gameObjects.Add(currentTarget);
            lastTile = IsoTile.Find(currentTarget);

            if (bResult)
                AddToMakeUpedList(targetObject, currentTarget);

            if (bNullStart && lastTile)
                selectedPos = lastTile.transform.position;

            vTargetCellPos = new Vector3(selectedPos.x, vTargetCellPos.y, selectedPos.z);
            if (bAreadyPressed == false)
            {
                bAreadyPressed = true;
                fixedYAxisValue = vTargetCellPos.y = selectedPos.y;
            }

            return bResult;
        }
        void AddToMakeUpedList(params GameObject[] adds)
        {
            foreach (var add in adds)
            {
                if (add != null && !makeUpedTargetList.Contains(add))
                    makeUpedTargetList.Add(add);
            }
        }
        bool bMultiCheck(MagicWand wand, GameObject targetObject, GameObject wandTargetObject)
        {
            bool bMulti = bMultipleApply && (wand == null || wand.bAllowMultipleApplyOnAClick);

            if (wandTargetObject != null &&
                ((!bMulti && makeUpedTargetList.Contains(wandTargetObject))
                || (bMulti && lastTarget_gameObjects.Contains(targetObject))))
                return true;

            return false;
        }
#endregion

#region EditorWindow
        [MenuItem("Window/Anonym/Magic Wand Window")]
        public static void CreateWindow()
        {
            if (IsoMap.IsNull)
            {
                Debug.LogError(NullIsoMapMsg);
                return;
            }

            EditorWindow window = EditorWindow.CreateInstance<MasterPaletteWindow>();
            window.titleContent.text = "Magic Palette";
            window.Show();
        }
        private void OnEnable()
        {
            if (grid == null && !IsoMap.IsNull)
                grid = IsoMap.instance.gGrid;

            UpdateAllPaletteDic();
            ToggleBrushMode(false);

            SceneView.onSceneGUIDelegate += OnSceneView_HotKey_WithoutFocus;
        }
        private void OnDestroy()
        {
#if UNITY_EDITOR && !UNITY_2018_3_OR_NEWER
            DestroyColorPickerWindow();
#endif
            ToggleBrushMode(false);

            SceneView.onSceneGUIDelegate -= OnSceneView_HotKey_WithoutFocus;
        }
        void OnInspectorUpdate()
        {
            Repaint();
        }
        void OnGUI()
        {
            if (IsoMap.IsNull)
            {
                if (bPipetteMode)
                    TogglePipetteMode(false);

                if (bBrushMode)
                    ToggleBrushMode(false);

                EditorGUILayout.HelpBox(NullIsoMapMsg, MessageType.Warning);
                return;
            }

            Event e = Event.current;
            Rect windowRect = new Rect(0, 0, position.width, position.height);
            if (windowRect.Contains(e.mousePosition))
            {
                UpdateCustomCursorTexture();
                CustomCursor(windowRect);
            }

            CheckBrushModeHotKey(Event.current);

            showHelpMSG();
            TopField();

            if (ShowPaletteDic())
                UpdateSelection();

            ShowSelectedWand();

            if (bJustSerialized)
                UpdateWandParams();

            ShowBtns();
             
            bJustSerialized = false;
        }
#endregion

#region UnityEditor
        bool isAvailableHotKeyOnSceneView
        {
            get
            {                
                return EditorWindow.focusedWindow == this || (SceneView.focusedWindow && SceneView.focusedWindow.wantsMouseMove);
            }
        }
        string HotKeyMsg(string Msg, int iHotKey)
        {
            return isAvailableHotKeyOnSceneView ? string.Format("{0} (F{1})", Msg, iHotKey) : Msg;
        }
        void UpdateCustomCursorTexture()
        {
            CustomCursorTexture = null;
            if (bPipetteMode)
                CustomCursorTexture = CustomCursor_Pipette;
            else if (bBrushMode)
            {
                if (selectionForUse.Count == 0)
                {
                    updateBrushMode();
                    return;
                }

                MagicWand wnad = selectionForUse.First();
                if (wnad is TileControlWand)
                    CustomCursorTexture = wnad.GetTextures().First() as Texture2D;
                else
                    CustomCursorTexture = AssetIconTexture;
            }
        }
        void updateBrushMode()
        {
            if (!bBrushMode && selectionForUse.Count > 0)
                ToggleBrushMode(true);
            else if (bBrushMode && selectionForUse.Count == 0)
                ToggleBrushMode(false);
        }
        void CustomCursor(Rect rt, int controlID)
        {
            Cursor.SetCursor(CustomCursorTexture, Vector2.zero, CursorMode.Auto);
            EditorGUIUtility.AddCursorRect(rt, MouseCursor.CustomCursor, controlID);
        }
        void CustomCursor(Rect rt)
        {
            Cursor.SetCursor(CustomCursorTexture, Vector2.zero, CursorMode.Auto);
            EditorGUIUtility.AddCursorRect(rt, MouseCursor.CustomCursor);
        }

        void OnSceneView_HotKey_WithoutFocus(SceneView sceneView)
        {
            CheckBrushModeHotKey(Event.current);
        }
        void OnSceneViewUpdate(SceneView sceneView)
        {
            iSceneViewID = GUIUtility.GetControlID(sceneView.GetInstanceID(), FocusType.Passive);
            UpdateCustomCursorTexture();
            CustomCursor(sceneView.camera.pixelRect, iSceneViewID);

            Event e = Event.current; 
            switch (e.type)
            {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(iSceneViewID);
                    break;
                case EventType.MouseDown:
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        MakeUpAt(e.mousePosition);
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        makeUpedTargetList.Clear();
                        clearLastTarget();
                        bAreadyPressed = false;
                        e.Use();
                    }
                    break;
            }

            if (targetType != TargetType.MouseOver)
                OnGridGUI();
        }
        bool CheckBrushModeHotKey(Event e)
        {
            if (IsoMap.IsNull)
                return false;

            bool bUse = false;
            if (e.type == EventType.KeyDown && e.functionKey)
            {
                switch(e.keyCode)
                {
                    case KeyCode.F12:
                        ToggleBrushMode();
                        bUse = true;
                        break;
                    case KeyCode.F11:
                        break;
                    case KeyCode.F10:
                        if (paramTypes.Contains(MagicWand.ParamType.IsoTile))
                        {
                            TogglePipetteMode(!bPipetteMode);
                            bUse = true;
                        }
                        break;
                    case KeyCode.F9:
                        if (!IsoMap.IsNull)
                            IsoMap.instance.Update_TileAngle();
                        break;
                    default:
                        var tokens = e.keyCode.ToString().Split('F');
                        if (tokens.Length > 1)
                        {
                            int num;
                            if (int.TryParse(tokens[1], out num))
                            {
                                num -= 1;
                                if (bBrushMode && selection.Count > num)
                                {
                                    selectionForUse.Clear();
                                    selectionForUse.Add(selection[num]);
                                    UpdateSelection();
                                    UpdateWandParams();
                                    e.Use();
                                }
                            }
                        }
                        break;
                }
            }

            if (bUse)
                e.Use();

            return bUse;
        }
#endregion

#region PaletteDic
        void UpdateAllPaletteDic()
        {
            if (defaultPalette == null && !string.IsNullOrEmpty(DefaultControlPalettePath))
            {
                defaultPalette = AssetDatabase.LoadAssetAtPath<AbstractMagicWandPalette>(DefaultControlPalettePath);
                if (defaultPalette != null)
                {
                    // updatePalettes(defaultPalette);
                    // SimpleGridForPalette grid = SimpleGrids[Palettes.IndexOf(defaultPalette)];
                    // grid.selection.AddRange(defaultPalette.GetMagicWands());
                    // selectionForUse.Add(grid.selection.First());
                    // grid.bFoldOUt = false;
                    UpdateSelection();
                    selectionForUse.Add(selection.First());
                }
            }

            var resources = Resources.FindObjectsOfTypeAll<AbstractMagicWandPalette>();
            var enumerator = resources.GetEnumerator();
            while (enumerator.MoveNext())
            {
                updatePalettes(enumerator.Current as AbstractMagicWandPalette);
            }
        }
        void updatePalettes(AbstractMagicWandPalette newPalette)
        {
            if (newPalette == null || newPalette == defaultPalette)
                return;

            var wands = newPalette.GetMagicWands();
            wands.ForEach(w => (w as ITag).ClearTags());

            if (!Palettes.Contains(newPalette))
            {
                var simpleGridData = new SimpleGridForPalette();
                simpleGridData.Init(newPalette.name, 75, wands,
                    "Deselect All", simpleGridData.DeselectAll, Color.gray, true, true, newPalette.bMultiSelectable);
                Palettes.Add(newPalette);
                SimpleGrids.Add(simpleGridData);
                simpleGridData.bFoldOUt = true;
            }
            else
            {
                SimpleGrids[Palettes.IndexOf(newPalette)].UpdateList(wands);
            }
        }
        bool ShowPaletteDic()
        {
            CustomEditorGUI.DrawSeperator();
            Palettes.RemoveAll(p => p == null);
            GUILayout.Label(string.Format("[Palette: loaded({0})]", Palettes.Count), EditorStyles.boldLabel);

            UpdatePaletteField();

            bool bChanged = false;
            for (int i = 0; i < SimpleGrids.Count; ++i)
                bChanged |= SimpleGrids[i].ShowGrid(MagicWand.OnCustomGUI);

            return bChanged;
        }
#endregion

#region PaletteField
        void UpdatePaletteField()
        {
            AbstractMagicWandPalette newPalette = PaletteField(selectedPalette, null, true);
            if (defaultPalette == newPalette)
            {
                Debug.LogWarning("This palette can not be used by user. The selection is canceled. " + newPalette.name);
                newPalette = selectedPalette;
            }
            else if (selectedPalette != newPalette)
            {
                selectedPalette = newPalette;
                updatePalettes(selectedPalette);
            }
        }
        AbstractMagicWandPalette PaletteField(AbstractMagicWandPalette palette, string label = "", bool bNullable = false)
        {
            if (!string.IsNullOrEmpty(label))
                GUILayout.Label(label, EditorStyles.boldLabel);

            if (!bNullable && palette == null)
                return null;

            using (new EditorGUILayout.HorizontalScope())
            {
                return EditorGUILayout.ObjectField("Pick the palette to load.", palette, typeof(AbstractMagicWandPalette), allowSceneObjects: false) as AbstractMagicWandPalette;
            }
        }
#endregion

#region Selection
        void OnCustomGUIWithShortCut(MagicWand wand, Rect rect)
        { 
            MagicWand.OnCustomGUIWithLabel(wand, rect);

            if (isAvailableHotKeyOnSceneView)
            {
                int num = selection.FindIndex(r => r == wand) + 1;
                if (num >= 9)
                    return;

                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = rect.width * 0.3f;
                EditorGUI.DrawRect(rect, Color.black);
                rect = new Rect(rect.position + Vector2.one, rect.size - 2 * Vector2.one);
                MagicWand.OnCustomGUIExLabel(rect, CustomEditorGUI.Color_LightBlue, "F" + num);
            }
        }
        void ShowSelectedWand()
        {
            CustomEditorGUI.DrawSeperator();
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(string.Format("[Lookup Magic Wand({0})]", selection.Count), EditorStyles.boldLabel);
                if (selection.Count == 0)
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Reset"))
                    {
                        ClearSelection();
                    }
                }
            }

            if (selection.Count > 0)
            {
                if (CustomEditorGUI.SimpleGrid<MagicWand>.ShowGrid(
                    MagicWand.TypeArray(selection), OnCustomGUIWithShortCut,
                    selection.GetEnumerator(), "Deselect TileWand", ClearSelection,
                    ref fSelectedWandCellSize, ref bSelectionFoldout, ref vSelectedWandScrollPos,
                    selectionForUse, false, true, Color.cyan, false))
                {
                    selectionForUse.Reverse(); 
                    var enumerator = selectionForUse.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        bool isExclusive = true; // current.IsExclusive;
                        // System.Type exclusiveType = current.GetType();

                        if (selectionForUse.RemoveAll(r => r != current 
                            // && r.GetType() == exclusiveType
                            && (isExclusive || r.IsExclusive)) > 0)
                        {
                            enumerator = selectionForUse.GetEnumerator();
                        }
                    }
                    selectionForUse.Reverse();
                    UpdateWandParams();
                    updateBrushMode();
                    TogglePipetteMode(false);
                }
            }
        }
        void UpdateSelection()
        {
            if (selection.Count != 0)
                selection.Clear();

            var wands = defaultPalette.GetMagicWands();
            selection.AddRange(wands);
            SimpleGrids.ForEach(g => selection.AddRange(g.selection));
            selectionForUse.RemoveAll(r => !selection.Contains(r));
            updateBrushMode();
        }
        void ClearSelection()
        {
            SimpleGrids.ForEach(g => g.selection.RemoveAll(r => selection.Contains(r)));
            selectionForUse.Clear();
            UpdateSelection();
        }
        void UpdateWandParams()
        {
            paramTypes.Clear();
            selectionForUse.ForEach(w =>
            { 
                var Params = w.Params;
                if (Params != null)
                    paramTypes.AddRange(Params);
            });

            if (bPipetteMode && !paramTypes.Contains(MagicWand.ParamType.IsoTile))
                TogglePipetteMode(false);
        }        
#endregion

#region ModeField
        void TopField()
        {
            if (AssetIconTexture == null)
                AssetIconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + GizmosIconCopy.resourcePath + "/" + OptionIconPath);

            if (PipetteOn == null)
            {
                PipetteOn = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + GizmosIconCopy.resourcePath + "/" + PipetteOnIconPath);
                CustomCursor_Pipette = EditorGUIUtility.Load("Assets" + GizmosIconCopy.resourcePath + "/" + CustomCursor_PipettePath) as Texture2D;
            }

            if(PipetteOff == null)
                PipetteOff = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + GizmosIconCopy.resourcePath + "/" + PipetteOffIconPath);

            CustomEditorGUI.DrawSeperator();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("[Magic Wand Option]", EditorStyles.boldLabel);
                bFoldoutOption = EditorGUILayout.Foldout(bFoldoutOption, "fold out");
                GUILayout.FlexibleSpace();
            }
            if (bFoldoutOption)
            {
                ModeField_Wand();
            }
        }
        void ModeField_Wand()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUIContent label = new GUIContent("Target Type ", "[Fixed mode]\nis good for working tiles of the same height.\nOnly objects of the same height as the first selected. And, the mouse will always point to the top of the target tile.");
                        Vector2 textSize = GUI.skin.label.CalcSize(label);

                        EditorGUILayout.LabelField(label, GUILayout.Width(textSize.x));
                        targetType = (TargetType) EditorGUILayout.EnumPopup(targetType);
                        GUILayout.FlexibleSpace();                        
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        bLockedBulk = EditorGUILayout.ToggleLeft(new GUIContent("Lock Bulk Target", "When a bulk target is locked, only the tiles contained in that bulk will be the target of the operation."), bLockedBulk);
                        if (bLockedBulk)
                        {
                            GUILayout.FlexibleSpace();
                            cLockedBulk = EditorGUILayout.ObjectField(cLockedBulk, typeof(IsoTileBulk), allowSceneObjects: true) as IsoTileBulk;
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUIContent label = new GUIContent("Allows multiple apply to the same object during one click.",
                            "Multiple apply occurs when the mouse cursor re-enters the same object during MouseButtonDown.");
                        bMultipleApply = EditorGUILayout.ToggleLeft(label, bMultipleApply);
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayoutOption minWidth = GUILayout.MinWidth(120);
                        EditorGUILayout.LabelField("SceneView Toggle", minWidth);

                        if (bJustSerialized)
                        {
                            bShowNativeGrid = AnnotationToggleUtility.ShowGrid;
                            bShowNativeBoxCollider = AnnotationToggleUtility.ShowBoxColliderGizmo;
                            bShowNativeCanvas = AnnotationToggleUtility.ShowCanvasGizmo;
                        }
                        using (var result = new EditorGUI.ChangeCheckScope())
                        {
                            bShowNativeBoxCollider = EditorGUILayout.ToggleLeft("BoxCollider", bShowNativeBoxCollider, minWidth);
                            if (result.changed)
                                AnnotationToggleUtility.ShowBoxColliderGizmo = bShowNativeBoxCollider;
                        }
                        using (var result = new EditorGUI.ChangeCheckScope())
                        {
                            bShowNativeCanvas = EditorGUILayout.ToggleLeft("Canvas", bShowNativeCanvas, minWidth);
                            if (result.changed)
                                AnnotationToggleUtility.ShowCanvasGizmo = bShowNativeCanvas;
                        }
                        using (var result = new EditorGUI.ChangeCheckScope())
                        {
                            bShowNativeGrid = EditorGUILayout.ToggleLeft("Grid", bShowNativeGrid, minWidth);
                            if (result.changed)
                                AnnotationToggleUtility.ShowGrid = bShowNativeGrid;
                        }
                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }
#endregion

#region WandPresets
        void PresetField()
        {
            CustomEditorGUI.DrawSeperator();
            if (wandPreset != null)
                GUILayout.Label(wandPreset.name, EditorStyles.boldLabel);

            wandPreset = EditorGUILayout.ObjectField(wandPreset, typeof(MagicWandSelection), allowSceneObjects: false) as MagicWandSelection;
        }
        void ResetSelectedPreset()
        {

        }
#endregion

#region ColorPicker
        Color colorPickerColor { get {
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
                return colorSP;
#else
                return (Color)getColor.Invoke(colorEditorWindow, null); 
#endif
                }
        }

        public Color GetColorPicked { get
            {
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
                return colorSP;
#else
                if (colorEditorWindow == null || getColor == null)
                    return PickedColor;

                return PickedColor = colorPickerColor;
#endif
            }
        }

#if UNITY_EDITOR && !UNITY_2018_3_OR_NEWER
        Color SetColorPickerColor { set
            {
                if (colorEditorWindow == null || setColor == null)
                    return;

                setColor.Invoke(colorEditorWindow, new object[] { value });
            }
        }
#endif

#if UNITY_EDITOR && !UNITY_2018_3_OR_NEWER
        void CreateColorPickerWindow()
        {
            System.Type type = ColorPickerType;
                       
            if (colorEditorWindow == null)
            {
                colorEditorWindow = EditorWindow.CreateInstance(type) as EditorWindow;
                colorEditorWindow.titleContent = new GUIContent("Wand Color");
                colorEditorWindow.ShowUtility();

                if (setColor == null)
                    setColor = type.GetMethod("set_color", BindingFlags.Public | BindingFlags.Static);

                if (getColor == null)
                    getColor = type.GetMethod("get_color", BindingFlags.Public | BindingFlags.Static);

                try
                {
                    SetColorPickerColor = PickedColor;
                }
                catch
                {
                    DestroyColorPickerWindow(false);
                    LogErr();
                }
            }
        }
        void DestroyColorPickerWindow(bool withColorBackup = true)
        {
            if (colorEditorWindow)
            {
                if (withColorBackup)
                    PickedColor = colorPickerColor;
                colorEditorWindow.Close();
                EditorWindow.DestroyImmediate(colorEditorWindow);
                colorEditorWindow = null;
            }
        }
        System.Type ColorPickerType
        {
            get {
                var assembly = Assembly.GetAssembly(typeof(EditorWindow));
                var types = assembly.GetTypes();
                // Debug.Log(string.Join(", ", types.Select(t => t.Name).Where(s => s.Contains("ColorPicker")).ToArray()));

                var resultType = types.First(t => t.Name.Equals("ColorPicker"));
                if (resultType == null)
                    LogErr();

                return resultType;
            }
        }
        void LogErr()
        {
            Debug.LogError("The current version of Unity can not support the ColorPicker(Native Class).");
        }
#endif

#endregion

#region BottomMenu
        void ShowBtns()
        {
            GUILayout.FlexibleSpace();

            string label = "[Wand Option] " + (selectionForUse.Count == 0 ? "Nothing Selected!" : MagicWand.NameArray(selectionForUse));
            GUILayout.Label(label, EditorStyles.boldLabel);
            CustomEditorGUI.DrawSeperator();

            using (new EditorGUILayout.VerticalScope())
            {
#if !UNITY_2018_3_OR_NEWER
                GUILayoutOption wOption = GUILayout.Width(30);
#endif
                GUILayoutOption wMinOption = GUILayout.MinWidth(30);
                GUILayoutOption hOption = GUILayout.Height(30);

                if (paramTypes.Contains(MagicWand.ParamType.AutoIsoLight))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(new GUIContent("Auto IsoLight", "If enabled, the tile is automatically registered in IsoLight when Use Create and Copycat."));
                        bAutoIsoLight = EditorGUILayout.ToggleLeft("Enable", bAutoIsoLight, wMinOption);
                        GUILayout.FlexibleSpace();
                    }
                }

                if (!paramTypes.Contains(MagicWand.ParamType.Position))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(new GUIContent("Select Axis for Expand", "The Magic Wand applies to all tiles with the same axis value as the selected tile."));
                        using (new EditorGUI.DisabledScope(bBulkParam_All || bPlaneParam_XY || bPlaneParam_XZ))
                        {
                            bAxisParam_X = EditorGUILayout.ToggleLeft("X", bAxisParam_X, wMinOption);
                        }
                        using (new EditorGUI.DisabledScope(!bYAxisExpandable || bBulkParam_All || bPlaneParam_XY || bPlaneParam_YZ))
                        {
                            bAxisParam_Y = EditorGUILayout.ToggleLeft("Y", bAxisParam_Y, wMinOption);
                        }
                        using (new EditorGUI.DisabledScope(bBulkParam_All || bPlaneParam_YZ || bPlaneParam_XZ))
                        {
                            bAxisParam_Z = EditorGUILayout.ToggleLeft("Z", bAxisParam_Z, wMinOption);
                        }
                        using (new EditorGUI.DisabledScope(bBulkParam_All))
                        {
                            using (new EditorGUI.DisabledGroupScope(!bYAxisExpandable))
                            {
                                bPlaneParam_XY = EditorGUILayout.ToggleLeft("XY", bPlaneParam_XY, wMinOption);
                                bPlaneParam_YZ = EditorGUILayout.ToggleLeft("YZ", bPlaneParam_YZ, wMinOption);
                            }
                            bPlaneParam_XZ = EditorGUILayout.ToggleLeft("XZ", bPlaneParam_XZ, wMinOption);
                        }

                        using (new EditorGUI.DisabledGroupScope(!bYAxisExpandable))
                        {
                            bBulkParam_All = EditorGUILayout.ToggleLeft("Bulk", bBulkParam_All, wMinOption);
                        }

                        GUILayout.FlexibleSpace();
                    }
                }

                if (paramTypes.Contains(MagicWand.ParamType.Parts))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(new GUIContent("Select Target Parts", "Applies to selected Part only."));
                        bIncludeTileBodyParam = EditorGUILayout.ToggleLeft("Body", bIncludeTileBodyParam, wMinOption);
                        bIncludeTileAttachments = EditorGUILayout.ToggleLeft("Attachment", bIncludeTileAttachments, wMinOption);
                        GUILayout.FlexibleSpace();
                    }
                }

                if (paramTypes.Contains(MagicWand.ParamType.New))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(new GUIContent("When Empty", "If this toggle is on, a tile will automatically be created when you click on an empty space."));
                        bNewParam = EditorGUILayout.ToggleLeft("Auto Creation", bNewParam, wMinOption);
                        GUILayout.FlexibleSpace();
                    }
                }

                if (paramTypes.Contains(MagicWand.ParamType.fWeight))
                    floatParam = CustomEditorGUI.FloatSlider(
                        EditorGUILayout.GetControlRect(), "Wand Strength", floatParam, 0, 1);

                if (paramTypes.Contains(MagicWand.ParamType.Color))
                {
#if UNITY_2018_3_OR_NEWER
                    colorSP = EditorGUILayout.ColorField(colorSP);
#else
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        Rect rt = EditorGUILayout.GetControlRect(wOption, hOption);
                        EditorGUIUtility.DrawColorSwatch(rt, GetColorPicked);
                        CustomEditorGUI.Button(true, GetColorPicked, "Color Picker On/Off", () =>
                        {
                            if (colorEditorWindow == null)
                                CreateColorPickerWindow();
                            else
                                DestroyColorPickerWindow();
                        }, hOption);
                    }
#endif
                }
                
                if (paramTypes.Contains(MagicWand.ParamType.IsoTile))
                {
                    float fHeight = EditorGUIUtility.singleLineHeight * 3;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (bPipetteMode != GUI.Toggle(getIconRect(fHeight, 10), bPipetteMode,
                            bPipetteMode ? PipetteOn : PipetteOff))
                        {
                            TogglePipetteMode(!bPipetteMode);

                        }

                        using (new EditorGUILayout.VerticalScope())
                        {
                            // GameObject of Tile gameObjectParam
                            EditorGUILayout.LabelField(HotKeyMsg("Please select a tile to reference", 10));
                            IsoTile tile = EditorGUILayout.ObjectField(tileParam, typeof(IsoTile), allowSceneObjects: true) as IsoTile;
                            if (tile != null)
                            {
                                if (bJustSerialized || tile != tileParam)
                                {
                                    SetTileParam(tile);
                                }
                            }
                            else
                            {
                                tileParam = null;
                                drawerSO = null;
                            }
                        }
                    }

                    // Tile Drawer
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (drawerSO != null)
                        {
                            bool guiEnabled = GUI.enabled;
                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(drawerSO, GUILayout.Height(fHeight));
                            GUI.enabled = guiEnabled;

                            if (tileParam != null && GUILayout.Button("Make a Tile Wand!\nFor Reuse.", GUILayout.Height(fHeight)))
                            {
                                if (!AssetDatabase.IsValidFolder(DefaultTileSetFolderPath))
                                    AssetDatabase.CreateFolder(DefaultPath, DefaultTileSetFolder);

                                if (!defaultTileSetPalette)
                                {
                                    defaultTileSetPalette = AssetDatabase.LoadAssetAtPath<AbstractMagicWandPalette>(DefaultTileSetPath);
                                    if (!defaultTileSetPalette)
                                    {
                                        defaultTileSetPalette = MagicWandPalette.CreateAsset(DefaultTileSetPath);
                                    }
                                }

                                var tempGO = GameObject.Instantiate(tileParam, tileParam.transform.parent);
                                TileWand newTileWand = TileWand.CreateAsset(DefaultTileWandPathName, tempGO);
                                DestroyImmediate(tempGO.gameObject);

                                if (defaultTileSetPalette)
                                {
                                    defaultTileSetPalette.AddMagicWand(newTileWand);
                                    updatePalettes(defaultTileSetPalette);
                                }

                                EditorUtility.SetDirty(defaultTileSetPalette);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                            }
                        }
                    }
                }

                CustomEditorGUI.DrawSeperator();
                if (!IsoMap.IsNull)
                    CustomEditorGUI.Button(true, Color.gray, HotKeyMsg("Reset SceneView.Camera", 9), IsoMap.instance.Update_TileAngle, hOption);

                hOption = GUILayout.Height(40);
                using (new EditorGUILayout.HorizontalScope())
                {
                    CustomEditorGUI.Button(selectionForUse.Count > 0,
                        bBrushMode ? Color.cyan : Color.gray, HotKeyMsg("Magic Wand Toggle", 12),
                        () => ToggleBrushMode(!bBrushMode), hOption);
                    CustomEditorGUI.Button(true, Color.gray, "Refresh Palette", UpdateAllPaletteDic, hOption);
                }
            }
        }
#endregion

#region GridGUI
        void OnGridGUI()
        {
            const int iLineCount = 6;
            int iHalfCount = iLineCount / 2;

            Vector3 vGridInterval = grid == null ? Vector3.one : grid.TileSize;
            Vector3 vOrigin = vTargetCellPos + vGridInterval * 0.5f;
            if (targetType != TargetType.MouseOver)
                vOrigin.y = fTopOfTileBound;

            Vector3 vAdjustmentForYAxis = vTargetCellPos + new Vector3(-vGridInterval.x * 0.5f, 0f, vGridInterval.z * 0.5f);

            Handles.Label(vAdjustmentForYAxis, vTargetCellPos.ToString());

            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            for (int x = -iHalfCount; x < iHalfCount; x += Mathf.RoundToInt(vGridInterval.x))
            {
                for (int z = -iHalfCount; z < iHalfCount; z += Mathf.RoundToInt(vGridInterval.z))
                {
                    Handles.color = Color.gray * Mathf.Lerp(0, 1, 1 - Mathf.Abs(x * z) / (float)iLineCount);
                    Vector3 vPoint = vOrigin + Vector3.right * x * vGridInterval.x + Vector3.forward * z * vGridInterval.z;
                    Handles.DrawDottedLine(vPoint + Vector3.left, vPoint + Vector3.right, 0.25f);
                    Handles.DrawDottedLine(vPoint + Vector3.forward, vPoint + Vector3.back, 0.25f);
                }
            }

            Handles.color = Color.yellow * 0.9f;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            Handles.DrawWireCube(vTargetCellPos, vTopOfTileBound);
        }
#endregion

#region Etc
        void TogglePipetteMode(bool bFlag)
        {
            if (bFlag)
                ToggleBrushMode(true);
            bPipetteMode = bFlag;
            SceneView.RepaintAll();
        }
        void ToggleBrushMode()
        {
            ToggleBrushMode(!bBrushMode);
        }
        void ToggleBrushMode(bool bFlag)
        {
            if (bBrushMode && !bFlag)
                SceneView.onSceneGUIDelegate -= OnSceneViewUpdate;
            else if (!bBrushMode && bFlag)
                SceneView.onSceneGUIDelegate += OnSceneViewUpdate;

            bBrushMode = bFlag;
        }
        void showHelpMSG()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GUILayout.Label("Not available in Play mode!", EditorStyles.boldLabel);
                return;
            }
        }
        IsoTile findTile(GameObject go)
        {
            IsoTile tile = isInvalidObject(go) ? null : IsoTile.Find(go);
            return tile;
        }
        bool isInvalidObject(GameObject go)
        {
            bool bResult = false; // go != null ? true : false;

            if (go != null)
            {
                if (bLockedBulk && cLockedBulk != null)
                {
                    if (!go.transform.IsChildOf(cLockedBulk.transform))
                        bResult = true;
                    //else
                    //    Debug.Log(go.name + " is not a child of Locked Bulk(" + cLockedBulk.name + ")");
                }
            }

            return bResult;
        }
        static Rect getIconRect(float fSize, float fGap)
        {
            Rect rect_Icon = EditorGUILayout.GetControlRect(GUILayout.Height(fSize), GUILayout.Width(fSize + 2 * fGap));
            rect_Icon.xMin += fGap;
            return rect_Icon;
        }
#endregion
    }
}
