namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MobileUIJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public enum AxisOption
        {
            Both, // Use both
            OnlyHorizontal, // Only horizontal
            OnlyVertical // Only vertical
        }

        public int MovementRange = 100;
        public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
        public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
        public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input
        Vector3 m_StartPos;
        bool m_UseX; // Toggle for using the x axis
        bool m_UseY; // Toggle for using the Y axis
        MobileInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
        MobileInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

        void Start()
        {
            m_StartPos = transform.position;
            CreateVirtualAxes();
        }

        void UpdateVirtualAxes(Vector3 value)
        {
            var delta = m_StartPos - value;
            delta.y = -delta.y;
            delta /= MovementRange;
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Update(-delta.x);
            }
            if (m_UseY)
            {
                m_VerticalVirtualAxis.Update(delta.y);
            }
        }

        void CreateVirtualAxes()
        {
            m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
            m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

            if (m_UseX)
            {
                if (!MobileInputManager.AxisExists(horizontalAxisName))
                {
                    m_HorizontalVirtualAxis = new MobileInputManager.VirtualAxis(horizontalAxisName);
                    MobileInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
                }
                else
                {
                    m_HorizontalVirtualAxis = MobileInputManager.VirtualAxisReference(horizontalAxisName);
                }
            }
            if (m_UseY)
            {
                if (!MobileInputManager.AxisExists(verticalAxisName))
                {
                    m_VerticalVirtualAxis = new MobileInputManager.VirtualAxis(verticalAxisName);
                    MobileInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
                }
                else
                {
                    m_VerticalVirtualAxis = MobileInputManager.VirtualAxisReference(verticalAxisName);
                }
            }
        }

        public void OnDrag(PointerEventData data)
        {
            Vector3 newPos = Vector3.zero;
            if (m_UseX)
            {
                int delta = (int)(data.position.x - m_StartPos.x);
                newPos.x = delta;
            }
            if (m_UseY)
            {
                int delta = (int)(data.position.y - m_StartPos.y);
                newPos.y = delta;
            }
            transform.position = Vector3.ClampMagnitude(new Vector3(newPos.x, newPos.y, newPos.z), MovementRange) + m_StartPos;
            UpdateVirtualAxes(transform.position);
        }

        public void OnPointerUp(PointerEventData data)
        {
            transform.position = m_StartPos;
            UpdateVirtualAxes(m_StartPos);
        }

        public void OnPointerDown(PointerEventData data) { }

        void OnDisable()
        {
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Remove();
            }
            if (m_UseY)
            {
                m_VerticalVirtualAxis.Remove();
            }
        }
    }
}