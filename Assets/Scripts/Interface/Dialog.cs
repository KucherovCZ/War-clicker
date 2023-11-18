using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public static Dialog Instance;

    private Button YesButton, NoButton, CloseButton;
    private GameObject UIObject;
    private bool yesPressed, noPressed, closePressed;

    // Use this for initialization
    public void Init()
    {
        UIObject = gameObject;
        YesButton = transform.Find("YesButton")?.GetComponent<Button>() ?? null;
        NoButton = transform.Find("NoButton")?.GetComponent<Button>() ?? null;
        CloseButton = transform.Find("CloseButton")?.GetComponent<Button>() ?? null;
    }

    public DialogResult ShowDialog(DialogButtons buttons)
    {
        // set gameObject to active (maybe add shadow BG to it)
        gameObject.SetActive(true);

        yesPressed = noPressed = closePressed = false;

        // show buttons based on enum
        if(YesButton != null) YesButton.gameObject.SetActive(buttons == DialogButtons.Yes || buttons == DialogButtons.YesNo || buttons == DialogButtons.YesNoClose);
        if(NoButton != null) NoButton.gameObject.SetActive(buttons == DialogButtons.YesNo || buttons == DialogButtons.YesNoClose);
        if(CloseButton != null) CloseButton.gameObject.SetActive(buttons == DialogButtons.YesNoClose);

        // wait for button click
        while (!yesPressed && !noPressed && !closePressed)
        { }
                
        gameObject.SetActive(false);

        // send as result
        if (yesPressed) return DialogResult.Yes;
        else return DialogResult.No;
    }

    public void YesButtonClicked()
    {
        yesPressed = true;
    }

    public void NoButtonClicked()
    {
        noPressed = true;
    }

    public void CloseButtonClicked()
    {
        closePressed = true;
    }
}

public enum DialogButtons
{ 
    Yes,
    YesNo,
    YesNoClose
}

public enum DialogResult
{ 
    Yes,
    No
}