using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static DeckConfig;

public class UIAttackNumbersWidget : MonoBehaviour
{
    public static Action OnAttackNumbersLockedChanged;
    public int CardNum { get; private set; }
    public int DeckNum { get; private set; }

    [SerializeField] private TextMeshProUGUI attNumText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI deckNameText;
    [SerializeField] private Button subtractButton;
    [SerializeField] private Button addButton;

    [SerializeField] private GameObject lockedIcon;

    public EDeckType DeckType { get; private set; }

    public int AttackNum {get; private set;}

    private bool isCardViewMode;

    private CardData cardData;

    public void Init(CardData inCardData, int inDeckNum, int inCardNum, bool inIsCardViewMode)
    {
        this.cardData = inCardData;
        this.CardNum = inCardNum;
        this.DeckNum = inDeckNum;
        this.isCardViewMode = inIsCardViewMode;
        this.countText.transform.parent.gameObject.SetActive(this.isCardViewMode);
        RefreshLockedButtons();

        OnAttackNumbersLockedChanged += RefreshLockedButtons;
    }

    private void OnDestroy()
    {
        OnAttackNumbersLockedChanged -= RefreshLockedButtons;
    }

    private void RefreshLockedButtons()
    {
        var locked = this.cardData.AreAttackNumbersLocked;

        this.addButton.interactable = !locked;
        this.subtractButton.interactable = !locked;

        this.addButton.gameObject.SetActive(!this.isCardViewMode);
        this.subtractButton.gameObject.SetActive(!this.isCardViewMode);

        this.lockedIcon.SetActive(this.isCardViewMode && locked);
        this.lockedIcon.SetActive(locked);
    }

    public void SetCount(int inCount)
    {
        this.countText.text = $"x{inCount}";
    }

    public void SetAttackNum(int inAttackNum)
    {
        this.AttackNum = inAttackNum;
        this.attNumText.text = inAttackNum.ToString();

        var locked = this.cardData.AreAttackNumbersLocked;
        if (locked) return;

        this.subtractButton.interactable = inAttackNum > 0;
        this.addButton.interactable = inAttackNum < 9;
    }

    public void SetDeck(EDeckType inDeckType)
    {
        this.DeckType = inDeckType;
        var deckName = inDeckType.ToString();
        this.deckNameText.text = deckName;
    }

    public void HandleChangeButtonClick(int inAmount)
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        var oldValue = ConvertListToString(UIPackEditor.SelectedCardData.AttackNumbersObjects[this.DeckNum].AttackNumValues);

        UIPackEditor.SelectedCardData.AttackNumbersObjects[this.DeckNum].AttackNumValues[this.CardNum] += inAmount;

        var newCount = UIPackEditor.SelectedCardData.AttackNumbersObjects[this.DeckNum].AttackNumValues[this.CardNum];

        SetAttackNum(newCount);

        var newValue = ConvertListToString(UIPackEditor.SelectedCardData.AttackNumbersObjects[this.DeckNum].AttackNumValues);

        PlayerData.Data.SetEditNote(UIPackEditor.SelectedCardData, EEditNoteType.AttackNumbers.ToString(), oldValue, newValue);

        UIEditNotesPanel.OnPlayerDataChanged?.Invoke();

        UIPackEditor.OnCardDataChanged?.Invoke();

        UICardCloseUp.IN.ScrollToBottom();

        PlayerData.Data.SetCardDirty(UIPackEditor.SelectedCardData);
    }

    public static string ConvertListToString(List<int> inList)
    {
        var result = string.Empty;

        for (int i = 0; i < inList.Count; i++)
        {
            result += inList[i].ToString();

            if (i < inList.Count - 1)
            {
                result += ", ";
            }
        }

        return result;
    }
}
