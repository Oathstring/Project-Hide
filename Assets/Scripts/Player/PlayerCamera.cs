using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Oathstring
{
    public class PlayerCamera : MonoBehaviour
    {
        private Camera m_camera;
        private PlayerController m_playerController;
        private Rigidbody m_rigidbody;

        private float m_yRot;
        private float m_xRot;

        [Tooltip("Camera Variables")]
        [SerializeField] float sensitivity = 1;
        [SerializeField] float maxXRot = 75;
        [SerializeField] float minXRot = -75;
        /*[Space]
        [SerializeField] Vector3 normalCameraPos;
        [SerializeField] Vector3 runCameraOffset;
        [SerializeField] float smoothTime = .01f;*/
        [Space]
        [SerializeField] Transform headLook;
        [SerializeField] float maxXHeadRot = 75;
        [SerializeField] float minXHeadRot = -75;

        public Camera Cam
        {
            get { return m_camera; }
        }

        // Awake is called when the script object is initialised
        private void Awake()
        {
            m_camera = GetComponentInChildren<Camera>();
            m_playerController = GetComponent<PlayerController>();
            m_rigidbody = GetComponent<Rigidbody>();

            //normalCameraPos = m_camera.transform.localPosition;
            m_xRot = m_camera.transform.localEulerAngles.x;
            m_yRot = transform.localEulerAngles.y;
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            if(m_playerController.IsPlaying)
            {
                PlayerRotate();
                CameraLook();
            }

            else
            {
                m_xRot = 0;
                m_yRot = transform.localEulerAngles.y;
            }

            // full body script
            /*Vector3 cameraRun = Vector3.Lerp(m_camera.transform.localPosition, runCameraOffset, smoothTime);
            Vector3 cameraWalk = Vector3.Lerp(m_camera.transform.localPosition, normalCameraPos, smoothTime);

            *//*Vector3 cameraRun = runCameraOffset;
            Vector3 cameraWalk = normalCameraPos;*//*

            m_camera.transform.localPosition = InputHandler.Instance.Sprint && InputHandler.Instance.MoveInput != Vector2.zero ? 
                cameraRun : cameraWalk;*/

        }

        private void CameraLook()
        {
            // old input system 
            /*m_yRot += Input.GetAxis("Mouse X");
            m_xRot -= Input.GetAxis("Mouse Y");*/

            // new input system
            m_yRot += InputHandler.Instance.LookInput.x * sensitivity;
            m_xRot -= InputHandler.Instance.LookInput.y * sensitivity;

            m_xRot = Mathf.Clamp(m_xRot, minXRot, maxXRot); // limit X axis

            Vector3 cameraRot = new(m_xRot, 0, 0); // get value from input rotation

            m_camera.transform.localRotation = Quaternion.Euler(cameraRot); // apply the rotation

            // full body script
            /*float xHeadRot = Mathf.Clamp(m_xRot, minXHeadRot, maxXHeadRot);
            Vector3 headRot = new(xHeadRot, 0, 0);
            headLook.transform.localRotation = Quaternion.Euler(headRot);*/
        }

        private void PlayerRotate()
        {
            Vector3 playerRot = new(0, m_yRot, 0); // get value from input rotation
            Quaternion targetRot = Quaternion.Euler(playerRot); // convert vector 3 to quaternion
            m_rigidbody.MoveRotation(targetRot); // apply the rotation
        }
    }
}
