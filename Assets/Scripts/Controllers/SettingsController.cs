using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    private Image lastChangedButton;
    private GameObject lastOpenedPage;

    private void Start()
    {
        OnButtonClick("Settings");
    }

    public void OnButtonClick(string btnName)
    {
        if (string.IsNullOrEmpty(btnName))
            Logger.Log(LogLevel.ERROR, "Settings button click - passed empty string", "");

        Transform button = transform.Find("Button" + btnName);
        Image btnBackground = button.GetComponent<Image>();

        // button
        if (lastChangedButton != null)
            lastChangedButton.color = new Color(0.6f, 0.6f, 0.6f);
        btnBackground.color = new Color(1f, 1f, 1f);
        lastChangedButton = btnBackground;

        if (lastOpenedPage != null)
            lastOpenedPage.SetActive(false);

        // enable current gameObject
        lastOpenedPage = transform.Find(btnName).gameObject;
        lastOpenedPage.SetActive(true);
    }
}
