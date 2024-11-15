using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Oathstring
{
    public class BasementDoor : Door
    {
        [Header("Settings")]
        [SerializeField] GameObject endScreenEscape;
        [SerializeField] PlayableDirector escapeTimeline;

        public override void TryToUnlockDoor(PlayerSlot slotRequired, PlayerInventory playerInventory, PlayerEventHandler eventHandler)
        {
            base.TryToUnlockDoor(slotRequired, playerInventory, eventHandler);

            if (!Locked)
            {
                print("You are escaped");
                endScreenEscape.SetActive(true);
                escapeTimeline.Play();
            }
        }
    }
}
