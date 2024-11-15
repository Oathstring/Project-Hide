using UnityEngine;

namespace Oathstring
{
    public class Door : MonoBehaviour
    {
        private bool m_opened;
        private float m_zRot;
        private Collider m_collider;
        private Rigidbody m_doorRigidbody;

        [SerializeField] ItemType itemRequired;
        [SerializeField] bool locked = true;
        [Space]
        [SerializeField] float openRot = 100;
        [SerializeField] float closeRot = 0;
        [SerializeField] float rotationSmooth = 0.05f;
        [SerializeField] float fixOffset = 90;
        [SerializeField] float zFixOffset = 90;

        public bool Opened { get { return m_opened; } }
        public bool Locked { get { return locked; } }

        // Awake is called when the script object is initialised
        private void Awake()
        {
            m_collider = transform.GetChild(0).GetComponent<Collider>();
            m_doorRigidbody = transform.GetChild(0).GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        private void Update()
        {
            
        }

        private void FixedUpdate()
        {
            if (m_opened)
            {
                m_zRot = Mathf.Lerp(m_zRot, openRot, rotationSmooth);

                Vector3 doorRot = new(m_doorRigidbody.transform.rotation.x - fixOffset, m_doorRigidbody.transform.rotation.y, m_zRot - zFixOffset);
                //door.rotation = Quaternion.Euler(doorRot);
                m_doorRigidbody.MoveRotation(Quaternion.Euler(doorRot));

                m_collider.isTrigger = m_zRot < openRot - 1;
            }

            else
            {
                m_zRot = Mathf.Lerp(m_zRot, closeRot, rotationSmooth);

                Vector3 doorRot = new(m_doorRigidbody.transform.rotation.x - fixOffset, m_doorRigidbody.transform.rotation.y, m_zRot - zFixOffset);
                //door.rotation = Quaternion.Euler(doorRot);
                m_doorRigidbody.MoveRotation(Quaternion.Euler(doorRot));

                m_collider.isTrigger = m_zRot > closeRot + 1;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                m_opened = true; // automatic open door if enemy
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                m_opened = false; // automatic close door if enemy
            }
        }

        public virtual void Open(PlayerInventory playerInventory)
        {
            if (locked)
            {
                PlayerEventHandler eventHandler = playerInventory.GetComponent<PlayerEventHandler>();

                playerInventory.OpenInventory();
                eventHandler.interactionEvent = TryToUnlockDoor;
            }

            m_opened = !locked;
        }

        public virtual void Close()
        {
            m_opened = false;
        }

        public virtual void TryToUnlockDoor(PlayerSlot slotRequired, PlayerInventory playerInventory, PlayerEventHandler eventHandler)
        {
            if (slotRequired.Item.itemType == itemRequired)
            {
                locked = false;
                m_opened = !locked;

                slotRequired.Clear();
                eventHandler.interactionEvent = null;
                playerInventory.CloseInventory();

                TraumaTrigger traumaTrigger = GetComponent<TraumaTrigger>();

                if (traumaTrigger)
                {
                    Rigidbody playerRB = playerInventory.GetComponent<Rigidbody>();

                    traumaTrigger.FightAgainstTrauma(playerRB);
                }
            }

            else
            {
                print("Item not Match!");
            }
        }
    }
}
