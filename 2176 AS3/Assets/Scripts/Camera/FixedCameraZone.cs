using System;
using System.Collections.Generic;
using UnityEngine;

public class FixedCameraZone : MonoBehaviour
{
    public static FixedCameraZone ActiveZone;

    [Header("Assign Fixed Camera Targets")]
    public List<Transform> cameraTargets;

    [Header("Zone Size (Half Extents)")]
    public Vector3 zoneHalfExtents = new Vector3(2f, 2f, 2f);

    [Header("Player Tag")]
    public string playerTag = "Player";

    private int index = 0;
    private bool playerInside = false;

    void Update()
    {
        // Check every collider in the OverlapBox
        Collider[] hits = Physics.OverlapBox(
            transform.position,
            zoneHalfExtents,
            transform.rotation
        );

        bool isInside = false;

        // Filter only the collider with Player tag
        foreach (var col in hits)
        {
            if (col.CompareTag(playerTag))
            {
                isInside = true;
                break;
            }
        }

        // Enter zone
        if (!playerInside && isInside)
        {
            playerInside = true;
            ActiveZone = this;
            index = 0;

            CameraControl.Instance.SetFixedCamera(cameraTargets[index]);
            Debug.Log("Player entered FIXED CAMERA ZONE");
        }
        // Exit zone
        else if (playerInside && !isInside)
        {
            playerInside = false;
            ActiveZone = null;

            CameraControl.Instance.ClearFixedCamera();
            Debug.Log("Player left FIXED CAMERA ZONE");
        }
    }

    public void SwitchToNextCamera()
    {
        if (!playerInside || cameraTargets.Count <= 1) return;

        index = (index + 1) % cameraTargets.Count;
        CameraControl.Instance.SetFixedCamera(cameraTargets[index]);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, zoneHalfExtents * 2);
    }
}
