using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    int CurrentField = 0;
    int CurrentField2 = -1;

    public Vector2 FieldSize = Vector2.one;

    public List<Transform> leftSpawnPositions, rightSpawnPositions;    

    public GameObject Enemy;
    public GameObject[] asteroids;
        

    public int startingEnemyNumberEachSide = 80;    

    public float MaxSpawnTime = 3f;
    public float MinSpawnTime = 3f;
    public float MaxDifficultyTime = 30f;
    float MaxDifficultyTimer = 0f;
    public float SpawnTimeRandomization = 0f;
    float SpawnTimer = 0f;
    float CurrentSpawnTime = 3f;
    public float IntialSpawnTime = 4f;

    void Start()
    {
        CurrentSpawnTime = IntialSpawnTime;

        leftSpawnPositions.Clear();
        int count = transform.Find("LeftSideSpawners").childCount;
        for (int i = 0; i < count; i++)
        {
            leftSpawnPositions.Add(transform.Find("LeftSideSpawners").GetChild(i));
        }
        rightSpawnPositions.Clear();
        count = transform.Find("RightSideSpawners").childCount;
        for (int i = 0; i < count; i++)
        {
            rightSpawnPositions.Add(transform.Find("RightSideSpawners").GetChild(i));
        }


        for (int i = 0; i < startingEnemyNumberEachSide; i++)
        {
            if (!GameManager.instance.isAsteroidLevel)
            {
                SpawnEnemy();
            }
            else
            {
                SpawnAsteroid();
            }

        }
    }
	
	/*
	void Update ()
    {
        MaxDifficultyTimer += Time.deltaTime;
        SpawnTimer += Time.deltaTime;
        
        if (SpawnTimer > CurrentSpawnTime)
        {
            SpawnEnemy();
            SpawnTimer -= CurrentSpawnTime;
            float NotRandomizedNewSpawnTime = Mathf.Lerp(MaxSpawnTime, MinSpawnTime, (MaxDifficultyTime / MaxDifficultyTimer));
            CurrentSpawnTime = Random.Range(NotRandomizedNewSpawnTime * (1f + SpawnTimeRandomization), NotRandomizedNewSpawnTime * (1f - SpawnTimeRandomization));
        }
    }*/

   

    void SpawnEnemy()
    {       
        GameObject leftSideENemy = Instantiate(Enemy, GrabEnemySpawner(true), transform.rotation);
        GameManager.instance.leftSideEnemies.Add(leftSideENemy.GetComponent<Enemy>());
        GameObject rightSideEnemy = Instantiate(Enemy, GrabEnemySpawner(false), transform.rotation);
        GameManager.instance.rightSideEnemies.Add(rightSideEnemy.GetComponent<Enemy>());        
    }


    void SpawnAsteroid()
    {
            GameObject randAsteroid = asteroids[Random.Range(0, asteroids.Length)];
            GameObject leftSideENemy = Instantiate(randAsteroid, GrabEnemySpawner(true), transform.rotation);
            GameManager.instance.leftSideEnemies.Add(leftSideENemy.GetComponent<Enemy>());
            GameObject rightSideEnemy = Instantiate(randAsteroid, GrabEnemySpawner(false), transform.rotation);
            GameManager.instance.rightSideEnemies.Add(rightSideEnemy.GetComponent<Enemy>());
        }
   

    Vector3 GrabEnemySpawner(bool leftSide)
    {
        if (leftSide)
        {
            return leftSpawnPositions[Random.Range(0, leftSpawnPositions.Count)].position;
        }
        else
        {
            return rightSpawnPositions[Random.Range(0, rightSpawnPositions.Count)].position;
        }
    }
    

}
