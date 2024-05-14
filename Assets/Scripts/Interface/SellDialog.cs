using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellDialog : MonoBehaviour
{
    public static SellDialog Instance;

    public cWeapon Weapon { get; set; }

    private GameObject UIObject;
    private Slider SliderObject;
    private TextMeshProUGUI StoredText, PriceText, TotalPriceText;
    private TMP_InputField InputField;
    private int selectedAmount = 0;
    private bool changedWithSlider = true;

    // Use this for initialization
    public void Init()
    {

        UIObject = gameObject;
        SliderObject = transform.Find("Slider").GetComponent<Slider>();
        InputField = transform.Find("Amount").Find("Input").GetComponent<TMP_InputField>();

        StoredText = transform.Find("Stored").GetComponent<TextMeshProUGUI>();
        PriceText = transform.Find("Price").GetComponent<TextMeshProUGUI>();
        TotalPriceText = transform.Find("TotalPrice").GetComponent<TextMeshProUGUI>();
    }

    int counter = 0;
    int framesPerUpdate = 50 / CustomUtils.UpdateFrequency;
    private void FixedUpdate()
    {
        counter++;
        if (counter == framesPerUpdate)
        {
            counter = 0;

            // update selected amount if using slider
            if (changedWithSlider)
            {
                selectedAmount = Mathf.CeilToInt(SliderObject.value * Weapon.Stored);
                InputField.text = selectedAmount.ToString();
                TotalPriceText.text = CustomUtils.FormatNumber((int)(selectedAmount * Weapon.SellPrice));
            }
            else
            {
                SliderObject.SetValueWithoutNotify(selectedAmount / (float)Weapon.Stored);
            }

            // update UI
            StoredText.text = CustomUtils.FormatNumber(Weapon.Stored);
        }
    }

    public void ShowDialog(cWeapon weapon)
    {
        this.Weapon = weapon;
        StoredText.text = CustomUtils.FormatNumber(Weapon.Stored);
        PriceText.text = CustomUtils.FormatNumber(Weapon.SellPrice);
        TotalPriceText.text = "0";

        // set gameObject to active (maybe add shadow BG to it)
        gameObject.SetActive(true);

        // TODO reset slider and text input
        SliderObject.value = 1f;
    }

    public void CloseDialog(bool approved)
    {
        if (approved)
        {
            PlayerController.Instance.AddMoney(Weapon.SellPrice * selectedAmount);
            Weapon.Stored -= selectedAmount;
            ProductionController.Instance.WarehouseUsed[(int)Weapon.Type] -= selectedAmount;

            UIController.Instance.UpdateDetailStored();
        }

        gameObject.SetActive(false);
    }

    public void OnSliderValueChanged()
    {
        selectedAmount = Mathf.CeilToInt(SliderObject.value * Weapon.Stored);
        changedWithSlider = true;
    }

    public void OnInputFieldSelect()
    {
        changedWithSlider = false;
    }

    public void OnInputFieldEndEdit()
    {
        if (int.TryParse(InputField.text, out int value))
        {
            value = Mathf.Clamp(value, 0, Weapon.Stored);
            InputField.text = value.ToString();
            selectedAmount = value;
            changedWithSlider = false;

            TotalPriceText.text = CustomUtils.FormatNumber((int)(selectedAmount * Weapon.SellPrice));
            SliderObject.SetValueWithoutNotify(selectedAmount / (float)Weapon.Stored);
        }
        else
        {
            Debug.Log("SellDialog - Invalid amount input.");
        }
    }
}