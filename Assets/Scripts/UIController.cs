using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Profiling.LowLevel;
using Unity.VisualScripting;
using UnityEditor.Advertisements;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Entities; 

public class UIController : MonoBehaviour
{
    // parent transform = canvas

    // Start is called before the first frame update

    public static UIController Instance = null;

    void Start()
    {
        Instance = this;

        InitControllers();

        LoadPlayerItems();
        LoadProductionItems();

        OnProductionButtonClick("Infantry");
    }

    int counter = 0;
    int framesPerUpdate = 50 / CustomUtils.UpdateFrequency;
    private void FixedUpdate()
    {
        counter++;
        if (counter == framesPerUpdate)
        {
            counter = 0;
            MoneyLabel.text = CustomUtils.FormatMoney(PlayerController.Instance.Money);
            WarXPLabel.text = PlayerController.Instance.WarFunds.ToString();
        }
    }

    #region Player

    private Transform topMenu;
    private TextMeshProUGUI MoneyLabel { get; set; }
    private TextMeshProUGUI WarXPLabel { get; set; }
    private TextMeshProUGUI MoneyGlobalPosLabel { get; set; }
    private TextMeshProUGUI WarXPGlobalPosLabel { get; set; }

    private void InitControllers()
    {
        ProductionController.Instance.UIController = this;
        PlayerController.Instance.UIController = this;
    }

    private void LoadPlayerItems()
    {
        topMenu = transform.Find("TopMenu");

        MoneyLabel = topMenu.Find("Money").GetComponent<TextMeshProUGUI>();
        WarXPLabel = topMenu.Find("WarXP").GetComponent<TextMeshProUGUI>();
        MoneyGlobalPosLabel = topMenu.Find("MoneyGlobalPos").GetComponent<TextMeshProUGUI>();
        WarXPGlobalPosLabel = topMenu.Find("WarXPGlobalPos").GetComponent<TextMeshProUGUI>();
    }

    public void OnQuickSellButtonClick()
    {
        //PlayerController.Instance.QuickSell();
    }

    #endregion

    #region Production

    private bool DetailOpen
    {
        get
        {
            return openPrItem != null;
        }
    }

    private Transform productionView;
    private Transform viewport;
    private ScrollRect productionScrollView;
    private Image lastChangedIcon = null, lastChangedButton = null;

    private ProductionItem openPrItem = null;

    private Transform weaponDetail;
    private Button Plus, Minus;
    private TMP_InputField FactoryInput;
    private TextMeshProUGUI ProdTimeLabel;
    private TextMeshProUGUI StoredLabel;

    private void LoadProductionItems()
    {
        productionView = transform.Find("WeaponProduction");
        viewport = productionView.Find("Viewport");
        productionScrollView = productionView.GetComponent<ScrollRect>();
        weaponDetail = productionView.Find("WeaponDetail");

        // detail items
        Plus = weaponDetail.Find("Factories").Find("Plus").GetComponent<Button>();
        Plus.onClick.AddListener(AddFactoryButtonOnClick);
        Minus = weaponDetail.Find("Factories").Find("Minus").GetComponent<Button>();
        Minus.onClick.AddListener(RemoveFactoryButtonOnClick);
        FactoryInput = weaponDetail.Find("Factories").Find("Input").GetComponent<TMP_InputField>();
        ProdTimeLabel = weaponDetail.Find("ProdTime").GetComponent<TextMeshProUGUI>();
        StoredLabel = weaponDetail.Find("Stored").GetComponent<TextMeshProUGUI>();
    }

    public void OnProductionButtonClick(string type)
    {
        // Update button and icon color
        if (string.IsNullOrEmpty(type))
            Debug.LogError("Production button click - passed empty string");

        Transform button = productionView.Find("Button" + type);
        Image btnBackground = button.GetComponent<Image>();
        Image btnImage = button.Find("Image").GetComponent<Image>();

        // button
        if (lastChangedButton != null)
            lastChangedButton.color = new Color(0.6f, 0.6f, 0.6f);
        btnBackground.color = new Color(1f, 1f, 1f);
        lastChangedButton = btnBackground;

        // icon
        if (lastChangedIcon != null)
            lastChangedIcon.color = new Color(1f, 1f, 1f);
        btnImage.color = new Color(0.5f, 0.5f, 0.5f);
        lastChangedIcon = btnImage;

        // Update scrollView content
        RectTransform content = (RectTransform)viewport.Find(type);

        if (productionScrollView.content != null)
            productionScrollView.content.localScale = new Vector3(0, 1, 1);
        productionScrollView.content = content;
        content.localScale = new Vector3(1, 1, 1);
        //content.gameObject.SetActive(true);
    }

    public void UpdateDetailStored()
    {
        if (DetailOpen)
            StoredLabel.text = openPrItem.Weapon.Stored.ToString();
    }

    public Sprite GetWeaponIcon(string weaponName)
    {
        Texture2D tempPic = Resources.Load("Graphics/Weapons/GE/" + weaponName) as Texture2D;
        if (tempPic == null)
        {
            Debug.LogWarning("Image for " + weaponName + " has not been found. Please check Graphics/Weapos/GE");
            return null;
        }
        return Sprite.Create(tempPic, new Rect(0, 0, 128, 128), new Vector2());
    }

    public void OpenWeaponDetail(cWeapon weapon, ProductionItem prItem)
    {
        weaponDetail.gameObject.SetActive(true);

        openPrItem = prItem;
        weaponDetail.Find("IconBackground").Find("Icon").GetComponent<Image>().sprite = GetWeaponIcon(weapon.Name);
        weaponDetail.Find("WeaponName").GetComponent<TextMeshProUGUI>().text = weapon.DisplayName;
        weaponDetail.Find("Flags").GetComponent<TextMeshProUGUI>().text = weapon.FlagsString;
        weaponDetail.Find("Price").GetComponent<TextMeshProUGUI>().text = weapon.SellPrice.ToString();
        ProdTimeLabel.text = CustomUtils.FormatTime(weapon.ProductionTime);
        weaponDetail.Find("Damage").GetComponent<TextMeshProUGUI>().text = weapon.Damage.ToString();
        weaponDetail.Find("Bonus").GetComponent<TextMeshProUGUI>().text = weapon.Bonus.ToString();
        weaponDetail.Find("AntiTank").GetComponent<TextMeshProUGUI>().text = weapon.AntiTank.ToString();
        weaponDetail.Find("AntiAir").GetComponent<TextMeshProUGUI>().text = weapon.AntiAir.ToString();
        weaponDetail.Find("AntiNavy").GetComponent<TextMeshProUGUI>().text = weapon.AntiNavy.ToString();
        weaponDetail.Find("Factories").Find("Text").GetComponent<TextMeshProUGUI>().text = weapon.FactoriesAssigned.ToString();
        StoredLabel.text = weapon.Stored.ToString();
        weaponDetail.Find("Autosell").GetComponent<SliderColorChange>().SetSlider(weapon.Autosell);
        weaponDetail.Find("Description").Find("Text").GetComponent<TextMeshProUGUI>().text = weapon.Description;
        FactoryInput.text = weapon.FactoriesAssigned.ToString();

        Plus.enabled = CheckFreeFactories();
    }

    public bool CheckFreeFactories()
    {
        return ProductionController.Instance.GetFreeFactories(openPrItem.Weapon.Type) > 0;   
    }

    public void AddFactoryButtonOnClick()
    {
        int freeFactories = ProductionController.Instance.GetFreeFactories(openPrItem.Weapon.Type);
        if (freeFactories > 0)
        {
            openPrItem.FactoriesUpdated(1);
            FactoryInput.text = openPrItem.Weapon.FactoriesAssigned.ToString();
            ProdTimeLabel.text = CustomUtils.FormatTime(openPrItem.Weapon.ProductionTime);
        }

        if (freeFactories == 1)
            Plus.enabled = false;
    }

    public void RemoveFactoryButtonOnClick()
    {
        if (openPrItem.Weapon.FactoriesAssigned > 0)
        {
            openPrItem.FactoriesUpdated(-1);
            FactoryInput.text = openPrItem.Weapon.FactoriesAssigned.ToString();
            ProdTimeLabel.text = CustomUtils.FormatTime(openPrItem.Weapon.ProductionTime);
            Plus.enabled = true;
        }
    }

    public void ChangeAutosell(bool enabled)
    {
        openPrItem.Weapon.Autosell = enabled;
    }

    #endregion
}
