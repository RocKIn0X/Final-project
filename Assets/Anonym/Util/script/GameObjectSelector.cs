using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Util
{
    using Isometric;

    [RequireComponent(typeof(BoxCollider))]
    [ExecuteInEditMode]
    public class GameObjectSelector : MethodBTN_MonoBehaviour
    {
        [SerializeField]
        Color colliderColor = new Color32(232, 129, 255, 74);

        [SerializeField]
        Color gizmoColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);

        [SerializeField]
        List<Collider> colliders = new List<Collider>();

        List<Bounds> boundsForGizmo = new List<Bounds>();

        [SerializeField]
        List<GameObject> gameObjects = new List<GameObject>();

        [SerializeField]
        List<System.Type> ComponentsForSelection = new List<System.Type>() {
            { typeof(IsoTile)},
        };

        void updateCollider()
        {
            colliders.Clear();
            colliders.AddRange(GetComponentsInChildren<Collider>());
        }

        void check()
        {
            gameObjects.Clear();
            boundsForGizmo.Clear();
            colliders.RemoveAll(r => r == null);

            foreach (var one in colliders)
            {
                Bounds bounds = one.bounds;
                var results = Physics.OverlapBox(bounds.center, bounds.extents);
                foreach (var result in results)
                {
                    var go = result.gameObject;
                    Component com = null;
                    if (ComponentsForSelection.Any(r => ((com = go.GetComponentInParent(r)) != null)))
                    {
                        if (!gameObjects.Contains(com.gameObject))
                        {
                            gameObjects.Add(com.gameObject);
                            boundsForGizmo.Add(result.bounds);
                        }
                    }
                }
            }
        }

        private void Awake()
        {
            updateCollider();

            foreach (var one in colliders)
                one.isTrigger = true;
        }

        private void OnDrawGizmosSelected()
        {
            check();

            Gizmos.color = gizmoColor;
            foreach (var one in boundsForGizmo)
            {
                Gizmos.DrawWireCube(one.center, one.size);
            }

            Gizmos.color = colliderColor;
            foreach (var one in colliders)
            {
                Bounds bounds = one.bounds;
                Gizmos.DrawCube(bounds.center, bounds.size);
            }
        }

#if UNITY_EDITOR
        [MethodBTN(false)]
        public void UpdateCollider()
        {
            updateCollider();
            check();
        }

        [MethodBTN(false)]
        public void Select()
        {
            UnityEditor.Selection.objects = gameObjects.ToArray();
        }
#endif
    }
}