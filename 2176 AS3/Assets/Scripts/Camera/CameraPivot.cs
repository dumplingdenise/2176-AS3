using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    public float sensitivity = 2f;
    public float minY = -30f;
    public float maxY = 60f;

    float yaw;
    float pitch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // STOP ROTATION in fixed camera mode
        if (CameraControl.Instance.IsInFixedCamera)
        {
            yaw = 0;
            pitch = 0;
            transform.rotation = Quaternion.identity;
            return;
        }
       
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
