using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FantomLib;
using TMPro;

public class WheresPUI : MonoBehaviour
{
    public enum Mode
    {
        PLACEOBJECT,
        PICKUP
    }

    public static WheresPUI instance;

    public Mode mode = Mode.PLACEOBJECT;

    public GameObject initializePanel;
    public GameObject exitPanel;
    public GameObject successPanel;
    public GameObject failurePanel;
    public GameObject winPanel;
    public Image inventory;
    public Sprite inventoryDefaultSprite;
    public Text debugText;
    public TextMeshProUGUI infoText;

    public GameObject[] objects;
    
    [TextArea(3, 10)]
    public string[] riddles;

    public TextToSpeechController textToSpeechControl; // = new TextToSpeechController();

    void Start()
    {
        instance = this;
        initializePanel.SetActive(true);
        //Screen.orientation = ScreenOrientation.LandscapeLeft;

        textToSpeechControl.Locale = ""; // empty string means AndroidLocale.Default;

        RedGreenBinUI.instance.inventory.color = Color.blue;
    }

    public void Speak(string text)
    {
        if (textToSpeechControl != null)
        {
            textToSpeechControl.StartSpeech(text);
            infoText.text = text;
            debugText.text += "Speaking now: " + text + "\n";
        }
    }

    public void OnSpeakStart()
    {
        debugText.text += "Speech started\n";
    }

    public void OnSpeakStop(string message)
    {
        debugText.text += "Speech stopped\n";
        debugText.text += "Stop Message: " + message + "\n";
    }

    public void OnSpeakDone()
    {
        debugText.text += "Speech done\n";
    }

    public void OnStatus(string message)
    {
        debugText.text += message + "\n";
    }

    public void OnClickBack()
    {
        exitPanel.SetActive(true);
    }

    public void OnClickContinue()
    {
        if (initializePanel.activeSelf)
            inventory.color = Color.blue;

        initializePanel.SetActive(false);
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
