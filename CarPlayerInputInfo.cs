namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;

    [System.Serializable]
    public struct CarPlayerInputInfo
    {
        [Header("Axes")]
        public string steering;
        public string throttle;
        public string brake;
        public string handBrake;
        public string shiftUp;
        public string shiftDown;
        public bool invertFootBrake;
        public bool releaseBrakeToReverse;
        [Header("KeyCode")]
        public KeyCode lookBackKey;
        public KeyCode cycleCameraKey;
        public KeyCode nitroKey;
    }
}