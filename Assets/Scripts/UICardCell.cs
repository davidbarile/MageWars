using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using static UICardFlip;
using static UIPlayerBoard;

public class UICardCell : MonoBehaviour
{
    public enum ECellType
    {
        Deck,
        Discard,
        Hand,
        ElementZone
    }

    public ECellType CellType;

    [ReadOnly]
    public EPlayerType PlayerType;

    public int MaxCards = 1;
    public int MaxVisibleCardsInDeck;

    public Action<UICardCell> OnCardRemoved;

    [Space]
    [ReadOnly]
    public List<Card> Cards = new();
    [Space]
    public Transform CardHolder;
    public Transform AnimTarget;

    public Card TopCard
    {
        get
        {
            if (this.Cards.Count > 0)
            {
                int lastIndex = this.Cards.Count - 1;
                return this.Cards[lastIndex];
            }

            return null;
        }
    }

    private void OnValidate()
    {
        if (this.CardHolder == null)
            this.CardHolder = this.transform;
    }

    public void RefreshCellsList()
    {
        this.Cards.Clear();

        var childCards = this.CardHolder.GetComponentsInChildren<Card>(true);

        foreach (var card in childCards)
        {
            this.Cards.Add(card);
        }

        RefreshVisibleCardCount();
    }

    //public void SetCardsDraggable(bool inIsDraggable)
    //{
    //    foreach (var card in this.Cards)
    //    {
    //        card.Draggable.IsDraggable = inIsDraggable;
    //    }
    //}

    public void RefreshVisibleCardCount()
    {
        for (int i = 0; i < this.Cards.Count; i++)
        {
            var card = this.Cards[i];

            bool shouldShow = this.MaxVisibleCardsInDeck == 0 || i >= this.Cards.Count - this.MaxVisibleCardsInDeck;
            card.gameObject.SetActive(shouldShow);
        }
    }

    public void ReturnCardToCell(string inSource = null)
    {
        //if(inSource != null) Debug.Log(inSource);

        var target = this.AnimTarget != null ? this.AnimTarget : this.CardHolder;

        this.TopCard.transform.DOMove(target.position, .2f).OnComplete(() =>
        {
            SetParentCell(this.TopCard);
        });
    }

    public void HandleDrop(UICardCell inSourceCell)
    {
        TryAddToCell(inSourceCell);
    }

    public void TryAddToCell(UICardCell inSourceCell, bool inIsCalledByHand = false)
    {
        var dropCellPermitted = this.Cards.Count < this.MaxCards || inIsCalledByHand;

        var isOwnHand = this.PlayerType == inSourceCell.PlayerType;
        var isFromDeck = inSourceCell.CellType == ECellType.Deck;

        if (dropCellPermitted && this.CellType == ECellType.Hand)
        {
            var hand = this.GetComponentInParent<UIHand>();
            var openCell = hand.GetFirstOpenCell();
        }


        if (inSourceCell == this)
        {
            dropCellPermitted = false;
        }

        if (dropCellPermitted)
        {
            var sourceTopCard = inSourceCell.TopCard;

            this.Cards.Add(sourceTopCard);
            SetParentCell(sourceTopCard);

            inSourceCell.Cards.Remove(sourceTopCard);
            inSourceCell.RefreshVisibleCardCount();

            RefreshVisibleCardCount();

            if (inSourceCell.CellType == ECellType.Deck)
                sourceTopCard.CardFlip.SetAnimState(ECardFlipState.FlipToFront);

            if (!inIsCalledByHand)
                inSourceCell.OnCardRemoved?.Invoke(inSourceCell);

            if (inSourceCell.CellType == ECellType.Deck)
            {
                CardsManager.IN.AddBatchOfCardsToGameDeck();
            }
        }
    }

    public void AddCardToCell(Card inCard)
    {
        this.Cards.Add(inCard);
        SetParentCell(inCard);
        //RefreshVisibleCardCount();
        inCard.CardFlip.SetAnimState(ECardFlipState.Front);
    }

    public static void DeleteCard(UICardCell inSourceCell)
    {
        var sourceTopCard = inSourceCell.TopCard;
        inSourceCell.Cards.Remove(sourceTopCard);

        inSourceCell.OnCardRemoved?.Invoke(inSourceCell);
    }

    public void SetParentCell(Card inCard, bool shouldRefreshGrid = true)
    {
        if (shouldRefreshGrid)
            RefreshVisibleCardCount();

        inCard.transform.SetParent(this.CardHolder);
        inCard.transform.localPosition = Vector3.zero;
        inCard.transform.localScale = Vector3.one;
        inCard.transform.localRotation = Quaternion.identity;

        if (this.CellType == ECellType.Hand)
        {
            var grid = this.GetComponentInParent<HorizontalLayoutGroup>();

            if (grid != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(grid.GetComponent<RectTransform>());
        }
    }

    public static UICardCell GetCellAtPosition(Vector3 inPosition)
    {
        var results = UIDraggable.GameObjectAtPosition(inPosition);

        foreach (var result in results)
        {
            var cell = result.gameObject.GetComponent<UICardCell>();

            if (cell != null)
                return cell;
        }

        return null;
    }
}