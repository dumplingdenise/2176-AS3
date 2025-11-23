using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform TPCameraTarget;
    public Transform FPCameraTarget;

    public float pLerp = .02f;
    public float rLerp = .01f;

    public float switchSpeed = 5f;

    public MeshRenderer hatMesh;

    private Transform currentTarget;
    private bool isFirstPerson = false;

    // For camera collision
    public LayerMask cameraCollisionMask;  
    public float cameraCollisionRadius = 0.2f;
    public float collisionSmooth = 10f;    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTarget = TPCameraTarget; // start in 3rd person
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {

            isFirstPerson = !isFirstPerson;
            currentTarget = isFirstPerson ? FPCameraTarget : TPCameraTarget;

            hatMesh.enabled = !isFirstPerson;
        }

        transform.position = Vector3.Lerp(transform.position, currentTarget.position, pLerp);
        /*transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, rLerp);*/
    }

    void LateUpdate()
    {
        // Desired target position (FP or TP)
        Vector3 desiredPos = currentTarget.position;

        // Pivot is the parent object of the camera
        Transform pivot = transform.parent;
        Vector3 pivotPos = pivot.position;

        // Direction from pivot to desired camera pos
        Vector3 direction = desiredPos - pivotPos;
        float distance = direction.magnitude;

        // Check for camera collision
        if (Physics.SphereCast(
            pivotPos,
            cameraCollisionRadius,
            direction.normalized,
            out RaycastHit hit,
            distance,
            cameraCollisionMask))
        {
            // Push camera in front of the obstacle
            desiredPos = hit.point - direction.normalized * cameraCollisionRadius;
        }

        // Smooth camera movement to final position
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            Time.deltaTime * collisionSmooth
        );
    }
}
