using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }
    public void OnClickDoItYourself()
    {
        SceneManager.LoadScene("Minecraft");
    }

    public void OnClickRedGreen()
    {

    }

    public void OnClickWheresTheButton()
    {

    }

    public void OnClickFunWithNumbers()
    {

    }

    public void OnClickQuit()
    {

    }
}
