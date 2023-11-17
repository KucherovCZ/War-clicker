using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Entities;
using TMPro;

public class SellDialog : MonoBehaviour
{
    public static SellDialog Instance;

    public cWeapon Weapon { get; set; }

    private GameObject UIObject;
    private Button YesButton, NoButton;
    private TextMeshProUGUI StoredText, PriceText;
    private bool yesPressed, noPressed;

    // Use this for initialization
    void Start()
    {
        Instance = this;

        UIObject = gameObject;
        YesButton = transform.Find("YesButton")?.GetComponent<Button>() ?? null;
        NoButton = transform.Find("NoButton")?.GetComponent<Button>() ?? null;

        StoredText = transform.Find("Stored").GetComponent<TextMeshProUGUI>();
        PriceText = transform.Find("Price").GetComponent<TextMeshProUGUI>();

        StoredText.text = CustomUtils.FormatMoney(Weapon.Stored);
        PriceText.text = CustomUtils.FormatMoney(Weapon.SellPrice);
    }

    int counter = 0;
    int framesPerUpdate = 50 / CustomUtils.UpdateFrequency;
    private void FixedUpdate()
    {
        counter++;
        if (counter == framesPerUpdate)
        {
            counter = 0;
            StoredText.text = CustomUtils.FormatMoney(Weapon.Stored);
        }
    }

    public DialogResult ShowDialog(cWeapon weapon)
    {
        this.Weapon = weapon;
        return ShowDialog(DialogButtons.YesNo);
    }

    public DialogResult ShowDialog(DialogButtons buttons)
    {
        // set gameObject to active (maybe add shadow BG to it)
        gameObject.SetActive(true);

        yesPressed = noPressed = false;

        // show buttons based on enum
        if (YesButton != null) YesButton.gameObject.SetActive(buttons == DialogButtons.Yes || buttons == DialogButtons.YesNo || buttons == DialogButtons.YesNoClose);
        if (NoButton != null) NoButton.gameObject.SetActive(buttons == DialogButtons.YesNo || buttons == DialogButtons.YesNoClose);

        // wait for button click
        while (!yesPressed && !noPressed)
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
}