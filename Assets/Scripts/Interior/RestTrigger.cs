using UnityEngine;
using UnityEngine.Playables;

namespace Oathstring
{
    public class RestTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] GameObject endScreenRest;
        [SerializeField] PlayableDirector restTimeline;

        public void OpenEndScreenRest()
        {
            endScreenRest.SetActive(true);
            restTimeline.Play();
        }
    }
}
