using UnityEngine;

[RequireComponent(typeof(Light))]
public class CandleFlicker : MonoBehaviour
{
    [Header("Intensity Settings")]
    [Tooltip("The lowest brightness the candle will reach.")]
    public float minIntensity = 0.5f;

    [Tooltip("The highest brightness the candle will reach.")]
    public float maxIntensity = 1.0f;

    [Header("Timing & Smoothness")]
    [Tooltip("How fast the light moves towards the new intensity (Higher = Sharper flicker, Lower = Lazier flame).")]
    public float smoothingSpeed = 10f;

    [Tooltip("Minimum time to wait before changing targets.")]
    public float minInterval = 0.05f;

    [Tooltip("Maximum time to wait before changing targets.")]
    public float maxInterval = 0.2f;

    // Internal state variables
    private Light myLight;
    private float targetIntensity;
    private float flickerTimer;

    void Awake()
    {
        myLight = GetComponent<Light>();
        
        // Start with a valid target
        targetIntensity = myLight.intensity;
    }

    void Update()
    {
        // 1. Move the current intensity gradually towards the target intensity
        myLight.intensity = Mathf.Lerp(myLight.intensity, targetIntensity, Time.deltaTime * smoothingSpeed);

        // 2. Handle the Timer
        flickerTimer -= Time.deltaTime;

        // 3. Time to pick a new target?
        if (flickerTimer <= 0)
        {
            // Pick a new random brightness target
            targetIntensity = Random.Range(minIntensity, maxIntensity);

            // Reset the timer with a random duration
            flickerTimer = Random.Range(minInterval, maxInterval);
        }
    }
}