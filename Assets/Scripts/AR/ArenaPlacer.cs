using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// Tap-to-place. The first tap on a horizontal plane spawns the arena and anchors it there.
// After that, extra taps are ignored and we stop scanning for new planes.
[RequireComponent(typeof(ARRaycastManager))]
public class ArenaPlacer : MonoBehaviour
{
    [SerializeField] GameObject arenaPrefab;
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] ARAnchorManager anchorManager;
    [SerializeField] bool placementEnabled = true;

    public event Action<Transform> ArenaPlaced;

    public bool IsPlaced => arena != null;
    public Transform Arena => arena;

    ARRaycastManager raycastManager;
    Transform arena;

    static readonly List<ARRaycastHit> arHits = new();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    public void SetPlacementEnabled(bool value) => placementEnabled = value;

    void Update()
    {
        if (IsPlaced || !placementEnabled) return;

        // Pointer covers both touch on the phone and mouse in the editor
        var pointer = Pointer.current;
        if (pointer == null || !pointer.press.wasPressedThisFrame) return;

        Vector2 screenPos = pointer.position.ReadValue();
        if (ScreenInput.IsOverUI(screenPos)) return;

        if (!raycastManager.Raycast(screenPos, arHits, TrackableType.PlaneWithinPolygon)) return;

        ARPlane plane = planeManager.GetPlane(arHits[0].trackableId);
        if (plane == null || plane.alignment != PlaneAlignment.HorizontalUp) return;

        Place(plane, arHits[0].pose);
    }

    void Place(ARPlane plane, Pose hitPose)
    {
        // turn the arena so its back faces away from the player, that way enemies come from the far side
        Vector3 away = hitPose.position - Camera.main.transform.position;
        away.y = 0f;
        Quaternion rotation = away.sqrMagnitude > 0.001f ? Quaternion.LookRotation(away) : hitPose.rotation;

        // anchoring to the plane keeps the arena in place when ARCore refines its tracking
        ARAnchor anchor = anchorManager.AttachAnchor(plane, new Pose(hitPose.position, rotation));
        Transform parent = anchor != null ? anchor.transform : null;

        arena = Instantiate(arenaPrefab, hitPose.position, rotation, parent).transform;

        StopPlaneDetection();
        ArenaPlaced?.Invoke(arena);
    }

    void StopPlaneDetection()
    {
        planeManager.enabled = false;
        foreach (ARPlane plane in planeManager.trackables)
            plane.gameObject.SetActive(false);
    }
}
