using UnityEngine;
using TMPro;
using static Card;

public class UICardTypeWidget : MonoBehaviour
{
    public ECardType CardType { get; private set; }
    [SerializeField] private TextMeshProUGUI cardTypeText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject disabledOverlay;
    [SerializeField] private GameObject dimmedOverlay;

    private int count;

    public bool IsEnabled => !this.disabledOverlay.activeSelf;

    public void Init(int inIndex)
    {
        this.CardType = (ECardType)inIndex;

        this.cardTypeText.text = $"{this.CardType}";

        this.dimmedOverlay.SetActive(false);

        SetEnabled(true);
    }

    public void SetCount(int inCount)
    {
        this.count = inCount;
        this.countText.text = inCount.ToString();
        this.dimmedOverlay.SetActive(inCount == 0);
    }

    public void HandleClick()
    {
        SetEnabled(!this.IsEnabled);
        UIPackEditor.OnCardDataChanged?.Invoke();
    }

    public void SetEnabled(bool inIsEnabled)
    {
        this.disabledOverlay.SetActive(!inIsEnabled);
    }
}