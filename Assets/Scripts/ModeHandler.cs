using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeHandler : MonoBehaviour
{
    public enum Mode
    {
        PLACE,
        PICKUP,
        APPEND
    }

    public Mode mode = Mode.PLACE;
    public Text buttonText;
    public Text debugText;

    public static ModeHandler instance;

    void Start()
    {
        ModeHandler.instance = this;
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
            buttonText.text = "Switch to Append Mode";
        }
        else if (mode == Mode.APPEND)
        {
            buttonText.text = "Switch to Place Mode";
        }
    }
}
