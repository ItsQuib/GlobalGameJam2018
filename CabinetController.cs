using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinetController : MonoBehaviour {

    public float cableLength = 7.3f;

    bool cabinetActivated;

    public bool IsStartingCabinet = false;

    public GameObject connectedParticles;

    public AudioSource source;
    public SpriteRenderer spriteRend;

    public AudioClip connectSound;
    public Sprite connectedSprite;



    public void SetCabinetCableLength(float length)
    {
        cableLength = length;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cabinetActivated) { return; }
        if(collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerController>().hasCable || IsStartingCabinet)
            {
                if (IsStartingCabinet)
                {
                    cabinetActivated = true;
                }
                else
                {
                    cabinetActivated = !collision.gameObject.GetComponent<PlayerController>().canReuseCabinets;
                }
                collision.gameObject.GetComponent<PlayerController>().AttachCable(this.transform, cableLength, IsStartingCabinet);
                ConnectCabinetEffects();
            }
        }
    }

    void ConnectCabinetEffects()
    {
        connectedParticles.SetActive(false);
        spriteRend.sprite = connectedSprite;
        source.PlayOneShot(connectSound, 1f);
    }


}
