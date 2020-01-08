using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour {

    Rigidbody2D rb;

    public float maxSpinspeed;

    public float startMoveSpeed = 1f;
	
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        float randVel = Random.Range(-maxSpinspeed, maxSpinspeed);
        rb.angularVelocity = randVel;
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.AddForce(randomDirection * startMoveSpeed, ForceMode2D.Impulse);
	}
	
}
