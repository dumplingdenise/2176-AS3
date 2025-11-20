using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KeySystem
{
    public class KeyRaycaast : MonoBehaviour
    {
        [SerializeField] private int rayLength = 5;                  // Length of the raycast
        [SerializeField] private LayerMask layerMaskInteract;       // Layer mask for interactable objects
        [SerializeField] private string excluseLayerName = null;    // Layer name to exclude from raycast

        private KeyItemController raycastedObject;
        [SerializeField] private KeyCode openDoorKey = KeyCode.Mouse0;   // Key to interact with objects

        [SerializeField] private Image crosshair = null;    // Reference to the crosshair UI element
        private bool isCrosshairActive;
        private bool doOnce;

        private string interactableTag = "InteractiveObject";    // Tag for interactable objects

        private void Update()
        {
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            int mask = 1 << LayerMask.NameToLayer(excluseLayerName) | layerMaskInteract.value;

            if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
            {
                if (hit.collider.CompareTag(interactableTag))
                {
                    if (!doOnce)
                    {
                        raycastedObject = hit.collider.gameObject.GetComponent<KeyItemController>();
                        CrosshairChange(true);
                    }
                    isCrosshairActive = true;
                    doOnce = true;

                    if (Input.GetKeyDown(openDoorKey))
                    {
                        raycastedObject.ObjectInteraction();
                    }
                }
            }
            else
            {
                if (isCrosshairActive)
                {
                    CrosshairChange(false);
                    doOnce = false;
                }
            }
        }

        void CrosshairChange(bool on)       // Change crosshair color based on interaction state
        {
            if (on && !doOnce)
            {
                crosshair.color = Color.red;        // Change crosshair color to red when aiming at an interactable object
            }
            else
            {
                crosshair.color = Color.white;      // Revert crosshair color to white when not aiming at an interactable object
                isCrosshairActive = false;      // Reset interaction state
            }
        }
    }
}
