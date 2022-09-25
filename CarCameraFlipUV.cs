namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;
    public class CarCameraFlipUV : MonoBehaviour
    {
		public Camera _camera;
        private Matrix4x4 matrix;

		void OnPreCull()
        {
			_camera.ResetWorldToCameraMatrix ();
			_camera.ResetProjectionMatrix ();
            matrix = _camera.projectionMatrix;
            matrix *= Matrix4x4.Scale (new Vector3(-1, 1, 1));
			_camera.projectionMatrix = matrix;
		}

		void OnPreRender()
        {
			GL.invertCulling = true;
		}

		void OnPostRender()
        {
			GL.invertCulling = false;
		}

	}
}