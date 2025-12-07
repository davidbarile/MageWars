using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using static Card;
using static CardConfig;
using static DeckConfig;

[Serializable]
public class CardData : CardDataBase
{
    [Serializable]
    public class AttackNums
    {
        public EDeckType DeckType;
        public List<int> AttackNumValues = new();
    }

    [Header("Custom Card Data")]
    public string CardName;
    public string CardSubtitle;

    [Space]
    public ECardType CardType;
    [ShowIf("@CardType == Card.ECardType.Mage || CardType == Card.ECardType.Structure")]
    public EAlignment Alignment;

    [Space]
    public EElement Elements;

    [Space]
    [HideIf("@CardType == Card.ECardType.Mage || CardType == Card.ECardType.Fate")]
    [Range(1, 5)] public int Level = 1;

    [Space]
    public EDeckType AttackDeck;
    public EDeckType DefenseDeck;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Mage || CardType == Card.ECardType.Item || CardType == Card.ECardType.Creature || CardType == Card.ECardType.Structure")]
    [Range(1, 20)] public int MaxHealth = 5;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Mage")]
    [Header("0=Enchanted, 1=Mystic, 2=Arcane, 3=Quest")]
    public int[] HealthCards = new int[4];

    [Space]
    public string IllustrationSpriteName;
    [ShowIf("@CardType == Card.ECardType.Fate")]
    public string SecondarySpriteName;

    [Space]
    [TextArea(1, 10)]
    public string Description;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Challenge")]
    [TextArea(1, 10)]
    public string Reward;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Challenge")]
    [TextArea(1, 10)]
    public string Consequence;

    public HorizontalAlignmentOptions DescriptionAlignment = HorizontalAlignmentOptions.Left;
    public bool ShouldGenerateDescription = true;
    [Range(8, 100)] public int DescriptionFontSize = 0;

    [Space]
    public List<string> ResourceNames;
    public List<string> RequirementNames;
    public string PermanentResourceName;
    [Header("1=Mage, 2=Fate, 3=Enchanted, 4=Mystic, 5=Arcane, 6=Quest")]
    public int[] DeckCounts = new int[(int)EDeckType.Count];//do not set from code, use SetCountInDeck() or AddCountInDeck()

    //Mage vars
    [Header("Mage Data")]
    [ShowIf("CardType", ECardType.Mage)]
    [Range(1, 10)] public int DrawCardsPerTurn = 1;

    [ShowIf("CardType", ECardType.Mage)]
    [Range(5, 20)] public int MaxCardsInHand = 10;

    [ShowIf("CardType", ECardType.Mage)]
    [Range(1, 5)] public int MaxArmor = 1;

    [ShowIf("CardType", ECardType.Mage)]
    [Range(0, 5)] public int MaxPets = 0;

    [ShowIf("CardType", ECardType.Mage)]
    [Range(0, 5)] public int MaxPlants = 0;

    [ShowIf("CardType", ECardType.Mage)]
    [Range(0, 5)] public int MaxItems = 0;

    [ShowIf("CardType", ECardType.Mage)]
    public string WinCondition;

    [Header("Card Backs")]
    public List<AttackNums> AttackNumbersObjects = new();
    public bool AreAttackNumbersLocked;

    public int AddHealthCards(EDeckType inDeckType, int inCount)
    {
        var index = UICardBacksPanel.GetDeckIndex(inDeckType);

        if (inCount < 0 && -inCount > this.HealthCards[index])
        {
            inCount = -this.HealthCards[index];
        }

        this.HealthCards[index] += inCount;
        this.MaxHealth += inCount;

        PlayerData.Data.SetCardDirty(this);

        return this.HealthCards[index];
    }

    public int AddCountInDeck(EDeckType inDeckType, int inCount)
    {
        if (inCount < 0 && -inCount > this.DeckCounts[(int)inDeckType])
        {
            inCount = -this.DeckCounts[(int)inDeckType];
        }

        this.DeckCounts[(int)inDeckType] += inCount;
        this.Count += inCount;

        PlayerData.Data.SetCardDirty(this);

        return this.DeckCounts[(int)inDeckType];
    }

    public void ClearDeckCounts()
    {
        for (int i = 0; i < this.DeckCounts.Length; i++)
        {
            this.DeckCounts[i] = 0;
        }

        this.DeckCounts[0] = 1;

        this.Count = 0;

        PlayerData.Data.SetCardDirty(this);
    }

    public int GetCountInAllDecks()
    {
        var count = 0;

        for (int i = 1; i < this.DeckCounts.Length; i++)
        {
            if (this.DeckCounts[i] > 0)
                count += this.DeckCounts[i];
        }

        return count;
    }

    public virtual void CopyDataFrom(CardData inData)
    {
        this.ID = inData.ID;
        this.Count = inData.Count;
        this.Name = inData.Name;

        this.CardFrontPrefabName = inData.CardFrontPrefabName;
        this.CardBackPrefabName = inData.CardBackPrefabName;

        this.CardName = inData.CardName;
        this.CardSubtitle = inData.CardSubtitle;
        this.CardType = inData.CardType;
        this.Alignment = inData.Alignment;
        this.Elements = inData.Elements;
        this.Level = inData.Level;
        this.AttackDeck = inData.AttackDeck;
        this.DefenseDeck = inData.DefenseDeck;
        this.MaxHealth = inData.MaxHealth;
        this.IllustrationSpriteName = inData.IllustrationSpriteName;
        this.SecondarySpriteName = inData.SecondarySpriteName;
        this.Description = inData.Description;
        this.DescriptionAlignment = inData.DescriptionAlignment;
        this.ShouldGenerateDescription = inData.ShouldGenerateDescription;
        this.DescriptionFontSize = inData.DescriptionFontSize;
        this.ResourceNames = new List<string>(inData.ResourceNames);
        this.RequirementNames = new List<string>(inData.RequirementNames);
        this.PermanentResourceName = inData.PermanentResourceName;
        this.DeckCounts = (int[])inData.DeckCounts.Clone();

        //mage
        this.DrawCardsPerTurn = inData.DrawCardsPerTurn;
        this.MaxCardsInHand = inData.MaxCardsInHand;
        this.HealthCards = (int[])inData.HealthCards.Clone();
        this.MaxArmor = inData.MaxArmor;
        this.MaxPets = inData.MaxPets;
        this.MaxPlants = inData.MaxPlants;
        this.MaxItems = inData.MaxItems;
        this.WinCondition = inData.WinCondition;

        //card backs
        this.AttackNumbersObjects = new List<AttackNums>(inData.AttackNumbersObjects);
        this.AreAttackNumbersLocked = inData.AreAttackNumbersLocked;

        PlayerData.Data.SetCardDirty(this);
    }

    public CardData Clone(CardData inSource)
    {
        var cardData = new CardData
        {
            ID = inSource.ID,
            Count = inSource.Count,
            Index = inSource.Index,
            Name = inSource.Name,
            CardFrontPrefabName = inSource.CardFrontPrefabName,
            CardBackPrefabName = inSource.CardBackPrefabName,
            CardName = inSource.CardName,
            CardSubtitle = inSource.CardSubtitle,
            CardType = inSource.CardType,
            Elements = inSource.Elements,
            Level = inSource.Level,
            AttackDeck = inSource.AttackDeck,
            DefenseDeck = inSource.DefenseDeck,
            MaxHealth = inSource.MaxHealth,
            DeckCounts = (int[])inSource.DeckCounts.Clone(),
            ResourceNames = new List<string>(inSource.ResourceNames),
            RequirementNames = new List<string>(inSource.RequirementNames),
            PermanentResourceName = inSource.PermanentResourceName,
            Description = inSource.Description,
            DescriptionAlignment = inSource.DescriptionAlignment,
            DescriptionFontSize = inSource.DescriptionFontSize,
            ShouldGenerateDescription = inSource.ShouldGenerateDescription,
            IllustrationSpriteName = inSource.IllustrationSpriteName,
            SecondarySpriteName = inSource.SecondarySpriteName,

            //mage attributes
            Alignment = inSource.Alignment,
            DrawCardsPerTurn = inSource.DrawCardsPerTurn,
            MaxCardsInHand = inSource.MaxCardsInHand,
            HealthCards = (int[])inSource.HealthCards.Clone(),
            MaxArmor = inSource.MaxArmor,
            MaxPets = inSource.MaxPets,
            MaxPlants = inSource.MaxPlants,
            MaxItems = inSource.MaxItems,
            WinCondition = inSource.WinCondition,

            //challenge attributes
            Reward = inSource.Reward,
            Consequence = inSource.Consequence,

            //card backs
            AttackNumbersObjects = new List<AttackNums>(inSource.AttackNumbersObjects),
            AreAttackNumbersLocked = inSource.AreAttackNumbersLocked
        };

        return cardData;
    }
}