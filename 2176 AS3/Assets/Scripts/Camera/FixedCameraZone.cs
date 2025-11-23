using System;
using System.Collections.Generic;
using UnityEngine;

public class FixedCameraZone : MonoBehaviour
{
    public static FixedCameraZone ActiveZone;

    [Header("Assign Fixed Camera Targets")]
    public List<Transform> cameraTargets;

    private int index = 0;
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        ActiveZone = this;

        index = 0;
        CameraControl.Instance.SetFixedCamera(cameraTargets[index]);

        Debug.LogWarning("Player in fixed camera zone");
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        ActiveZone = null;

        CameraControl.Instance.ClearFixedCamera();
    }

    public void SwitchToNextCamera()
    {
        if (!playerInside || cameraTargets.Count <= 1) return;

        index = (index + 1) % cameraTargets.Count;
        CameraControl.Instance.SetFixedCamera(cameraTargets[index]);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
