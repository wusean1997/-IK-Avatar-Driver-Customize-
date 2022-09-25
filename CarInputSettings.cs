namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "CarInputSettings", menuName = "TurnTheGameOn/IK Avatar Driver/CarInputSettings")]
    public class CarInputSettings : ScriptableObject
    {
        public CarUIManager mobileCanvas;
        public CarUIManager defaultCanvas;
        public UIType uIType;
        public MobileSteeringType mobileSteeringType;
        public CarPlayerInputInfo inputAxes;
    }
}