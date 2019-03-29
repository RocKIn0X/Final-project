using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.EventSystems;
#endif

namespace Anonym.Isometric
{
    using Util;
    [DisallowMultipleComponent]
    [System.Serializable]
    [ExecuteInEditMode][DefaultExecutionOrder(1)]
    public class GridCoordinates : MonoBehaviour, IGridOperator
    {
        #region Grid
        [SerializeField]
        Grid _grid;
        public Grid grid
        {
            get
            {
                if (_grid == null)
                {
                    GridReset();
                }
                return _grid;
            }
        }
        public void GridReset()
        {
            _grid = GetComponent<Grid>();

            if (_grid == null)
            {
                Transform _parent = transform.parent;
                while (_grid == null && _parent != null)
                {
                    _grid = _parent.GetComponent<Grid>();
                    _parent = _parent.parent;
                }

                if (_grid == null && !IsoMap.IsNull)
                {
                    _grid = IsoMap.instance.gGrid;
                }
            }
        }
        #endregion

        #region Coordinates
        public bool bSnapFree = false;

        Vector3 _lastXYZ;
        public Vector3 _xyz
        {
            get
            {
                return _lastXYZ;
            }
        }//xyz(transform.localPosition);} }

        public void UpdateXYZ()
        {
            if (grid == null)
                return;

            _lastXYZ = PositionToCoordinates(transform.localPosition, !bSnapFree);
#if UNITY_EDITOR
            update_LastLocalPosition();
#endif
        }
        public void Translate(Vector3 _coord, string _undoName = "Coordinates:Move")
        {
            Translate(Mathf.RoundToInt(_coord.x), Mathf.RoundToInt(_coord.y), Mathf.RoundToInt(_coord.z), _undoName);
        }

        public void Translate(int _x, int _y, int _z, string _undoName = "Coordinates:Move")
        {
#if UNITY_EDITOR
            Undo.RecordObject(transform, _undoName);
#endif
            gameObject.transform.localPosition +=
                new Vector3(GridInterval.x * _x, GridInterval.y * _y, GridInterval.z * _z);
#if UNITY_EDITOR
            Undo.RecordObject(gameObject, _undoName);
#endif
            UpdateXYZ();
        }

        public void MoveToWorldPosition(Vector3 position)
        {
            gameObject.transform.position = position;
            Apply_SnapToGrid();
        }

        public void Move(Vector3 _coord, string _undoName = "Coordinates:Move")
        {
            Move(_coord.x, _coord.y, _coord.z, _undoName);
        }

        public void Move(float _x, float _y, float _z, string _undoName = "Coordinates:Move")
        {
            Move(Mathf.RoundToInt(_x), Mathf.RoundToInt(_y), Mathf.RoundToInt(_z), _undoName);
        }

        public void Move(int _x, int _y, int _z, string _undoName = "Coordinates:Move")
        {
#if UNITY_EDITOR
            Undo.RecordObject(transform, _undoName);
#endif
            gameObject.transform.localPosition =
                new Vector3(GridInterval.x * _x, GridInterval.y * _y, GridInterval.z * _z);
#if UNITY_EDITOR
            Undo.RecordObject(gameObject, _undoName);
#endif
            UpdateXYZ();
        }

        public void Apply_SnapToGrid()
        {
            if (bSnapFree || grid == null)
                return;// + grid.Centor;

            UpdateXYZ();
            Vector3 v3Delta = transform.localPosition;

            v3Delta -= Vector3.Scale(_lastXYZ, GridInterval);

            if (v3Delta != Vector3.zero)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(transform, "IsoTile:Move");
#endif
                transform.localPosition -= v3Delta;
#if UNITY_EDITOR
                update_LastLocalPosition();
#endif
            }
        }

        public static bool IsSameWithTolerance(Vector3 _coordinatesA, Vector3 _coordinatesB, float fTolerance)
        {
            return (_coordinatesA - _coordinatesB).magnitude < fTolerance;
        }

        public bool IsSame(Vector3 _ref_coordinates, bool X = true, bool Y = true, bool Z = true)
        {
            return (!X || _ref_coordinates.x.Equals(_lastXYZ.x))
                && (!Y || _ref_coordinates.y.Equals(_lastXYZ.y))
                && (!Z || _ref_coordinates.z.Equals(_lastXYZ.z));
        }
        #endregion

        #region IGridOperator
        public Vector3 PositionToCoordinates(Vector3 position, bool bSnap = false)
        {
            if (grid)
                return _grid.PositionToCoordinates(position, bSnap);
            return position;
        }
        public Vector3 CoordinatesToPosition(Vector3 coordinates, bool bSnap = false)
        {
            if (grid)
                return _grid.CoordinatesToPosition(coordinates, bSnap);
            return coordinates;
        }
        public int CoordinatesCountInTile(Vector3 _direction)
        {
            Vector3 result = Vector3.Scale(_direction, TileSize);
            Vector3 size = GridInterval;
            return Mathf.Abs(Mathf.RoundToInt(result.x / size.x + result.y / size.y + result.z / size.z));
        }

        IsoTile _tile = null;
        IsoTile Tile
        {
            get
            {
                if (!_tile)
                    _tile = IsoTile.Find(gameObject);
                return _tile;
            }
        }
        public Vector3 TileSize
        {
            get
            {
                if (grid)
                    return _grid.TileSize;

                if (Tile)
                {
                    var bounds = Tile.GetBounds_SideOnly();
                    return bounds.size;
                }

                return Vector3.one;
            }
        }
        public Vector3 GridInterval
        {
            get
            {
                if (grid)
                    return _grid.GridInterval;

                if (Tile)
                {
                    var bounds = Tile.GetBounds_SideOnly();
                    return bounds.size;
                }

                return Vector3.zero;
            }
        }
        public bool IsInheritGrid
        {
            get
            {
                if (grid)
                    return _grid.IsInheritGrid;

                return false;
            }
        }
        #endregion

        void _reset()
        {
            _grid = null;
            UpdateXYZ();
        }

        private void OnEnable()
        {
            _reset();
        }

#if UNITY_EDITOR

        [SerializeField]
        AutoNaming _autoName;

        [HideInInspector]
        public AutoNaming autoName
        {
            get
            {
                return _autoName == null ?
                    _autoName = GetComponent<AutoNaming>() : _autoName;
            }
        }

        void OnTransformParentChanged()
        {
            _reset();
        }

        public bool bChangedforEditor = false;
        bool bIgnoreTransformChanged = false;

        void Update()
        {         
			if (!Application.isEditor || Application.isPlaying || !enabled)
                // ||  gameObject.transform.root == gameObject.transform)
				return;
			
            if (transform.hasChanged)
            {
                if (bIgnoreTransformChanged == true)
                    bIgnoreTransformChanged = false;
                else
                    Update_TransformChanged();
            }
		}

        public void Update_TransformChanged()
        {
            Apply_SnapToGrid();
            bChangedforEditor = true;
        }

		
        [HideInInspector]
        Vector3 _lastLocalPosition;
        void update_LastLocalPosition()
        {
            if (_lastLocalPosition != transform.localPosition)
            {
                _lastLocalPosition = transform.localPosition;
                Rename();
            }
        }

        public void Update_Grid(bool _bIgnoreTransformChanged)
        {
            Vector3 _NewPos = Vector3.Scale(GridInterval, _lastXYZ);
			if (_NewPos != Vector3.zero)
			{                
			    UnityEditor.Undo.RecordObject(transform, "IsoTile:Move");
				transform.localPosition = _NewPos;
				update_LastLocalPosition();
                bIgnoreTransformChanged = _bIgnoreTransformChanged;
			}
        }
        
        public void Rename()
		{
			if (autoName != null)
				autoName.AutoName();
		}

        public void Sync(GridCoordinates with)
        {
            bSnapFree = with.bSnapFree;
            _reset();
        }
#endif
    }
}