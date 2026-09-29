using UnityEngine;

// The gun in the bottom corner of the screen. Sits on the AR camera so it follows the phone.
// Kicks back and flashes on every shot, trails a little behind when the phone turns,
// and is only shown while a round is being played.
public class WeaponView : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] SpriteRenderer muzzleFlash;
    [SerializeField] float flashTime = 0.05f;

    [Header("Recoil")]
    [SerializeField] float kickBack = 0.03f;   // metres
    [SerializeField] float kickUp = 7f;        // degrees
    [SerializeField] float recoverSpeed = 14f;

    [Header("Sway")]
    [SerializeField] float swayAmount = 0.5f;
    [SerializeField] float maxSway = 4f;
    [SerializeField] float swaySmoothing = 8f;

    Transform cameraTransform;
    Quaternion lastCameraRotation;
    Vector3 restPosition;
    Quaternion restRotation;
    Vector3 flashScale;
    Vector2 sway;
    float recoil;
    float flashTimer;

    void Awake()
    {
        cameraTransform = transform.parent;
        lastCameraRotation = cameraTransform.rotation;
        restPosition = transform.localPosition;
        restRotation = transform.localRotation;
        flashScale = muzzleFlash.transform.localScale;
        muzzleFlash.enabled = false;
    }

    void OnEnable()
    {
        GameEvents.PlayerFired += Kick;
        GameEvents.StateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        GameEvents.PlayerFired -= Kick;
        GameEvents.StateChanged -= OnStateChanged;
    }

    void OnStateChanged(GameStateId state)
    {
        model.SetActive(state == GameStateId.Playing);
        muzzleFlash.enabled = false;
        recoil = 0f;
    }

    void Kick()
    {
        recoil = 1f;
        flashTimer = flashTime;

        // random spin and size so no two flashes look the same
        muzzleFlash.enabled = true;
        muzzleFlash.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        muzzleFlash.transform.localScale = flashScale * Random.Range(0.8f, 1.25f);
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;

        // how far the phone turned since last frame, the gun lags the opposite way
        Quaternion turn = Quaternion.Inverse(lastCameraRotation) * cameraTransform.rotation;
        lastCameraRotation = cameraTransform.rotation;
        Vector3 angles = turn.eulerAngles;
        var target = new Vector2(
            Mathf.Clamp(-Mathf.DeltaAngle(0f, angles.y) * swayAmount, -maxSway, maxSway),
            Mathf.Clamp(-Mathf.DeltaAngle(0f, angles.x) * swayAmount, -maxSway, maxSway));
        sway = Vector2.Lerp(sway, target, swaySmoothing * dt);

        recoil = Mathf.Lerp(recoil, 0f, recoverSpeed * dt);

        transform.localPosition = restPosition + Vector3.back * (kickBack * recoil);
        transform.localRotation = restRotation * Quaternion.Euler(-kickUp * recoil + sway.y, sway.x, 0f);

        if (flashTimer > 0f)
        {
            flashTimer -= dt;
            if (flashTimer <= 0f) muzzleFlash.enabled = false;
        }
    }
}
