#region UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Morkwa.Test.Mechanics.AI
{
    [CustomEditor(typeof(AIView))]
    public class FieldOfViewEditor : Editor
    {
        private void OnSceneGUI()
        {
            AIView fow = (AIView)target;

            Handles.color = Color.yellow;
            Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.GetRadiusView());

            Vector3 viewAngleA = fow.DirectionFromAngle(-fow.GetViewAngle() / 2, false);
            Vector3 viewAngleB = fow.DirectionFromAngle(fow.GetViewAngle() / 2, false);

            Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.GetRadiusView());
            Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.GetRadiusView());

            Handles.color = Color.green;

            foreach (Transform visibleTarget in fow.GetVisibleTargets())
            {
                Handles.DrawLine(fow.transform.position, visibleTarget.transform.position);
            }
        }
    }
}
#endregion