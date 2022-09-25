namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(CarDriveSystem))]
    public class Editor_VehicleDriveSystem : Editor
    {
        private bool showInputOverrideSettings;
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((CarDriveSystem)target), typeof(CarDriveSystem), false);
            GUI.enabled = true;

            SerializedProperty vehicleSettings = serializedObject.FindProperty("vehicleSettings");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(vehicleSettings, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            SerializedProperty wheelSettings = serializedObject.FindProperty("wheelSettings");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(wheelSettings, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            #region Input Override Settings
            showInputOverrideSettings = EditorGUILayout.Foldout(showInputOverrideSettings, "Input Override Settings", true);
            if (showInputOverrideSettings)
            {
                EditorGUILayout.BeginVertical("Box");

                SerializedProperty overrideBrake = serializedObject.FindProperty("overrideBrake");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(overrideBrake, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                SerializedProperty overrideAcceleration = serializedObject.FindProperty("overrideAcceleration");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(overrideAcceleration, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                SerializedProperty overrideSteering = serializedObject.FindProperty("overrideSteering");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(overrideSteering, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                SerializedProperty overrideSteeringPower = serializedObject.FindProperty("overrideSteeringPower");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(overrideSteeringPower, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                SerializedProperty overrideBrakePower = serializedObject.FindProperty("overrideBrakePower");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(overrideBrakePower, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                SerializedProperty overrideAccelerationPower = serializedObject.FindProperty("overrideAccelerationPower");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(overrideAccelerationPower, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                EditorGUILayout.EndVertical();
            }
            #endregion
            EditorGUILayout.Space();

            SerializedProperty wheels = serializedObject.FindProperty("wheels");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(wheels, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Align Wheel Colliders"))
            {
                AlignWheelColliders();
            }
        }

        public void AlignWheelColliders()
        {
            CarDriveSystem driveSystem = (CarDriveSystem)target;
            Transform defaultColliderParent = driveSystem.wheels[0].collider.transform.parent; // make a reference to the colliders original parent

            driveSystem.wheels[0].collider.transform.parent = driveSystem.wheels[0].mesh.transform;// move colliders to the reference positions
            driveSystem.wheels[1].collider.transform.parent = driveSystem.wheels[1].mesh.transform;
            driveSystem.wheels[2].collider.transform.parent = driveSystem.wheels[2].mesh.transform;
            driveSystem.wheels[3].collider.transform.parent = driveSystem.wheels[3].mesh.transform;

            driveSystem.wheels[0].collider.transform.position = new Vector3(driveSystem.wheels[0].mesh.transform.position.x,
                driveSystem.wheels[0].collider.transform.position.y, driveSystem.wheels[0].mesh.transform.position.z); //adjust the wheel collider positions on x and z axis to match the new wheel position
            driveSystem.wheels[1].collider.transform.position = new Vector3(driveSystem.wheels[1].mesh.transform.position.x,
                driveSystem.wheels[1].collider.transform.position.y, driveSystem.wheels[1].mesh.transform.position.z);
            driveSystem.wheels[2].collider.transform.position = new Vector3(driveSystem.wheels[2].mesh.transform.position.x,
                driveSystem.wheels[2].collider.transform.position.y, driveSystem.wheels[2].mesh.transform.position.z);
            driveSystem.wheels[3].collider.transform.position = new Vector3(driveSystem.wheels[3].mesh.transform.position.x,
                driveSystem.wheels[3].collider.transform.position.y, driveSystem.wheels[3].mesh.transform.position.z);

            driveSystem.wheels[0].collider.transform.parent = defaultColliderParent; // move colliders back to the original parent
            driveSystem.wheels[1].collider.transform.parent = defaultColliderParent;
            driveSystem.wheels[2].collider.transform.parent = defaultColliderParent;
            driveSystem.wheels[3].collider.transform.parent = defaultColliderParent;
        }
    }
}