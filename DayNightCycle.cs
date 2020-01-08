using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour {

    public float leastRandomNumber;
    public float mostRandomNumber;

    public GameObject leftField;
    public GameObject rightField;

//    public Text timeTillSwitchText;
    public Text gameTimerText;
    public float gameTimer;

    private bool leftFieldDark = false;
    private bool rightFieldDark = true;

    private float timeVar;
    private float secondsTillSwitch;

    LevelStart levelStart;

    private SpriteRenderer leftSpriteRenderer;
    private SpriteRenderer rightSpriteRenderer;

    private Color blackColor = new Color(255, 255, 255, 255);
    private Color whiteColor = new Color(0, 0, 0, 255);
    //    private Animator topAnim;
    //    private Animator botAnim;

    void Start () {

        /*        if(topFieldLit && botFieldLit) {

                    botFieldLit = false;

                }*/

        //        topAnim = topAnimParent.GetComponent<Animator>();
        //        botAnim = botAnimParent.GetComponent<Animator>();

        levelStart = FindObjectOfType<LevelStart>();

        leftSpriteRenderer = leftField.GetComponent<SpriteRenderer>();
        rightSpriteRenderer = rightField.GetComponent<SpriteRenderer>();


        if (leftFieldDark) {

            leftSpriteRenderer.color = blackColor;
            rightSpriteRenderer.color = whiteColor;            

        } else {

            leftSpriteRenderer.color = whiteColor;
            rightSpriteRenderer.color = blackColor;           

        }
        secondsTillSwitch = Random.Range(leastRandomNumber, mostRandomNumber);
        timeVar = Time.time + secondsTillSwitch;

        //timeTillSwitchText.text = secondsTillSwitch.ToString();
        gameTimerText.text = gameTimer.ToString();
        GameManager.instance.FreezeLeftSideEnemies(true);
        GameManager.instance.FreezeRightSideEnemies(true);
    }

    public void StartTimer()
    {
        secondsTillSwitch = Random.Range(leastRandomNumber, mostRandomNumber);
        timeVar = Time.time + secondsTillSwitch;
    }

    void Update () {

        if (!levelStart.gamePlaying)
        {
            return;
        }

        secondsTillSwitch -= Time.deltaTime;

//        timeTillSwitchText.text = Mathf.Round(secondsTillSwitch).ToString();


        gameTimer += Time.deltaTime;
        gameTimerText.text = Mathf.Round(gameTimer).ToString();

        if(Time.time > timeVar) {

            switchFields(false);

            secondsTillSwitch = Random.Range(leastRandomNumber, mostRandomNumber);

            timeVar = timeVar + secondsTillSwitch;

        }
	}

    public void switchFieldsExternal() {
        /*
        switchFields();

        secondsTillSwitch = Random.Range(leastRandomNumber, mostRandomNumber);
        timeVar = timeVar + secondsTillSwitch;
        */
    }

    public void startupFlashes(int actionNum) {

        switch (actionNum) {

            case 1:
                leftSpriteRenderer.color = whiteColor;
                rightSpriteRenderer.color = whiteColor;
                break;
            case 2:
                leftSpriteRenderer.color = blackColor;
                rightSpriteRenderer.color = whiteColor;
                break;
            case 3:
                leftSpriteRenderer.color = blackColor;
                rightSpriteRenderer.color = blackColor;
                break;
            case 4:
                if (leftFieldDark) {
                    leftSpriteRenderer.color = whiteColor;
                    rightSpriteRenderer.color = blackColor;
                } else {
                    leftSpriteRenderer.color = blackColor;
                    rightSpriteRenderer.color = whiteColor;
                }
                break;
        }
    }

    public void switchFields(bool fake) {

        if(!fake) {

            if (leftFieldDark) {
                leftFieldDark = !leftFieldDark;
                leftSpriteRenderer.color = whiteColor;
            } else {
                leftFieldDark = !leftFieldDark;
                leftSpriteRenderer.color = blackColor;
            }
            if (rightFieldDark) {
                rightFieldDark = !rightFieldDark;
                rightSpriteRenderer.color = whiteColor;
            } else {
                rightFieldDark = !rightFieldDark;
                rightSpriteRenderer.color = blackColor;
            }

            GameManager.instance.FreezeLeftSideEnemies(leftFieldDark);
            GameManager.instance.FreezeRightSideEnemies(rightFieldDark);
            GameManager.instance.FreezeLeftSideCabinets(leftFieldDark);
            GameManager.instance.FreezeRightSideCabinets(rightFieldDark);

        }
        else {

            if (leftFieldDark) {
                leftFieldDark = !leftFieldDark;
                leftSpriteRenderer.color = whiteColor;
            } else {
                leftFieldDark = !leftFieldDark;
                leftSpriteRenderer.color = blackColor;
            }
            if (rightFieldDark) {
                rightFieldDark = !rightFieldDark;
                rightSpriteRenderer.color = whiteColor;
            } else {
                rightFieldDark = !rightFieldDark;
                rightSpriteRenderer.color = blackColor;
            }

            GameManager.instance.FreezeLeftSideCabinets(leftFieldDark);
            GameManager.instance.FreezeRightSideCabinets(rightFieldDark);
        }
    }
}