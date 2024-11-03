using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    public OvenAnimator animator;

    public Pulsate oven, coffee;
    public VisualEffect ovenSmoke, coffeeSmoke;
    public Poof poof;
    public OvenPoof ovenPoof;

    public static GameManager instance;

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
            ExitGame();
        }
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

    void UpdateTimer()
    {
        sessionTime += Time.deltaTime;

        if (!onBreak)
        {
            TimeSpan t = TimeSpan.FromSeconds(secondsOfStudying - studyTime);

            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);

            // timeText.text = answer;
        }
    }

    void ExitGame()
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
}
