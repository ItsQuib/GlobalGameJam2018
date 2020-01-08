using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    Rigidbody2D rb;

    bool IsFollowingCharacter = false;    
    Vector2 Target;
    public float moveSpeed = 1f;
    int type;
    public Animator animator;

    float minimumDistaneToAvoidCable = 1f;

    GameObject nearestPlayer;
    LevelStart levelStart;

	// Use this for initialization
	void Start ()
    {
        levelStart = FindObjectOfType<LevelStart>();
        rb = GetComponent<Rigidbody2D>();
        //animator.SetFloat(0, Random.Range(0, 1));

        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length); //Set a random part of the animation to start from
        animator.Play("EnemyAmination", 0, randomIdleStart);
    }

    // how long it stops and doesn't move for
    public float stopTime = 1f;
    // how long it moves for before stopping
    public float moveTime = 2f;

    bool movementFrozen;

    void Update()
    {
        if (levelStart.gamePlaying)
        {
            Movement();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void Movement()
    {
        rb.velocity = Target * moveSpeed;
        rb.velocity = new Vector2(rb.velocity.x + rb.velocity.x, rb.velocity.y + rb.velocity.y);
    }

    public void FreezeMovement()
    {
        movementFrozen = true;
        StopCoroutine("RandomMovement");
        ChooseNoDirection();
    }

    public void StartMovement()
    {
        movementFrozen = false;
        StartCoroutine("RandomMovement");
    }


    IEnumerator RandomMovement()
    {
        if (!movementFrozen)
        {
            AvoidNearestCablePoint(); 
            yield return new WaitForSeconds(moveTime / 2);
            ChooseNoDirection();
            yield return new WaitForSeconds(Random.Range(0f, stopTime));
            //ChoosePlayerDirection();
            AvoidNearestCablePoint();
            yield return new WaitForSeconds(moveTime);
            ChooseNoDirection();
            yield return new WaitForSeconds(Random.Range(0f, stopTime));
            StartCoroutine("RandomMovement");
        }
    }

    public void ChooseRandomDirection()
    {
        Target = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));        
    }

    public void ChooseNoDirection()
    {
        Target = new Vector2(0, 0);
    }

    public void ChoosePlayerDirection()
    {
        Target = (FindNearestPlayer().transform.position - transform.position).normalized;
    }

    public void ChooseOppositeDirection(Vector3 dirToAvoid)
    {
        Target = (transform.position - dirToAvoid).normalized;
        //Target = Rotate(Target, Random.Range(-90, 90));
    }

    public Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public GameObject FindNearestPlayer()
    {
        float dist = Mathf.Infinity;
        for (int i = 0; i < GameManager.instance.allPlayers.Length; i++)
        {
            float thisDist = Vector3.Distance(transform.position, GameManager.instance.allPlayers[i].transform.position);
            if (thisDist < dist)
            {
                dist = thisDist;
                nearestPlayer = GameManager.instance.allPlayers[i];
            }
        }
        return nearestPlayer;
    }
        
    void AvoidNearestCablePoint()
    {
        List<Vector3> cablePositions = new List<Vector3>();
        for (int i = 0; i < GameManager.instance.allPlayers.Length; i++)
        {
            if (GameManager.instance.allPlayers[i])
            {
                Vector3[] asd = new Vector3[GameManager.instance.allPlayers[i].GetComponent<PlayerController>().CableLineRendered.positionCount];
                GameManager.instance.allPlayers[i].GetComponent<PlayerController>().CableLineRendered.GetPositions(asd);
                cablePositions.AddRange(asd);
            }
        }

        float minDist = minimumDistaneToAvoidCable;
        Vector3 cablePointToAvoid = new Vector3();
        for(int i = 0; i < cablePositions.Count; i++)
        {
            float dist = Vector3.Distance(cablePositions[i], transform.position);
            if(dist < minDist)
            {
                minDist = dist;
                cablePointToAvoid = cablePositions[i];
            }
        }
        if(minDist < minimumDistaneToAvoidCable)
        {
            ChooseOppositeDirection(cablePointToAvoid);
        }
        else
        {
            ChooseRandomDirection();
        }
    }


}