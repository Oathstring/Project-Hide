using UnityEngine;
using UnityEngine.Playables;

namespace Oathstring
{
    public class CutsceneManager : MonoBehaviour
    {
        private PlayableDirector m_currentDirector;

        // Awake is called when the script object is initialised
        private void Awake()
        {
        
        }

        // Update is called once per frame
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) && m_currentDirector && !MenuManager.Instance.Paused)
            {
                if(m_currentDirector.state == PlayState.Playing)
                {
                    double skipTime = m_currentDirector.playableAsset.duration;
                    m_currentDirector.time = skipTime;
                }
            }
        }

        public void GetDirector(PlayableDirector director)
        {
            m_currentDirector = director;
        }
    }
}
