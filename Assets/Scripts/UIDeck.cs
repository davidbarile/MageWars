using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using static DeckConfig;

public class UIDeck : MonoBehaviour
{
    public bool ShouldShuffleCards = true;
    public bool ShouldLoadCardsFromConfigs;
    [Space]
    [SerializeField] private TextMeshProUGUI deckNameText;

    public EDeckType DeckType;
    [Range(1, 5)] public int DeckLevel = 1;

    [Space]
    public List<CardData> CardDatas = new();
    [ReadOnly] public List<Card> Cards = new();

    private List<int> attackNumbers = new();

    private void Awake()
    {
        this.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void Init()
    {
        var deckIndex = -1;

        var deckAllowsAttackNumbers = this.DeckType == EDeckType.Enchanted ||
                this.DeckType == EDeckType.Mystic || this.DeckType == EDeckType.Arcane ||
                this.DeckType == EDeckType.Quest;

        if (this.ShouldLoadCardsFromConfigs)
        {
            this.transform.DestroyAllChildren<Card>();

            this.GetComponent<CanvasGroup>().alpha = 1;

            this.Cards.Clear();

            deckIndex = UICardBacksPanel.GetDeckIndex(this.DeckType);

            foreach (var cardData in this.CardDatas)
            {
                var countInDeck = cardData.DeckCounts[(int)this.DeckType];

                for (int i = 0; i < countInDeck; i++)
                {
                    var card = CardsManager.SpawnCard(cardData, this.transform, true);
                    card.transform.localScale = Vector3.one;
                    card.name = $"{card.name}_{i}";

                    card.Configure(cardData);
                    card.SpawnCardBack(this.DeckType);
                    card.ParentDeck = this;
                    card.SetFlipCornerVisible(true);

                    if (deckAllowsAttackNumbers)
                    {
                        if (cardData.AttackNumbersObjects[deckIndex].AttackNumValues.Count <= i)
                            cardData.AttackNumbersObjects[deckIndex].AttackNumValues.Add(0);

                        card.CardBack.SetAttackNumber(cardData.AttackNumbersObjects[deckIndex].AttackNumValues[i]);
                    }

                    this.Cards.Add(card);
                }
            }
        }

        if (!this.ShouldLoadCardsFromConfigs && this.Cards.Count == 0)
        {
            var cards = this.GetComponentsInChildren<Card>(true);

            foreach (var card in cards)
            {
                this.Cards.Add(card);
            }
        }

        if (this.ShouldShuffleCards)
        {
            this.Cards.RandomizeList();

            for (int i = 0; i < this.Cards.Count; i++)
            {
                this.Cards[i].transform.SetSiblingIndex(i);
            }
        }

        if (deckAllowsAttackNumbers)
        {
            this.attackNumbers.Clear();

            deckIndex = UICardBacksPanel.GetDeckIndex(this.DeckType);

            foreach (var card in this.Cards)
            {
                var cardAttackNums = card.Data.AttackNumbersObjects[deckIndex].AttackNumValues;
                foreach (var attackNum in cardAttackNums)
                {
                    if (attackNum > 0)
                        this.attackNumbers.Add(attackNum);
                }
            }
        }

        RefreshVisibleCardCount();
    }

    public int GetRandomAttackNumber()
    {
        //print($"GetRandomAttackNumber({this.DeckType})  AttackNumbers: {string.Join(", ", this.attackNumbers)}");

        if (this.attackNumbers.Count == 0)
            return 0;

        return this.attackNumbers[UnityEngine.Random.Range(0, this.attackNumbers.Count)];
    }

    public void SetDeckName(string inDeckName)
    {
        if (this.deckNameText != null)
            this.deckNameText.text = inDeckName;
    }

    public void RefreshVisibleCardCount()
    {
        var cardCell = this.GetComponent<UICardCell>();

        for (int i = 0; i < this.Cards.Count; i++)
        {
            var card = this.Cards[i];

            bool shouldShow = cardCell.MaxVisibleCardsInDeck == 0 || i >= this.Cards.Count - cardCell.MaxVisibleCardsInDeck;
            card.gameObject.SetActive(shouldShow);
        }
    }

    public void RemoveCardFromDeck(Card inCard)
    {
        this.Cards.Remove(inCard);

        RefreshVisibleCardCount();
    }

    public void AddCardToDeck(Card inCard)
    {
        inCard.CardFlip.SetAnimStateToBack();
        this.Cards.Insert(0, inCard);
        inCard.transform.SetParent(this.transform);
        inCard.transform.SetAsFirstSibling();

        inCard.transform.localScale = Vector3.one;

        if (this.Cards.Count > 1)
        {
            //remove top card
            var lastIndex = this.Cards.Count - 1;
            var topCard = this.Cards[lastIndex];

            this.Cards.Remove(topCard);

            this.Cards.RandomizeList();

            //add top card back on top
            this.Cards.Add(topCard);
        }

        //sort cards in hierarchy
        for (int i = 0; i < this.Cards.Count; i++)
        {
            var card = this.Cards[i];
            card.transform.SetSiblingIndex(i);
        }

        RefreshVisibleCardCount();
    }
}