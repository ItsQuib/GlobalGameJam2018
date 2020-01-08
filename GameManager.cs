
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Transform[] playerSpawners;

    public PlayerController playerPrefab;

    public GameObject[] allPlayers;

    int assignedGamepads;

    public static GameManager instance;

    public List<Enemy> leftSideEnemies = new List<Enemy>();
    public List<Enemy> rightSideEnemies = new List<Enemy>();

    public NumberOfPlayers numberOfPlayers;

    public bool isAsteroidLevel;

    public enum NumberOfPlayers
    {
        TWO_PLAYER,
        FOUR_PLAYER
    }

    private void Awake()
    {
        instance = this;
    }

    public CabinetController[] cabinetControllers = new CabinetController[0];

    void Start()
    {               
        if (numberOfPlayers == NumberOfPlayers.TWO_PLAYER)
        {
            allPlayers = new GameObject[2];
        }
        if (numberOfPlayers == NumberOfPlayers.FOUR_PLAYER)
        {
            allPlayers = new GameObject[4];
        }
        SpawnPlayers();

        cabinetControllers = new CabinetController[FindObjectsOfType<CabinetController>().Length];
        cabinetControllers = FindObjectsOfType<CabinetController>();
    }

   

    void SpawnPlayers()
    {
        switch (numberOfPlayers)
        {
            case NumberOfPlayers.TWO_PLAYER:
                for (int i = 0; i < 2; i++)
                {
                    PlayerController newPlayer = Instantiate(playerPrefab, playerSpawners[i].position, playerSpawners[i].rotation);
                    AssignPlayerToController(newPlayer);
                }
                break;
            case NumberOfPlayers.FOUR_PLAYER:
                for (int i = 0; i < 4; i++)
                {
                    PlayerController newPlayer = Instantiate(playerPrefab, playerSpawners[i].position, playerSpawners[i].rotation);
                    AssignPlayerToController(newPlayer);
                }
                break;
        }
    }


    void AssignPlayerToController(PlayerController newPlayer)
    {        
        newPlayer.SetPlayerToGamepad(assignedGamepads);
        allPlayers[assignedGamepads] = newPlayer.gameObject;
        assignedGamepads++;
    }

    public void FreezeLeftSideEnemies(bool active)
    {
        if (isAsteroidLevel) { return; }
        if (!active)
        {
            for (int i = 0; i < leftSideEnemies.Count; i++)
            {
                leftSideEnemies[i].FreezeMovement();
            }
        }
        else
        {
            for (int i = 0; i < leftSideEnemies.Count; i++)
            {
                leftSideEnemies[i].StartMovement();
            }
        }
    }

    public void FreezeLeftSideCabinets(bool active)
    {
        if (!active)
        {
            foreach (CabinetController cabinetController in cabinetControllers)
            {
                if (cabinetController.transform.position.x < 0)
                    cabinetController.GetComponentInChildren<SpriteRenderer>().color = Color.black;
            }
        }
        else
        {
            foreach (CabinetController cabinetController in cabinetControllers)
            {
                if (cabinetController.transform.position.x < 0)
                    cabinetController.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }
    }

    public void FreezeRightSideEnemies(bool active)
    {
        if (isAsteroidLevel) { return; }
        if (!active)
        {
            for (int i = 0; i < rightSideEnemies.Count; i++)
            {
                rightSideEnemies[i].FreezeMovement();
            }

            foreach (CabinetController cabinetController in cabinetControllers)
            {
                if (cabinetController.transform.position.x >= 0)
                    cabinetController.GetComponentInChildren<SpriteRenderer>().color = Color.black;
            }
        }
        else
        {
            for (int i = 0; i < rightSideEnemies.Count; i++)
            {
                rightSideEnemies[i].StartMovement();
            }

            foreach (CabinetController cabinetController in cabinetControllers)
            {
                if (cabinetController.transform.position.x >= 0)
                    cabinetController.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }
    }

    public void FreezeRightSideCabinets(bool active)
    {        
        if (!active)
        {
            foreach (CabinetController cabinetController in cabinetControllers)
            {
                if (cabinetController.transform.position.x >= 0)
                    cabinetController.GetComponentInChildren<SpriteRenderer>().color = Color.black;
            }
        }
        else
        {
            foreach (CabinetController cabinetController in cabinetControllers)
            {
                if (cabinetController.transform.position.x >= 0)
                    cabinetController.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }
    }

    public void WinGame(int winner) {

        UIController.instance.WinPanel(true, winner);
        Invoke("RestartCurrentScene", 3f);
    }
    

    public void RestartCurrentScene()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        Time.timeScale = 1.0F;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);        
    }

    public void Quit()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        //Application.Quit();
    }

    
}
