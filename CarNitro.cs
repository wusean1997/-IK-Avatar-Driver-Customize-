namespace TurnTheGameOn.IKAvatarDriver
{
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(CarDriveSystem))]
    public class CarNitro : MonoBehaviour
    {
        public GameObject nitroEffectObject;
        private CarDriveSystem driveSystem;
        private bool canUpdateNitroAmount;
        public bool nitroOn { get; private set; }
        public float nitroAmount { get; private set; }

        [ContextMenu("NiroON")]
        public void NitroOn()
        {
            if (driveSystem._vehicleSettings.enableNitro && !nitroOn && nitroAmount > 2.0f)
            {
                nitroEffectObject.SetActive(true);
                driveSystem.topSpeed = driveSystem.vehicleSettings.topSpeed + driveSystem._vehicleSettings.nitroTopSpeed;
                driveSystem._vehicleSettings.fullTorqueOverAllWheels = driveSystem._vehicleSettings.fullTorqueOverAllWheels + driveSystem._vehicleSettings.nitroFullTorque;
                nitroOn = true;
            }
        }

        [ContextMenu("NiroOFF")]
        public void NitroOff()
        {
            if (nitroOn && driveSystem._vehicleSettings.enableNitro)
            {
                nitroEffectObject.SetActive(false);
                driveSystem.topSpeed = driveSystem.vehicleSettings.topSpeed - driveSystem._vehicleSettings.nitroTopSpeed;
                driveSystem._vehicleSettings.fullTorqueOverAllWheels = driveSystem._vehicleSettings.fullTorqueOverAllWheels - driveSystem._vehicleSettings.nitroFullTorque;
                nitroOn = false;
            }
        }

        void Start()
        {
            if (!driveSystem) driveSystem = GetComponent<CarDriveSystem>();
            if (driveSystem.vehicleSettings.enableNitro)
            {
                nitroAmount = driveSystem._vehicleSettings.nitroDuration;
                StartCoroutine("UpdateNitroAmount");
            }
        }

        void OnDisable()
        {
            canUpdateNitroAmount = false;
            StopAllCoroutines();
        }

        IEnumerator UpdateNitroAmount()
        {
            canUpdateNitroAmount = true;
            while (canUpdateNitroAmount)
            {
                if (!nitroOn && nitroAmount < driveSystem._vehicleSettings.nitroDuration)
                {
                    nitroAmount += driveSystem._vehicleSettings.nitroRefillRate * Time.deltaTime;
                    if (nitroAmount > driveSystem._vehicleSettings.nitroDuration) nitroAmount = driveSystem._vehicleSettings.nitroDuration;
                }
                else
                {
                    nitroAmount -= driveSystem._vehicleSettings.nitroSpendRate * Time.deltaTime;
                    if (nitroAmount < 0)
                    {
                        nitroAmount = 0;
                        NitroOff();
                    }
                }
                yield return null;
            }
        }
    }
}