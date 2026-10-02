using TMPro;
using UnityEngine;

// Small frame rate readout in the corner, used to measure performance on the phone.
// Shows the current FPS plus the average and lowest FPS of the current round.
// Untick "Show" to hide it for the final video.
public class FpsCounter : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] bool show = true;
    [SerializeField] float refreshTime = 0.5f;

    float timer;
    int frames;

    // round stats
    float totalTime;
    int totalFrames;
    float lowestFps = float.MaxValue;
    bool firstReading = true;

    void OnEnable() => GameEvents.RoundStarted += ResetStats;
    void OnDisable() => GameEvents.RoundStarted -= ResetStats;

    void Start() => label.gameObject.SetActive(show);

    void Update()
    {
        if (!show) return;

        float dt = Time.unscaledDeltaTime;
        timer += dt;
        frames++;
        totalTime += dt;
        totalFrames++;

        if (timer < refreshTime) return;

        float fps = frames / timer;

        // the first reading still has the loading hitch in it, so it's left out of the minimum
        if (firstReading) firstReading = false;
        else lowestFps = Mathf.Min(lowestFps, fps);

        float shownMin = lowestFps == float.MaxValue ? fps : lowestFps;
        float average = totalFrames / totalTime;
        label.text = $"{fps:0} FPS  ·  {1000f / fps:0.0} ms\nAVG {average:0}  ·  MIN {shownMin:0}";

        timer = 0f;
        frames = 0;
    }

    void ResetStats()
    {
        totalTime = 0f;
        totalFrames = 0;
        lowestFps = float.MaxValue;
        firstReading = true;
    }
}
