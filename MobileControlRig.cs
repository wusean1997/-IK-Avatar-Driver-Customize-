namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MobileControlRig : MonoBehaviour
    {
        public CarDriveSystem vehicleController;
        public GameObject turnLeftButton;
        public GameObject turnRightButton;
        public GameObject steeringJoystick;
        public GameObject tiltInput;
        public GameObject steeringWheel;
        public GameObject shiftUpButton;
        public GameObject shiftDownButton;
        public EventTrigger cameraSwitchTrigger;

        void Start()
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(true);
            }

            steeringJoystick.SetActive(false);
            turnLeftButton.SetActive(false);
            turnRightButton.SetActive(false);
            tiltInput.SetActive(false);
            steeringWheel.SetActive(false);

            MobileSteeringType mobileSteeringType = vehicleController.GetComponent<CarPlayerInput>().playerInputSettings.mobileSteeringType;
            switch (mobileSteeringType)
            {
                case MobileSteeringType.UIButtons:          // Arrow Button Steering
                    turnLeftButton.SetActive(true);
                    turnRightButton.SetActive(true);
                    break;
                case MobileSteeringType.Tilt:               // Tilt Steering
                    tiltInput.SetActive(true);
                    break;
                case MobileSteeringType.UIJoystick:         // Joystick Steering
                    steeringJoystick.SetActive(true);
                    break;
                case MobileSteeringType.UISteeringWheel:    // Steering Wheel
                    steeringWheel.SetActive(true);
                    break;
            }

            shiftUpButton.SetActive(vehicleController.vehicleSettings.manual);
            shiftDownButton.SetActive(vehicleController.vehicleSettings.manual);

            CarCamera carCamera = FindObjectOfType<CarCamera>();
            cameraSwitchTrigger.triggers.Add(carCamera.switchCameraEvent);
        }

    }
}