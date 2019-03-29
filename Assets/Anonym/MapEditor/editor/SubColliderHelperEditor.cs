using UnityEditor;
using System.Linq;

namespace Anonym.Isometric
{
    using Util;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(SubColliderHelper))]
    public class SubColliderHelperEditor : Editor
    {
        bool IsPrefab = false;
        bool bAutoReParent = false;

        private void OnEnable()
        {
            if (IsPrefab = PrefabHelper.IsPrefab(targets.Select(r => (r as SubColliderHelper).gameObject).ToArray()))
                return;
        }

        public override void OnInspectorGUI()
        {
            if (IsPrefab)
            {
                base.DrawDefaultInspector();
                return;
            }

            CustomEditorGUI.ColliderControlHelperGUI(targets);
        }

        private void OnSceneGUI()
        {
            if (IsPrefab)
                return;
        }

    }
}
