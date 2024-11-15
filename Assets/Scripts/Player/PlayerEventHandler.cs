using UnityEngine;
using UnityEngine.UI;

namespace Oathstring
{
    public class PlayerEventHandler : MonoBehaviour
    {
        public delegate void InventorySlotEvent();
        public InventorySlotEvent inventorySlotEvent;

        public delegate void InteractionEvent(PlayerSlot slotRequired, PlayerInventory playerInventory, PlayerEventHandler eventHandler);
        public InteractionEvent interactionEvent;

        private PlayerInventory m_playerInventory;

        // Awake is called when the script object is initialised
        private void Awake()
        {
            m_playerInventory = GetComponent<PlayerInventory>();
        }

        // Update is called once per frame
        private void Update()
        {
            inventorySlotEvent?.Invoke();
        }
    }
}
