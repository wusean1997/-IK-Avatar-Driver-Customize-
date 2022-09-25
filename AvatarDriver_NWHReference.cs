namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;

    public class AvatarDriver_NWHReference : MonoBehaviour
    {

        private void Awake()
        {
        }

        public float nwh_Horizontal
        {
            get
            {
                return 0;
            }
        }
        public float nwh_Vertical
        {
            get
            {
                return 0;
            }
        }
        public string nwh_gear
        {
            get
            {
                return "";
            }
        }

        public delegate void UpdateInput_NWH();
        public event UpdateInput_NWH OnUpdateInput_NWH;

        void Update()
        {
            OnUpdateInput_NWH();
        }
    }
}