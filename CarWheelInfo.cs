namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;

    [System.Serializable]
    public struct CarWheelInfo
    {
        public string name;
        public GameObject mesh;
        public Transform meshTransform;
        public WheelCollider collider;
        public CarWheelSkidEffect wheel;
        public AudioSource audioSource;
        public ParticleSystem wheelSmokeParticleSystem;
    }
}