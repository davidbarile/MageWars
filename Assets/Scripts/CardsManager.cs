using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using static Card;
using static UICardFlip;

public class CardsManager : MonoBehaviour
{
    public static CardsManager IN;

    public static readonly string ILLUSTRATIONS_PATH = "Illustrations";
    public static readonly string RESOURCE_ICONS_PATH = "Sprite Assets/TmPro_SpriteSheet";

    public int CardSpawnBatchSize = 10;
    public int MinDeckSize = 50;

    public List<CardConfig> AllCardConfigs = new();
    public List<CardData> AllCardDatas = new();

    [Space]
    [ReadOnly]
    public List<Card> SpawnedCards = new();
    [Space]
    public List<CardData> ActivePacksCardDatas = new();
    public List<int> DefaultCardCounts = new();
    [Space]
    [ReadOnly]
    public List<CardData> ShuffledCardDatas = new();

    [Space]
    [SerializeField] private Sprite[] attackNumbers;

    [Space]
    [SerializeField] private Sprite[] attackDecks;

    [Space]
    [SerializeField] private CardPrefabLookup[] cardPrefabLookups;
    public static Dictionary<ECardType, CardPrefabLookup> CardPrefabLookupDict = new();

    [Serializable]
    public class CardPrefabLookup
    {
        public ECardType CardType;
        public UICardFrontBase FrontPrefab;
        public UICardBackBase BackPrefab;
    }

    public static Dictionary<string, Sprite> SpritesDict = new();

    public void Init()
    {
        foreach (var item in this.cardPrefabLookups)
        {
            if (!CardPrefabLookupDict.ContainsKey(item.CardType))
                CardPrefabLookupDict.Add(item.CardType, item);
            else
                Debug.LogError($"<color=red>CardPrefabLookupDict already contains entry for CardType: {item.CardType}!</color>");
        }

        var allCardConfigs = Resources.LoadAll<CardConfig>("Configs/Cards");

        this.AllCardConfigs = allCardConfigs.ToList();
        this.AllCardConfigs.RemoveAll(x => x.CardType == ECardType.None);

        SortConfigs(this.AllCardConfigs);

        CreateResourceSpritesDict();
    }

    private void CreateResourceSpritesDict()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(RESOURCE_ICONS_PATH);

        foreach (Sprite sprite in sprites)
        {
            SpritesDict.Add(sprite.name, sprite);
        }

        print($"SpritesDict.Count: {SpritesDict.Count}");
    }

    public static void SortConfigs(List<CardConfig> cards)
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

    public void LoadDataFromCardConfigs()
    {
        this.AllCardDatas.Clear();

        for (int i = 0; i < this.AllCardConfigs.Count; i++)
        {
            var config = this.AllCardConfigs[i];
            var cardData = CardConfig.CreateCardData(config, i);
            this.AllCardDatas.Add(cardData);
        }
    }

    public void AddBatchOfCardsToGameDeck()
    {
        this.ShuffledCardDatas.RandomizeList();//TODO: think about this

        int numToSpawn = this.CardSpawnBatchSize - DeckManager.IN.GameDeck.Cards.Count;

        for (int i = 0; i < numToSpawn; i++)
        {
            if (this.ShuffledCardDatas.Count > 0)
            {
                var cardData = this.ShuffledCardDatas[0];
                CardsManager.SpawnCard(cardData, DeckManager.IN.GameDeck.transform, true);

                this.ShuffledCardDatas.RemoveAt(0);
            }
            else
            {
                Debug.Log("<color=red>ShuffledCardDatas Count = ZERO</color>");
                break;
            }
        }

        DeckManager.IN.GameDeck.RefreshCellsList();
    }

    public void GenerateShuffledGameDeck()
    {
        this.ShuffledCardDatas.Clear();

        foreach (var cardData in this.ActivePacksCardDatas)
        {
            for (int i = 0; i < cardData.Count; i++)
            {
                this.ShuffledCardDatas.Add(cardData);
            }
        }

        this.ShuffledCardDatas.RandomizeList();
    }

    public static Card SpawnCard(CardData inCardData, Transform inParent, bool inIsFaceDown, bool inIsDraggable = true, bool inShouldSnapToCenter = false)
    {
        var prefab = Resources.Load($"Card Prefab/Card");

        var cardGo = Instantiate(prefab, inParent) as GameObject;
        var card = cardGo.GetComponent<Card>();
        card.name = $"{inCardData.Name}";

        card.transform.SetParent(inParent);
        card.transform.localScale = Vector3.one;
        card.transform.rotation = Quaternion.identity;

        if (inIsFaceDown)
            card.CardFlip.SetAnimState(ECardFlipState.Back);
        else
            card.CardFlip.SetAnimState(ECardFlipState.Front);

        if (inShouldSnapToCenter)
            card.transform.localPosition = Vector3.zero;

        card.Draggable.IsDraggable = inIsDraggable;

        card.Configure(inCardData);

        CardsManager.IN.SpawnedCards.Add(card);

        return card;
    }

    public int GetNumCardsOfType(ECardType inCardType)
    {
        var count = 0;

        foreach (var cardData in CardsManager.IN.AllCardDatas)
        {
            if (cardData.CardType == inCardType)
                count += cardData.Count;
        }

        return count;
    }

    public void DestroyAllCards()
    {
        while (CardsManager.IN.SpawnedCards.Count > 0)
        {
            var card = CardsManager.IN.SpawnedCards[0];

            if (card != null)
                card.DestroySelf();
        }
    }

    public static Sprite GetSpriteFromName(ECardType inCardType, string inName)
    {
        var path = $"{CardsManager.ILLUSTRATIONS_PATH}/";

        if (inCardType != ECardType.None)
            path = $"{CardsManager.ILLUSTRATIONS_PATH}/{inCardType}/{inName}";

        var sprite = Resources.Load<Sprite>(path);

        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>($"{CardsManager.ILLUSTRATIONS_PATH}/Card Illus Default");

            // if (!string.IsNullOrWhiteSpace(inName))
            //     Debug.Log($"<color=red>Sprite not found for: {path}</color>");
        }

        return sprite;
    }

    public static Sprite GetResourceSpriteFromName(string inName)
    {
        Sprite sprite = null;

        if (SpritesDict.ContainsKey(inName))
            sprite = SpritesDict[inName];

        if (sprite == null && !string.IsNullOrWhiteSpace(inName))
            Debug.Log($"<color=red>Sprite not found for: {inName}</color>");

        return sprite;
    }

    public Sprite GetAttackNumberSprite(int inNumber)
    {
        if (inNumber < 0 || inNumber > 9)
        {
            Debug.Log($"<color=red>GetAttackNumberSprite({inNumber})  Invalid</color>");
            return null;
        }

        return this.attackNumbers[inNumber];
    }

    public Sprite GetAttackDeckSprite(int inDeckIndex)
    {
        return this.attackDecks[inDeckIndex];
    }
}