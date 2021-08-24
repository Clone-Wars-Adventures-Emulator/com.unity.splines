using Unity.Mathematics;
using UnityEngine.Splines;
using UnityEngine;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.EditorTools;
#else
using ToolManager = UnityEditor.EditorTools.EditorTools;
#endif

namespace UnityEditor.Splines
{
    static class SplineMenu
    {
        const string k_MenuPath = "GameObject/3D Object/Spline";

        static GameObject CreateSplineGameObject(MenuCommand menuCommand, Spline spline = null)
        {
            var name = GameObjectUtility.GetUniqueNameForSibling(null, "Spline");
            var gameObject = ObjectFactory.CreateGameObject(name, typeof(SplineContainer));
            
#if ST_EXPOSE_GO_CREATE_PLACEMENT_LANDED
            ObjectFactory.PlaceGameObject(gameObject, menuCommand.context as GameObject);
#else
            if (menuCommand.context is GameObject go)
            {
                Undo.RecordObject(gameObject.transform, "Re-parenting");
                gameObject.transform.SetParent(go.transform);
            }
#endif

            if (spline != null)
            {
                var container = gameObject.GetComponent<SplineContainer>();
                container.Spline = spline;
                Selection.activeGameObject = gameObject;
            }

            return gameObject;
        }
        
        [MenuItem(k_MenuPath + "/Freeform")]
        static void CreateNewSpline(MenuCommand menuCommand)
        {
            var gameObject = CreateSplineGameObject(menuCommand);

            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

            Selection.activeObject = gameObject;
            ActiveEditorTracker.sharedTracker.RebuildIfNecessary();
            ToolManager.SetActiveContext<SplineToolContext>();
            ToolManager.SetActiveTool<KnotPlacementTool>();
        }
        
        [MenuItem(k_MenuPath + "/Square")]
        static void CreateSquare(MenuCommand command)
        {
            CreateSplineGameObject(command, SplineFactory.CreateSquare(1f));
        }
        
        [MenuItem(k_MenuPath + "/Circle")]
        static void CreateCircle(MenuCommand command)
        {
            // .36 is just an eye-balled approximation
            CreateSplineGameObject(command, SplineFactory.CreateRoundedSquare(1f, .36f));
        }
    }
}