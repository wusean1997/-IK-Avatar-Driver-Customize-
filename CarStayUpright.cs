namespace TurnTheGameOn.IKAvatarDriver
{
	using UnityEngine;
    using System.Collections;
    [RequireComponent(typeof(Rigidbody))]
	public class CarStayUpright : MonoBehaviour
	{
		public float waitTime = 3f;
		public float velocityThreshold = 1f;
        public float updateFrequency = 0.5f;
		private float lastOkTime;
		private Rigidbody rigidbodyCached;
        private Transform transformCached;

        private void OnEnable()
        {
            StartCoroutine(CheckOrientation());
        }

        private void OnDisable()
        {
            StopCoroutine(CheckOrientation());
        }

        IEnumerator CheckOrientation()
        {
            rigidbodyCached = GetComponent<Rigidbody>();
            transformCached = transform;
            while (true)
            {
                yield return new WaitForSeconds(updateFrequency);
                if (transformCached.up.y > 0f || rigidbodyCached.velocity.magnitude > velocityThreshold)
                {
                    lastOkTime = Time.time;
                }
                if (Time.time > lastOkTime + waitTime)
                {
                    RightCar();
                }
            }
        }
			
		void RightCar()
        {
            transformCached.position += Vector3.up;
            transformCached.rotation = Quaternion.LookRotation(transformCached.forward);
		}
	}
}