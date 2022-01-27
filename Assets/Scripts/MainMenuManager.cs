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
        SceneManager.LoadScene("RedGreenBin");
    }

    public void OnClickWheresTheButton()
    {
        SceneManager.LoadScene("WheresP");
    }

    public void OnClickFunWithNumbers()
    {

    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
