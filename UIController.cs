using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public GameObject player;
    public Slider playerOneSlider;
    public Slider playerTwoSlider;

    public Selectable exitPanelSelectedButton;

    public GameObject winPanel, exitPanel;
    public Text winPlayer;
    public static UIController instance;

    private PlayerController playerScript;

    void Start() {
        playerScript = FindObjectOfType<PlayerController>();
    }

    private void Awake()
    {
        instance = this;
    }

    public void WinPanel(bool active, int winner)
    {
        if (winner == 1)
        {
            winPlayer.text = "PLAYER 1 TRANSMITTED";
        }
        else
        {
            winPlayer.text = "PLAYER 2 TRANSMITTED";
        }
        winPanel.SetActive(active);
    }

    private void Update()
    {
        PauseCheck();

        

    }

    public void PauseCheck()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (Time.timeScale == 1.0F)
            {
                Time.timeScale = 0.0F;
                exitPanel.SetActive(true);
                exitPanelSelectedButton.Select();
            }
            else
            {
                Time.timeScale = 1.0F;
                exitPanel.SetActive(false);
            }
        }
    }
}