using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardBackBase : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject levelDisplay;

    [SerializeField] private Image attackNumberImage;
    [SerializeField] private Image attackIconImage;

    public int AttackNumber {get; private set;}

    /// <summary>
    /// Override this with specificy UI display code depending on prefab
    /// </summary>
    public virtual void RefreshDisplay(CardData inData)
    {
        SetLevel(inData.Level);

        if(UICardCloseUp.IN.ClickedCard == null || UICardCloseUp.IN.ClickedCard.CardBack == null) return;
        
        SetAttackNumber(UICardCloseUp.IN.ClickedCard.CardBack.AttackNumber);
    }

    private void SetLevel(int inLevel)
    {
        if (this.levelText == null) return;

        this.levelText.text = inLevel.ToString();

        this.levelDisplay.SetActive(inLevel > 0);
    }

    public void SetAttackNumber(int inNumber)
    {
        if (this.attackNumberImage != null)
            this.attackNumberImage.sprite = CardsManager.IN.GetAttackNumberSprite(inNumber);

        if (this.attackIconImage != null)
            this.attackNumberImage.gameObject.SetActive(inNumber > 0);

        this.AttackNumber = inNumber;
    }
}