using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OvenPoof : MonoBehaviour
{
    public Transform foodSpawn, targetOnPlate;
    public DomeAnimation domeAnimation;
    public List<GameObject> foodPrefabs = new List<GameObject>();

    public float speed = 2f;  // Speed of movement
    public float height = 2f;  // Height of the curve

    public IEnumerator PoofPoof()
    {
        OrderItem food = HistoryManager.instance.fetchCurrentOrder().foodItem;
        GameObject prefab = foodPrefabs.First(f => f.name == food.prefabName);

        if (prefab == null)
        {
            Debug.LogError("Couldnt find food: " + food.prefabName);
        }

        GameObject foodItem = Instantiate(prefab, foodSpawn);

        yield return new WaitForSeconds(5.0f);

        foodItem.transform.parent = targetOnPlate;

        Vector3 startPosition = foodItem.transform.position;
        Vector3 targetPosition = targetOnPlate.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float journeyTime = journeyLength / speed;
        float elapsedTime = 0f;

        while (elapsedTime < journeyTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / journeyTime;

            // Calculate the smooth curve using a quadratic function
            float heightOffset = height * (1 - Mathf.Pow(t - 0.5f, 2) * 4); // Peaks at t = 0.5
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * heightOffset;

            foodItem.transform.position = newPosition;

            if (Vector3.Distance(newPosition, targetPosition) < 0.1f)
            {
                elapsedTime = journeyTime;
                break;
            }

            yield return null; // Wait for the next frame
        }

        // Ensure the food item ends exactly at the target position
        foodItem.transform.position = targetOnPlate.position;

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(domeAnimation.Play());

        Destroy(foodItem, 2.0f);
    }
}
