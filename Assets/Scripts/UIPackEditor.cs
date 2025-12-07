using System;
using System.Collections;
using System.Collections.Generic;
using Leguar.TotalJSON;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Sirenix.OdinInspector;
using static Card;
using static CardData;
using static DeckConfig;

public class UIPackEditor : UIPanelBase
{
    public static UIPackEditor IN;
    public static CardData SelectedCardData;
    public static CardData ClipboardCardData;
    public static UIPackCardItem SelectedPackCardItem;
    public static Action OnCardDataChanged;
    public static bool IsPackEditMode => UIPackEditor.IN.IsShowing;
    public static bool ShouldBlockEdit
    {
        get
        {
            var shouldBlock = IsPackEditMode && UIPackEditor.IN.EditingPack.IsDefault;

            if (shouldBlock)
            {
                var bgColor = new Color(.95f, 0, 0);
                UIFeedbackToastPanel.IN.Show($"Cannot Edit Default Pack!", bgColor, 1.5f);
            }
            return shouldBlock;
        }
    }

    public TextMeshProUGUI EditingPackNameText => this.editingPackNameText;
    public Pack EditingPack { get; private set; }

    [SerializeField] private TextMeshProUGUI editingPackNameText, totalCardsText;
    [SerializeField] private GameObject pasteButton, hideInactiveCardsButton;
    [SerializeField] private TextMeshProUGUI[] levelCardsTexts;
    [SerializeField] private Transform scrollContent;
    [Space]
    [SerializeField] private UIDeckEditWidget[] deckEditWidgets;
    [Space]
    [SerializeField] private UICardTypeWidget[] cardTypeWidgets;
    [Space]
    [SerializeField] private Transform attackNumbersParent;
    [SerializeField] private UIAttackNumbersWidget attackNumbersWidgetPrefab;
    [SerializeField] private GameObject attackNumbersLockToggleIcon;
    [SerializeField] private GameObject attackNumbersWidget;

    [Space]
    [SerializeField] private UIPackCardItem packCardItemPrefab;
    [ReadOnly] public List<UIPackCardItem> PackCardItems { get; private set; } = new();
    [Header("Email to send pack export to")]
    [SerializeField] private string emailAddress;
    [Space]
    [SerializeField] private string cardConfigsPath = "Assets/Configs/Cards";
    [SerializeField] private CardConfig defaultCardConfig;
    [Space]
    [SerializeField] private ResetScrollRectOnEnable resetScrollRectOnEnable;
    [Space]
    [SerializeField] private GameObject loadingScreen;

    private int newCardCounter = -1;
    private bool shouldShowInactiveCards = true;

    private void Start()
    {
        this.loadingScreen.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        ClipboardCardData = null;

        SetPasteButtonVisibility(false);

        this.resetScrollRectOnEnable.ResetPosition();

        this.loadingScreen.SetActive(true);

        UIPackEditor.OnCardDataChanged += RefreshDataDisplay;

        UICardBacksPanel.OnAttackNumbersChanged += RefreshAttackNumbersWidgets;

        if (this.newCardCounter < 0)
            this.newCardCounter = CardsManager.IN.AllCardDatas.Count;

        this.EditingPack = PlayerData.Data.ActivePack;

        Invoke(nameof(HideLoadingScreen), 0.1f);
    }

    private void HideLoadingScreen()
    {
        DisplayPack(this.EditingPack);
        this.loadingScreen.SetActive(false);
    }

    public void DisplayPack(Pack inPack)
    {
        this.EditingPack = inPack;
        if (!this.EditingPack.IsDefault)
        {
            SortDeckCards(this.EditingPack.CardDatas);
        }

        PlayerData.Data.ActivePackId = inPack.ID;
        this.editingPackNameText.text = this.EditingPack.Name;

        RefreshCardsGrid();
    }

    /// <summary>
    /// Deletes and respawns all deck items.
    /// </summary>
    public void RefreshCardsGrid()
    {
        DeckManager.SetActiveGameDecks();
        RecreatePackItems();
        ConfigureDeckEditWidgets();
        ConfigureCardTypeWidgets();
        RefreshDataDisplay();
    }

    private void RecreatePackItems()
    {
        this.PackCardItems.Clear();
        this.scrollContent.DestroyAllChildren<UIPackCardItem>();
        GC.Collect();

        for (int i = 0; i < this.EditingPack.CardDatas.Count; i++)
        {
            var cardData = this.EditingPack.CardDatas[i];
            SpawnPackCardItem(cardData, i);
        }
    }

    public void CopyEditingPack(bool inShouldRefreshUI = true)
    {
        var copy = UIPackListPanel.IN.CopyPack(UIPackEditor.IN.EditingPack);

        PlayerData.Data.SetDirty();

        if (inShouldRefreshUI)
            UIPackEditor.IN.DisplayPack(copy);
    }

    private UIPackCardItem SpawnPackCardItem(CardData inCardData, int inIndex)
    {
        var packItem = Instantiate(this.packCardItemPrefab, this.scrollContent);

        packItem.name = $"PackItem_{inCardData.CardName}_{inIndex}";

        var card = packItem.SpawnCard(inCardData, inIndex);

        this.PackCardItems.Add(packItem);

        return packItem;
    }

    private void RefreshDataDisplay()
    {
        var totalCardCount = 0;
        var levelCounts = new int[this.levelCardsTexts.Length];

        for (int i = 0; i < this.PackCardItems.Count; i++)
        {
            var packItem = this.PackCardItems[i];
            var cardData = packItem.Card.Data;

            var isNoneDeck = cardData.DeckCounts[0] >= 0;

            if (isNoneDeck)
            {
                packItem.SetCount(0);
                packItem.gameObject.SetActive(this.shouldShowInactiveCards);
            }
            else
            {
                var numCardsInDeck = GetNumCardsInDeck(cardData);
                packItem.SetCount(numCardsInDeck);
                packItem.gameObject.SetActive(true);

                var shouldShow = numCardsInDeck > 0;

                packItem.gameObject.SetActive(shouldShow);

                totalCardCount += numCardsInDeck;

                if (cardData.Level > 0)
                    levelCounts[cardData.Level - 1] += numCardsInDeck;
            }

        }

        this.totalCardsText.text = $"Total Cards: {totalCardCount}";

        for (int i = 0; i < this.levelCardsTexts.Length; i++)
        {
            float numCards = levelCounts[i];
            var percent = numCards / totalCardCount * 100;
            var label = this.levelCardsTexts[i];
            label.text = $"Level {i + 1} Cards: {numCards} ({percent:N1}%)";
            label.color = numCards > 0 ? Color.black : new Color(0, 0, 0, .5f);
            bool shouldShow = i < 3;// && numCards > 0;
            label.transform.parent.gameObject.SetActive(shouldShow);
        }

        RefreshDeckEditWidgets();

        RefreshCardTypeWidgets();

        RefreshAttackNumbersWidgets();

        void RefreshCardTypeWidgets()
        {
            var numCardTypes = (int)ECardType.Count - 1;
            for (int i = 0; i < this.cardTypeWidgets.Length; i++)
            {
                var widget = this.cardTypeWidgets[i];

                var shouldShow = i < numCardTypes;

                widget.gameObject.SetActive(shouldShow);

                if (shouldShow)
                {
                    var numCardsInDeck = CardsManager.IN.GetNumCardsOfType((ECardType)i + 1);
                    widget.SetCount(numCardsInDeck);
                }
            }
        }
    }

    private void RefreshAttackNumbersWidgets()
    {
        this.attackNumbersParent.DestroyAllChildren<UIAttackNumbersWidget>();

        if (SelectedCardData == null)
        {
            this.attackNumbersWidget.SetActive(false);
            return;
        }

        this.attackNumbersLockToggleIcon.SetActive(SelectedCardData.AreAttackNumbersLocked);

        var deckIndex = 0;//this is the index of the attack number object (corresponds to the deck type)

        for (int i = 0; i < SelectedCardData.DeckCounts.Length; i++)
        {
            var deckType = (EDeckType)i;
            var deckAllowsAttackNumbers = deckType == EDeckType.Enchanted ||
            deckType == EDeckType.Mystic || deckType == EDeckType.Arcane ||
            deckType == EDeckType.Quest;

            if (!deckAllowsAttackNumbers) continue;

            this.attackNumbersWidget.SetActive(true);

            var countInDeck = SelectedCardData.DeckCounts[i];

            //if not enough attack number objects, add blank ones
            while (SelectedCardData.AttackNumbersObjects.Count <= deckIndex)
            {
                SelectedCardData.AttackNumbersObjects.Add(new AttackNums
                {
                    DeckType = deckType,
                    AttackNumValues = new List<int>()
                });
            }

            //for each deck, add the attack number for every card
            for (int j = 0; j < countInDeck; j++)
            {
                var widget = Instantiate(this.attackNumbersWidgetPrefab, this.attackNumbersParent);
                widget.Init(SelectedCardData, deckIndex, j, false);
                widget.SetDeck(deckType);

                var attackNumValues = SelectedCardData.AttackNumbersObjects[deckIndex].AttackNumValues;

                while (attackNumValues.Count <= j)
                {
                    attackNumValues.Add(0);
                }

                var count = attackNumValues[j];
                widget.SetAttackNum(count);
            }

            ++deckIndex;
        }
    }

    private int GetNumCardsInDeck(CardData inCardData, bool inShouldFilterCardTypes = true)
    {
        if (inCardData.CardType == ECardType.None)
            return 0;

        //special case for None deck
        var numCardsInNoneDeck = inCardData.DeckCounts[0];

        if (numCardsInNoneDeck > -1)
            return 0;

        if (inShouldFilterCardTypes)
        {
            var cardType = (int)inCardData.CardType;
            var isCardTypeEnabled = this.cardTypeWidgets[cardType - 1].IsEnabled;

            if (!isCardTypeEnabled)
                return 0;
        }

        var count = 0;

        for (int i = 0; i < this.deckEditWidgets.Length; i++)
        {
            var deckWidget = this.deckEditWidgets[i];

            if (!deckWidget.gameObject.activeSelf) break;

            if (deckWidget.IsEnabled)
            {
                var numCardsInDeck = inCardData.DeckCounts[i + 1];

                if (numCardsInDeck > -1)
                    count += numCardsInDeck;
            }
        }

        return count;
    }

    private void ConfigureDeckEditWidgets()
    {
        for (int i = 0; i < this.deckEditWidgets.Length; i++)
        {
            var widget = this.deckEditWidgets[i];
            var shouldShow = i < DeckManager.IN.DeckDatas.Length;

            widget.gameObject.SetActive(shouldShow);

            if (shouldShow)
                widget.Init(i, false);
        }

        RefreshDeckEditWidgets();
    }

    private void RefreshDeckEditWidgets()
    {
        for (int i = 0; i < this.deckEditWidgets.Length; i++)
        {
            var widget = this.deckEditWidgets[i];
            var shouldShow = i < DeckManager.IN.DeckDatas.Length;

            if (shouldShow)
            {
                var deckData = DeckManager.IN.DeckDatas[i];
                var numCardsInDeck = DeckManager.IN.GetNumCardsInDeck(deckData.DeckConfig.DeckType);
                var numCurseCardsInDeck = DeckManager.IN.GetNumCurseCardsInDeck(deckData.DeckConfig.DeckType);
                widget.SetCount(numCardsInDeck, numCurseCardsInDeck);
            }
        }
    }

    private void ConfigureCardTypeWidgets()
    {
        var numCardTypes = (int)ECardType.Count - 1;
        for (int i = 0; i < this.cardTypeWidgets.Length; i++)
        {
            var widget = this.cardTypeWidgets[i];

            var shouldShow = i < numCardTypes;

            widget.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                widget.Init(i + 1);
                var numCardsInDeck = CardsManager.IN.GetNumCardsOfType((ECardType)i + 1);
                widget.SetCount(numCardsInDeck);
            }
        }
    }

    public void HandleNewButtonPress()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        ++this.newCardCounter;
        var cardData = CardConfig.CreateCardData(this.defaultCardConfig, this.newCardCounter);
        var newDeckItem = SpawnPackCardItem(cardData, this.newCardCounter);

        SelectedCardData = cardData;

        CardsManager.IN.AllCardDatas.Add(cardData);
        PlayerData.Data.SetCardDirty(cardData);
        UIPackEditor.OnCardDataChanged?.Invoke();
    }
    public void HandleCopyButtonPress()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        ClipboardCardData = new CardData();
        ClipboardCardData.CopyDataFrom(SelectedCardData);

        SetPasteButtonVisibility(true);

        UIFeedbackToastPanel.IN.Show($"{ClipboardCardData.Name} copied to Clipboard!", Color.yellow, 1.5f);
    }

    public void HandlePasteButtonPress()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        ++this.newCardCounter;
        var cardData = new CardData();
        cardData.CopyDataFrom(ClipboardCardData);
        cardData.Name = $"{cardData.Name}_{this.newCardCounter}";
        var newDeckItem = SpawnPackCardItem(cardData, this.newCardCounter);

        SelectedCardData = cardData;

        CardsManager.IN.AllCardDatas.Add(cardData);
        PlayerData.Data.SetCardDirty(cardData);
        UIPackEditor.OnCardDataChanged?.Invoke();
    }

    public void HandleDuplicateButtonPress()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        var cardData = CreateCardData(SelectedCardData);

        SelectedCardData = null;

        UICardCloseUp.IN.Hide();

        UIPackEditor.OnCardDataChanged?.Invoke();
    }

    public void HandleDeleteButtonPress()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        Action doDelete = () =>
        {
            CardsManager.IN.AllCardDatas.Remove(SelectedCardData);

            PlayerData.Data.SetDirty();
            SelectedCardData = null;
            RecreatePackItems();
            UIPackEditor.OnCardDataChanged?.Invoke();

            UICardCloseUp.IN.Hide();
        };

#if UNITY_EDITOR
        doDelete.Invoke();
        return;
#endif

        UIConfirmPanel.IN.Show("Delete Card?", $"Delete <color=white>{SelectedCardData.Name}</color>?\nThis cannot be undone.\n\nYou may want to remove the card from all packs instead.", () =>
        {
            doDelete.Invoke();
        });
    }

    public void HandleExportButtonPress()
    {
        UIConfirmPanel.IN.Show("Export Pack?", $"Are you sure you want to export <color=white>{PlayerData.Data.ActivePack.Name}</color>?", Export);
    }

    private void Export()
    {
        var exportItem = new ExportItem
        {
            Name = this.EditingPack.Name
        };

        var numCards = 0;
        for (int i = 0; i < this.EditingPack.CardDatas.Count; i++)
        {
            var cardData = this.EditingPack.CardDatas[i];
            exportItem.Cards.Add(cardData);
            numCards += cardData.Count;
        }
        exportItem.NumCards = numCards;

        var json = JSON.Serialize(exportItem);

        var jsonAsString = json.CreateString();
        //var jsonAsString = json.CreatePrettyString();

        //copy to clipboard
        var te = new TextEditor
        {
            text = jsonAsString
        };
        te.SelectAll();
        te.Copy();

        jsonAsString = WWW.EscapeURL(jsonAsString);

        StartCoroutine(Upload(jsonAsString));
    }

    private IEnumerator Upload(string inDataToSend)
    {
        using UnityWebRequest www = UnityWebRequest.Post("https://um-macaroni.com/MageWars/receiveJson.php", inDataToSend, "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var bgColor = new Color(0, .75f, 0);
            UIFeedbackToastPanel.IN.Show($"Successfully Uploaded {this.EditingPack.Name}", bgColor, 3);
        }
        else
        {
            var bgColor = new Color(.95f, 0, 0);
            UIFeedbackToastPanel.IN.Show($"{www.error}", bgColor, 3);
        }
    }

    public void HandleResetAppButtonPress()
    {
        UIConfirmPanel.IN.Show("Reset App?", $"Are you sure you want to\n reset this app?\n\nAll data will be lost!", () =>
        {
            GameManager.IN.DeletePlayerPrefs();
        });
    }

    private CardData CreateCardData(CardData inCardDataToCopy)
    {
        var cardData = new CardData();

        if (inCardDataToCopy == null)
            inCardDataToCopy = CardConfig.CreateCardData(this.defaultCardConfig, CardsManager.IN.AllCardDatas.Count);

        cardData.CopyDataFrom(inCardDataToCopy);

        // var nameLength = inCardDataToCopy.Name.Length;
        // var hasNumberPostfix = nameLength > 3 && inCardDataToCopy.Name[nameLength - 3] == '_';

        // if (hasNumberPostfix)
        // {
        //     var name = inCardDataToCopy.Name.Substring(0, nameLength - 3);
        // }

        var cardNumString = (newCardCounter < 10) ? $"0{newCardCounter}" : newCardCounter.ToString();
        var fileName = $"{inCardDataToCopy.Name}_{cardNumString}";

        cardData.Name = fileName;

        ++this.newCardCounter;

        var newDeckItem = SpawnPackCardItem(cardData, this.newCardCounter);
        newDeckItem.SetCount(0);

        CardsManager.IN.AllCardDatas.Add(cardData);
        PlayerData.Data.SetCardDirty(cardData);

        return cardData;
    }

    public void HandleViewResourcesButtonPress()
    {
        var spriteNames = UISpriteSelectPanel.IN.GetAllSpriteNamesList();

        UISpriteSelectPanel.IN.Show(spriteNames, 0, true);
    }

    public void HandleAllDecksButtonPress()
    {
        foreach (var widget in this.deckEditWidgets)
        {
            widget.SetEnabled(true);
        }

        UIPackEditor.OnCardDataChanged?.Invoke();
    }

    public void HandleNoDecksButtonPress()
    {
        foreach (var widget in this.deckEditWidgets)
        {
            widget.SetEnabled(false);
        }

        UIPackEditor.OnCardDataChanged?.Invoke();
    }

    public void HandleAllCardTypesButtonPress()
    {
        foreach (var widget in this.cardTypeWidgets)
        {
            widget.SetEnabled(true);
        }

        UIPackEditor.OnCardDataChanged?.Invoke();
    }

    public void HandleNoCardTypesButtonPress()
    {
        foreach (var widget in this.cardTypeWidgets)
        {
            widget.SetEnabled(false);
        }

        UIPackEditor.OnCardDataChanged?.Invoke();
    }

    public void HandleShowHideInactiveCarsButtonPress(bool inShouldShow)
    {
        this.shouldShowInactiveCards = inShouldShow;
        this.hideInactiveCardsButton.SetActive(inShouldShow);
        UIPackEditor.OnCardDataChanged?.Invoke();
    }

    public void SetPasteButtonVisibility(bool inShouldShow)
    {
        this.pasteButton.SetActive(inShouldShow);
    }

    public void HandlePrevButtonPress()
    {
        var index = UIPackEditor.IN.PackCardItems.IndexOf(UIPackEditor.SelectedPackCardItem);
        if (index > 0)
        {
            --index;

            UIPackEditor.SelectedPackCardItem = UIPackEditor.IN.PackCardItems[index];

            if (UIPackEditor.SelectedPackCardItem.gameObject.activeInHierarchy)
            {
                UICardCloseUp.IN.Hide();
                UIPackEditor.SelectedPackCardItem = UIPackEditor.IN.PackCardItems[index];//Hide nulls out SelectedDeckItem, so must set it again
                UICardCloseUp.IN.Show(UIPackEditor.SelectedPackCardItem.Card);
            }
            else
                HandlePrevButtonPress();
        }
    }

    public void HandleNextButtonPress()
    {
        var index = UIPackEditor.IN.PackCardItems.IndexOf(UIPackEditor.SelectedPackCardItem);
        if (index < UIPackEditor.IN.PackCardItems.Count - 1)
        {
            ++index;
            
            UIPackEditor.SelectedPackCardItem = UIPackEditor.IN.PackCardItems[index];

            if (UIPackEditor.SelectedPackCardItem.gameObject.activeInHierarchy)
            {
                UICardCloseUp.IN.Hide();
                UIPackEditor.SelectedPackCardItem = UIPackEditor.IN.PackCardItems[index];//Hide nulls out SelectedDeckItem, so must set it again
                UICardCloseUp.IN.Show(UIPackEditor.SelectedPackCardItem.Card);
            }
            else
                HandleNextButtonPress();
        }
    }

    public override void Hide()
    {
        UIPackEditor.OnCardDataChanged -= RefreshDataDisplay;
        UICardBacksPanel.OnAttackNumbersChanged -= RefreshAttackNumbersWidgets;

        var isActiveDeckDirty = false;
        foreach (var deck in PlayerData.Data.PacksDict.Values)
        {
            foreach (var card in deck.CardDatas)
            {
                if (card.IsDirty)
                {
                    isActiveDeckDirty = true;
                    break;
                }
            }

            if (isActiveDeckDirty) break;
        }

        if (isActiveDeckDirty || PlayerData.Data.IsDirty)
        {
            if (this.EditingPack != null && !this.EditingPack.IsDefault)
            {
                SortDeckCards(this.EditingPack.CardDatas);
            }

            UIConfirmPanel.IN.Show("Reset Game?", "You have changed something in an active pack.\n\nWould you like to reset the game?", () =>
            {
                UIGameBoard.IN.ResetAllCards();
                DeckManager.IN.Init();
                PlayerData.Data.SetAllCardsClean(false);
                base.Hide();
            }, null, "Cancel", "Reset");
        }
        else
            base.Hide();
    }

    public static void SortDeckCards(List<CardData> cards)
    {
        cards.Sort((x, y) =>
        {
            int cardTypeComparison = x.CardType.CompareTo(y.CardType);
            if (cardTypeComparison != 0)
                return cardTypeComparison;

            int alignmentComparison = x.Alignment.CompareTo(y.Alignment);
            if (alignmentComparison != 0)
                return alignmentComparison;

            return x.Level.CompareTo(y.Level);
        });
    }

    public void HandleLockAttackNumbersButtonPress()
    {
        var isLocked = !this.attackNumbersLockToggleIcon.activeSelf;
        this.attackNumbersLockToggleIcon.SetActive(isLocked);

        SelectedCardData.AreAttackNumbersLocked = isLocked;
        PlayerData.Data.SetCardDirty(SelectedCardData);

        UIAttackNumbersWidget.OnAttackNumbersLockedChanged?.Invoke();
    }
}