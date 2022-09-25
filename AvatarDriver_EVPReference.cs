namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;

    public class AvatarDriver_EVPReference : MonoBehaviour
    {
        public int evp_Gear;

        public delegate void UpdateInput_EVP();
        public event UpdateInput_EVP OnUpdateInput_EVP;

        void Update()
        {
            OnUpdateInput_EVP();
        }
    }
}