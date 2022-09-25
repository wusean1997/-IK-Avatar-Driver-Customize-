namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEditor;

    [CustomEditor(typeof(CarInputSettings))]
    public class Editor_CarPlayerInputSettings : Editor
    {
        public override void OnInspectorGUI()
        {
            CarInputSettings vehicleInput = (CarInputSettings)target;

            SerializedProperty defaultCanvas = serializedObject.FindProperty("defaultCanvas");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(defaultCanvas, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            SerializedProperty mobileCanvas = serializedObject.FindProperty("mobileCanvas");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mobileCanvas, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            SerializedProperty uIType = serializedObject.FindProperty("uIType");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(uIType, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            if (vehicleInput.uIType == UIType.Mobile)
            {
                SerializedProperty mobileSteeringType = serializedObject.FindProperty("mobileSteeringType");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(mobileSteeringType, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }

            SerializedProperty inputAxes = serializedObject.FindProperty("inputAxes");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(inputAxes, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(vehicleInput);
        }
    }
}