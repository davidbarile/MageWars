using UnityEngine;

public class UICardEditPanel : UIPanelBase
{
    [SerializeField] private UICustomCardEditor customCardEditor;

    [Space]
    [SerializeField] private UIDeckEditWidget[] deckEditWidgets;

    private Card selectedCard;
    private int editorCardDataIndex = -1;
    private bool shouldRefreshEditorCardData;

    public void Show(Card inCard)
    {  
#if UNITY_EDITOR
        this.shouldRefreshEditorCardData = false;
        UIPackEditor.OnCardDataChanged -= RefreshEditorCardData;

        var editorCardData = CardsManager.IN.AllCardDatas.Find(x => x.ID == inCard.Data.ID);

        if(editorCardData != null)
        {
            this.editorCardDataIndex = CardsManager.IN.AllCardDatas.IndexOf(editorCardData);
            print($"<color=#00FF00>Found {editorCardData.Name}   id = {editorCardData.ID}  index = {this.editorCardDataIndex}</color>");
            UIPackEditor.OnCardDataChanged += RefreshEditorCardData;
        }
        else
            print($"<color=red>Could Not Find  {inCard.Data.Name}    id = {inCard.Data.ID}</color>");
#endif

        this.selectedCard = inCard;

        if (this.customCardEditor == null)
            this.customCardEditor = this.GetComponent<UICustomCardEditor>();

        this.customCardEditor.Init(inCard);
        UIPackEditor.OnCardDataChanged += this.customCardEditor.RefreshDisplay;

        ConfigureDeckEditWidgets();

        base.Show();
    }

    private void RefreshEditorCardData()
    {
        CardsManager.IN.AllCardDatas[this.editorCardDataIndex] = UIPackEditor.SelectedCardData.Clone(UIPackEditor.SelectedCardData);

        //hack to prevent first show of card from setting it dirty
        if(!this.shouldRefreshEditorCardData)
        {
            this.shouldRefreshEditorCardData = true;
            return;
        }

        PlayerData.Data.SetCardDirty(CardsManager.IN.AllCardDatas[this.editorCardDataIndex]);
    }

    private void ConfigureDeckEditWidgets()
    {
        for (int i = 0; i < this.deckEditWidgets.Length; i++)
        {
            var widget = this.deckEditWidgets[i];
            var shouldShow = i < DeckManager.IN.DeckDatas.Length;
            widget.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                var deckData = DeckManager.IN.DeckDatas[i];
                var numCardsInDeck = UIPackEditor.SelectedCardData.DeckCounts[(int)deckData.DeckConfig.DeckType];
                widget.gameObject.SetActive(numCardsInDeck > -1);

                if (numCardsInDeck > -1)
                {
                    widget.Init(i, true);
                    widget.SetCount(numCardsInDeck);
                }
            }
        }
    }

    public override void Hide()
    {
        UIPackEditor.OnCardDataChanged -= this.customCardEditor.RefreshDisplay;
        UIPackEditor.OnCardDataChanged -= RefreshEditorCardData;

        base.Hide();
    }
}