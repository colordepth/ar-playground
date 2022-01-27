using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class RedGreenBinUI : MonoBehaviour
{
    public enum Mode
    {
        PLACEBASKET,
        PLACEOBJECT,
        PICKUP
    }

    public static RedGreenBinUI instance;

    public Mode mode = Mode.PLACEBASKET;

    public GameObject initializePanel;
    public GameObject placeObjectsPanel;
    public GameObject startGamePanel;
    public GameObject exitPanel;
    public GameObject successPanel;
    public GameObject failurePanel;
    public GameObject winPanel;
    public Image inventory;
    public Sprite inventoryDefaultSprite;
    public Text debugText;

    void Start()
    {
        instance = this;
        initializePanel.SetActive(true);
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    public void OnClickBack()
    {
        exitPanel.SetActive(true);
    }

    public void OnClickContinue()
    {
        initializePanel.SetActive(false);
        placeObjectsPanel.SetActive(false);
        startGamePanel.SetActive(false);
        exitPanel.SetActive(false);
        successPanel.SetActive(false);
        failurePanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        instance = null;
        SceneManager.LoadScene("HomeScreen");
    }
}
