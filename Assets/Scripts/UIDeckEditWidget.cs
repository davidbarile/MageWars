using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static DeckManager;

public class UIDeckEditWidget : MonoBehaviour
{
    public int Index { get; private set; }
    [SerializeField] private TextMeshProUGUI deckNameText;
    [SerializeField] private TextMeshProUGUI countText;

    [SerializeField] private Button subtractButton;
    [SerializeField] private Button addButton;
    [SerializeField] private GameObject disabledOverlay;
    [SerializeField] private GameObject dimmedOverlay;

    private int count;
    private DeckData deckData;
    private bool isCardViewMode;

    public bool IsEnabled => !this.disabledOverlay.activeSelf;

    public void Init(int inIndex, bool inIsCardViewMode)
    {
        this.Index = inIndex;
        this.isCardViewMode = inIsCardViewMode;

        this.deckData = DeckManager.IN.DeckDatas[inIndex];

        this.deckNameText.text = $"{this.deckData.DeckConfig.Name}";

        this.addButton.gameObject.SetActive(inIsCardViewMode);
        this.subtractButton.gameObject.SetActive(inIsCardViewMode);

        this.dimmedOverlay.SetActive(false);

        SetEnabled(true);
    }

    public void SetCount(int inCount, int inCurseCount = 0)
    {
        this.count = inCount;

        this.countText.text = $"{inCount}";

        if (inCurseCount > 0)
        {
            var percent = (float)inCurseCount / inCount * 100;
            this.deckNameText.text = $"<color=black>{this.deckData.DeckConfig.Name}</color>  <color=#EE0000>{inCurseCount}</color>  <i><color=#FF9999>({percent:N1}%)</color></i>";
        }

        //var minCount = this.deckData.DeckConfig.DeckType == DeckConfig.EDeckType.Mage ? 1 : 0;
        this.subtractButton.interactable = inCount > 0;

        if (this.isCardViewMode)
            this.dimmedOverlay.SetActive(inCount == 0);
    }

    public void HandleClick()
    {
        if (this.isCardViewMode)
        {
            //print($"DeckEditWidget: HandleClick: {this.Index}");
        }
        else
        {
            SetEnabled(!this.IsEnabled);
            UIPackEditor.OnCardDataChanged?.Invoke();
        }
    }

    public void SetEnabled(bool inIsEnabled)
    {
        this.disabledOverlay.SetActive(!inIsEnabled);
    }

    public void HandleSubstractButtonClick()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        var oldValue = GenerateDeckList(UIPackEditor.SelectedCardData.DeckCounts);

        var newCount = UIPackEditor.SelectedCardData.AddCountInDeck(this.deckData.DeckConfig.DeckType, -1);
        SetCount(newCount);

        //if removed from all decks, it should be added to the None deck
        if (UIPackEditor.SelectedCardData.GetCountInAllDecks() == 0)
            UIPackEditor.SelectedCardData.DeckCounts[0] = 1;

        var newValue = GenerateDeckList(UIPackEditor.SelectedCardData.DeckCounts);

        PlayerData.Data.SetEditNote(UIPackEditor.SelectedCardData, EEditNoteType.Decks.ToString(), oldValue, newValue);

        UIEditNotesPanel.OnPlayerDataChanged?.Invoke();

        UIPackEditor.OnCardDataChanged?.Invoke();

        Invoke(nameof(ScrollToBottom), .1f);
    }

    public void HandleAddButtonClick()
    {
        if (UIPackEditor.ShouldBlockEdit) return;

        var oldValue = GenerateDeckList(UIPackEditor.SelectedCardData.DeckCounts);

        var newCount = UIPackEditor.SelectedCardData.AddCountInDeck(this.deckData.DeckConfig.DeckType, 1);
        SetCount(newCount);

        //once added to any deck, it should be removed from the None deck
        if (UIPackEditor.SelectedCardData.DeckCounts[0] > 0)
            UIPackEditor.SelectedCardData.DeckCounts[0] = -1;

        var newValue = GenerateDeckList(UIPackEditor.SelectedCardData.DeckCounts);

        PlayerData.Data.SetEditNote(UIPackEditor.SelectedCardData, EEditNoteType.Decks.ToString(), oldValue, newValue);
        UIEditNotesPanel.OnPlayerDataChanged?.Invoke();

        UIPackEditor.OnCardDataChanged?.Invoke();

        Invoke(nameof(ScrollToBottom), .1f);
    }

    private void ScrollToBottom()
    {
        UICardCloseUp.IN.ScrollToBottom();
    }

    //lots of off-by-one fun!
    private List<string> GenerateDeckList(int[] deckCounts)
    {
        var deckList = new List<string>();
        for (int i = 1; i <= DeckManager.IN.DeckDatas.Length; i++)
        {
            var deckData = DeckManager.IN.DeckDatas[i - 1];
            var numCardsInDeck = deckCounts[(int)deckData.DeckConfig.DeckType];

            if (numCardsInDeck > -1)
            {
                var deckConfig = deckData.DeckConfig;
                deckList.Add($"<size=30>{deckConfig.Name}: {deckCounts[i]}</size>");
            }
        }

        return deckList;
    }
}