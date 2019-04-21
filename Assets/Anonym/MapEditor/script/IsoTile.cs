using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Isometric
{	
	using Util;

	public enum SelectionType
	{
		LastTile,
		NewTile,
		AllTile,
	}

    [System.Serializable]
    public class AttachedIso2D : Attachment<Iso2DObject> { }
    [System.Serializable]
    public class AttachedIso2Ds : AttachmentHierarchy<AttachedIso2D> { }

    [SelectionBase]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(GridCoordinates))]
	[RequireComponent(typeof(IsometricSortingOrder))]
	[RequireComponent(typeof(RegularCollider))]
	[ExecuteInEditMode]
    public class IsoTile : MonoBehaviour
    {
        #region Basic
        [SerializeField]
        GridCoordinates _coordinates = null;
        [HideInInspector]
        public GridCoordinates coordinates
        {
            get
            {
                return _coordinates == null ?
                    _coordinates = GetComponent<GridCoordinates>() : _coordinates;
            }
        }
        #endregion

        #region GetBounds
        public Bounds GetBounds_SideOnly()
        {
            return GetBounds(Iso2DObject.Type.Side_Union, Iso2DObject.Type.Side_X, Iso2DObject.Type.Side_Y, Iso2DObject.Type.Side_Z);
        }


        public Bounds GetBounds()
        {
            Collider[] _colliders = transform.GetComponentsInChildren<Collider>();
            if (_colliders == null || _colliders.Length == 0)
                return new Bounds(transform.position, Vector3.zero);

            Bounds _bounds = new Bounds(_colliders[0].bounds.center, Vector3.zero);
            for (int i = 0; i < _colliders.Length; ++i)
            {
                if (_colliders[i] is BoxCollider)
                    _bounds.Encapsulate((_colliders[i] as BoxCollider).GetStatelessBounds());
                else
                    _bounds.Encapsulate(_colliders[i].bounds);
            }
            _bounds.Expand(Grid.fGridTolerance);
            return _bounds;
        }

        public Bounds GetBounds(params Iso2DObject.Type[] _types)
        {
            Iso2DObject[] _Iso2Ds = GetSideObjects(_types);
            Bounds _bounds = new Bounds(transform.position, Vector3.zero);
            if (_Iso2Ds != null)
            {
                for (int i = 0; i < _Iso2Ds.Length; ++i)
                    _bounds.Encapsulate(_Iso2Ds[i].RC.GlobalBounds);
            }
            return _bounds;
        }
        #endregion

        #region GetSideObject
        AttachedIso2Ds AttachedList { get
            {
#if UNITY_EDITOR
                return _attachedList;
#else
                var tmp = new AttachedIso2Ds();
                tmp.Init(gameObject);
                return tmp;
#endif
            }
        }
        public Iso2DObject GetSideObject(Iso2DObject.Type _type)
        {
            if (AttachedList.childList.Exists(r => r.AttachedObj._Type == _type))
                return AttachedList.childList.Find(r => r.AttachedObj._Type == _type).AttachedObj;
            return null;
        }
        public Iso2DObject[] GetSideObjects(params Iso2DObject.Type[] _types)
        {
            if (_types == null || _types.Length == 0)
                _types = new Iso2DObject.Type[]{
                    Iso2DObject.Type.Obstacle, Iso2DObject.Type.Overlay,
                    Iso2DObject.Type.Side_Union, Iso2DObject.Type.Side_X,
                    Iso2DObject.Type.Side_Y, Iso2DObject.Type.Side_Z,
                };
            List<Iso2DObject> results = new List<Iso2DObject>();
            AttachedList.childList.ForEach(r => {
                if (r.AttachedObj != null && _types.Contains(r.AttachedObj._Type))
                    results.Add(r.AttachedObj);
            });
            return results.ToArray();
        }
        #endregion

        #region RuntimeEtc
        [SerializeField]
        IsoTileBulk _bulk;
        [HideInInspector]
        public IsoTileBulk Bulk
        {
            get
            {
                if (_bulk != null)
                    return _bulk;
                if (transform.parent != null)
                    return _bulk = transform.parent.GetComponent<IsoTileBulk>();
                return null;
            }
        }

        public static IsoTile Find(GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            var tile = gameObject.GetComponentInChildren<IsoTile>();
            if (tile == null)
                tile = gameObject.GetComponentInParent<IsoTile>();
            return tile;
        }

        public bool IsAccumulatedTile_Collider(Vector3 _direction)
        {
            Vector3 _xyz = coordinates._xyz;
            List<IsoTile> _tiles = Bulk.GetTiles_At(_xyz, _direction, false, true);

            Bounds _bounds = GetBounds();
            // Vector3 _diff = transform.position - _bounds.center;
            // _bounds.SetMinMax(_bounds.min + 2f * _diff, _bounds.max + 2f * _diff);
            for (int i = 0; i < _tiles.Count; ++i)
            {
                if (_tiles[i].GetBounds().Intersects(_bounds))
                    return true;
            }
            return false;
        }

        public void Clear_Attachment(bool bCanUndo)
        {
            Iso2DObject[] _iso2Ds = transform.GetComponentsInChildren<Iso2DObject>();
            for (int i = 0; i < _iso2Ds.Length; ++i)
            {
                Iso2DObject _iso2D = _iso2Ds[i];
                if (_iso2D != null && _iso2D.IsAttachment)
                    _iso2D.DestoryGameObject(bCanUndo, false);
            }
        }
        #endregion

#if UNITY_EDITOR
        #region MapEditor
        [SerializeField]
        AutoNaming _autoName = null;
        [HideInInspector]
        public AutoNaming autoName
        {
            get
            {
                return _autoName == null ?
                    _autoName = GetComponent<AutoNaming>() : _autoName;
            }
        }

		public void Rename()
		{
			autoName.AutoName(); 
		}

		[SerializeField]
        IsometricSortingOrder _so = null;
		[HideInInspector]
        public IsometricSortingOrder sortingOrder{get{
            return _so != null ? _so : _so = GetComponent<IsometricSortingOrder>();
        }}

        public void Update_SortingOrder()
        {
            if (sortingOrder != null)
			{
                sortingOrder.Update_SortingOrder(true);
			}
        }
		
		[HideInInspector, SerializeField]
        public AttachedIso2Ds _attachedList = new AttachedIso2Ds();
		public void Update_AttachmentList()
        { 
			_attachedList.Init(gameObject);   
        }

		[SerializeField]
		public bool bAutoFit_ColliderScale = true;
		[SerializeField]
		public bool bAutoFit_SpriteSize = true;

		public bool IsUnionCube()
		{
			return _attachedList.childList.Exists(r => r.AttachedObj._Type == Iso2DObject.Type.Side_Union);
		}		

#region MonoBehaviour
		void OnEnable()
		{
			Update_AttachmentList();
		}
		void OnTransformParentChanged()
		{
			_bulk = null;
		}
		void OnTransformChildrenChanged()
		{
			if (autoName.bPostfix_Sprite)
				Rename();
			Update_AttachmentList();
		}
#endregion MonoBehaviour
        private static bool KeepOrKick(GameObject go, bool bIncludeSide, bool bIncludeAttachment)
        {
            var enumerator = go.GetComponentsInChildren<Iso2DObject>();
            return enumerator.Any(iso2D => (bIncludeSide && (iso2D.IsTileRCAttachment || iso2D.IsSideOfTile)) || (bIncludeAttachment && iso2D.IsColliderAttachment));
        }
        public void Copycat(IsoTile from, bool bCopyBody ,bool bCopyChild = true, bool bUndoable = true)
        {
            copycat(from, bCopyBody, bCopyChild, bUndoable);
        }
        public void Copycat(IsoTile from, bool bCopyChild = true, bool bUndoable = true)
		{
            copycat(from, true, bCopyChild, bUndoable);
        }
        void copycat(IsoTile from, bool bCopySide = true, bool bCopyAttachment = true, bool bUndoable = true)
        {
            if (from == this || !(bCopySide || bCopyAttachment))
                return;

            string undoName = "IsoTile:Copycat";
            Undo.RecordObject(gameObject, undoName);

            List<GameObject> newList = new List<GameObject>();

            List<IsoLight> lights = GetLights();
            var isoEnumerator = GetComponentsInChildren<Iso2DObject>().GetEnumerator();

            foreach (Transform child in from.transform)
            {
                if (KeepOrKick(child.gameObject, bCopySide, bCopyAttachment))
                    newList.Add(GameObject.Instantiate(child.gameObject, transform, false));

                if (newList.Count > 0)
                {
                    var newOne = newList.Last();
                    if (bUndoable)
                        Undo.RegisterCreatedObjectUndo(newOne, undoName);
                }
            }

            LightRecivers_RemoveAll(bUndoable);

            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                GameObject current = transform.GetChild(i).gameObject;
                if (newList.Contains(current)
                    || !KeepOrKick(current, bCopySide, bCopyAttachment))
                    continue;

                if (bUndoable)
                    Undo.DestroyObjectImmediate(current);
                else
                    DestroyImmediate(current);
            }

            for (int i = 0; i < lights.Count; ++i)
            {
                if (lights[i] != null)
                    lights[i].AddTarget(transform.gameObject, true);
            }

            LightRecivers_UpdateAll();

            if (!IsoMap.IsNull && IsoMap.instance.bUseIsometricSorting)
                sortingOrder.Update_SortingOrder();
            else
                sortingOrder.Reset_SortingOrder(0, false);

            coordinates.Apply_SnapToGrid();
        }
        void copycat_origin(IsoTile from, bool bCopyBody = true, bool bCopyChild = true, bool bUndoable = true)
        {
            if (from == this)
                return;

            string undoName = "IsoTile:Copycat";
            Undo.RecordObject(gameObject, undoName);

            if (bCopyChild)
            {
                List<GameObject> newList = new List<GameObject>();
                List<IsoLight> lights = GetLights();

                foreach (Transform child in from.transform)
                {
                    newList.Add(GameObject.Instantiate(child.gameObject, transform, false));
                    var newOne = newList.Last();
                    if (bUndoable)
                        Undo.RegisterCreatedObjectUndo(newOne, undoName);
                }

                LightRecivers_RemoveAll(bUndoable);

                for (int i = transform.childCount - 1; i >= 0; --i)
                {
                    GameObject current = transform.GetChild(i).gameObject;
                    if (newList.Contains(current))
                        continue;

                    if (bUndoable)
                        Undo.DestroyObjectImmediate(current);
                    else
                        DestroyImmediate(current);
                }

                for (int i = 0; i < lights.Count; ++i)
                {
                    if (lights[i] != null)
                        lights[i].AddTarget(transform.gameObject, true);
                }
                LightRecivers_UpdateAll();
            }
            sortingOrder.Reset_SortingOrder(0, false);
            coordinates.Apply_SnapToGrid();
            // Update_AttachmentList();
        }

        public List<IsoLight> GetLights()
        {
            List <IsoLight>  lights = new List<IsoLight>();
            var lightsEnum = transform.GetComponentsInChildren<IsoLightReciver>().Select(r => r.GetAllLightList()).GetEnumerator();
            while (lightsEnum.MoveNext())
            {
                lights.AddRange(lightsEnum.Current as IsoLight[]);
            }
            return lights.Distinct().ToList();
        }

		void Clear_SideObject(bool bCanUndo)
        {
			Iso2DObject[] _sideObjects = GetSideObjects(
				Iso2DObject.Type.Side_X, Iso2DObject.Type.Side_Y, 
				Iso2DObject.Type.Side_Z, Iso2DObject.Type.Side_Union);

            for (int i = 0; i < _sideObjects.Length; ++i)
			{
				if (_sideObjects[i] != null)
				{
					_sideObjects[i].DestoryGameObject(bCanUndo, true);
				}
			}
        }

		void Add_SideObject(GameObject _prefab, string _UndoMSG)
		{
			GameObject _obj = GameObject.Instantiate(_prefab, transform, false);
			_obj.transform.SetAsFirstSibling();
			RegularCollider _rc = _obj.GetComponent<RegularCollider>();
            _rc.Toggle_UseGridTileScale(bAutoFit_ColliderScale);
			Undo.RegisterCreatedObjectUndo(_obj, _UndoMSG);
			Update_AttachmentList();
		}

		public void Reset_SideObject(bool _bTrueUnion)
		{
			Clear_SideObject(true);
			Add_SideObject(_bTrueUnion ? IsoMap.Prefab_Side_Union : IsoMap.Prefab_Side_Y, "Change Tile Style");
		}

		public void Toggle_Side(bool _bToggle, Iso2DObject.Type _toggleType)
		{
			Iso2DObject _obj = GetSideObject(_toggleType);
			if (_bToggle)
            {
                if (_obj == null)
                {
					Add_SideObject(IsoMap.GetSidePrefab(_toggleType),
						"Created : " + _toggleType + " Object");
                }
            }
            else
            {
                if (_obj != null)
                {
					_obj.DestoryGameObject(true, true);
                    Update_AttachmentList();
                }
            }
		}

        public bool IsAccumulatedTile_Coordinates(Vector3 _direction)
        {
			Vector3 _xyz = coordinates._xyz;
            List<IsoTile> _tiles = Bulk.GetTiles_At(_xyz, _direction, false, true);

            int iCheckValue = coordinates.CoordinatesCountInTile(_direction);
			
            iCheckValue *= iCheckValue;
            for(int i = 0 ; i < _tiles.Count ; ++i)
            {
                Vector3 diff = Vector3.Scale(_xyz - _tiles[i].coordinates._xyz, _direction);
                if (Mathf.RoundToInt(diff.sqrMagnitude) < iCheckValue)
                {
                    return true;
                }
            }
            return false;
        }  
        
        public void MoveToZeroground()
        {
            Vector3 _ZeroGround = coordinates._xyz;
            coordinates.Move(_ZeroGround.x, 0, _ZeroGround.z, "IsoTile:MoveToZeroGround");
        }

		public void Init()
		{
            Update_ColliderScale();
            Update_Attached_Iso2DScale();
        }

        public void Update_Grid()
		{
			coordinates.Update_Grid(true);
            Update_ColliderScale();
            Update_Attached_Iso2DScale(true, "IsoTile: Update Grid");
        }

        public void Update_ColliderScale()
        {
            RegularCollider[] _RCs = GetComponentsInChildren<RegularCollider>();
            foreach (var _RC in _RCs)
            {
                _RC.Toggle_UseGridTileScale(bAutoFit_ColliderScale);
                _RC.AdjustScale();
            }
        }

		public void Update_Attached_Iso2DScale(bool bUndoable = false, string undoName = null)
		{
            foreach (var _attached in _attachedList.childList)
                Update_Attached_Iso2DScale(_attached.AttachedObj, bUndoable, undoName);
		}

        public void Update_Attached_Iso2DScale(Iso2DObject _Iso2D, bool bUndoable = false, string undoName = null)
        {
            if (_Iso2D != null)
            {
                if (bUndoable)
                    Undo.RecordObject(_Iso2D, undoName);
                _Iso2D.UpdateIsometricRotationScale();
                if (bUndoable)
                    Undo.RecordObject(_Iso2D.transform, undoName);
                _Iso2D.AdjustScale();
            }
        }

        public void SyncIsoLight(GameObject target)
        {
            var allLightRecivers = target.GetComponentsInChildren<IsoLightReciver>();
            if (allLightRecivers != null && allLightRecivers.Length > 0)
            {
                foreach (var one in allLightRecivers)
                    one.ClearAllLights();
            }

            allLightRecivers = GetComponentsInChildren<IsoLightReciver>().Where(r => !allLightRecivers.Contains(r)).ToArray();
            allLightRecivers.Select(r => r.GetAllLightList());
            List<IsoLight> allLights = new List<IsoLight>();
            foreach (var one in allLightRecivers)
                allLights.AddRange(one.GetAllLightList().Where(r => !allLights.Contains(r)));
            allLights.ForEach(r => r.AddTarget(target, true));
        }

        public void LightRecivers_RevertAll()
        {
            var lightRecivers = transform.GetComponentsInChildren<IsoLightReciver>();

            foreach (var one in lightRecivers)
                one.RevertSpriteRendererColor();
        }

        public void LightRecivers_RemoveAll(bool bUndoable)
        {
            var lightRecivers = transform.GetComponentsInChildren<IsoLightReciver>();

            foreach (var one in lightRecivers)
            {
#if UNITY_EDITOR
                if (bUndoable)
                    Undo.DestroyObjectImmediate(one);
                else
                    DestroyImmediate(one, true);
#else
                DestroyImmediate(one);
#endif
            }
        }

        public void LightRecivers_UpdateAll()
        {
            var lightRecivers = transform.GetComponentsInChildren<IsoLightReciver>();
            for (int i = 0; i < lightRecivers.Length; ++i)
                lightRecivers[i].UpdateLightColor();
        }
#endregion MapEditor
#endif
    }
}