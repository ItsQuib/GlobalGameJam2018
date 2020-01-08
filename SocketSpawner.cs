using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketSpawner : MonoBehaviour {

    public GameObject socket;


	void Start ()
    {
        Instantiate(socket, transform.position, transform.rotation);
        Destroy(gameObject);
	}
	
}
