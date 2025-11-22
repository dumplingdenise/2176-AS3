using UnityEngine;

public class PlayerPush_Shumin : MonoBehaviour
{
    public float pushForce = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic)
        {
            return;
        }

        if (!hit.collider.CompareTag("Pushable"))
        {
            return;
        }

        // push in the horizontal direction you are moving
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        body.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }
}
