using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class MinecraftUI : MonoBehaviour
{
    public enum Mode
    {
        PLACE,
        PICKUP
    }

    public enum Object
    {
        CUBE,
        CONE,
        CYLINDER
    }

    public Mode mode = Mode.PLACE;
    public Object objectType = Object.CUBE;

    public Text buttonText;
    public Text debugText;
    public Image HammerImage;
    public Image BombImage;

    public GameObject shapeMenu;
    public GameObject colorMainButton;
    public GameObject colorMenu;

    public Color objectColor = Color.yellow;

    private Color colorInactive = new Color(0.34f, 0.34f, 0.34f);
    private Color colorActive = new Color(1, 1, 1);

    public static MinecraftUI instance;

    void Start()
    {
        MinecraftUI.instance = this;

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    public void SwitchMode()
    {
        mode = (Mode)((int)(mode + 1) % Enum.GetNames(typeof(Mode)).Length);
        debugText.text += "Mode set to " + mode.ToString() + "\n";

        if (mode == Mode.PLACE)
        {
            buttonText.text = "Switch to Pickup Mode";
        }
        else if (mode == Mode.PICKUP)
        {
            buttonText.text = "Switch to Place Mode";
        }
    }

    public void BuildMode()
    {
        mode = Mode.PLACE;
        debugText.text += "Mode set to " + mode.ToString() + "\n";

        BombImage.color = colorInactive;
        HammerImage.color = colorActive;
    }

    public void DestroyMode()
    {
        mode = Mode.PICKUP;
        debugText.text += "Mode set to " + mode.ToString() + "\n";

        BombImage.color = colorActive;
        HammerImage.color = colorInactive;
    }

    public void OnClickReset()
    {
        ARSession arSession = GetComponent<ARSession>();
        arSession.Reset();
    }

    public void OnClickBack()
    {
        instance = null;
        SceneManager.LoadScene("HomeScreen");
        //SceneManager.UnloadScene("Minecraft");
    }

    public void OnClickShapes()
    {
        shapeMenu.SetActive(!shapeMenu.activeSelf);
    }

    public void OnClickCubeShape()
    {
        objectType = Object.CUBE;
        shapeMenu.SetActive(false);
    }

    public void OnClickCylinderShape()
    {
        objectType = Object.CYLINDER;
        shapeMenu.SetActive(false);
    }

    public void OnClickConeShape()
    {
        objectType = Object.CONE;
        shapeMenu.SetActive(false);
    }

    public void OnClickColorMainButton()
    {
        bool isColorMenuOpen = colorMenu.activeSelf;
        colorMenu.SetActive(!isColorMenuOpen);
        colorMainButton.SetActive(isColorMenuOpen);
    }

    public void OnClickRed()
    {
        objectColor = Color.red;
        OnClickColorMainButton();
    }

    public void OnClickGreen()
    {
        objectColor = Color.green;
        OnClickColorMainButton();
    }

    public void OnClickBlue()
    {
        objectColor = Color.blue;
        OnClickColorMainButton();
    }

    public void OnClickYellow()
    {
        objectColor = Color.yellow;
        OnClickColorMainButton();
    }
}
