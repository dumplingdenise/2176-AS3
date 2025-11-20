using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem
{
    public class KeyDoorController : MonoBehaviour
    {
        private Animator doorAnim;
        private bool doorOpen = false;

        [Header("Animation Names")]
        [SerializeField] private string openAnimationName = "DoorOpen";     // Name of the open door animation
        [SerializeField] private string closeAnimationName = "DoorClose";   // Name of the close door animation

        [SerializeField] private int timeToShowUI = 1;                      // Time to show the "Need Key" UI message
        [SerializeField] private GameObject showDoorLockedUI = null;        // Reference to the "Door Locked" UI element

        [SerializeField] private KeyInventory _keyInventory = null;         // Reference to the KeyInventory script

        [SerializeField] private int waitTimer = 1;                           // Wait time before closing the door
        [SerializeField] private bool pauseInteraction = false;               // Flag to pause interaction during door animation

        private void Awake()
        {
            doorAnim = gameObject.GetComponent<Animator>();
        }

        private IEnumerator PauseDoorInteraction()
        {
            pauseInteraction = true;
            yield return new WaitForSeconds(waitTimer);
            pauseInteraction = false;
        }

        public void PlayAnimation()
        {
            if(_keyInventory.hasRedKey)
            {
                OpenDoor();             // Open or close the door based on its current state
            }
           
            else
            {
                StartCoroutine(ShowDoorLocked());
            }
        }

        void OpenDoor()
        {
            if (!doorOpen && !pauseInteraction)
            {
                doorAnim.Play(openAnimationName, 0, 0.0f);
                doorOpen = true;
                StartCoroutine(PauseDoorInteraction());
            }

            else if (doorOpen && !pauseInteraction)
            {
                doorAnim.Play(closeAnimationName, 0, 0.0f);
                doorOpen = false;
                StartCoroutine(PauseDoorInteraction());
            }
        }


        IEnumerator ShowDoorLocked()
        {
            showDoorLockedUI.SetActive(true);
            yield return new WaitForSeconds(timeToShowUI);
            showDoorLockedUI.SetActive(false);
        }
    }
}
