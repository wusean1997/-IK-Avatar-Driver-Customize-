namespace TurnTheGameOn.IKAvatarDriver
{
	using UnityEngine;
	using UnityEngine.UI;

	public class CarUIManager : MonoBehaviour
    {
        public CarDriveSystem vehicleController;
		public CarNitro vehicleControllerNitro { get; private set; }
        public CarUIManager dashboardUI { get; private set; }
        public bool primaryUIController;
		public CarUIAnalogNeedle speedometerNeedle;
		public CarUIAnalogNeedle tachometerNeedle;
		public Slider nitroSlider;
		public Text distanceTypeText;
		public Text speedText;
		public Text gearText;
		float gearFactor;
		public GameObject rearViewMirrorObject;

        public void RegisterDriveSystem(CarDriveSystem _carDriveSystem)
        {
            vehicleController = _carDriveSystem;
        }

		void Start ()
		{
			if (primaryUIController) 
			{
				vehicleController.vehicleUI = this;
                vehicleControllerNitro = vehicleController.GetComponent<CarNitro>();
                dashboardUI = vehicleController.GetComponentInChildren<CarUIManager>();
				switch (vehicleController.vehicleSettings.speedType)
				{
				case SpeedType.KPH:
					distanceTypeText.text = "kph";
					if (dashboardUI) dashboardUI.distanceTypeText.text = "kph";
					break;
				case SpeedType.MPH:
					distanceTypeText.text = "mph";
					if (dashboardUI) dashboardUI.distanceTypeText.text = "mph";
					break;
				}
			}
			else
			{
				enabled = false;
			}
		}

        void Update()
        {
            // speedometer needle
            if (dashboardUI) dashboardUI.speedometerNeedle.SetValue(vehicleController.currentSpeed);
            speedometerNeedle.SetValue(vehicleController.currentSpeed);
            // tachometer needle
            gearFactor = vehicleController.gearFactor;
            if (gearFactor < vehicleController.vehicleSettings.minRPM) gearFactor = vehicleController.vehicleSettings.minRPM;
            tachometerNeedle.SetValue(gearFactor * vehicleController.vehicleSettings.RPMLimit);
            if (dashboardUI) dashboardUI.tachometerNeedle.SetValue(gearFactor * vehicleController.vehicleSettings.RPMLimit);
            // speed text
            speedText.text = vehicleController.currentSpeed.ToString("F0");
            if (dashboardUI) dashboardUI.speedText.text = vehicleController.currentSpeed.ToString("F0");

            if (vehicleControllerNitro) // nitro bar
            {
                nitroSlider.value = vehicleControllerNitro.nitroAmount;
                if (dashboardUI) dashboardUI.nitroSlider.value = vehicleControllerNitro.nitroAmount;
            }

            if (gearText && !vehicleController.vehicleSettings.manual)
            {
                if (vehicleController.BrakeInput > 0f && vehicleController.isReversing && vehicleController.currentSpeed >= 1)
                {
                    gearText.text = "R";
                    if (dashboardUI) dashboardUI.gearText.text = "R";
                }
                else if (vehicleController.currentGear == 0)
                {
                    gearText.text = "N";
                    if (dashboardUI) dashboardUI.gearText.text = "N";
                }
                if (vehicleController.AccelInput > 0f)
                {
                    gearText.text = (vehicleController.currentGear + 1f).ToString();
                    if (dashboardUI) dashboardUI.gearText.text = (vehicleController.currentGear + 1f).ToString();
                }
            }
            else if (gearText)
            {
                gearText.text = vehicleController.currentGear == 0 ? "N" : vehicleController.currentGear == -1 ? "R" : (vehicleController.currentGear).ToString();
                if (dashboardUI) dashboardUI.gearText.text = vehicleController.currentGear == 0 ? "N" : vehicleController.currentGear == -1 ? "R" : (vehicleController.currentGear).ToString();
            }

        }

		public void ToggleRearViewMirror(bool _setActive)
		{
			rearViewMirrorObject.SetActive(_setActive);
		}

	}
}