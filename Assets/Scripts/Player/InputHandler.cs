using UnityEngine;
using UnityEngine.InputSystem;

namespace Oathstring
{
    public class InputHandler : MonoBehaviour
    {
        private InputAction m_moveAction;
        private InputAction m_lookAction;
        private InputAction m_crouchAction;
        private InputAction m_sprintAction;
        private InputAction m_pointAction;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool CrouchIsOn { get; private set; }
        public bool Sprint { get; private set; }
        public Vector2 MousePosition { get; private set; }

        [Header("Input Asset")]
        [SerializeField] InputActionAsset inputActions;

        [Header("Input Action Map Ref")]
        [SerializeField] string actionMapName = "Player";
        [SerializeField] string actionUIMapName = "UI";

        [Header("Input Action Ref")]
        [SerializeField] string move = "Move";
        [SerializeField] string look = "Look";
        [SerializeField] string crouch = "Crouch";
        [SerializeField] string sprint = "Sprint";
        [Space]
        [SerializeField] string mousePoint = "Point";

        public static InputHandler Instance { get; private set; }

        // Awake is called when the script object is initialised
        private void Awake()
        {
            if (!Instance) Instance = this;

            m_moveAction = inputActions.FindActionMap(actionMapName).FindAction(move);
            m_lookAction = inputActions.FindActionMap(actionMapName).FindAction(look);
            m_crouchAction = inputActions.FindActionMap(actionMapName).FindAction(crouch);
            m_sprintAction = inputActions.FindActionMap(actionMapName).FindAction(sprint);
            m_pointAction = inputActions.FindActionMap(actionUIMapName).FindAction(mousePoint);


            RegisterInputActions();
        }

        private void RegisterInputActions()
        {
            m_moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
            m_moveAction.canceled += context => MoveInput = Vector2.zero;

            m_lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
            m_lookAction.canceled += context => LookInput = Vector2.zero;

            m_crouchAction.performed += context => CrouchIsOn = !CrouchIsOn;

            m_sprintAction.performed += context => Sprint = true;
            m_sprintAction.canceled += context => Sprint = false;

            m_pointAction.performed += context => MousePosition = context.ReadValue<Vector2>();
        }

        // Update is called once per frame
        private void Update()
        {

        }

        private void OnEnable()
        {
            m_moveAction.Enable();
            m_lookAction.Enable();
            m_crouchAction.Enable();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            m_moveAction.Disable();
            m_lookAction.Disable();
            m_crouchAction.Disable();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
