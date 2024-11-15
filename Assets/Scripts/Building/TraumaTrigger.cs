using UnityEngine;
using UnityEngine.Playables;

namespace Oathstring
{
    public class TraumaTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] PlayableDirector traumaCutscene;
        [SerializeField] Transform playerPos;

        // Awake is called when the script object is initialised
        private void Awake()
        {

        }

        // Update is called once per frame
        private void Update()
        {
        
        }

        public void FightAgainstTrauma(Rigidbody playerRB)
        {
            playerRB.position = playerPos.position;
            playerRB.rotation = Quaternion.Euler(new(0, 90, 0));

            traumaCutscene.Play();
        }
    }
}
