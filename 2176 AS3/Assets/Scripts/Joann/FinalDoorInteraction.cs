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
    public BoxCollider victoryTrigger; // a separate trigger to detect when the player wins
    private bool victoryTriggered = false;
    public Image fadePanel;
    public ParticleSystem confetti;
    public float fadeSpeed = 1f;

    // internal state variables for the door
    private bool playerInRange = false;
    private bool isOpen = false;
    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private NavMeshObstacle navMeshObstacle;

    void Start()
    {
        // get navmesh obstacle component & ensure it's enabled at start
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = true;

        // store the initial closed rotation & calculate the target open rotation
        _closedRotation = transform.rotation;
        _openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

        // ensure all ui is hidden on start
        interactionUI.SetActive(false);
        lockedUI.SetActive(false);

        // safety checks for all assigned references
        if (fadePanel != null) fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0); // ensure fade panel is transparent at start
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
        // only check for input if the player is within the trigger zone
        if (playerInRange)
        {
            // check with the gamemanager if the unlock condition has been met
            if (gameManager != null && gameManager.AreAllTasksComplete)
            {
                // only show interaction prompt if the door isn't already open
                if (!isOpen)
                {
                    interactionUI.SetActive(true);
                    lockedUI.SetActive(false);

                    // listen for the player's click to open the door
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(OpenFinalDoor());
                    }
                }
            }
            else
            {
                // if tasks are not complete, show the locked prompt
                interactionUI.SetActive(false);
                lockedUI.SetActive(true);
            }
        }
        else
        {
            // ensure ui is turned off when the player is not in range
            interactionUI.SetActive(false);
            lockedUI.SetActive(false);
        }
    }

    // coroutine to handle the door opening animation over several frames
    IEnumerator OpenFinalDoor()
    {
        isOpen = true; // set state to open to prevent this from running again
        navMeshObstacle.enabled = false; // disable obstacle so ai can pass through
        interactionUI.SetActive(false); // hide all ui prompts
        lockedUI.SetActive(false);

        if (AudioManager.instance != null) AudioManager.instance.PlaySound("DoorOpen");

        // smoothly rotate the door towards its open position
        while (Quaternion.Angle(transform.rotation, _openRotation) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _openRotation, openSpeed * 100f * Time.deltaTime);
            yield return null;
        }
    }

    // detect when the player enters the door's main trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    // detect when the player leaves the door's main trigger
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionUI.SetActive(false);
            lockedUI.SetActive(false);
        }
    }

    // this is checked every frame the player is inside the trigger
    private void OnTriggerStay(Collider other)
    {
        // only check for victory after the door is open & victory hasn't already been triggered
        if (isOpen && !victoryTriggered && other.CompareTag("Player"))
        {
            // checks if the player is intersecting with the specific victory trigger box
            if (other.bounds.Intersects(victoryTrigger.bounds))
            {
                victoryTriggered = true;
                StartCoroutine(VictorySequence());
            }
        }
    }

    // coroutine to handle the sequence of events upon winning the game
    private System.Collections.IEnumerator VictorySequence()
    {
        // play visual effects
        if (confetti != null)
        {
            confetti.Play();
        }

        // smoothly fade the screen to black
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

        // tell the uimanager to show the final victory screen
        if (uiManager != null)
        {
            uiManager.ShowVictoryScreen();
        }
    }
}