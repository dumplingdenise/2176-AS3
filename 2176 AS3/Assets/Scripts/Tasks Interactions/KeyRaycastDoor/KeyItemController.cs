using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem
{
    public class KeyItemController : MonoBehaviour
    {
        [SerializeField] private bool RedDoor = false;        // to know whether Is this object a door?
        [SerializeField] private bool RedKey = false;        // to know whether Is this object a key?

        [SerializeField] private KeyInventory _keyInventory = null;     // Reference to the KeyInventory script

        private KeyDoorController doorObject;

        private void Start()
        {
            if(RedKey)
            {
                doorObject = GetComponent<KeyDoorController>();
            }
        }
        public void ObjectInteraction()
        {
            if (RedDoor)
            {
                doorObject.PlayAnimation();     // Play the door animation
            }
            else if (RedKey)
            {
                _keyInventory.hasRedKey = true;       // Update inventory to indicate the player has the key
                gameObject.SetActive(false);        // Deactivate the key object in the scene
            }
        }
    }
}


