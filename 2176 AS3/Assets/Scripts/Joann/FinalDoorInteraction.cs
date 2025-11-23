using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
        if (playerInRange && !isOpen)
        {
            if (gameManager != null && gameManager.AreAllTasksComplete)
            {
                interactionUI.SetActive(true);
                lockedUI.SetActive(false);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(OpenFinalDoor());
                }
            }
            else
            {
                interactionUI.SetActive(false);
                lockedUI.SetActive(true);
            }
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
        if (!victoryTriggered && other.CompareTag("Player"))
        {
            if (other.bounds.Intersects(victoryTrigger.bounds))
            {
                victoryTriggered = true;
                Debug.Log("Player entered the victory zone!");

                if (uiManager != null)
                {
                    uiManager.ShowVictoryScreen();
                }
            }
        }
    }
}