using UnityEngine;

public class LookInteraction : MonoBehaviour
{
    public float range = 5f;
    public LayerMask interactLayer;
    public Camera cam;
    public Transform pivot;

    private LightInteraction currentTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInteractable();

        if (currentTarget != null && currentTarget.CanInteract && Input.GetKeyDown(KeyCode.E))
        {
            currentTarget.ToggleLight();
        }
    }

    void CheckForInteractable()
    {
        Ray ray = new Ray(cam.transform.position, pivot.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, interactLayer))
        {
            LightInteraction interactable = hit.collider.GetComponent<LightInteraction>();

            if (interactable != null)
            {
                if (currentTarget != interactable)
                {
                    if (currentTarget != null)
                        currentTarget.HideText();

                    currentTarget = interactable;
                    currentTarget.ShowText();
                }

                return;
            }
        }

        if (currentTarget != null)
        {
            currentTarget.HideText();
            currentTarget = null;
        }
    }

    void OnDrawGizmos()
    {
        if (cam == null) return;

        // Draw the raycast in scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(cam.transform.position, pivot.forward * range);
    }
}
