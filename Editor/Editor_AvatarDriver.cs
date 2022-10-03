namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(AvatarDriver))]
    public class Editor_AvatarDriver : Editor
    {
        private bool showCurrentIKDriverTargets, showCurrentIKTargetObjects, showIKSteeringWheelTargets, showOtherIKTargetObjects, showObjectParents, showIKPickIKTargets, showPickObjects, showLookTarget, showHand, showIKGestureTargets;

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((AvatarDriver)target), typeof(AvatarDriver), false);
            GUI.enabled = true;

            EditorGUILayout.BeginVertical("Box");
            EditorGUI.BeginChangeCheck();

            SerializedProperty avatarSettings = serializedObject.FindProperty("avatarSettings");
            EditorGUILayout.PropertyField(avatarSettings, true);

            SerializedProperty animator = serializedObject.FindProperty("animator");
            EditorGUILayout.PropertyField(animator, true);

            SerializedProperty steeringWheel = serializedObject.FindProperty("steeringWheel");
            EditorGUILayout.PropertyField(steeringWheel, true);

            SerializedProperty bodyJoint = serializedObject.FindProperty("bodyJoint");
            EditorGUILayout.PropertyField(bodyJoint, true);

            SerializedProperty readOnlySteeringWheel = serializedObject.FindProperty("readOnlySteeringWheel");
            EditorGUILayout.PropertyField(readOnlySteeringWheel, true);

            SerializedProperty vehicleRigidbody = serializedObject.FindProperty("vehicleRigidbody");
            EditorGUILayout.PropertyField(vehicleRigidbody, true);

            SerializedProperty gearText = serializedObject.FindProperty("gearText");
            EditorGUILayout.PropertyField(gearText, true);

            SerializedProperty Hip = serializedObject.FindProperty("Hip");
            EditorGUILayout.PropertyField(Hip, true);

            SerializedProperty Jaw = serializedObject.FindProperty("Jaw");
            EditorGUILayout.PropertyField(Jaw, true);

            SerializedProperty use = serializedObject.FindProperty("use");
            EditorGUILayout.PropertyField(use, true);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            showLookTarget = EditorGUILayout.Foldout(showLookTarget, "Look Target", true);
            if (showLookTarget)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.BeginChangeCheck();

                SerializedProperty Look_01 = serializedObject.FindProperty("Look_01");
                EditorGUILayout.PropertyField(Look_01, true);

                SerializedProperty Look_02 = serializedObject.FindProperty("Look_02");
                EditorGUILayout.PropertyField(Look_02, true);

                SerializedProperty Look_03 = serializedObject.FindProperty("Look_03");
                EditorGUILayout.PropertyField(Look_03, true);
                
                SerializedProperty Look_04 = serializedObject.FindProperty("Look_04");
                EditorGUILayout.PropertyField(Look_04, true);

                SerializedProperty Look_05 = serializedObject.FindProperty("Look_05");
                EditorGUILayout.PropertyField(Look_05, true);
                
                SerializedProperty Look_06 = serializedObject.FindProperty("Look_06");
                EditorGUILayout.PropertyField(Look_06, true);

                SerializedProperty Look_07 = serializedObject.FindProperty("Look_07");
                EditorGUILayout.PropertyField(Look_07, true);

                SerializedProperty Look_08 = serializedObject.FindProperty("Look_08");
                EditorGUILayout.PropertyField(Look_08, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            showObjectParents = EditorGUILayout.Foldout(showObjectParents, "Pick Obj Parents", true);
            if (showObjectParents)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.BeginChangeCheck();

                SerializedProperty Object_pA = serializedObject.FindProperty("Object_pA");
                EditorGUILayout.PropertyField(Object_pA, true);

                SerializedProperty Object_pB = serializedObject.FindProperty("Object_pB");
                EditorGUILayout.PropertyField(Object_pB, true);

                SerializedProperty Object_pC = serializedObject.FindProperty("Object_pC");
                EditorGUILayout.PropertyField(Object_pC, true);

                SerializedProperty Object_pD = serializedObject.FindProperty("Object_pD");
                EditorGUILayout.PropertyField(Object_pD, true);

                SerializedProperty handTemp = serializedObject.FindProperty("handTemp");
                EditorGUILayout.PropertyField(handTemp, true);

                SerializedProperty handTempIK = serializedObject.FindProperty("handTempIK");
                EditorGUILayout.PropertyField(handTempIK, true);

                SerializedProperty Head = serializedObject.FindProperty("Head");
                EditorGUILayout.PropertyField(Head, true);

                SerializedProperty HeadSubstitude = serializedObject.FindProperty("HeadSubstitude");
                EditorGUILayout.PropertyField(HeadSubstitude, true);

                SerializedProperty GlassesSubstitude = serializedObject.FindProperty("GlassesSubstitude");
                EditorGUILayout.PropertyField(GlassesSubstitude, true); 

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }


            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            showIKPickIKTargets = EditorGUILayout.Foldout(showIKPickIKTargets, "Pick IK Target", true);
            if (showIKPickIKTargets)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.BeginChangeCheck();

                SerializedProperty Object_A = serializedObject.FindProperty("Object_A");
                EditorGUILayout.PropertyField(Object_A, true);

                SerializedProperty Object_B = serializedObject.FindProperty("Object_B");
                EditorGUILayout.PropertyField(Object_B, true);

                SerializedProperty Object_C = serializedObject.FindProperty("Object_C");
                EditorGUILayout.PropertyField(Object_C, true);

                SerializedProperty Object_D = serializedObject.FindProperty("Object_D"); 
                EditorGUILayout.PropertyField(Object_D, true);

                SerializedProperty Mouth = serializedObject.FindProperty("Mouth");
                EditorGUILayout.PropertyField(Mouth, true);

                SerializedProperty Beer_Mouth = serializedObject.FindProperty("Beer_Mouth");
                EditorGUILayout.PropertyField(Beer_Mouth, true);

                SerializedProperty Face = serializedObject.FindProperty("Face");
                EditorGUILayout.PropertyField(Face, true);
                
                
                SerializedProperty Hand = serializedObject.FindProperty("Hand");
                EditorGUILayout.PropertyField(Hand, true);

                SerializedProperty phoneHand = serializedObject.FindProperty("phoneHand");
                EditorGUILayout.PropertyField(phoneHand, true);

                SerializedProperty Finger = serializedObject.FindProperty("Finger");
                EditorGUILayout.PropertyField(Finger, true);

                SerializedProperty Center = serializedObject.FindProperty("Center");
                EditorGUILayout.PropertyField(Center, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            showIKGestureTargets = EditorGUILayout.Foldout(showIKGestureTargets, "Gesture", true);
            if (showIKGestureTargets)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.BeginChangeCheck();

                SerializedProperty Gesture = serializedObject.FindProperty("Gesture");
                EditorGUILayout.PropertyField(Gesture, true);

                SerializedProperty Gesture_1 = serializedObject.FindProperty("Gesture_1");
                EditorGUILayout.PropertyField(Gesture_1, true);

                SerializedProperty Gesture_2 = serializedObject.FindProperty("Gesture_2");
                EditorGUILayout.PropertyField(Gesture_2, true);

                SerializedProperty Gesture_3 = serializedObject.FindProperty("Gesture_3");
                EditorGUILayout.PropertyField(Gesture_3, true);

                SerializedProperty Gesture_4 = serializedObject.FindProperty("Gesture_4");
                EditorGUILayout.PropertyField(Gesture_4, true);

                SerializedProperty Gesture_5 = serializedObject.FindProperty("Gesture_5");
                EditorGUILayout.PropertyField(Gesture_5, true);

                SerializedProperty Gesture_6 = serializedObject.FindProperty("Gesture_6");
                EditorGUILayout.PropertyField(Gesture_6, true);

                SerializedProperty Gesture_7 = serializedObject.FindProperty("Gesture_7");
                EditorGUILayout.PropertyField(Gesture_7, true);

                SerializedProperty Gesture_8 = serializedObject.FindProperty("Gesture_8"); 
                EditorGUILayout.PropertyField(Gesture_8, true);

                SerializedProperty Gesture_9 = serializedObject.FindProperty("Gesture_9");
                EditorGUILayout.PropertyField(Gesture_9, true);

                SerializedProperty Gesture_10 = serializedObject.FindProperty("Gesture_10");
                EditorGUILayout.PropertyField(Gesture_10, true);

                SerializedProperty Gesture_11 = serializedObject.FindProperty("Gesture_11");
                EditorGUILayout.PropertyField(Gesture_11, true);

                SerializedProperty Gesture_12 = serializedObject.FindProperty("Gesture_12");
                EditorGUILayout.PropertyField(Gesture_12, true);

                SerializedProperty Gesture_13 = serializedObject.FindProperty("Gesture_13");
                EditorGUILayout.PropertyField(Gesture_13, true);

                SerializedProperty Gesture_14 = serializedObject.FindProperty("Gesture_14");
                EditorGUILayout.PropertyField(Gesture_14, true);

                SerializedProperty Gesture_15 = serializedObject.FindProperty("Gesture_15");
                EditorGUILayout.PropertyField(Gesture_15, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }

            showHand = EditorGUILayout.Foldout(showHand, "GestureHand", true);
            if (showHand)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.BeginChangeCheck();

                SerializedProperty Index_1 = serializedObject.FindProperty("Index_1");
                EditorGUILayout.PropertyField(Index_1, true);
                SerializedProperty Index_2 = serializedObject.FindProperty("Index_2");
                EditorGUILayout.PropertyField(Index_2, true);
                SerializedProperty Index_3 = serializedObject.FindProperty("Index_3");
                EditorGUILayout.PropertyField(Index_3, true);

                SerializedProperty Middle_1 = serializedObject.FindProperty("Middle_1");
                EditorGUILayout.PropertyField(Middle_1, true);
                SerializedProperty Middle_2 = serializedObject.FindProperty("Middle_2");
                EditorGUILayout.PropertyField(Middle_2, true);
                SerializedProperty Middle_3 = serializedObject.FindProperty("Middle_3");
                EditorGUILayout.PropertyField(Middle_3, true);

                SerializedProperty Ring_1 = serializedObject.FindProperty("Ring_1");
                EditorGUILayout.PropertyField(Ring_1, true);
                SerializedProperty Ring_2 = serializedObject.FindProperty("Ring_2");
                EditorGUILayout.PropertyField(Ring_2, true);
                SerializedProperty Ring_3 = serializedObject.FindProperty("Ring_3");
                EditorGUILayout.PropertyField(Ring_3, true);

                SerializedProperty Pinky_1 = serializedObject.FindProperty("Pinky_1");
                EditorGUILayout.PropertyField(Pinky_1, true);
                SerializedProperty Pinky_2 = serializedObject.FindProperty("Pinky_2");
                EditorGUILayout.PropertyField(Pinky_2, true);
                SerializedProperty Pinky_3 = serializedObject.FindProperty("Pinky_3");
                EditorGUILayout.PropertyField(Pinky_3, true);

                SerializedProperty Thumb_1 = serializedObject.FindProperty("Thumb_1");
                EditorGUILayout.PropertyField(Thumb_1, true);
                SerializedProperty Thumb_2 = serializedObject.FindProperty("Thumb_2");
                EditorGUILayout.PropertyField(Thumb_2, true);
                SerializedProperty Thumb_3 = serializedObject.FindProperty("Thumb_3");
                EditorGUILayout.PropertyField(Thumb_3, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }


            showPickObjects = EditorGUILayout.Foldout(showPickObjects, "Pick Objects", true);
            if (showPickObjects)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.BeginChangeCheck();

                SerializedProperty Beer = serializedObject.FindProperty("Beer");
                EditorGUILayout.PropertyField(Beer, true);

                SerializedProperty Glasses = serializedObject.FindProperty("Glasses");
                EditorGUILayout.PropertyField(Glasses, true);

                SerializedProperty Cigarette = serializedObject.FindProperty("Cigarette");
                EditorGUILayout.PropertyField(Cigarette, true);

                SerializedProperty Phone = serializedObject.FindProperty("Phone");
                EditorGUILayout.PropertyField(Phone, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }

            showCurrentIKDriverTargets = EditorGUILayout.Foldout(showCurrentIKDriverTargets, "IK Control Points", true);
            if (showCurrentIKDriverTargets)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.BeginChangeCheck();

                SerializedProperty headLookIKCP = serializedObject.FindProperty("headLookIKCP");
                EditorGUILayout.PropertyField(headLookIKCP, true);

                SerializedProperty targetLeftHandIK = serializedObject.FindProperty("targetLeftHandIK");
                EditorGUILayout.PropertyField(targetLeftHandIK, true);

                SerializedProperty targetRightHandIK = serializedObject.FindProperty("targetRightHandIK");
                EditorGUILayout.PropertyField(targetRightHandIK, true);

                SerializedProperty targetLeftFootIK = serializedObject.FindProperty("targetLeftFootIK");
                EditorGUILayout.PropertyField(targetLeftFootIK, true);

                SerializedProperty targetRightFootIK = serializedObject.FindProperty("targetRightFootIK");
                EditorGUILayout.PropertyField(targetRightFootIK, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }

            showCurrentIKTargetObjects = EditorGUILayout.Foldout(showCurrentIKTargetObjects, "Current IK Targets", true);
            if (showCurrentIKTargetObjects)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.BeginChangeCheck();

                SerializedProperty leftHandObj = serializedObject.FindProperty("leftHandObj");
                EditorGUILayout.PropertyField(leftHandObj, true);

                SerializedProperty rightHandObj = serializedObject.FindProperty("rightHandObj");
                EditorGUILayout.PropertyField(rightHandObj, true);

                SerializedProperty leftFootObj = serializedObject.FindProperty("leftFootObj");
                EditorGUILayout.PropertyField(leftFootObj, true);

                SerializedProperty rightFootObj = serializedObject.FindProperty("rightFootObj");
                EditorGUILayout.PropertyField(rightFootObj, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }

            showIKSteeringWheelTargets = EditorGUILayout.Foldout(showIKSteeringWheelTargets, "IK Targets", true);
            if (showIKSteeringWheelTargets)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.BeginChangeCheck();

                SerializedProperty lhswt_W = serializedObject.FindProperty("lhswt_W");
                EditorGUILayout.PropertyField(lhswt_W, true);

                SerializedProperty lhswt_NW = serializedObject.FindProperty("lhswt_NW");
                EditorGUILayout.PropertyField(lhswt_NW, true);

                SerializedProperty lhswt_N = serializedObject.FindProperty("lhswt_N");
                EditorGUILayout.PropertyField(lhswt_N, true);

                SerializedProperty lhswt_NE = serializedObject.FindProperty("lhswt_NE");
                EditorGUILayout.PropertyField(lhswt_NE, true);

                SerializedProperty lhswt_E = serializedObject.FindProperty("lhswt_E");
                EditorGUILayout.PropertyField(lhswt_E, true);

                SerializedProperty lhswt_SE = serializedObject.FindProperty("lhswt_SE");
                EditorGUILayout.PropertyField(lhswt_SE, true);

                SerializedProperty lhswt_S = serializedObject.FindProperty("lhswt_S");
                EditorGUILayout.PropertyField(lhswt_S, true);

                SerializedProperty lhswt_SW = serializedObject.FindProperty("lhswt_SW");
                EditorGUILayout.PropertyField(lhswt_SW, true);

                SerializedProperty rhswt_W = serializedObject.FindProperty("rhswt_W");
                EditorGUILayout.PropertyField(rhswt_W, true);

                SerializedProperty rhswt_NW = serializedObject.FindProperty("rhswt_NW");
                EditorGUILayout.PropertyField(rhswt_NW, true);

                SerializedProperty rhswt_N = serializedObject.FindProperty("rhswt_N");
                EditorGUILayout.PropertyField(rhswt_N, true);

                SerializedProperty rhswt_NE = serializedObject.FindProperty("rhswt_NE");
                EditorGUILayout.PropertyField(rhswt_NE, true);

                SerializedProperty rhswt_E = serializedObject.FindProperty("rhswt_E");
                EditorGUILayout.PropertyField(rhswt_E, true);

                SerializedProperty rhswt_SE = serializedObject.FindProperty("rhswt_SE");
                EditorGUILayout.PropertyField(rhswt_SE, true);

                SerializedProperty rhswt_S = serializedObject.FindProperty("rhswt_S");
                EditorGUILayout.PropertyField(rhswt_S, true);

                SerializedProperty rhswt_SW = serializedObject.FindProperty("rhswt_SW");
                EditorGUILayout.PropertyField(rhswt_SW, true);

                SerializedProperty handShift = serializedObject.FindProperty("handShift");
                EditorGUILayout.PropertyField(handShift, true);

                SerializedProperty footBrake = serializedObject.FindProperty("footBrake");
                EditorGUILayout.PropertyField(footBrake, true);

                SerializedProperty footGas = serializedObject.FindProperty("footGas");
                EditorGUILayout.PropertyField(footGas, true);

                SerializedProperty leftFootIdle = serializedObject.FindProperty("leftFootIdle");
                EditorGUILayout.PropertyField(leftFootIdle, true);

                SerializedProperty footClutch = serializedObject.FindProperty("footClutch");
                EditorGUILayout.PropertyField(footClutch, true);

                SerializedProperty rightFootIdle = serializedObject.FindProperty("rightFootIdle");
                EditorGUILayout.PropertyField(rightFootIdle, true);

                SerializedProperty returnShiftTarget = serializedObject.FindProperty("returnShiftTarget");
                EditorGUILayout.PropertyField(returnShiftTarget, true);

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }
    }
}