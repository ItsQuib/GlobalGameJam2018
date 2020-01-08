using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStart : MonoBehaviour {

    public Text timerText;

    public DayNightCycle dayNightScript;

    public GameObject gamepadLayout_WB;
    public GameObject gamepadLayout_BW;

    private bool gameStarted = false;
    public bool gamePlaying = false;

    void Start()
    {
    }

    void Update() {
        if (!gameStarted) {

            gameStarted = true;

            StartCoroutine(Countdown());

        }
    }

    private IEnumerator Countdown()
    {
        //Time.timeScale = 0.0f;
        gamepadLayout_BW.SetActive(true);
        timerText.text = "3";
        dayNightScript.switchFields(true);
        yield return new WaitForSeconds(1f);
        gamepadLayout_BW.SetActive(false);
        gamepadLayout_WB.SetActive(true);
        timerText.text = "2";
        dayNightScript.switchFields(true);
        yield return new WaitForSeconds(1f);
        gamepadLayout_WB.SetActive(false);
        gamepadLayout_BW.SetActive(true);
        timerText.text = "1";
        dayNightScript.switchFields(true);
        yield return new WaitForSeconds(1f);
        gamepadLayout_BW.SetActive(false);
        gamepadLayout_WB.SetActive(true);
        timerText.text = "TRANSMIT";
        dayNightScript.switchFields(true);
        yield return new WaitForSeconds(1f);
        gamepadLayout_WB.SetActive(false);
        timerText.enabled = false;
        Time.timeScale = 1;
        dayNightScript.StartTimer();
        gamePlaying = true;
    }
}