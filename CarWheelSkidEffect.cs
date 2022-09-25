namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;

    public class CarWheelSkidEffect : MonoBehaviour
    {
        public bool isSkidding { get; private set; }
        public bool isPlayingAudio { get; private set; }
        public bool canPlayAudio;
        private Transform skidTrail;
        public Transform skidTrailParent;
        private Vector3 previousRotationEuler;
        private Vector3 colliderOffset;
        private ParticleSystem.EmissionModule em;
        private string currentSkidTrailPrefabName;

        public void WheelSkid (CarDriveSystem driveSystem, WheelCollider wheelCollider, Transform skidPrefab, ParticleSystem skidParticles, float skidAmount, AudioSource audioSource, CarAudio carAudio)
        {
            if (skidParticles)
            {
                skidParticles.transform.position = transform.position - transform.up * wheelCollider.radius;
                em = skidParticles.emission;
                em.enabled = true;
                skidParticles.Emit (1);
                if (audioSource)
                {
                    if (audioSource.enabled && !isPlayingAudio)
                    {
                        if (!carAudio.isPlayingSkidAudio) audioSource.Play ();
                        carAudio.isPlayingSkidAudio = true;
                        isPlayingAudio = true;
                    }
                }
            }
            if (!isSkidding)
            {
                isSkidding = true;
                SkidOn(wheelCollider, skidPrefab);
            }
            else if (skidTrail != null && isSkidding)
            {
                if (previousRotationEuler != skidTrail.transform.eulerAngles || skidTrail.name != skidPrefab.name)
                {
                   SkidOn(wheelCollider, skidPrefab); 
                }
            }
            previousRotationEuler = skidTrail.transform.eulerAngles;
        }
        
        void SkidOn(WheelCollider wheelCollider, Transform skidPrefab)
        {
            if (skidTrail != null)
            {
                skidTrail.parent = skidTrailParent;
                skidTrail = null;
            }
            skidTrail = Instantiate(skidPrefab);
            skidTrail.parent = transform;
            if (skidTrail)
            {
                skidTrail.localPosition = -Vector3.up * wheelCollider.radius;
                colliderOffset = wheelCollider.center * 2;
                skidTrail.localPosition += colliderOffset;
            }
        }

        public void WheelSkid_Stop(AudioSource audioSource, CarAudio carAudio, ParticleSystem skidParticles)
        {
            if (isSkidding)
            {
                em = skidParticles.emission;
                em.enabled = false;
                audioSource.Stop();
                carAudio.isPlayingSkidAudio = false;
                isPlayingAudio = false;
                isSkidding = false;
                skidTrail.parent = skidTrailParent;
                skidTrail = null;
            }
        }

    }
}