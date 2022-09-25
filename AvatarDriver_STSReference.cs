namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;

    public class AvatarDriver_STSReference : MonoBehaviour
    {
        public float sts_Horizontal;
        public float sts_Vertical;

        public delegate void UpdateInput_STS();
        public event UpdateInput_STS OnUpdateInput_STS;

        void Update()
        {
            OnUpdateInput_STS();
        }
    }
}