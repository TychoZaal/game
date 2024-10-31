using UnityEngine;

public class Pulsate2 : MonoBehaviour
{
    public Pulsate pulsate;

    public float speed = 1.0f;  // Speed of pulsation
    public float intensity = 0.5f; // Max size change (0.5 means it can go to 1.5x size)

    private Vector3 originalScale;

    void Start()
    {
        // Store the original scale of the object
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (!pulsate.shouldPulsate || pulsate.isJumping)
            return;

        // Calculate the scale based on a sine wave
        float scaleFactor = 1 + Mathf.Sin(Time.time * speed) * intensity;

        // Apply the new scale
        transform.localScale = originalScale * scaleFactor;
    }
}
