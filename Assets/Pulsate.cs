using UnityEngine;
using System.Collections;

public class Pulsate : MonoBehaviour
{
    public float jumpHeight = 1.0f; // Height of the jump
    public float jumpDuration = 0.5f; // Duration of the jump
    public float minWaitTime = 1.0f; // Minimum wait time between jumps
    public float maxWaitTime = 3.0f; // Maximum wait time between jumps
    public float rotationAmount = 10.0f; // Max rotation angle

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public bool isJumping = false;

    public bool shouldPulsate = true;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        StartCoroutine(JumpRoutine());
    }

    private IEnumerator JumpRoutine()
    {
        while (true)
        {
            // Wait for a random duration
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // Perform the jump
            if (!isJumping)
            {
                if (shouldPulsate)
                {
                    StartCoroutine(Jump());
                }
            }
        }
    }

    private IEnumerator Jump()
    {
        isJumping = true;

        // Store the target rotation
        Quaternion targetRotation = originalRotation * Quaternion.Euler(
            Random.Range(-rotationAmount, rotationAmount),
            Random.Range(-rotationAmount, rotationAmount),
            Random.Range(-rotationAmount, rotationAmount)
        );

        Vector3 targetPosition = originalPosition + new Vector3(0, jumpHeight, 0);
        float elapsedTime = 0f;

        // Jump upwards
        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;
            transform.position = Vector3.Lerp(originalPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Return to original position and rotation
        elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;
            transform.position = Vector3.Lerp(targetPosition, originalPosition, t);
            transform.rotation = Quaternion.Slerp(targetRotation, originalRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends exactly at the original position and rotation
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        isJumping = false;
    }
}
