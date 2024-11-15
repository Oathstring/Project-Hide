using UnityEngine;

namespace Oathstring
{
    [CreateAssetMenu(fileName ="Item Data", menuName = "Add/Item")]
    public class Item : ScriptableObject
    {
        public string itemName;
        public ItemType itemType;
        public Sprite itemSprite;
        [Space]
        public int maxCount;
        [Space]
        public GameObject dropObject;
    }

    [System.Serializable]
    public enum ItemType
    {
        Key, BasementKey, SpecialKey, DoorBreaker
    }
}
