using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform TPCameraTarget;
    public Transform FPCameraTarget;

    public float pLerp = .02f;
    public float rLerp = .01f;

    public float switchSpeed = 5f;

    private Transform currentTarget;
    private bool isFirstPerson = false;

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
        }

        transform.position = Vector3.Lerp(transform.position, currentTarget.position, pLerp);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, rLerp);
    }
}
