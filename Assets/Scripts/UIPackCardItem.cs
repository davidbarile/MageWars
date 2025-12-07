using TMPro;
using UnityEngine;

public class UIPackCardItem : MonoBehaviour
{
    [Range(0, 2f)][SerializeField] private float spawnCardScale = .5f;

    [SerializeField] private Transform cardParent;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject disabledOverlay;
    [SerializeField] private EditedNoteButton editedNoteButton;

    public Card Card { get; private set; }

    public int Count { get; private set; }

    public Card SpawnCard(CardData inCardData, int inIndex)
    {
        this.Card = CardsManager.SpawnCard(inCardData, this.cardParent, true);
        this.Card.Configure(inCardData);
        this.Card.SetFlipCornerVisible(false);
        this.Card.transform.localScale = new Vector3(this.spawnCardScale, this.spawnCardScale, 1);
        this.Card.transform.localPosition = Vector3.zero;
        this.Card.name = $"{Card.name}_{inIndex}";
        this.Card.CardFlip.SetAnimStateToFront();
        this.Card.Draggable.IsDraggable = false;

        UIEditNotesPanel.OnPlayerDataChanged += RefreshButtonState;

        RefreshButtonState();

        return Card;
    }

    private void OnDestroy()
    {
        UIEditNotesPanel.OnPlayerDataChanged -= RefreshButtonState;
    }

    public void SetCount(int inCount)
    {
        this.Count = inCount;
        this.countText.text = inCount.ToString();
        this.countText.transform.parent.gameObject.SetActive(inCount > 0);
        this.disabledOverlay.SetActive(inCount == 0);
    }

    public void HandleClick()
    {
        UIPackEditor.SelectedPackCardItem = this;
        UICardCloseUp.IN.Show(this.Card);
    }

    private void RefreshButtonState()
    {
        if (this.Card == null || this.Card.Data == null) return;

        var cardData = this.Card.Data;

        //check if any of the edit notes have a comment
        var hasComment = false;
        if (cardData.CardEditNotesDict != null)
        {
            foreach (var kvp in cardData.CardEditNotesDict)
            {
                hasComment = !string.IsNullOrEmpty(kvp.Value.Comment);
                if (hasComment) break;
            }
        }

        if (cardData.CardEditNotesDict != null && cardData.CardEditNotesDict.Count > 0)
        {
            this.editedNoteButton.SetVisibility(true);
            this.editedNoteButton.SetIconColor(hasComment);
        }
        else
        {
            this.editedNoteButton.SetVisibility(false);
        }
    }
}