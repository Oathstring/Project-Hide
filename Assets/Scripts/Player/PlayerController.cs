using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Oathstring
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private Transform m_playerRoot;
        private Rigidbody m_rigidbody;
        private PlayerCamera m_playerCamera;
        private PlayerInventory m_playerInventory;
        private readonly RaycastHit[] m_groundBuffer = new RaycastHit[10];
        private readonly Collider[] m_groundOverlapBuffer = new Collider[10];
        private CapsuleCollider m_capsuleCollder;
        private Hideable m_hideable;
        //private Animator m_animator; // full body script

        private float m_speed = 10;
        private float m_currentMoveSensitivity = 0.3f;
        private readonly float m_moveSensitivityMultiplier = 2;
        private bool m_hide;
        private bool m_inCutscene = true;
        private bool m_isCaptured = false;

        [Header("Settings")]
        [SerializeField] float walkSpeed = 10;
        [SerializeField] float sprintSpeed = 12;
        [SerializeField] [Range(0.3f, 1)] float moveSensitivity = 0.3f;
        [Space]
        [SerializeField] float groundCheck = 1.1f;
        [SerializeField] LayerMask groundLayer;
        [Space]
        [SerializeField] float jumpHeight = 10;
        [Header("Animation Settings")]
        [SerializeField] float animDampTime = .2f;

        public bool IsPlaying
        {
            get { return !m_inCutscene; }
        }

        public bool Enabled
        {
            get { return InputHandler.Instance.enabled; }
        }

        // Awake is called when the script object is initialised
        private void Awake()
        {
            m_playerRoot = transform.parent;
            m_rigidbody = GetComponent<Rigidbody>();
            m_playerCamera = m_playerRoot.GetComponentInChildren<PlayerCamera>();
            m_playerInventory = GetComponent<PlayerInventory>();
            m_capsuleCollder = GetComponentInChildren<CapsuleCollider>();
            //m_animator = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!m_inCutscene && !MenuManager.Instance.Paused)
            {
                HandleSpeed();

                InputHandler.Instance.enabled = !m_playerInventory.Opened;

                if (Input.GetButtonDown("Jump") && Hide)
                {
                    m_hideable.UnHidePlayer(this, m_rigidbody);
                }

                m_rigidbody.isKinematic = Hide;
                m_capsuleCollder.isTrigger = Hide;
            }

            else if (IsFallen && m_isCaptured) m_capsuleCollder.material = null;

            if (!m_playerInventory.Opened && MenuManager.Instance.Paused)
            {
                Disable();
            }

            else if (!m_playerInventory.Opened && !m_inCutscene)
            {
                Enable();
            }
        }

        private void HandleSpeed()
        {
            m_speed = InputHandler.Instance.Sprint ? sprintSpeed : walkSpeed;
            m_currentMoveSensitivity = InputHandler.Instance.Sprint ? moveSensitivity * m_moveSensitivityMultiplier : moveSensitivity;
        }

        private void FixedUpdate()
        {
            if (!Hide)
            {
                HandleMovement();
            }
            //HandleAnimation(); // full body script
        }

        private void HandleMovement()
        {
            if (!m_inCutscene && !MenuManager.Instance.Paused)
            {
                float horizontal = InputHandler.Instance.MoveInput.x;
                float vertical = InputHandler.Instance.MoveInput.y;

                Vector3 moveDir = (transform.right * horizontal + transform.forward * vertical).normalized;

                if (IsGrounded)
                {
                    m_rigidbody.linearVelocity = Vector3.MoveTowards(m_rigidbody.linearVelocity, moveDir * m_speed, m_currentMoveSensitivity);
                }
            }
        }

        // full body script
        /*private void HandleAnimation()
        {
            m_animator.SetFloat("Vertical Move", InputHandler.Instance.MoveInput.y, animDampTime, Time.fixedDeltaTime);
            m_animator.SetFloat("Horizontal Move", InputHandler.Instance.MoveInput.x, animDampTime, Time.fixedDeltaTime);
            m_animator.SetBool("Sprint", InputHandler.Instance.Sprint);
        }*/

        public void SetHide(bool value, Hideable hideable)
        {
            m_hide = value;
            m_hideable = hideable;
        }

        public void SetInCutscene(bool value)
        {
            m_inCutscene = value;
        }

        public void Enable()
        {
            InputHandler.Instance.enabled = true;
        }

        public void Disable()
        {
            m_rigidbody.linearVelocity = new();
            InputHandler.Instance.enabled = false;
        }

        public void SetCapture(bool value)
        {
            m_isCaptured = value;
        }

        public Rigidbody PlayerRigidBody
        {
            get { return m_rigidbody; }
        }

        public bool Hide
        {
            get { return m_hide; }
        }

        public bool IsGrounded
        {
            get
            {
                Vector3 centerOfSphere1 = transform.position + Vector3.up * (m_capsuleCollder.radius + Physics.defaultContactOffset);
                Vector3 centerOfSphere2 = transform.position + Vector3.up * (m_capsuleCollder.height - m_capsuleCollder.radius + Physics.defaultContactOffset);

                int ground = Physics.CapsuleCastNonAlloc(centerOfSphere1, centerOfSphere2, m_capsuleCollder.radius, -transform.up, m_groundBuffer, groundCheck, groundLayer);

                return ground > 0;
            }
        }

        public bool IsFallen
        {
            get
            {
                Transform groundCheck = m_playerCamera.Cam.transform;
                float checkRadius = .5f;

                int ground = Physics.OverlapSphereNonAlloc(groundCheck.position, checkRadius, m_groundOverlapBuffer, groundLayer);

                return ground > 0; 
            }
        }
    }
}
