using UnityEngine;
using UnityEngine.InputSystem;

// FPS shooting. Hold a finger anywhere on the screen to fire at the crosshair.
// Bullets come out of a point just below the camera, like a gun held in front of you,
// but are aimed at whatever sits under the crosshair so what you see is what you hit.
[RequireComponent(typeof(Camera))]
public class PlayerShooter : MonoBehaviour
{
    [SerializeField] ProjectilePool bulletPool;
    [SerializeField, Min(1)] int damage = 1;
    [SerializeField, Min(0.5f)] float fireRate = 5f; // shots per second
    [SerializeField] float aimRange = 20f;
    [Tooltip("Default + Enemy. Leave Player out so we don't aim at ourselves.")]
    [SerializeField] LayerMask aimMask = ~0;
    [SerializeField] Vector3 muzzleOffset = new(0.04f, -0.06f, 0.15f);

    float nextShotTime;

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameStateId.Playing) return;

        var pointer = Pointer.current;
        if (pointer == null || !pointer.press.isPressed) return;
        if (Time.time < nextShotTime) return;
        if (ScreenInput.IsOverUI(pointer.position.ReadValue())) return;

        Fire();
    }

    void Fire()
    {
        nextShotTime = Time.time + 1f / fireRate;

        Vector3 muzzle = transform.TransformPoint(muzzleOffset);

        Vector3 target = Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, aimRange, aimMask, QueryTriggerInteraction.Collide)
            ? hit.point
            : transform.position + transform.forward * aimRange;

        bulletPool.Fire(muzzle, (target - muzzle).normalized, damage);
        GameEvents.RaisePlayerFired();
    }
}
