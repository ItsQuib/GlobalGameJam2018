using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

public class SocketController : MonoBehaviour {

    int playersTouchingSocket = 0;
    bool playerOneTouchingSocket = false;
    bool playerTwoTouchingSocket = false;

    public AudioSource source;
    public SpriteRenderer spriteRend;

    public GameObject connectedParticles;

    public AudioClip connectSound;
    public Sprite connectedSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {

            collision.gameObject.GetComponent<PlayerController>().Finished();

            if (collision.gameObject.transform.position.x > 0) {
                SocketConnected(2);

            } else {

                SocketConnected(1);

            }
            
            //Destroy(gameObject);
            //playersTouchingSocket++;
        }        
    }
    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {

            if (collision.gameObject.transform.position.x > 0) {

                playerOneTouchingSocket = false;

            } else {

                playerTwoTouchingSocket = false;

            }
            //            playersTouchingSocket++;
        }
    }*/


    void SocketConnected(int winner)
    {
        connectedParticles.SetActive(false);
        spriteRend.sprite = connectedSprite;
        source.PlayOneShot(connectSound, 1f);
        GameManager.instance.WinGame(winner);
    }

}
