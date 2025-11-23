using UnityEngine;

public class LookInteraction : MonoBehaviour
{
    public float range = 5f;
    public LayerMask interactLayer;
    public Camera cam;
    public Transform pivot;

    private LightInteraction currentTarget;

    public Transform player;
    public float requiredFacingAngle = 60f;

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
                Vector3 toTarget = interactable.transform.position - player.position;
                toTarget.y = 0; // ignore vertical angle

                float angle = Vector3.Angle(player.forward, toTarget);

                if (angle > requiredFacingAngle)
                {
                    // Player is NOT facing the object
                    if (currentTarget != null)
                    {
                        Debug.Log("Current target = " + currentTarget.name +
              " | CanInteract = " + currentTarget.CanInteract);
                        currentTarget.HideText();
                        currentTarget = null;
                    }
                    return;
                }

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
