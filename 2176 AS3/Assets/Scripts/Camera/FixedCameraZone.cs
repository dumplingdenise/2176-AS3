using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCameraZone : MonoBehaviour
{
    public static FixedCameraZone ActiveZone;

    [Header("Group ID (Zones with same ID act as ONE zone)")]
    public string zoneGroupID = "DefaultGroup";

    [Header("Assign Fixed Camera Targets")]
    public List<Transform> cameraTargets;

    [Header("Zone Size (Half Extents)")]
    public Vector3 zoneHalfExtents = new Vector3(2f, 2f, 2f);

    [Header("Player Tag")]
    public string playerTag = "Player";

    private int index = 0;
    private bool playerInside = false;
    private bool switchingCamera = false;
    private float zoneSwitchCooldown = 0.3f;
    private float lastZoneSwitchTime = -1f;

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

        if (!playerInside && isInside)
            EnterZone();
        else if (playerInside && !isInside)
            ExitZone();
    }

    void EnterZone()
    {
        if (Time.time < lastZoneSwitchTime + zoneSwitchCooldown)
            return;

        lastZoneSwitchTime = Time.time;

        playerInside = true;

        if (ActiveZone == null)
        {
            ActivateThisZone();
            return;
        }

        if (ActiveZone.zoneGroupID == this.zoneGroupID)
        {
            ActiveZone = this;
            return;
        }

        ActivateThisZone();
    }

    void ActivateThisZone()
    {
        ActiveZone = this;
        index = 0;
        StartCoroutine(DelayedSetCamera());
    }

    void ExitZone()
    {
        playerInside = false;

        // Only clear the camera if absolutely leaving the ENTIRE zone group
        if (ActiveZone == this)
        {
            ActiveZone = null;
            CameraControl.Instance.ClearFixedCamera();
        }
    }
    private IEnumerator DelayedSetCamera()
    {
        switchingCamera = true;

        // Wait exactly 1 frame – essential to avoid bounce
        yield return null;

        if (playerInside)
            CameraControl.Instance.SetFixedCamera(cameraTargets[index]);

        switchingCamera = false;
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
