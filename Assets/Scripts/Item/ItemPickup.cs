using UnityEngine;

namespace Oathstring
{
    public class ItemPickup : MonoBehaviour
    {
        private Rigidbody m_rigidbody;

        [SerializeField] Item item;
        [SerializeField] string insidePlayer = "Inside Player";
        [SerializeField] string outsidePlayer = "Default";
        [Space]
        [SerializeField] LayerMask excludeLayerOnInsidePlayer;

        public string InsidePlayer { get { return insidePlayer; } }

        public Item GetItem
        {
            get { return item; } 
        }

        private void OnEnable()
        {
            gameObject.layer = LayerMask.NameToLayer(insidePlayer);
            m_rigidbody.excludeLayers = excludeLayerOnInsidePlayer;
        }

        // Awake is called when the script object is initialised
        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        private void Update()
        {
        
        }

        private void OnCollisionEnter(Collision collision)
        {
            gameObject.layer = LayerMask.NameToLayer(outsidePlayer);
            m_rigidbody.excludeLayers = 0;
        }
    }
}
