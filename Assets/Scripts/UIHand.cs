using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UICardCell;
using static UIPlayerBoard;

public class UIHand : MonoBehaviour
{
    [ReadOnly]
    public List<UICardCell> CardCells;
    private List<UICardCell> orderedCardCells;

    [Space]
    [SerializeField] private TextMeshProUGUI promptText;
    public RectTransform HandGrid;

    [Space]
    [SerializeField] private int maxCardsInHand = 15;

    public int NumCardsInHand
    {
        get
        {
            int numCards = 0;
            foreach (var cell in this.CardCells)
            {
                if (cell.TopCard != null)
                    ++numCards;
            }

            return numCards;
        }
    }

    private void Awake()
    {
        RefreshCardCells();

        this.orderedCardCells = this.transform.GetComponentsInChildren<UICardCell>(true).ToList();

        foreach (var cell in this.CardCells)
        {
            cell.OnCardRemoved += HandleCardRemoved;
        }
    }

    private void OnDestroy()
    {
        foreach (var cell in this.CardCells)
        {
            cell.OnCardRemoved -= HandleCardRemoved;
        }
    }

    public void RefreshCardCells()
    {
        this.CardCells = this.transform.GetComponentsInChildren<UICardCell>(true).ToList();
    }

    public void ResetCardCellSiblingIndices()
    {
        for (int i = 0; i < this.orderedCardCells.Count; i++)
        {
            this.orderedCardCells[i].transform.SetSiblingIndex(i);
        }

        RefreshCardCells();
    }

    public void HandleCardRemoved(UICardCell inCardCell)
    {
        RefreshCardCells();
    }

    public bool HandleDrop(UICardCell inSourceCell)
    {
        if (inSourceCell.CellType != ECellType.Hand)
        {
            int numCells = Mathf.Min(this.CardCells.Count, this.maxCardsInHand);

            for (int i = 0; i < numCells; i++)
            {
                var cell = this.CardCells[i];

                if (!cell.gameObject.activeSelf)
                {
                    if (cell.Cards.Count > 0)
                    {
                        //Debug.Log($"<color=red>UIHand.HandleDrop()   i = {i}   cell = {cell.name} is invisible but card count is {cell.Cards.Count}   cell.Cards[0] = {cell.Cards[0]}  </color>", cell.gameObject);
                        cell.Cards.Clear();
                    }

                    cell.gameObject.SetActive(true);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(this.HandGrid);

                    cell.TryAddToCell(inSourceCell, true);
                    return true;
                }
            }
        }

        inSourceCell.ReturnCardToCell(string.Empty);
        return false;
    }

    public UICardCell GetFirstOpenCell()
    {
        int numCells = Mathf.Min(this.CardCells.Count, this.maxCardsInHand);

        for (int i = 0; i < numCells; i++)
        {
            var cell = this.CardCells[i];

            if (!cell.gameObject.activeSelf)
            {
                if (cell.Cards.Count > 0)
                {
                    //Debug.Log($"<color=red>UIHand.GetFirstOpenCell()   i = {i}   cell = {cell.name} is invisible but card count is {cell.Cards.Count}   cell.Cards[0] = {cell.Cards[0]}  </color>", cell.gameObject);
                    cell.Cards.Clear();
                }

                return cell;
            }
        }

        return null;
    }

    public void InsertCell(UICardCell inSourceCell, UICardCell inTargetCell)
    {
        var insertedCell = GetFirstOpenCell();

        bool isOwnHand = inSourceCell.PlayerType == inTargetCell.PlayerType || inSourceCell.PlayerType == EPlayerType.None;

        if (insertedCell != null && isOwnHand)
        {
            //do not allow draw cards if no turns left

            int targetIndex = inTargetCell.transform.GetSiblingIndex();

            insertedCell.transform.SetSiblingIndex(targetIndex + 1);
            insertedCell.gameObject.SetActive(true);

            var sourceTopCard = inSourceCell.TopCard;
            var targetTopCard = inTargetCell.TopCard;

            insertedCell.Cards.Add(sourceTopCard);
            insertedCell.SetParentCell(sourceTopCard);

            inSourceCell.Cards.Remove(sourceTopCard);
            inSourceCell.OnCardRemoved?.Invoke(inSourceCell);

            RefreshCardCells();

        }
    }

    public static UIHand GetUIHandAtPosition(Vector3 inPosition)
    {
        var results = UIDraggable.GameObjectAtPosition(inPosition);

        foreach (var result in results)
        {
            var hand = result.gameObject.GetComponent<UIHand>();

            if (hand != null)
                return hand;
        }

        return null;
    }

    public List<Card> GetCardsInHand()
    {
        List<Card> cardsInHand = new();

        foreach (var cell in this.CardCells)
        {
            if (cell.TopCard != null)
            {
                cardsInHand.Add(cell.TopCard);
            }
        }

        return cardsInHand;
    }
}