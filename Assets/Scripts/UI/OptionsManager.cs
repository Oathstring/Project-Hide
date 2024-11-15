using System;
using TMPro;
using UnityEngine;

namespace Oathstring
{
    public class OptionsManager : MonoBehaviour
    {
        [SerializeField] TMP_InputField fpsIF;
        [SerializeField] int minFps = 15;

        // Awake is called when the script object is initialised
        private void Awake()
        {
            FPSIFSet();
        }

        // Update is called once per frame
        private void Update()
        {
        
        }

        public void FPSIFSet()
        {
            int fpsValue = Convert.ToInt32(fpsIF.text);

            if (fpsValue < minFps)
            {
                fpsIF.text = 15.ToString();
                Application.targetFrameRate = Convert.ToInt32(fpsIF.text);
            }

            else
            {
                Application.targetFrameRate = Convert.ToInt32(fpsIF.text);
            }
        }
    }
}
