using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshObstacle))]
public class FinalDoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("References")]
    public Transform player;
    public GameManager gameManager;
    public UIManager uiManager;

    [Header("UI Prompts")]
    public GameObject interactionUI;
    public GameObject lockedUI;

    [Header("Victory Trigger Settings")]
    public BoxCollider victoryTrigger;
    private bool victoryTriggered = false;
    public Image fadePanel;
    public ParticleSystem confetti;
    public float fadeSpeed = 1f;

    private bool playerInRange = false;
    private bool isOpen = false;
    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private NavMeshObstacle navMeshObstacle;

    void Start()
    {
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = true;

        _closedRotation = transform.rotation;
        _openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

        interactionUI.SetActive(false);
        lockedUI.SetActive(false);

        if (fadePanel != null) fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0); // Ensure fade panel is transparent at start
        else Debug.LogError("Fade Panel is not assigned!", this.gameObject);

        if (confetti == null) Debug.LogError("Confetti particle system is not assigned!", this.gameObject);

        if (victoryTrigger == null)
        {
            Debug.LogError("Victory Trigger collider is not assigned!", this.gameObject);
        }
        else
        {
            victoryTrigger.isTrigger = true;
        }
    }

    void Update()
    {
        if (playerInRange) // Only check for input if the player is in range
        {
            // Check if all tasks are complete
            if (gameManager != null && gameManager.AreAllTasksComplete)
            {
                // If the door is not already open, show the interaction prompt
                if (!isOpen)
                {
                    interactionUI.SetActive(true);
                    lockedUI.SetActive(false);

                    // Check for the interaction key press (CHANGED TO MOUSE CLICK)
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(OpenFinalDoor());
                    }
                }
            }
            else
            {
                // If tasks are not complete, show the locked prompt
                interactionUI.SetActive(false);
                lockedUI.SetActive(true);
            }
        }
        else
        {
            // Ensure UI is turned off when the player is not in range
            interactionUI.SetActive(false);
            lockedUI.SetActive(false);
        }
    }

    IEnumerator OpenFinalDoor()
    {
        isOpen = true;

        navMeshObstacle.enabled = false;

        interactionUI.SetActive(false);
        lockedUI.SetActive(false);

        if (AudioManager.instance != null) AudioManager.instance.PlaySound("DoorOpen");

        while (Quaternion.Angle(transform.rotation, _openRotation) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _openRotation, openSpeed * 100f * Time.deltaTime);
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionUI.SetActive(false);
            lockedUI.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // This check has been improved to only run if the door is open
        if (isOpen && !victoryTriggered && other.CompareTag("Player"))
        {
            if (other.bounds.Intersects(victoryTrigger.bounds))
            {
                victoryTriggered = true;
                Debug.Log("Player entered the victory zone!");

                // Instead of calling the UIManager directly, we start our new sequence.
                StartCoroutine(VictorySequence());
            }
        }
    }

    private System.Collections.IEnumerator VictorySequence()
    {
        // Play confetti particle system
        if (confetti != null)
        {
            confetti.Play();
        }

        // Fade to black
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            while (c.a < 1)
            {
                c.a += Time.deltaTime * fadeSpeed;
                fadePanel.color = c;
                yield return null;
            }
        }

        if (uiManager != null)
        {
            uiManager.ShowVictoryScreen();
        }
    }
}