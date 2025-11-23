using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    public float sensitivity = 2f;
    /*public float minY = -30f;
    public float maxY = 60f;*/

    public float fpMinY = -85f;
    public float fpMaxY = 85f;

    public float tpMinY = -30f;
    public float tpMaxY = 60f;

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
        if (CameraControl.Instance.IsInFixedCamera)
        {
            yaw = 0;
            pitch = 0;
            transform.rotation = Quaternion.identity;
            return;
        }
       
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        /*pitch = Mathf.Clamp(pitch, minY, maxY);*/
        if (CameraControl.Instance.isFirstPerson)   // whatever your boolean is
        {
            pitch = Mathf.Clamp(pitch, fpMinY, fpMaxY);
        }
        else
        {
            pitch = Mathf.Clamp(pitch, tpMinY, tpMaxY);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
