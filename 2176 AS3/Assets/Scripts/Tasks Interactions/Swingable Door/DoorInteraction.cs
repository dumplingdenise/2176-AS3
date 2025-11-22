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

    public Transform player;         // assign in Inspector
    public GameObject interactionUI;        // assign in Inspector

    private bool canOpen = false;   // This is only true when player is inside trigger

    void Start()
    {
        _closedRotation = transform.rotation;

        _openRotationForward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        _openRotationBackward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -openAngle, 0));
    }

    void Update()
    {
        // Only open when inside trigger and click left mouse
        if (canOpen && Input.GetMouseButtonDown(0))
        {
            interactionUI.SetActive(false); // Interaction UI Text to disappear after opening the door.
            StartCoroutine(ToggleDoor());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            canOpen = true;
        interactionUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canOpen = false;
        interactionUI.SetActive(false);
    }

    IEnumerator ToggleDoor()
    {
        isOpen = !isOpen;

        Quaternion targetRot = _closedRotation;

        if (isOpen)
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, toPlayer);

            targetRot = (dot > 0) ? _openRotationBackward : _openRotationForward;
        }

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                openSpeed * 100f * Time.deltaTime
            );
            yield return null;
        }

        transform.rotation = targetRot;
    }
}

