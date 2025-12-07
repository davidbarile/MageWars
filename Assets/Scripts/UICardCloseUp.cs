using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICardCloseUp : UIPanelBase
{
    public static UICardCloseUp IN;

    [SerializeField] private RectTransform overlay;
     [SerializeField] private RectTransform packModeCardParent;
    [SerializeField] private UICardEditPanel editPanel;

    [SerializeField] private GameObject[] scrollButtons;
    [SerializeField] private ResetScrollRectOnEnable resetScrollRectOnEnable;

    public Card ClickedCard {get; private set;}

    private Card card;

    private float doubleClickTolerance = .3f;
    private float doubleClickStartTime = -1;

    public void Show(Card inCard)
    {
        if (this.IsShowing) return;

        base.Show();

        LayoutRebuilder.ForceRebuildLayoutImmediate(this.overlay);

        this.ClickedCard = inCard;

        UISpriteSelectPanel.IN.Hide();

        var cardParent = UIPackEditor.IsPackEditMode ? this.packModeCardParent : this.overlay;

        this.card = CardsManager.SpawnCard(inCard.Data, cardParent, true);
        this.card.name = $"{card.name}";

        if (inCard.CardFlip.CardFlipState == UICardFlip.ECardFlipState.Back)
            StartCoroutine(DelayedCardFlipCo());

        UIPackEditor.SelectedCardData = this.card.Data;

        var cardRT = this.card.GetComponent<RectTransform>();
        float screenH = cardParent.rect.height - 40;
        var scale = (screenH / cardRT.rect.height) / this.card.CardFrontParent.localScale.x;
        this.card.transform.localScale = Vector3.one * scale;

        this.card.Configure(inCard.Data);
        if (inCard.ParentDeck != null)
            this.card.SpawnCardBack(inCard.ParentDeck.DeckType);

        this.card.Draggable.enabled = false;
        this.card.CardFlip.SetAnimStateToFront();

        this.card.transform.localPosition = Vector3.zero;

        if (UIPackEditor.IsPackEditMode)
            this.editPanel.Show(this.card);
        else
            this.editPanel.SetVisible(false);

        foreach (var button in this.scrollButtons)
        {
            button.SetActive(UIPackEditor.IsPackEditMode);
        }
    }

    private IEnumerator DelayedCardFlipCo()
    {
        yield return null;

        this.card.CardFlip.SetAnimStateToBack();
        this.card.SetDontFlipCornerVisible(false);
    }

    public override void Hide()
    {
        if (this.card != null)
        {
            Destroy(this.card.gameObject);
            this.card = null;
        }

        UIPackEditor.SelectedPackCardItem = null;

        UITextIconPanel.IN.Hide();

        base.Hide();
    }

    public void HandleBgClick()
    {
        if (!UIPackEditor.IsPackEditMode)
        {
            Hide();
            return;
        }

        float elapsedTimeSinceLastClick = Time.time - this.doubleClickStartTime;

        if (this.doubleClickStartTime > -1 && elapsedTimeSinceLastClick < this.doubleClickTolerance)
        {
            this.doubleClickStartTime = -1;
            Hide();
        }
        else
        {
            this.doubleClickStartTime = Time.time;
            StartCoroutine(ResetDoubleClickTimer());
        }
    }

    private IEnumerator ResetDoubleClickTimer()
    {
        yield return new WaitForSeconds(this.doubleClickTolerance);
        this.doubleClickStartTime = -1;
    }

    public void ScrollToBottom()
    {
        this.resetScrollRectOnEnable.ScrollToPosition(0);
    }
}