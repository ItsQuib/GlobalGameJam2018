using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{

    public GameObject spriteHolder, deathParticles;

    public void ShowPlayerDies()
    {            
        Instantiate(deathParticles, transform.position, transform.rotation);
       
    }

  
}
