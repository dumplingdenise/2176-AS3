using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraControl : MonoBehaviour
{
    public static CameraControl Instance;

    public Transform TPCameraTarget;
    public Transform FPCameraTarget;

    public float pLerp = .02f;
    public float rLerp = .01f;

    public float switchSpeed = 5f;

    public MeshRenderer hatMesh;

    private Transform currentTarget;
    public bool isFirstPerson = false;

    // For camera collision
    public LayerMask cameraCollisionMask;  
    public float cameraCollisionRadius = 0.2f;
    public float collisionSmooth = 10f;

    // For fixed camera angle
    private Transform fixedCameraTarget = null;

    public bool IsInFixedCamera => fixedCameraTarget != null;

    void Awake()
    {
        Instance = this;
    }

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
            if (fixedCameraTarget != null)
            {
                FixedCameraZone.ActiveZone?.SwitchToNextCamera();
                return;
            }

            isFirstPerson = !isFirstPerson;
            currentTarget = isFirstPerson ? FPCameraTarget : TPCameraTarget;

            hatMesh.enabled = !isFirstPerson;
        }

        /*transform.position = Vector3.Lerp(transform.position, currentTarget.position, pLerp);*/
        /*transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, rLerp);*/
        
    }

    void LateUpdate()
    {
        Transform pivot = transform.parent;
        Vector3 pivotPos = pivot.position;

        Transform target = (fixedCameraTarget != null)
            ? fixedCameraTarget
            : currentTarget;

        Vector3 desiredPos = target.position;


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

        // Fixed camera angle
        if (fixedCameraTarget != null)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                fixedCameraTarget.rotation,
                rLerp
            );
        }
        else
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                pivot.rotation,
                rLerp
            );
        }
    }

    public void SetFixedCamera(Transform t)
    {
        fixedCameraTarget = t;

        // RESET PIVOT ROTATION
        Transform pivot = transform.parent;
        pivot.rotation = Quaternion.identity;   // no tilt, no yaw, no pitch
    }

    public void ClearFixedCamera()
    {
        fixedCameraTarget = null;
    }
}
