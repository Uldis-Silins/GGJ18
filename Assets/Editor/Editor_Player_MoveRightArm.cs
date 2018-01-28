using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player_MoveRightArm))]
public class Editor_Player_MoveRightArm : Editor
{
    private const float ROTATION_TEXT_DISTANCE = 0.7f;
    private readonly Color[] JOINT_COLORS = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta };

    public bool showInfo;

    private void OnSceneGUI()
    {
        if (showInfo)
        {
            Player_MoveRightArm rightArm = target as Player_MoveRightArm;

            foreach (var joint in rightArm.jointTransforms)
            {
                Handles.color = JOINT_COLORS[ArrayUtility.IndexOf(rightArm.jointTransforms, joint)];
                Vector3 labelPosition = (joint.forward * ROTATION_TEXT_DISTANCE) + joint.position;
                Handles.DrawLine(joint.transform.position, labelPosition);
                Handles.Label(labelPosition, joint.eulerAngles.ToString());
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
