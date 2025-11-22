using System.Collections;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    private Quaternion _closedRotation;
    private Quaternion _openRotationForward;
    private Quaternion _openRotationBackward;

    public Transform player; // assign in Inspector

    void Start()
    {
        _closedRotation = transform.rotation;

        // Two possible open directions:
        _openRotationForward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        _openRotationBackward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -openAngle, 0));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ToggleDoor());
        }
    }

    IEnumerator ToggleDoor()
    {
        isOpen = !isOpen;

        Quaternion targetRot = _closedRotation;

        if (isOpen)
        {
            // Determine which side the player is on to prevent the door from slamming the player LOL
            Vector3 toPlayer = (player.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, toPlayer);

            targetRot = (dot > 0) ? _openRotationBackward : _openRotationForward;
        }

        // Smooth rotation using RotateTowards (never stops early, does a full open and close)
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                openSpeed * 100f * Time.deltaTime
            );
            yield return null;
        }

        transform.rotation = targetRot; // Snap to exact target rotation
    }
}
