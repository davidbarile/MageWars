using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using static Card;
using static DeckConfig;
using static CardData;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/CardConfig", order = 1)]
public class CardConfig : CardConfigBase
{
    public enum EAlignment
    {
        None,
        Good,
        Neutral,
        Evil
    }

    [Header("Card Config")]
    [ReadOnly]
    public string ConfigID;
    public string CardName;
    [Header("If left blank, will default to CardType")]
    public string CardSubtitle;
    [Space]
    public ECardType CardType;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Mage || CardType == Card.ECardType.Structure")]
    public EAlignment Alignment;
    [Space]
    [HideIf("@CardType == Card.ECardType.Level || CardType == Card.ECardType.Fate")]
    public EElement Elements;

    [Space]
    [Range(1, 5)]
    [HideIf("@CardType == Card.ECardType.Mage || CardType == Card.ECardType.Fate")]
    public int Level = 1;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Magic || CardType == Card.ECardType.Item || CardType == Card.ECardType.Creature")]
    public EAttackDeckType AttackDeck;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Structure || CardType == Card.ECardType.Creature")]
    public EAttackDeckType DefenseDeck;

    [ShowIf("@CardType == Card.ECardType.Mage || CardType == Card.ECardType.Item || CardType == Card.ECardType.Creature || CardType == Card.ECardType.Structure")]
    [Range(1, 20)] public int MaxHealth = 5;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Mage")]
    [Header("0=Enchanted, 1=Mystic, 2=Arcane, 3=Quest")]
    public int[] HealthCards = new int[4];

    [Space]
    public Sprite IllustrationSprite;
    [ShowIf("@CardType == Card.ECardType.Fate")]
    public Sprite SecondarySprite;

    [Space]
    [TextArea(3, 10)]
    public string Description;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Challenge")]
    [TextArea(1, 10)]
    public string Reward;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Challenge")]
    [TextArea(1, 10)]
    public string Consequence;

    [Space]
    [ShowIf("@CardType == Card.ECardType.Level || CardType == Card.ECardType.Creature || CardType == Card.ECardType.Structure || CardType == Card.ECardType.Resource")]
    [TextArea(1, 10)]
    public string InitialEffect;
    [Space]
    [ShowIf("@CardType == Card.ECardType.Level || CardType == Card.ECardType.Creature || CardType == Card.ECardType.Structure ||  CardType == Card.ECardType.Resource")]
    [TextArea(1, 10)]
    public string PermanentEffect;

    public HorizontalAlignmentOptions DescriptionAlignment = HorizontalAlignmentOptions.Left;
    public bool ShouldGenerateDescription = true;
    [Range(0, 100)] public int DescriptionFontSize = 0;
    [Space]
    [HideIf("CardType", ECardType.Mage)]
    public List<Sprite> Resources;
    [HideIf("@CardType == Card.ECardType.Fate")]
    public List<Sprite> Requirements;
    [ShowIf("@CardType == Card.ECardType.Mage || CardType == Card.ECardType.Item || CardType == Card.ECardType.Creature || CardType == Card.ECardType.Structure || CardType == Card.ECardType.Level")]
    public Sprite PermanentResource;

    [Header("1=Mage, 2=Fate, 3=Enchanted, 4=Mystic, 5=Arcane, 6=Quest")]
    public int[] DeckCounts = new int[(int)EDeckType.Count];//do not set from code, use SetCountInDeck() or AddCountInDeck()

    //Mage Area
    [Header("Mage Data")]
    [ShowIf("CardType", ECardType.Mage)]
    [Range(1, 10)] public int DrawCardsPerTurn = 1;

    [ShowIf("CardType", ECardType.Mage)]
    [Range(5, 20)] public int MaxCardsInHand = 10;

    [ShowIf("CardType", ECardType.Mage)]
    [Range(0, 5)] public int MaxArmor = 1;

    [ShowIf("CardType", ECardType.Mage)]
    [Range(0, 5)] public int MaxPets = 0;

    [ShowIf("CardType", ECardType.Mage)]
    [Range(0, 5)] public int MaxPlants = 0;

    [ShowIf("CardType", ECardType.Mage)]
    [Range(0, 5)] public int MaxItems = 0;

    [ShowIf("CardType", ECardType.Mage), ReadOnly]
    public int Sum = 0;

    [ShowIf("CardType", ECardType.Mage)]
    [TextArea(3, 10)] public string WinCondition;

    private void Awake()
    {
        if(string.IsNullOrWhiteSpace(ConfigID))
            ConfigID = $"CardID_{Random.Range(0, 999999)}_{System.DateTime.Now.ToLongTimeString()}";
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            RefreshCount();
    }

    private void RefreshCount()
    {
        this.Count = 0;

        for (int i = 1; i < this.DeckCounts.Length; i++)
        {
            if (this.DeckCounts[i] > 0)
                this.Count += this.DeckCounts[i];
        }

        var maxHealth = 0;
        for (int i = 0; i < this.HealthCards.Length; i++)
        {
            if (this.HealthCards[i] > 0)
                maxHealth += this.HealthCards[i];
        }

        if (maxHealth > 0)
            this.MaxHealth = maxHealth;

        this.Sum = (this.DrawCardsPerTurn * 2) + this.MaxHealth + this.MaxCardsInHand + this.MaxArmor + this.MaxPets + this.MaxPlants + this.MaxItems;
    }

    public static CardData CreateCardData(CardConfig inConfig, int inIndex)
    {
        //convert to strings list for JSON serialization
        var resourceNames = new List<string>();
        foreach (var sprite in inConfig.Resources)
        {
            var spriteName = sprite != null ? sprite.name : string.Empty;
            resourceNames.Add(spriteName);
        }

        var requirementNames = new List<string>();
        foreach (var sprite in inConfig.Requirements)
        {
            var spriteName = sprite != null ? sprite.name : string.Empty;
            requirementNames.Add(spriteName);
        }

        //init attack numbers with one empty number
        var attackNums = new List<AttackNums>
        {
            new AttackNums
            {
                DeckType = EDeckType.Enchanted,
                AttackNumValues = new List<int>(1)
            },
            new AttackNums
            {
                DeckType = EDeckType.Mystic,
                AttackNumValues = new List<int>(1)
            },
            new AttackNums
            {
                DeckType = EDeckType.Arcane,
                AttackNumValues = new List<int>(1)
            },
            new AttackNums
            {
                DeckType = EDeckType.Quest,
                AttackNumValues = new List<int>(1)
            }
        };

        var cardData = new CardData
        {
            //ID = $"CardData_{Random.Range(0, 999999)}_{System.DateTime.Now.ToLongTimeString()}",
            ID = inConfig.ConfigID,
            Count = inConfig.Count,
            Index = inIndex,
            Name = inConfig.name,
            CardFrontPrefabName = inConfig.CardFrontPrefab != null ? inConfig.CardFrontPrefab.name : string.Empty,
            CardBackPrefabName = inConfig.CardBackPrefab != null ? inConfig.CardBackPrefab.name : string.Empty,
            CardName = inConfig.CardName,
            CardSubtitle = inConfig.CardSubtitle,
            CardType = inConfig.CardType,
            Alignment = inConfig.Alignment,
            Elements = inConfig.Elements,
            Level = inConfig.Level,
            AttackDeck = (EDeckType)inConfig.AttackDeck,
            DefenseDeck = (EDeckType)inConfig.DefenseDeck,
            MaxHealth = inConfig.MaxHealth,
            Description = inConfig.Description,
            DescriptionAlignment = inConfig.DescriptionAlignment,
            ShouldGenerateDescription = inConfig.ShouldGenerateDescription,
            DescriptionFontSize = inConfig.DescriptionFontSize,
            IllustrationSpriteName = inConfig.IllustrationSprite != null ? inConfig.IllustrationSprite.name : string.Empty,
            SecondarySpriteName = inConfig.SecondarySprite != null ? inConfig.SecondarySprite.name : string.Empty,
            ResourceNames = resourceNames,
            RequirementNames = requirementNames,
            PermanentResourceName = inConfig.PermanentResource != null ? inConfig.PermanentResource.name : string.Empty,
            DeckCounts = (int[])inConfig.DeckCounts.Clone(),
            AttackNumbersObjects = new List<AttackNums>(attackNums),

            //mage
            DrawCardsPerTurn = inConfig.DrawCardsPerTurn,
            MaxCardsInHand = inConfig.MaxCardsInHand,
            HealthCards = (int[])inConfig.HealthCards.Clone(),
            MaxArmor = inConfig.MaxArmor,
            MaxPets = inConfig.MaxPets,
            MaxPlants = inConfig.MaxPlants,
            MaxItems = inConfig.MaxItems,
            WinCondition = inConfig.WinCondition,

            //challenge
            Reward = inConfig.Reward,
            Consequence = inConfig.Consequence
        };

        return cardData;
    }

    public virtual void CopyFromConfig(CardConfig inConfig)
    {
        this.Count = inConfig.Count;

        this.CardFrontPrefab = inConfig.CardFrontPrefab;
        this.CardBackPrefab = inConfig.CardBackPrefab;

        this.CardName = inConfig.CardName;
        this.CardType = inConfig.CardType;
        this.Alignment = inConfig.Alignment;
        this.Elements = inConfig.Elements;
        this.Level = inConfig.Level;
        this.AttackDeck = inConfig.AttackDeck;
        this.DefenseDeck = inConfig.DefenseDeck;
        this.MaxHealth = inConfig.MaxHealth;
        this.Description = inConfig.Description;
        this.DescriptionAlignment = inConfig.DescriptionAlignment;
        this.DescriptionFontSize = inConfig.DescriptionFontSize;
        this.ShouldGenerateDescription = inConfig.ShouldGenerateDescription;
        this.IllustrationSprite = inConfig.IllustrationSprite;
        this.SecondarySprite = inConfig.SecondarySprite;
        this.Resources = inConfig.Resources;
        this.Requirements = inConfig.Requirements;
        this.PermanentResource = inConfig.PermanentResource;
        this.DeckCounts = inConfig.DeckCounts;

        //mage
        this.DrawCardsPerTurn = inConfig.DrawCardsPerTurn;
        this.MaxCardsInHand = inConfig.MaxCardsInHand;
        this.HealthCards = inConfig.HealthCards;
        this.MaxArmor = inConfig.MaxArmor;
        this.MaxPets = inConfig.MaxPets;
        this.MaxPlants = inConfig.MaxPlants;
        this.MaxItems = inConfig.MaxItems;
        this.WinCondition = inConfig.WinCondition;

        //challenge
        this.Reward = inConfig.Reward;
        this.Consequence = inConfig.Consequence;
    }

    public virtual void CopyFromCardData(CardData inCardData)
    {
        this.Count = inCardData.Count;

        //convert to strings list for JSON serialization
        var resourceSprites = new List<Sprite>();
        foreach (var spriteName in inCardData.ResourceNames)
        {
            var sprite = CardsManager.GetResourceSpriteFromName(spriteName);
            resourceSprites.Add(sprite);
        }

        var requirementSprites = new List<Sprite>();
        foreach (var spriteName in inCardData.RequirementNames)
        {
            var sprite = CardsManager.GetResourceSpriteFromName(spriteName);
            requirementSprites.Add(sprite);
        }

        Sprite illustrationSprite = null;

        illustrationSprite = CardsManager.GetSpriteFromName(inCardData.CardType, inCardData.IllustrationSpriteName);

        var secondarySprite = inCardData.CardType == ECardType.Fate ? CardsManager.GetSpriteFromName(inCardData.CardType, inCardData.SecondarySpriteName) : null;

        this.CardName = inCardData.CardName;
        this.CardType = inCardData.CardType;
        this.Alignment = inCardData.Alignment;
        this.Elements = inCardData.Elements;
        this.Level = inCardData.Level;
        this.AttackDeck = (EAttackDeckType)inCardData.AttackDeck;
        this.DefenseDeck = (EAttackDeckType)inCardData.DefenseDeck;
        this.MaxHealth = inCardData.MaxHealth;
        this.Description = inCardData.Description;
        this.DescriptionAlignment = inCardData.DescriptionAlignment;
        this.DescriptionFontSize = inCardData.DescriptionFontSize;
        this.IllustrationSprite = illustrationSprite;
        this.SecondarySprite = secondarySprite;
        this.Resources = resourceSprites;
        this.Requirements = requirementSprites;
        this.PermanentResource = CardsManager.GetResourceSpriteFromName(inCardData.PermanentResourceName);
        this.DeckCounts = (int[])inCardData.DeckCounts.Clone();

        //mage
        this.DrawCardsPerTurn = inCardData.DrawCardsPerTurn;
        this.MaxCardsInHand = inCardData.MaxCardsInHand;
        this.HealthCards = inCardData.HealthCards;
        this.MaxArmor = inCardData.MaxArmor;
        this.MaxPets = inCardData.MaxPets;
        this.MaxPlants = inCardData.MaxPlants;
        this.MaxItems = inCardData.MaxItems;
        this.WinCondition = inCardData.WinCondition;

        //challenge
        this.Reward = inCardData.Reward;
        this.Consequence = inCardData.Consequence;
    }
}