using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAttackPanel : UIPanelBase
{
    public static UIAttackPanel IN;
    [SerializeField] private UICardBackBase[] attackResults;
    [SerializeField] private UICardBackBase[] defenseResults;

    [SerializeField] private GameObject attackButtonsParent;
    [SerializeField] private GameObject defenseButtonsParent;

    [Space, SerializeField] private TextMeshProUGUI resutltText;
    [SerializeField] private GameObject resultObject;

    [Space, SerializeField, Range(0, 2)] float resultDelay = 1f;

    [Space, SerializeField] private Image buttonAttackNum;
    [SerializeField] private Image buttonDefenseNum;

    private int attackValue = 0;
    private int defenseValue = 0;

    private int numClickedItems = 0;

    private int resultValue => this.attackValue - this.defenseValue;

    public override void Show()
    {
        base.Show();

        this.numClickedItems = 0;

        this.attackValue = 0;
        this.defenseValue = 0;

        SetAttackResultActive(-1);
        SetDefenseResultActive(-1);

        this.resultObject.SetActive(false);
        this.resutltText.text = string.Empty;

        this.attackButtonsParent.SetActive(true);
        this.defenseButtonsParent.SetActive(true);
    }

    public void HandleAttackDeckChoice(UIDeck inDeck)
    {
        this.attackValue = inDeck.GetRandomAttackNumber();

        var deckIndex = UICardBacksPanel.GetDeckIndex(inDeck.DeckType);
        SetAttackResultActive(deckIndex);
    }

    public void HandleDefenseDeckChoice(UIDeck inDeck)
    {
        this.defenseValue = inDeck.GetRandomAttackNumber();

        var deckIndex = UICardBacksPanel.GetDeckIndex(inDeck.DeckType);
        SetDefenseResultActive(deckIndex);
    }

    public override void Hide()
    {
        base.Hide();
    }

    private void SetAttackResultActive(int inIndex)
    {
        for (int i = 0; i < this.attackResults.Length; i++)
        {
            this.attackResults[i].gameObject.SetActive(i == inIndex);
            this.attackResults[i].SetAttackNumber(this.attackValue);
        }

        this.attackButtonsParent.SetActive(inIndex == -1);

        if (inIndex == -1)
            return;

        ++this.numClickedItems;

        if (this.numClickedItems == 2)
        {
            Invoke(nameof(ShowResult), this.resultDelay);
        }
    }

    private void SetDefenseResultActive(int inIndex)
    {
        for (int i = 0; i < this.defenseResults.Length; i++)
        {
            this.defenseResults[i].gameObject.SetActive(i == inIndex);
            this.defenseResults[i].SetAttackNumber(this.defenseValue);
        }

        this.defenseButtonsParent.SetActive(inIndex == -1);

        if (inIndex == -1)
            return;

        ++this.numClickedItems;

        if (this.numClickedItems == 2)
        {
            Invoke(nameof(ShowResult), this.resultDelay);
        }
    }

    private void ShowResult()
    {
        this.resultObject.SetActive(true);

        var color = this.resultValue > 0 ? "#00FF00" : "#FF0000";
        var darkColor = this.resultValue > 0 ? "#00AA00" : "#AA0000";

        var equation = $"<color={darkColor}>{this.attackValue} - {this.defenseValue} = {this.resultValue}</color>";

        if (this.resultValue > 0)
        {
            this.resutltText.text = $"<size=110><color={color}>HIT!</color></size>\n{this.resultValue} Damage\n<size=70>{equation}</size>";
        }
        else
        {
            this.resutltText.text = $"<size=110><color={color}>MISS!</color></size>\n<size=70>{equation}</size>";
        }

        this.buttonAttackNum.sprite = CardsManager.IN.GetAttackNumberSprite(this.attackValue);
        this.buttonDefenseNum.sprite = CardsManager.IN.GetAttackNumberSprite(this.defenseValue);

        this.buttonAttackNum.color = this.resultValue > 0 ? Color.green : Color.red;
        this.buttonDefenseNum.color = this.resultValue > 0 ? Color.green : Color.red;
    }
}