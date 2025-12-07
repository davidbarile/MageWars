using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Card;
using static DeckConfig;

/// <summary>
/// Put game-specific functions here
/// </summary>
public class UICustomCardEditor : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField descriptionInput;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI cardTypeText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI drawCardsText;
    [SerializeField] private TextMeshProUGUI maxHandText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI[] healthTexts;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI petsText;
    [SerializeField] private TextMeshProUGUI plantsText;
    [SerializeField] private TextMeshProUGUI itemsText;
    [SerializeField] private TextMeshProUGUI permResourceTitleText;

    [Space]
    [SerializeField] private Image attackDeckImage;
    [SerializeField] private Image defenseDeckImage;

    [Space]
    [SerializeField] private UISpriteSelectItem[] resourceSpriteItems;
    [SerializeField] private UISpriteSelectItem[] requirementSpriteItems;
    [SerializeField] private UISpriteSelectItem permanentResourceSpriteItem;

    [Header("Editor Widgets")]
    [SerializeField] private GameObject descriptionWidget;
    [SerializeField] private GameObject levelWidget;
    [SerializeField] private GameObject drawCardsWidget;
    [SerializeField] private GameObject maxHandWidget;
    [SerializeField] private GameObject maxHealthWidget;
    [SerializeField] private GameObject armorWidget;
    [SerializeField] private GameObject petsWidget;
    [SerializeField] private GameObject plantsWidget;
    [SerializeField] private GameObject itemsWidget;
    [SerializeField] private GameObject attackDeckWidget;
    [SerializeField] private GameObject defenseDeckWidget;
    [SerializeField] private GameObject attackNumbersWidget;
    [SerializeField] private GameObject[] itemsToHideOnMage;
    [SerializeField] private GameObject[] itemsToShowOnMage;

    [SerializeField] private GameObject resourcesWidget;
    [SerializeField] private GameObject resourcesInnerWidget;
    [SerializeField] private GameObject permanenResourceWidget;
    [SerializeField] private GameObject requirementsWidget;
    [Space, SerializeField] private Transform permResourceEditButton;
    [SerializeField] private Transform rightPermResourceEditButtonLocation;
    [SerializeField] private Transform leftPermResourceEditButtonLocation;

    [Space, SerializeField] private Transform permResource;
    [SerializeField] private Transform rightPermResourceLocation;
    [SerializeField] private Transform leftPermResourceLocation;

    private Card card;

    private string oldMaxHealthValue;
    private string oldHealthCardsValue;
    private int oldCountValue;
    private int oldLevelValue;
    private int oldDrawCardsValue;
    private int oldAttackDeckValue;
    private int oldDefenseDeckValue;
    private int oldMaxHandValue;
    private int oldArmorValue;
    private int oldPetsValue;
    private int oldPlantsValue;
    private int oldItemsValue;


    public void Init(Card inCard)
    {
        this.card = inCard;

        this.oldCountValue = this.card.Data.Count;
        this.oldMaxHealthValue = this.card.Data.MaxHealth.ToString();
        this.oldHealthCardsValue = CreateStringFromHealthCards(this.card.Data.MaxHealth, this.card.Data.HealthCards);
        this.oldLevelValue = this.card.Data.Level;
        this.oldAttackDeckValue = (int)this.card.Data.AttackDeck;
        this.oldDefenseDeckValue = (int)this.card.Data.DefenseDeck;
        this.oldDrawCardsValue = this.card.Data.DrawCardsPerTurn;
        this.oldMaxHandValue = this.card.Data.MaxCardsInHand;
        this.oldArmorValue = this.card.Data.MaxArmor;
        this.oldPetsValue = this.card.Data.MaxPets;
        this.oldPlantsValue = this.card.Data.MaxPlants;
        this.oldItemsValue = this.card.Data.MaxItems;

        ConfigureForCardType();

        RefreshDisplay(true);

        if (UITextIconPanel.IN.TargetInputField == null)
            UITextIconPanel.IN.TargetInputField = this.descriptionInput;
    }

    private void ConfigureForCardType()
    {
        var isMage = this.card.Data.CardType == ECardType.Mage;

        this.levelWidget.SetActive(true);
        this.drawCardsWidget.SetActive(false);
        this.maxHandWidget.SetActive(false);
        this.maxHealthWidget.SetActive(true);
        this.defenseDeckWidget.SetActive(false);
        this.armorWidget.SetActive(false);
        this.petsWidget.SetActive(false);
        this.plantsWidget.SetActive(false);
        this.itemsWidget.SetActive(false);
        this.descriptionWidget.SetActive(true);
        this.resourcesWidget.SetActive(true);
        this.resourcesInnerWidget.SetActive(true);
        this.permanenResourceWidget.SetActive(true);
        this.requirementsWidget.SetActive(true);
        this.permResourceTitleText.alignment = isMage ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        this.permResourceEditButton.position = isMage ? this.leftPermResourceEditButtonLocation.position : this.rightPermResourceEditButtonLocation.position;
        this.permResource.position = isMage ? this.leftPermResourceLocation.position : this.rightPermResourceLocation.position;
        this.attackNumbersWidget.SetActive(true);
        this.attackDeckWidget.SetActive(false);

        foreach (var item in this.itemsToHideOnMage)
            item.SetActive(!isMage);

        foreach (var item in this.itemsToShowOnMage)
            item.SetActive(isMage);

        switch (this.card.Data.CardType)
        {
            case ECardType.None:
                this.levelWidget.SetActive(false);
                this.maxHealthWidget.SetActive(false);
                this.descriptionWidget.SetActive(false);
                this.resourcesWidget.SetActive(false);
                this.requirementsWidget.SetActive(false);
                this.attackNumbersWidget.SetActive(false);
                break;
            case ECardType.Mage:
                this.levelWidget.SetActive(false);
                this.drawCardsWidget.SetActive(true);
                this.maxHandWidget.SetActive(true);
                this.armorWidget.SetActive(true);
                this.petsWidget.SetActive(true);
                this.plantsWidget.SetActive(true);
                this.itemsWidget.SetActive(true);
                this.resourcesInnerWidget.SetActive(false);
                this.attackNumbersWidget.SetActive(false);
                break;
            case ECardType.Fate:
                this.levelWidget.SetActive(false);
                this.maxHealthWidget.SetActive(false);
                this.resourcesWidget.SetActive(false);
                this.requirementsWidget.SetActive(false);
                this.attackNumbersWidget.SetActive(false);
                break;
            case ECardType.Level:
                this.maxHealthWidget.SetActive(false);
                this.attackNumbersWidget.SetActive(false);
                break;
            case ECardType.Challenge:
                this.maxHealthWidget.SetActive(false);
                this.permanenResourceWidget.SetActive(false);
                this.attackNumbersWidget.SetActive(false);
                break;
            case ECardType.Structure:
                this.defenseDeckWidget.SetActive(true);
                break;
            case ECardType.Creature:
                this.attackDeckWidget.SetActive(true);
                this.defenseDeckWidget.SetActive(true);
                break;
            case ECardType.Item:
                this.attackDeckWidget.SetActive(true);
                break;
            case ECardType.Magic:
                this.attackDeckWidget.SetActive(true);
                this.maxHealthWidget.SetActive(false);
                break;
            case ECardType.Resource:
                this.levelWidget.SetActive(false);
                this.maxHealthWidget.SetActive(false);
                this.descriptionWidget.SetActive(false);
                this.resourcesWidget.SetActive(false);
                break;
            case ECardType.Curse:
            case ECardType.Blessing:
                this.resourcesWidget.SetActive(false);
                this.maxHealthWidget.SetActive(false);
                this.requirementsWidget.SetActive(false);
                this.permanenResourceWidget.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void HandleHealthMinusButtonPress(int inIndex)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        if (inIndex == -1)
        {
            if (this.card.Data.MaxHealth == 0) return;

            --this.card.Data.MaxHealth;

            PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.Health.ToString(), this.oldMaxHealthValue, this.card.Data.MaxHealth.ToString());
        }
        else
        {
            if (this.card.Data.HealthCards[inIndex] == 0) return;

            --this.card.Data.HealthCards[inIndex];
            --this.card.Data.MaxHealth;

            var newValue = CreateStringFromHealthCards(this.card.Data.MaxHealth, this.card.Data.HealthCards);

            PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.Health.ToString(), this.oldHealthCardsValue, newValue);
        }

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandleHealthPlusButtonPress(int inIndex)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        if (inIndex == -1)
        {
            ++this.card.Data.MaxHealth;

            PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.Health.ToString(), this.oldMaxHealthValue, this.card.Data.MaxHealth.ToString());
        }
        else
        {
            ++this.card.Data.HealthCards[inIndex];
            ++this.card.Data.MaxHealth;

            var newValue = CreateStringFromHealthCards(this.card.Data.MaxHealth, this.card.Data.HealthCards);

            PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.Health.ToString(), this.oldHealthCardsValue, newValue);
        }

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    private string CreateStringFromHealthCards(int inMaxHealth, int[] inHealthCards)
    {
        string healthString = $"Health: {inMaxHealth}";

        var sum = inHealthCards[0] + inHealthCards[1] + inHealthCards[2] + inHealthCards[3];

        if (sum > 0)
            healthString += $" |  <sprite name=\"Card_Enchanted\">{inHealthCards[0]}";

        if (sum > 0)
            healthString += $"  <sprite name=\"Card_Mystic\">{inHealthCards[1]}";

        if (sum > 0)
            healthString += $"  <sprite name=\"Card_Arcane\">{inHealthCards[2]}";

        if (inHealthCards[3] > 0)
            healthString += $"  <sprite name=\"Card_Quest\">{inHealthCards[3]}";

        return healthString;
    }

    //don't use.  This is handled by Deck Counts now
    public void HandleCountChangeButtonPress(int inValue)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        if (inValue < 0 && this.card.Data.Count == 0) return;

        this.card.Data.Count += inValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.Count.ToString(), this.oldCountValue.ToString(), this.card.Data.Count.ToString());

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandleLevelChangeButtonPress(int inValue)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        if (inValue < 0 && this.card.Data.Level == 1) return;

        this.card.Data.Level += inValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.Level.ToString(), this.oldLevelValue.ToString(), this.card.Data.Level.ToString());

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandleDrawCardsChangeButtonPress(int inValue)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        if (inValue < 0 && this.card.Data.DrawCardsPerTurn == 1) return;

        this.card.Data.DrawCardsPerTurn += inValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.DrawCards.ToString(), this.oldDrawCardsValue.ToString(), this.card.Data.DrawCardsPerTurn.ToString());

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandleMaxHandChangeButtonPress(int inValue)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        if (inValue < 0 && this.card.Data.MaxCardsInHand == 1) return;

        this.card.Data.MaxCardsInHand += inValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.MaxHand.ToString(), this.oldMaxHandValue.ToString(), this.card.Data.MaxCardsInHand.ToString());

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandleArmorChangeButtonPress(int inValue)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        if (inValue < 0 && this.card.Data.MaxArmor == 0) return;

        this.card.Data.MaxArmor += inValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.MaxArmor.ToString(), this.oldArmorValue.ToString(), this.card.Data.MaxArmor.ToString());

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandlePetsChangeButtonPress(int inValue)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        if (inValue < 0 && this.card.Data.MaxPets == 0) return;

        this.card.Data.MaxPets += inValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.MaxPets.ToString(), this.oldPetsValue.ToString(), this.card.Data.MaxPets.ToString());

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandlePlantsChangeButtonPress(int inValue)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        if (inValue < 0 && this.card.Data.MaxPlants == 0) return;

        this.card.Data.MaxPlants += inValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.MaxPlants.ToString(), this.oldPlantsValue.ToString(), this.card.Data.MaxPlants.ToString());

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandleItemsChangeButtonPress(int inValue)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        if (inValue < 0 && this.card.Data.MaxItems == 0) return;

        this.card.Data.MaxItems += inValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.MaxItems.ToString(), this.oldItemsValue.ToString(), this.card.Data.MaxItems.ToString());

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandlePrevTypeButtonPress()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        var oldValue = this.card.Data.CardType.ToString();

        var typeIndex = (int)this.card.Data.CardType;

        if (typeIndex > (int)ECardType.None + 1)
            --this.card.Data.CardType;
        else
            this.card.Data.CardType = ECardType.Count - 1;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.Type.ToString(), oldValue, this.card.Data.CardType.ToString());

        ConfigureForCardType();
        RefreshCardName();
        RefreshDisplay(true);

        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandleNextTypeButtonPress()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        var oldValue = this.card.Data.CardType.ToString();

        var typeIndex = (int)this.card.Data.CardType;

        if (typeIndex < (int)ECardType.Count - 1)
            ++this.card.Data.CardType;
        else
            this.card.Data.CardType = ECardType.Mage;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.Type.ToString(), oldValue, this.card.Data.CardType.ToString());

        ConfigureForCardType();
        RefreshCardName();
        RefreshDisplay(true);

        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandleChangeAttackDeckClick(int inValue)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        var newValue = EDeckType.None;

        if (inValue < 0)
        {
            switch (this.card.Data.AttackDeck)
            {
                case EDeckType.None:
                    newValue = EDeckType.Quest;
                    break;
                case EDeckType.Enchanted:
                    newValue = EDeckType.None;
                    break;
                case EDeckType.Mystic:
                    newValue = EDeckType.Enchanted;
                    break;
                case EDeckType.Arcane:
                    newValue = EDeckType.Mystic;
                    break;
                case EDeckType.Quest:
                    newValue = EDeckType.Arcane;
                    break;
            }
        }
        else
        {
            switch (this.card.Data.AttackDeck)
            {
                case EDeckType.None:
                    newValue = EDeckType.Enchanted;
                    break;
                case EDeckType.Enchanted:
                    newValue = EDeckType.Mystic;
                    break;
                case EDeckType.Mystic:
                    newValue = EDeckType.Arcane;
                    break;
                case EDeckType.Arcane:
                    newValue = EDeckType.Quest;
                    break;
                case EDeckType.Quest:
                    newValue = EDeckType.None;
                    break;
            }
        }

        this.card.Data.AttackDeck = newValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.AttackDeck.ToString(), this.oldAttackDeckValue.ToString(), this.card.Data.AttackDeck.ToString());

        RefreshDisplay(true);

        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandleChangeDefenseDeckClick(int inValue)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        var newValue = EDeckType.None;

        if (inValue < 0)
        {
            switch (this.card.Data.DefenseDeck)
            {
                case EDeckType.None:
                    newValue = EDeckType.Quest;
                    break;
                case EDeckType.Enchanted:
                    newValue = EDeckType.None;
                    break;
                case EDeckType.Mystic:
                    newValue = EDeckType.Enchanted;
                    break;
                case EDeckType.Arcane:
                    newValue = EDeckType.Mystic;
                    break;
                case EDeckType.Quest:
                    newValue = EDeckType.Arcane;
                    break;
            }
        }
        else
        {
            switch (this.card.Data.DefenseDeck)
            {
                case EDeckType.None:
                    newValue = EDeckType.Enchanted;
                    break;
                case EDeckType.Enchanted:
                    newValue = EDeckType.Mystic;
                    break;
                case EDeckType.Mystic:
                    newValue = EDeckType.Arcane;
                    break;
                case EDeckType.Arcane:
                    newValue = EDeckType.Quest;
                    break;
                case EDeckType.Quest:
                    newValue = EDeckType.None;
                    break;
            }
        }

        this.card.Data.DefenseDeck = newValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.DefenseDeck.ToString(), this.oldDefenseDeckValue.ToString(), this.card.Data.DefenseDeck.ToString());

        RefreshDisplay(true);

        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void OnNameInputEndEdit()
    {
#if !UNITY_EDITOR
            SaveName();
#endif
    }

    public void OnDescriptionInputEndEdit()
    {
#if !UNITY_EDITOR
            SaveDescription();
#endif
    }

    public void SaveName()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        var inputValue = this.nameInput.text;
        if (inputValue.Equals(this.card.Data.CardName)) return;

        var oldValue = this.card.Data.CardName;

        this.card.Data.CardName = inputValue;

        RefreshCardName();

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.Name.ToString(), oldValue, this.card.Data.CardName);

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    private void RefreshCardName()
    {
        this.card.Data.Name = $"Card_{this.card.Data.CardType}_{this.card.Data.CardName}";
    }

    public void SaveDescription()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        var inputValue = this.descriptionInput.text;
        if (inputValue.Equals(this.card.Data.Description)) return;

        var oldValue = this.card.Data.Description;

        this.card.Data.Description = inputValue;

        PlayerData.Data.SetEditNote(this.card.Data, EEditNoteType.Description.ToString(), oldValue, this.card.Data.Description);

        RefreshDisplay(true);
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void HandleResourceClick(int inIndex)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        UISpriteSelectPanel.IN.Show(this.card.Data.ResourceNames, inIndex, false, true, SetCardDirty);
    }

    public void HandleRequirementClick(int inIndex)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        UISpriteSelectPanel.IN.Show(this.card.Data.RequirementNames, inIndex, false, false, SetCardDirty);
    }

    public void HandlePermanentResourceClick()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        UISpriteSelectPanel.IN.Show(this.card.Data.PermanentResourceName, false, SetCardDirty);
    }

    private void SetCardDirty()
    {
        PlayerData.Data.SetCardDirty(this.card.Data);
    }

    public void RefreshDisplay()
    {
        RefreshDisplay(false);
    }

    private void RefreshDisplay(bool inShouldUpdatePackEditor)
    {
        this.countText.text = this.card.Data.Count.ToString();
        this.levelText.text = this.card.Data.Level.ToString();
        this.drawCardsText.text = this.card.Data.DrawCardsPerTurn.ToString();
        this.maxHandText.text = this.card.Data.MaxCardsInHand.ToString();
        this.healthText.text = this.card.Data.MaxHealth.ToString();
        this.armorText.text = this.card.Data.MaxArmor.ToString();
        this.petsText.text = this.card.Data.MaxPets.ToString();
        this.plantsText.text = this.card.Data.MaxPlants.ToString();
        this.itemsText.text = this.card.Data.MaxItems.ToString();
        this.cardTypeText.text = this.card.Data.CardType.ToString();

        this.attackDeckImage.sprite = GetAttackDeckSprite(this.card.Data.AttackDeck);
        this.defenseDeckImage.sprite = GetAttackDeckSprite(this.card.Data.DefenseDeck);

        for (int i = 0; i < this.healthTexts.Length; i++)
        {
            this.healthTexts[i].text = this.card.Data.HealthCards[i].ToString();
        }

        this.nameInput.text = this.card.Data.CardName;
        this.descriptionInput.text = this.card.Data.Description;

        for (int i = 0; i < this.resourceSpriteItems.Length; i++)
        {
            var resourceItem = this.resourceSpriteItems[i];

            if (i < this.card.Data.ResourceNames.Count)
            {
                var resourceSprite = CardsManager.GetResourceSpriteFromName(this.card.Data.ResourceNames[i]);
                resourceItem.SetSprite(resourceSprite);
            }
            else
                resourceItem.SetSprite(null);
        }

        for (int i = 0; i < this.requirementSpriteItems.Length; i++)
        {
            var reqItem = this.requirementSpriteItems[i];

            if (i < this.card.Data.RequirementNames.Count)
            {
                var reqSprite = CardsManager.GetResourceSpriteFromName(this.card.Data.RequirementNames[i]);
                reqItem.SetSprite(reqSprite);
            }
            else
                reqItem.SetSprite(null);
        }

        if (this.permanentResourceSpriteItem != null)
        {
            if (string.IsNullOrWhiteSpace(this.card.Data.PermanentResourceName))
                this.permanentResourceSpriteItem.SetSprite(null);
            else
            {
                var permSprite = CardsManager.GetResourceSpriteFromName(this.card.Data.PermanentResourceName);
                this.permanentResourceSpriteItem.SetSprite(permSprite);
            }
        }

        if (inShouldUpdatePackEditor)
            UIPackEditor.OnCardDataChanged?.Invoke();

        UIEditNotesPanel.OnPlayerDataChanged?.Invoke();
    }

    private Sprite GetAttackDeckSprite(EDeckType inDeckType)
    {
        var deckIndex = 0;

        switch (inDeckType)
        {
            case EDeckType.None:
                deckIndex = 0;
                break;
            case EDeckType.Enchanted:
                deckIndex = 1;
                break;
            case EDeckType.Mystic:
                deckIndex = 2;
                break;
            case EDeckType.Arcane:
                deckIndex = 3;
                break;
            case EDeckType.Quest:
                deckIndex = 4;
                break;
        }

        return CardsManager.IN.GetAttackDeckSprite(deckIndex);
    }

    public void OnDescriptionTextSelected()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        UITextIconPanel.IN.Show();
    }
}