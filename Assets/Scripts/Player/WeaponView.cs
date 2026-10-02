using UnityEngine;

// The gun in the bottom corner of the screen. Sits on the AR camera so it follows the phone.
// Kicks back and flashes on every shot, trails a little behind when the phone turns,
// and is only shown while a round is being played.
public class WeaponView : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] SpriteRenderer muzzleFlash;
    [SerializeField] float flashTime = 0.05f;

    [Header("Screen fit")]
    [SerializeField] Vector2 screenPoint = new(0.78f, 0.22f); // where the gun sits, 0-1 across the screen
    [SerializeField] float holdDistance = 0.24f;              // metres in front of the camera
    [SerializeField] float designViewWidth = 0.3f;            // view width at holdDistance the model was sized for

    [Header("Recoil")]
    [SerializeField] float kickBack = 0.03f;   // metres
    [SerializeField] float kickUp = 7f;        // degrees
    [SerializeField] float recoverSpeed = 14f;

    [Header("Sway")]
    [SerializeField] float swayAmount = 0.5f;
    [SerializeField] float maxSway = 4f;
    [SerializeField] float swaySmoothing = 8f;

    Transform cameraTransform;
    Camera cam;
    Quaternion lastCameraRotation;
    Vector3 restPosition;
    Quaternion restRotation;
    Vector3 baseScale;
    float fit = 1f;
    Vector3 flashScale;
    Vector2 sway;
    float recoil;
    float flashTimer;

    void Awake()
    {
        cameraTransform = transform.parent;
        cam = cameraTransform.GetComponent<Camera>();
        lastCameraRotation = cameraTransform.rotation;
        restPosition = transform.localPosition;
        restRotation = transform.localRotation;
        baseScale = transform.localScale;
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

        FitToScreen();
        transform.localPosition = restPosition + Vector3.back * (kickBack * fit * recoil);
        transform.localRotation = restRotation * Quaternion.Euler(-kickUp * recoil + sway.y, sway.x, 0f);

        if (flashTimer > 0f)
        {
            flashTimer -= dt;
            if (flashTimer <= 0f) muzzleFlash.enabled = false;
        }
    }

    // A phone camera in portrait sees a much narrower view than the editor does, so a gun
    // with a fixed size ends up covering half the screen. Instead it's placed and sized
    // from the AR camera's real field of view, which ARCore sets on the projection matrix.
    void FitToScreen()
    {
        Matrix4x4 projection = cam.projectionMatrix;
        float halfWidth = holdDistance / projection.m00;
        float halfHeight = holdDistance / projection.m11;

        fit = halfWidth * 2f / designViewWidth;
        restPosition = new Vector3(
            (screenPoint.x * 2f - 1f) * halfWidth,
            (screenPoint.y * 2f - 1f) * halfHeight,
            holdDistance);
        transform.localScale = baseScale * fit;
    }
}
