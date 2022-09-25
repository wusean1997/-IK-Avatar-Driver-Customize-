namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;

    public class AvatarDriver_RCCReference : MonoBehaviour
    {
        public float rcc_Horizontal;
        public float rcc_Vertical;

        public delegate void UpdateInput_RCC();
        public event UpdateInput_RCC OnUpdateInput_RCC;

        void Update()
        {
            OnUpdateInput_RCC();
        }
    }
}