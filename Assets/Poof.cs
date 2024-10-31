using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Poof : MonoBehaviour
{
    public Animator animator;
    public float animationTime = 1.0f;
    public GameObject poof;
    public Transform drinkSpawn, targetOnPlate;

    public List<GameObject> drinksPrefab = new List<GameObject>();

    public float speed = 2f;  // Speed of movement
    public float height = 2f;  // Height of the curve

    public IEnumerator PoofPoof()
    {
        poof.SetActive(true);
        animator.SetTrigger("Play");
        yield return new WaitForSeconds(animationTime / 2);

        OrderItem drink = HistoryManager.instance.fetchCurrentOrder().drinkItem;
        GameObject prefab = drinksPrefab.FirstOrDefault(d => d.name == drink.prefabName);

        GameObject drinkItem = null;

        if (prefab == null)
        {
            Debug.LogError("Couldnt find drink: " + drink.prefabName);
        }
        else
        {
            drinkItem = Instantiate(prefab, drinkSpawn);

            yield return new WaitForSeconds(animationTime / 2);

            drinkItem.transform.parent = targetOnPlate;
        }     
        poof.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        drinkItem.transform.parent = targetOnPlate;

        Vector3 startPosition = drinkItem.transform.position;
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

            drinkItem.transform.position = newPosition;

            if (Vector3.Distance(newPosition, targetPosition) < 0.1f)
            {
                elapsedTime = journeyTime;
                break;
            }

            yield return null; // Wait for the next frame
        }

        // Ensure the food item ends exactly at the target position
        drinkItem.transform.position = targetOnPlate.position;

        Destroy(drinkItem, 3.5f);
    }
}
