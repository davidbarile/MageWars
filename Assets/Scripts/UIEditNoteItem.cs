using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIEditNoteItem : MonoBehaviour
{
    [SerializeField] private GameObject expandButton;
    [SerializeField] private GameObject collapseButton;
    [SerializeField] private GameObject expandedContent;
    [SerializeField] private GameObject markSeenButtonText;
    [SerializeField] private GameObject markUnseenButtonText;
    [SerializeField] private GameObject inputPopup;
    [SerializeField] private TMP_InputField commentInput;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private EditedNoteButton editedNoteButton;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI timeStampText;
    [SerializeField] private TextMeshProUGUI oldValueText;
    [SerializeField] private TextMeshProUGUI newValueText;
    [SerializeField] private GameObject longNewValuesParent;
    [SerializeField] private TextMeshProUGUI longOldValueText;
    [SerializeField] private TextMeshProUGUI longNewValueText;
    [SerializeField] private TextMeshProUGUI commentText;
    [SerializeField] private TextMeshProUGUI cardText;

    private EditNoteData editNoteData;
    private CardData cardData;

    private int longValueLength;

    private bool isExpanded;

    public void Configure(CardData inCardData, EditNoteData inEditNoteData)
    {
        this.editNoteData = inEditNoteData;
        this.cardData = inCardData;

        this.typeText.text = inEditNoteData.EditNoteType;
        this.timeStampText.text = inEditNoteData.TimeStamp;

        this.cardText.text = $"Card Name: {inCardData.Name} ({inCardData.CardType})";

        if (inEditNoteData.EditNoteType == EEditNoteType.PermanentResource.ToString())
        {
            this.typeText.text = "Perm Rsrc";

            this.oldValueText.text = $"<sprite name=\"{inEditNoteData.OldValue}\">";
            if (string.IsNullOrWhiteSpace(inEditNoteData.OldValue))
                this.oldValueText.text = "(none)";

            this.newValueText.text = $"<sprite name=\"{inEditNoteData.NewValue}\">";
            if (string.IsNullOrWhiteSpace(inEditNoteData.NewValue))
                this.newValueText.text = "(none)";

            this.longValueLength = 5;
        }
        else if (inEditNoteData.EditNoteType == EEditNoteType.Resources.ToString() || inEditNoteData.EditNoteType == EEditNoteType.Requirements.ToString())
        {
            var wrappedOldSpriteNames = WrapSpriteNames(inEditNoteData.OldListValue);
            var wrappedNewSpriteNames = WrapSpriteNames(inEditNoteData.NewListValue);
            this.oldValueText.text = string.Join("", wrappedOldSpriteNames);
            this.newValueText.text = string.Join("", wrappedNewSpriteNames);
            this.longOldValueText.text = string.Join("", wrappedOldSpriteNames);
            this.longNewValueText.text = string.Join("", wrappedNewSpriteNames);

            this.longValueLength = 5 * wrappedOldSpriteNames.Count;
        }
        else if (inEditNoteData.EditNoteType == EEditNoteType.Decks.ToString())
        {
            this.oldValueText.text = "Decks";
            this.newValueText.text = "Decks*";
            this.longOldValueText.text = string.Join("\n", inEditNoteData.OldListValue);
            this.longNewValueText.text = string.Join("\n", inEditNoteData.NewListValue);

            this.longValueLength = 100;
        }
        else
        {
            this.oldValueText.text = inEditNoteData.OldValue;
            this.newValueText.text = inEditNoteData.NewValue;
            this.longOldValueText.text = inEditNoteData.OldValue;
            this.longNewValueText.text = inEditNoteData.NewValue;

            this.longValueLength = Mathf.Max(inEditNoteData.OldValue.Length, inEditNoteData.NewValue.Length);
        }

        this.commentText.text = inEditNoteData.Comment;

        this.editedNoteButton.SetVisibility(!inEditNoteData.IsViewed);
        this.markSeenButtonText.SetActive(!this.editNoteData.IsViewed);
        this.markUnseenButtonText.SetActive(this.editNoteData.IsViewed);

        var hasComment = !string.IsNullOrEmpty(inEditNoteData.Comment);
        this.editedNoteButton.SetIconColor(hasComment);

        SetExpandedState(false);

        List<string> WrapSpriteNames(List<string> inSpriteNames)
        {
            var wrappedSpriteNames = new List<string>();
            foreach (var spriteName in inSpriteNames)
            {
                if (string.IsNullOrWhiteSpace(spriteName))
                {
                    wrappedSpriteNames.Add("(none)");
                    continue;
                }
                wrappedSpriteNames.Add($"<sprite name=\"{spriteName}\">");
            }

            return wrappedSpriteNames;
        }
    }

    public void HandleDeleteButtonPress()
    {
        if (UIPackEditor.SelectedCardData.CardEditNotesDict != null && UIPackEditor.SelectedCardData.CardEditNotesDict.ContainsKey(this.editNoteData.EditNoteType))
        {
            UIPackEditor.SelectedCardData.CardEditNotesDict.Remove(this.editNoteData.EditNoteType);

            if(UIPackEditor.SelectedCardData.CardEditNotesDict.Count == 0)
                UIPackEditor.SelectedCardData.CardEditNotesDict = null;

            PlayerData.Data.SetDirty();

            //fire event that hides button
            UIEditNotesPanel.OnPlayerDataChanged?.Invoke();

            Destroy(this.gameObject);
        }
    }

    public void HandleMarkSeenButtonPress()
    {
        this.editNoteData.IsViewed = !this.editNoteData.IsViewed;

        UIEditNotesPanel.OnPlayerDataChanged?.Invoke();
        PlayerData.Data.SetDirty();

        this.markSeenButtonText.SetActive(!this.editNoteData.IsViewed);
        this.markUnseenButtonText.SetActive(this.editNoteData.IsViewed);
        this.editedNoteButton.SetVisibility(!this.editNoteData.IsViewed);
    }

    public void HandleExpandButtonPress()
    {
        this.isExpanded = true;
        SetExpandedState(this.isExpanded);
    }

    public void HandleCollapseButtonPress()
    {
        this.isExpanded = false;
        SetExpandedState(this.isExpanded);
    }

    private void SetExpandedState(bool inIsExpanded)
    {
        var shouldShowLongValues = this.longValueLength > 50;
        this.longNewValuesParent.SetActive(inIsExpanded && shouldShowLongValues);
        var expandedContentHeight = shouldShowLongValues ? 950 : 450;
        this.layoutElement.minHeight = inIsExpanded ? expandedContentHeight : 80;

        this.expandButton.SetActive(!inIsExpanded);
        this.collapseButton.SetActive(inIsExpanded);
        this.expandedContent.SetActive(inIsExpanded);

        if (!inIsExpanded)
            this.inputPopup.SetActive(false);
    }

    public void HandleCreateCommentClick()
    {
        this.inputPopup.SetActive(true);
        this.commentInput.text = this.editNoteData.Comment;
        EventSystem.current.SetSelectedGameObject(this.commentInput.gameObject);
    }

    public void HandleSubmitCommentClick()
    {
        this.editNoteData.Comment = this.commentInput.text;
        this.commentText.text = this.commentInput.text;

        var hasComment = !string.IsNullOrEmpty(this.editNoteData.Comment);
        this.editedNoteButton.SetIconColor(hasComment);

        UIEditNotesPanel.OnPlayerDataChanged?.Invoke();
        PlayerData.Data.SetDirty();

        this.inputPopup.SetActive(false);
    }

    public void HandleCancelCommentClick()
    {
        this.inputPopup.SetActive(false);
    }
}