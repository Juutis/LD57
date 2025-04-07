using UnityEngine;
using Unity.Cinemachine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;
    private CinemachineCamera cam;

    private float triggered = -1;
    private float duration = 0.1f;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeAmplitude = 1.0f;

    void Awake() {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        noise = (CinemachineBasicMultiChannelPerlin)cam.GetCinemachineComponent(CinemachineCore.Stage.Noise);
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered > 0 && triggered > Time.time - duration) {
            var t = (Time.time - triggered) / duration;
            var amp = Mathf.Lerp(shakeAmplitude, 0.0f, t);
            noise.AmplitudeGain = amp;
        } else {
            noise.AmplitudeGain = 0.0f;
        }
    }

    public void Shake(float amplitude = 1.0f) {
        shakeAmplitude = amplitude;
        triggered = Time.time;
    }
}
