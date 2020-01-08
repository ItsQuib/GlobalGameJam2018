using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableSourceController : MonoBehaviour {

    public GameObject cableEnd;
	
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //collision.GetComponent<PlayerController>().AttachCable(cableEnd);
        }       
    }
    
}
