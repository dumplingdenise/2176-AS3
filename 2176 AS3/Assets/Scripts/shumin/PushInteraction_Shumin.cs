using UnityEngine;

public class PushInteraction_Shumin : MonoBehaviour
{
    [Header("Box to push")]
    public Rigidbody boxRb;      

    [HideInInspector] public bool isPlayerNear = false;
    [HideInInspector] public bool isUnlocked = false;

    Coroutine relockRoutine;

    void Start()
    {
        if (boxRb == null)
        {
            return;
        }

        // Box starts locked
        boxRb.isKinematic = true;
        isUnlocked = false;
    }

    void Update()
    {
        if (!isPlayerNear || boxRb == null)
            return;

        // Player is near and presse E
        if (Input.GetKeyDown(KeyCode.E))
        {
            boxRb.isKinematic = false;

            isUnlocked = true;

          //  Debug.Log("PushInteraction: box unlocked (isKinematic = false).");

            // cancel any pending relock if player just unlocked again
            if (relockRoutine != null)
            {
                StopCoroutine(relockRoutine);
                relockRoutine = null;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNear = true;

        // if player comes back in, cancel relock
        if (relockRoutine != null)
        {
            StopCoroutine(relockRoutine);
            relockRoutine = null;
        }

       // Debug.Log("PushInteraction: player entered push area.");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNear = false;

        // Start small delay before locking, to avoid instant exit when the object jiggles a bit
        if (relockRoutine == null)
            relockRoutine = StartCoroutine(RelockAfterDelay());
    }

    System.Collections.IEnumerator RelockAfterDelay()
    {
        // small buffer so slight movement doesn't spam exit/enter
        yield return new WaitForSeconds(0.15f);

        if (!isPlayerNear && boxRb != null)
        {
            boxRb.isKinematic = true; 
            isUnlocked = false;

           // Debug.Log("PushInteraction: player left, box locked (isKinematic = true).");
        }

        relockRoutine = null;
    }
}
