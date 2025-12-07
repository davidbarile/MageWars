public class UIGameBoard : UIPanelBase
{
    public static UIGameBoard IN;

    public void ResetAllCardsToRespectiveDecks()
    {
        var cards = this.transform.GetComponentsInChildren<Card>(true);

        if (cards.Length == 0) return;

        UIConfirmPanel.IN.Show("Reset Board?", "This will reset all cards to their source decks.\nAre you sure you want to?", ResetAllCards);
    }

    public void ResetAllCards()
    {
        var cards = this.transform.GetComponentsInChildren<Card>(true);

        foreach (var card in cards)
        {
            card.ParentDeck?.AddCardToDeck(card);
        }
    }
}