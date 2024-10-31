using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    private string fileName = "/history.json";

    public List<OrderItem> foodItems = new List<OrderItem>();
    public List<OrderItem> drinkItems = new List<OrderItem>();

    public Orders orders = new Orders(new List<Order>());

    public GameObject hangerPrefab;
    public Queue<GameObject> hangers = new Queue<GameObject>();

    public List<Transform> hangerSpot = new List<Transform>();
    public List<bool> occupiedHanger = new List<bool>();

    public static HistoryManager instance;

    public GameObject confetti;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {  instance = this; }

        LoadHistory();
    }

    private void Start()
    {
        StartCoroutine(CatchUpOnHistory());
    }

    private IEnumerator CatchUpOnHistory()
    {
        foreach (Order order in orders.orders)
        {
            yield return new WaitForSeconds(0.3f);
            SpawnHanger(order.foodItem.itemName, order.drinkItem.itemName);
        }

        while (orders.orders.Count < 3)
        {
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(generateOrder());
        }

        yield break;
    }

    public void LoadHistory()
    {
        string filePath = Application.persistentDataPath + fileName;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath, Encoding.UTF8);

            try
            {
                orders = JsonUtility.FromJson<Orders>(json);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to parse JSON: " + ex.Message);
            }
        }
    }

    public void SaveHistory()
    {
        string filePath = Application.persistentDataPath + fileName;
        string json = JsonUtility.ToJson(orders, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Data Saved to " + filePath);
    }

    private IEnumerator generateOrder()
    {
        OrderItem food = foodItems[Random.Range(0, foodItems.Count)];
        OrderItem drink = drinkItems[Random.Range(0, drinkItems.Count)];

        orders.orders.Add(new Order(food, drink));

        yield return new WaitForSeconds(GameManager.instance.secondsOfStudying / 3.0f);

        SpawnHanger(food.itemName, drink.itemName);
    }

    private void SpawnHanger(string food, string drink)
    {
        int i = 0;

        while (true)
        {
            i = Random.Range(0, hangerSpot.Count);
            if (occupiedHanger[i] == false)
            {
                break;
            }
        }

        Transform t = hangerSpot[i];
        occupiedHanger[i] = true;
        GameObject hanger = Instantiate(hangerPrefab, t.position, Quaternion.identity, null);
        hanger.transform.parent = t;
        TextMeshProUGUI text = hanger.GetComponentInChildren<TextMeshProUGUI>();
        text.text = string.Format("- {0} \n - {1}", drink, food);
        hangers.Enqueue(hanger);
    }

    public Order fetchCurrentOrder()
    {
        return orders.orders[0];
    }

    public IEnumerator FinalizeOrder()
    {
        orders.orders.RemoveAt(0);

        // Hanger
        GameObject hanger = hangers.Peek();

        yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
        GameObject conf = Instantiate(confetti, hanger.transform.position, Quaternion.identity, null);
        Destroy(conf, 3.5f);

        hangers.Dequeue();
        Destroy(hanger);

        for (int i = 0; i < hangerSpot.Count; i++)
        {
            if (hangerSpot[i].childCount == 0)
            {
                occupiedHanger[i] = false;
            }
        }

        yield return new WaitForSeconds(Random.Range(1, 3));

        StartCoroutine(generateOrder());
    }
}
