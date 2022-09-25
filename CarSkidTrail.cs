namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;
    using System.Collections;

    public class CarSkidTrail : MonoBehaviour
    {
		public float persistTime;

		private IEnumerator Start()
        {
			while (true)
            {
				yield return null;
				if (transform.parent.parent == null)
                {
					Destroy(gameObject, persistTime);
				}
			}
		}
	}
}