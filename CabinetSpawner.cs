using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinetSpawner : MonoBehaviour {

    public GameObject cabinet, socket;
    public bool isStartingCabinet;
    public float cableLength = 7.57f;

    void Start ()
    {
        GameObject newCab = Instantiate(cabinet, transform.position, transform.rotation);
        newCab.GetComponent<CabinetController>().IsStartingCabinet = isStartingCabinet;
        newCab.GetComponent<CabinetController>().cableLength = cableLength;
        Destroy(gameObject);
	}
	
}
