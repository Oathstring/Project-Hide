using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Oathstring
{
    public class PlayerInventory : MonoBehaviour
    {
        private Transform[] m_slotImages;
        private PlayerSlot[] m_playerSlots;
        private bool m_inventoryOpened;

        private PlayerEventHandler playerEventHandler;
        private PlayerCamera m_playerCamera;
        private PlayerController m_playerController;

        private readonly List<Image> m_selectedSlotImages = new();
        private readonly List<Item> m_firstSelectedItems = new();
        private readonly List<Item> m_endSelectedItems = new();

        [Header("Settings")]
        [SerializeField] CanvasGroup inventory;
        [SerializeField] Transform slotImageHolder;
        [SerializeField] Image dragItemImage;
        [SerializeField] Vector3 dropPositionOffset;

        public bool Opened
        {
            get { return m_inventoryOpened; }
        }

        // Awake is called when the script object is initialised
        private void Awake()
        {
            playerEventHandler = GetComponent<PlayerEventHandler>();
            playerEventHandler.inventorySlotEvent = OnNothingSelected;

            m_playerCamera = GetComponent<PlayerCamera>();
            m_playerController = GetComponent<PlayerController>();

            // Init Slot UI
            m_slotImages = new Transform[slotImageHolder.childCount];

            for (int i = 0; i < slotImageHolder.childCount; i++)
            {
                m_slotImages[i] = slotImageHolder.GetChild(i);
            }

            // Init Slot Data
            m_playerSlots = new PlayerSlot[slotImageHolder.childCount];

            for (int i = 0; i < m_playerSlots.Length; i++)
            {
                m_playerSlots[i] = new PlayerSlot(); // Ensure each slot is instantiated.

                string slotName = "Slot";
                m_playerSlots[i].TryRegisterSlot(slotName, i);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            m_inventoryOpened = Input.GetKeyDown(KeyCode.Tab) && m_playerController.IsPlaying ? m_inventoryOpened = !m_inventoryOpened : m_inventoryOpened;

            inventory.alpha = m_inventoryOpened ? inventory.alpha = 1 : inventory.alpha = 0;

            if (!Opened) playerEventHandler.inventorySlotEvent = OnNothingSelected;
        }

        public void OpenInventory()
        {
            m_inventoryOpened = true;
        }

        public void CloseInventory()
        {
            m_inventoryOpened = false;
        }

        public void SlotBtn(Image slotImage)
        {
            if(slotImage.CompareTag("Inventory Slot"))
            {
                Image itemImage = slotImage.transform.GetChild(0).GetComponent<Image>();

                if (playerEventHandler.interactionEvent == null)
                {
                    if (playerEventHandler.inventorySlotEvent.Method.Name == "OnSlotFirstSelected")
                    {
                        SetSelectedSlotImage(slotImage);

                        playerEventHandler.inventorySlotEvent = OnSlotEndSelected;
                        print(playerEventHandler.inventorySlotEvent.GetMethodInfo());
                    }

                    else if (itemImage.sprite)
                    {
                        SetSelectedSlotImage(slotImage);

                        playerEventHandler.inventorySlotEvent = OnSlotFirstSelected;
                        print(playerEventHandler.inventorySlotEvent.GetMethodInfo());
                    }
                }

                else if(itemImage.sprite && playerEventHandler.interactionEvent.Method.Name == "TryToUnlockDoor")
                {
                    for(int i = 0; i < slotImageHolder.childCount; i++)
                    {
                        if (m_playerSlots[i].name == slotImage.name)
                        {
                            playerEventHandler.interactionEvent(m_playerSlots[i], this, playerEventHandler);
                        }
                    }
                }
            }
        }

        public void SetSelectedSlotImage(Image slotImage)
        {
            m_selectedSlotImages.Add(slotImage);
        }

        public void OnNothingSelected()
        {
            for (int i = 0; i < slotImageHolder.childCount; i++)
            {
                Image itemImage = m_slotImages[i].GetChild(0).GetComponent<Image>();

                itemImage.sprite = m_playerSlots[i].Item ? m_playerSlots[i].Item.itemSprite : null;
                itemImage.color = itemImage.sprite ? Color.white : Color.clear;
            }

            dragItemImage.color = Color.clear;

            // Clear all selected
            m_selectedSlotImages.Clear();
            m_firstSelectedItems.Clear();
            m_endSelectedItems.Clear();
        }

        public void OnSlotFirstSelected()
        {
            for (int i = 0; i < slotImageHolder.childCount; i++)
            {
                if(m_selectedSlotImages.Count > 0)
                {
                    Image itemImage = m_slotImages[i].GetChild(0).GetComponent<Image>();
                    Image selectedSlotItemImage = m_selectedSlotImages[0].transform.GetChild(0).GetComponent<Image>();

                    if (itemImage != selectedSlotItemImage)
                    {
                        itemImage.sprite = m_playerSlots[i].Item ? m_playerSlots[i].Item.itemSprite : null;
                        itemImage.color = itemImage.sprite ? Color.white : Color.clear;
                    }

                    else
                    {
                        itemImage.color = Color.clear;

                        dragItemImage.sprite = itemImage.sprite;
                        dragItemImage.color = Color.white;
                        dragItemImage.transform.position = InputHandler.Instance.MousePosition;
                    }
                }
            }
        }

        public void OnSlotEndSelected()
        {
            for(int i = 0; i < slotImageHolder.childCount; i++)
            {
                if (m_selectedSlotImages.Count > 1 && m_selectedSlotImages[0] == m_selectedSlotImages[1])
                {
                    print("No Changes, Same Slot.");
                    playerEventHandler.inventorySlotEvent = OnNothingSelected;
                }

                if (m_firstSelectedItems.Count > 0)
                {
                    if (m_selectedSlotImages.Count > 1)
                    {
                        // switch the item to each other
                        if (m_playerSlots[i].name == m_selectedSlotImages[0].name)
                        {
                            m_playerSlots[i].Replace(m_endSelectedItems.ToArray());
                            m_selectedSlotImages.RemoveAt(0);
                        }
                    }

                    else if (m_playerSlots[i].name == m_selectedSlotImages[0].name)
                    {
                        m_playerSlots[i].Replace(m_firstSelectedItems.ToArray());

                        print("Slot Item Switch Success!");
                        playerEventHandler.inventorySlotEvent = OnNothingSelected;
                    }
                }

                else if (m_selectedSlotImages.Count > 0)
                {
                    // store to local variables to make sure it will not cleared
                    if (m_playerSlots[i].name == m_selectedSlotImages[0].name)
                    {
                        m_firstSelectedItems.AddRange(m_playerSlots[i].GetItems);
                    }

                    if (m_playerSlots[i].Item && m_playerSlots[i].name == m_selectedSlotImages[1].name)
                    {
                        m_endSelectedItems.AddRange(m_playerSlots[i].GetItems);
                    }
                }
            }
        }

        public void DropItem()
        {
            for(int i = 0; i < slotImageHolder.childCount; i++)
            {
                if (m_selectedSlotImages.Count > 0)
                {
                    if (m_playerSlots[i].name == m_selectedSlotImages[0].name)
                    {
                        print("Drop Item " + m_playerSlots[i].Item.itemName);

                        GameObject itemObject = Instantiate(m_playerSlots[i].Item.dropObject, m_playerCamera.Cam.transform.position + dropPositionOffset, m_playerCamera.Cam.transform.rotation);

                        itemObject.GetComponent<Rigidbody>().AddForce(m_playerCamera.Cam.transform.forward * 5, ForceMode.Impulse);

                        if (m_playerSlots[i].GetItems.Length > 1)
                        {
                            m_playerSlots[i].Remove();
                        }

                        else
                        {
                            m_playerSlots[i].Clear();
                            m_selectedSlotImages.Clear();

                            playerEventHandler.inventorySlotEvent = OnNothingSelected;
                        }
                    }
                }
            }
        }

        public void AddItem(ItemPickup itemPickup)
        {
            for(int i = 0; i < m_playerSlots.Length; i++)
            {
                if (m_playerSlots[i].Full && i >= m_playerSlots.Length - 1)
                {
                    print("No Slot Available");
                }

                else if (!m_playerSlots[i].Full)
                {
                    if (m_playerSlots[i].Item == itemPickup.GetItem)
                    {
                        m_playerSlots[i].Add(itemPickup.GetItem);
                        Destroy(itemPickup.gameObject);
                        break; // Item added success, break the looping operation!
                    }

                    else if (!m_playerSlots[i].Item)
                    {
                        m_playerSlots[i].Add(itemPickup.GetItem);
                        Destroy(itemPickup.gameObject);
                        break; // Item added success, break the looping operation!
                    }
                }
            }
        }
    }

    public class PlayerSlot
    {
        public string name = "";
        [SerializeField] List<Item> items = new ();

        public Item Item
        {
            get
            {
                return items.Count > 0 ? items[0] : null;
            }
        }

        public Item[] GetItems
        {
            get { return items.ToArray(); }
        }

        public bool Full
        {
            get
            {
                return Item && items.Count >= Item.maxCount;
            }
        }

        public bool HasRegistered
        {
            get
            {
                return name != "";
            }
        }

        public void Add(Item item)
        {
            items.Add(item);
        }

        public void Remove()
        {
            items.Remove(items[0]);
        }

        public void Clear()
        {
            items.Clear();
        }

        public void Replace(Item[] items)
        {
            this.items.Clear();
            if(items.Length > 0) this.items.AddRange(items);
        }

        public void TryRegisterSlot(string slotName, int number)
        {
            name = number < 1 ? slotName : slotName + " (" + number + ")";
        }
    }
}
