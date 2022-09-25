namespace TurnTheGameOn.IKAvatarDriver
{
	using UnityEngine;

	[RequireComponent(typeof(CarDriveSystem))]
	public class CarLights : MonoBehaviour
	{		
		public GameObject brakeLightsObject;
		public GameObject reverseLightsObject;
		private CarDriveSystem driveSystem;

        void OnEnable ()
		{
			if (!driveSystem) driveSystem = GetComponent <CarDriveSystem>();
            driveSystem.OnSetIsBraking += OnSetIsBraking;
            driveSystem.OnSetIsNotBraking += OnSetIsNotBraking;
            driveSystem.OnSetIsReversing += OnSetIsReversing;
            driveSystem.OnSetIsNotReversing += OnSetIsNotReversing;
		}

		void OnDisable ()
		{
            driveSystem.OnSetIsBraking -= OnSetIsBraking;
            driveSystem.OnSetIsNotBraking -= OnSetIsNotBraking;
            driveSystem.OnSetIsReversing -= OnSetIsReversing;
            driveSystem.OnSetIsNotReversing -= OnSetIsNotReversing;
		}

        void OnSetIsBraking ()
		{
			TurnOnBrakeLights();
		}

		void OnSetIsNotBraking ()
		{
			TurnOffBrakeLights();
		}

		void OnSetIsReversing ()
		{
			TurnOnReverseLights();
		}

		void OnSetIsNotReversing ()
		{
			TurnOffReverseLights();
		}

		void TurnOff()
		{
			TurnOffReverseLights ();
			TurnOffBrakeLights ();
		}

		void TurnOnBrakeLights ()
		{
			TurnOffReverseLights ();
			brakeLightsObject.SetActive(true);
		}

		void TurnOffBrakeLights ()
		{
			brakeLightsObject.SetActive(false);
		}

		void TurnOnReverseLights ()
		{
			TurnOffBrakeLights ();
			reverseLightsObject.SetActive(true);
		}

		void TurnOffReverseLights ()
		{
			reverseLightsObject.SetActive(false);
		}
		
	}
}