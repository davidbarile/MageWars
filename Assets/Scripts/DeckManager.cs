using System;
using UnityEngine;
using static Card;
using static DeckConfig;

public class DeckManager : MonoBehaviour
{
    public static DeckManager IN;

    [Serializable]
    public class DeckData
    {
        public UIDeck UiDeck;
        public DeckConfig DeckConfig;
    }

    public UICardCell GameDeck;
    [Space]
    [SerializeField] private DeckData[] deckDatas;
    public DeckData[] DeckDatas => this.deckDatas;

    [Header("Default Pack")]
    public Pack DefaultPack;

    public void Init()
    {
        DeckManager.SetActiveGameDecks();

        foreach (var deckData in this.deckDatas)
        {
            deckData.UiDeck.CardDatas.Clear();
            deckData.UiDeck.SetDeckName(deckData.DeckConfig.Name);

            foreach (var cardData in CardsManager.IN.ActivePacksCardDatas)
            {
                if (cardData.DeckCounts[(int)deckData.DeckConfig.DeckType] > 0)
                {
                    deckData.UiDeck.CardDatas.Add(cardData);
                }
            }

            deckData.UiDeck.Init();
        }
    }

    public static void SetActiveGameDecks()
    {
        CardsManager.IN.ActivePacksCardDatas.Clear();

        var counter = 0;

        GC.Collect();

        foreach (var kvp in PlayerData.Data.PacksDict)
        {
            var pack = kvp.Value;
            if (pack.IsActiveInGame)
            {
                for (int i = 0; i < pack.CardDatas.Count; i++)
                {
                    var card = pack.CardDatas[i];
                    CardsManager.IN.ActivePacksCardDatas.Add(card);
                    ++counter;
                }
            }
        }
    }

    public int GetNumCardsInDeck(EDeckType inDeckType)
    {
        var count = 0;

        foreach (var cardData in CardsManager.IN.ActivePacksCardDatas)
        {
            var deckCount = cardData.DeckCounts[(int)inDeckType];
            if (deckCount > -1)
                count += deckCount;
        }

        return count;
    }

    public int GetNumCurseCardsInDeck(EDeckType inDeckType)
    {
        var count = 0;

        foreach (var cardData in CardsManager.IN.ActivePacksCardDatas)
        {
            var deckCount = cardData.DeckCounts[(int)inDeckType];
            if (deckCount > -1 && cardData.CardType == ECardType.Curse)
                count += deckCount;
        }

        return count;
    }
}