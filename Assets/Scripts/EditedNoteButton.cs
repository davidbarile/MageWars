using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button)), RequireComponent(typeof(CanvasGroup))]
public class EditedNoteButton : MonoBehaviour
{
    public EEditNoteType EditNoteType => this.editNoteType;
    [SerializeField] private EEditNoteType editNoteType;
    [Space, SerializeField] private Image iconImage;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color commentsColor;

    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        SetVisibility(false);
    }

    public void HandleClick()
    {
        UIEditNotesPanel.IN.ShowNote(this);
    }

    public void HandleCardClick()
    {
        var packCardItem = GetComponentInParent<UIPackCardItem>();
        packCardItem.HandleClick();//shows card and sets UIPackEditor.SelectedCardData

        UIEditNotesPanel.IN.Show();
        UIEditNotesPanel.IN.HandleShowCardButtonPress();
    }

    public void HandleDeckClick()
    {
        foreach (var cardData in UIPackEditor.IN.EditingPack.CardDatas)
        {
            if (cardData.CardEditNotesDict != null && cardData.CardEditNotesDict.Count > 0)
            {
                UIPackEditor.SelectedCardData = cardData;
                break;
            }
        }

        UIEditNotesPanel.IN.Show();
        UIEditNotesPanel.IN.HandleShowAllButtonPress();
    }

    public void SetVisibility(bool inIsVisible)
    {
        this.canvasGroup.alpha = inIsVisible ? 1 : 0;
        this.canvasGroup.interactable = inIsVisible;
        this.canvasGroup.blocksRaycasts = inIsVisible;
    }

    public void SetIconColor(bool inHasComment)
    {
        this.iconImage.color = inHasComment ? this.commentsColor : this.normalColor;
    }
}