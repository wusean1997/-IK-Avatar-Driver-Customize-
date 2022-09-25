namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MobileAxisTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // works in a pair with another axis touch button // (typically with one having -1 and one having 1 axisValues)
        public string axisName = "Horizontal"; // The name of the axis
        public float axisValue = 1; // The axis that the value has
        public float responseSpeed = 3; // The speed at which the axis touch button responds
        public float returnToCentreSpeed = 3; // The speed at which the button will return to its centre
        MobileInputManager.VirtualAxis m_Axis; // A reference to the virtual axis as it is in the cross platform input
        bool buttonPressed;

        void OnEnable()
        {
            if (!MobileInputManager.AxisExists(axisName))
            {
                // if the axis doesnt exist create a new one in cross platform input
                m_Axis = new MobileInputManager.VirtualAxis(axisName);
                MobileInputManager.RegisterVirtualAxis(m_Axis);
            }
            else
            {
                m_Axis = MobileInputManager.VirtualAxisReference(axisName);
            }
            FindPairedButton();
        }

        void FindPairedButton()
        {
            var otherAxisButtons = FindObjectsOfType(typeof(MobileAxisTouchButton)) as MobileAxisTouchButton[];
            if (otherAxisButtons != null)
            {
                for (int i = 0; i < otherAxisButtons.Length; i++)
                {
                    if (otherAxisButtons[i].axisName == axisName && otherAxisButtons[i] != this)
                    {
                        //m_PairedWith = otherAxisButtons[i];
                    }
                }
            }
        }

        void Update()
        {
            if (buttonPressed)
            {
                m_Axis.Update(Mathf.MoveTowards(m_Axis.GetValue, axisValue, responseSpeed * Time.deltaTime));
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            buttonPressed = true;
        }

        public void OnPointerUp(PointerEventData data)
        {
            buttonPressed = false;
            m_Axis.Update(Mathf.MoveTowards(m_Axis.GetValue, 0, responseSpeed * Time.deltaTime));
        }
    }
}