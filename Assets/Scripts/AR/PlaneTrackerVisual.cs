using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// Goes on the plane prefab. Fades the name texture in when a plane is found
// and gives it a slow pulse so the player can see where they can place the arena.
[RequireComponent(typeof(ARPlane), typeof(MeshRenderer))]
public class PlaneTrackerVisual : MonoBehaviour
{
    [SerializeField] float fadeInTime = 0.6f;
    [SerializeField] float pulseSpeed = 2f;
    [SerializeField, Range(0f, 1f)] float minAlpha = 0.55f;

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    ARPlane plane;
    MeshRenderer meshRenderer;
    MaterialPropertyBlock block;
    Color baseColor;
    float age;

    void Awake()
    {
        plane = GetComponent<ARPlane>();
        meshRenderer = GetComponent<MeshRenderer>();
        block = new MaterialPropertyBlock();
        baseColor = meshRenderer.sharedMaterial.GetColor(BaseColorId);
    }

    void Update()
    {
        // we only play on floors and tables, so ceilings and walls stay hidden
        meshRenderer.enabled = plane.alignment == PlaneAlignment.HorizontalUp;
        if (!meshRenderer.enabled) return;

        age += Time.deltaTime;
        float fade = Mathf.Clamp01(age / fadeInTime);
        float pulse = Mathf.Lerp(minAlpha, 1f, (Mathf.Sin(age * pulseSpeed) + 1f) * 0.5f);

        Color color = baseColor;
        color.a *= fade * pulse;

        meshRenderer.GetPropertyBlock(block);
        block.SetColor(BaseColorId, color);
        meshRenderer.SetPropertyBlock(block);
    }
}
