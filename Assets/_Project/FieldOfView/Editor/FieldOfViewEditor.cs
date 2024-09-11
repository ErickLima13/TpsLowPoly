using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(fow.transform.position, Vector3.up,
            Vector3.forward, 360, fow.viewRadius);

        Handles.color = Color.red;

        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2);

        Handles.DrawLine(fow.transform.position,
            fow.transform.position + viewAngleA * fow.viewRadius);

        Handles.DrawLine(fow.transform.position,
          fow.transform.position + viewAngleB * fow.viewRadius);

        // segundo campo de visão
        Vector3 viewAngleAb = fow.DirFromAngle(-fow.viewAngleSecondary / 2);
        Vector3 viewAngleBb = fow.DirFromAngle(fow.viewAngleSecondary / 2);

        Handles.color = Color.yellow;

        Handles.DrawLine(fow.transform.position,
            fow.transform.position + viewAngleAb * fow.viewRadius);

        Handles.DrawLine(fow.transform.position,
          fow.transform.position + viewAngleBb * fow.viewRadius);

    }
}
