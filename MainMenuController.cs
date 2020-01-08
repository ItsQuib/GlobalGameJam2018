using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public GameObject quitImage;
    public GameObject start2pImage;
    public GameObject starteSpaceImage;
    
    private Button quitButton;
    private Button start2pButton;
    private Button startSpaceButton;

    public static int players = 2;
       

    void Start() {

        quitButton = quitImage.GetComponent<Button>();

        start2pButton = start2pImage.GetComponent<Button>();
        startSpaceButton = starteSpaceImage.GetComponent<Button>();

        start2pButton.onClick.AddListener(Load2p);
        startSpaceButton.onClick.AddListener(Load4p);

        quitButton.onClick.AddListener(Quit);

    }

    public void Select2PButton()
    {
        start2pButton.GetComponent<Selectable>().Select();
    }

    public void SelectSpaceGame()
    {
        startSpaceButton.GetComponent<Selectable>().Select();
    }

    public void Quit() {
        Application.Quit();
    }

    public void Load2p() {

        SceneManager.LoadScene(1);
        
    }

    public void Load4p() {

        SceneManager.LoadScene(2);

    }
   

}