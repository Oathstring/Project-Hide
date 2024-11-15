using System.Collections;
using UnityEngine;

namespace Oathstring
{
    public class Hideable : MonoBehaviour
    {
        private Vector3 m_hidePos;
        private Vector3 m_outPos;

        [SerializeField] Vector3 hidePosOffset;
        [SerializeField] Vector3 outPosOffset;
        [SerializeField] bool corner;

        // Awake is called when the script object is initialised
        private void Awake()
        {
            
        }

        // Update is called once per frame
        private void Update()
        {
        
        }

        private void HidePosCalculating()
        {
            m_hidePos = transform.position + hidePosOffset;
        }

        private void OutPosCalculating()
        {
            if (corner)
            {
                // only one direction out pos
                Vector3 outOffSet = new(outPosOffset.x, outPosOffset.y, 0);
                float zOutOffSet = outPosOffset.z;
                m_outPos = transform.position + (transform.forward * zOutOffSet + outOffSet);
            }

            else
            {
                // randomize out pos 50/50
                float randomValue = Random.Range(0f, 1f);
                if (randomValue <= .5f)
                {
                    Vector3 outOffSet = new(outPosOffset.x, outPosOffset.y, 0);
                    float zOutOffSet = outPosOffset.z;
                    m_outPos = transform.position + (-transform.forward * zOutOffSet + outOffSet);
                }

                else
                {
                    Vector3 outOffSet = new(outPosOffset.x, outPosOffset.y, 0);
                    float zOutOffSet = outPosOffset.z;
                    m_outPos = transform.position + (transform.forward * zOutOffSet + outOffSet);
                }
            }
        }

        public void HidePlayer(PlayerController player, Rigidbody playerRB)
        {
            player.SetHide(true, this);
            HidePosCalculating();
            playerRB.position = m_hidePos;
        }

        public void UnHidePlayer(PlayerController player, Rigidbody playerRB)
        {
            player.SetHide(false, null);
            OutPosCalculating();
            playerRB.position = m_outPos;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, transform.position + hidePosOffset);
            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * outPosOffset.z));
        }
    }
}
