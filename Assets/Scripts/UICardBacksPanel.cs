using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static CardData;
using static DeckConfig;

public class UICardBacksPanel : UIPanelBase
{
    public static UICardBacksPanel IN;

    public static Action OnAttackNumbersChanged;
    [SerializeField] private TextMeshProUGUI currentPackText;
    [SerializeField] private GameObject attackNumbersLockToggleIcon;

    [Space, SerializeField] private Transform attackNumbersParent;
    [SerializeField] private UIAttackNumbersWidget attackNumbersWidgetPrefab;
    [SerializeField] private UIAttackNumbersCategory attackNumbersCategoryPrefab;

    [Space]
    [SerializeField] private UIDeckBalanceWidget[] deckBalanceWidgets;
    [SerializeField] private GameObject deckBalancesToggleIcon;
    [Space]
    [SerializeField] private TextMeshProUGUI countText;

    private bool allDecksValid;

    private bool shouldClearLocked => this.attackNumbersLockToggleIcon.activeSelf;

    private bool wasOpened;

    private EDeckType[] decksArray = new EDeckType[]
    {
        EDeckType.Enchanted,
        EDeckType.Mystic,
        EDeckType.Arcane,
        EDeckType.Quest
    };

    private List<List<CardData>> deckCardDatas = new();

    public static int GetDeckIndex(EDeckType inDeckType)
    {
        switch (inDeckType)
        {
            case EDeckType.Enchanted:
                return 0;
            case EDeckType.Mystic:
                return 1;
            case EDeckType.Arcane:
                return 2;
            case EDeckType.Quest:
                return 3;
        }

        return -1;
    }

    public static EDeckType GetDeckEnum(int inDeckIndex)
    {
        switch (inDeckIndex)
        {
            case 0:
                return EDeckType.Enchanted;
            case 1:
                return EDeckType.Mystic;
            case 2:
                return EDeckType.Arcane;
            case 3:
                return EDeckType.Quest;
        }

        return EDeckType.None;
    }

    public static int GetCountInDeck(EDeckType inDeckType)
    {
        var cardDatas = UIPackEditor.IN.EditingPack.CardDatas;

        var count = 0;

        for (int i = 0; i < cardDatas.Count; i++)
        {
            var cardData = cardDatas[i];
            var deckCount = cardData.DeckCounts[(int)inDeckType];
            if (deckCount > 0)
                count += deckCount;
        }

        return count;
    }

    public static List<CardData> GetCardsOfDeck(List<CardData> inCardDatas, EDeckType inDeckType)
    {
        var cardsOfDeckType = new List<CardData>();

        foreach (var cardData in inCardDatas)
        {
            var deckCount = cardData.DeckCounts[(int)inDeckType];

            if (deckCount > 0)
                cardsOfDeckType.Add(cardData);
        }

        return cardsOfDeckType;
    }

    public override void Show()
    {
        base.Show();

        this.wasOpened = true;

        //create list of lists of card datas for each deck type
        this.deckCardDatas.Clear();

        foreach (var deckType in this.decksArray)
        {
            var cardDatas = GetCardsOfDeck(UIPackEditor.IN.EditingPack.CardDatas, deckType);
            this.deckCardDatas.Add(cardDatas);
        }

        OnAttackNumbersChanged += RefreshAttackNumbersWidgets;
        RefreshAttackNumbersWidgets();

        var pack = UIPackEditor.IN.EditingPack;

        for (var i = 0; i < this.deckBalanceWidgets.Length; i++)
        {
            var widget = this.deckBalanceWidgets[i];
            var preset = pack.AttackNumsPresets[i];
            widget.Init(RefreshValidDeckCounts, preset.Counts);
        }
    }

    private void RefreshValidDeckCounts()
    {
        var validDecksCount = 0;
        var nonZeroWidgets = 0;
        for (var i = 0; i < this.deckBalanceWidgets.Length; i++)
        {
            var widget = this.deckBalanceWidgets[i];

            if (widget.MaxCount > 0)
                ++nonZeroWidgets;

            if (widget.Count == widget.MaxCount && widget.MaxCount > 0)
                ++validDecksCount;
        }

        this.allDecksValid = validDecksCount == nonZeroWidgets && nonZeroWidgets > 0;

        var colorString = this.allDecksValid ? "#00FF00" : "red";
        this.countText.text = $"Balanced Decks: <color={colorString}>{validDecksCount}/{nonZeroWidgets}</color>";
    }

    public override void Hide()
    {
        SaveData();

        base.Hide();

        OnAttackNumbersChanged -= RefreshAttackNumbersWidgets;
    }

    public void SaveData()
    {
        if (!this.wasOpened) return;

        var pack = UIPackEditor.IN.EditingPack;
        for (var i = 0; i < this.deckBalanceWidgets.Length; i++)
        {
            var widget = this.deckBalanceWidgets[i];

            for (var j = 0; j < widget.AttackNumsSliders.Length; j++)
            {
                var slider = widget.AttackNumsSliders[j];
                pack.AttackNumsPresets[i].Counts[j] = slider.Count;
            }
        }

        PlayerData.Data.SetDirty();
    }

    private void RegeneratePackAttackNums()
    {
        var numsToAdd = new List<List<int>>();

        for (int i = 0; i < this.decksArray.Length; i++)
        {
            numsToAdd.Add(new List<int>());

            var deckBalanceWidget = this.deckBalanceWidgets[i];

            for (int l = 0; l < deckBalanceWidget.AttackNumsSliders.Length; l++)
            {
                var count = deckBalanceWidget.AttackNumsSliders[l].Count;

                for (int m = 0; m < count; m++)
                {
                    var attackNum = l + 1;
                    numsToAdd[i].Add(attackNum);
                }
            }
        }

        for (int i = 0; i < this.decksArray.Length; i++)
        {
            var deckType = decksArray[i];
            var deckIndex = (int)deckType;
            var cardDatas = this.deckCardDatas[i];

            numsToAdd[i].RandomizeList();

            foreach (var card in cardDatas)
            {
                if (card.AreAttackNumbersLocked) continue;

                var numCardsInDeck = card.DeckCounts[deckIndex];

                if (numCardsInDeck < 1) continue;

                card.AttackNumbersObjects[i].AttackNumValues.Clear();

                for (int l = 0; l < numCardsInDeck; l++)
                {
                    card.AttackNumbersObjects[i].AttackNumValues.Add(numsToAdd[i][0]);
                    numsToAdd[i].RemoveAt(0);
                }
            }
        }

        OnAttackNumbersChanged?.Invoke();
    }

    private void ClearAllAttackNums()
    {
        foreach (var kvp in PlayerData.Data.PacksDict)
        {
            var pack = kvp.Value;
            ClearAttackNumsForPack(pack);
        }

        OnAttackNumbersChanged?.Invoke();
    }

    private void ClearPackAttackNums()
    {
        ClearAttackNumsForPack(UIPackEditor.IN.EditingPack);
    }

    private void ClearAttackNumsForPack(Pack inPack)
    {
        foreach (var cardData in inPack.CardDatas)
        {
            if (!this.shouldClearLocked && cardData.AreAttackNumbersLocked) continue;

            cardData.AreAttackNumbersLocked = false;

            foreach (var attackNums in cardData.AttackNumbersObjects)
            {
                attackNums.AttackNumValues = new List<int>(1);
            }
        }

        OnAttackNumbersChanged?.Invoke();
    }

    private void RefreshAttackNumbersWidgets()
    {
        this.currentPackText.text = UIPackEditor.IN.EditingPack.Name;

        this.attackNumbersParent.DestroyAllChildren<UIAttackNumbersWidget>();
        this.attackNumbersParent.DestroyAllChildren<UIAttackNumbersCategory>();

        var attackNumWidgetDatas = new List<AttackNumWidgetData>();
        var existingWidgetsDict = new Dictionary<string, AttackNumWidgetData>();

        PrepareCardAttackNumbers();

        var deckType = EDeckType.None;

        for (int i = 0; i < this.decksArray.Length; i++)
        {
            deckType = decksArray[i];
            var deckIndex = (int)deckType;
            var cardDatas = this.deckCardDatas[i];

            var count = GetCountInDeck(deckType);

            for (int j = 0; j < cardDatas.Count; j++)
            {
                var cardData = cardDatas[j];

                var deckCount = cardData.DeckCounts[deckIndex];

                if (deckCount < 1) continue;

                for (int k = 0; k < deckCount; k++)
                {
                    var attackNumValues = cardData.AttackNumbersObjects[i].AttackNumValues;

                    var attackNum = attackNumValues[k];

                    if (attackNum == 0) continue;

                    var key = $"{deckType}_{attackNum}";

                    if (existingWidgetsDict.ContainsKey(key))
                    {
                        ++existingWidgetsDict[key].Count;
                        continue;
                    }

                    var widgetData = new AttackNumWidgetData
                    {
                        DeckType = deckType,
                        CardData = cardData,
                        DeckIndex = i,
                        CardNum = k,
                        Count = 1,
                        AttackNumber = attackNumValues[k]
                    };

                    existingWidgetsDict.Add(key, widgetData);
                    attackNumWidgetDatas.Add(widgetData);
                }
            }
        }

        attackNumWidgetDatas.Sort((a, b) =>
        {
            int deckTypeComparison = a.DeckType.CompareTo(b.DeckType);

            if (deckTypeComparison == 0)
                return a.AttackNumber.CompareTo(b.AttackNumber);

            return deckTypeComparison;
        });

        deckType = EDeckType.None;

        for (int k = 0; k < attackNumWidgetDatas.Count; k++)
        {
            var data = attackNumWidgetDatas[k];

            if (data.Count == 0) continue;

            if (deckType != data.DeckType)
            {
                deckType = data.DeckType;
                var category = Instantiate(this.attackNumbersCategoryPrefab, this.attackNumbersParent);
                category.DeckNameText.text = $"{deckType}:";
            }

            var widget = Instantiate(this.attackNumbersWidgetPrefab, this.attackNumbersParent);
            widget.Init(data.CardData, data.DeckIndex, k, true);
            widget.SetDeck(data.DeckType);
            widget.SetCount(data.Count);
            widget.SetAttackNum(data.AttackNumber);
        }

        PlayerData.Data.SetDirty();
    }

    private static void PrepareCardAttackNumbers()
    {
        var cardDatas = UIPackEditor.IN.EditingPack.CardDatas;

        for (int i = 0; i < cardDatas.Count; i++)
        {
            var cardData = cardDatas[i];

            var deckIndex = 0;//this is the index of the attack number object (corresponds to the deck type)

            for (int j = 0; j < cardData.DeckCounts.Length; j++)
            {
                var deckType = (EDeckType)j;
                var deckAllowsAttackNumbers = deckType == EDeckType.Enchanted ||
                deckType == EDeckType.Mystic || deckType == EDeckType.Arcane ||
                deckType == EDeckType.Quest;

                if (!deckAllowsAttackNumbers) continue;

                var countInDeck = cardData.DeckCounts[j];

                if (countInDeck < 1) continue;

                //if not enough attack number objects, add blank ones
                while (cardData.AttackNumbersObjects.Count < 4)
                {
                    cardData.AttackNumbersObjects.Add(new AttackNums
                    {
                        DeckType = deckType,
                        AttackNumValues = new List<int>()
                    });
                }

                var attackNumValues = cardData.AttackNumbersObjects[deckIndex].AttackNumValues;
                while (attackNumValues.Count <= countInDeck)
                {
                    attackNumValues.Add(0);
                }

                ++deckIndex;
            }
        }
    }

    public void HandleChangePackButtonPress()
    {
        SaveData();
        OnAttackNumbersChanged -= RefreshAttackNumbersWidgets;
        UIPackListPanel.IN.Show();
    }

    public void HandleRegenPackButtonPress()
    {
        if (this.allDecksValid)
        {
            UIConfirmPanel.IN.Show("Regenerate Attack Numbers", "Are you sure you want to regenerate all Attack Numbers on Card Backs for this Pack?", () =>
            {
                RegeneratePackAttackNums();
            });
        }
        else
        {
            UIConfirmPanel.IN.Show("Invalid Decks", "All Decks must have 100% Attack Numbers to Regenerate.", null);
        }
    }

    public void HandleClearPackButtonPress()
    {
        UIConfirmPanel.IN.Show("Clear Card Attack Numbers", "Are you sure you want to clear the Attack Numbers on Card Backs for this Pack?", () =>
        {
            ClearPackAttackNums();
        });
    }

    public void HandleClearAllButtonPress()
    {
        UIConfirmPanel.IN.Show("Clear All Attack Numbers", "Are you sure you want to clear the Attack Numbers on Card Backs for ALL Packs?", () =>
        {
            ClearAllAttackNums();
        });
    }

    public void HandleResetWidgetsButtonPress()
    {
        UIConfirmPanel.IN.Show("Reset All Widgets", "Are you sure you want to reset all Attack Number Widgets?", () =>
        {
            foreach (var widget in this.deckBalanceWidgets)
            {
                widget.Reset();
            }
        });
    }

    public void HandleLockAttackNumbersButtonPress()
    {
        var isLocked = !this.attackNumbersLockToggleIcon.activeSelf;
        this.attackNumbersLockToggleIcon.SetActive(isLocked);
    }

    public void HandleToggleDeckBalancesButtonPress()
    {
        var shouldHide = !this.deckBalancesToggleIcon.activeSelf;
        this.deckBalancesToggleIcon.SetActive(shouldHide);

        foreach (var widget in this.deckBalanceWidgets)
        {
            widget.gameObject.SetActive(shouldHide);
        }
    }
}

[Serializable]
public class AttackNumWidgetData
{
    public EDeckType DeckType;
    public CardData CardData;
    public int DeckIndex;
    public int CardNum;
    public int AttackNumber;
    public int Count;
}

[Serializable]
public class DeckCountData
{
    public EDeckType DeckType;
    public int Count;
}