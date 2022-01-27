using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class RedGreenBinUI : MonoBehaviour
{
    public static RedGreenBinUI instance;

    public GameObject initializePanel;
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
    }

    public void OnClickBack()
    {
        exitPanel.SetActive(true);
    }

    public void OnClickContinue()
    {
        initializePanel.SetActive(false);
        exitPanel.SetActive(false);
        successPanel.SetActive(false);
        failurePanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("HomeScreen");
    }
}
