using Entities;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // parent transform = canvas

    // Start is called before the first frame update

    public static UIController Instance = null;

    void Start()
    {
        Instance = this;

        InitPopup();

        LoadPlayerItems();

        InitControllers();
        LoadProductionItems();
        LoadResearchItems();

        LoadMainMenuItems();

        OnMainMenuButtonClick("Production");
        OnProductionButtonClick("Infantry");
        OnResearchButtonClick("Infantry");
    }

    int counter = 0;
    int framesPerUpdate = 50 / CustomUtils.UpdateFrequency;
    private void FixedUpdate()
    {
        counter++;
        if (counter == framesPerUpdate)
        {
            counter = 0;
            MoneyLabel.text = CustomUtils.FormatNumber(PlayerController.Instance.Money);
            WarXPLabel.text = CustomUtils.FormatNumber(PlayerController.Instance.WarFunds);
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
        ResearchController.Instance.UIController = this;

        Dialog.Instance = transform.Find("Dialog").GetComponent<Dialog>();
        Dialog.Instance.Init();
        SellDialog.Instance = transform.Find("SellDialog").GetComponent<SellDialog>();
        SellDialog.Instance.Init();
    }

    private void LoadPlayerItems()
    {
        topMenu = transform.Find("TopMenu");

        MoneyLabel = topMenu.Find("Money").GetComponent<TextMeshProUGUI>();
        WarXPLabel = topMenu.Find("WarFunds").GetComponent<TextMeshProUGUI>();
        MoneyGlobalPosLabel = topMenu.Find("MoneyGlobalPos").GetComponent<TextMeshProUGUI>();
        WarXPGlobalPosLabel = topMenu.Find("WarXPGlobalPos").GetComponent<TextMeshProUGUI>();
    }

    #endregion

    #region Production


    private bool ProductionDetailOpen
    {
        get
        {
            return openPrItem != null;
        }
    }

    private Transform productionView;
    private Transform scrollRect;
    private Transform viewport;
    private ScrollRect productionScrollView;
    private Image lastChangedIcon = null, lastChangedButton = null;

    private ProductionItem openPrItem = null;

    private WeaponType currentWeaponType;
    private Transform weaponDetail, researchDetail;
    private Button Plus, Minus;
    private TMP_InputField FactoryInput;
    private TextMeshProUGUI ProdTimeLabel;
    private TextMeshProUGUI StoredLabel;

    private Transform FactoryManager, WarehouseManager;
    private Button BuyFactory, BuyWarehouse;
    private TextMeshProUGUI BuyFactoryPriceLabel, BuyWarehousePriceLabel;
    private TextMeshProUGUI BuyFactoryAmountLabel, BuyWarehouseAmountLabel;
    private TextMeshProUGUI FactoryAmountLabel, WarehouseCapacityLabel;
    private TextMeshProUGUI FactoryLevelLabel, WarehouseLevelLabel;

    private void LoadProductionItems()
    {
        productionView = transform.Find("Production");
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

        FactoryManager = productionView.Find("FactoryManager");
        BuyFactory = FactoryManager.Find("UpgradeButton").GetComponent<Button>();
        BuyFactoryPriceLabel = FactoryManager.Find("UpgradeButton").Find("Price").GetComponent<TextMeshProUGUI>();
        BuyFactoryAmountLabel = FactoryManager.Find("UpgradeButton").Find("Amount").GetComponent<TextMeshProUGUI>();
        FactoryLevelLabel = FactoryManager.Find("Level").GetComponent<TextMeshProUGUI>();
        FactoryAmountLabel = FactoryManager.Find("Amount").GetComponent<TextMeshProUGUI>();

        WarehouseManager = productionView.Find("WarehouseManager");
        BuyWarehouse = WarehouseManager.Find("UpgradeButton").GetComponent<Button>();
        BuyWarehousePriceLabel = WarehouseManager.Find("UpgradeButton").Find("Price").GetComponent<TextMeshProUGUI>();
        BuyWarehouseAmountLabel = WarehouseManager.Find("UpgradeButton").Find("Amount").GetComponent<TextMeshProUGUI>();
        WarehouseLevelLabel = WarehouseManager.Find("Level").GetComponent<TextMeshProUGUI>();
        WarehouseCapacityLabel = WarehouseManager.Find("Capacity").GetComponent<TextMeshProUGUI>();
    }

    public void OnProductionButtonClick(string type)
    {
        // Update button and icon color
        if (string.IsNullOrEmpty(type))
            Debug.LogError("Production button click - passed empty string");

        currentWeaponType = EnumUtils.GetWeaponType(type);

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

        // update factory manager content
        UpdateProductionUI();

        if (productionScrollView.content != null)
            productionScrollView.content.localScale = new Vector3(0, 1, 1);
        productionScrollView.content = content;
        content.localScale = new Vector3(1, 1, 1);
        //content.gameObject.SetActive(true);
    }

    public void UpdateDetailStored()
    {
        if (ProductionDetailOpen)
            StoredLabel.text = openPrItem.Weapon.Stored.ToString();
        WarehouseCapacityLabel.text = ProductionController.Instance.WarehouseUsed[(int)currentWeaponType].ToString() + "/" + ProductionController.Instance.WarehouseCapacity[(int)currentWeaponType].ToString();
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

    public void OpenWeaponDetail(DbWeapon weapon, ProductionItem prItem)
    {
        weaponDetail.gameObject.SetActive(true);

        openPrItem = prItem;
        weaponDetail.Find("IconBackground").Find("Icon").GetComponent<Image>().sprite = prItem.Sprite;
        weaponDetail.Find("WeaponName").GetComponent<TextMeshProUGUI>().text = weapon.DisplayName;
        weaponDetail.Find("Flags").GetComponent<TextMeshProUGUI>().text = weapon.FlagsString;
        weaponDetail.Find("Price").GetComponent<TextMeshProUGUI>().text = CustomUtils.FormatNumber(weapon.SellPrice);
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

        Plus.interactable = CheckFreeFactories();
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
            UpdateProductionUI();
        }

        if (freeFactories == 1)
            Plus.interactable = false;
    }

    public void RemoveFactoryButtonOnClick()
    {
        if (openPrItem.Weapon.FactoriesAssigned > 0)
        {
            openPrItem.FactoriesUpdated(-1);
            FactoryInput.text = openPrItem.Weapon.FactoriesAssigned.ToString();
            ProdTimeLabel.text = CustomUtils.FormatTime(openPrItem.Weapon.ProductionTime);
            Plus.interactable = true;
            UpdateProductionUI();
        }
    }

    public void ChangeAutosell(bool enabled)
    {
        openPrItem.Weapon.Autosell = enabled;
    }

    //public void SendWeaponsToWar()
    //{ 

    //}

    public void SellWeaponsButtonOnClick()
    {
        SellDialog.Instance.ShowDialog(openPrItem.Weapon);
    }

    public void BuyFactoryButtonOnClick()
    {
        if (PlayerController.Instance.TryBuyMoney(ProductionController.Instance.GetCurrentFactoryPrice(currentWeaponType)))
        {
            ProductionController.Instance.Factories[(int)currentWeaponType] += ProductionController.Instance.GetNewFactoryAmount(currentWeaponType);
            ProductionController.Instance.FactoryLevel[(int)currentWeaponType] += 1;
            UpdateProductionUI();
        }
    }

    public void BuyWarehouseButtonOnClick()
    {
        if (PlayerController.Instance.TryBuyMoney(ProductionController.Instance.GetCurrentWarehousePrice(currentWeaponType)))
        {
            ProductionController.Instance.WarehouseCapacity[(int)currentWeaponType] += ProductionController.Instance.GetNewWarehouseAmount(currentWeaponType);
            ProductionController.Instance.WarehouseLevel[(int)currentWeaponType] += 1;
            UpdateProductionUI();
        }
    }

    public void UpdateProductionUI()
    {
        // lvl $LEVEL
        FactoryLevelLabel.text = "lvl " + ProductionController.Instance.FactoryLevel[(int)currentWeaponType].ToString();
        WarehouseLevelLabel.text = "lvl " + ProductionController.Instance.WarehouseLevel[(int)currentWeaponType].ToString();
        // $PRICE
        BuyFactoryPriceLabel.text = CustomUtils.FormatNumberShort(ProductionController.Instance.GetNewFactoryPrice(currentWeaponType));
        BuyWarehousePriceLabel.text = CustomUtils.FormatNumberShort(ProductionController.Instance.GetNewWarehousePrice(currentWeaponType));
        // $USED/$TOTAL
        FactoryAmountLabel.text = ProductionController.Instance.UsedFactories[(int)currentWeaponType].ToString() + "/" + ProductionController.Instance.Factories[(int)currentWeaponType].ToString();
        WarehouseCapacityLabel.text = ProductionController.Instance.WarehouseUsed[(int)currentWeaponType].ToString() + "/" + ProductionController.Instance.WarehouseCapacity[(int)currentWeaponType].ToString();

        // $ + $NEXTLEVELAMOUNT
        BuyFactoryAmountLabel.text = "+ " + CustomUtils.FormatNumberShort(ProductionController.Instance.GetNewFactoryAmount(currentWeaponType));
        BuyWarehouseAmountLabel.text = "+ " + CustomUtils.FormatNumberShort(ProductionController.Instance.GetNewWarehouseAmount(currentWeaponType));
    }



    #endregion

    #region Research

    private Transform researchView;
    private Transform researchViewport;
    private ScrollRect researchScrollview;
    private Image lastChangedIconRes = null, lastChangedButtonRes = null;

    private ResearchItem openResItem = null;

    private void LoadResearchItems()
    {
        researchView = transform.Find("Research");
        researchViewport = researchView.Find("Viewport");
        researchScrollview = researchView.GetComponent<ScrollRect>();
        researchDetail = researchView.Find("ResearchDetail");
    }

    public void OnResearchButtonClick(string type)
    {
        // Update button and icon color
        if (string.IsNullOrEmpty(type))
            Debug.LogError("Research button click - passed empty string");

        Transform button = researchView.Find("Button" + type);
        Image btnBackground = button.GetComponent<Image>();
        Image btnImage = button.Find("Image").GetComponent<Image>();

        // button
        if (lastChangedButtonRes != null)
            lastChangedButtonRes.color = new Color(0.6f, 0.6f, 0.6f);
        btnBackground.color = new Color(1f, 1f, 1f);
        lastChangedButtonRes = btnBackground;

        // icon
        if (lastChangedIconRes != null)
            lastChangedIconRes.color = new Color(1f, 1f, 1f);
        btnImage.color = new Color(0.5f, 0.5f, 0.5f);
        lastChangedIconRes = btnImage;

        // Update scrollView content
        RectTransform content = (RectTransform)researchViewport.Find(type);

        if (researchScrollview.content != null)
            researchScrollview.content.localScale = new Vector3(0, 1, 1);
        researchScrollview.content = content;
        content.localScale = new Vector3(1, 1, 1);
    }

    public void OpenResearchDetail(DbResearchItem item, ResearchItem resItem)
    {
        researchDetail.gameObject.SetActive(true);

        string weaponNamesList = "";
        string[] weaponNames = resItem.Weapons.Select(w => w.DisplayName).ToArray();
        foreach (string name in weaponNames)
            weaponNamesList += name + "\n";

        openResItem = resItem;
        researchDetail.Find("IconBackground").Find("Icon").GetComponent<Image>().sprite = resItem.Sprite;
        researchDetail.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.DisplayName;
        researchDetail.Find("WeaponsList").GetComponent<TextMeshProUGUI>().text = weaponNamesList;
        researchDetail.Find("Price").GetComponent<TextMeshProUGUI>().text = CustomUtils.FormatNumber(item.Price);
        researchDetail.Find("ResearchButton").GetComponent<Button>().interactable = !item.Researched && resItem.IsUnlocked;

        TextMeshProUGUI buttonLabel = researchDetail.Find("ResearchButton").Find("Label").GetComponent<TextMeshProUGUI>();
        if (resItem.IsUnlocked)
        {
            buttonLabel.text = item.Researched ? "Researched" : "Research";
            buttonLabel.color = item.Researched ? Color.gray : Color.white;
        }
        else
        {
            buttonLabel.text = "Locked";
            buttonLabel.color = Color.gray;
        }
    }

    public void ResearchButtonOnClick()
    {
        if (openResItem.UnlockResearchItem())
        {
            // succeeded, close detail
            researchDetail.gameObject.SetActive(false);
        }
        else
        {
            // not enough warfunds, show dialog, or warning (just popup warning for 1s is better)
            Debug.Log("Player doesnt have enough WarFunds -> show popup warning");
        }
    }

    public Sprite GetResearchIcon(string researchItemName)
    {
        Texture2D tempPic = Resources.Load("Graphics/Research/GE/" + researchItemName) as Texture2D;
        if (tempPic == null)
        {
            Debug.LogWarning("Image for " + researchItemName + " has not been found. Please check Graphics/Research/GE");
            return null;
        }
        return Sprite.Create(tempPic, new Rect(0, 0, 128, 128), new Vector2());
    }

    #endregion


    // Needs to be last - references others
    #region MainMenu

    public Transform mainMenu;
    private Image lastChangedButtonMain = null;
    private GameObject lastOpenedPage = null;


    public void LoadMainMenuItems()
    {
        mainMenu = transform.Find("MainMenu");
    }

    public void OnMainMenuButtonClick(string btnName)
    {
        if (string.IsNullOrEmpty(btnName))
            Debug.LogError("MainMenu button click - passed empty string");

        Transform button = mainMenu.Find("Button" + btnName);
        Image btnBackground = button.GetComponent<Image>();
        Image btnImage = button.Find("Image").GetComponent<Image>();

        // button
        if (lastChangedButtonMain != null)
            lastChangedButtonMain.color = new Color(0.4424517f, 0.5849056f, 0.305696f);
        btnBackground.color = new Color(1f, 1f, 1f);
        lastChangedButtonMain = btnBackground;

        // icon
        //if (lastChangedIconMain != null)
        //    lastChangedIconMain.color = new Color(1f, 1f, 1f);
        //btnImage.color = new Color(0.7f, 0.7f, 0.7f);
        //lastChangedIconMain = btnImage;

        // production page needs to stay enabled for ProductionItems to work
        if (lastOpenedPage != null && !lastOpenedPage.name.Equals("Production"))
            lastOpenedPage.SetActive(false);

        // enable current gameObject
        lastOpenedPage = transform.Find(btnName).gameObject;
        lastOpenedPage.SetActive(true);
    }
    #endregion

    #region Popup

    private Animator PopupAnimator = null;
    private AudioSource PopupSound = null;
    private TextMeshProUGUI PopupText = null;

    private void InitPopup()
    {
        transform.Find("Popup").gameObject.SetActive(true); // Popup is disabled for easier work in editor
        PopupAnimator = transform.Find("Popup").GetComponent<Animator>();
        PopupSound = transform.Find("Popup").GetComponent<AudioSource>();
        PopupText = transform.Find("Popup").Find("Text").GetComponent<TextMeshProUGUI>();
    }

    public void PopupDialog(string message, Color? textColor = null)
    {
        PopupText.color = textColor ?? Color.white;
        PopupText.text = message;
        PopupAnimator.SetTrigger("Show"); // shows popup for 3s
        PopupSound.Play();
    }
    #endregion
}
