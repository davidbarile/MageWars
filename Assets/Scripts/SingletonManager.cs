using UnityEngine;

public class SingletonManager : MonoBehaviour
{
    //[SerializeField] private ScreenShakeController screenShakeController;
    [SerializeField] private UIGameBoard gameBoard;
    [SerializeField] private UIConfirmPanel confirmPanel;
    [SerializeField] private UIDragOverlay dragOverlay;
    [SerializeField] private UIPackEditor packEditor;
    [SerializeField] private UICardCloseUp cardCloseUp;
    [SerializeField] private UISpriteSelectPanel spriteSelectPanel;
    [SerializeField] private UIPackListPanel packListPanel;
    [SerializeField] private UICardBacksPanel cardBacksPanel;
    [SerializeField] private UIAttackPanel attackPanel;
    [SerializeField] private UITextIconPanel textIconPanel;
    [SerializeField] private UIFeedbackToastPanel feedbackToastPanel;
    [SerializeField] private UIEditNotesPanel editNotesPanel;

    public void Init()
    {
        GameManager.IN = this.GetComponent<GameManager>();
        DeckManager.IN = this.GetComponent<DeckManager>();
        CardsManager.IN = this.GetComponent<CardsManager>();
        SaveManager.IN = this.GetComponent<SaveManager>();
        LocalizationManager.IN = this.GetComponent<LocalizationManager>();

        UIGameBoard.IN = this.gameBoard;
        UIDragOverlay.IN = this.dragOverlay;

        UIConfirmPanel.IN = this.confirmPanel;
        UIConfirmPanel.IN.SetVisible(false);

        UICardCloseUp.IN = this.cardCloseUp;
        UICardCloseUp.IN.SetVisible(false);

        UISpriteSelectPanel.IN = this.spriteSelectPanel;
        UISpriteSelectPanel.IN.SetVisible(false);
        UISpriteSelectPanel.IN.CreateElementSpriteDict();

        UIPackListPanel.IN = this.packListPanel;
        UIPackListPanel.IN.SetVisible(false);

        UITextIconPanel.IN = this.textIconPanel;
        UITextIconPanel.IN.SetVisible(false);

        UIPackEditor.IN = this.packEditor;
        UIPackEditor.IN.SetVisible(false);

        UICardBacksPanel.IN = this.cardBacksPanel;
        UICardBacksPanel.IN.SetVisible(false);

        UIAttackPanel.IN = this.attackPanel;
        UIAttackPanel.IN.SetVisible(false);

        UIFeedbackToastPanel.IN = this.feedbackToastPanel;
        UIFeedbackToastPanel.IN.SetVisible(false);

        UIEditNotesPanel.IN = this.editNotesPanel;
        UIEditNotesPanel.IN.SetVisible(false);
    }
}