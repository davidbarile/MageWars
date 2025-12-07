using System;
using Sirenix.OdinInspector;
using UnityEngine;
using static DeckConfig;

public class Card : MonoBehaviour
{
    public CardData Data;

    #region Enums
    public enum ECardType
    {
        None,
        Mage,
        Fate,
        Level,
        Challenge,
        Structure,
        Creature,
        Item,
        Magic,
        Resource,
        Curse,
        Blessing,
        Count
    }

    [Flags]
    public enum EElement
    {
        None = 0,
        Air = 1,
        Earth = 2,
        Fire = 4,
        Water = 8,
        Lightning = 16,
        Count = 32
    }
    #endregion

    [Space]
    public ECardType CardType;

    [Space]
    public UICardFlip CardFlip;
    public UIDraggable Draggable;

    [Space]
    [ReadOnly]
    public UICardFrontBase CardFront;
    public Transform CardFrontParent;
    [SerializeField] private GameObject flipCorner;
    [SerializeField] private GameObject dontFlipCorner;

    [Space]
    [ReadOnly]
    public UICardBackBase CardBack;
    public Transform CardBackParent;

    public UIDeck ParentDeck { get; set; }

    private string cardFrontPrefabName = string.Empty;
    private string cardBackPrefabName = string.Empty;

    public void Configure(CardData inCardData)
    {
        this.Data = inCardData;

        SetFlipCornerVisible(false);
        SetDontFlipCornerVisible(true);

        RefreshDisplay();

        UIPackEditor.OnCardDataChanged -= RefreshDisplay;
        UIPackEditor.OnCardDataChanged += RefreshDisplay;
    }

    public void SetFlipCornerVisible(bool inIsVisible)
    {
        if (this.flipCorner != null)
            this.flipCorner.SetActive(inIsVisible);
    }

    public void SetDontFlipCornerVisible(bool inIsVisible)
    {
        if (this.dontFlipCorner != null)
            this.dontFlipCorner.SetActive(inIsVisible);
    }

    public void RefreshDisplay()
    {
        var frontPrefab = CardsManager.CardPrefabLookupDict[this.Data.CardType].FrontPrefab;

        if (!this.cardFrontPrefabName.Equals(frontPrefab.name))
        {
            this.cardFrontPrefabName = frontPrefab.name;

            SpawnCardFront();
        }

        var backPrefab = CardsManager.CardPrefabLookupDict[this.Data.CardType].BackPrefab;

        if (!this.cardBackPrefabName.Equals(backPrefab.name))
        {
            this.cardBackPrefabName = backPrefab.name;

            SpawnCardBack();//not used anymore
        }

        this.CardFront.RefreshDisplay(this.Data);

        if (this.CardBack != null)
            this.CardBack.RefreshDisplay(this.Data);
    }

    private void SpawnCardFront()
    {
        this.CardFrontParent.DestroyAllChildren();

        var frontPrefab = CardsManager.CardPrefabLookupDict[this.Data.CardType].FrontPrefab;
        var cardFrontGo = Instantiate(frontPrefab.gameObject, this.CardFrontParent, false) as GameObject;

        cardFrontGo.name = $"Card_{this.Data.CardName}";
        cardFrontGo.transform.localScale = Vector3.one;
        cardFrontGo.transform.rotation = Quaternion.identity;
        cardFrontGo.transform.localPosition = Vector3.zero;

        this.CardFront = cardFrontGo.GetComponent<UICardFrontBase>();
    }

    private void SpawnCardBack()
    {
        // this.CardBackParent.DestroyAllChildren();

        // var backPrefab = CardsManager.CardPrefabLookupDict[this.Data.CardType].BackPrefab;
        // this.CardBack = Instantiate(backPrefab, this.CardBackParent, false);

        // this.CardBack.name = "Card Back";
        // this.CardBack.transform.localScale = Vector3.one;
        // this.CardBack.transform.rotation = Quaternion.identity;
    }

    //called manually now based on deck type
    public void SpawnCardBack(EDeckType inDeckType)
    {
        this.CardBackParent.DestroyAllChildren();

        var backPrefab = DeckManager.IN.DeckDatas[(int)inDeckType - 1].DeckConfig.CardBackPrefab;
        this.CardBack = Instantiate(backPrefab, this.CardBackParent, false);

        this.CardBack.name = "Card Back";
        this.CardBack.transform.localScale = Vector3.one;
        this.CardBack.transform.rotation = Quaternion.identity;

        this.CardBack.RefreshDisplay(this.Data);
    }

    private void OnDestroy()
    {
        UIPackEditor.OnCardDataChanged -= RefreshDisplay;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}