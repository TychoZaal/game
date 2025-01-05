using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public HistoryManager HistoryManager;

    [Tooltip("1500")]
    public int secondsOfStudying = 10;
    [Tooltip("300")]
    public int secondsOfBreak = 5;

    public float studyTime = 0;
    public float breakTime = 0;
    public float sessionTime = 0;
    public bool onBreak = false;
    public bool inMenu = true;
    public bool isSummarySession = true;

    public OvenAnimator animator;

    public Pulsate oven, coffee;
    public VisualEffect ovenSmoke, coffeeSmoke;
    public Poof poof;
    public OvenPoof ovenPoof;

    public GameObject blur, timers, summary;
    public TextMeshProUGUI sessionTimer;

    public Color activeColor, backgroundColor;
    public Image sessionBackground, allTimeBackground;
    public TextMeshProUGUI sessionButtonText, allTimeButtonText;

    public static GameManager instance;
 
    public TextMeshProUGUI hours, minutes, seconds;
    public TextMeshProUGUI hoursT, minutesT, secondsT;

    [Tooltip("0:25:0")]
    public double hoursS = 0, minutesS = 25, secondsS = 0;
    [Tooltip("0:5:0")]
    public double hoursB = 0, minutesB = 5, secondsB = 0;

    public List<SummaryItem> summaryItems = new List<SummaryItem>();
    public List<SummaryItem> summaryAllTimeItems = new List<SummaryItem>();

    // public TextMeshProUGUI timeText;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!onBreak)
        {
            if (studyTime < secondsOfStudying)
            {
                studyTime += Time.deltaTime;
            }
            else
            {
                StartCoroutine(StartBreak());
            }
        }
        else
        {
            if (breakTime < secondsOfBreak)
            {
                breakTime += Time.deltaTime;
            }
            else
            {
                StartCoroutine(DoneWithBreak());
            }
        }
        
        UpdateTimer();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        blur.SetActive(inMenu);

        UpdateSummaryView();
    }

    public void SwitchToSession()
    {
        isSummarySession = true;
    }

    public void SwitchToAllTime()
    {
        isSummarySession = false;
    }

        IEnumerator StartBreak()
    {
        studyTime = 0;
        onBreak = true;
        oven.shouldPulsate = false;
        coffee.shouldPulsate = false;
        ovenSmoke.Stop();
        coffeeSmoke.Stop();

        StartCoroutine(ovenPoof.PoofPoof());

        yield return new WaitForSeconds(1);
        animator.OpenDoor();

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.2f));
        StartCoroutine(poof.PoofPoof());
    }

    public void StartStudying()
    {
        inMenu = false;

        secondsOfStudying = (int)(secondsS + (minutesS * 60) + (hoursS * 3600));
        secondsOfBreak = (int)(secondsB + (minutesB * 60) + (hoursB * 3600));

        timers.SetActive(false);
    }

    IEnumerator DoneWithBreak()
    {
        breakTime = 0;
        onBreak = false;
        animator.CloseDoor();
        StartCoroutine(HistoryManager.FinalizeOrder());
        ovenSmoke.Play();
        coffeeSmoke.Play();

        yield return new WaitForSeconds(1);

        oven.shouldPulsate = true;
        coffee.shouldPulsate = true;
    }

    public void ChangeHours(int diff)
    {
        hoursS += diff;

        if (hoursS < 0) { hoursS = 0; }
    }

    public void ChangeMinutes(int diff)
    {
        minutesS += diff;

        if (minutesS < 0) { minutesS = 0; }
    }

    public void ChangeSeconds(int diff)
    {
        secondsS += diff;

        if (secondsS < 0) { secondsS = 0; }
    }

    public void ChangeHoursB(int diff)
    {
        hoursB += diff;

        if (hoursB < 0) { hoursB = 0; }
    }

    public void ChangeMinutesB(int diff)
    {
        minutesB += diff;

        if (minutesB < 0) { minutesB = 0; }
    }

    public void ChangeSecondsB(int diff)
    {
        secondsB += diff;

        if (secondsB < 0) { secondsB = 0; }
    }

    void UpdateTimer()
    {
        sessionTime += Time.deltaTime;
        HistoryManager.instance.orders.allTimeSessionCounter += Time.deltaTime;

        hours.text = hoursS.ToString("00") + ":";
        minutes.text = minutesS.ToString("00") + ":";
        seconds.text = secondsS.ToString("00");

        hoursT.text = hoursB.ToString("00") + ":";
        minutesT.text = minutesB.ToString("00") + ":";
        secondsT.text = secondsB.ToString("00");

        float timeToDisplay = isSummarySession ? sessionTime : HistoryManager.instance.orders.allTimeSessionCounter;
        var time = TimeSpan.FromSeconds(timeToDisplay);
        sessionTimer.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
            time.Hours,
            time.Minutes,
            time.Seconds);
    }

    public void QuitGame()
    {
        HistoryManager.SaveHistory();

#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Pause()
    {
        inMenu = !inMenu;
        summary.SetActive(!timers.activeSelf && isSummarySession);
    }

    private void UpdateSummaryView()
    {
        sessionBackground.color = isSummarySession ? activeColor : backgroundColor;
        sessionButtonText.color = isSummarySession ? backgroundColor : activeColor;

        allTimeBackground.color = !isSummarySession ? activeColor : backgroundColor;
        allTimeButtonText.color = !isSummarySession ? backgroundColor : activeColor;

        // Reset all counts
        foreach (var summaryItem in summaryItems)
        {
            summaryItem.UpdateDisplay(0);
        }

        var orders = isSummarySession ? HistoryManager.instance.sessionHistory : HistoryManager.instance.orders.allTimeHistory;

        if (orders != null && orders.Count > 0)
        {
            var foodCounts = orders
                .GroupBy(o => o.foodItem.itemName)
                .Select(g => new { FoodName = g.Key, Count = g.Count() }).ToList();

            var drinkCounts = orders
                .GroupBy(o => o.drinkItem.itemName)
                .Select(g => new { DrinkName = g.Key, Count = g.Count() }).ToList();

            foreach (var food in foodCounts)
            {
                try
                {
                    var foodItem = summaryItems.First(item => item.itemName == food.FoodName);
                    foodItem.UpdateDisplay(food.Count);
                }
                catch
                {
                    Debug.LogError("Unable to find: " + food.FoodName);
                }
            }

            foreach (var drink in drinkCounts)
            {
                try
                {
                    var foodItem = summaryItems.First(item => item.itemName == drink.DrinkName);
                    foodItem.UpdateDisplay(drink.Count);
                }
                catch {
                    Debug.LogError("Unable to find: " + drink.DrinkName);
                }                
            }
        }
    }
}
