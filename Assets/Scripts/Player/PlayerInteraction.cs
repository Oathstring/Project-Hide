using UnityEngine;
using UnityEngine.UI;

namespace Oathstring
{
    public class PlayerInteraction : MonoBehaviour
    {
        private PlayerInventory m_playerInventory;
        private PlayerController m_playerController;

        [Header("Settings")]
        [SerializeField] LayerMask interactlayer;
        [SerializeField] float interactLength = 5;
        [SerializeField] Image crosshairImg;

        // Awake is called when the script object is initialised
        private void Awake()
        {
            m_playerInventory = GetComponentInParent<PlayerInventory>();
            m_playerController = GetComponentInParent<PlayerController>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (HitInfo.collider && !m_playerInventory.Opened && !MenuManager.Instance.Paused)
            {
                if (HitInfo.collider.CompareTag("Item"))
                {
                    crosshairImg.color = Color.blue;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        ItemPickup itemPickup = HitInfo.collider.GetComponentInParent<ItemPickup>();

                        m_playerInventory.AddItem(itemPickup);
                    }
                }

                else if (HitInfo.collider.CompareTag("Door"))
                {
                    crosshairImg.color = Color.green;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Door door = HitInfo.collider.GetComponentInParent<Door>();

                        if (!door.Opened)
                        {
                            door.Open(m_playerInventory);
                        }

                        else
                        {
                            door.Close();
                        }
                    }
                }

                else if (HitInfo.collider.CompareTag("Hideable") && !m_playerController.Hide)
                {
                    crosshairImg.color = Color.magenta;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Hideable hideable = HitInfo.collider.GetComponentInParent<Hideable>();
                        hideable.HidePlayer(m_playerController, m_playerController.PlayerRigidBody);
                    }
                }

                else if(HitInfo.collider.CompareTag("Rest Bed") && !m_playerController.Hide)
                {
                    crosshairImg.color = Color.blue;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        RestTrigger restTrigger = HitInfo.collider.GetComponentInParent<RestTrigger>();
                        restTrigger.OpenEndScreenRest();
                    }
                }

                else
                {
                    crosshairImg.color = Color.white;
                }
            }

            else if(!MenuManager.Instance.Paused)
            {
                crosshairImg.color = Color.white;
            }

            if (MenuManager.Instance.Paused) crosshairImg.color = Color.clear;
        }

        private RaycastHit HitInfo
        {
            get
            {
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, interactLength, interactlayer))
                {
                    return hitInfo;
                }

                else
                {
                    return new();
                }
            }
        }
    }
}
